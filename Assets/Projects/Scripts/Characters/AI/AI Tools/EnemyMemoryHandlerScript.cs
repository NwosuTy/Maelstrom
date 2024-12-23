namespace Creotly_Studios
{
    public class EnemyMemoryHandlerScript
    {
        public void UpdateVisualTargets(AIManager aiManager)
        {
            for(int i = 0; i < aiManager.aiLogicHandler.characterManagers.Count; i++)
            {
                CharacterManager target = aiManager.aiLogicHandler.characterManagers[i];
                RefreshVisualTarget(aiManager, target);
            }
        }
        
        private void RefreshVisualTarget(AIManager aiManager, CharacterManager target)
        {
            Target visualTarget = FetchVisualTarget(aiManager, target);

            visualTarget.visualTarget = target;
            visualTarget.SetDetails(TargetType.Visual, null, target);
            visualTarget.CalculateParameters(aiManager.transform);
        }

        private Target FetchVisualTarget(AIManager aiManager, CharacterManager target)
        {
            Target visualTarget = aiManager.possibleVisualTargets.Find(x => x.visualTarget == target);
            if(visualTarget == null)
            {
                visualTarget = new Target();
                aiManager.possibleVisualTargets.Add(visualTarget);
            }
            return visualTarget;
        }

        public void ForgetVisualTarget(AIManager aiManager, float olderThan)
        {
            aiManager.possibleVisualTargets.RemoveAll(x => x.Age > olderThan);
            aiManager.possibleVisualTargets.RemoveAll(x => x.visualTarget == null);
            aiManager.possibleVisualTargets.RemoveAll(x => x.visualTarget?.isDead == true);
        }
    }
}
