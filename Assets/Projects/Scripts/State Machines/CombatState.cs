using UnityEngine;

namespace Creotly_Studios
{
    [CreateAssetMenu(fileName = "Combat State", menuName = "Creotly Studio/AIStates/CombatState")]
    public class CombatState : AIState
    {
        private bool isBulletSmall;

        //Strafing Parameters
        private float stoppingDistance;
        private float verticalMovement;
        private float horizontalMovement;
        private bool setStrafingDirection;
        public float maximumEngagementDistance = 5.0f;
        
        public override AIState AISate_Updater(AIManager aiManager)
        {
            aiManager.isLockedIn = true;
            if(aiManager.dontMove != true && aiManager.navMeshAgent.enabled == false)
            {
                aiManager.navMeshAgent.enabled = true;
            }
            
            if(aiManager.DistanceToTarget >= aiManager.navMeshAgent.stoppingDistance * 1.5f)
            {
                return SwitchState(aiManager.pursueState, aiManager);
            }
            return HandleAction(aiManager);
        }

        protected AIState HandleAction(AIManager aiManager)
        {
            isBulletSmall = false;
            if (aiManager.enemyType == EnemyType.Humanoid)
            {
                //ThrowGrenade(aiManager);
                HandleStrafing(aiManager);
            }
            aiManager.aILocomotionManager.HandleRotationWhileAttacking(aiManager);

            if (aiManager.target.targetType != TargetType.Visual)
            {
                return SwitchState(aiManager.patrolState, aiManager);
            }

            if (aiManager.coolDown == false && isBulletSmall != true)
            {
                return SwitchState(aiManager.attackState, aiManager);
            }

            if (aiManager.enemyType == EnemyType.Humanoid)
            {
                aiManager.isMoving = true;
                aiManager.aIAnimationManager.SetBlendTreeParameter(verticalMovement, horizontalMovement, false, Time.deltaTime);
            }
            return this;
        }

        public void HandleStrafing(AIManager aiManager)
        {
            if (setStrafingDirection == true)
            {
                return;
            }

            setStrafingDirection = true;
            horizontalMovement = RandomValue();
            verticalMovement = aiManager.DistanceToTarget / maximumEngagementDistance;
        }

        private float RandomValue()
        {
            float randomValue = Random.Range(-1, 1);

            if(randomValue >= -1.0f && randomValue <= 0.0f)
            {
                return -0.5f;
            }
            return 0.5f;
        }

        public void CheckIFTooClose(AIManager aiManager)
        {
            aiManager.isMoving = true;

            if(aiManager.DistanceToTarget <= stoppingDistance)
            {
                aiManager.aIAnimationManager.SetBlendTreeParameter(horizontalMovement, -0.5f, false, Time.deltaTime);
            }
            if(aiManager.DistanceToTarget >= stoppingDistance && aiManager.DistanceToTarget <= maximumEngagementDistance - 1.0f)
            {
                aiManager.aIAnimationManager.SetBlendTreeParameter(horizontalMovement, 0f, false, Time.deltaTime);
            }
            else
            {
                aiManager.aIAnimationManager.SetBlendTreeParameter(horizontalMovement, verticalMovement, false, Time.deltaTime);
            }
            setStrafingDirection = false;
        }

        private void ThrowGrenade(AIManager aIManager)
        {
            int random = Random.Range(0, 100);

            if(random >= aIManager.aICombatManager.grenadeThrowChance)
            {
                aIManager.canThrowGrenade = true;
            }
        }

        protected override void ResetStateParameters(AIManager aIManager)
        {
            aIManager.isLockedIn = false;
            base.ResetStateParameters(aIManager);
        }
    }
}
