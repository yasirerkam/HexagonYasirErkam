using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    [SerializeField]
    private MyGrid myGrid;
    [SerializeField]
    private HexagonManager hexagonManager;
    [SerializeField]
    private Circle circle;
    private GlobalVariables globalVariables;

    public MyGrid MyGrid { get => myGrid; set => myGrid = value; }
    public HexagonManager HexagonManager { get => hexagonManager; set => hexagonManager = value; }
    public Circle Circle { get => circle; set => circle = value; }
    public GlobalVariables GlobalVariables { get => globalVariables; set => globalVariables = value; }

    private void Awake()
    {
        globalVariables = GetComponent<GlobalVariables>();
    }
}