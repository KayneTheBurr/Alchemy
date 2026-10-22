using System.Collections;
using UnityEngine;

namespace KE
{
    public class PlayerMovementManager : CharacterMovementManager
    {
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

        private void HandleMovement()
        {
            if (player.isDead) return;
            if (!player.canMove || player.isPerformingAction) return;
            if (isDodging) return;

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
            float finalSpeed = baseSpeed * player.playerStatsManager.moveSpeedMultiplier;
            player.characterController.maxWalkSpeed = finalSpeed;

            player.characterController.SetMovementDirection(moveDirection.normalized);
            player.playerAnimationManager.UpdateAnimatorMovementParameters(moveDirection, isSprinting);
        }
        private void HandleRotation()
        {
            if (player.isDead) return;
            if (!player.canRotate) return;
            if (isDodging) return;

            Vector3 targetLookDir = Vector3.zero;
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
                    Plane groundPlane = new Plane(Vector3.up, transform.position);

                    if (groundPlane.Raycast(ray, out float distance))
                    {
                        Vector3 hitPoint = ray.GetPoint(distance);
                        targetLookDir = (hitPoint - transform.position);  
                        targetLookDir.y = 0;
                        targetLookDir.Normalize();
                    }
                }
                if (targetLookDir != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetLookDir);
                    Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    player.characterController.SetRotation(newRotation);
                }
            }
            else
            {
                player.characterController.rotationRate = rotationSpeed;
                player.characterController.SetRotationMode(ECM2.Character.RotationMode.OrientRotationToMovement);
            }
        }
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
    }
}


