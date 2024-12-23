using UnityEngine;

namespace Creotly_Studios
{
    public class AICombatManager : CharacterCombatManager
    {
        protected AIManager aiManager;

        //Private Parameters
        private float blendOut;
        private Quaternion aimTowards;
        private Quaternion blendRotation;

        [Header("Parameters")]
        public float weight;
        public int iterations;
        public Transform aimTransform;

        [Header("Combo Chances")]
        public int grenadeThrowChance;

        protected override void Awake()
        {
            base.Awake();
            aiManager = characterManager as AIManager;
        }
        
        // Start is called before the first frame update
        protected override void Start()
        {
        
        }

        // Update is called once per frame
        public override void CharacterCombatManager_Update()
        {
        
        }

        public void AICombatManager_LateUpdate()
        {
            TargetingSystem();
        }

        private void TargetingSystem()
        {
            if(aiManager.isLockedIn != true || aiManager.target.source == null)
            {
                return;
            }

            Vector3 targetPosition = TargetPosition();
            for(int i = 0; i < iterations; i++)
            {
                for(int j = 0; j < aiManager.aIAnimationManager.BoneTransforms.Length; j++)
                {
                    Transform bone = aiManager.aIAnimationManager.BoneTransforms[j];
                    float boneWeight = aiManager.aIAnimationManager.HumanBones[j].weight * weight;
                    AimAtTarget(bone, targetPosition, boneWeight);
                }
            }
        }

        private void AimAtTarget(Transform bone, Vector3 targetPosition, float weight)
        {
            Vector3 aimDirection = aimTransform.forward;
            Vector3 targetDirection = (targetPosition - aimTransform.position).normalized;

            aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
            blendRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
            bone.rotation *= blendRotation;
        }

        private Vector3 TargetPosition()
        {
            blendOut = 0.0f;
            Vector3 targetDirection = (aiManager.target.targetPosition - aimTransform.position).normalized;
            Vector3 aimDirection = aimTransform.forward;

            float targetAngle = Maths_PhysicsHelper.CalculateViewAngle(aimDirection, targetDirection);
            if(targetAngle > aiManager.angleLimit.upperBound)
            {
                blendOut += (aiManager.AngleOfTarget - aiManager.angleLimit.upperBound) / 50.0f;
            }

            float targetDistance = targetDirection.magnitude;
            if(targetDistance <= aiManager.navMeshAgent.stoppingDistance)
            {
                blendOut += aiManager.navMeshAgent.stoppingDistance - targetDistance;
            }

            Vector3 direction = Vector3.Slerp(aiManager.DirectionToTarget, aimDirection, blendOut);
            return aimTransform.position + direction;
        }
    }
}
