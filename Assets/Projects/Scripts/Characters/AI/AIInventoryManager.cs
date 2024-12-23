using UnityEngine;

namespace Creotly_Studios
{
    public class AIInventoryManager : CharacterInventoryManager
    {
        AIManager aIManager;

        protected override void Awake()
        {
            base.Awake();
            aIManager = characterManager as AIManager;
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void SetDefaultWeapons()
        {
            if(aIManager.enemyType == EnemyType.Mech)
            {
                activeWeapons.Add(PrimaryWeapon);
                activeWeapons.Add(Grenade);
                return;
            }

            base.SetDefaultWeapons();

            SetWeaponParent(PrimaryWeapon, inactiveWeaponSpawnPoint[0]);
            PrimaryWeapon.inactiveWeaponHolder = inactiveWeaponSpawnPoint[0];
            
            activeWeapons.Add(PrimaryWeapon);
            activeWeapons.Add(Grenade);
        }

        protected override void SetCurrentWeapon(int selectedWeaponOption)
        {
            base.SetCurrentWeapon(selectedWeaponOption);

            GunWeaponManager gunWeaponManager = currentWeaponManager as GunWeaponManager;
            if(gunWeaponManager != null)
            {
                aIManager.aICombatManager.aimTransform = gunWeaponManager.MuzzlePoint;
            }
        }
    }
}
