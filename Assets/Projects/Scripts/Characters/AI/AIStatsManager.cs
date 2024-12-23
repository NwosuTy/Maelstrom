using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creotly_Studios
{
    public class AIStatsManager : CharacterStatsManager
    {
        AIManager aiManager;

        private bool showHealthBar;
        private WaitForSeconds waitForSeconds;

        protected override void Awake()
        {
            base.Awake();
            aiManager = characterManager as AIManager;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            waitForSeconds = new WaitForSeconds(3.5f);
            if(aiManager.healthBarUI != null) aiManager.healthBarUI.gameObject.SetActive(false);
        }

        // Update is called once per frame
        public override void CharacterStatsManager_Update(float delta)
        {
            if(showHealthBar)
            {
                StartCoroutine(DisplayHealthBarCoroutine());
            }
            base.CharacterStatsManager_Update(delta);
        }

        //Functionalities 


        //Functionalities

        public override void ResetHealth()
        {
            base.ResetHealth();
        }
        
        protected override void HandleDeath(int deathAnimation)
        {
            showHealthBar = false;
            characterManager.isDead = true;
            //Release from Pool
            base.HandleDeath(deathAnimation);
        }

        public override void TakeHealthDamage(int damageAnimation, int deathAnimation, float damageValue)
        {
            currentHealth -= damageValue;
            
            if(currentHealth <= 0.0f)
            {
                HandleDeath(deathAnimation);
                return;
            }
            showHealthBar = true;
            aiManager.healthBarUI.gameObject.SetActive(showHealthBar);
            base.TakeHealthDamage(damageAnimation, deathAnimation, damageValue);
        }

        private IEnumerator DisplayHealthBarCoroutine()
        {
            while(showHealthBar == true)
            {
                yield return waitForSeconds;
                showHealthBar = false;
                aiManager.healthBarUI.gameObject.SetActive(showHealthBar);
            }
        }
    }
}
