using UnityEngine;

public struct Maze{
    public int Length {get; private set;} // Length is x-axis
    public int Width {get; private set;} // Width is y-axis

    private GameObject CellPrefab, WallPrefab;
    public Cell[,] MazeData {get; private set;}

    public Maze(int Length, int Width, GameObject CellPrefab, GameObject WallPrefab){
        this.Length = Length;
        this.Width = Width;
        this.CellPrefab = CellPrefab;
        this.WallPrefab = WallPrefab;
        MazeData = new Cell[Length, Width];
        populateGrid();
        setNeighbors();
    }

    private void populateGrid(){
        for (int y = 0; y < Width; y++)
            for (int x = 0; x < Length; x++)
                MazeData[x, y] = new Cell(new Vector2Int(x, y));
    }

    private void setNeighbors(){
        for (int y = 0; y < Width; y++)
            for (int x = 0; x < Length; x++){
                Cell c = MazeData[x, y];
                if (y + 1 < Width)
                    c.neighbors.Add(MazeData[x, y + 1]);
                if (x + 1 < Length)
                    c.neighbors.Add(MazeData[x + 1, y]);
                if (y - 1 > 0)
                    c.neighbors.Add(MazeData[x, y - 1]);
                if (x - 1 > 0)
                    c.neighbors.Add(MazeData[x - 1, y]);
            }        
    }
}