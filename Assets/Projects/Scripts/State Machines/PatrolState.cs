using UnityEngine;
using UnityEngine.AI;

namespace Creotly_Studios
{
    [CreateAssetMenu(fileName = "Patrol State", menuName = "Creotly Studio/AIStates/PatrolState")]
    public class PatrolState : AIState
    {
        private float idleTime = 7.5f;

        [Header("General Parameters")]
        public float sphereRadius = 10.0f;

        [Header("Time")]
        public float idleTimeDefault = 7.5f;
        public float interactTimeDefault = 10.0f;

        [Header("Status")]
        private bool destinationSet;
        public PatrolMode patrolMode = PatrolMode.Idle;

        public override AIState AISate_Updater(AIManager aiManager)
        {
            if (aiManager.performingAction)
            {
                aiManager.aIAnimationManager.SetBlendTreeParameter(0f, 0f, false, Time.deltaTime);
                return this;
            }

            if (aiManager.navMeshAgent.enabled == false)
            {
                aiManager.navMeshAgent.enabled = true;
            }

            if (aiManager.target.visualTarget != null || aiManager.target.audioTarget != null)
            {
                return SwitchState(aiManager.pursueState, aiManager);
            }
            return HandleAction(aiManager);
        }

        private AIState HandleAction(AIManager aiManager)
        {
            if (aiManager.enemyType == EnemyType.Humanoid)
            {
                if (aiManager.AngleOfTarget < aiManager.angleLimit.lowerBound || aiManager.AngleOfTarget > aiManager.angleLimit.upperBound)
                {
                    aiManager.aILocomotionManager.PivotTowardsTarget(aiManager);
                }
            }
            aiManager.aILocomotionManager.RotateTowardsTarget();

            if (patrolMode == PatrolMode.Idle)
            {
                return Idle(aiManager);
            }
            return Walk(aiManager);
        }

        #region Actions

        private AIState Idle(AIManager robot)
        {
            idleTime -= Time.deltaTime;
            robot.navMeshAgent.enabled = false;
            robot.aIAnimationManager.SetBlendTreeParameter(0f, 0f, false, Time.deltaTime);

            if (idleTime <= 0.0f)
            {
                if (destinationSet != true)
                {
                    SetDestination(robot);
                }

                if (destinationSet)
                {
                    destinationSet = false;
                    idleTime = idleTimeDefault;

                    patrolMode = PatrolMode.Walk;
                    robot.navMeshAgent.enabled = true;
                }
            }
            return this;
        }

        private AIState Walk(AIManager aiManager)
        {
            aiManager.SetPersonalTargetDetails(enemyDestination);
            if (aiManager.DistanceToTarget >= aiManager.navMeshAgent.stoppingDistance)
            {
                aiManager.aIAnimationManager.HandleAnimation(3.0f);
                aiManager.aILocomotionManager.HandleMovement(enemyDestination, aiManager.aILocomotionManager.movementSpeed);
            }
            else
            {
                patrolMode = PatrolMode.Idle;
                aiManager.navMeshAgent.enabled = false;
            }
            return this;
        }

        #endregion

        private void SetDestination(AIManager aiManager)
        {
            Vector3 randomPoint = Random.insideUnitSphere * sphereRadius + aiManager.transform.position;

            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(randomPoint, out navMeshHit, sphereRadius, NavMesh.AllAreas))
            {
                destinationSet = true;
                enemyDestination = navMeshHit.position;
                aiManager.SetPersonalTargetDetails(enemyDestination);
                return;
            }
            destinationSet = false;
        }

        protected override void ResetStateParameters(AIManager aiManager)
        {
            base.ResetStateParameters(aiManager);
            SetFieldOfViewDetails(aiManager, aiManager.enemyDetectionScript.originalViewRadius, aiManager.enemyDetectionScript.originalViewAngle);
        }

        private void SetFieldOfViewDetails(AIManager aiManager, float fieldOfViewRadius, float fieldOfViewAngle)
        {
            aiManager.enemyDetectionScript.viewAngle = fieldOfViewAngle;
            aiManager.enemyDetectionScript.viewRadius = fieldOfViewRadius;
        }
    }
}
