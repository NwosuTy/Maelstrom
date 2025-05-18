using log4net.Util;
using UnityEngine;

namespace Creotly_Studios
{
    [CreateAssetMenu(fileName = "NormalMovement", menuName = "Creotly Studio/PlayerMovementState/Normal State")]
    public class NormalMovement : PlayerMovementState
    {
        private float sprintEnduranceCost;

        public override void EnterState(PlayerManager player)
        {
            base.EnterState(player);
            sprintEnduranceCost = playerLocomotion.sprintEnduranceCost;
        }

        public override void ExitState(PlayerManager player)
        {
            base.ExitState(player);
        }

        public override void HandleMovement(float delta, PlayerManager player)
        {
            if (player.isGrounded != true)
            {
                return;
            }

            float verticalInput = player.playerInputManager.verticalMovementInput;
            float horizontalInput = player.playerInputManager.horizontalMovementInput;

            moveDirection = verticalInput * cameraObject.forward;
            moveDirection += horizontalInput * cameraObject.right;

            moveDirection.Normalize();
            moveDirection.y = 0.0f;
            float acceleration;

            if (player.sprintFlag)
            {
                acceleration = 2.4f;
                player.playerStatsManager.ReduceEndurancePeriodically(sprintEnduranceCost, delta);
            }
            else
            {
                acceleration = (horizontalInput >= 0.11f) ? 0.6f : 1f;
            }
            player.characterController.Move((walkSpeed * acceleration) * delta * moveDirection);
            player.characterAnimationManager.SetBlendTreeParameter(verticalInput, horizontalInput, player.sprintFlag, delta);
        }

        public override void HandleRotation(float delta, PlayerManager player)
        {
            float yawCamera = cameraObject.rotation.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, yawCamera, 0f);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, rotateSpeed * delta);
        }
    }
}
