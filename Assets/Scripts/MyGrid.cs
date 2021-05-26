using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGrid : MonoBehaviour
{
    public float distanceOfHexagons;
    public GameObject hexagon;
    private int countY;
    private int countX;
    private float cellSize;
    private Transform[,] gridArray;
    private MyGameManager myGameManager;

    public MyGameManager MyGameManager { get => myGameManager; set => myGameManager = value; }

    private void Awake()
    {
        MyGameManager = GameObject.Find("MyGameManager").GetComponent<MyGameManager>();
        distanceOfHexagons = 0.025f;
    }

    private void Start()
    {
        CreateGrid(9, 8, .5f);
    }

    public void CreateGrid(int countX, int countY, float cellSize)
    {
        this.countY = countY;
        this.countX = countX;
        this.cellSize = cellSize;

        gridArray = new Transform[countY, countX];

        bool showDebug = true;
        if (showDebug)
        {
            TextMesh[,] debugTextArray = new TextMesh[countY, countX];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    GameObject obj;
                    if (x % 2 == 0)
                    {
                        obj = Instantiate(hexagon, GetWorldPosition(x, y) + new Vector3(cellSize * -.125f * x + distanceOfHexagons * x, cellSize * .5f * 2f + distanceOfHexagons * y), Quaternion.identity, transform);
                    }
                    else
                    {
                        obj = Instantiate(hexagon, GetWorldPosition(x, y) + new Vector3(cellSize * -.125f * x + distanceOfHexagons * x, cellSize * .5f + distanceOfHexagons * y), Quaternion.identity, transform);
                    }
                    obj.transform.localScale *= cellSize;
                    SetValue(x, y, obj.transform);

                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, countX), GetWorldPosition(countY, countX), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(countY, 0), GetWorldPosition(countY, countX), Color.white, 100f);
        }

        MyGameManager.HexagonManager.SetColors();
    }

    public int GetWidth()
    {
        return countY;
    }

    public int GetHeight()
    {
        return countX;
    }

    public float GetCellSize()
    {
        return cellSize;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + transform.position;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - transform.position).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - transform.position).y / cellSize);
    }

    public void SetValue(int x, int y, Transform value)
    {
        if (x >= 0 && y >= 0 && x < countY && y < countX)
        {
            gridArray[x, y] = value;
        }
    }

    public void SetValue(Vector3 worldPosition, Transform value)
    {
        int x, y;

        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public Transform GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < countY && y < countX)
        {
            return gridArray[x, y];
        }
        else
        {
            throw new Exception();
        }
    }

    public Transform GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }
}