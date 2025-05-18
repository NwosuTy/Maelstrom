using System.Collections;
using UnityEngine;

namespace Creotly_Studios
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager playerManager;
        public PlayerMovementState CurrentState { get; private set; }

        [field: Header("Movement States")]
        [field: SerializeField] public CoverMovement CoverState { get; private set; }
        [field: SerializeField] public NormalMovement NormalState { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            playerManager = characterManager as PlayerManager;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            CoverState = Instantiate(CoverState);
            NormalState = Instantiate(NormalState);

            CurrentState = NormalState;
            CurrentState.EnterState(playerManager);
        }

        // Update is called once per frame
        public override void CharacterLocomotionManager_Update(float delta)
        {
            base.CharacterLocomotionManager_Update(delta);
            HandleFreeFall(delta);
            HandleJumpingMovement(delta);
        }

        public override void CharacterLocomotion_FixedUpdate(float delta)
        {
            if (playerManager.dontMove != true)
            {
                HandleRotation(delta);
            }
        }

        //Functionalities

        public void HandleJumping()
        {
            if(playerManager.performingAction || playerManager.isLockedIn || playerManager.dontMove)
            {
                return;
            }

            if(playerManager.isJumping)
            {
                return;
            }

            if(playerManager.playerStatsManager.currentEndurance <= jumpEnduranceCost)
            {
                return;
            }

            if(playerManager.isGrounded != true)
            {
                return;
            }

            if(playerManager.isCrouching == true)
            {
                playerManager.isCrouching = false;
            }

            playerManager.isJumping = true;
            playerManager.footIKSystem.SetBoneIKConstraint(0.0f);

            jumpDirection = cameraObject.transform.forward * playerManager.playerInputManager.verticalMovementInput;
            jumpDirection += cameraObject.transform.right * playerManager.playerInputManager.horizontalMovementInput;
            jumpDirection.y = 0.0f;

            if(jumpDirection != Vector3.zero)
            {
                if(playerManager.sprintFlag)
                {
                    jumpDirection *= 1.0f;
                }
                else if(playerManager.playerInputManager.totalMoveAmount >= 0.5f)
                {
                    jumpDirection *= 0.5f;
                }
                else if (playerManager.playerInputManager.totalMoveAmount < 0.5f)
                {
                    jumpDirection *= 0.25f;
                }
            }
            int jumpHash = (jumpDirection == Vector3.zero) ? AnimatorHashNames.jumpHash : AnimatorHashNames.jumpFwdHash;
            playerManager.playerAnimationManager.PlayTargetAnimation(jumpHash, false);
        }
        
        protected virtual void HandleJumpingMovement(float delta)
        {
            if(characterManager.isJumping)
            {
                characterManager.characterController.Move(delta * walkingSpeed * jumpDirection);
            }
        }
        
        public virtual void ApplyJumpingVelocity()
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * - 2.0f * gravityForce);
            playerManager.playerStatsManager.ReduceEndurancePeriodically(jumpEnduranceCost, 1.0f);
        }

        public void HandleMovementStateSwitch(PlayerMovementState current, PlayerMovementState next, bool quickSwitch = true)
        {
            if(quickSwitch)
            {
                current.ExitState(playerManager);
                next.EnterState(playerManager);
                CurrentState = next;
                return;
            }
            StartCoroutine(SwitchMovement(current, next));
        }

        private IEnumerator SwitchMovement(PlayerMovementState current, PlayerMovementState next)
        {
            current.ExitState(playerManager);
            yield return null;

            next.EnterState(playerManager);
            yield return null;

            CurrentState = next;
        }

        protected virtual void HandleFreeFall(float delta)
        {
            if(characterManager.isGrounded != true)
            {
                Vector3 freeFallDirection;

                freeFallDirection = cameraObject.forward * playerManager.playerInputManager.verticalMovementInput;
                freeFallDirection += cameraObject.right * playerManager.playerInputManager.horizontalMovementInput;
                freeFallDirection.y = 0.0f;

                characterManager.characterController.Move(walkingSpeed * delta * freeFallDirection);
            }
        }

        protected override void HandleRotation(float delta)
        {
            if(CurrentState != null)
                CurrentState.HandleRotation(delta, playerManager);
        }

        protected override void HandleMovement(float delta)
        {
            if (CurrentState != null)
                CurrentState.HandleMovement(delta, playerManager);
        }
    }
}
