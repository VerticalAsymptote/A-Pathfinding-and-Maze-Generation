using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Maze : MonoBehaviour{

    [Header("Size"), Tooltip("Actual Size is 2 * Side of Prefab - 1"), SerializeField]
    public int Length; // Length is the x-axis
    public int Width; // Width is the y-axis
    
    [Header("Location"), Tooltip("Location of the bottom-left most cell"), SerializeField]
    public Vector3 Location;

    [Header("Prefabs"), SerializeField]
    private GameObject WallPrefab, CellPrefab;

    public bool IsGenerated {get; private set;} // True when the maze is finished generating
    public bool[,] MazeData {get; private set;} // True - Cell, False - Wall

    //-------------------------------------- Maze Generation Fields--------------------------------------//
    private Dictionary<(int, int), Cell> OpenList;
    private HashSet<Cell> ClosedList; 
    

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
    
    public void GenerateMaze(){
        Cell random = new List<Cell>(OpenList.Values)[Random.Range(0, OpenList.Count)];
        ClosedList.Add(random);
        OpenList.Remove((random.position.x, random.position.y));
        while (OpenList.Count > 0){
            random = new List<Cell>(OpenList.Values)[Random.Range(0, OpenList.Count)];
            HashSet<Cell> path = performRandomWalk(random);
            foreach (Cell cell in path){
                cell.isVisited = true;
                OpenList.Remove((cell.position.x, cell.position.y));
                ClosedList.Add(cell);
            }
        }
        ProcessWalls();
        IsGenerated = true;
    }

    public void DisplayMaze(){
        if (IsGenerated){
            foreach (Cell c in ClosedList)
                Instantiate(CellPrefab, new Vector3(c.position.x + Location.x, Location.y, c.position.y + Location.z), c.getRotation());
        }

        //if (IsGenerated){
        //    for (int y = 0; y < Length; y++)
        //        for (int x = 0; x < Width; x++)
        //            if (MazeData[x, y])
        //                Instantiate(CellPrefab, new Vector3(x, 0f, y), Quaternion.identity);
        //} else throw new System.Exception("Maze is not generated!");
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
            startCell.setRotation(nextCell.position);
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
    private void ProcessWalls(){
        for (int y = 1; y < 2 * Length - 1; y += 2)
            for (int x = 1; x < 2 * Width - 1; x += 2)
                Instantiate(WallPrefab, new Vector3(x + Location.x, 1.5f + Location.y, y + Location.z), Quaternion.identity);
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