using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    [SerializeField]
    private List<Color> colors;

    public List<Color> Colors { get => colors; set => colors = value; }
}