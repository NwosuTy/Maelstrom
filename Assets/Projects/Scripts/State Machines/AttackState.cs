using UnityEngine;

namespace Creotly_Studios
{
    [CreateAssetMenu(fileName = "Attack State", menuName = "Creotly Studio/AIStates/Attack State")]
    public class AttackState : AIState
    {
        public float maxCoolDownTimer;
        private GunWeaponManager gunWeapon;
        private WeaponManager currentWeapon;

        public override AIState AISate_Updater(AIManager aIManager)
        {
            aIManager.coolDownTimer += Time.deltaTime;
            currentWeapon = aIManager.characterInventoryManager.currentWeaponManager;

            aIManager.isAttacking = true;
            gunWeapon = currentWeapon as GunWeaponManager;

            if(aIManager.target.visualTarget == null || aIManager.target.visualTarget.isDead)
            {
                return SwitchState(aIManager.patrolState, aIManager);
            }

            aIManager.aILocomotionManager.HandleRotationWhileAttacking(aIManager);
            aIManager.aIAnimationManager.SetBlendTreeParameter(0f,0f,false,Time.deltaTime);

            if(aIManager.performingAction)
            {
                return this;
            }

            if(gunWeapon != null && gunWeapon.bulletLeft <= 0)
            {
                return SwitchState(aIManager.combatState, aIManager);
            }

            if(aIManager.coolDownTimer >= maxCoolDownTimer)
            {
                aIManager.coolDownTimer = maxCoolDownTimer;
                return SwitchState(aIManager.combatState, aIManager);
            }
            return this;
        }

        protected override void ResetStateParameters(AIManager aIManager)
        {
            aIManager.isAttacking = false;
        }
    }
}
