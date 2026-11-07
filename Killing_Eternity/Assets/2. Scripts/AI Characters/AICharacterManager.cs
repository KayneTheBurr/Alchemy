using UnityEngine;
using UnityEngine.AI;
namespace KE
{
    public class AICharacterManager : CharacterManager
    {
        [HideInInspector] public AICombatManager aiCombatManager;
        [HideInInspector] public AIMovementManager aiMovementManager;
        [HideInInspector] public AIAnimationManager aiAnimationManager;
        [HideInInspector] public AIStatsManager aiStatsManager;
        [HideInInspector] public AIInventoryManager aiInventoryManager;
        [HideInInspector] public AIEquipmentManager aiEquipmentManager;
        public BaseAIManager aiManager;

        [Header("AI Navigation")]
        public NavMeshAgent navMeshAgent;

        protected override void Awake()
        {
            base.Awake();
            navMeshAgent = GetComponent<NavMeshAgent>();

            aiCombatManager = GetComponent<AICombatManager>();
            aiMovementManager = GetComponent<AIMovementManager>();
            aiAnimationManager = GetComponent<AIAnimationManager>();
            aiStatsManager = GetComponent<AIStatsManager>();
            aiInventoryManager = GetComponent<AIInventoryManager>();
            aiEquipmentManager = GetComponent<AIEquipmentManager>();
            aiManager = GetComponent<BaseAIManager>();

            //disable ecm2 so it doesnt fight navmesh
            if (characterController != null)
            {
                characterController.enabled = false;
            }
        }

        public override void OnIsDeadChanged(bool oldStatus, bool newStatus)
        {
            base.OnIsDeadChanged(oldStatus, newStatus);
            if (newStatus == true)
            {
                navMeshAgent.enabled = false;

                //trigger loot, exp, death anim here
                Destroy(gameObject);
            }
        }
        public override void TakeDamage(float damageValue, Element element, IDamageable damageSource = null, bool isDoT = false)
        {
            base.TakeDamage(damageValue, element);

            float rawResistance = characterStatsManager.GetResistance(element);
            float effectiveResistance = Mathf.Min(rawResistance, 100f);

            float damageMultiplier = 1f - (effectiveResistance / 100f);
            float finalDamage = Mathf.Max(damageValue * damageMultiplier, 0f);

            characterStatsManager.currentHealth.Value -= finalDamage;

            if (characterStatsManager.currentHealth.Value <= 0)
            {
                isDead.Value = true;
                if (navMeshAgent != null) navMeshAgent.enabled = false;
                //cant set a state since its looking for the base manager and current state is only on normal ai manager
            }
            else
            {
                if (aiManager != null && aiCombatManager.currentTarget == null && damageSource != null)
                {
                    if (WorldUtilityManager.instance.CanIDamageThisTarget(MyFaction, damageSource.MyFaction))
                    {
                        aiManager.AlertToTarget(damageSource);
                        aiManager.ShoutToAllies(damageSource);
                    }
                }
            }
        }
    }
}
