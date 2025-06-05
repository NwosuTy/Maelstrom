using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

namespace Creotly_Studios
{
    public class GunWeaponManager : WeaponManager
    {
        //Private Parameters
        protected float accumulatedTime;

        //Private Bullet Parameters
        protected Quaternion targetRotation;
        protected ObjectPool<TrailRenderer> bulletTrailPool;
        protected List<Bullet> bulletList = new List<Bullet>();

        [Header("Gun Status")]
        [SerializeField] protected int fireRate = 25;
        [SerializeField] protected float bulletDrop = 300f;
        [SerializeField] protected float bulletSpeed = 1000f;
        [SerializeField] protected float maxBulletTime = 3.0f;

        [Header("FX")]
        [SerializeField] protected float simulationSpeed;
        [SerializeField] protected ParticleSystem[] muzzleFlash;
        [SerializeField] protected TrailRenderer bulletTrailPrefab;

        public override void Initialize(CharacterManager cm)
        {
            base.Initialize(cm);
            hasBeenInitialized = true;
        }

        protected override void Awake()
        {
            base.Awake();
            Vector3 rotation = new(90.0f, 0.0f, 0.0f);
            targetRotation = Quaternion.Euler(rotation);
            bulletTrailPool = ObjectPooler.TrailPool(bulletTrailPrefab);
        }

        protected void HandleVFX()
        {
            foreach(var particle in muzzleFlash)
            {
                particle.Emit(1);
            }
        }

        public virtual void HandleShooting(Vector3 targetPosition, float delta)
        {
            if(characterManager == null)
            {
                return;
            }

            if(characterManager.performingAction)
            {
                return;
            }

            if (characterManager.isAttacking != true)
            {
                return;
            }

            accumulatedTime += delta;
            float fireInterval = 1.0f / fireRate;

            while (accumulatedTime > 0.0f)
            {
                FireBullet(targetPosition);
                accumulatedTime -= fireInterval;
            }
        }

        public void UpdateBullet(float delta)
        {
            SimulateBullet(delta);
            bulletList.RemoveAll(x => x.time >= maxBulletTime);
        }

        #region Bullet Functions

        protected Bullet CreateBullet(Vector3 pos, Vector3 vel)
        {
            Bullet bullet = new(pos, vel);
            return bullet;
        }

        protected Vector3 GetBulletPosition(Bullet bullet)
        {
            //Pos = bPos + bVel * bTime + 0.5 * grv * bTime^2
            Vector3 gravity = Vector3.down * bulletDrop;
            return bullet.initialPosition + (bullet.initialVelocity * bullet.time) + (0.5f * bullet.time * bullet.time * gravity);
        }

        protected void SimulateBullet(float delta)
        {
            bulletList.ForEach
            (
                bullet =>
                {
                    Vector3 p0 = GetBulletPosition(bullet);
                    bullet.time += delta;
                    Vector3 p1 = GetBulletPosition(bullet);
                    HandleRaycastSegment(p0, p1, bullet);
                }
            );
        }

        protected void HandleRaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
        {
            Vector3 dir = end - start;
            float distance = dir.magnitude;

            Ray ray = new(start, dir);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, distance, EnemyLayerMask))
            {
                Collider collider = raycastHit.collider;

                BulletFX bulletFX = GetBulletFX(collider);
                TrailFX.HandleTrailFX(simulationSpeed, start, raycastHit.point, bulletTrailPool, this);
                CharacterStatsManager shotCharacter = collider.GetComponentInParent<CharacterStatsManager>();

                InstantiateBulletHoles(bulletFX, raycastHit, shotCharacter);
                if (shotCharacter != null && shotCharacter.characterManager.characterType != characterManager.characterType)
                {
                    float directionFromHit = Vector3.SignedAngle(characterManager.transform.position, shotCharacter.transform.position, Vector3.up);

                    deathAnimation = GetDeathAnimation(ray.direction, shotCharacter.transform);
                    damageAnimation = AnimatorHashNames.DamageTargetAnimation(directionFromHit);
                    shotCharacter.TakeHealthDamage(damageAnimation, deathAnimation, damageValue);
                }
                bullet.time = maxBulletTime;
                return;
            }
            TrailFX.HandleTrailFX(simulationSpeed, start, raycastHit.point, bulletTrailPool, this);
        }

        protected virtual void FireBullet(Vector3 targetPosition)
        {
            HandleVFX();
        }

        protected void InstantiateBulletHoles(BulletFX bulletFX, RaycastHit raycastHit, CharacterStatsManager shotCharacter)
        {
            if (shotCharacter != null)
                return;

            float decalOffset = 0.05f;
            Quaternion spawnRotation = Quaternion.LookRotation(raycastHit.normal);
            Vector3 spawnPosition = raycastHit.point + raycastHit.normal * decalOffset;

            bulletFX.HandleBulletImpact(spawnPosition, spawnRotation);
        }

        protected BulletFX GetBulletFX(Collider damagedCollider)
        {
            string tag = damagedCollider.tag;
            return GameObjectManager.Instance.GetBulletFX(tag);
        }

        #endregion

        protected int GetDeathAnimation(Vector3 direction, Transform damagedCharacter)
        {
            float dotProduct = Vector3.Dot(damagedCharacter.forward, direction);
            return AnimatorHashNames.DeathAnimation(dotProduct);
        }
    }
}
