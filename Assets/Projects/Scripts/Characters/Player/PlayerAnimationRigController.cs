using UnityEngine;

namespace Creotly_Studios
{
    public class PlayerAnimationRigController : CharacterAnimatorRigController
    {
        private Transform aimTarget;
        private PlayerManager playerManager;

        protected override void Awake()
        {
            base.Awake();
            playerManager = characterManager as PlayerManager;
            aimTarget = GameObjectTools.FindChildObject("Aim Look At").transform;
        }

        protected void Start()
        {
            SetAimTarget(aimTarget);
        }
    }
}
