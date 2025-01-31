using UnityEngine;
using Cinemachine;

namespace Creotly_Studios
{
    public class GrenadeWeaponManager : WeaponManager
    {
        //Damage Collider
        public CinemachineImpulseSource impulseSource;
        public GrenadeDamageCollider grenadeDamageCollider;

        [field: Header("Grenade Physics")]
        [field: SerializeField] public float Mass {get; private set;}
        [field: SerializeField] public bool UseGravity {get; private set;}

        [field: Header("Grenade Stats")]
        [field: SerializeField] public float explosionRange {get; private set;}
        [field: SerializeField] public float explosionForce {get; private set;}

        protected override void Awake()
        {
            base.Awake();
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        public override void Initialize(CharacterManager cm)
        {
            characterManager = cm;
            aiManager = characterManager as AIManager;
            playerManager = characterManager as PlayerManager;

            hasBeenInitialized = true;
        }

        public override void ResetAllStats()
        {
            base.ResetAllStats();
        }

        public override void WeaponManager_Update(float delta)
        { 
            if(hasBeenInitialized != true)
            {
                return;
            }
            base.WeaponManager_Update(delta);
        }
    }
}
