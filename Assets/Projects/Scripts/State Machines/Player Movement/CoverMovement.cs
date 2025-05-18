using System;
using UnityEngine;

namespace Creotly_Studios
{
    [CreateAssetMenu(fileName = "CoverMovement", menuName = "Creotly Studio/PlayerMovementState/Cover State")]
    public class CoverMovement : PlayerMovementState
    {
        [Header("Parameters")]
        [SerializeField] private float runSpeed;
        [Range(0.85f, 2.0f)][SerializeField] private float distanceThreshold = 1.2f;

        public override void EnterState(PlayerManager player)
        {
            base.EnterState(player);
            player.coverDetector.DetectCover();

            if(player.coverState != CoverState.EnteringCover)
            {
                playerLocomotion.HandleMovementStateSwitch(playerLocomotion.CurrentState, playerLocomotion.NormalState, false);
                return;
            }
        }

        public override void ExitState(PlayerManager player)
        {
            base.ExitState(player);

            player.enterCover = false;
            player.coverState = CoverState.NoCover;
        }

        public override void HandleMovement(float delta, PlayerManager player)
        {
            MoveTowardsCover(delta, player);
            if(player.coverState == CoverState.InCover)
            {
                InCover_Move(delta, player);
            }
        }

        public override void HandleRotation(float delta, PlayerManager player)
        {
            RotateTowardsCover(delta, player);
            if(player.coverState == CoverState.InCover)
            {
                InCover_Rotate(delta, player);
            }
        }

        private void InCover_Move(float delta, PlayerManager player)
        {
            bool noLeft = player.coverDetector.atLeftEdge;
            bool noRight = player.coverDetector.atRightEdge;
            float horizontalInput = player.playerInputManager.horizontalMovementInput;

            PlayerAnimationManager playerAnim = player.playerAnimationManager;
            Vector3 tangent = Maths_PhysicsHelper.GetTangent(player.coverDetector.coverHitPoint.rot).normalized;

            bool dirAllowed = (horizontalInput < 0 && noLeft != true) || (horizontalInput > 0 && noRight != true);
            float finalInput = (dirAllowed) ? horizontalInput : 0;

            moveDirection = tangent * finalInput;
            moveDirection.y = 0;
            float speed = 0.45f * walkSpeed * delta;

            player.characterController.Move(moveDirection * speed);
            playerAnim.SetBlendTreeParameter(1f, -finalInput, false, delta);
        }

        private void InCover_Rotate(float delta, PlayerManager player)
        {
            Vector3 forward = -player.coverDetector.coverHitPoint.rot;

            Transform myTransform = player.transform;
            Quaternion rot = Quaternion.LookRotation(forward, Vector3.up);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, rot, rotateSpeed * delta);
        }

        private void RotateTowardsCover(float delta, PlayerManager player)
        {
            if (player.coverState != CoverState.EnteringCover)
            {
                return;
            }

            Transform myTransform = player.transform;
            Vector3 pos = player.coverDetector.coverHitPoint.pos;

            Vector3 direction = (pos - player.transform.position);
            direction.y = 0;

            if(direction != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(direction);
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation, rot, rotateSpeed * delta);
            }
        }

        private void MoveTowardsCover(float delta, PlayerManager player)
        {
            if(player.coverState != CoverState.EnteringCover)
            {
                return;
            }
            Transform myTransform = player.transform;
            Vector3 pos = player.coverDetector.coverHitPoint.pos;
            CharacterAnimationManager animManager = player.characterAnimationManager;

            int verticalAnim = animManager.verticalMovementHash;
            float distance = Vector3.Distance(myTransform.position, pos);
            Vector3 moveDirection = (pos - myTransform.position).normalized;

            if (distance > distanceThreshold)
            {
                animManager.InstantSetFloat(verticalAnim, 2.0f);
                player.characterController.Move(delta * runSpeed * moveDirection);
                return;
            }
            animManager.InstantSetFloat(verticalAnim, 0.0f);
            player.enterCover = true;
            player.coverState = CoverState.InCover;
        }
    }
}
