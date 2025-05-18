using UnityEngine;

namespace Creotly_Studios
{
    public class PlayerMovementState : ScriptableObject
    {
        protected float walkSpeed;
        protected float rotateSpeed;

        protected Vector3 moveDirection;
        protected Transform cameraObject;
        protected PlayerLocomotionManager playerLocomotion;

        public virtual void EnterState(PlayerManager player)
        {
            playerLocomotion = player.playerLocomotionManager;

            walkSpeed = playerLocomotion.walkingSpeed;
            rotateSpeed = playerLocomotion.rotationSpeed;
            cameraObject = playerLocomotion.cameraObject;
        }

        public virtual void ExitState(PlayerManager player)
        {

        }

        public virtual void HandleMovement(float delta, PlayerManager player)
        {

        }

        public virtual void HandleRotation(float delta, PlayerManager player)
        {

        }
    }
}
