using UnityEngine;

namespace Creotly_Studios
{
    public class EquipWeapon : StateMachineBehaviour
    {
        private CharacterManager characterManager;

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(characterManager == null)
            {
                characterManager = animator.GetComponent<CharacterManager>();
            }

            WeaponManager currentWeaponManager = characterManager.characterInventoryManager.currentWeaponManager;
            if(currentWeaponManager != null)
            {
                characterManager.characterAnimationManager.PlayTargetAnimation(AnimatorHashNames.equipWeapon, true);
            }
            else
            {
                characterManager.characterAnimationManager.PlayTargetAnimation("New State", true);
            }
        }
    }
}
