using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    [SerializeField]
    private List<Color> colors;
    private float creatingDistance;
    private float droppingTime;
    private float hexagonScale;

    public float HexagonScale
    {
        get { return hexagonScale; }
        set { hexagonScale = value; }
    }

    public List<Color> Colors { get => colors; set => colors = value; }
    public float CreatingDistance { get => creatingDistance; set => creatingDistance = value; }

    public float DroppingTime
    {
        get { return droppingTime; }
        set { droppingTime = value; }
    }

    private void Awake()
    {
        CreatingDistance = 6;
        DroppingTime = .15f;
        HexagonScale = .23f;
    }
}