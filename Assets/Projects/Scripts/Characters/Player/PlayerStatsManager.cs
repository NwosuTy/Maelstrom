using UnityEngine;

namespace Creotly_Studios
{
    public class PlayerStatsManager : CharacterStatsManager
    {
        PlayerManager playerManager;

        //Parameters
        private float maxEndurance;
        [SerializeField] private float enduranceLevel;
        [field : SerializeField] public float currentEndurance {get; protected set;}

        [Header("Stamina Regenerator")]
        public bool canRegenerate;
        public float enduranceTickTimer = 0f;
        public float enduranceRegenerateTimer = 0f;
        public float enduranceRegenerateAmount = 2f;

        protected override void Awake()
        {
            base.Awake();
            playerManager = characterManager as PlayerManager;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        public override void CharacterStatsManager_Update(float delta)
        {
            RegenerateEndurance();
            base.CharacterStatsManager_Update(delta);
            playerManager.staminaBarUI.SetCurrentValue(currentEndurance);
        }

        //Functionalities

        public override void ResetHealth()
        {
            base.ResetHealth();
        }

        public void ResetEndurance()
        {
            maxEndurance = 10 * enduranceLevel;

            currentEndurance = maxEndurance;
            playerManager.staminaBarUI.SetMaxValue(currentEndurance);
            playerManager.staminaBarUI.SetMaxValue(currentEndurance);
        }

        protected override void HandleDeath(int deathAnimation)
        {
            playerManager.isDead = true;
            base.HandleDeath(deathAnimation);
            //Show Exit Dialog
        }

        public override void TakeHealthDamage(int damageAnimation, int deathAnimation, float damageValue)
        {
            currentHealth -= damageValue;

            if(currentHealth <= 0.0f)
            {
                HandleDeath(deathAnimation);
                return;
            }
            base.TakeHealthDamage(damageAnimation, deathAnimation, damageValue);
        }

        private void RegenerateEndurance()
        {
            if(playerManager.sprintFlag)
            {
                return;
            }
            if(playerManager.performingAction)
            {
                return;
            }

            if(currentEndurance < maxEndurance)
            {
                canRegenerate = true;
            }
            else if(currentEndurance >= maxEndurance)
            {
                currentEndurance = maxEndurance;
                canRegenerate = false;
            }

            if(canRegenerate == true)
            {
                enduranceRegenerateTimer += Time.deltaTime;
                if (enduranceRegenerateTimer >= 2f)
                {
                    enduranceTickTimer += Time.deltaTime;

                    if (enduranceTickTimer >= 0.1f)
                    {
                        enduranceTickTimer = 0f;
                        currentEndurance += Mathf.RoundToInt(enduranceRegenerateAmount);
                    }
                }
                canRegenerate = false;
            }
            else if(canRegenerate != true)
            {
                enduranceRegenerateTimer = 0f;
            }
        }

        public void ReduceEndurancePeriodically(float floatToReduceBy, float delta)
        {
            currentEndurance -= floatToReduceBy * delta;
        }
    }
}
