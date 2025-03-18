using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Creotly_Studios
{
    public class UIWeaponsManager : MonoBehaviour
    {
        [SerializeField] private GameObject weaponDetailObject;

        [Header("Weapon Details")]
        [SerializeField] private Image weaponSprite;
        [SerializeField] private TextMeshProUGUI grenadeCountText;
        [field: SerializeField] public TextMeshProUGUI weaponName { get; private set; }

        [field: Header("Bullet Details")]
        [SerializeField] private TextMeshProUGUI magazineSizeText;
        [SerializeField] private TextMeshProUGUI totalBulletCountText;
        [SerializeField] private TextMeshProUGUI currentBulletCountText;

        public void UpdateGrenadeCountUI(int count)
        {
            grenadeCountText.text = $"Grenade Count: {count}";
        }

        public void SetWeaponParameters(WeaponManager weaponManager)
        {
            if(weaponManager == null)
            {
                weaponDetailObject.SetActive(false);
                return;
            }
            weaponDetailObject.SetActive(true);
            WeaponDataHolder dataHolder = weaponManager.weaponDataHolder;
            dataHolder.UpdateWeaponParameters(weaponManager);

            weaponName.text = weaponManager.weaponName;
            weaponSprite.sprite = weaponManager.weaponImage;
            grenadeCountText.text = dataHolder.quantity.ToString();

            currentBulletCountText.text = dataHolder.bulletLeft.ToString();
            totalBulletCountText.text = dataHolder.totalBulletCount.ToString();
        }

        public void UpdateMagazineCount(int count, int magazineCount)
        {
            currentBulletCountText.text = count.ToString();
            magazineSizeText.text = $"Magazine Count: {magazineCount}";
        }

        public void UpdateBulletCountUI(int count, int maxCount)
        {
            currentBulletCountText.text = count.ToString();
            totalBulletCountText.text = maxCount.ToString();
        }
    }
}
