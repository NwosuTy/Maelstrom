using UnityEngine;

namespace Creotly_Studios
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        protected PlayerManager playerManager;

        protected override void Awake()
        {
            base.Awake();
            playerManager = characterManager as PlayerManager;
        }

        // Update is called once per frame
        public override void CharacterCombatManager_Update(float delta)
        {
            playerManager.isAttacking = playerManager.playerInputManager.attackInput;

            ThrowGrenade();
            HandleGun(delta);
        }

        public override void ThrowGrenadePhysics()
        {
            base.ThrowGrenadePhysics();
            playerManager.playerUIManager.weaponsManager.UpdateGrenadeCountUI(grenadesLeft);
        }

        private void HandleGun(float delta)
        {
            if(playerManager.isAttacking != true || playerManager.playerInventoryManager.currentWeaponManager == null)
            {
                return;
            }

            GunWeaponManager gun = playerManager.playerInventoryManager.currentWeaponManager as GunWeaponManager;
            if (gun == null)
            {
                return;
            }
            gun.HandleShooting(playerManager.crossHairTransform.position, delta);
        }
    }
}
