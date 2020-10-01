using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Cell {

    #region Properties

    // private bool Visited = false;

    internal bool UpWall = true;
    internal bool DownWall = true;
    internal bool LeftWall = true;
    internal bool RightWall = true;

    internal int RowPos { get; set; }
    internal int ColPos { get; set; }

    private Maze Maze { get; set; }

    private Cell CellUp {
        get {
            if (RowPos - 1 >= 0)
                return Maze.Cells.Where(c => c.ColPos == this.ColPos && c.RowPos == this.RowPos - 1).FirstOrDefault();
            return null;
        }
    }
    private Cell CellRight
    {
        get {
            if (ColPos + 1 < this.Maze.Width)
                return Maze.Cells.Where(c => c.ColPos == this.ColPos + 1 && c.RowPos == this.RowPos).FirstOrDefault();
            return null;
        }
    }
    private Cell CellBottom
    {
        get {
            if (RowPos + 1 < this.Maze.Height)
                return Maze.Cells.Where(c => c.ColPos == this.ColPos && c.RowPos == this.RowPos + 1).FirstOrDefault();
            return null;
        }
    }
    private Cell CellLeft
    {
        get {
            if (ColPos - 1 >= 0)
                return Maze.Cells.Where(c => c.ColPos == this.ColPos - 1 && c.RowPos == this.RowPos).FirstOrDefault();
            return null;
        }
    }

    #endregion

    #region Constructor

    public Cell(int x, int y, Maze maze) {
        this.RowPos = x;
        this.ColPos = y;
        this.Maze = maze;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Get neighboring cells
    /// </summary>
    /// <returns>List of cells</returns>
    public List<Cell> GetNeighbours() {

        var neighbours = new List<Cell>();
        
        if (CellUp != null) neighbours.Add(CellUp);
        if (CellRight!= null) neighbours.Add(CellRight);
        if (CellBottom != null) neighbours.Add(CellBottom);
        if (CellLeft != null) neighbours.Add(CellLeft);

        return neighbours;
    }

    public override string ToString()
    {
        return $"This is the cell on position X: {RowPos}, Y: {ColPos}" + 
            $"\n this is on row: {RowPos + 1}, column: {ColPos + 1}";
    }

    #endregion

}


