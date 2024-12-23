using UnityEngine;

namespace Creotly_Studios
{
    public class MechGunWeaponManager : WeaponManager
    {
        //Raycast Parameters
        private Ray ray;
        private RaycastHit raycastHit;
        private GameObject bulletHoles;
        private float timeBeforeRelease;

        [Header("Gun Parameters")]
        [SerializeField] private float range;
        [SerializeField] private float spread;
        [SerializeField] private GameObject[] muzzleFlash;
        [field: SerializeField] public Transform MuzzlePoint {get; private set;}

        
        protected override void Awake()
        {
            foreach(var flash in muzzleFlash)
            {
                flash.SetActive(false);
            }
            aIManager = GetComponentInParent<AIManager>();
        }

        public void EnableMuzzleFlash(int enable)
        {
            bool active = (enable <= 0) ? false : true;
            foreach(var flash in muzzleFlash)
            {
                flash.SetActive(active);
            }
        }

        public override void WeaponManager_Update(float delta)
        {
            HandleShooting(delta);
        }

        private void HandleShooting(float delta)
        {
            if(playerManager != null && playerManager.performingAction)
            {
                return;
            }

            if(characterManager.isAttacking != true)
            {
                return;
            }
            
            ray = ShootingRay();
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
            }
            ReleaseHoles(raycastHit.collider, delta);
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

        private Ray ShootingRay()
        {
            //Spread
            float x = Random.Range(-spread, spread);
            float y = Random.Range(-spread, spread);

            if(characterManager.isMoving)
            {
                x *= 1.5f;
                y *= 1.5f;
            }
            return new Ray(MuzzlePoint.transform.position, aIManager.DirectionToTarget + new Vector3(x,y,0));
        }

        private int GetDeathAnimation(Transform damagedCharacter)
        {
            Vector3 characterDirectionFromGrenade = (MuzzlePoint.position - damagedCharacter.forward).normalized;

            float dotProduct = Vector3.Dot(damagedCharacter.forward, characterDirectionFromGrenade);
            return AnimatorHashNames.DeathAnimation(dotProduct);
        }
    }
}
