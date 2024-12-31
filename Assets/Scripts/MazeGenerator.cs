using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour{

    [SerializeField]
    public int length, width; // Length = x-axis, Width = y-axis

    [SerializeField]
    GameObject cellPrefab, wallPrefab;

    private Dictionary<(int, int), Cell> openList;
    private HashSet<Cell> closedList;
    private HashSet<Vector3> wallsList;
    
    void Start(){
        openList = new Dictionary<(int, int), Cell>(length * width);
        closedList = new HashSet<Cell>(length * width);
        wallsList = new HashSet<Vector3>();
        initCells();
        generateMaze();
        drawMaze();
        for (int i = 0; i < width; i++){
            Instantiate(wallPrefab, new Vector3(-0.5f, 1f, i), Quaternion.identity);
            Instantiate(wallPrefab, new Vector3(length - 0.5f, 1f, i), Quaternion.identity);
        }
        for (int i = 0; i < length; i++){
            Instantiate(wallPrefab, new Vector3(i, 1f, -0.5f), Quaternion.Euler(0f, 90f, 0f));
            Instantiate(wallPrefab, new Vector3(i, 1f, width - 0.5f), Quaternion.Euler(0f, 90f, 0f));
        }
        Cell endCell = getRandom();
        closedList.Add(endCell);
    }

    void initCells(){
        for (int x = 0; x < length; x++)
            for (int y = 0; y < width; y++){
                Cell newCell = new Cell(new Vector2Int(x, y));
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
            while (closedList.Contains(currentCell))
                currentCell = getRandom();
            List<Cell> path = findPath(currentCell);
            processWalls(path);
            addPathtoList(path);
        }
    }

    // Displays the maze in according to paths and walls.
    private void drawMaze(){
        foreach (Cell cell in closedList){
            Vector3 position = new Vector3(cell.position.x, 0f, cell.position.y);
            Instantiate(cellPrefab, position, cell.getDirection());
        }
    }

    // Finds path by determining a starting cell and applying a random walk. When the next cell is in the maze, the path is created.
    // If the path loops upon itself, the current loop is reset.
    // After the path is completed, retravel the path to remove extra paths.
    // Returns a list of cells.
    private List<Cell> findPath(Cell startingCell){
        List<Cell> path = new List<Cell>();
        while (!closedList.Contains(startingCell)){
            Cell nextCell = performRandomWalk(startingCell);
            if (path.Contains(nextCell)){
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
            cell.isVisited = true;
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
        List<Cell> cells = new List<Cell>(4);
        Cell neighbor;
        if (openList.TryGetValue((cell.position.x, cell.position.y + 1), out neighbor))
            cells.Add(neighbor);
         if (openList.TryGetValue((cell.position.x + 1, cell.position.y), out neighbor))
            cells.Add(neighbor);
        if (openList.TryGetValue((cell.position.x, cell.position.y - 1), out neighbor))
            cells.Add(neighbor);
        if (openList.TryGetValue((cell.position.x - 1, cell.position.y), out neighbor))
            cells.Add(neighbor);
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

    private Vector3 averagePosition(Cell a, Cell b){
        float x = (a.position.x + b.position.x) / 2.0f;
        float z = (a.position.y + b.position.y) / 2.0f;
        return new Vector3(x, 1f, z);
    } 
    
    private void processWalls(List<Cell> path){
        foreach (Cell currentCell in path){
            foreach (Cell neighbor in getNeighbors(currentCell)){
                if (currentCell.nextCell != neighbor && neighbor.nextCell != currentCell)
                    if (neighbor.isVisited && neighbor != null){
                        Vector3 position = averagePosition(currentCell, neighbor);
                        if (wallsList.Contains(position))
                            continue;
                        wallsList.Add(position);
                        Vector2Int direction = neighbor.position - currentCell.position;
                        Quaternion wallRotation = Quaternion.identity;
                        if (direction == Vector2Int.up || direction == Vector2Int.down)
                            wallRotation = Quaternion.Euler(0f, 90f, 0f);
                        Instantiate(wallPrefab, position, wallRotation);  
                    }
            }
        }
    }

}