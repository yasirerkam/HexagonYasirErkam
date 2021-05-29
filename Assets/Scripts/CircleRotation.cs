using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class CircleRotation : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MyGameManager myGameManager = animator.GetComponent<Circle>().MyGameManager;
        HexagonManager hexagonManager = myGameManager.HexagonManager;

        hexagonManager.ChangeParent(myGameManager.Circle.transform);
        hexagonManager.IsRotating = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float eulerZ = animator.transform.rotation.eulerAngles.z;
        if (Math.Abs(eulerZ - 0) < 40)
        {
            animator.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (Math.Abs(eulerZ - 120) < 40)
        {
            animator.transform.rotation = Quaternion.Euler(0, 0, 120);
        }
        else if (Math.Abs(eulerZ - 240) < 40)
        {
            animator.transform.rotation = Quaternion.Euler(0, 0, 240);
        }
        else if (Math.Abs(eulerZ - 360) < 40)
        {
            animator.transform.rotation = Quaternion.Euler(0, 0, 360);
        }

        MyGameManager myGameManager = animator.GetComponent<Circle>().MyGameManager;
        HexagonManager hexagonManager = myGameManager.HexagonManager;

        hexagonManager.ChangeParent(hexagonManager.transform);

        myGameManager.MyGrid.CheckTrans3Matches();
        hexagonManager.IsRotating = false;
    }

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