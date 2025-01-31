using UnityEngine;
using System.Collections.Generic;

namespace Creotly_Studios
{
    public class AILogicHandler : MonoBehaviour
    {
        [Header("Classes")]
        AIManager aiManager;
        public EnemyMemoryHandlerScript memory = new EnemyMemoryHandlerScript();

        [Header("Scan Parameters")]
        public float memorySpan = 3.0f;

        [Header("Target Parameters")]
        public Target currentVisualTarget;
        [Range(0,1)] public float ageWeight = 0.75f;
        [Range(0,1)] public float angleWeight = 0.55f;
        [Range(0,1)] public float distanceWeight = 0.6f;
        public List<CharacterManager> characterManagers = new();

        [Header("Private Fields")]
        [SerializeField] Collider[] characterColliders;

        private void Awake()
        {
            aiManager = GetComponent<AIManager>();
            characterColliders = new Collider[10];
        }

        public void AILogicHandler_Updater()
        {
            DetectForTarget();
        }

        private void DetectForTarget()
        {
            VisuallyDetectTarget();
            memory.UpdateVisualTargets(aiManager);
            memory.ForgetVisualTarget(aiManager, memorySpan);

            EvaluateVisualTargetScore();
        }

        bool isInSight(CharacterManager potentialTarget)
        {
            Vector3 targetDirection = (potentialTarget.transform.position - aiManager.transform.position).normalized;
            float viewableAngle = Vector3.Angle(targetDirection, aiManager.transform.forward);
            
            if (viewableAngle < (aiManager.enemyDetectionScript.viewAngle / 2))
            {
                if (!Physics.Linecast
                    (aiManager.transform.position, potentialTarget.transform.position, aiManager.enemyDetectionScript.obstacleMask))
                {
                    return true;
                }
            }
            return false;
        }

        void VisuallyDetectTarget()
        {
            Physics.OverlapSphereNonAlloc(aiManager.transform.position, aiManager.enemyDetectionScript.viewRadius,
                characterColliders, aiManager.enemyDetectionScript.targetMask);

            characterManagers.Clear();
            foreach(Collider characterCollider in characterColliders)
            {
                if(characterCollider == null)
                {
                    continue;
                }
                
                CharacterManager character = characterCollider.GetComponentInParent<CharacterManager>();
                if (character != null)
                {
                    if(characterManagers.Contains(character) == true)
                    {
                        continue;
                    }

                    if(aiManager.characterType != character.characterType)
                    {
                        if(isInSight(character))
                        {
                            characterManagers.Add(character);
                        }
                    }
                }
            }
        }

        void EvaluateVisualTargetScore()
        {
            //aiManager.target.ClearDetails();
            for(int i = 0; i < aiManager.possibleVisualTargets.Count; i++)
            {
                Target target = aiManager.possibleVisualTargets[i];
                target.targetScore = CalculateVisualTargetScore(target);

                if(aiManager.target.source == null || target.targetScore > currentVisualTarget.targetScore)
                {
                    aiManager.target = target;
                    currentVisualTarget = target;
                }
            }
        }

        float Normalize(float minValue, float maxValue)
        {
            return 1 - (minValue / maxValue);
        }

        float CalculateVisualTargetScore(Target target)
        {
            float ageScore = Normalize(target.Age, memorySpan) * ageWeight;
            float angleScore = Normalize(target.targetDetectAngle, aiManager.enemyDetectionScript.viewAngle) * angleWeight;
            float distanceScore = Normalize(target.targetDistance, aiManager.enemyDetectionScript.viewRadius) * distanceWeight;

            float targetScore = distanceScore + angleScore + ageScore;
            return targetScore;
        }
    }
}
