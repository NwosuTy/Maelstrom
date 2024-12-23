using UnityEngine;
using UnityEngine.AI;

namespace Creotly_Studios
{
    [CreateAssetMenu(fileName = "Patrol State", menuName = "Creotly Studio/AIStates/PatrolState")]
    public class PatrolState : AIState
    {
        //Private
        private float idleTime = 7.5f;

        [Header("General Parameters")]
        public float sphereRadius = 5.0f;

        [Header("Time")]
        public float idleTimeDefault = 7.5f;
        public float interactTimeDefault = 10.0f;

        [Header("Status")]
        private bool destinationSet;
        public PatrolMode patrolMode = PatrolMode.Idle;

        public override AIState AISate_Updater(AIManager aIManager)
        {
            if(aIManager.performingAction)
            {
                aIManager.aIAnimationManager.SetBlendTreeParameter(0f, 0f, false, Time.deltaTime);
                return this;
            }

            if(aIManager.navMeshAgent.enabled == false)
            {
                aIManager.navMeshAgent.enabled = true;
            }

            if(aIManager.target.visualTarget != null || aIManager.target.audioTarget != null)
            {
                return SwitchState(aIManager.pursueState, aIManager);
            }

            if(aIManager.enemyType == EnemyType.Mech)
            {
                return MechEnemy_Updater(aIManager);
            }
            return HumanoidEnemy_Updater(aIManager);
        }

        protected override AIState MechEnemy_Updater(AIManager aiManager)
        {
            aiManager.aILocomotionManager.RotateTowardsTarget();
            if(patrolMode == PatrolMode.Idle)
            {
                return Idle(aiManager);
            }
            return Walk(aiManager);
        }

        protected override AIState HumanoidEnemy_Updater(AIManager aiManager)
        {
            if(aiManager.AngleOfTarget < aiManager.angleLimit.lowerBound || aiManager.AngleOfTarget > aiManager.angleLimit.upperBound)
            {
                aiManager.aILocomotionManager.PivotTowardsTarget(aiManager);
            }
            
            aiManager.aILocomotionManager.RotateTowardsTarget();
            if(patrolMode == PatrolMode.Idle)
            {
                return Idle(aiManager);
            }
            return Walk(aiManager);
        }

        protected override void ResetStateParameters(AIManager aIManager)
        {
            idleTime = idleTimeDefault;
            base.ResetStateParameters(aIManager);
            SetFieldOfViewDetails(aIManager, aIManager.enemyDetectionScript.originalViewRadius, aIManager.enemyDetectionScript.originalViewAngle);
        }

        private void SetFieldOfViewDetails(AIManager aIManager, float fieldOfViewRadius, float fieldOfViewAngle)
        {
            aIManager.enemyDetectionScript.viewAngle = fieldOfViewAngle;
            aIManager.enemyDetectionScript.viewRadius = fieldOfViewRadius;
        }


        //Functionalities

        private AIState Idle(AIManager aiManager)
        {
            idleTime -= Time.deltaTime;
            aiManager.isMoving = false;
            aiManager.characterAnimationManager.SetBlendTreeParameter(0f, 0f, false, Time.deltaTime);

            if(idleTime <= 0.0f)
            {
                if(destinationSet != true)
                {
                    SetDestination(aiManager);
                }

                if(destinationSet)
                {
                    destinationSet = false;
                    patrolMode = PatrolMode.Walk;
                    return SwitchState(Walk(aiManager), aiManager);
                }
            }
            return this;
        }

        private AIState Walk(AIManager aiManager)
        {
            aiManager.aILocomotionManager.RotateTowardsTarget();
            if(aiManager.DistanceToTarget >= aiManager.navMeshAgent.stoppingDistance)
            {
                aiManager.characterAnimationManager.SetBlendTreeParameter(0f, 0.55f, false, Time.deltaTime);
                aiManager.aILocomotionManager.HandleMovement(enemyDestination, aiManager.aILocomotionManager.movementSpeed);
            }
            else
            {
                patrolMode = PatrolMode.Idle;
                aiManager.navMeshAgent.enabled = false;
                return SwitchState(Idle(aiManager), aiManager);
            }
            return this;
        }

        private void SetDestination(AIManager aiManager)
        {
            Vector3 randomPoint = Random.insideUnitSphere * sphereRadius + aiManager.transform.position;
            
            NavMeshHit navMeshHit;
            if(NavMesh.SamplePosition(randomPoint, out navMeshHit, sphereRadius, NavMesh.AllAreas))
            {
                Debug.Log(0);
                enemyDestination = navMeshHit.position;
                aiManager.SetPersonalTargetDetails(enemyDestination);
                destinationSet = true;
            }
            else
            {
                Debug.Log(1);
                destinationSet = false;
            }
        }
    }
}
