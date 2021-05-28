using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyGameManager : MonoBehaviour
{
    [SerializeField]
    private MyGrid myGrid;
    [SerializeField]
    private HexagonManager hexagonManager;
    [SerializeField]
    private Circle circle;
    private SpriteRenderer circleSpriteRenderer;
    private GlobalVariables globalVariables;
    [SerializeField]
    private Text score;

    public MyGrid MyGrid { get => myGrid; set => myGrid = value; }
    public HexagonManager HexagonManager { get => hexagonManager; set => hexagonManager = value; }
    public Circle Circle { get => circle; set => circle = value; }
    public GlobalVariables GlobalVariables { get => globalVariables; set => globalVariables = value; }
    public SpriteRenderer CircleSpriteRenderer { get => circleSpriteRenderer; set => circleSpriteRenderer = value; }
    public Text Score { get => score; set => score = value; }

    private void Awake()
    {
        globalVariables = GetComponent<GlobalVariables>();
        CircleSpriteRenderer = circle.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        MyGrid.CreateGrid(GlobalVariables.CountX, GlobalVariables.CountY);
    }

    public void SetInitPosition()
    {
        (float sizeX, float sizeY) = GetSize();
        Vector3 moveDistance = new Vector3(-sizeX / 2, -2 * sizeY / 3, 0);
        MyGrid.transform.position = moveDistance;

        for (int i = 0; i < MyGrid.GridPosArray.GetLength(0); i++)
        {
            for (int j = 0; j < MyGrid.GridPosArray.GetLength(1); j++)
            {
                MyGrid.GridPosArray[i, j] += moveDistance;
            }
        }
    }

    private (float sizeX, float sizeY) GetSize()
    {
        Transform[] childs = MyGrid.GetComponentsInChildren<Transform>();
        Transform firstChild = childs[1];
        Transform lastChild = childs[childs.Length - 1];

        //float sizeX = (lastChild.position.x + firstChild.lossyScale.x / 2) - (firstChild.position.x - firstChild.lossyScale.x / 2);
        //float sizeY = (lastChild.position.y + firstChild.lossyScale.y / 2) - (firstChild.position.y - firstChild.lossyScale.y / 2);

        float sizeX = (lastChild.localPosition.x - firstChild.localPosition.x);
        float sizeY = (lastChild.localPosition.y - firstChild.localPosition.y);

        return (sizeX, sizeY);
    }

    public bool CheckInsideOfGameArea(Vector3 point)
    {
        Transform[] childs = MyGrid.GetComponentsInChildren<Transform>();
        Vector3 firstChild = Camera.main.WorldToScreenPoint(childs[1].position);
        Vector3 lastChild = Camera.main.WorldToScreenPoint(childs[childs.Length - 1].position);

        if (point.x < lastChild.x && point.x > firstChild.x && point.y < lastChild.y && point.y > firstChild.y)
        {
            return true;
        }
        else return false;
    }
}