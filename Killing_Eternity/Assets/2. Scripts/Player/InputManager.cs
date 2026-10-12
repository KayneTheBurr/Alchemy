using UnityEngine;
using UnityEngine.InputSystem;

namespace KE
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;
        private PlayerControls playerControls;
        public PlayerManager player;

        [Header("Action Inputs")]
        public bool dodge_Input = false;
        public bool potion_Input;

        [Header("Skill Inputs")]
        public bool skill0_Input;
        public bool skill1_Input;
        public bool skill2_Input;
        public bool skill3_Input;
        public bool skill4_Input;
        public bool skill5_Input;

        [Header("Movement Values")]
        public Vector3 moveDirection;
        [SerializeField] Vector2 movementInput;

        [Header("Look Values")]
        public Vector2 lookInput;
        public Vector3 lookDirection;
        public float lookDeltaTolerance = 0.1f;
        public bool lookInputActive = false;
        public bool isGamepadActive;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            if(playerControls == null)
            {
                playerControls = new PlayerControls();

                // PLAYER MOVEMENT INPUTS
                playerControls.Player.Move.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.Player.Move.canceled += i => movementInput = Vector2.zero;
                playerControls.Player.Look.performed += i =>
                {
                    lookInput = i.ReadValue<Vector2>();
                    isGamepadActive = i.control.device is Gamepad;
                };
                playerControls.Player.Look.canceled += i => lookInput = Vector2.zero;

                // Action Inputs
                playerControls.Player.Dodge.performed += i => dodge_Input = true;


                // Skill Inputs
                playerControls.Player.Skill1.performed += i => skill0_Input = true;
                playerControls.Player.Skill2.performed += i => skill1_Input = true;
                playerControls.Player.Skill3.performed += i => skill2_Input = true;
                playerControls.Player.Skill4.performed += i => skill3_Input = true;
                playerControls.Player.Skill5.performed += i => skill4_Input = true;
                playerControls.Player.Skill6.performed += i => skill5_Input = true;

                playerControls.Player.Potion.performed += i => potion_Input = true;


                playerControls.Enable();
            }
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }
        private void Update()
        {
            HandleInputActions();
        }

        private void HandleInputActions()
        {
            HandleMoveInput();
            HandleLookInput();
            HandleDodgeInput();
            HandleSkillInput();
            HandlePotionUse();
        }

        private void HandleMoveInput()
        {
            //If no movement detected, set movement target to current pos
            if(movementInput.magnitude < 0.01f)
            {
                moveDirection = Vector3.zero;
                return;
            }

            Vector3 cameraForwardDirection = Camera.main.transform.forward;
            cameraForwardDirection.y = 0;
            cameraForwardDirection.Normalize();

            Vector3 cameraRightDirection = Camera.main.transform.right;
            cameraRightDirection.y = 0;
            cameraRightDirection.Normalize();

            //create move direction based oninput and camera position
            moveDirection = (cameraForwardDirection * movementInput.y) + (cameraRightDirection * movementInput.x);
        
        }

        private void HandleLookInput()
        {
            lookInputActive = false;

            //USING CONTROLLER
            if (InputManager.instance.isGamepadActive && InputManager.instance.lookInput.magnitude > lookDeltaTolerance)
            {
                lookInputActive = true;

                Vector3 camForward = Camera.main.transform.forward;
                camForward.y = 0;
                camForward.Normalize();

                Vector3 camRight = Camera.main.transform.right;
                camRight.y = 0;
                camRight.Normalize();

                lookDirection = (camForward * InputManager.instance.lookInput.y) + (camRight * InputManager.instance.lookInput.x);
            }
            //USING MOUSE AIM
            else if (!InputManager.instance.isGamepadActive)
            {
                lookInputActive = true;

                lookInput = Mouse.current.position.ReadValue();

            }
            //NO LOOK INPUT
            else
            {
                lookInputActive = false;
                lookDirection = moveDirection;
            }
        }
        private void HandleDodgeInput()
        {
            if (dodge_Input)
            {
                dodge_Input = false;

                //disable when menu is open
                if (PlayerUIManager.instance.menuWindowOpen) return;

                player.playerMovementManager.AttemptToDodge();
            }
        }

        public void HandleSkillInput()
        {
            // Skill 1 / Interact
            if (skill0_Input)
            {
                skill0_Input = false; // Consume the input

                // Hypothetical check: If standing near a chest, open it instead of casting.
                // if (player.playerInteractManager.currentInteractable != null)
                // {
                //     player.playerInteractManager.Interact();
                //     return; 
                // }

                player.playerCombatManager.HandleSkillInput(0);
            }

            // Standard Skills
            if (skill1_Input)
            {
                skill1_Input = false;
                player.playerCombatManager.HandleSkillInput(1);
            }
            if (skill2_Input)
            {
                skill2_Input = false;
                player.playerCombatManager.HandleSkillInput(2);
            }
            if (skill3_Input)
            {
                skill3_Input = false;
                player.playerCombatManager.HandleSkillInput(3);
            }
            if (skill4_Input)
            {
                skill4_Input = false;
                player.playerCombatManager.HandleSkillInput(4);
            }
            if (skill5_Input)
            {
                skill5_Input = false;
                player.playerCombatManager.HandleSkillInput(5);
            }
        }

        public void HandlePotionUse()
        {
            if (potion_Input)
            {
                potion_Input = false;

                Debug.Log("Potion consumed!");
            }
        }
    }
}
