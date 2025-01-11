using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

// Maze Generation Class
public class Maze : MonoBehaviour{

    [Header("Size"), Tooltip("Actual Size is 2 * Side of Prefab - 1"), SerializeField]
    public int Length; // Length is the x-axis
    public int Width; // Width is the y-axis
    
    [Header("Location"), Tooltip("Location of the bottom-left most cell"), SerializeField]
    public Vector3 Location;

    [Header("Prefabs"), SerializeField]
    private GameObject WallPrefab, PlanePrefab;

    public bool IsGenerated {get; private set;} // True when the maze is finished generating
    public bool[,] MazeData {get; private set;} // True - Cell, False - Wall

    private Dictionary<(int, int), Cell> OpenList; // List of all unproccessed cells
    private HashSet<Cell> ClosedList;  // List of all processed cells
    
    void Awake(){
        OpenList = new Dictionary<(int, int), Cell>(Length * Width);
        ClosedList = new HashSet<Cell>(Length * Width);
        MazeData = new bool[2 * Length - 1, 2 * Width - 1];
        for (int y = 0; y < Length; y++)
            for (int x = 0; x < Width; x++){
                Cell cell = new Cell(new Vector2Int(x * 2, y * 2));
                OpenList.Add((x * 2, y * 2), cell);
            }
        SetNeighbors();
        GenerateMaze();
        DisplayMaze();
    }
    
    // Generates the maze
    public void GenerateMaze(){
        Cell random = new List<Cell>(OpenList.Values)[Random.Range(0, OpenList.Count)];
        ClosedList.Add(random);
        OpenList.Remove((random.position.x, random.position.y));
        while (OpenList.Count > 0){
            random = new List<Cell>(OpenList.Values)[Random.Range(0, OpenList.Count)];
            HashSet<Cell> path = performRandomWalk(random);
            Cell previousCell = new Cell(Vector2Int.one, true);
            foreach (Cell cell in path){
                OpenList.Remove((cell.position.x, cell.position.y));
                ClosedList.Add(cell);
                MazeData[cell.position.y, cell.position.x] = true;
                if (!previousCell.isNull){
                    int x = (cell.position.x + previousCell.position.x) / 2;
                    int z = (cell.position.y + previousCell.position.y) / 2;
                    MazeData[z, x] = true;
                }
                previousCell = cell;
            }
        }
        IsGenerated = true;
    }

    // Displays the maze
    public void DisplayMaze(){
        if (IsGenerated){
            int xSize = 2 * Length - 1, ySize = 2 * Width - 1;
            GameObject plane = Instantiate(PlanePrefab, new Vector3(Location.x + xSize / 2.0f - 0.5f, Location.y, Location.z + ySize / 2.0f - 0.5f), Quaternion.identity);
            plane.transform.localScale = new Vector3(xSize / 10.0f, 1f, ySize / 10.0f);
            plane = Instantiate(PlanePrefab, new Vector3(Location.x - 0.5f, Location.y + 1f, (Location.z * 2 + ySize) / 2.0f - 0.5f), Quaternion.Euler(0f, 0f, -90f));
            plane.transform.localScale = new Vector3(0.2f, 1f, ySize / 10.0f);
            plane = Instantiate(PlanePrefab, new Vector3(Location.x + xSize - 0.5f, Location.y + 1f, (Location.z * 2 + ySize) / 2.0f - 0.5f), Quaternion.Euler(0f, 0f, 90f));
            plane.transform.localScale = new Vector3(0.2f, 1f, ySize / 10.0f);
            plane = Instantiate(PlanePrefab, new Vector3((Location.x * 2 + xSize) / 2.0f - 0.5f, Location.y + 1f, Location.z + ySize - 0.5f), Quaternion.Euler(0f, -90f, 90f));
            plane.transform.localScale = new Vector3(0.2f, 1f, xSize / 10.0f);
            plane = Instantiate(PlanePrefab, new Vector3((Location.x * 2 + xSize) / 2.0f - 0.5f, Location.y + 1f, Location.z - 0.5f), Quaternion.Euler(0f, 90f, 90f));
            plane.transform.localScale = new Vector3(0.2f, 1f, xSize / 10.0f);
            for (int y = 0; y < 2 * Width - 1; y++)
                for (int x = 0; x < 2 * Length - 1; x++){
                    if (!MazeData[x, y])
                        Instantiate(WallPrefab, new Vector3(x + Location.x, 1 + Location.y, y + Location.z), Quaternion.identity);
                }
        } else throw new System.Exception("Maze is not generated!");
    }

    private HashSet<Cell> performRandomWalk(Cell startCell){
        HashSet<Cell> currentPath = new HashSet<Cell>();
         while (!ClosedList.Contains(startCell)){
            Cell nextCell = startCell.neighbors[Random.Range(0, startCell.neighbors.Count)];
            if (currentPath.Contains(nextCell)){
                currentPath = eraseLoop(currentPath, nextCell);
                startCell = currentPath.Last();
                continue;
            }
            currentPath.Add(startCell);
            startCell = nextCell;
        }
        currentPath.Add(startCell);
        return currentPath;
    }

    private HashSet<Cell> eraseLoop(HashSet<Cell> path, Cell cell){
        List<Cell> newPath = path.ToList();
        int index = newPath.IndexOf(cell);
        HashSet<Cell> cells = new HashSet<Cell>();
        for (int i = 0; i <= index; i++)
            cells.Add(newPath[i]);
        return cells;
    }

    private void SetNeighbors(){
        foreach (Cell cell in OpenList.Values){
            Cell temp;
            if (OpenList.TryGetValue((cell.position.x, cell.position.y + 2), out temp))
                cell.neighbors.Add(temp);
            if (OpenList.TryGetValue((cell.position.x + 2, cell.position.y), out temp))
                cell.neighbors.Add(temp);
            if (OpenList.TryGetValue((cell.position.x, cell.position.y - 2), out temp))
                cell.neighbors.Add(temp);
            if (OpenList.TryGetValue((cell.position.x - 2, cell.position.y), out temp))
                cell.neighbors.Add(temp);
        }
    }
}

// Cell struct used in generating a maze
public struct Cell{
    public Vector2Int position {get; private set;} // Actual Position of the Cell
    public List<Cell> neighbors {get; set;} // List of cells (memoization)
    public bool isNull {get; private set;} // Null check since Cell is a struct

    // Constructor required for a struct
    public Cell(Vector2Int position, bool ifNull = false){
        this.position = position;
        isNull = ifNull;
        neighbors = new List<Cell>(4);
    }
}
