using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Creotly_Studios
{
    public class AIAnimationRigController : CharacterAnimatorRigController
    {
        private AIManager aiManager;

        protected override void Awake()
        {
            base.Awake();
            aiManager = characterManager as AIManager;
        }
    }
}