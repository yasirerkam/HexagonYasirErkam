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
            Dictionary<Color, List<Transform>> colorCount = new Dictionary<Color, List<Transform>>(myGameManager.GlobalVariables.Colors.Count);
            foreach (var color in myGameManager.GlobalVariables.Colors)
            {
                colorCount.Add(color, new List<Transform>());
            }

            List<KeyValuePair<Transform, float>> closestTransformList = hexagonManager.CalcClosestTransforms(Camera.main.WorldToScreenPoint(closest3TransformList[i].Key.position)).ToList();
            for (int j = 1; j < closestTransformList.Count; j++)
            {
                if (closestTransformList[j].Value < 60)
                {
                    if (colorCount.ContainsKey(closestTransformList[j].Key.GetComponent<SpriteRenderer>().color))
                    {
                        colorCount[closestTransformList[j].Key.GetComponent<SpriteRenderer>().color].Add(closestTransformList[j].Key);
                    }
                }
                else
                    break;
            }

            Color closesestColor = closest3TransformList[i].Key.GetComponent<SpriteRenderer>().color;
            //colorCount[closesestColor].Add(closest3TransformList[i].Key);
            HashSet<Transform> willDestroy = new HashSet<Transform>();
            if (colorCount[closesestColor].Count > 1)
            {
                for (int k = 0; k < colorCount[closesestColor].Count - 1; k++)
                {
                    for (int l = k + 1; l < colorCount[closesestColor].Count; l++)
                    {
                        float d = Vector3.Distance(colorCount[closesestColor][k].position, colorCount[closesestColor][l].position);
                        if (d < 0.60)
                        {
                            willDestroy.Add(colorCount[closesestColor][k]);
                            willDestroy.Add(colorCount[closesestColor][l]);
                            break;
                        }
                    }
                }

                if (willDestroy.Count > 1)
                {
                    foreach (var trnsfrms in willDestroy)
                    {
                        int x, y;
                        myGameManager.MyGrid.GetXY(trnsfrms.position, out x, out y);
                        Destroy(trnsfrms.gameObject);
                    }
                    Destroy(closest3TransformList[i].Key.gameObject);
                }

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