using TMPro;
using UnityEngine;

namespace Creotly_Studios
{
    public class WeaponDetails : MonoBehaviour
    {
        private WeaponDataHolder dataHolder;

        [field: Header("Status")]
        [field: SerializeField] public WeaponType weaponType {get; private set;}

        [field: Header("Parameters")]
        [field: SerializeField] public TextMeshProUGUI damageValueText {get; private set;}
        [field: SerializeField] public TextMeshProUGUI magazineSizeText {get; private set;}
        [field: SerializeField] /*Bullets Left for guns*/ public TextMeshProUGUI quantityText {get; private set;}

        public void WeaponDetails_Update(PlayerManager playerManager)
        {
            dataHolder = playerManager.characterInventoryManager.currentWeaponManager.weaponDataHolder;

            SetQuantityAndMagazineSize();
            damageValueText.text = "Damage Value:  " + dataHolder.damageValue;
        }

        private void SetQuantityAndMagazineSize()
        {
            if(weaponType == WeaponType.Guns)
            {
                quantityText.text = "Bullets Left:  " + dataHolder.bulletLeft;
                magazineSizeText.text = "Magazine Size:  " + dataHolder.magazineSize;
                return;
            }
            quantityText.text = "Quantity:  " + dataHolder.quantity;
        }
    }
}
