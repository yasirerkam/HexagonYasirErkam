using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MyGrid : MonoBehaviour
{
    private MyGameManager myGameManager;
    public GameObject hexagon;
    public float distanceOfHexagons;
    private int countY;
    private int countX;
    private Vector3[,] gridPosArray;

    private float DroppingTime { get; set; }
    public MyGameManager MyGameManager { get => myGameManager; set => myGameManager = value; }
    public Vector3[,] GridPosArray { get => gridPosArray; set => gridPosArray = value; }
    public GameObject Hexagon { get => hexagon; set => hexagon = value; }

    private void Awake()
    {
        MyGameManager = GameObject.Find("MyGameManager").GetComponent<MyGameManager>();
        distanceOfHexagons = 0.025f;
        DroppingTime = myGameManager.GlobalVariables.DroppingTime;
    }

    public void CreateGrid(int countX, int countY, float cellSize)
    {
        this.countY = countY;
        this.countX = countX;

        GridPosArray = new Vector3[countX, countY];

        bool showDebug = true;
        if (showDebug)
        {
            TextMesh[,] debugTextArray = new TextMesh[countY, countX];

            for (int x = 0; x < countX; x++)
            {
                for (int y = 0; y < countY; y++)
                {
                    GameObject go;

                    go = Instantiate(Hexagon, GetWorldPosition(x, y), Quaternion.identity, transform);
                    go.transform.localScale *= MyGameManager.GlobalVariables.HexagonScale;

                    GridPosArray[x, y] = go.transform.position;

                    //Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    //Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            //Debug.DrawLine(GetWorldPosition(0, countX), GetWorldPosition(countY, countX), Color.white, 100f);
            //Debug.DrawLine(GetWorldPosition(countY, 0), GetWorldPosition(countY, countX), Color.white, 100f);
        }

        MyGameManager.HexagonManager.SetColors();
        MyGameManager.SetInitPosition();
    }

    public int GetWidth()
    {
        return countY;
    }

    public int GetHeight()
    {
        return countX;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        Vector3 pos = transform.position + new Vector3(x, y) * Hexagon.GetComponent<SpriteRenderer>().size.x * MyGameManager.GlobalVariables.HexagonScale - new Vector3(x * Hexagon.GetComponent<SpriteRenderer>().size.x * MyGameManager.GlobalVariables.HexagonScale * .125f, 0, 0);

        if (x % 2 == 0)
        {
            return pos + new Vector3(0, Hexagon.GetComponent<SpriteRenderer>().size.x * MyGameManager.GlobalVariables.HexagonScale / 2, 0);
        }
        else
        {
            return pos;
        }
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        for (int i = 0; i < gridPosArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridPosArray.GetLength(1); j++)
            {
                if (Vector3.Distance(gridPosArray[i, j], worldPosition) < .3f)
                {
                    x = i;
                    y = j;
                    return;
                }
            }
        }

        throw new Exception();

        //x = Mathf.FloorToInt(((worldPosition - transform.position).x + cellSize / 2) / cellSize);

        //if ((Mathf.FloorToInt((worldPosition - transform.position).y % 0.25f)) % 2 == 0)
        //    y = Mathf.FloorToInt(((worldPosition - transform.position).y - 0.25f) / cellSize);
        //else
        //    y = Mathf.FloorToInt((worldPosition - transform.position).y / cellSize);
    }

    public Transform GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < countX && y < countY)
        {
            var tranforms = GetComponentsInChildren<Transform>();
            for (int i = 1; i < tranforms.Length; i++)
            {
                if (Vector3.Distance(tranforms[i].position, gridPosArray[x, y]) < .3f)
                {
                    return tranforms[i];
                }
            }
        }

        throw new Exception();
    }

    public Transform GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }

    public void MoveHexagonsToEmpty(List<Vector3> emptyPositions)
    {
        Dictionary<int, List<int>> xyDictEmptyList = new Dictionary<int, List<int>>();
        List<(int x, int y)> xyEmptyList = new List<(int x, int y)>();

        for (int i = 0; i < emptyPositions.Count; i++)
        {
            int x, y;

            MyGameManager.MyGrid.GetXY(emptyPositions[i], out x, out y);

            xyEmptyList.Add((x, y));
        }

        foreach (var xy in xyEmptyList)
        {
            if (!xyDictEmptyList.ContainsKey(xy.x))
            {
                xyDictEmptyList.Add(xy.x, new List<int>());
            }
        }

        foreach (var xy in xyEmptyList)
        {
            xyDictEmptyList[xy.x].Add(xy.y);
        }

        foreach (var xyEmpty in xyDictEmptyList)
        {
            if (xyEmpty.Value.Count == 2)
            {
                int y;
                if (xyEmpty.Value[0] < xyEmpty.Value[1])
                    y = xyEmpty.Value[0];
                else
                    y = xyEmpty.Value[1];
                try
                {
                    Transform trnsfrm = GetValue(xyEmpty.Key, y + 2);
                    IEnumerator coroutine = Move(trnsfrm, xyEmpty, y, DroppingTime, 2);
                    StartCoroutine(coroutine);
                }
                catch (Exception)
                {
                    CreateAndFall(xyEmpty, y, 2);
                }
            }
            else if (xyEmpty.Value.Count == 1)
            {
                int y = xyEmpty.Value[0];

                try
                {
                    Transform trnsfrm = GetValue(xyEmpty.Key, y + 1);
                    IEnumerator coroutine = Move(trnsfrm, xyEmpty, y, DroppingTime, 1);
                    StartCoroutine(coroutine);
                }
                catch (Exception)
                {
                    CreateAndFall(xyEmpty, y, 1);
                }
            }
        }
    }

    private IEnumerator Move(Transform trnsfrm, KeyValuePair<int, List<int>> xyEmpty, int y, float duration, int hexCount)
    {
        Assert.IsNotNull(trnsfrm, "Transform can't be Null");
        Vector3 targetPosition = GetWorldPosition(xyEmpty.Key, y);
        float time = 0;
        Vector3 startPosition = trnsfrm.position;

        while (time < duration)
        {
            trnsfrm.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        trnsfrm.position = targetPosition;

        yield return new WaitForEndOfFrame();

        y++;
        if (y < countY - hexCount)
        {
            Transform trnsfrmP = GetValue(xyEmpty.Key, y + hexCount);
            IEnumerator coroutine = Move(trnsfrmP, xyEmpty, y, DroppingTime, hexCount);
            StartCoroutine(coroutine);
        }
        else
        {
            CreateAndFall(xyEmpty, y, hexCount);
        }
    }

    private void CreateAndFall(KeyValuePair<int, List<int>> xy, int y, int hexCount)
    {
        if (y < countY)
        {
            var go = Instantiate(Hexagon, GetWorldPosition(xy.Key, y) + new Vector3(0, MyGameManager.GlobalVariables.CreatingDistance, 0), Quaternion.identity, transform);
            go.transform.localScale *= MyGameManager.GlobalVariables.HexagonScale;
            go.GetComponent<SpriteRenderer>().color = MyGameManager.GlobalVariables.Colors[UnityEngine.Random.Range(0, MyGameManager.GlobalVariables.Colors.Count)];
            Transform trnsfrmP = go.transform;
            IEnumerator coroutine = Move(trnsfrmP, xy, y, DroppingTime, hexCount);
            StartCoroutine(coroutine);
        }
        else
        {
            if (IsInvoking("CheckMatches") == false)
            {
                Invoke("CheckMatches", .75f);
            }
        }
    }

    public void CheckMatches()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>();

        for (int i = 1; i < transforms.Length; i++)
        {
            int x, y;
            GetXY(transforms[i].position, out x, out y);
            Transform trns = transforms[i];

            if (CheckMatche(trns))
            {
                break;
            }
        }
    }

    public bool CheckMatche(Transform trns)
    {
        Color closesestColor = trns.GetComponent<SpriteRenderer>().color;
        Dictionary<Color, List<Transform>> colorCount = MyGameManager.HexagonManager.CalcColorCount(trns.position);

        //colorCount[closesestColor].Add(closest3TransformList[i].Key);
        HashSet<Transform> willDestroy = new HashSet<Transform>();
        if (colorCount[closesestColor].Count > 1)
        {
            MyGameManager.HexagonManager.DetermineWhichWillDestroy(closesestColor, colorCount, willDestroy);
            MyGameManager.HexagonManager.DestroyDetermined(trns, willDestroy);
            if (willDestroy.Count > 0)
            {
                return true;
            }
        }

        return false;
    }
}