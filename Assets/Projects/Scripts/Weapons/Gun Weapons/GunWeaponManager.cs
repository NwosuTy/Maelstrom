using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
using System.Collections.Generic;

namespace Creotly_Studios
{
    public class GunWeaponManager : WeaponManager
    {
        //Private Parameters
        protected Ray ray;
        protected float accumulatedTime;

        //Private Bullet Parameters
        protected GameObject bulletHoles;
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

            ray.origin = start;
            ray.direction = dir;

            if (Physics.Raycast(ray, out RaycastHit raycastHit, distance, EnemyLayerMask))
            {
                CharacterStatsManager shotCharacter = raycastHit.collider.GetComponent<CharacterStatsManager>();
                StartCoroutine(HandleTrailFX(start, raycastHit.point));

                if (shotCharacter == null)
                {
                    InstantiateBulletHoles(raycastHit);
                }

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
            StartCoroutine(HandleTrailFX(start, end));
        }

        protected virtual void FireBullet(Vector3 targetPosition)
        {
            HandleVFX();
        }

        protected void InstantiateBulletHoles(RaycastHit raycastHit)
        {
            float positionMultiplier = 0.5f;
            float spawnX = raycastHit.point.x - ray.direction.x * positionMultiplier;
            float spawnY = raycastHit.point.y - ray.direction.y * positionMultiplier;
            float spawnZ = raycastHit.point.z - ray.direction.z * positionMultiplier;
            Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);

            bulletHoles = ImpactHole(raycastHit.collider);
            Quaternion targetRotation = Quaternion.LookRotation(ray.direction);
            bulletHoles.transform.SetPositionAndRotation(spawnPosition, targetRotation);
        }

        protected GameObject ImpactHole(Collider damagedCollider)
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

        #endregion

        protected IEnumerator HandleTrailFX(Vector3 start, Vector3 end)
        {
            TrailRenderer trail = bulletTrailPool.Get();
            trail.transform.position = start;
            yield return null;

            trail.emitting = true;
            float distance  = Vector3.Distance(start, end);
            float remainingDistance = distance;
            while (remainingDistance > 0f)
            {
                trail.transform.position = Vector3.Lerp(start, end, Mathf.Clamp01(1 - (remainingDistance/distance)));

                remainingDistance -= simulationSpeed * Time.deltaTime;
                yield return null;
            }

            trail.transform.position = end;
            yield return new WaitForSeconds(trail.time);
            yield return null;

            trail.emitting = false;
            bulletTrailPool.Release(trail);
        }

        protected int GetDeathAnimation(Vector3 direction, Transform damagedCharacter)
        {
            float dotProduct = Vector3.Dot(damagedCharacter.forward, direction);
            return AnimatorHashNames.DeathAnimation(dotProduct);
        }
    }
}
