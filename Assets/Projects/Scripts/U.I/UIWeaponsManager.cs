using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Creotly_Studios
{
    public class UIWeaponsManager : MonoBehaviour
    {
        //Managers
        PlayerManager playerManager;

        //Weapon Details
        private WeaponDetails gunDetails;
        private WeaponDetails otherDetails;
        private WeaponDetails currentDetail;

        [Header("Weapon Details")]
        [SerializeField] private Image weaponSprite;
        [SerializeField] private WeaponDetails[] weaponDetailHolders;
        [field: SerializeField] public TextMeshProUGUI weaponName {get; private set;}

        private void Awake()
        {
            WeaponDetailSetters();
            playerManager = GetComponentInParent<PlayerManager>();
        }

        private void WeaponDetailSetters()
        {
            weaponDetailHolders = GetComponentsInChildren<WeaponDetails>();

            foreach(var weaponDetailHolder in weaponDetailHolders)
            {
                if(weaponDetailHolder.weaponType == WeaponType.Guns)
                {
                    gunDetails = weaponDetailHolder;
                    continue;
                }
                otherDetails = weaponDetailHolder;
            }
        }

        public void UIWeaponManager_Update()
        {
            WeaponManager currentWeapon = playerManager.characterInventoryManager.currentWeaponManager;

            //SetDetail
            if(currentWeapon == null)
            {
                return;
            }
            weaponSprite.sprite = currentWeapon.weaponImage;
            weaponName.text = "Name:  " + currentWeapon.weaponName;
            currentDetail = (currentWeapon.weaponType == WeaponType.Guns) ? gunDetails : otherDetails;

            gunDetails.gameObject.SetActive(currentDetail == gunDetails);
            otherDetails.gameObject.SetActive(currentDetail == otherDetails);

            currentDetail.WeaponDetails_Update(playerManager);
        }
    }
}
