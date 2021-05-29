using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private int countDown;
    public Sprite[] bombs;
    public MyGameManager MyGameManager { get; set; }
    public HexagonManager HexagonManager { get; set; }

    public int CountDown
    {
        get { return countDown; }
        set
        {
            countDown = value;
            if (value < 0)
            {
                MyGameManager.UI.gameOver.SetActive(true);
            }
            else
                GetComponent<SpriteRenderer>().sprite = bombs[value];
        }
    }

    private void Awake()
    {
        HexagonManager = transform.parent.GetComponent<HexagonManager>();
        MyGameManager = HexagonManager.MyGameManager;
    }

    private void Start()
    {
    }

    private void Update()
    {
    }
}