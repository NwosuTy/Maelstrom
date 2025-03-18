using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Creotly_Studios
{
    public class HandGunWeaponManager : GunWeaponManager
    {
        private bool isReloading;
        private UIWeaponsManager playerWeaponUI;

        //Bullet Parameters
        public int bulletLeft { get; private set; }

        [Header("Bullet Statistics")]
        [field: SerializeField] public int maxBullet { get; private set; }
        [field: SerializeField] public int MagazineSize { get; private set; }

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
                SetWeaponPose(characterManager.characterAnimatorRigController);
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
            if (isReloading || characterManager.performingAction || characterManager.canReload != true)
            {
                return;
            }

            if (bulletLeft >= maxBullet)
            {
                return;
            }

            if (MagazineSize <= 0)
            {
                MagazineSize = 0;
                return;
            }
            characterManager.animator.SetBool(AnimatorHashNames.isReloadingHash, true);
            characterManager.characterAnimationManager.PlayTargetAnimation(AnimatorHashNames.reloadingHash, true);

            int bulletsNeeded = maxBullet - bulletLeft;
            bulletLeft += bulletsNeeded;
            if (playerManager != null) 
            { 
                MagazineSize -= bulletsNeeded;
                playerWeaponUI.UpdateMagazineCount(bulletLeft, MagazineSize);
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

        #region Animation Rigging
        private void SetWeaponPose(CharacterAnimatorRigController rigController)
        {
            if (gunType == GunType.Handgun || characterManager.characterType == CharacterType.Enemy)
            {
                SetWeaponPoseParameters(0, rigController);
                return;
            }
            SetWeaponPoseParameters(1, rigController);
        }

        private void SetWeaponPoseParameters(int constraintIndex, CharacterAnimatorRigController rigController)
        {
            for (int i = 0; i < rigController.WeaponAimObjects.Length; i++)
            {
                rigController.WeaponAimObjects[i].SetActive(i == constraintIndex);
            }

            for (int i = 0; i < rigController.WeaponParentConstraints.Length; i++)
            {
                MultiParentConstraint parConstraint = rigController.WeaponParentConstraints[i];
                parConstraint.weight = (i == constraintIndex) ? 1f : 0f;
            }

            for (int i = 0; i < rigController.WeaponPositionConstraints.Length; i++)
            {
                MultiPositionConstraint posConstraint = rigController.WeaponPositionConstraints[i];
                posConstraint.weight = (i == constraintIndex) ? 1f : 0f;
            }
            rigController.rigBuilder.Build();
        }
        #endregion
    }
}
