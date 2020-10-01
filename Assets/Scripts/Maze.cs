using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Maze : MonoBehaviour
{
    public List<Cell> Cellen { get; set; } // important list of all cells inside the maze

    public int Width { get; set; } // the width of the maze
    public int Height { get; set; } // the height of the maze

    // prefabs
    public GameObject Wall;
    public GameObject Floor;

    // UI vars
    public Dropdown AlgorithmDropdown;
    public InputField HeightField;
    public InputField WidthField;

    // game vars
    public Material startMat;
    public Material endMat;
    public GameObject player;

    void Start()
    {
        if (HeightField == null)
        {
            this.Height = 5;
            this.Width = 5;
        }
        else
        {
            int rows, columns;

            if (int.TryParse(HeightField.text ?? "", out rows)) this.Height = Mathf.Max(2, rows);  // set the minimum rows to 2.

            if (int.TryParse(WidthField.text ?? "", out columns)) this.Width = Mathf.Max(2, columns); // set the minimum columns to 2.
        }

        this.Cellen = new List<Cell>();

        // make new cells based on how many rows and columns
        for (int i = 0; i < this.Height; i++)
        {
            for (int j = 0; j < this.Width; j++)
            {
                this.Cellen.Add(new Cell(i, j, this));
            }
        }

        //DebugAlgorithmDropdown.GetComponent<Dropdown>().value;

        foreach (var cell in this.Cellen)
        {
            GenerateCell(cell);
        }

        // switch to change algorithims on the fly
        switch (0)
        {
            case 0:
                this.PrimsAlgorithm();
                break;

            case 1:

                break;
        }

        foreach (var cell in this.Cellen)
        {
            GenerateCell(cell);
        }
    }

    void GenerateMaze()
    {
        // destroy all the children of this transform object from last maze generate
        foreach (Transform transform in transform)
        {
            Destroy(transform.gameObject);
        }

    }

    public void PrimsAlgorithm()
    {
        // create 2 list een voor die all het pad zijn, en een voor die nog moeten gedaan worden.
        var cellenInHetPad = new List<Cell>();
        var buurCellen = new List<Cell>();

        // kies random start cell.
        var randomStartCell = Cellen[Random.Range(0, Cellen.Count)];

        // voeg random startcell aan het pad.
        cellenInHetPad.Add(randomStartCell);

        // voeg de buren toe aan een lijst.
        buurCellen.AddRange(randomStartCell.GetBuren()); 

        // while er nog buur cellen zijn, loop totdat er geen cellen meer zijn in buurcellen.
        while (buurCellen.Count != 0)
        {
            
            // random cell kiezen die het pad aanraakt.
            var randomBuurCell = buurCellen[Random.Range(0, buurCellen.Count)];

            // remove de random gekozen buurcell van buurcell lijst (later voegen we hem toe aan het pad).
            buurCellen.Remove(randomBuurCell);

            // haal de buren van de huidige buurcell en zoek naar een die element is van het pad met linq (waarin we kijken of de (mogelijke) buurcellen ook voorkomen in het pad).
            var cellUitHetPad = randomBuurCell.GetBuren().Where(c => cellenInHetPad.Contains(c)).FirstOrDefault();

            // voeg de buurcellen weer toe aan de lijst, ALLEEN waneer ze niet in het padlijst zitten en niet al in de buurlijst zitten.
            var buurCellenVanRandomBuurCell = randomBuurCell.GetBuren().Where(c => !buurCellen.Contains(c) && !cellenInHetPad.Contains(c));

            // voeg de buurcellen van de random buurcell weer toe aan de buurcellen lijst.
            buurCellen.AddRange(buurCellenVanRandomBuurCell);

            // muren tussen de huidige cel en de nieuwe cel weghalen.
            DestroyWall(randomBuurCell, cellUitHetPad);

            // voeg de weggehaalde buur uit de buurcellen lijst weer in de pad lijst.
            cellenInHetPad.Add(randomBuurCell);
            
        }
    }

    private void GenerateCell(Cell cell)
    {

        float size = this.Wall.transform.localScale.x;

        GameObject floor = Instantiate(this.Floor, new Vector3(cell.ColPos * size, 0, -cell.RowPos * size), Quaternion.identity);
        floor.name = "Floor_" + cell.RowPos + "_" + cell.ColPos;

        if (cell.UpWall)
        {
            GameObject upWall = Instantiate(this.Wall, new Vector3(cell.ColPos * size, 1.75f, -cell.RowPos * size + 1.25f), Quaternion.identity);
            upWall.name = "UpWall_" + cell.RowPos + "_" + cell.ColPos;
        }

        if (cell.DownWall)
        {
            GameObject downWall = Instantiate(this.Wall, new Vector3(cell.ColPos * size, 1.75f, -cell.RowPos * size - 1.25f), Quaternion.identity);
            downWall.name = "DownWall_" + cell.RowPos + "_" + cell.ColPos;
        }

        if (cell.LeftWall)
        {
            GameObject leftWall = Instantiate(this.Wall, new Vector3(cell.ColPos * size - 1.25f, 1.75f, -cell.RowPos * size), Quaternion.Euler(0, 90, 0));
            leftWall.name = "LeftWall_" + cell.RowPos + "_" + cell.ColPos;
        }

        if (cell.RightWall)
        {
            GameObject rightWall = Instantiate(this.Wall, new Vector3(cell.ColPos * size + 1.25f, 1.75f, -cell.RowPos * size), Quaternion.Euler(0, 90, 0));
            rightWall.name = "RightWall_" + cell.RowPos + "_" + cell.ColPos;
        }
    }


    private void DestroyWall(Cell originCell, Cell neighbouringCell)
    {
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
}
