using UnityEngine;

namespace Creotly_Studios
{
    public class AIAnimationManager : CharacterAnimationManager
    {
        AIManager aiManager;

        [field: Header("Parameters")]
        [field: SerializeField] public HumanBones[] HumanBones {get; private set;}
        [field: SerializeField] public Transform[] BoneTransforms {get; private set;}

        protected override void Awake()
        {
            base.Awake();
            aiManager = characterManager as AIManager;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            GetBoneTransforms();
        }

        // Update is called once per frame
        public override void CharacterAnimatorManager_Update(float delta)
        {
        
        }

        private void GetBoneTransforms()
        {
            BoneTransforms = new Transform[HumanBones.Length];
            if(aiManager.enemyType == EnemyType.Mech)
            {
                for(int i = 0; i < BoneTransforms.Length; i++)
                {
                    BoneTransforms[i] = HumanBones[i].boneTransform;
                }
                return;
            }

            for(int i = 0; i < BoneTransforms.Length; i++)
            {
                BoneTransforms[i] = aiManager.animator.GetBoneTransform(HumanBones[i].bone);
            }
        }

        private float HaltRunning(float maxDistance)
        {
            return maxDistance + 2.5f;
        }

        public void HandleAnimation(float maxDistance)
        {
            if(aiManager.DistanceToTarget >= maxDistance)
            {
                float shouldRun = HaltRunning(maxDistance);
                if(aiManager.DistanceToTarget > shouldRun)
                {
                    aiManager.aIAnimationManager.SetBlendTreeParameter(2.0f, 0.0f, true, Time.deltaTime);
                    return;
                }
                aiManager.aIAnimationManager.SetBlendTreeParameter(1.0f, 0.0f, false, Time.deltaTime);
            }
        }
    }
}
