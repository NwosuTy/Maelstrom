using UnityEngine;
using System.Collections.Generic;

namespace Creotly_Studios
{
    public class CharacterInventoryManager : MonoBehaviour
    {
        protected CharacterManager characterManager;

        //Weapons
        protected WeaponManager previousWeaponManager;
        public WeaponManager currentWeaponManager {get; protected set;}

        [Header("Active Weapons")]
        public int grenadesLeft;
        protected int selectedOption;
        public List<WeaponManager> activeWeapons = new();

        [field: Header("Parameters")]
        [SerializeField] protected Transform[] inactiveWeaponSpawnPoint;
        [field: SerializeField] public Transform ActiveWeaponSpawnPoint {get; protected set;}

        [field: Header("Weapon Details")]
        [field: SerializeField] public WeaponManager PrimaryWeapon {get; protected set;}
        [field: SerializeField] public GrenadeWeaponManager Grenade {get; protected set;}

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }

        protected virtual void Start()
        {
            selectedOption = 0;
            SetDefaultWeapons();

            SetCurrentWeapon(selectedOption);
            SetWeaponParent(currentWeaponManager, ActiveWeaponSpawnPoint);
        }

        public virtual void CharacterInventory_Updater(float delta)
        {
            if(currentWeaponManager != null)
            {
                currentWeaponManager.WeaponManager_Update(delta);
            }
        }

        protected virtual void SetDefaultWeapons()
        {
            Grenade = Instantiate(Grenade);
            PrimaryWeapon = Instantiate(PrimaryWeapon);

            Grenade.SetPhysicsSystem(false);
            PrimaryWeapon.SetPhysicsSystem(false);

            Grenade.ResetAllStats();
            PrimaryWeapon.ResetAllStats();
            Grenade.gameObject.SetActive(false);

            Grenade.inactiveWeaponHolder = ActiveWeaponSpawnPoint;
            SetWeaponParent(Grenade, ActiveWeaponSpawnPoint);

            characterManager.characterCombatManager.grenadeObject = Grenade;
        }

        protected void SwapWeapon(int selectedWeaponOption)
        {
            SetCurrentWeapon(selectedWeaponOption);
           
            if(previousWeaponManager == null)
            {
                characterManager.characterAnimationManager.PlayTargetAnimation(AnimatorHashNames.equipWeapon, true);
                return;
            }
            characterManager.characterAnimationManager.PlayTargetAnimation(AnimatorHashNames.unEquipWeapon, true);
        }

        protected virtual void SetCurrentWeapon(int selectedWeaponOption)
        {
            previousWeaponManager = currentWeaponManager;
            currentWeaponManager = activeWeapons[selectedWeaponOption];
        }

        public void EquipWeapon()
        {
            if(currentWeaponManager == Grenade)
            {
                Grenade.gameObject.SetActive(true);
                currentWeaponManager.Initialize(characterManager);
                return;
            }
            SetWeaponParent(currentWeaponManager, ActiveWeaponSpawnPoint);
            currentWeaponManager.Initialize(characterManager);
        }

        public void UnEquipWeapon()
        {
            if(previousWeaponManager == Grenade)
            {
                Grenade.gameObject.SetActive(false);
            }
            SetWeaponParent(previousWeaponManager, previousWeaponManager.inactiveWeaponHolder);
        }

        protected void SetWeaponParent(WeaponManager weapon, Transform parent)
        {
            if(weapon == null || weapon.partOfBody == true)
            {
                return;
            }
            weapon.transform.SetParent(parent);
            weapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}
