using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

    [Header("Basic Stats")]
    public new string name = "Error";
    public int hp = 5;
    public int movement = 4;

    [Space(10)]

    [Header ("Attributes")]
    public int Str = 1;
    public int Int = 1;
    public int Agi = 1;
    public int Des = 1;
}