using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CircleRotation : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MyGameManager myGameManager = animator.GetComponent<Circle>().MyGameManager;
        HexagonManager hexagonManager = myGameManager.HexagonManager;

        foreach (var item in hexagonManager.Closest3Tranforms)
        {
            item.Key.parent = hexagonManager.transform;
        }

        List<KeyValuePair<Transform, float>> closest3TransformList = hexagonManager.Closest3Tranforms.ToList();
        for (int i = 0; i < closest3TransformList.Count; i++)
        {
            Color closesestColor = closest3TransformList[i].Key.GetComponent<SpriteRenderer>().color;

            Dictionary<Color, List<Transform>> colorCount = hexagonManager.CalcColorCount(closest3TransformList[i].Key.position);

            //colorCount[closesestColor].Add(closest3TransformList[i].Key);
            HashSet<Transform> willDestroy = new HashSet<Transform>();
            if (colorCount[closesestColor].Count > 1)
            {
                hexagonManager.DetermineWhichWillDestroy(closesestColor, colorCount, willDestroy);
                hexagonManager.DestroyDetermined(closest3TransformList, i, willDestroy);

                return;
            }
        }
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