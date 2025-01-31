using UnityEngine;

namespace Creotly_Studios
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {
        PlayerManager playerManager;

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
            HandleRotation(delta);
        }

        //Functionalities

        public void HandleJumping()
        {
            if(playerManager.performingAction || playerManager.isLockedIn)
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
            playerManager.playerAnimationManager.PlayTargetAnimation(AnimatorHashNames.jumpHash, false);
            playerManager.playerStatsManager.ReduceEndurancePeriodically(jumpEnduranceCost, 1.0f);

            jumpDirection = cameraObject.transform.forward * playerManager.playerInputManager.verticalMovementInput;
            jumpDirection += cameraObject.transform.right * playerManager.playerInputManager.horizontalMovementInput;
            jumpDirection.Normalize();

            jumpDirection.y = 0.0f;
            if(jumpDirection == Vector3.zero)
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
                    jumpDirection *= 0.0f;
                }
            }
        }
        
        protected virtual void HandleJumpingMovement(float delta)
        {
            if(characterManager.isJumping)
            {
                characterManager.characterController.Move(delta * sprintingSpeed * jumpDirection);
            }
        }
        
        public virtual void ApplyJumpingVelocity()
        {
            verticalVelocity.y = Mathf.Sqrt(jumpHeight - 2.0f * gravityForce);
        }

        protected virtual void HandleFreeFall(float delta)
        {
            if(characterManager.isGrounded != true)
            {
                Vector3 freeFallDirection;

                freeFallDirection = cameraObject.forward * playerManager.playerInputManager.verticalMovementInput;
                freeFallDirection += cameraObject.right * playerManager.playerInputManager.horizontalMovementInput;

                freeFallDirection.Normalize();
                freeFallDirection.y = 0.0f;

                characterManager.characterController.Move(movementSpeed * delta * freeFallDirection);
            }
        }

        protected override void HandleRotation(float delta)
        {
            float yawCamera = cameraObject.rotation.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, yawCamera, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        protected override void HandleMovement(float delta)
        {
            if(playerManager.isGrounded != true)
            {
                return;
            }
            
            float verticalInput = playerManager.playerInputManager.verticalMovementInput;
            float horizontalInput = playerManager.playerInputManager.horizontalMovementInput;

            moveDirection = verticalInput * cameraObject.forward;
            moveDirection += horizontalInput * cameraObject.right;

            moveDirection.Normalize();
            moveDirection.y = 0.0f;

            if(playerManager.sprintFlag)
            {
                playerManager.playerStatsManager.ReduceEndurancePeriodically(sprintEnduranceCost, delta);
                playerManager.characterController.Move((sprintingSpeed * acceleration) * delta * moveDirection);
            }
            else
            {
                playerManager.characterController.Move((movementSpeed * acceleration) * delta * moveDirection);
            }
            playerManager.characterAnimationManager.SetBlendTreeParameter(verticalInput, horizontalInput, playerManager.sprintFlag, delta);
        }
    }
}
