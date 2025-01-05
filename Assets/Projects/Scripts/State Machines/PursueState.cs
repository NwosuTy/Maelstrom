using UnityEngine;

namespace Creotly_Studios
{
    [CreateAssetMenu(fileName = "Pursue State", menuName = "Creotly Studio/AIStates/Pursue State")]
    public class PursueState : AIState
    {
        [SerializeField] protected float timeInState = 7.5f;
        public override AIState AISate_Updater(AIManager aiManager)
        {
            if(aiManager.performingAction)
            {
                aiManager.aIAnimationManager.SetBlendTreeParameter(0f, 0f, false, Time.deltaTime);
                return this;
            }

            timeInState -= Time.deltaTime;
            if(aiManager.target.source == null)
            {
                return SwitchState(aiManager.patrolState, aiManager);
            }

            if(aiManager.navMeshAgent.enabled == false)
            {
                aiManager.navMeshAgent.enabled = true;
            }

            if(aiManager.enemyType == EnemyType.Mech)
            {
                return MechEnemy_Updater(aiManager);
            }
            return HumanoidEnemy_Updater(aiManager);
        }

        protected override AIState MechEnemy_Updater(AIManager aiManager)
        {
            if(aiManager.AngleOfTarget < aiManager.angleLimit.lowerBound || aiManager.AngleOfTarget > aiManager.angleLimit.upperBound)
            {
                aiManager.aILocomotionManager.PivotTowardsTarget(aiManager);
            }
            aiManager.aILocomotionManager.RotateTowardsTarget();
            
            if(aiManager.target.targetType == TargetType.Visual)
            {
                return VisualChase(aiManager);
            }
            return SoundChase(aiManager);
        }

        protected override AIState HumanoidEnemy_Updater(AIManager aiManager)
        {
            aiManager.aILocomotionManager.RotateTowardsTarget();
            
            if(aiManager.target.targetType == TargetType.Visual)
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
                aiManager.aIAnimationManager.HandleAnimation(aiManager.navMeshAgent.stoppingDistance);
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
            if(aiManager.DistanceToTarget >= aiManager.navMeshAgent.stoppingDistance)
            {
                aiManager.aIAnimationManager.HandleAnimation(aiManager.navMeshAgent.stoppingDistance * 2.5f);
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
