using UnityEngine;

public class MazeGenerator : MonoBehaviour{

    [SerializeField]
    public int length, width; // Length = x-axis, Width = y-axis

    [SerializeField]
    GameObject cellPrefab, wallPrefab;

    public Cell[,] maze;
    
    void Start(){
        InitCells();
        GenerateMaze();
    }

    void InitCells(){
        maze = new Cell[length, width];
        for (int x = 0; x < length; x++)
            for (int y = 0; y < width; y++){
                Cell newCell = new Cell();
                newCell.position.x = x;
                newCell.position.y = y;
                maze[x, y] = newCell;
            }
    }

    void GenerateMaze(){
        // Use Wilson's algorithm to generate maze?
    }


}

