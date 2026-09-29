using ECM2;
using UnityEngine;


namespace KE
{
    public class PlayerManager : CharacterManager
    {
        [HideInInspector] public PlayerMovementManager playerMovementManager;
        [HideInInspector] public PlayerAnimationManager playerAnimationManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;

        [Header("Player Flags")]
        public bool canMove = true;
        public bool canRotate = true;
        public bool isDead = false;
        public bool isPerformingAction;

        protected override void Awake()
        {
            base.Awake();

            DontDestroyOnLoad(gameObject);

            playerMovementManager   = GetComponent<PlayerMovementManager>();
            playerAnimationManager  = GetComponent<PlayerAnimationManager>();
            playerStatsManager      = GetComponent<PlayerStatsManager>();
        }

        protected override void Update()
        {
            base.Update();
        }


        public void OnIsMovingChanged(bool oldStatus, bool newStatus)
        {
            animator.SetBool("IsMoving", playerMovementManager.isMoving.GetBool());
        }
    }
}
