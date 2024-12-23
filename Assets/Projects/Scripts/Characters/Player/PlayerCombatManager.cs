using UnityEngine;

namespace Creotly_Studios
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        protected PlayerManager playerManager;
        
        //Weapons
        private bool canThrowGrenade;
        private WeaponManager currentWeapon;

        protected override void Awake()
        {
            base.Awake();
            playerManager = characterManager as PlayerManager;
        }

        // Update is called once per frame
        public override void CharacterCombatManager_Update()
        {
            canThrowGrenade = playerManager.playerInputManager.tapShootInput;
            currentWeapon = playerManager.characterInventoryManager.currentWeaponManager;

            ThrowGrenade();
        }

        public override void ThrowGrenade()
        {
            if(playerManager.performingAction)
            {
                return;
            }
    
            if(currentWeapon != grenadeObject || canThrowGrenade != true)
            {
                return;
            }
            base.ThrowGrenade();
        }
    }
}
