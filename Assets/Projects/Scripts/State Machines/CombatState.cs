using UnityEngine;

namespace Creotly_Studios
{
    [CreateAssetMenu(fileName = "Combat State", menuName = "Creotly Studio/AIStates/CombatState")]
    public class CombatState : AIState
    {
        private GunWeaponManager gunWeapon;
        private WeaponManager currentWeapon;

        //Strafing Parameters
        private float stoppingDistance;
        private float verticalMovement;
        private float horizontalMovement;
        public float maximumEngagementDistance = 5.0f;
        [HideInInspector] public bool setStrafingDirection;

        public override AIState AISate_Updater(AIManager aiManager)
        {
            currentWeapon = aiManager.characterInventoryManager.currentWeaponManager;
            if(aiManager.isLockedIn == false && currentWeapon.weaponType == WeaponType.Guns)
            {
                aiManager.isLockedIn = true;
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
            aiManager.aILocomotionManager.RotateTowardsTarget();
            HandleReloading(aiManager);

            if(aiManager.target.targetType != TargetType.Visual)
            {
                return SwitchState(aiManager.patrolState, aiManager);
            }
            return this;
        }

        protected override AIState HumanoidEnemy_Updater(AIManager aiManager)
        {
            if(aiManager.isMoving != true)
            {
                if(aiManager.AngleOfTarget < aiManager.angleLimit.lowerBound || aiManager.AngleOfTarget > aiManager.angleLimit.upperBound)
                {
                    aiManager.aILocomotionManager.PivotTowardsTarget(aiManager);
                }
            }
            aiManager.aILocomotionManager.RotateTowardsTarget();

            ThrowGrenade(aiManager);
            HandleReloading(aiManager);

            if(aiManager.target.targetType != TargetType.Visual)
            {
                return SwitchState(aiManager.patrolState, aiManager);
            }

            if(setStrafingDirection != true)
            {
                HandleStrafing(aiManager.DistanceToTarget);
            }
            return this;
        }

        public void HandleStrafing(float distance)
        {
            setStrafingDirection = true;
            horizontalMovement = RandomValue();
            verticalMovement = distance / maximumEngagementDistance;
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

            if(aIManager.characterInventoryManager.grenadesLeft > 0 && random >= aIManager.aICombatManager.grenadeThrowChance)
            {
                aIManager.aICombatManager.ThrowGrenade();
            }
        }

        private void HandleReloading(AIManager aIManager)
        {
            gunWeapon = currentWeapon as GunWeaponManager;
            if(gunWeapon != null)
            {
                aIManager.canReload = (gunWeapon.bulletLeft <= 0);
            }
        }

        protected override void ResetStateParameters(AIManager aIManager)
        {
            aIManager.isLockedIn = false;
            base.ResetStateParameters(aIManager);
        }
    }
}
