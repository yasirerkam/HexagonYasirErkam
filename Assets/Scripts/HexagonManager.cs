using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexagonManager : MonoBehaviour
{
    public MyGameManager myGameManager;
    private IEnumerable<KeyValuePair<Transform, float>> closest3Tranforms;
    private Animator circleAnimator;

    public IEnumerable<KeyValuePair<Transform, float>> Closest3Tranforms { get => closest3Tranforms; set => closest3Tranforms = value; }

    private void Awake()
    {
        myGameManager = GameObject.Find("MyGameManager").GetComponent<MyGameManager>();
        circleAnimator = myGameManager.Circle.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !circleAnimator.GetCurrentAnimatorStateInfo(0).IsName("CircleRotation"))
        {
            CalcClosest3Transforms(Input.mousePosition, out closest3Tranforms);
            CreatePointObject();
            Rotate3Transform();
        }
    }

    private void CreatePointObject()
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (var kvp in Closest3Tranforms)
        {
            positions.Add(kvp.Key.position);
        }
        Vector3 center = GetCentroid(positions) - Vector3.forward;

        if (myGameManager.Circle.GetComponent<SpriteRenderer>().enabled == false)
        {
            myGameManager.Circle.GetComponent<SpriteRenderer>().enabled = true;
        }

        myGameManager.Circle.transform.position = center;
    }

    private void Rotate3Transform()
    {
        foreach (var item in Closest3Tranforms)
        {
            item.Key.parent = myGameManager.Circle.transform;
        }

        circleAnimator.SetTrigger("rotate");
    }

    private void CalcClosest3Transforms(Vector3 originPos, out IEnumerable<KeyValuePair<Transform, float>> closest3Transforms)
    {
        closest3Transforms = CalcClosestTransforms(originPos).Take(3);
    }

    public IEnumerable<KeyValuePair<Transform, float>> CalcClosestTransforms(Vector3 originPos)
    {
        IEnumerable<KeyValuePair<Transform, float>> closestTransforms;
        Dictionary<Transform, float> childDistance = new Dictionary<Transform, float>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i).transform;
            Vector2 childScrPos = Camera.main.WorldToScreenPoint(child.position);

            float distance = Vector2.Distance(childScrPos, originPos);

            childDistance.Add(child, distance);
        }

        closestTransforms = childDistance.OrderBy(kvp => kvp.Value);

        return closestTransforms;
    }

    public void DestroyDetermined(List<KeyValuePair<Transform, float>> closest3TransformList, int i, HashSet<Transform> willDestroy)
    {
        List<Vector3> emptyPos = new List<Vector3>();
        if (willDestroy.Count > 1)
        {
            foreach (var trnsfrmDestroy in willDestroy)
            {
                int x, y;
                myGameManager.MyGrid.GetXY(trnsfrmDestroy.position, out x, out y);
                //Move(myGameManager.MyGrid.GetValue(x + 1, y + 1), myGameManager.MyGrid.GetValue(x + 1, y + 1).position - Vector3.up);

                emptyPos.Add(trnsfrmDestroy.position);
                Destroy(trnsfrmDestroy.gameObject);
            }
            emptyPos.Add(closest3TransformList[i].Key.position);
            Destroy(closest3TransformList[i].Key.gameObject);

            myGameManager.MyGrid.MoveHexagonsToEmpty(myGameManager, emptyPos);
        }
    }

    public void DetermineWhichWillDestroy(Color colorAtCenterHex, Dictionary<Color, List<Transform>> colorCount, HashSet<Transform> willDestroy)
    {
        for (int k = 0; k < colorCount[colorAtCenterHex].Count - 1; k++)
        {
            for (int l = k + 1; l < colorCount[colorAtCenterHex].Count; l++)
            {
                float d = Vector3.Distance(colorCount[colorAtCenterHex][k].position, colorCount[colorAtCenterHex][l].position);
                if (d < 0.60)
                {
                    willDestroy.Add(colorCount[colorAtCenterHex][k]);
                    willDestroy.Add(colorCount[colorAtCenterHex][l]);
                }
            }
        }
    }

    public Dictionary<Color, List<Transform>> CalcColorCount(Vector3 hexAtCenter)
    {
        Dictionary<Color, List<Transform>> colorCount = new Dictionary<Color, List<Transform>>(myGameManager.GlobalVariables.Colors.Count);
        foreach (var color in myGameManager.GlobalVariables.Colors)
        {
            colorCount.Add(color, new List<Transform>());
        }

        List<KeyValuePair<Transform, float>> closestTransformList = CalcClosestTransforms(Camera.main.WorldToScreenPoint(hexAtCenter)).ToList();
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

        return colorCount;
    }

    public void SetColors()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Dictionary<Color, int> colorCount = new Dictionary<Color, int>(myGameManager.GlobalVariables.Colors.Count);
            foreach (var color in myGameManager.GlobalVariables.Colors)
            {
                colorCount.Add(color, 0);
            }

            IEnumerable<KeyValuePair<Transform, float>> closestTransforms = CalcClosestTransforms(Camera.main.WorldToScreenPoint(transform.GetChild(i).position));
            List<KeyValuePair<Transform, float>> closestTransformList = closestTransforms.ToList();
            for (int j = 1; j < closestTransformList.Count; j++)
            {
                if (closestTransformList[j].Value < 60)
                {
                    if (colorCount.ContainsKey(closestTransformList[j].Key.GetComponent<SpriteRenderer>().color))
                    {
                        colorCount[closestTransformList[j].Key.GetComponent<SpriteRenderer>().color]++;
                    }
                }
                else
                    break;
            }

            for (int j = 0; j < myGameManager.GlobalVariables.Colors.Count; j++)
            {
                int rnd = UnityEngine.Random.Range(0, myGameManager.GlobalVariables.Colors.Count);
                Color rndColor = myGameManager.GlobalVariables.Colors[rnd];

                if (colorCount[rndColor] < 2)
                {
                    transform.GetChild(i).GetComponent<SpriteRenderer>().color = rndColor;
                    break;
                }
            }
        }
    }

    public static Vector3 GetCentroid(List<Vector3> poly)
    {
        float accumulatedArea = 0.0f;
        float centerX = 0.0f;
        float centerY = 0.0f;

        for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
        {
            float temp = poly[i].x * poly[j].y - poly[j].x * poly[i].y;
            accumulatedArea += temp;
            centerX += (poly[i].x + poly[j].x) * temp;
            centerY += (poly[i].y + poly[j].y) * temp;
        }

        if (Math.Abs(accumulatedArea) < 1E-7f)
            return new Vector3();  // Avoid division by zero

        accumulatedArea *= 3f;
        return new Vector3(centerX / accumulatedArea, centerY / accumulatedArea);
    }
}