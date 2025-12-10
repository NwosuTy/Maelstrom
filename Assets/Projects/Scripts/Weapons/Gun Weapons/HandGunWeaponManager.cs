using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Creotly_Studios
{
    public class HandGunWeaponManager : GunWeaponManager
    {
        private bool isReloading;
        private UIWeaponsManager playerWeaponUI;
        [SerializeField] private GunRecoil gunRecoil;

        //Bullet Parameters
        public int bulletLeft { get; private set; }

        [field: Header("Bullet Statistics")]
        [field: SerializeField] public int maxBullet { get; private set; }
        [field: SerializeField] public int ReserveAmmo { get; private set; }

        [Header("Cross Hair Properties")]
        [SerializeField] private Sprite crossHairImage;
        [SerializeField] private Sprite aimingCrossHairImage;

        [Header("Gun Parameters")]
        [SerializeField] private GunType gunType = GunType.AssaultRifle;
        [field: SerializeField] public Transform MuzzlePoint { get; private set; }

        public override void Initialize(CharacterManager cm)
        {
            base.Initialize(cm);
            if (playerManager != null)
            {
                playerManager.crossHairImage = crossHairImage;
                playerManager.aimingCrossHairImage = aimingCrossHairImage;              
                playerWeaponUI = playerManager.playerUIManager.weaponsManager;
                gunRecoil.Initialize(this, playerManager.playerLocomotionManager.cameraObject);
            }
        }

        public override void WeaponManager_Update(float delta)
        {
            if(hasBeenInitialized != true)
            {
                return;
            }
            if(aiManager != null)   {   aiManager.canReload = (bulletLeft <= 0);    }
            isReloading = characterManager.animator.GetBool(AnimatorHashNames.isReloadingHash);

            HandleReloading();
            UpdateBullet(delta);
            base.WeaponManager_Update(delta);
        }

        public override void HandleShooting(Vector3 targetPosition, float delta)
        {
            if(isReloading == true || bulletLeft <= 0)
            {
                return;
            }
            base.HandleShooting(targetPosition, delta);
            if (playerManager != null) { gunRecoil.GenerateRecoil(delta); }
        }

        protected override void FireBullet(Vector3 targetPosition)
        {
            Vector3 velocity = (targetPosition - MuzzlePoint.position).normalized * bulletSpeed;
            Bullet bullet = CreateBullet(MuzzlePoint.position, velocity);

            bulletLeft--;
            bulletList.Add(bullet);
            if(playerWeaponUI != null) { playerWeaponUI.UpdateBulletCountUI(bulletLeft, maxBullet); }
        }

        private void HandleReloading()
        {
            if (isReloading || characterManager.performingAction || !characterManager.canReload || bulletLeft >= maxBullet || ReserveAmmo <= 0)
            {
                return;
            }
            characterManager.animator.SetBool(AnimatorHashNames.isReloadingHash, true);
            characterManager.characterAnimationManager.PlayTargetAnimation(AnimatorHashNames.reloadingHash, true);

            int needed = maxBullet - bulletLeft;
            int toAdd = Mathf.Min(ReserveAmmo, needed);

            bulletLeft += toAdd;
            ReserveAmmo -= toAdd;
            if (playerManager != null)
            {
                playerWeaponUI.UpdateMagazineCount(bulletLeft, ReserveAmmo);
            }
        }


        public override void ResetAllStats()
        {
            if (hasReset)
            {
                return;
            }

            hasReset = true;
            base.ResetAllStats();
            bulletLeft = maxBullet;
        }
    }
}
