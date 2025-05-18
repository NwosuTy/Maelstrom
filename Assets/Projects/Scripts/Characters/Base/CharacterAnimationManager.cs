using UnityEngine;

namespace Creotly_Studios
{
    public class CharacterAnimationManager : MonoBehaviour
    {
        //Components
        protected CharacterManager characterManager;

        //HashNames
        public int verticalMovementHash { get; protected set; }
        public int horizontalMovementHash { get; protected set; }

        //Parameters
        private bool hasHashed;
        protected Vector3 deltaPosition;

        protected virtual void Awake()
        {
            characterManager = GetComponent<CharacterManager>();
        }

        private void OnEnable()
        {
            if(hasHashed)
            {
                return;
            }
            AnimatorHashNames.StringsToHash();
            verticalMovementHash = Animator.StringToHash("verticalMovement");
            horizontalMovementHash = Animator.StringToHash("horizontalMovement");
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            
        }

        private void OnDisable()
        {
            if(hasHashed != true)
            {
                return;
            }
            hasHashed = false;
        }

        protected virtual void OnAnimatorMove()
        {
            if(characterManager.isGrounded != true || characterManager.isDead)
            {
                return;
            }
            deltaPosition = characterManager.animator.deltaPosition;
            characterManager.characterController.Move(deltaPosition);
            characterManager.transform.rotation *= characterManager.animator.deltaRotation;
        }

        protected virtual void OnAnimatorIK(int layerIndex)
        {
            if(characterManager.isGrounded != true || characterManager.isDead)
            {
                return;
            }
            //characterManager.footIKSystem.FootIKSystem_AnimatorIK();
        }

        // Update is called once per frame
        public virtual void CharacterAnimatorManager_Update(float delta)
        {
            
        }

        //Functionalities
        public void InstantSetFloat(int hash, float value)
        {
            characterManager.animator.SetFloat(hash, value);
        }

        public void SetBlendTreeParameter(float verticalInput, float horizontalInput, bool isSprinting, float delta)
        {
            float snappedVertical = verticalInput;
            float snappedHorizontal = horizontalInput;

            if(isSprinting)
            {
                snappedVertical = 2.0f;
                snappedHorizontal = 0.0f;
            }
            characterManager.animator.SetFloat(verticalMovementHash, snappedVertical, 0.1f, delta);
            characterManager.animator.SetFloat(horizontalMovementHash, snappedHorizontal, 0.1f, delta);
        }

        public void PlayRootTargetAnimation(int targetAnimation, bool performAction, float transitionDuration = 0.1f)
        {
            characterManager.animator.applyRootMotion = performAction;
            characterManager.animator.SetBool(AnimatorHashNames.rootMotionRotateHash, true);
            characterManager.animator.SetBool(AnimatorHashNames.interactHash, performAction);
            characterManager.animator.CrossFade(targetAnimation, transitionDuration);
        }

        public void PlayTargetAnimation(int targetAnimation, bool performingAction, float transitionDuration= 0.2f, bool canRotate = true)
        {
            characterManager.animator.applyRootMotion = performingAction;
            characterManager.canRotate = canRotate;
            characterManager.animator.SetBool(AnimatorHashNames.interactHash, performingAction);
            characterManager.animator.CrossFade(targetAnimation, transitionDuration);
        }

        public void PlayTargetAnimation(string targetAnimation, bool performingAction, float transitionDuration = 0.2f, bool canRotate = true)
        {
            characterManager.animator.applyRootMotion = performingAction;
            characterManager.canRotate = canRotate;
            characterManager.animator.SetBool(AnimatorHashNames.interactHash, performingAction);
            characterManager.animator.CrossFade(targetAnimation, transitionDuration);
        }
    }
}
