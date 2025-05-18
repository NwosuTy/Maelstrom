using UnityEngine;

namespace Creotly_Studios
{
    public class CoverIdleAnim : StateMachineBehaviour
    {
        int idleAnimHash;
        CoverDetector detector;

        //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (detector == null)
            {
                detector = animator.GetComponent<CoverDetector>();
                idleAnimHash = Animator.StringToHash("IdleAnim");
            }

            int idleAnim = 0;
            if (detector.atLeftEdge)
            {
                idleAnim = 1;
            }
            else if (detector.atRightEdge)
            {
                idleAnim = -1;
            }
            animator.SetFloat(idleAnimHash, idleAnim);
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
