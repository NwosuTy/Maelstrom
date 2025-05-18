using UnityEngine;
using Unity.Cinemachine;

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
        [field: SerializeField] public CinemachineCamera InCoverCamera {get; private set;}
        [field: SerializeField] public CinemachineCamera FreeLookCamera {get; private set;}
        [field: SerializeField] public CinemachineCamera LockedInCamera {get; private set;}


        // Start is called before the first frame update
        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();

            FreeLookCamera = GameObject.Find("Free Camera").GetComponent<CinemachineCamera>();
            InCoverCamera = GameObject.Find("Cover Camera").GetComponent<CinemachineCamera>();
            LockedInCamera = GameObject.Find("Locked Camera").GetComponent<CinemachineCamera>();
        }

        private void Start()
        {
            InitializeCamera(FreeLookCamera, true);
            InitializeCamera(InCoverCamera, false);
            InitializeCamera(LockedInCamera, false);

            mainCamera = Camera.main;
            cameraTransform = mainCamera.transform;
        }

        // Update is called once per frame
        public void PlayerCameraController_Update()
        {
            InCoverCamera.gameObject.SetActive(playerManager.enterCover);
            LockedInCamera.gameObject.SetActive(playerManager.isLockedIn);
        }

        //Functionalities

        private void InitializeCamera(CinemachineCamera camera, bool status)
        {
            camera.Target.TrackingTarget = playerManager.targetPoint;
            camera.gameObject.SetActive(status);
        }
    }
}
