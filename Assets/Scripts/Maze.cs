using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Maze
{
    public List<Cell> Cellen { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }

    public Maze(int w, int h) {

        this.Width = w;
        this.Height = h;

        for (int i = 0; i < w; i++) {
            for (int j = 0; j < h; j++) {
                this.Cellen.Add(new Cell(i, j, this));
            }
        }
    }

    public void Generate() {
        var cellenDieHetPadAanraken = new List<Cell>();

        //kies random start cell
        var randomStartCell = Cellen[Random.Range(0, Cellen.Count)];

        // voegen de buren toe aan een lijst
        cellenDieHetPadAanraken.AddRange(randomStartCell.GetBuren());

        // random cell kiezen die het pad aanraakt
        var randomCellDieHetPadAanraakt = cellenDieHetPadAanraken[Random.Range(0, cellenDieHetPadAanraken.Count)];

        //muren tussen de huidige cel en de nieuwe cel weghalen
        
    }
}
