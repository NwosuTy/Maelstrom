using UnityEngine;

namespace Creotly_Studios
{
    [CreateAssetMenu(fileName = "Pursue State", menuName = "Creotly Studio/AIStates/Pursue State")]
    public class PursueState : AIState
    {
        [SerializeField] private float timeInState = 7.5f;

        public override AIState AISate_Updater(AIManager aiManager)
        {
            if(aiManager.performingAction)
            {
                aiManager.aIAnimationManager.SetBlendTreeParameter(0f, 0f, false, Time.deltaTime);
                return this;
            }

            aiManager.isLockedIn = false;
            timeInState -= Time.deltaTime;
            
            if(aiManager.target.source == null)
            {
                return SwitchState(aiManager.patrolState, aiManager);
            }

            if (aiManager.dontMove != true && aiManager.navMeshAgent.enabled == false)
            {
                aiManager.navMeshAgent.enabled = true;
            }
            return HandleAction(aiManager);
        }

        protected AIState HandleAction(AIManager aiManager)
        {
            aiManager.aILocomotionManager.RotateTowardsTarget();

            if (aiManager.target.targetType == TargetType.Visual)
            {
                return VisualChase(aiManager);
            }
            return SoundChase(aiManager);
        }

        protected override void ResetStateParameters(AIManager aIManager)
        {

        }

        private AIState SoundChase(AIManager aiManager)
        {
            if(aiManager.DistanceToTarget >= aiManager.navMeshAgent.stoppingDistance)
            {
                aiManager.aIAnimationManager.HandleAnimation(5.0f);
                aiManager.aILocomotionManager.HandleMovement(aiManager.target.targetPosition, aiManager.aILocomotionManager.movementSpeed);
            }
            if(timeInState <= 0.0f)
            {
                timeInState = 10.0f;
                return SwitchState(aiManager.patrolState, aiManager);
            }
            return this;
        }

        private AIState VisualChase(AIManager aiManager)
        {
            bool notClose = aiManager.DistanceToTarget >= aiManager.navMeshAgent.stoppingDistance;

            if (notClose)
            {
                aiManager.aIAnimationManager.HandleAnimation(5.0f);
                aiManager.aILocomotionManager.HandleMovement(aiManager.target.targetPosition, aiManager.aILocomotionManager.movementSpeed);
            }
            else
            {
                aiManager.isMoving = false;
                return SwitchState(aiManager.combatState, aiManager);
            }
            return this;
        }
    }
}
