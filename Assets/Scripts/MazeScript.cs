using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeScript : MonoBehaviour
{
    // ui and crucial vars
    public int Rows = 2;
    public int Columns = 2;
    public GameObject Wall;
    public GameObject Floor;
    public Dropdown AlgorithmDropdown;
    public InputField HeightField;
    public InputField WidthField;

    // algorithm vars
    private MazeCell[,] grid;
    private int currentRow;
    private int currentColumn;
    private bool scanComplete;

    // game vars
    public Material startMat;
    public Material endMat;
    public GameObject player;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        // destroy all the children of this transform object.
        foreach (Transform transform in transform)
        {
            Destroy(transform.gameObject);
        }

        // first, we create the grid with all the walls and floors.
        CreateGrid();

        // then, we fix the camera position so it's centered.
        ChangeCameraPosition();

        // reset the algorithm variables.
        currentRow = 0;
        currentColumn = 0;
        scanComplete = false;

        // then we run the chosen algorithm to carve the paths.
        switch(AlgorithmDropdown.GetComponent<Dropdown>().value) 
        {
            case 0:
                // Depth First-Search
                HuntAndKill();
                break;

            case 1:
                // Randomized Prim's Algorithim
                PrimsAlgorithm();
                break;

            case 2:
                Debug.Log("None");
                break;
        }
    }

    void CreateGrid()
    {
        float size = Wall.transform.localScale.x;
        grid = new MazeCell[Rows, Columns];

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                GameObject floor = Instantiate(Floor, new Vector3(j * size, 0, -i * size), Quaternion.identity);
                floor.name = "Floor_" + i + "_" + j;

                GameObject upWall = Instantiate(Wall, new Vector3(j * size, 1.75f, -i * size + 1.25f), Quaternion.identity);
                upWall.name = "UpWall_" + i + "_" + j;

                GameObject downWall = Instantiate(Wall, new Vector3(j * size, 1.75f, -i * size - 1.25f), Quaternion.identity);
                downWall.name = "DownWall_" + i + "_" + j;

                GameObject leftWall = Instantiate(Wall, new Vector3(j * size - 1.25f, 1.75f, -i * size), Quaternion.Euler(0, 90, 0));
                leftWall.name = "LeftWall_" + i + "_" + j;

                GameObject rightWall = Instantiate(Wall, new Vector3(j * size + 1.25f, 1.75f, -i * size), Quaternion.Euler(0, 90, 0));
                rightWall.name = "RightWall_" + i + "_" + j;

                // create the maze cell and add references to its walls.
                grid[i, j] = new MazeCell();
                grid[i, j].row = i;
                grid[i, j].column = j;
                grid[i, j].UpWall = upWall;
                grid[i, j].DownWall = downWall;
                grid[i, j].LeftWall = leftWall;
                grid[i, j].RightWall = rightWall;

                // add these objects to the maze game object for cleaner hierarchy.
                floor.transform.parent = transform;
                upWall.transform.parent = transform;
                downWall.transform.parent = transform;
                leftWall.transform.parent = transform;
                rightWall.transform.parent = transform;

                // add material to start and end floor also spawn the player.
                if (i == 0 && j == 0)
                {
                    floor.GetComponent<MeshRenderer>().material = startMat;
                    floor.name = "StartFloor";
                    GameObject tempPlayer = Instantiate(player);
                    tempPlayer.transform.parent = transform;
                }

                //  add material to end floor
                if (i == Rows - 1 && j == Columns - 1)
                {
                    floor.GetComponent<MeshRenderer>().material = endMat;
                    floor.name = "EndFloor";
                }
            }
        }
    }

    // set the camera position at the center of the grid and change y to see the whole grid.
    void ChangeCameraPosition()
    {
        float size = Wall.transform.localScale.x;
        Vector3 cameraPosition = Camera.main.transform.position;
        cameraPosition.x = Mathf.Round(Columns / 2) * size;
        cameraPosition.z = -Mathf.Round(Rows / 2) * size;
        Camera.main.transform.position = cameraPosition;
        Camera.main.orthographicSize = Mathf.Max(Columns, Rows) * 2f;
    }

    void HuntAndKill()
    {
        // mark the first cell of the random walk as visited.
        grid[currentRow, currentColumn].Visited = true;

        while (!scanComplete)
        {
            Walk();
            Hunt();
        }
    }

    void Walk()
    {
        while (AreThereUnvisitedNeighbors())
        {
            // then go to a random direction.
            int direction = Random.Range(0, 4);

            // check up.
            if (direction == 0)
            {
                // make sure the above cell is unvisited and within grid boundaries.
                if (IsCellUnvisitedAndWithinBoundaries(currentRow - 1, currentColumn))
                {
                    // destroy the up wall of this cell if there's any.
                    if (grid[currentRow, currentColumn].UpWall)
                    {
                        Destroy(grid[currentRow, currentColumn].UpWall);
                    }

                    currentRow--;
                    grid[currentRow, currentColumn].Visited = true;

                    // destroy the down wall of the cell above if there's any.
                    if (grid[currentRow, currentColumn].DownWall)
                    {
                        Destroy(grid[currentRow, currentColumn].DownWall);
                    }
                }
            }
            // check down.
            else if (direction == 1)
            {
                // make sure the below cell is unvisited and within grid boundaries.
                if (IsCellUnvisitedAndWithinBoundaries(currentRow + 1, currentColumn))
                {
                    // destroy the down wall of this cell if there's any.
                    if (grid[currentRow, currentColumn].DownWall)
                    {
                        Destroy(grid[currentRow, currentColumn].DownWall);
                    }

                    currentRow++;
                    grid[currentRow, currentColumn].Visited = true;

                    // destroy the up wall of the cell below if there's any.
                    if (grid[currentRow, currentColumn].UpWall)
                    {
                        Destroy(grid[currentRow, currentColumn].UpWall);
                    }
                }
            }
            // check left.
            else if (direction == 2)
            {
                // make sure the left cell is unvisited and within grid boundaries.
                if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn - 1))
                {
                    // destroy the left wall of this cell if there's any.
                    if (grid[currentRow, currentColumn].LeftWall)
                    {
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                    }

                    currentColumn--;
                    grid[currentRow, currentColumn].Visited = true;

                    // destroy the right wall of the cell at the left if there's any.
                    if (grid[currentRow, currentColumn].RightWall)
                    {
                        Destroy(grid[currentRow, currentColumn].RightWall);
                    }
                }
            }
            // check right.
            else if (direction == 3)
            {
                // make sure the right cell is unvisited and within grid boundaries.
                if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn + 1))
                {
                    // destroy the right wall of this cell if there's any.
                    if (grid[currentRow, currentColumn].RightWall)
                    {
                        Destroy(grid[currentRow, currentColumn].RightWall);
                    }

                    currentColumn++;
                    grid[currentRow, currentColumn].Visited = true;

                    // destroy the left wall of the cell at the right if there's any.
                    if (grid[currentRow, currentColumn].LeftWall)
                    {
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                    }
                }
            }
        }
    }

    // after random walk is complete, we run Hunt.
    void Hunt()
    {
        // assume the scan is complete.
        scanComplete = true;

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                // if the condition is satisfied that a cell is unvisited and it has a visited neighbour, do another random walk from new cell.
                if (!grid[i, j].Visited && AreThereVisitedNeighbors(i, j))
                {
                    // scan is not actually complete.
                    scanComplete = false;
                    // set the new current row and column.
                    currentRow = i;
                    currentColumn = j;
                    // mark it as visited.
                    grid[currentRow, currentColumn].Visited = true;
                    // and create a passage (by destroying wall/s) between the new current cell and any adjacent cell.
                    
                    DestroyAdjacentWall();

                    return;
                }
            }
        }
    }

    void DestroyAdjacentWall()
    {
        bool destroyed = false;

        while (!destroyed)
        {
            // pick a random adjacent cell that is visited and within boundaries,
            // and destroy the wall/s between the current cell and adjacent cell.
            int direction = Random.Range(0, 4);

            // check up.
            if (direction == 0)
            {
                if (currentRow > 0 && grid[currentRow - 1, currentColumn].Visited)
                {
                    if (grid[currentRow, currentColumn].UpWall)
                    {
                        Destroy(grid[currentRow, currentColumn].UpWall);
                    }

                    if (grid[currentRow - 1, currentColumn].DownWall)
                    {
                        Destroy(grid[currentRow - 1, currentColumn].DownWall);
                    }

                    destroyed = true;
                }
            }
            // check down.
            else if (direction == 1)
            {
                if (currentRow < Rows - 1 && grid[currentRow + 1, currentColumn].Visited)
                {
                    if (grid[currentRow, currentColumn].DownWall)
                    {
                        Destroy(grid[currentRow, currentColumn].DownWall);
                    }

                    if (grid[currentRow + 1, currentColumn].UpWall)
                    {
                        Destroy(grid[currentRow + 1, currentColumn].UpWall);
                    }

                    destroyed = true;
                }
            }
            // check left.
            else if (direction == 2)
            {
                if (currentColumn > 0 && grid[currentRow, currentColumn - 1].Visited)
                {
                    if (grid[currentRow, currentColumn].LeftWall)
                    {
                        Destroy(grid[currentRow, currentColumn].LeftWall);
                    }

                    if (grid[currentRow, currentColumn - 1].RightWall)
                    {
                        Destroy(grid[currentRow, currentColumn - 1].RightWall);
                    }

                    destroyed = true;
                }
            }
            // check right.
            else if (direction == 3)
            {
                if (currentColumn < Columns - 1 && grid[currentRow, currentColumn + 1].Visited)
                {
                    if (grid[currentRow, currentColumn].RightWall)
                    {
                        Destroy(grid[currentRow, currentColumn].RightWall);
                    }

                    if (grid[currentRow, currentColumn + 1].LeftWall)
                    {
                        Destroy(grid[currentRow, currentColumn + 1].LeftWall);
                    }

                    destroyed = true;
                }
            }
        }
    }

    bool AreThereUnvisitedNeighbors()
    {
        // check up.
        if (IsCellUnvisitedAndWithinBoundaries(currentRow - 1, currentColumn))
        {
            return true;
        }

        // check down.
        if (IsCellUnvisitedAndWithinBoundaries(currentRow + 1, currentColumn))
        {
            return true;
        }

        // check left.
        if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn + 1))
        {
            return true;
        }

        // check right.
        if (IsCellUnvisitedAndWithinBoundaries(currentRow, currentColumn - 1))
        {
            return true;
        }

        return false;
    }

    public bool AreThereVisitedNeighbors(int row, int column)
    {
        // check up.
        if (row > 0 && grid[row - 1, column].Visited)
        {
            return true;
        }

        // check down.
        if (row < Rows - 1 && grid[row + 1, column].Visited)
        {
            return true;
        }

        // check left.
        if (column > 0 && grid[row, column - 1].Visited)
        {
            return true;
        }

        // check right.
        if (column < Columns - 1 && grid[row, column + 1].Visited)
        {
            return true;
        }

        return false;
    }

    // do a boundary check and unvisited check.
    bool IsCellUnvisitedAndWithinBoundaries(int row, int column)
    {
        if (row >= 0 && row < Rows && column >= 0 && column < Columns
            && !grid[row, column].Visited)
        {
            return true;
        }

        return false;
    }

    public void Regenerate()
    {
        int rows = 0;
        int columns = 0;

        if (int.TryParse(HeightField.text, out rows))
        {
            // set the minimum rows to 2.
            Rows = Mathf.Max(2, rows);
        }

        if (int.TryParse(WidthField.text, out columns))
        {
            // set the minimum columns to 2.
            Columns = Mathf.Max(2, columns);
        }

        GenerateGrid();
    }

    public void PrimsAlgorithm()
    {
        /*
            choose random cell
            stop neighbors in lijst
            kies random neighbor uit lijst
            remove random wall tussen huidige cell van de neighbor
            maak pad tussen twee 
       */

        // make list for neighboring cells
        List<MazeCell> cellenDieHetPadAanraken = new List<MazeCell>();
        MazeCell[,] tempgrid;

        // chose random cell from the made up grid
        currentRow = UnityEngine.Random.Range(0, Rows);
        currentColumn = UnityEngine.Random.Range(0, Columns);


        // stop de dichtbijzijnde cellen in de list en voeg ze toe aan cellenDieHetPadAanraken
        var tempCloseNeighbors = PrimsBuurCellen(currentRow, currentColumn);



        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
             
             
            }
        }
    }

    List<(int, int)> PrimsBuurCellen(int row, int col)
    {
        // make new list
        var buren = new List<(int, int)>();

        for (int i = -1; i < 1; i++){
            for (int j = -1; j < 1; j++){
                if ((row + i) < 0) break;
                if ((row + i) > row) break;
                buren.Add((row + i, col + j));
            }
        }

        // return buren
        return buren;
    }

    /*
    bool BuurExists(int row, int col)
    {
        if (PrimsBuurCellen.Exists(grid[row, col]))
        {
            return true;
        }
        return false;
    }
    */
}
