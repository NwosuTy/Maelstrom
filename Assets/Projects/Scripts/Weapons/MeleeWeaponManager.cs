using UnityEngine;

namespace Creotly_Studios
{
    public class MeleeWeaponManager : WeaponManager
    {
        [SerializeField] private DamageCollider[] damageColliders;

        protected override void Awake()
        {
            base.Awake();
            rigidBody = GetComponent<Rigidbody>();
            Initialize(GetComponentInParent<CharacterManager>());
        }

        private void Start()
        {
            foreach(var damageCollider in damageColliders)
            {
                damageCollider.Initialize(this);
            }
        }

        public override void Initialize(CharacterManager cm)
        {
            base.Initialize(cm);
            hasBeenInitialized = true;
        }

        public override void ResetAllStats()
        {
            base.ResetAllStats();
        }

        public void EnableCollider()
        {
            rigidBody.isKinematic = true;
            foreach(var collider in damageColliders)
            {
                collider.EnableCollider();
            }
        }

        public void DisableCollider()
        {
            rigidBody.isKinematic = false;
            foreach(var collider in damageColliders)
            {
                collider.DisableCollider();
            }
        }

        public override void WeaponManager_Update(float delta)
        {
            if(hasBeenInitialized != true)
            {
                return;
            }
            
            HandleAttacking();
            base.WeaponManager_Update(delta);
        }

        private void HandleAttacking()
        {
            if(aIManager != null)
            {
                return;
            }

            //Sets Is Shooting to Hold if shooting can be held else set to Tap
            characterManager.isAttacking = playerManager.playerInputManager.tapShootInput;
        }
    }
}
