using UnityEngine;

namespace Creotly_Studios
{
    [CreateAssetMenu(fileName = "Attack State", menuName = "Creotly Studio/AIStates/Attack State")]
    public class AttackState : AIState
    {
        private float attackDuration;

        private float verticalMovement;
        private float horizontalMovement;
        private bool setStrafingDirection;
        public float maximumEngagementDistance = 5.0f;
        [SerializeField] private float maxAttackDuration;

        public override AIState AISate_Updater(AIManager aiManager)
        {
            if (aiManager.DistanceToTarget >= aiManager.navMeshAgent.stoppingDistance * 1.5f)
            {
                return SwitchState(aiManager.pursueState, aiManager);
            }

            attackDuration += Time.deltaTime;
            if (attackDuration >=  maxAttackDuration)
            {
                return SwitchState(aiManager.combatState, aiManager);
            }
            aiManager.isAttacking = true;
            aiManager.aILocomotionManager.HandleRotationWhileAttacking(aiManager);
            
           
            if(aiManager.target.visualTarget == null || aiManager.target.visualTarget.isDead)
            {
                return SwitchState(aiManager.patrolState, aiManager);
            }

            if (aiManager.enemyType == EnemyType.Humanoid)
            {
                HandleStrafing(aiManager);
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

            if (randomValue >= -1.0f && randomValue <= 0.0f)
            {
                return -0.5f;
            }
            return 0.5f;
        }

        protected override void ResetStateParameters(AIManager aiManager)
        {
            attackDuration = 0.0f;
            aiManager.coolDown = true;
            aiManager.isAttacking = false;
        }
    }
}
