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
            HandleDodge();
        }

        private void HandleMovement()
        {
            if (player.isDead) return;
            if (!player.canMove) return;

            bool currentlyMoving = moveDirection != Vector3.zero;
            if (isMoving != currentlyMoving)
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
            player.characterController.maxWalkSpeed = baseSpeed;

            player.characterController.SetMovementDirection(moveDirection.normalized);
            player.playerAnimationManager.UpdateAnimatorMovementParameters(moveDirection, isSprinting);
        }
        private void HandleRotation()
        {
            if (player.isDead) return;
            if (!player.canRotate) return;

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
                    Ray ray = Camera.main.ScreenPointToRay(InputManager.instance.lookInput);
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
        private void HandleDodge()
        {
            //Dodge in direction of movement, not facing direction
        }
    }
}


