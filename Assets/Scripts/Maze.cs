using System;
using UnityEngine;

public class Maze : ScriptableObject{
    public int Length {get; private set;} // Length is x-axis
    public int Width {get; private set;} // Width is y-axis
    public bool IsGenerated {get; set;}
    public Cell[,] MazeData {get; private set;}

    private GameObject CellPrefab, WallPrefab;

    public Maze instantiateMaze(int Length, int Width, GameObject CellPrefab, GameObject WallPrefab){
        this.Length = Length;
        this.Width = Width;
        this.CellPrefab = CellPrefab;
        this.WallPrefab = WallPrefab;
        IsGenerated = false;
        MazeData = new Cell[Length, Width];
        populateGrid();
        setNeighbors();
        return this;
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

    public void displayMaze(){
        if (IsGenerated){
            for (int y = 0; y < Width; y++)
                for (int x = 0; x < Length; x++){
                    if (MazeData[x, y].isVisited){
                        Instantiate(CellPrefab, new Vector3(x, 0f, y), MazeData[x, y].getRotation());
                    }
                }
        }
        else throw new Exception("Maze has not been generated!");
    }
}