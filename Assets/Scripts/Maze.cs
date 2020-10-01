﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Maze : MonoBehaviour
{
    #region Properties

    internal List<Cell> Cells { get; set; } // Important list of all cells inside the maze

    internal int Width { get; set; } = 10; // The default width of the maze
    internal int Height { get; set; } = 10; // The default height of the maze

    // Prefabs
    [SerializeField]
    private GameObject Wall;
    [SerializeField]
    private GameObject Floor;

    // UI vars
    [SerializeField]
    private Dropdown AlgorithmDropdown;
    [SerializeField]
    private InputField HeightField;
    [SerializeField]
    private InputField WidthField;

    // Game vars
    [SerializeField]
    private Material StartMat;
    [SerializeField]
    private Material EndMat;
    [SerializeField]
    private GameObject Player;

    // Most important cells
    private Cell StartCell => this.Cells.Where(c => c.ColPos == 0 && c.RowPos == 0).First();
    private Cell EndCell => this.Cells.Where(c => c.ColPos == this.Width - 1 && c.RowPos == this.Height - 1).First();

    //Debug Mode 
    public bool DEBUG_MODE = false;

    #endregion

    #region Monobehavior Constructor

    /// <summary>
    /// Standard method of Monobehaviour Script
    /// </summary>
    void Start()
    {
        GenerateMaze();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Generate the maze based upon user input
    /// </summary>
    public void GenerateMaze()
    {
        // destroy all the children of this transform object from last maze generate
        foreach (Transform transform in transform)
        {
            Destroy(transform.gameObject);
        }

        // get inputs
        if (HeightField.text != null)
        {
            // temp
            int rows, columns;

            // try get user input and
            if (int.TryParse(HeightField.text ?? "", out rows)) this.Height = Mathf.Max(2, rows);  // set the minimum rows to 2.
            if (int.TryParse(WidthField.text ?? "", out columns)) this.Width = Mathf.Max(2, columns); // set the minimum columns to 2.
        }

        // clear cellen list for new generation
        this.Cells = new List<Cell>();

        // make new cells based on how many rows and columns
        for (int i = 0; i < this.Height; i++)
        {
            for (int j = 0; j < this.Width; j++)
            {
                this.Cells.Add(new Cell(i, j, this));
            }
        }

        // get value of dropdown and use the switch to change algorithims on the fly
        switch (AlgorithmDropdown.GetComponent<Dropdown>().value)
        {
            case 0:
                this.PrimsAlgorithm();
                break;

            case 1:
                // DFS
                break;
        }

        // generate each cell walls, floors and objects
        foreach (var cell in this.Cells)
        {
            GenerateCell(cell);
        }

        // after generation change cam pos
        ChangeCameraPosition();
    }

    /// <summary>
    /// Change camera position based on dimensions of the grid/maze
    /// </summary>
    private void ChangeCameraPosition()
    {
        // get scale of wall and current camera pos
        float size = Wall.transform.localScale.x;
        Vector3 cameraPosition = Camera.main.transform.position;

        // using modulo to center camera with odd numbers for columns
        if (Width % 2 == 0) {
            cameraPosition.x = (Mathf.Round(Width/ 2) * size) - 1.5f;
        }
        else {
            cameraPosition.x = Mathf.Round(Width / 2) * size;
        }

        // using modulo to center camera with odd numbers for rows
        if (Height % 2 == 0)  {
            cameraPosition.z = (Mathf.Round(-Height / 2) * size) + 1.5f;
        }
        else {
            cameraPosition.z = Mathf.Round(-Height / 2) * size;
        }

        // change pos and orthographic size
        Camera.main.transform.position = cameraPosition;
        Camera.main.orthographicSize = Mathf.Max(Width, Height) * 1.6f;
    }

    /// <summary>
    /// Use prims algorithim to carve a path trough the grid
    /// </summary>
    private void PrimsAlgorithm()
    {
        // create 2 list een voor die all het pad zijn, en een voor die nog moeten gedaan worden.
        var pathCells = new List<Cell>();
        var neighbourCells = new List<Cell>();

        // kies random start cell.
        var randomStartCell = Cells[Random.Range(0, Cells.Count)];

        if (DEBUG_MODE) Debug.Log("Startcell: ");
        if (DEBUG_MODE) Debug.Log(randomStartCell);

        // voeg random startcell aan het pad.
        pathCells.Add(randomStartCell);

        // voeg de buren toe aan een lijst.
        neighbourCells.AddRange(randomStartCell.GetNeighbours()); 

        // while er nog buur cellen zijn, loop totdat er geen cellen meer zijn in buurcellen.
        while (neighbourCells.Count > 0)
        {
            if (DEBUG_MODE) Debug.Log($"Iteratie {pathCells.Count - 1}");
            if (DEBUG_MODE) Debug.Log($"Er zijn reeds {pathCells.Count} cellen op het pad");
            if (DEBUG_MODE) Debug.Log($"Er zijn nu {neighbourCells.Count} mogelijke buren");

            // random cell kiezen die het pad aanraakt.
            var index = Random.Range(0, neighbourCells.Count - 1);
            var randomNeighbourCell = neighbourCells[index];

            if (DEBUG_MODE) Debug.Log($"De random cell die gekozen werd uit burenlijst heeft index: {index}");
            if (DEBUG_MODE) Debug.Log(randomNeighbourCell);

            // remove de random gekozen buurcell van buurcell lijst (later voegen we hem toe aan het pad).
            neighbourCells.Remove(randomNeighbourCell);

            // haal de buren van de huidige buurcell en zoek naar een die element is van het pad met linq (waarin we kijken of de (mogelijke) buurcellen ook voorkomen in het pad).
            var pathCell = randomNeighbourCell.GetNeighbours().Where(c => pathCells.Contains(c)).FirstOrDefault();

            // voeg de buurcellen weer toe aan de lijst, ALLEEN waneer ze niet in het padlijst zitten en niet al in de buurlijst zitten.
            var pathCellNeighbourCells = randomNeighbourCell.GetNeighbours().Where(c => !neighbourCells.Contains(c) && !pathCells.Contains(c));

            // voeg de buurcellen van de random buurcell weer toe aan de buurcellen lijst.
            neighbourCells.AddRange(pathCellNeighbourCells);

            // muren tussen de huidige cel en de nieuwe cel weghalen.
            DestroyWall(randomNeighbourCell, pathCell);

            // voeg de weggehaalde buur uit de buurcellen lijst weer in de pad lijst.
            pathCells.Add(randomNeighbourCell);

            if (DEBUG_MODE) Debug.Log($"Er zitten nu {pathCells.Count} cellen in het pad");
        }
    }

    /// <summary>
    /// Create Gameobjects based on the provided cell
    /// </summary>
    /// <param name="cell">The cell to be instantiated</param>
    private void GenerateCell(Cell cell)
    {
        // get the x scale of the wall prefab for calculations
        float size = this.Wall.transform.localScale.x;

        // instantiate the wall with cell variables, and change the name for cleaner overview
        GameObject floor = Instantiate(this.Floor, new Vector3(cell.ColPos * size, 0, -cell.RowPos * size), Quaternion.identity);
        floor.name = "Floor_" + cell.RowPos + "_" + cell.ColPos;

        // add this object to the maze game object for cleaner hierarchy.
        floor.transform.parent = transform; 

        
        if (cell.Equals(this.StartCell)) {
            // when generating start cell also spawn player, change name and material of object
            floor.GetComponent<MeshRenderer>().material = StartMat;
            floor.name = "StartFloor";
            GameObject tempPlayer = Instantiate(Player);

            // add this object to the maze game object for cleaner hierarchy.
            tempPlayer.transform.parent = transform;
        }

        if (cell.Equals(this.EndCell))
        {   // when generating end cell also change name and material of object
            floor.GetComponent<MeshRenderer>().material = EndMat;
            floor.name = "EndFloor";
        }

        if (cell.UpWall)
        {
            // instantiate the wall with cell variables, and change the name for cleaner overview
            GameObject upWall = Instantiate(this.Wall, new Vector3(cell.ColPos * size, 1.75f, -cell.RowPos * size + 1.25f), Quaternion.identity);
            upWall.name = "UpWall_" + cell.RowPos + "_" + cell.ColPos;

            // add this object to the maze game object for cleaner hierarchy.
            upWall.transform.parent = transform; 
        }

        if (cell.DownWall)
        {
            // instantiate the wall with cell variables, and change the name for cleaner overview
            GameObject downWall = Instantiate(this.Wall, new Vector3(cell.ColPos * size, 1.75f, -cell.RowPos * size - 1.25f), Quaternion.identity);
            downWall.name = "DownWall_" + cell.RowPos + "_" + cell.ColPos;

            // add this object to the maze game object for cleaner hierarchy.
            downWall.transform.parent = transform; 
        }

        if (cell.LeftWall)
        {
            // instantiate the wall with cell variables, and change the name for cleaner overview
            GameObject leftWall = Instantiate(this.Wall, new Vector3(cell.ColPos * size - 1.25f, 1.75f, -cell.RowPos * size), Quaternion.Euler(0, 90, 0));
            leftWall.name = "LeftWall_" + cell.RowPos + "_" + cell.ColPos;

            // add this object to the maze game object for cleaner hierarchy.
            leftWall.transform.parent = transform; 
        }

        if (cell.RightWall)
        {
            // instantiate the wall with cell variables, and change the name for cleaner overview
            GameObject rightWall = Instantiate(this.Wall, new Vector3(cell.ColPos * size + 1.25f, 1.75f, -cell.RowPos * size), Quaternion.Euler(0, 90, 0));
            rightWall.name = "RightWall_" + cell.RowPos + "_" + cell.ColPos;

            // add this object to the maze game object for cleaner hierarchy.
            rightWall.transform.parent = transform; 
        }
    }

    /// <summary>
    /// Destroy walls inbetween provided cells
    /// </summary>
    /// <param name="originCell">The origin cell</param>
    /// <param name="neighbouringCell">The neighboring cell</param>
    private void DestroyWall(Cell originCell, Cell neighbouringCell)
    {
        if (DEBUG_MODE) Debug.Log("We verwijderen de muren tussen");
        if (DEBUG_MODE) Debug.Log(originCell);
        if (DEBUG_MODE) Debug.Log(neighbouringCell);
        if (DEBUG_MODE) Debug.Log("De muur is nu weg!");

        // well calculate the delta and get the direction of wall we need to remove.
        int deltaX = originCell.ColPos - neighbouringCell.ColPos;
        int deltaY = originCell.RowPos - neighbouringCell.RowPos;

        // since we working with grid we have 4 scenarios, well change the vars to text to make it easier to read.
        switch ($"{deltaX}_{deltaY}")
        {
            // in relation to origin left
            case "1_0":
                originCell.LeftWall = false;
                neighbouringCell.RightWall = false;
                break;

            // in relation to origin top
            case "0_1":
                originCell.UpWall = false;
                neighbouringCell.DownWall = false;
                break;

            // in relation to origin right
            case "-1_0":
                originCell.RightWall = false;
                neighbouringCell.LeftWall = false;
                break;

            // in relation to origin bottom
            case "0_-1":
                originCell.DownWall = false;
                neighbouringCell.UpWall = false;
                break;

            default:
                //throw new Exception();
                break;
        }
    }

    #endregion
}
