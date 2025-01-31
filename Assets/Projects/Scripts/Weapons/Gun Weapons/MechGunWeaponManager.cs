using UnityEngine;

namespace Creotly_Studios
{
    public class MechGunWeaponManager : GunWeaponManager
    {
        [field: SerializeField] public Transform[] FirePoints { get; private set; }

        public override void Initialize(CharacterManager cm)
        {
            characterManager = cm;
            aiManager = characterManager as AIManager;

            hasBeenInitialized = true;
        }

        protected override void Awake()
        {
            weaponDataHolder = Instantiate(weaponDataHolder);

            Vector3 rotation = new(90.0f, 0.0f, 0.0f);
            bulletTrailPool = ObjectPooler.TrailPool(bulletTrailPrefab);
        }

        public override void WeaponManager_Update(float delta)
        {
            if(hasBeenInitialized != true)
            {
                return;
            }
            UpdateBullet(delta);
        }

        protected override void FireBullet(Vector3 targetPosition)
        {
            HandleVFX();
            foreach(Transform firePoint in FirePoints)
            {
                MuzzlePointFireBullet(targetPosition, firePoint);
            }
        }

        private void MuzzlePointFireBullet(Vector3 targetPosition, Transform firePoint)
        {
            Vector3 velocity = (targetPosition - firePoint.position).normalized * bulletSpeed;

            Bullet bullet = CreateBullet(firePoint.position, velocity);
            bulletList.Add(bullet);
        }
    }
}
