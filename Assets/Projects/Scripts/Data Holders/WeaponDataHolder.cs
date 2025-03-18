using UnityEngine;

namespace Creotly_Studios
{
    [CreateAssetMenu(fileName = "Weapon Data Holder" , menuName = "Creotly/WeaponDataHolder" )]
    public class WeaponDataHolder : ScriptableObject
    {
        [field: Header("Weapon Parameters")]
        //Others
        [field: SerializeField] public int quantity {get; private set;}

        //Gun
        [field: SerializeField] public int bulletLeft {get; private set;}
        [field: SerializeField] public int totalBulletCount {get; private set;}

        public void UpdateWeaponParameters(WeaponManager weaponManager)
        {
            if(weaponManager.weaponType == WeaponType.Guns)
            {
                UpdateGuns(weaponManager);
                return;
            }
            UpdateOthers(weaponManager);
        }

        public void UpdateGuns(WeaponManager weaponManager)
        {
            HandGunWeaponManager gun = weaponManager as HandGunWeaponManager;
            if(gun != null)
            {
                bulletLeft = gun.bulletLeft;
                totalBulletCount = gun.maxBullet;
            }
        }

        public void UpdateOthers(WeaponManager weaponManager)
        {
            if(weaponManager.weaponType == WeaponType.Grenade)
            {
                quantity = weaponManager.characterManager.characterCombatManager.grenadesLeft;
                return;
            }
            quantity = 1;
        }
    }
}
