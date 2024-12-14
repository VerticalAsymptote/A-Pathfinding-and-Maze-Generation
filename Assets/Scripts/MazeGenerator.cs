using System.Collections.Generic;
using System.Linq;
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
        initCells();
        generateMaze();
        drawMaze();
    }

    void initCells(){
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
    private void generateMaze(){
        Cell endCell = getRandom();
        closedList.Add(endCell);

        while (closedList.Count != openList.Count){
            Cell currentCell = getRandom();
            while (closedList.Any(c => c == currentCell))
                currentCell = getRandom();
            List<Cell> path = FindPath(currentCell);
            addPathtoList(path);
        }

        Debug.Log("Time to Generate: " + Time.realtimeSinceStartup + "s");
    }

    // Displays the maze in according to paths and walls.
    private void drawMaze(){
        foreach (Cell cell in closedList){
            Vector3 position = new Vector3(cell.position.x, 0f, cell.position.y);
            //Instantiate(cellPrefab, position, Quaternion.identity);
            if (cell.isWall)
                Instantiate(wallPrefab, position, Quaternion.identity);
        }
    }

    // Finds path by determining a starting cell and applying a random walk. When the next cell is in the maze, the path is created.
    // If the path loops upon itself, the current loop is reset.
    // After the path is completed, retravel the path to remove extra paths.
    // Returns a list of cells.
    private List<Cell> FindPath(Cell startingCell){
        List<Cell> path = new List<Cell>();
        while (!closedList.Any(c => c.position == startingCell.position)){
            Cell nextCell = performRandomWalk(startingCell);
            if (path.Any(c => c.position == nextCell.position)){
                path = eraseLoop(nextCell, path);
                startingCell = path.Last();
                continue;
            }
            startingCell.nextCell = nextCell;
            path.Add(startingCell);
            startingCell = nextCell;
            }

        Cell cell = path[0];
        List<Cell> fixedPath = new List<Cell>{cell};
        while (cell != path.Last()){
            Cell nextCell = cell.nextCell;
            fixedPath.Add(nextCell);
            cell = nextCell;
        }
        return fixedPath;
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

    // When the cell is in path, it erases all data after the index of the cell.
    // Returns a new list of cells.
    private List<Cell> eraseLoop(Cell cell, List<Cell> path){
        List<Cell> newPath = new List<Cell>();
        int index = path.IndexOf(cell);
        for (int i = 0; i <= index; i++){
            newPath.Add(path[i]);
        }
        return newPath;
    }

    private void addPathtoList(List<Cell> path){
        foreach (Cell cell in path){
            closedList.Add(cell);
        }
    }
}