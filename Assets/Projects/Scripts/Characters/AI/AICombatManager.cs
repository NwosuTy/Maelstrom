using UnityEngine;

namespace Creotly_Studios
{
    public class AICombatManager : CharacterCombatManager
    {
        private GunWeaponManager gun;
        protected AIManager aiManager;

        [Header("Parameters")]
        public int grenadeThrowChance;
        [SerializeField] private float inaccuracy;
        [SerializeField] private Vector3 targetOffset;
        [SerializeField] private float defaultTimer = 7.5f;

        protected override void Awake()
        {
            base.Awake();
            aiManager = characterManager as AIManager;
        }
        
        // Start is called before the first frame update
        protected override void Start()
        {
        
        }

        // Update is called once per frame
        public override void CharacterCombatManager_Update(float delta)
        {
            AttackTarget(delta);
            ResetCoolDownTimer(delta);
        }

        private void AttackTarget(float delta)
        {
            if(aiManager.isAttacking != true || aiManager.aIInventoryManager.currentWeaponManager == null)
            {
                return;
            }

            gun = aiManager.aIInventoryManager.currentWeaponManager as GunWeaponManager;
            if (gun == null)
            {
                return;
            }

            Vector3 target = aiManager.target.targetPosition + targetOffset;
            target += Random.insideUnitSphere * inaccuracy;
            gun.HandleShooting(target, delta);
        }

        private void ResetCoolDownTimer(float delta)
        {
            if(aiManager.coolDown == true)
            {
                aiManager.coolDownTimer -= delta;
                if(aiManager.coolDownTimer <= 0.0f)
                {
                    aiManager.coolDown = false;
                    aiManager.coolDownTimer = defaultTimer;
                }
            }
        }
    }
}
