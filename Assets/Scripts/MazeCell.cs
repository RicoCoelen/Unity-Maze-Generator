using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeCell
{
    public int row;
    public int column;
    public bool Visited = false;
    public GameObject UpWall;
    public GameObject DownWall;
    public GameObject LeftWall;
    public GameObject RightWall;
}
