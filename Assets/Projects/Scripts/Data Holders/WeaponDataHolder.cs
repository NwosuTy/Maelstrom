using UnityEngine;

namespace Creotly_Studios
{
    [CreateAssetMenu(fileName = "Weapon Data Holder" , menuName = "Creotly/WeaponDataHolder" )]
    public class WeaponDataHolder : ScriptableObject
    {
        [field: Header("Weapon Parameters")]
        [field: SerializeField] public float damageValue {get; private set;}

        //Others
        [field: SerializeField] public int quantity {get; private set;}

        //Gun
        [field: SerializeField] public int bulletLeft {get; private set;}
        [field: SerializeField] public int magazineSize {get; private set;}

        public void UpdateWeaponParameters(WeaponManager weaponManager)
        {
            damageValue = weaponManager.damageValue;

            if(weaponManager.weaponType == WeaponType.Guns)
            {
                UpdateGuns(weaponManager);
                return;
            }
            UpdateOthers(weaponManager);
        }

        public void UpdateGuns(WeaponManager weaponManager)
        {
            GunWeaponManager gun = weaponManager as GunWeaponManager;
            if(gun != null)
            {
                bulletLeft = gun.bulletLeft;
                magazineSize = gun.MagazineSize;
            }
        }

        public void UpdateOthers(WeaponManager weaponManager)
        {
            if(weaponManager.weaponType == WeaponType.Grenade)
            {
                quantity = weaponManager.characterManager.characterInventoryManager.grenadesLeft;
                return;
            }
            quantity = 1;
        }
    }
}
