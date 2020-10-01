using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Cell {

    private Cell _CellUp;
    private Cell _CellRight;
    private Cell _CellBottom;
    private Cell _CellLeft;

    public bool Visited = false;

    public GameObject GOupWall;
    public GameObject GOdownWall;
    public GameObject GOleftWall;
    public GameObject GOrightWall;

    public bool UpWall = true;
    public bool DownWall = true;
    public bool LeftWall = true;
    public bool RightWall = true;

    public int RowPos { get; set; }
    public int ColPos { get; set; }

    public Maze Maze { get; private set; }

    public Cell CellUp {
        private set { _CellUp = value; }
        get {
            if (RowPos - 1 > 0)
                return Maze.Cellen.Where(c => c.ColPos == this.ColPos && c.RowPos == this.RowPos - 1).FirstOrDefault();
            return null;
        }
    }
    public Cell CellRight
    {
        private set { _CellRight = value; }
        get {
            if (ColPos + 1 < this.Maze.Width)
                return Maze.Cellen.Where(c => c.ColPos == this.ColPos + 1 && c.RowPos == this.RowPos).FirstOrDefault();
            return null;
        }
    }
    public Cell CellBottom
    {
        private set { _CellBottom = value; }
        get {
            if (RowPos + 1 < this.Maze.Height)
                return Maze.Cellen.Where(c => c.ColPos == this.ColPos && c.RowPos == this.RowPos + 1).FirstOrDefault();
            return null;
        }
    }
    public Cell CellLeft
    {
        private set { _CellLeft = value; }
        get {
            if (ColPos - 1 > 0)
                return Maze.Cellen.Where(c => c.ColPos == this.ColPos - 1 && c.RowPos == this.RowPos).FirstOrDefault();
            return null;
        }
    }

    public Cell(int x, int y, Maze maze) {
        this.RowPos = x;
        this.ColPos = y;
        this.Maze = maze;
    }

    public List<Cell> GetBuren() {
        var buren = new List<Cell>();
        buren.Add(CellUp);
        buren.Add(CellRight);
        buren.Add(CellBottom);
        buren.Add(CellLeft);
        return buren;
    }
}


