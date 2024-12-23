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
        public virtual void CharacterCombatManager_Update()
        {
        
        }

        public virtual void ThrowGrenade()
        {
            characterManager.characterAnimationManager.PlayTargetAnimation(AnimatorHashNames.throwingObjectHash, true);
        }

        public void ThrowGrenadePhysics()
        {
            GrenadeWeaponManager grenade = GameObjectManager.grenadePool.Get();
            grenade.Initialize(characterManager);

            Rigidbody rb = grenade.rigidBody;
            // grenade.transform.SetPositionAndRotation(characterManager.handHolder.position, characterManager.handHolder.rotation);

            rb.isKinematic = false;
            grenade.weaponCollider.enabled = true;

            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(grenade.transform.forward * horizontalVelocity);
            rb.AddForce(grenade.transform.up * verticalVelocity);

            rb.mass = grenade.Mass;
            grenade.transform.SetParent(null);
            characterManager.characterInventoryManager.grenadesLeft--;
        }
    }
}
