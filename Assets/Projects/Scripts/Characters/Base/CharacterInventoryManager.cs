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
        protected int selectedOption;
        public List<WeaponManager> activeWeapons = new();

        [field: Header("Weapon Details")]
        [SerializeField] protected Transform[] inactiveWeaponSpawnPoint;
        [field: SerializeField] public WeaponManager PrimaryWeapon {get; protected set;}
        [field: SerializeField] public Transform ActiveWeaponSpawnPoint { get; protected set; }

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }

        protected virtual void Start()
        {
            selectedOption = 0;
            SetDefaultWeapons();
            SetCurrentWeapon(selectedOption);
        }

        protected virtual void SetDefaultWeapons()
        {
            PrimaryWeapon = Instantiate(PrimaryWeapon);
            PrimaryWeapon.SetPhysicsSystem(false);
            PrimaryWeapon.ResetAllStats();
        }

        public virtual void CharacterInventory_Updater(float delta)
        {
            if(currentWeaponManager == null)
            {
                characterManager.characterAnimatorRigController.LeftHandIKConstraint.weight = 0.0f;
                characterManager.characterAnimatorRigController.RightHandIKConstraint.weight = 0.0f;
                return;
            }
            currentWeaponManager.WeaponManager_Update(delta);
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

        public virtual void EquipWeapon()
        {

        }

        public virtual void UnEquipWeapon()
        {
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
