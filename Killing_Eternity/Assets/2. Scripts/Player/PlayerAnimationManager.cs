using System;
using UnityEngine;



namespace KE
{
    public class PlayerAnimationManager : CharacterAnimationManager
    {
        PlayerManager player;
        int vertical;
        int horizontal;
        int moveSpeedMult;

        [Header("Animation Tolerance")]
        public float idleToWalkTolerance = 0.1f;

        [Header("Damping")]
        public float dampTime = 0.1f;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
            moveSpeedMult = Animator.StringToHash("MoveSpeedModifier");
        }

        public void UpdateAnimatorMovementParameters(Vector3 moveDirection, bool isSprinting)
        {
            float h = 0;
            float v = 0;

            if (moveDirection != Vector3.zero)
            {
                Vector3 localMove = transform.InverseTransformDirection(moveDirection);
                //localMove.Normalize();

                h = SnapAxis(localMove.x);
                v = SnapAxis(localMove.z);

                if (isSprinting)
                {
                    h *= 2f;
                    v *= 2f;
                }
            }

            //Pass values in to the animator to update correct anim to play 
            player.animator.SetFloat(horizontal, h, dampTime, Time.deltaTime);
            player.animator.SetFloat(vertical, v, dampTime, Time.deltaTime);

            //Increase Movement Animator State by same percent as our base movepseed amount
            float currentMoveSpeedMultiplier = player.playerStatsManager.moveSpeedMultiplier;
            player.animator.SetFloat(moveSpeedMult, currentMoveSpeedMultiplier);
        }
        private float SnapAxis(float value)
        {
            float abs = Mathf.Abs(value);
            if (abs < idleToWalkTolerance) return 0f;

            float sign = Mathf.Sign(value);

            if (abs <= 0.5f) return 0.5f * sign;

            return 1.0f * sign;
        }
    }
}
