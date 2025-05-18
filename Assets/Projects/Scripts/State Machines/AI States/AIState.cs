using UnityEngine;

namespace Creotly_Studios
{
    public abstract class AIState : ScriptableObject
    {
        protected Vector3 enemyDestination;
        public abstract AIState AISate_Updater(AIManager aIManager);

        public AIState SwitchState(AIState nextState, AIManager aIManager)
        {
            ResetStateParameters(aIManager);
            return nextState;
        }

        protected virtual AIState MechEnemy_Updater(AIManager aiManager)
        {
            return this;
        }

        protected virtual AIState HumanoidEnemy_Updater(AIManager aiManager)
        {
            return this;
        }

        protected virtual void ResetStateParameters(AIManager aIManager)
        {
            enemyDestination = Vector3.zero;
        }
    }
}
