using UnityEngine;

namespace Creotly_Studios
{
    public class PlayerAnimationManager : CharacterAnimationManager
    {
        PlayerManager playerManager;

        [HideInInspector] public RuntimeAnimatorController mainAnimatorController;
        [HideInInspector] public AnimatorOverrideController crouchAnimatorController;

        [Header("Animator Controllers")]
        [SerializeField] private RuntimeAnimatorController unarmedStandingController;
        [SerializeField] private AnimatorOverrideController unarmedCrouchingController;

        protected override void Awake()
        {
            base.Awake();
            playerManager = characterManager as PlayerManager;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        public override void CharacterAnimatorManager_Update(float delta)
        {
            Stand_Or_Crouch();
        }

        private void Stand_Or_Crouch()
        {
            if(playerManager.isCrouching != true)
            {
                playerManager.animator.runtimeAnimatorController = mainAnimatorController;
                return;
            }
            playerManager.animator.runtimeAnimatorController = crouchAnimatorController;
        }

        public void Unarmed_StandOrCrouch()
        {
            characterManager.characterAnimatorRigController.SetTwoBoneIKConstraint(null, null);

            if (playerManager.isCrouching != true)
            {
                mainAnimatorController = unarmedStandingController;
                return;
            }
            crouchAnimatorController = unarmedCrouchingController;
        }
    }
}
