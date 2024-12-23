using UnityEngine;

namespace Creotly_Studios
{
    public class PlayerInventoryManager : CharacterInventoryManager
    {
        PlayerManager playerManager;

        [field: SerializeField] public GunWeaponManager SecondaryWeapon {get; protected set;}

        protected override void Awake()
        {
            base.Awake();
            playerManager = characterManager as PlayerManager;
        }

        protected override void Start()
        {
            base.Start();
        }

        public override void CharacterInventory_Updater(float delta)
        {
            WeaponChange();
            base.CharacterInventory_Updater(delta);
        }

        protected override void SetDefaultWeapons()
        {
            base.SetDefaultWeapons();
            SecondaryWeapon = Instantiate(SecondaryWeapon);

            SecondaryWeapon.SetPhysicsSystem(false);

            SecondaryWeapon.ResetAllStats();

            SetWeaponParent(PrimaryWeapon, inactiveWeaponSpawnPoint[0]);
            SetWeaponParent(SecondaryWeapon, inactiveWeaponSpawnPoint[1]);

            PrimaryWeapon.inactiveWeaponHolder = inactiveWeaponSpawnPoint[0];
            SecondaryWeapon.inactiveWeaponHolder = inactiveWeaponSpawnPoint[1];
            
            activeWeapons.Add(null);
            activeWeapons.Add(PrimaryWeapon);
            activeWeapons.Add(SecondaryWeapon);
            activeWeapons.Add(Grenade);
        }

        private void WeaponChange()
        {
            if(playerManager.playerInputManager.swapWeaponInput != true)
            {
                return;
            }

            PlayerSwapWeapon();
            SwapWeapon(selectedOption);
            
            if(currentWeaponManager == Grenade && grenadesLeft <= 0)
            {
                selectedOption = 0;
                SetCurrentWeapon(selectedOption);
                Grenade.gameObject.SetActive(false);
                SetWeaponParent(currentWeaponManager, ActiveWeaponSpawnPoint);
            }
        }

        private void PlayerSwapWeapon()
        {
            selectedOption++;

            if(selectedOption >= activeWeapons.Count)
            {
                selectedOption = 0;
            }

            if(selectedOption >= activeWeapons.Count - 1 && grenadesLeft <= 0)
            {
                grenadesLeft = 0;
                selectedOption = 0;
            }
        }

        protected override void SetCurrentWeapon(int selectedWeaponOption)
        {
            base.SetCurrentWeapon(selectedWeaponOption);

            if(currentWeaponManager == null)
            {
                playerManager.playerAnimationManager.Unarmed_StandOrCrouch();
                return;
            }
        }
    }
}
