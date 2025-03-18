using UnityEngine;

namespace Creotly_Studios
{
    public class CharacterCombatManager : MonoBehaviour
    {
        protected CharacterManager characterManager;

        [Header("Grenade Physics")]
        public GrenadeWeaponManager grenadeObject;
        [SerializeField] private float verticalVelocity;
        [SerializeField] private float horizontalVelocity;

        [Header("Objects To Spawn")]
        public int grenadesLeft = 5;
        [SerializeField] private Transform grenadeHandler;
        [SerializeField] private GrenadeWeaponManager grenadePrefab;

        [field: Header("Damage Effect")]
        [field: SerializeField] public MeleeDamageEffect melee_DamageEffect {get; protected set;}

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }
        
        // Start is called before the first frame update
        protected virtual void Start()
        {
            melee_DamageEffect = Instantiate(melee_DamageEffect);
        }

        // Update is called once per frame
        public virtual void CharacterCombatManager_Update(float delta)
        {
        
        }

        public virtual void ThrowGrenade()
        {
            if (grenadesLeft <= 0)
            {
                grenadesLeft = 0;
                return;
            }

            if (characterManager.canThrowGrenade != true)
            {
                return;
            }
            characterManager.characterAnimationManager.PlayTargetAnimation(AnimatorHashNames.throwingObjectHash, true);
        }

        public virtual void ThrowGrenadePhysics()
        {
            GrenadeWeaponManager grenade = Instantiate(grenadePrefab, grenadeHandler.position, grenadeHandler.rotation);
            characterManager.characterAnimatorRigController.RightHandIKConstraint.weight = 0.0f;

            Rigidbody rb = grenade.rigidBody;
            grenade.Initialize(characterManager);
            
            rb.isKinematic = false;
            grenade.weaponCollider.enabled = true;

            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(grenade.transform.forward * horizontalVelocity);
            rb.AddForce(grenade.transform.up * verticalVelocity);

            rb.mass = grenade.Mass;
            grenade.transform.SetParent(null);

            grenadesLeft--;
            characterManager.characterAnimatorRigController.RightHandIKConstraint.weight = 1.0f;
        }
    }
}
