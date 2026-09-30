using Unity.Cinemachine;
using UnityEngine;

namespace KE
{
    public class PlayerCamera : MonoBehaviour
    {
        private PlayerManager player;

        [Header("Camera Main Components")]
        [SerializeField] CinemachineCamera cam;
        [SerializeField] Transform cameraRoot;
        [SerializeField] Transform cameraPivot;

        [Header("Speeds")]
        [SerializeField] float followSmoothSpeed = 0.1f;
        private Vector3 cameraFollowVelocity = Vector3.zero;

        private void Awake()
        {
            if (cam == null) cam = GetComponentInChildren<CinemachineCamera>();
            if (cameraRoot == null) cameraRoot = GetComponent<Transform>();
        }
        private void Start()
        {
            if (player == null)
            {
                //TEMP
                //once title screen is fully made, then player should be made in title
                //sceen when selecting character, and can be assigned to camera once sceen change
                player = FindFirstObjectByType<PlayerManager>();
            }
        }

        private void LateUpdate()
        {
            //Player moves in update, follow camera right after 
            HandleCameraActions();
        }

        private void HandleCameraActions()
        {
            //RotateCamera() is we want to allow this later
            HandleCameraMovement();
        }

        private void HandleCameraMovement()
        {
            if (player == null) return;

            Vector3 targetPosition = Vector3.SmoothDamp(cameraRoot.position, player.transform.position, ref cameraFollowVelocity, followSmoothSpeed);
            cameraRoot.position = targetPosition;
        }

    }
}
