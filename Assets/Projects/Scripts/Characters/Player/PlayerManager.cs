using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Creotly_Studios
{
    public class PlayerManager : CharacterManager
    {
        //Creotly Components
        public PlayerStatsManager playerStatsManager {get; private set;}
        public PlayerCombatManager playerCombatManager {get; private set;}
        public PlayerAnimationManager playerAnimationManager {get; private set;}
        public PlayerInventoryManager playerInventoryManager {get; private set;}
        public PlayerLocomotionManager playerLocomotionManager {get; private set;}

        //Player Components
        public InputManager playerInputManager {get; private set;}
        public PlayerUIManager playerUIManager {get; private set;}
        public PlayerCameraController playerCameraController {get; private set;}

        RaycastHit rayCastHit;

        [Header("Input Status and Parameters")]
        public bool sprintFlag;
        [field: SerializeField] public UIBar staminaBarUI {get; private set;}

        [Header("Cross Hair Properties")]
        public Sprite crossHairImage;
        public Sprite aimingCrossHairImage;
        [field: SerializeField] public Transform crossHairTransform {get; private set;}

        [Header("Movement")]
        public Cells currentCell;
        public Cells previousCell;
        public List<Cells> neighboringCells = new List<Cells>();

        [Header("Calculate Current Cell")]
        [SerializeField] private float rayLength;
        [SerializeField] private Vector3 worldAnchor;
        [SerializeField] private float distanceBeforeCheck = 3.0f;

        protected override void Awake()
        {
            animator = GetComponent<Animator>();
            if(animator.runtimeAnimatorController == null)
            {
                animator.runtimeAnimatorController = playerAnimationManager.mainAnimatorController;
            }

            base.Awake();
            playerInputManager = GetComponent<InputManager>();
            playerUIManager = GetComponentInChildren<PlayerUIManager>();
            playerCameraController = GetComponent<PlayerCameraController>();
            crossHairTransform = FindFirstObjectByType<CrossHairTarget>().transform;
            
            playerStatsManager = characterStatsManager as PlayerStatsManager;
            playerCombatManager = characterCombatManager as PlayerCombatManager;
            playerAnimationManager = characterAnimationManager as PlayerAnimationManager;
            playerInventoryManager = characterInventoryManager as PlayerInventoryManager;
            playerLocomotionManager = characterLocomotionManager as PlayerLocomotionManager;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            healthBarUI = playerUIManager.healthBar;
            staminaBarUI = playerUIManager.staminaBar;

            base.Start();
            playerStatsManager.ResetEndurance();
        }

        // Update is called once per frame
        protected override void Update()
        {
            if(isDead)
            {
                return;
            }

            float delta = Time.deltaTime;
            playerInputManager.InputManager_Updater();

            base.Update();
            playerUIManager.PlayerUIManager_Update(delta);
            playerCameraController.PlayerCameraController_Update();
        }
        
        protected override void FixedUpdate()
        {
            if(isDead)
            {
                return;
            }

            float delta = Time.deltaTime;

            base.FixedUpdate();
            characterLocomotionManager.CharacterLocomotion_FixedUpdate(delta);
        }

        protected override void LateUpdate()
        {
            if(isDead)
            {
                return;
            }

            float delta = Time.deltaTime;

            base.LateUpdate();
            playerInputManager.ResetInputs();
        }

        //Functionalities

        public void GetNeighboringCells()
        {
            if(isGrounded)
            {
                float distance = Vector3.Distance(transform.position, worldAnchor);
                if(distance > distanceBeforeCheck)
                {
                    if(Physics.Raycast(transform.position, Vector3.down, out rayCastHit, rayLength))
                    {
                        Cells cell = rayCastHit.collider.GetComponentInParent<Tiles>().cellHolder;
                        if(cell != null)
                        {
                            currentCell = cell;
                            worldAnchor = transform.position;
                        }
                    }
                }
            }
            if(currentCell != previousCell)
            {
                neighboringCells.Clear();
                previousCell = currentCell;
                neighboringCells.AddRange(currentCell.neighboringCells);
            }
        }

        public bool NeighboringCellsAreCollapsed()
        {
            if(neighboringCells.Count <= 0)
            {
                return false;
            }
            return neighboringCells.All(cell => cell.hasCollapsed == true);
        }
    }
}
