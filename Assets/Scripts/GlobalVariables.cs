using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    private MyGameManager myGameManager;
    [SerializeField]
    private List<Color> colors;
    private float creatingDistance;
    private float droppingTime;
    private float hexagonScale;
    private int countX;
    private int countY;
    private int score;
    private int scoreIncreaseAmount;
    private int bombEveryXscore;

    public int BombEveryXscore
    {
        get { return bombEveryXscore; }
        set { bombEveryXscore = value; }
    }

    public int ScoreIncreaseAmount
    {
        get { return scoreIncreaseAmount; }
        set { scoreIncreaseAmount = value; }
    }

    public int Score
    {
        get { return score; }
        set
        {
            score = value;
            MyGameManager.Score.text = "Score: " + value.ToString();

            if (value != 0 && value % BombEveryXscore == 0)
            {
                MyGameManager.MyGrid.CreateBomb = true;
            }
        }
    }

    public MyGameManager MyGameManager { get => myGameManager; set => myGameManager = value; }
    public int CountY
    {
        get { return countY; }
        set { countY = value; }
    }
    public int CountX
    {
        get { return countX; }
        set { countX = value; }
    }
    public float HexagonScale
    {
        get { return hexagonScale; }
        set { hexagonScale = value; }
    }
    public float DroppingTime
    {
        get { return droppingTime; }
        set { droppingTime = value; }
    }
    public float CreatingDistance { get => creatingDistance; set => creatingDistance = value; }
    public List<Color> Colors { get => colors; set => colors = value; }

    private void Awake()
    {
        MyGameManager = GameObject.Find("MyGameManager").GetComponent<MyGameManager>();
        Score = 0;

        CreatingDistance = 6;
        DroppingTime = .15f;
        HexagonScale = .23f;
        countX = 8;
        CountY = 9;
        ScoreIncreaseAmount = 5;
        BombEveryXscore = 20;
    }
}