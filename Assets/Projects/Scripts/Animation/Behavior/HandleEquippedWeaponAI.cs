using UnityEngine;

namespace Creotly_Studios
{
    public class HandleEquippedWeaponAI : StateMachineBehaviour
    {
        [SerializeField] private bool status;
        private AIInventoryManager inventoryManager;

        //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //inventoryManager = animator.GetComponent<AIInventoryManager>();
            //if (inventoryManager != null)
            //{
            //    inventoryManager.hasEquippedWeapon = status;
            //}
        }
    }
}
