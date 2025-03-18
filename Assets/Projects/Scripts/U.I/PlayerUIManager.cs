using UnityEngine;
using UnityEngine.UI;

namespace Creotly_Studios
{
    public class PlayerUIManager : MonoBehaviour
    {
        PlayerManager playerManager;
        WeaponManager currentWeapon;

        [Header("UI Components")]
        [SerializeField] private Image crossHairImage;
        [SerializeField] private UITimeStamp timeStamp;
        [field: SerializeField] public UIWeaponsManager weaponsManager { get; private set; }

        [field: Header("UI Bar")]
        private UIBar[] uiBars;
        [field: SerializeField] public UIBar healthBar {get; private set;}
        [field: SerializeField] public UIBar staminaBar {get; private set;}

        private void Awake()
        {
            playerManager = GetComponentInParent<PlayerManager>();

            SetUIBar();
            timeStamp = GetComponentInChildren<UITimeStamp>();
            weaponsManager = GetComponentInChildren<UIWeaponsManager>();
        }

        public void SetWeaponDetails(WeaponManager currentWeapon)
        {
            weaponsManager.SetWeaponParameters(currentWeapon);
        }

        public void PlayerUIManager_Update(float delta)
        {
            SetCrossHairImage();
            timeStamp.UITimeStamp_Updater(delta);
        }

        public void SetCrossHairImage()
        {
            currentWeapon = playerManager.characterInventoryManager.currentWeaponManager;

            crossHairImage.gameObject.SetActive(currentWeapon != null && currentWeapon.weaponType == WeaponType.Guns);
            crossHairImage.sprite = (playerManager.isLockedIn == true) ? playerManager.aimingCrossHairImage : playerManager.crossHairImage;
        }

        private void SetUIBar()
        {
            uiBars = GetComponentsInChildren<UIBar>();
            foreach(var bar in uiBars)
            {
                if(bar.name == "Health Bar")
                { healthBar = bar; }

                if(bar.name == "Stamina Bar")
                { staminaBar = bar; }
            }
        }
    }
}
