using UnityEngine;
using System.Collections;

namespace Creotly_Studios
{
    public class AIAnimationManager : CharacterAnimationManager
    {
        AIManager aiManager;

        [Header("Animator Controller")]
        public AnimatorOverrideController standingAnimatorController;
        [SerializeField] private AnimatorOverrideController unarmedController;

        protected override void Awake()
        {
            base.Awake();
            aiManager = characterManager as AIManager;
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

        private float HaltRunning(float maxDistance)
        {
            return maxDistance + 2.5f;
        }

        private void Stand_Or_Crouch()
        {
            aiManager.animator.runtimeAnimatorController = standingAnimatorController;
        }

        public void HandleAnimation(float maxDistance)
        {
            if(aiManager.DistanceToTarget >= maxDistance)
            {
                float shouldRun = HaltRunning(maxDistance);
                if(aiManager.DistanceToTarget > shouldRun)
                {
                    SetBlendTreeParameter(2.0f, 0.0f, true, Time.deltaTime);
                    return;
                }
                SetBlendTreeParameter(1.0f, 0.0f, false, Time.deltaTime);
            }
        }

        public void Unarmed_StandOrCrouch()
        {
            standingAnimatorController = unarmedController;
        }
    }
}
