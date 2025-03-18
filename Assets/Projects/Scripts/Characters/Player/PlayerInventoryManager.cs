using UnityEngine;

namespace Creotly_Studios
{
    public class PlayerInventoryManager : CharacterInventoryManager
    {
        int index;
        PlayerManager playerManager;

        [field: Header("Parameters")]
        [field: SerializeField] public GunWeaponManager SecondaryWeapon { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            playerManager = characterManager as PlayerManager;
        }

        protected override void Start()
        {
            base.Start();
            SetWeaponParent(currentWeaponManager, ActiveWeaponSpawnPoint);
        }

        public override void CharacterInventory_Updater(float delta)
        {
            WeaponChange();
            if (currentWeaponManager != null)
            {
                currentWeaponManager.WeaponManager_Update(delta);
            }
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
        }

        private void WeaponChange()
        {
            if(playerManager.playerInputManager.swapWeaponInput != true)
            {
                return;
            }

            PlayerSwapWeapon();
            SwapWeapon(selectedOption);
        }

        public override void EquipWeapon()
        {
            base.EquipWeapon();
            SetWeaponParent(currentWeaponManager, ActiveWeaponSpawnPoint);
            currentWeaponManager.Initialize(characterManager);
        }

        private void PlayerSwapWeapon()
        {
            index++;
            selectedOption = index % activeWeapons.Count;
        }

        protected override void SetCurrentWeapon(int selectedWeaponOption)
        {
            base.SetCurrentWeapon(selectedWeaponOption);
            playerManager.playerUIManager.SetWeaponDetails(currentWeaponManager);
            if(currentWeaponManager == null) { playerManager.playerAnimationManager.Unarmed_StandOrCrouch(); }
        }
    }
}
