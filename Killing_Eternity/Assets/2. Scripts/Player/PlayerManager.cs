using ECM2;
using UnityEngine;


namespace KE
{
    public class PlayerManager : CharacterManager
    {
        [HideInInspector] public PlayerMovementManager playerMovementManager;
        [HideInInspector] public PlayerAnimationManager playerAnimationManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;
        [HideInInspector] public PlayerCombatManager playerCombatManager;
        [HideInInspector] public PlayerInventoryManager playerInventoryManager;
        [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;

        //[Header("Player Flags")]


        protected override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(gameObject);

            playerMovementManager   = GetComponent<PlayerMovementManager>();
            playerAnimationManager  = GetComponent<PlayerAnimationManager>();
            playerStatsManager      = GetComponent<PlayerStatsManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        }

        protected override void Update()
        {
            base.Update();
        }

        public override void OnIsDeadChanged(bool oldStatus, bool newStatus)
        {
            base.OnIsDeadChanged(oldStatus, newStatus);
        }
        public override void OnIsMovingChanged(bool oldStatus, bool newStatus)
        {
            base.OnIsDeadChanged(oldStatus, newStatus);
        }
    }
}
