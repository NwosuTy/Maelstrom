using UnityEngine;
using System.Collections.Generic;

namespace Creotly_Studios
{
    public class AIInventoryManager : CharacterInventoryManager
    {
        AIManager aiManager;
        public bool hasEquippedWeapon;

        private MechGunWeaponManager mechGun;

        protected override void Awake()
        {
            base.Awake();
            aiManager = characterManager as AIManager;
        }

        protected override void Start()
        {
            if (aiManager.enemyType == EnemyType.Mech)
            {
                mechGun = GetComponentInChildren<MechGunWeaponManager>();

                currentWeaponManager = mechGun;
                currentWeaponManager.Initialize(characterManager);
                return;
            }

            SetDefaultWeapons();
            SetCurrentWeapon(0);
        }

        public override void CharacterInventory_Updater(float delta)
        {
            if(aiManager.enemyType == EnemyType.Mech)
            {
                mechGun.WeaponManager_Update(delta);
                return;
            }
            base.CharacterInventory_Updater(delta);
        }

        protected override void SetDefaultWeapons()
        {
            PrimaryWeapon = Instantiate(PrimaryWeapon);

            PrimaryWeapon.ResetAllStats();
            PrimaryWeapon.SetPhysicsSystem(false);

            SetWeaponParent(PrimaryWeapon, inactiveWeaponSpawnPoint[0]);
            PrimaryWeapon.inactiveWeaponHolder = inactiveWeaponSpawnPoint[0];

            activeWeapons.Add(null);
            activeWeapons.Add(PrimaryWeapon);
        }

        public void HandleSetWeapon(int weaponIndex)
        {
            SetCurrentWeapon(weaponIndex);
        }

        protected override void SetCurrentWeapon(int selectedWeaponOption)
        {
            base.SetCurrentWeapon(selectedWeaponOption);
            if (currentWeaponManager == null)
            {
                aiManager.aIAnimationManager.Unarmed_StandOrCrouch();
                return;
            }
        }

        public override void UnEquipWeapon()
        {
            SetWeaponParent(previousWeaponManager, previousWeaponManager.inactiveWeaponHolder);
            hasEquippedWeapon = false;
        }

        public override void EquipWeapon()
        {
            SetWeaponParent(currentWeaponManager, ActiveWeaponSpawnPoint);
            currentWeaponManager.Initialize(characterManager);
        }
    }
}
