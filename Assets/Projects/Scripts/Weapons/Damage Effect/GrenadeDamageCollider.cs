using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Creotly_Studios
{
    public class GrenadeDamageCollider : MonoBehaviour
    {
        //Status
        private bool hasExploded;

        //Animations
        private int backDead;
        private int frontDead;
        private int backDamage;
        private int frontDamage;
        private int explosionAnimation;
        private int explosionDeathAnimation;

        //Explosion Parameters
        private Vector3 impactNormal;
        private WaitForSeconds resetGrenadeTimer;
        private WaitForSeconds explosionDelayTimer;

        //Damage Characters
        private Collider[] colliders;
        private Transform cameraObject;
        private List<CharacterStatsManager> damagedCharacters = new();

        [Header("Explosion Parameters")]
        public float explosionDelay;
        public ParticleSystem explosionPrefab;
        public GrenadeWeaponManager grenadeWeaponManager;

        private void Start()
        {
            colliders = new Collider[20];

            backDead = AnimatorHashNames.backExplosionDeathHash;
            frontDead = AnimatorHashNames.frontExplosionDeathHash;

            backDamage = AnimatorHashNames.explosionBackAnimation;
            frontDamage = AnimatorHashNames.explosionFrontAnimation;

            resetGrenadeTimer = new WaitForSeconds(explosionDelay);
            explosionDelayTimer = new WaitForSeconds(explosionDelay);
            grenadeWeaponManager = GetComponentInParent<GrenadeWeaponManager>();
        }

        private void  OnCollisionStay(Collision other)
        {
            StartCoroutine(HandleExplosion());
        }

        private IEnumerator HandleExplosion()
        {
            yield return explosionDelayTimer;

            if(hasExploded == false)
            {
                SpawnExplosions();
                hasExploded = true;

                damagedCharacters.Clear();
                Physics.OverlapSphereNonAlloc(transform.position, grenadeWeaponManager.explosionRange, colliders, grenadeWeaponManager.EnemyLayerMask);

                foreach(Collider collider in colliders)
                {
                    if(collider == null)
                    {
                        continue;
                    }

                    CharacterStatsManager damagedCharacter = collider.GetComponentInParent<CharacterStatsManager>();
                    if(damagedCharacter != null && damagedCharacters.Contains(damagedCharacter) != true)
                    {
                        damagedCharacters.Add(damagedCharacter);
                    }
                }

                foreach(CharacterStatsManager damagedCharacter in damagedCharacters)
                {
                    explosionAnimation = GetExplosionAnimation(frontDamage, backDamage, damagedCharacter.transform);
                    explosionDeathAnimation = GetExplosionAnimation(frontDead, backDead, damagedCharacter.transform);
                    damagedCharacter.ExplosionDamage(explosionAnimation, explosionDeathAnimation, grenadeWeaponManager.damageValue, grenadeWeaponManager);
                }
                Invoke(nameof(ResetGrenade), 3f);
            }
        }

        private void ResetGrenade()
        {
            hasExploded = false;
            grenadeWeaponManager.rigidBody.mass = 1f;
            Destroy(grenadeWeaponManager.gameObject);
        }

        private void SpawnExplosions()
        {
            cameraObject = grenadeWeaponManager.characterManager.characterLocomotionManager.cameraObject;
            Quaternion explosionRotation = Quaternion.FromToRotation(Vector3.up, impactNormal);

            Instantiate(explosionPrefab, transform.position, explosionRotation);
            grenadeWeaponManager.impulseSource.GenerateImpulse(cameraObject.forward);
        }

        private int GetExplosionAnimation(int front, int back, Transform damagedCharacter)
        {
            Vector3 characterDirectionFromGrenade = (transform.position - damagedCharacter.forward).normalized;

            float dotProduct = Vector3.Dot(damagedCharacter.forward, characterDirectionFromGrenade);
            return AnimatorHashNames.ExplosionAnimation(front, back, dotProduct);
        }
    }
}