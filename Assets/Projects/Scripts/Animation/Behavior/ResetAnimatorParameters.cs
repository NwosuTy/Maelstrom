using UnityEngine;

namespace Creotly_Studios
{
    public class MultipleAnimatorReseter : StateMachineBehaviour
    {
        bool isEnemy;
        CharacterManager characterManager;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(characterManager == null)
            {
                characterManager = animator.GetComponent<CharacterManager>();
                isEnemy = characterManager.characterType == CharacterType.Enemy;
            }

            characterManager.isJumping = false;
            animator.SetBool(AnimatorHashNames.interactHash, false);
            animator.SetBool(AnimatorHashNames.canRotateHash, isEnemy);
            animator.SetBool(AnimatorHashNames.isReloadingHash, false);
            animator.SetBool(AnimatorHashNames.rootMotionRotateHash, true);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}
    }
}
