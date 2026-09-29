using UnityEngine;
using UnityEngine.InputSystem;

namespace KE
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;
        private PlayerControls playerControls;

        //[Header("Inputs")]

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

                //Add all input action mapping here
                playerControls.Player.Move.performed += i => movementInput = i.ReadValue<Vector2>();
                playerControls.Player.Move.canceled += i => movementInput = Vector2.zero;
                playerControls.Player.Look.performed += i =>
                {
                    lookInput = i.ReadValue<Vector2>();
                    isGamepadActive = i.control.device is Gamepad;
                };
                playerControls.Player.Look.canceled += i => lookInput = Vector2.zero;


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
    }
}
