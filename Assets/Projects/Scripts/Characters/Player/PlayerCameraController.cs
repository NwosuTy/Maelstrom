using UnityEngine;
using Cinemachine;

namespace Creotly_Studios
{
    public class PlayerCameraController : MonoBehaviour
    {
        #region Private Variables

        //Components
        PlayerManager playerManager;

        //Cameras
        private Camera mainCamera;
        private Transform cameraTransform;

        #endregion

        [field: Header("Cameras")]
        [field: SerializeField] public Transform cameraTarget {get; private set;}
        [field: SerializeField] public Transform miniMapTarget {get; private set;}

        [field: Header("Cameras")]
        [field: SerializeField] public CinemachineFreeLook normalCamera {get; private set;}
        [field: SerializeField] public CinemachineFreeLook lockedInCamera {get; private set;}


        // Start is called before the first frame update
        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();

            normalCamera = GameObject.Find("Normal Camera").GetComponent<CinemachineFreeLook>();
            lockedInCamera = GameObject.Find("Locked-In Camera").GetComponent<CinemachineFreeLook>();
        }

        private void Start()
        {
            InitializeCameras(normalCamera, true);
            InitializeCameras(lockedInCamera, false);

            mainCamera = Camera.main;
            cameraTransform = mainCamera.transform;
        }

        // Update is called once per frame
        public void PlayerCameraController_Update()
        {
            SwapCameraLockedIn();
        }

        //Functionalities

        private void InitializeCameras(CinemachineFreeLook cinemachineVirtualCamera, bool enableCamera)
        {
            cinemachineVirtualCamera.LookAt = cameraTarget;
            cinemachineVirtualCamera.Follow = playerManager.transform;
            cinemachineVirtualCamera.gameObject.SetActive(enableCamera);
        }

        private void SwapCameraLockedIn()
        {
            lockedInCamera.gameObject.SetActive(playerManager.isLockedIn);
        }
    }
}
