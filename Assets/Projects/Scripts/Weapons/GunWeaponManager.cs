using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Creotly_Studios
{
    public class GunWeaponManager : WeaponManager
    {
        //Private Parameters
        private bool isReloading;
        private bool coolDown;

        //Raycast Parameters
        Ray ray;
        private RaycastHit raycastHit;

        //Private Bullet Parameters
        private GameObject bulletHoles;
        private float timeBeforeRelease;
        private Transform crossHairTransform;
        public int bulletLeft {get; private set;}

        [Header("Status")]
        [SerializeField] private bool canHoldShooting;
        [SerializeField] private GunType gunType = GunType.AssaultRifle;

        [Header("Gun Stats")]
        [SerializeField] private float range;
        [SerializeField] private float spread;
        [SerializeField] private float timeBetweenShots;

        [Header("Gun Parameters")]
        [SerializeField] private int maxBullet;
        [SerializeField] private ParticleSystem[] muzzleFlash;
        [SerializeField] private TrailRenderer bulletPathTrail;
        [field: SerializeField] public int MagazineSize {get; private set;}
        [field: SerializeField] public Transform MuzzlePoint {get; private set;}

        [Header("Cross Hair Properties")]
        [SerializeField] private Sprite crossHairImage;
        [SerializeField] private Sprite aimingCrossHairImage;

        public override void Initialize(CharacterManager cm)
        {
            base.Initialize(cm);
            coolDown = false;

            if(playerManager != null)
            {
                playerManager.crossHairImage = crossHairImage;
                crossHairTransform = playerManager.crossHairTransform;
                playerManager.aimingCrossHairImage = aimingCrossHairImage;
            }
            SetWeaponPose(cm.characterAnimationRigController);
            hasBeenInitialized = true;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        private void SetWeaponPose(CharacterAnimationRigController rigController)
        {
            if(gunType == GunType.Handgun)
            {
                SetWeaponPoseParameters(0, rigController);
                return;
            }
            SetWeaponPoseParameters(1, rigController);
        }

        private void SetWeaponPoseParameters(int constraintIndex, CharacterAnimationRigController rigController)
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

            for(int i = 0; i < rigController.WeaponPositionConstraints.Length; i++)
            {
                MultiPositionConstraint posConstraint = rigController.WeaponPositionConstraints[i];
                posConstraint.weight = (i == constraintIndex) ? 1f : 0f;
            }
            rigController.rigBuilder.Build();
        }

        public override void ResetAllStats()
        {
            if(hasReset)
            {
                return;
            }

            hasReset = true;
            base.ResetAllStats();
            bulletLeft = maxBullet;
        }

        public override void WeaponManager_Update(float delta)
        {
            if(hasBeenInitialized != true)
            {
                return;
            }

            SetInput();
            isReloading = characterManager.animator.GetBool(AnimatorHashNames.isReloadingHash);

            HandleReloading();
            HandleShooting(delta);
            base.WeaponManager_Update(delta);
        }

        private void SetInput()
        {
            if(aIManager != null)
            {
                return;
            }
            //Sets Is Shooting to Hold if shooting can be held else set to Tap
            playerManager.isAttacking = canHoldShooting ? playerManager.playerInputManager.holdShootInput : playerManager.playerInputManager.tapShootInput;
        }

        private void HandleVFX()
        {
            foreach(var particle in muzzleFlash)
            {
                particle.Emit(1);
            }
        }

        private void HandleShooting(float delta)
        {
            if(playerManager != null && playerManager.performingAction)
            {
                return;
            }

            if(coolDown == true || characterManager.isAttacking != true || isReloading == true || bulletLeft <= 0)
            {
                return;
            }
            
            coolDown = true;

            ray.origin = MuzzlePoint.position;
            ray.direction = ShootingRay();

            HandleVFX();
            var tracer = Instantiate(bulletPathTrail, ray.origin, Quaternion.identity);

            tracer.AddPosition(ray.origin);

            if(Physics.Raycast(ray, out raycastHit, range, EnemyLayerMask))
            {
                CharacterStatsManager shotCharacter = raycastHit.collider.GetComponent<CharacterStatsManager>();
                if(shotCharacter != null && shotCharacter.characterManager.characterType != characterManager.characterType)
                {
                    float directionFromHit = Vector3.SignedAngle(characterManager.transform.position, shotCharacter.transform.position, Vector3.up);

                    deathAnimation = GetDeathAnimation(shotCharacter.transform);
                    damageAnimation = AnimatorHashNames.DamageTargetAnimation(directionFromHit);
                    shotCharacter.TakeHealthDamage(damageAnimation, deathAnimation, damageValue);
                }

                if(shotCharacter == null) 
                {
                    InstantiateBulletHoles(raycastHit.collider);
                }
                tracer.transform.position = raycastHit.point;
            }
            else
            {
                tracer.transform.position = ray.direction * range;
            }
            
            bulletLeft--;
            ReleaseHoles(raycastHit.collider, delta);
            Invoke(nameof(ResetCoolDown), timeBetweenShots);
        }

        private void InstantiateBulletHoles(Collider damagedCollider)
        {
            float positionMultiplier = 0.5f;
            float spawnX = raycastHit.point.x - ray.direction.x * positionMultiplier;
            float spawnY = raycastHit.point.y - ray.direction.y * positionMultiplier;
            float spawnZ = raycastHit.point.z - ray.direction.z * positionMultiplier;
            Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);

            bulletHoles = ImpactHole(damagedCollider);
            Quaternion targetRotation = Quaternion.LookRotation(ray.direction);
            bulletHoles.transform.SetPositionAndRotation(spawnPosition, targetRotation);
        }

        private GameObject ImpactHole(Collider damagedCollider)
        {
            if(damagedCollider.CompareTag("Wood"))
            {
                return GameObjectManager.woodBulletHolesPool.Get();
            }
            else if(damagedCollider.CompareTag("Metal"))
            {
                return GameObjectManager.metalBulletHolesPool.Get();
            } 
            return GameObjectManager.cementBulletHolesPool.Get();
        }

        private void ReleaseHoles(Collider damagedCollider, float delta)
        {
            timeBeforeRelease += delta;
            if(timeBeforeRelease >= 3.5f)
            {
                timeBeforeRelease = 0.0f;
                ReleaseHolesDependingOnTag(damagedCollider);
            }
        }

        private void ReleaseHolesDependingOnTag(Collider damagedCollider)
        {
            if(damagedCollider == null)
            {
                return;
            }

            if(damagedCollider.CompareTag("Wood"))
            {
                GameObjectManager.woodBulletHolesPool.Release(bulletHoles);
            }
            else if(damagedCollider.CompareTag("Metal"))
            {
                GameObjectManager.metalBulletHolesPool.Release(bulletHoles);
            }
            else if(damagedCollider.CompareTag("Cement"))
            {
                GameObjectManager.cementBulletHolesPool.Release(bulletHoles);
            }
        }

        private Vector3 ShootingRay()
        {
            //Spread
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);

            if(characterManager.isMoving)
            {
                x *= 1.5f;
                y *= 1.5f;
            }

            if(playerManager != null)
            {
                return crossHairTransform.position - MuzzlePoint.position;
            }
            return aIManager.DirectionToTarget + new Vector3(x,y,0);
        }

        private int GetDeathAnimation(Transform damagedCharacter)
        {
            Vector3 characterDirectionFromGrenade = (MuzzlePoint.position - damagedCharacter.forward).normalized;

            float dotProduct = Vector3.Dot(damagedCharacter.forward, characterDirectionFromGrenade);
            return AnimatorHashNames.DeathAnimation(dotProduct);
        }

        private void ResetCoolDown()
        {
            coolDown = false;
        }

        private void HandleReloading()
        {
            if(isReloading || characterManager.performingAction || characterManager.canReload != true)
            {
                return;
            }

            if(bulletLeft >= maxBullet)
            {
                return;
            }

            if(MagazineSize <= 0)
            {
                MagazineSize = 0;
                return;
            }
            characterManager.animator.SetBool(AnimatorHashNames.isReloadingHash, true);
            characterManager.characterAnimationManager.PlayTargetAnimation(AnimatorHashNames.reloadingHash, true);

            int bulletsNeeded = maxBullet - bulletLeft;
            bulletLeft += bulletsNeeded;
            if(playerManager != null) { MagazineSize -= bulletsNeeded;  }
        }
    }
}
