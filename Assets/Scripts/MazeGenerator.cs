using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MazeGenerator : MonoBehaviour{

    [SerializeField]
    private int length, width; // Length = x-axis, Width = y-axis

    [SerializeField]
    private GameObject cellPrefab, wallPrefab;

    private Dictionary<(int, int), Cell> openList; // Dictionary containing remaining cells not in maze. Key are x and y position of cell, Value are cell structs
    private HashSet<Cell> closedList; // HashSet containing all cells already in the maze

    public Maze maze {get; private set;} // Maze scriptable object
    
    // Acts as the main method. Will be removed after scripts calling generateMaze() are added
    void Start(){
        generateMaze();
    }

    
    public void generateMaze(){
        maze = (Maze)ScriptableObject.CreateInstance("Maze");
        maze.instantiateMaze(length, width, cellPrefab, wallPrefab);
        openList = new Dictionary<(int, int), Cell>(length * width);
        closedList = new HashSet<Cell>(length * width);
        for (int y = 0; y < length; y++)
            for (int x = 0; x < width; x++)
                openList.Add((x, y), maze.MazeData[x, y]);
        //----------------------------------------------------------- Initializing data structures-----------------------------------------------------------

        
        Cell random = new List<Cell>(openList.Values)[Random.Range(0, openList.Count)]; // Adds random cell to maze to kickstart algorithm
        closedList.Add(random);
        openList.Remove((random.position.x, random.position.y));
        for (int i = 0; i < 1; i++){ // Currently will only generate one path
            random = new List<Cell>(openList.Values)[Random.Range(0, openList.Count)]; // Chooses a random starting cell
            HashSet<Cell> path = performRandomWalk(random); 
            foreach (Cell cell in path){
                cell.isVisited = true;
                openList.Remove((cell.position.x, cell.position.y));
                closedList.Add(cell);
            }
        }
        maze.IsGenerated = true;
        maze.displayMaze();
    }

    // Performs the random walk of Wilson's algorithm. 
    private HashSet<Cell> performRandomWalk(Cell startCell){
        HashSet<Cell> currentPath = new HashSet<Cell>();
         while (!closedList.Contains(startCell)){ // Checks if the cell is in the maze, if not continue
            Cell nextCell = startCell.neighbors[Random.Range(0, startCell.neighbors.Count)]; // Chooses a maze possible cell to walk to
            if (currentPath.Contains(nextCell)){ // If the cell is in the path already, the algorithm has created a loop and will erase the loop
                currentPath = eraseLoop(currentPath, nextCell);
                startCell = currentPath.Last();
                continue;
            }
            startCell.setRotation(nextCell.position); // Mainly used for debugging
            currentPath.Add(startCell); // Adds cell to the current path
            startCell = nextCell;

            if (currentPath.Count > 500){
                Debug.LogError("Infinite Loop");
                break;
            }

        }
        currentPath.Add(startCell); // Adds the last cell to currentPath to avoid missing cells
        Debug.Log(currentPath.Count);
        return currentPath;
    }

    // Taking in the current path and the conflicting cell, cuts off the loop after the cell (inclusive of cell)
    private HashSet<Cell> eraseLoop(HashSet<Cell> path, Cell cell){
        List<Cell> newPath = path.ToList();
        int index = newPath.IndexOf(cell);
        HashSet<Cell> cells = new HashSet<Cell>();
        for (int i = 0; i <= index; i++)
            cells.Add(newPath[i]);
        return cells;
    }
}