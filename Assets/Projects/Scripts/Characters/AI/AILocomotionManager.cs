using UnityEngine;
using UnityEngine.AI;

namespace Creotly_Studios
{
    public class AILocomotionManager : CharacterLocomotionManager
    {
        AIManager aiManager;

        protected override void Awake()
        {
            base.Awake();
            aiManager = characterManager as AIManager;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        public override void CharacterLocomotionManager_Update(float delta)
        {
            HandleGravity(delta);
        }

        public override void CharacterLocomotion_FixedUpdate(float delta)
        {

        }

        //Functionalities

        public void HandleRotationWhileAttacking(AIManager aiManager)
        {
            if(aiManager.target.source == null)
            {
                return;
            }

            if(aiManager.canRotate != true)
            {
                return;
            }

            if(aiManager.performingAction != true)
            {
                return;
            }

            Vector3 targetDirection = aiManager.DirectionToTarget;
            targetDirection.y = 0.0f;
            targetDirection.Normalize();

            if(aiManager.DirectionToTarget == Vector3.zero)
            {
                targetDirection = aiManager.transform.forward;
            }
            
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            aiManager.transform.rotation = Quaternion.Slerp(aiManager.transform.rotation, targetRotation, 
                aiManager.characterLocomotionManager.rotationSpeed * Time.deltaTime);
        }

        public void PivotTowardsTarget(AIManager aiManager)
        {
            if(aiManager.performingAction)
            {
                return;
            }
            
            if(aiManager.AngleOfTarget >= 20 && aiManager.AngleOfTarget <= 145)
            {
                aiManager.aIAnimationManager.PlayRootTargetAnimation(AnimatorHashNames.turn_R_90, true);
            }
            else if(aiManager.AngleOfTarget >= -145  && aiManager.AngleOfTarget <= -20)
            {
                aiManager.aIAnimationManager.PlayRootTargetAnimation(AnimatorHashNames.turn_L_90, true);
            }
            else if(aiManager.AngleOfTarget > 145 && aiManager.AngleOfTarget <= 180)
            {
                aiManager.aIAnimationManager.PlayRootTargetAnimation(AnimatorHashNames.turn_R_180, true);
            }
            else if(aiManager.AngleOfTarget < -145 && aiManager.AngleOfTarget >= -180)
            {
                aiManager.aIAnimationManager.PlayRootTargetAnimation(AnimatorHashNames.turn_L_180, true);
            }

        }

        public void RotateTowardsTarget()
        {
            if(aiManager.isMoving == true)
            {
                aiManager.transform.rotation = aiManager.navMeshAgent.transform.rotation;
            }
        }

        public void HandleMovement(Vector3 targetPosition, float speed)
        {
            if(aiManager.navMeshPath == null)
            {
                aiManager.navMeshPath = new NavMeshPath();
            }
            if(aiManager.navMeshPath.status != NavMeshPathStatus.PathComplete)
            {
                aiManager.navMeshPath.ClearCorners();
            }
            if(aiManager.navMeshAgent.CalculatePath(targetPosition, aiManager.navMeshPath))
            {
                aiManager.navMeshAgent.SetPath(aiManager.navMeshPath);
            }
            Vector3 moveDirection = aiManager.navMeshAgent.desiredVelocity;
            aiManager.characterController.Move(speed * Time.deltaTime * moveDirection);
        }
    }
}
