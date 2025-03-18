using System.Collections;
using UnityEngine;

namespace Creotly_Studios
{
    public class CharacterLocomotionManager : MonoBehaviour
    {
       #region protected Parameters

        //Components
        protected CharacterManager characterManager;

        //Gravity
        protected Vector3 verticalVelocity;
        public float fallingTimer {get; protected set;}

        //Physics
        protected RaycastHit rayCastHit;
        protected Vector3 moveDirection;
        protected Vector3 jumpDirection;
        protected Vector3 rotationDirection;

        //Explosion Physics
        private Vector3 force;
        private Vector3 explosionDirection;
        private float distanceFromExplosion;
	protected float acceleration = 1.0f;

        #endregion

        [field: Header("Objects")]
        public Camera mainCamera {get; protected set;}
        public Transform cameraObject {get; protected set;}

        [field: Header("Physics")]
        [Tooltip("Force at which character is sticking to the ground")] [SerializeField] protected float groundedForce = -20f;

        [field : Header("Character Movement Stats")]
        [field: SerializeField] public float rotationSpeed {get; protected set;} = 12f;
        [field: SerializeField] public float movementSpeed {get; protected set;} = 3.5f;
        [field: SerializeField] public float crouchingSpeed {get; protected set;} = 3.5f;

        [field: Header("Gravity Stats")]
        [SerializeField] protected float jumpHeight = 4.0f;
        [SerializeField] protected float gravityForce = -30.0f;
        [field: SerializeField] public bool fallingVelocitySet {get; protected set;} = false;
        [Tooltip("Force at which character begins to fall")] [field: SerializeField] public float fallStartVelocity {get; protected set;} = -5.0f;

        [field: Header("Endurance Cost")]
        [field : SerializeField] public float jumpEnduranceCost {get; protected set;} = 10.0f;
        [field : SerializeField] public float sprintEnduranceCost {get; protected set;} = 10.0f;

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            mainCamera = Camera.main;
            cameraObject = mainCamera.transform;
        }

        // Update is called once per frame
        public virtual void CharacterLocomotionManager_Update(float delta)
        {
            HandleGravity(delta);
            HandleRotation(delta);
            HandleMovement(delta);
        }

        public virtual void CharacterLocomotion_FixedUpdate(float delta)
        {

        }

        //Functionalities

        protected virtual void HandleGravity(float delta)
        {
            CheckIfPlayerIsGrounded();
            if (characterManager.isGrounded)
            {
                if(verticalVelocity.y < 0.0f)
                {
                    fallingTimer = 0.0f;
                    fallingVelocitySet = false;
                    verticalVelocity.y = groundedForce;
                }
            }

            else if(characterManager.isGrounded != true)
            {
                if(characterManager.isJumping != true && fallingVelocitySet != true)
                {
                    fallingVelocitySet = true;
                    verticalVelocity.y = fallStartVelocity;
                }

                fallingTimer += delta;
                characterManager.animator.SetFloat(AnimatorHashNames.fallingTimerHash, fallingTimer);
                verticalVelocity.y += gravityForce * delta;
            }
            //Force of Gravity pushes character down
            characterManager.characterController.Move(verticalVelocity * delta);
        }

        protected virtual void CheckIfPlayerIsGrounded()
        {
            characterManager.isGrounded = characterManager.characterController.isGrounded;
            characterManager.animator.SetBool(AnimatorHashNames.isGroundedHash, characterManager.isGrounded);
        }

        protected virtual void HandleRotation(float delta)
        {

        }

        protected virtual void HandleMovement(float delta)
        {

        }

        public void ExplosionForce()
        {
            GrenadeWeaponManager grenade = characterManager.hitGrenadeWeapon;

            explosionDirection = transform.position - grenade.transform.position;
            distanceFromExplosion = explosionDirection.magnitude;

            if(distanceFromExplosion < grenade.explosionRange)
            {
                force = explosionDirection.normalized * (grenade.explosionForce / distanceFromExplosion);
                force.y += 1.0f;

                StartCoroutine(ApplyExplosionForce());
            }
        }

        private IEnumerator ApplyExplosionForce()
        {
            float duration = 0.3f;
            float time = 0.0f;

            while(time < duration)
            {
                characterManager.characterController.Move(force * Time.deltaTime);
                time += Time.deltaTime;
                yield return null;
            }
        }
    }
}
