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
    [SerializeField]
    private UI ui;
    private ((float x0, float x1) X, (float y0, float y1) Y) mouseArea;

    public MyGrid MyGrid { get => myGrid; set => myGrid = value; }
    public HexagonManager HexagonManager { get => hexagonManager; set => hexagonManager = value; }
    public Circle Circle { get => circle; set => circle = value; }
    public GlobalVariables GlobalVariables { get => globalVariables; set => globalVariables = value; }
    public SpriteRenderer CircleSpriteRenderer { get => circleSpriteRenderer; set => circleSpriteRenderer = value; }
    public Text Score { get => score; set => score = value; }
    public UI UI
    {
        get { return ui; }
        set { ui = value; }
    }
    public ((float x0, float x1) X, (float y0, float y1) Y) MouseArea { get => mouseArea; set => mouseArea = value; }

    private void Awake()
    {
        globalVariables = GetComponent<GlobalVariables>();
        CircleSpriteRenderer = circle.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        MyGrid.CreateGrid(GlobalVariables.CountX, GlobalVariables.CountY);
    }

    /// <summary>
    /// Sets init positions of grid and hexagons
    /// </summary>
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

        print("size/2 : " + moveDistance.x + ", " + moveDistance.y);

        Vector3 last = MyGrid.GridPosArray[MyGrid.GridPosArray.GetLength(0) - 1, MyGrid.GridPosArray.GetLength(1) - 1];
        MouseArea = ((moveDistance.x, last.x), (moveDistance.y, last.y + MyGrid.Hexagon.GetComponent<SpriteRenderer>().size.y * GlobalVariables.HexagonScale / 2.5f));
    }

    /// <summary>
    /// Get size of the grid
    /// </summary>
    /// <returns>tuple have sizes of x and y</returns>
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

    public bool CheckInsideOfMouseArea(Vector3 point)
    {
        point = Camera.main.ScreenToWorldPoint(point);

        if (point.x < mouseArea.X.x1 && point.x > mouseArea.X.x0 && point.y < mouseArea.Y.y1 && point.y > mouseArea.Y.y0)
        {
            return true;
        }
        else return false;
    }
}