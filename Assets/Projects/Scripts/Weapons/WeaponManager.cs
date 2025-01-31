using UnityEngine;

namespace Creotly_Studios
{
    public class WeaponManager : MonoBehaviour
    {
        //Animations
        protected bool hasReset;
        protected int deathAnimation;
        protected int damageAnimation;
        protected bool hasBeenInitialized;

        [Header("Information")]
        public bool partOfBody;
        public string weaponName;
        public Sprite weaponImage;
        public WeaponType weaponType;
        public WeaponDataHolder weaponDataHolder;

        [Header("Physics")]
        public Rigidbody rigidBody;
        public Collider weaponCollider;
        private MeshRenderer meshRenderer;

        [Header("Manager")]
        protected AIManager aiManager;
        protected PlayerManager playerManager;
        public CharacterManager characterManager {get; protected set;}

        [Header("Parameters")]
        [field: SerializeField] public float damageValue {get; protected set;}
        [field: SerializeField] public LayerMask EnemyLayerMask {get; protected set;}

        [Header("Transform Constraints")]
        [SerializeField] protected Transform weaponGrip;
        [SerializeField] protected Transform  weaponRest;
        [HideInInspector] public Transform inactiveWeaponHolder;

        [Header("Rest Constraints Positions")]
        [field: SerializeField] public Vector3 RestLockedPosition {get; protected set;}
        [field: SerializeField] public Vector3 RestOriginalPosition {get; protected set;}
        
        [Header("Animation")]
        [SerializeField] protected AnimatorOverrideController mainAnimatorController;
        [SerializeField] protected AnimatorOverrideController crouchAnimatorController;

        protected virtual void Awake()
        {
            weaponDataHolder = Instantiate(weaponDataHolder);
            
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            weaponCollider = meshRenderer.GetComponent<Collider>();
            rigidBody = meshRenderer.GetComponentInParent<Rigidbody>();
        }
        
        public virtual void Initialize(CharacterManager cm)
        {
            characterManager = cm;
            aiManager = characterManager as AIManager;
            playerManager = characterManager as PlayerManager;

            if(playerManager == null)
            {
                aiManager.aIAnimationManager.standingAnimatorController = mainAnimatorController;
            }
            else
            {
                playerManager.playerAnimationManager.mainAnimatorController = mainAnimatorController;
                playerManager.playerAnimationManager.crouchAnimatorController = crouchAnimatorController;
            }
            characterManager.characterAnimatorRigController.SetTwoBoneIKConstraint(weaponGrip, weaponRest);
            hasBeenInitialized = true;
        }

        public virtual void ResetAllStats()
        {

        }

        public virtual void WeaponManager_Update(float delta)
        {
            if(weaponType != WeaponType.Grenade)
            {
                SetPhysicsSystem(characterManager == null);
            }
            if(playerManager != null) {weaponDataHolder.UpdateWeaponParameters(this);}
        }

        public void SetPhysicsSystem(bool enable)
        {
            weaponCollider.enabled = enable;
            rigidBody.isKinematic = !(enable);
        }
    }
}
