using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MazeGenerator : MonoBehaviour{

    [SerializeField]
    public int length, width; // Length = x-axis, Width = y-axis

    [SerializeField]
    GameObject cellPrefab, wallPrefab;

    public Cell[,] mazeData;
    public Dictionary<(int, int), Cell> openList;
    public List<Cell> closedList;
    
    void Start(){
        openList = new Dictionary<(int, int), Cell>();
        closedList = new List<Cell>();
        InitCells();
        GenerateMaze();
        DrawMaze();
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
    private void GenerateMaze(){
        Cell endCell = getRandom();
        closedList.Add(endCell);
        Cell currentCell = getRandom();

        List<Cell> path = FindPath(currentCell);
        List<Cell> fixedPath = new List<Cell>{currentCell};
        currentCell = path[0];
        while (!closedList.Any(c => c.position == currentCell.position)){
            Cell nextCell = currentCell.nextCell;
            fixedPath.Add(currentCell);
            currentCell = nextCell;
        }

        foreach (Cell cell in fixedPath){
            closedList.Add(cell);
        }
    }

    // Finds path by determining a starting cell and applying a random walk. When the next cell is in the maze, the path is created.
    // If the path loops upon itself, the current loop is reset.
    // Returns a list of cells.
    private List<Cell> FindPath(Cell startingCell){
        List<Cell> path = new List<Cell>();
        while (!closedList.Any(c => c.position == startingCell.position)){
            Cell nextCell = performRandomWalk(startingCell);
            if (path.Any(c => c.position == nextCell.position)){
                startingCell = path[0];
                path.Clear();
                path.Add(startingCell);
                continue;
            }
            startingCell.nextCell = nextCell;
            path.Add(startingCell);
            startingCell = nextCell;
        }
        return path;
    }

    private void DrawMaze(){
        foreach (Cell cell in closedList){
            Vector3 position = new Vector3(cell.position.x, 0f, cell.position.y);
            //Instantiate(cellPrefab, position, Quaternion.identity);
            if (cell.isWall)
                Instantiate(wallPrefab, position, Quaternion.identity);
        }
    }

    // Gets a random cell from openList.
    // Returns the random cell.
    private Cell getRandom(){
        List<Cell> cells = new List<Cell>(openList.Values);
        return cells[Random.Range(0, cells.Count)];
    }

    // When provided a cell, chooses a random neighboring cell that is not yet chosen to go into.
    // Returns the chosen cell.
    private Cell performRandomWalk(Cell previousCell){
        List<Cell> neighbors = getNeighbors(previousCell);
        return neighbors[Random.Range(0, neighbors.Count)];
    }

    // Checks if there are any open cells in the 4 directions of the current cell - Up, Right, Down, Left.
    // Returns a list of cells that are open.
    private List<Cell> getNeighbors(Cell cell){
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

