using System.Collections;
using UnityEngine;

namespace KE
{
    public class PlayerMovementManager : CharacterMovementManager
    {
        #region Variables
        PlayerManager player;

        [Header("Movement Settings")]
        [SerializeField] Vector3 moveDirection;
        [SerializeField] Vector3 lookDirection;

        public float walkSpeed;
        public float runSpeed;
        public float sprintSpeed;
        [Tooltip(" Degrees per second")]
        public float rotationSpeed = 1200f;

        [Header("Dodge Settings")]
        public float dodgeCost = 100f;
        public float dodgeDuration = 0.25f;
        public float dodgeSpeed = 15f;
        public float dodgeIFrameDuration = 0.1f;
        #endregion

        #region Core Functions
        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
           
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            
        }
        protected override void Update()
        {
            base.Update();
            moveDirection = InputManager.instance.moveDirection;
            lookDirection = InputManager.instance.lookDirection;
            
            HandleMovement();
            HandleRotation();
        }
        #endregion

        #region Core Movement Functions
        private void HandleMovement()
        {
            if (player.isDead) return;
            if (isDodging) return;


            //allow player movement if performing action during a skill that allows movement
            bool canMoveDuringAction = false;
            if (player.isPerformingAction && player.characterCombatManager.currentCastingSkill != null)
            {
                // check for missing SO 
                if (player.characterCombatManager.currentCastingSkill.baseSkill != null)
                {
                    canMoveDuringAction = player.characterCombatManager.currentCastingSkill.baseSkill.canMoveWhileCasting;
                }
                else
                {
                    Debug.LogWarning("BaseSkill is null on the current casting skill!");
                }
            }

            //else root in place temporarily 
            if (!player.canMove || (player.isPerformingAction && !canMoveDuringAction))
            {
                return;
            }

            bool currentlyMoving = moveDirection != Vector3.zero;
            if (isMoving.Value != currentlyMoving)
            {
                isMoving.Value = currentlyMoving;
            }

            float baseSpeed = 0f;
            //determine speed of motion here: can we sprint? can we run? walk only?
            if (currentlyMoving)
            {
                if(isSprinting) //SPRINT SPEED
                {
                    baseSpeed = sprintSpeed;
                }
                else if(moveDirection.magnitude <= 0.5f) //WALK SPEED
                {
                    baseSpeed = walkSpeed;
                }
                else //RUN SPEED
                {
                    baseSpeed = runSpeed;
                }
            }

            float finalSpeed = baseSpeed * player.playerStatsManager.moveSpeedMultiplier.Value;

            Vector3 animatedMoveDirection = moveDirection;
            //apply the movespeed slow while casting ehre
            if (player.isPerformingAction && canMoveDuringAction)
            {
                float speedPenalty = player.characterCombatManager.currentCastingSkill.activeMoveSpeedPenalty.Value;

                finalSpeed *= speedPenalty;

                // use the same modifier on the blend tree as well 
                animatedMoveDirection *= speedPenalty;
            }


            player.characterController.maxWalkSpeed = finalSpeed;

            player.characterController.SetMovementDirection(moveDirection.normalized);
            player.playerAnimationManager.UpdateAnimatorMovementParameters(animatedMoveDirection, isSprinting);
        }
        private void HandleRotation()
        {
            if (player.isDead) return;
            if (!player.canRotate) return;
            if (isDodging) return;

            Vector3 targetLookDir = Vector3.zero;

            // If using input
            if (InputManager.instance.lookInputActive)
            {
                player.characterController.SetRotationMode(ECM2.Character.RotationMode.Custom);

                if (InputManager.instance.isGamepadActive)
                {
                    targetLookDir = InputManager.instance.lookDirection;
                }
                else
                {
                    Ray ray = PlayerCamera.instance.realCam.ScreenPointToRay(InputManager.instance.lookInput);

                    LayerMask aimMask = WorldUtilityManager.instance.GetCharacterLayers() | 
                        WorldUtilityManager.instance.GetEnviroLayers();
                    
                    // if we find a target, use thier position here
                    if (Physics.Raycast(ray, out RaycastHit hit, 150f, aimMask))
                    {
                        targetLookDir = (hit.point - transform.position);
                        targetLookDir.y = 0;
                    }

                    // if we find nothing, then create a hit location in space
                    else
                    {
                        Plane groundPlane = new Plane(Vector3.up, transform.position);
                        if (groundPlane.Raycast(ray, out float distance))
                        {
                            Vector3 hitPoint = ray.GetPoint(distance);
                            targetLookDir = (hitPoint - transform.position);
                            targetLookDir.y = 0;
                        }
                    }
                }
                if (targetLookDir != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetLookDir);
                    Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    player.characterController.SetRotation(newRotation);
                }
            }
            // if no input and we DO have a target
            else if (player.characterCombatManager.currentTarget != null
                && player.IsTargetValid(player.characterCombatManager.currentTarget))
            {
                player.characterController.SetRotationMode(ECM2.Character.RotationMode.Custom);

                targetLookDir = player.characterCombatManager.currentTarget.GetTransform().position - transform.position;
                targetLookDir.y = 0;
            }

            // if we had either a target OR were inputting a look direction:
            if (targetLookDir != Vector3.zero)
            {
                targetLookDir.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(targetLookDir);
                Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                player.characterController.SetRotation(newRotation);
            }

            // if no input or target, just use move direction
            else
            {
                player.characterController.rotationRate = rotationSpeed;
                player.characterController.SetRotationMode(ECM2.Character.RotationMode.OrientRotationToMovement);
            }

        }
        #endregion

        #region Dodge Functions
        public override void AttemptToDodge()
        {
            if (player.isPerformingAction || isDodging) return;

            if (!player.playerStatsManager.TryConsumeStamina(dodgeCost)) return;

            Vector3 dodgeDirection = Vector3.zero;
            string dodgeAnim = "";
            
            // If we are moving, dodge in the direction of movement
            if(isMoving.Value)
            {
                dodgeDirection = moveDirection.normalized;
                dodgeAnim = player.playerAnimationManager.dodgeForward;

                //face the player in the direction of the dodge first
                player.characterController.SetRotation(Quaternion.LookRotation(dodgeDirection));

            }
            // if we are NOT moving, dodge backwards
            else
            {
                dodgeDirection = -player.transform.forward;
                dodgeAnim = player.playerAnimationManager.dodgeBackward;
            }

            //Start Animation
            player.playerAnimationManager.PlayActionAnimation(dodgeAnim, true);

            //Start Coroutine
            StartCoroutine(DodgeRoutine(dodgeDirection));

        }
        private IEnumerator DodgeRoutine(Vector3 dodgeDirection)
        {
            isDodging = true;
            player.isInvincible.Value = true;

            //change layers to pass through enemies
            int originalLayer = gameObject.layer;
            gameObject.layer = LayerMask.NameToLayer("Ghost");

            player.characterController.maxWalkSpeed = dodgeSpeed;
            player.characterController.SetMovementDirection(dodgeDirection);

            yield return new WaitForSeconds(dodgeIFrameDuration);
            player.isInvincible.Value = false;

            float remainingDodgeTime = dodgeDuration - dodgeIFrameDuration;
            if (remainingDodgeTime > 0)
            {
                yield return new WaitForSeconds(remainingDodgeTime);
            }

            gameObject.layer = originalLayer;
            isDodging = false;
        }
        #endregion

        
    }
}


