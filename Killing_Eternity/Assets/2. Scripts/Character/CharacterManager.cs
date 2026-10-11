using ECM2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KE
{
    public class CharacterManager : MonoBehaviour
    {
        [HideInInspector] public Character characterController;
        [HideInInspector] public Animator animator;
        [HideInInspector] public CharacterCombatManager characterCombatManager;
        [HideInInspector] public CharacterMovementManager characterMovementManager;
        [HideInInspector] public CharacterAnimationManager characterAnimationManager;
        [HideInInspector] public CharacterStatsManager characterStatsManager;
        [HideInInspector] public CharacterInventoryManager characterInventoryManager;
        [HideInInspector] public CharacterEquipmentManager characterEquipmentManager;
        

        [Header("Character Name")]
        public ObservableVariable<string> characterName = new ObservableVariable<string>("");


        [Header("Character Flags")]
        public ObservableVariable<bool> isDead = new ObservableVariable<bool>();
        public bool canMove = true;
        public bool canRotate = true;
        public bool isPerformingAction;

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

    }
}

