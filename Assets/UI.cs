using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    private MyGameManager myGameManager;
    public GameObject gameOver;

    public MyGameManager MyGameManager { get => myGameManager; set => myGameManager = value; }

    private void Awake()
    {
        MyGameManager = GameObject.Find("MyGameManager").GetComponent<MyGameManager>();
    }
}