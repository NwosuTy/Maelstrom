using UnityEngine;

namespace Creotly_Studios
{
    public class CharacterManager : MonoBehaviour
    {
        //In-Built Components
        public Animator animator {get; protected set;}
        public CharacterController characterController {get; private set;}

        //Creotly Components
        public AnimatorEventsManager animatorEventsManager {get; private set;}
        public CharacterStatsManager characterStatsManager {get; private set;}
        public CharacterCombatManager characterCombatManager {get; private set;}
        public CharacterAnimationManager characterAnimationManager {get; private set;}
        public CharacterInventoryManager characterInventoryManager {get; private set;}
        public CharacterLocomotionManager characterLocomotionManager {get; private set;}
        public CharacterAnimationRigController characterAnimationRigController {get; private set;}

        //Status
        [HideInInspector] public bool isDead;
        [HideInInspector] public bool isMoving;
        [HideInInspector] public bool canRotate;
        [HideInInspector] public bool isJumping;
        [HideInInspector] public bool canReload;
        /*[HideInInspector]*/ public bool isGrounded;
        [HideInInspector] public bool isCrouching;
        [HideInInspector] public bool isAttacking;
        [HideInInspector] public bool performingAction;
        [HideInInspector] public bool rotateWithRootMotion;

        [field: Header("Status")]
        [HideInInspector] public bool isLockedIn;
        public GrenadeWeaponManager hitGrenadeWeapon {get; private set;}
        [field: SerializeField] public CharacterType characterType {get; private set;} = CharacterType.Enemy;

        [field: Header("UI Bars")]
        [field: SerializeField] public UIBar healthBarUI {get; protected set;}

        protected virtual void Awake()
        {
            characterController = GetComponent<CharacterController>();
            
            animatorEventsManager = GetComponent<AnimatorEventsManager>();
            characterStatsManager = GetComponent<CharacterStatsManager>();
            characterCombatManager = GetComponent<CharacterCombatManager>();

            characterInventoryManager = GetComponent<CharacterInventoryManager>();
            characterAnimationManager = GetComponent<CharacterAnimationManager>();
            characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
            characterAnimationRigController = GetComponentInChildren<CharacterAnimationRigController>();
        }

        protected virtual void Start()
        {
            characterStatsManager?.ResetHealth();
        }

        protected virtual void Update()
        {
            SetAnimatorParameters();
            float delta = Time.deltaTime;

            characterCombatManager.CharacterCombatManager_Update();
            characterStatsManager.CharacterStatsManager_Update(delta);
            
            characterAnimationManager.CharacterAnimatorManager_Update(delta);
            characterLocomotionManager.CharacterLocomotionManager_Update(delta);
            characterAnimationRigController?.CharacterAnimationRig_Updater(delta);

            characterInventoryManager.CharacterInventory_Updater(delta);
        }

        protected virtual void FixedUpdate()
        {
        
        }

        protected virtual void LateUpdate()
        {
        
        }

        private void SetAnimatorParameters()
        {
            animator.SetBool(AnimatorHashNames.movingHash, isMoving);
            
            canRotate = animator.GetBool(AnimatorHashNames.canRotateHash);
            performingAction = animator.GetBool(AnimatorHashNames.interactHash);
            rotateWithRootMotion = animator.GetBool(AnimatorHashNames.rootMotionRotateHash);
        }

        public void GotHitByGrenade(GrenadeWeaponManager grenade)
        {
            hitGrenadeWeapon = grenade;
        }
    }
}
