using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class MazeGenerator : MonoBehaviour{

    [SerializeField]
    public int length, width; // Length = x-axis, Width = y-axis

    [SerializeField]
    GameObject cellPrefab, wallPrefab;

    public Cell[,] mazeData;
    public Dictionary<(int, int), Cell> openList;
    public Dictionary<(int, int), Cell> closedList;
    
    void Start(){
        InitCells();
        GenerateMaze();
    }

    void InitCells(){
        for (int x = 0; x < length; x++)
            for (int y = 0; y < width; y++){
                Cell newCell = new Cell();
                Vector2Int pos = new Vector2Int(x, y);
                newCell.position = pos;
                openList.Add((x, y), newCell);
            }
    }

    // Chooses a random cell in openList to act as the first part of the maze.
    // Finds another random cell to act as the starting cell.
    // While the current cell is not the starting cell,
    void GenerateMaze(){
        closeCell(getRandom());
        Cell currentCell = getRandom();
        while (closedList.ContainsValue(currentCell)){
            Cell nextCell = performRandomWalk(currentCell);
            currentCell.nextCell = nextCell;
            closeCell(currentCell);
            currentCell = nextCell;
        }
    }

    // Gets a random cell from openList.
    // Returns the random cell.
    Cell getRandom(){
        List<Cell> cells = new List<Cell>(openList.Values);
        return cells[Random.Range(0, cells.Count)];
    }
    
    // Removes cell from openList and moves it to closedList.
    void closeCell(Cell cell){
        openList.Remove((cell.position.x, cell.position.y));
        closedList.Add((cell.position.x, cell.position.y), cell);   
    }

    // When provided a cell, chooses a random neighboring cell that is not yet chosen to go into.
    // Returns the chosen cell.
    Cell performRandomWalk(Cell previousCell){
        List<Cell> neighbors = getNeighbors(previousCell);
        Cell nextCell = neighbors[Random.Range(0, neighbors.Count)];
        return nextCell;
    }

    // Checks if there are any open cells in the 4 directions of the current cell - Up, Right, Down, Left.
    // Returns a list of cells that are open.
    List<Cell> getNeighbors(Cell cell){
        List<Cell> cells = new List<Cell>();
        if (openList.ContainsKey((cell.position.x, cell.position.y + 1)))
            cells.Add(openList[(cell.position.x, cell.position.y + 1)]);
        if (openList.ContainsKey((cell.position.x + 1, cell.position.y)))
            cells.Add(openList[(cell.position.x + 1, cell.position.y)]);        
        if (openList.ContainsKey((cell.position.x, cell.position.y - 1)))
            cells.Add(openList[(cell.position.x, cell.position.y - 1)]);       
        if (openList.ContainsKey((cell.position.x - 1, cell.position.y)))
            cells.Add(openList[(cell.position.x - 1, cell.position.y)]);            
        return cells;
    }

}

