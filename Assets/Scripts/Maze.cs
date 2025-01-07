using System.Collections.Generic;
using UnityEngine;

public class Maze : ScriptableObject{
    public int Length {get; private set;} // Length is x-axis
    public int Width {get; private set;} // Width is y-axis
    public bool IsGenerated {get; set;} // Check to see if maze has fully generated
    public Cell[,] MazeData {get; private set;} // MazeData that is passed along to AI pathfinding

    private List<GameObject> objects;
    private GameObject CellPrefab, WallPrefab;

    // "Constructor" method for scriptable object
    public Maze instantiateMaze(int Length, int Width, GameObject CellPrefab, GameObject WallPrefab){
        this.Length = Length;
        this.Width = Width;
        this.CellPrefab = CellPrefab;
        this.WallPrefab = WallPrefab;
        objects = new List<GameObject>();
        IsGenerated = false;
        MazeData = new Cell[Length, Width];
        populateGrid();
        setNeighbors();
        return this;
    }

    // Populates the field MazeData with new cell structs
    private void populateGrid(){
        for (int y = 0; y < Width; y++)
            for (int x = 0; x < Length; x++)
                MazeData[x, y] = new Cell(new Vector2Int(x, y));
    }

    // Finds all neighbors of the current cell and adds it to the current cell's neighbor field
    private void setNeighbors(){
        for (int y = 0; y < Width; y++)
            for (int x = 0; x < Length; x++){
                Cell c = MazeData[x, y];
                if (y + 1 < Width)
                    c.neighbors.Add(MazeData[x, y + 1]);
                if (x + 1 < Length)
                    c.neighbors.Add(MazeData[x + 1, y]);
                if (y - 1 >= 0) // These two lines of code killed me 
                    c.neighbors.Add(MazeData[x, y - 1]);
                if (x - 1 >= 0) // These two lines of code killed me I think ill cry
                    c.neighbors.Add(MazeData[x - 1, y]);
            }        
    }

    // Loops through MazeData and instantiate cell prefabs
    public void displayMaze(){
        //if (IsGenerated){
            for (int y = 0; y < Width; y++)
                for (int x = 0; x < Length; x++){
                    if (MazeData[x, y].isVisited){
                        objects.Add(Instantiate(CellPrefab, new Vector3(x, 0f, y), MazeData[x, y].getRotation()));
                    }
                }
        //}
        //else throw new Exception("Maze has not been generated!");
    }

    public void removeMaze(){
        foreach (GameObject prefab in objects){
            Vector2Int position = new Vector2Int((int)prefab.transform.position.x, (int)prefab.transform.position.z);
            MazeData[position.x, position.y].isVisited = false;
            Destroy(prefab);
        }
    }
}