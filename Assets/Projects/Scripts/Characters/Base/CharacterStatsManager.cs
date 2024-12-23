using UnityEngine;

namespace Creotly_Studios
{
    public class CharacterStatsManager : MonoBehaviour
    {
        //Components
        public CharacterManager characterManager {get; protected set;}

        [field: Header("Character Stats")]
        [SerializeField] private float healthLevel;
        [field : SerializeField] public float currentHealth {get; protected set;}

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
        
        }

        // Update is called once per frame
        public virtual void CharacterStatsManager_Update(float delta)
        {
        
        }

        //Functionalities

        public virtual void ResetHealth()
        {
            characterManager.isDead = false;
            currentHealth = healthLevel * 10.0f;

            characterManager.healthBarUI?.SetMaxValue(currentHealth);
            characterManager.healthBarUI?.SetCurrentValue(currentHealth);
        }

        protected virtual void HandleDeath(int deathAnimation)
        {
            currentHealth = 0.0f;
            characterManager.healthBarUI.SetCurrentValue(currentHealth);
        }

        public virtual void TakeHealthDamage(int damageAnimation, int deathAnimation, float damageValue)
        {
            characterManager.healthBarUI.SetCurrentValue(currentHealth);
            characterManager.characterAnimationManager.PlayTargetAnimation(damageAnimation, true);
        }

        public virtual void ExplosionDamage(int damageAnimation, int deathAnimation, float damageValue, GrenadeWeaponManager grenade)
        {
            characterManager.GotHitByGrenade(grenade);
            float distanceFromExplosion = Vector3.Distance(grenade.transform.position, transform.position);

            float finalDamageValue =  damageValue / distanceFromExplosion;
            currentHealth -= finalDamageValue;

            if(currentHealth <= 0.0f)
            {
                HandleDeath(deathAnimation);
                return;
            }
            characterManager.healthBarUI.SetCurrentValue(currentHealth);
            characterManager.characterAnimationManager.PlayTargetAnimation(damageAnimation, true);
        }
    }
}
