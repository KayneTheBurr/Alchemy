using ECM2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KE
{
    public class CharacterManager : MonoBehaviour, IDamageable
    {
        [HideInInspector] public Character characterController;
        [HideInInspector] public Animator animator;
        [HideInInspector] public CharacterCombatManager characterCombatManager;
        [HideInInspector] public CharacterMovementManager characterMovementManager;
        [HideInInspector] public CharacterAnimationManager characterAnimationManager;
        [HideInInspector] public CharacterStatsManager characterStatsManager;
        [HideInInspector] public CharacterInventoryManager characterInventoryManager;
        [HideInInspector] public CharacterEquipmentManager characterEquipmentManager;
        [HideInInspector] public StatusManager statusManager;

        [Header("Character Level")]
        public ObservableVariable<int> characterLevel = new ObservableVariable<int>(1);

        [Header("Character Name")]
        public ObservableVariable<string> characterName = new ObservableVariable<string>("");

        [Header("Factions")]
        public Faction faction = Faction.Neutral;
        public Faction MyFaction => faction;


        [Header("Character Flags")]
        public ObservableVariable<bool> isDead = new ObservableVariable<bool>();
        public bool IsDead() => isDead.Value;

        public bool canMove = true;
        public bool canRotate = true;
        public bool applyRootMotion = false;
        public bool isPerformingAction;
        public ObservableVariable<bool> isInvincible = new ObservableVariable<bool>();

        public SkinnedMeshRenderer mainMesh;

        #region Core Functions
        protected virtual void Awake()
        {
            characterController = GetComponent<Character>();
            animator = GetComponentInChildren<Animator>();

            characterCombatManager = GetComponent<CharacterCombatManager>();
            characterMovementManager = GetComponent<CharacterMovementManager>();
            characterAnimationManager = GetComponent<CharacterAnimationManager>();
            characterStatsManager = GetComponent<CharacterStatsManager>();
            characterInventoryManager = GetComponent<CharacterInventoryManager>();
            characterEquipmentManager = GetComponent<CharacterEquipmentManager>();
            statusManager = GetComponent<StatusManager>();
        }
        protected virtual void Start()
        {

        }
        protected virtual void OnEnable()
        {
            isDead.OnValueChanged += OnIsDeadChanged;
        }
        protected virtual void OnDisable()
        {
            isDead.OnValueChanged -= OnIsDeadChanged;
        }
        protected virtual void Update()
        {

        }
        protected virtual void LateUpdate()
        {

        }
        protected virtual void FixedUpdate()
        {

        }
        #endregion

        // Listener Functions
        public virtual void OnIsDeadChanged(bool oldStatus, bool newStatus)
        {

        }
        public virtual void OnIsMovingChanged(bool oldStatus, bool newStatus)
        {
            animator.SetBool("IsMoving", characterMovementManager.isMoving);
        }

        public virtual IEnumerator HandleDeathEvents(bool manuallySelectDeathAnim = false)
        {
            
            yield return null;
        }

        public virtual void TakeDamage(float damageValue, Element element, IDamageable damageSource = null, bool isDoT = false)
        {
            if (isDead.Value) return;

            // EVASION
            if (!isDoT)
            {
                // dont allow dodging a DOT effect
                int attackerLevel = 1; // TODO: Pull from damageSource later
                float evasionChance = characterStatsManager.CalculateEvasionChance(attackerLevel);

                // dodged if true!
                if (characterStatsManager.CheckEvaded(evasionChance)) return;
            }

            // RESISTANCE CALCULATION
            float rawResistance = characterStatsManager.GetResistance(element);
            float maxRes = characterStatsManager.GetMaxResistance(element);

            float effectiveResistance = Mathf.Min(rawResistance, maxRes);
            float damageMultiplier = 1f - (effectiveResistance / 100f);
            float damageTakenMultiplier = characterStatsManager.GetElementalDamageTaken(element);

            float finalDamage = Mathf.Max(damageValue * damageMultiplier * damageTakenMultiplier, 0f);

            // ARMOUR
            if (!isDoT)
            {
                float armorMitigation = characterStatsManager.CalculateArmorMitigation(finalDamage, element);
                finalDamage -= (finalDamage * armorMitigation);
            }


            // APPLY DAMAGE TO SHIELD FIRST
            float remainingDamage = finalDamage;

            if (characterStatsManager.currentShield.Value > 0)
            {
                float shieldDamageMultiplier = 1f;
                if (element == Element.Bile)
                {
                    shieldDamageMultiplier = AlchemyEngine.instance.tuningData.bileShieldDamageMultiplier;
                }

                float effectiveDamageToShield = remainingDamage * shieldDamageMultiplier;
                if (effectiveDamageToShield > characterStatsManager.currentShield.Value)
                {
                    float excessShieldDamage = effectiveDamageToShield - characterStatsManager.currentShield.Value;
                    characterStatsManager.currentShield.Value = 0;

                    //divide ehte to correct for the bile damag emult to shields
                    remainingDamage = excessShieldDamage / shieldDamageMultiplier;
                }
                else
                {
                    characterStatsManager.currentShield.Value -= effectiveDamageToShield;
                    remainingDamage = 0;
                }
            }

            // APPLY REMAINING DAMAGE TO LIFE
            if (remainingDamage > 0)
            {
                characterStatsManager.currentHealth.Value -= remainingDamage;
            }

            if (damageSource != null && damageSource.GetTransform().TryGetComponent(out CharacterStatsManager attackerStats))
            {
                attackerStats.CalculateResourceOnHit();
                attackerStats.CalculateResourceSteal(finalDamage);
            }

            characterStatsManager.AddRecoupInstance(finalDamage);

            // Determine outcome of damage recieved
            if (characterStatsManager.currentHealth.Value <= 0)
            {
                HandleDeath();
            }
            else
            {
                HandleHitReaction(damageSource);
            }
        }
        protected virtual void HandleDeath()
        {
            isDead.Value = true;
        }

        protected virtual void HandleHitReaction(IDamageable damageSource)
        {
            
        }
        public virtual Transform GetTransform()
        {
            return transform;
        }

        public virtual void ApplyBuildup(Element element, float buildupAmount, float dotAmount, IDamageable source = null)
        {
            statusManager.AddBuildup(element, buildupAmount, dotAmount, source);
        }

        public bool IsTargetValid(IDamageable target)
        {
            return target != null && (target as UnityEngine.Object) != null && !target.IsDead();
        }

    }
}

