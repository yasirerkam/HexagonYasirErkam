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
    private Vector2 touchPosStart, touchPosEnd;
    public IEnumerable<KeyValuePair<Transform, float>> Closest3Tranforms { get => closest3Tranforms; set => closest3Tranforms = value; }
    public string LastTriggered { get; set; }
    public bool LastTriggeredBool { get; set; }
    public bool PlayAgain { get; set; }

    public Animator CircleAnimator { get => circleAnimator; set => circleAnimator = value; }

    private float deltaRotation;
    private float previousRotation;
    private float currentRotation;
    public int TriggeredCount { get; set; }

    private void Awake()
    {
        myGameManager = GameObject.Find("MyGameManager").GetComponent<MyGameManager>();
        CircleAnimator = myGameManager.Circle.GetComponent<Animator>();
        PlayAgain = false;
    }

    private void Update()
    {
        if (myGameManager.CircleSpriteRenderer.enabled == false)
        {
            CreateCircle();
        }
        else
        {
            RotateCircle();
        }

        if (circleAnimator.GetCurrentAnimatorStateInfo(0).IsName("EmptyState") && PlayAgain)
        {
            PlayAgain = false;
            Rotate3Transform(LastTriggeredBool);
        }
    }

    private void CreateCircle()
    {
        if (Input.GetMouseButtonUp(0) && !CircleAnimator.GetCurrentAnimatorStateInfo(0).IsName("CircleRotation") && !myGameManager.MyGrid.IsMoving)
        {
            CalcClosest3Transforms(Input.mousePosition, out closest3Tranforms);
            CreatePointObject();
        }
    }

    private void RotateCircle()
    {
        if (Input.GetMouseButtonDown(0) && !myGameManager.MyGrid.IsMoving)
        {
            touchPosStart = Input.mousePosition;

            deltaRotation = 0f;
            previousRotation = AngleBetweenPoints(Camera.main.WorldToScreenPoint(CircleAnimator.transform.position), (Input.mousePosition));
        }
        else if (Input.GetMouseButtonUp(0) && !myGameManager.MyGrid.IsMoving)
        {
            touchPosEnd = Input.mousePosition;

            currentRotation = AngleBetweenPoints(Camera.main.WorldToScreenPoint(CircleAnimator.transform.position), (Input.mousePosition));
            deltaRotation = Mathf.DeltaAngle(currentRotation, previousRotation);

            if (deltaRotation > 10)
            {
                TriggeredCount = 0;
                Rotate3Transform(true);
            }
            else if (deltaRotation < -10)
            {
                TriggeredCount = 0;
                Rotate3Transform(false);
            }
            else
            {
                CalcClosest3Transforms(Input.mousePosition, out closest3Tranforms);
                CreatePointObject();
            }

            previousRotation = currentRotation;
        }
    }

    private float AngleBetweenPoints(Vector2 v2Position1, Vector2 v2Position2)
    {
        Vector2 v2FromLine = v2Position2 - v2Position1;
        Vector2 v2ToLine = new Vector2(1, 0);

        float fltAngle = Vector2.Angle(v2FromLine, v2ToLine);

        // If rotation is more than 180
        Vector3 v3Cross = Vector3.Cross(v2FromLine, v2ToLine);
        if (v3Cross.z > 0)
        {
            fltAngle = 360f - fltAngle;
        }

        return fltAngle;
    }

    private void CreatePointObject()
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (var kvp in Closest3Tranforms)
        {
            positions.Add(kvp.Key.position);
        }
        Vector3 center = GetCentroid(positions) - Vector3.forward;
        myGameManager.Circle.transform.position = center;

        myGameManager.CircleSpriteRenderer.enabled = true;
    }

    public void Rotate3Transform(bool inverse)
    {
        LastTriggeredBool = inverse;

        if (inverse == false)
        {
            CircleAnimator.SetTrigger("rotateClock");
        }
        else
        {
            CircleAnimator.SetTrigger("rotateClockInverse");
        }

        TriggeredCount++;
    }

    public void ChangeParent(Transform parent)
    {
        foreach (var item in Closest3Tranforms)
        {
            item.Key.parent = parent;
        }
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

    public void DestroyDetermined(Transform transformCenter, HashSet<Transform> willDestroy)
    {
        List<Vector3> emptyPos = new List<Vector3>();
        if (willDestroy.Count > 1)
        {
            myGameManager.CircleSpriteRenderer.enabled = false;

            foreach (var trnsfrmDestroy in willDestroy)
            {
                int x, y;
                myGameManager.MyGrid.GetXY(trnsfrmDestroy.position, out x, out y);
                //Move(myGameManager.MyGrid.GetValue(x + 1, y + 1), myGameManager.MyGrid.GetValue(x + 1, y + 1).position - Vector3.up);

                emptyPos.Add(trnsfrmDestroy.position);
                Destroy(trnsfrmDestroy.gameObject);
                myGameManager.GlobalVariables.Score += myGameManager.GlobalVariables.ScoreIncreaseAmount;
            }
            emptyPos.Add(transformCenter.position);
            Destroy(transformCenter.gameObject);
            myGameManager.GlobalVariables.Score += myGameManager.GlobalVariables.ScoreIncreaseAmount;

            myGameManager.MyGrid.MoveHexagonsToEmpty(emptyPos);
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