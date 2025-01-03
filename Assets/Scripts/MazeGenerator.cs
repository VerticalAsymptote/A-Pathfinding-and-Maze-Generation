using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour{

    [SerializeField]
    private int length, width; // Length = x-axis, Width = y-axis

    [SerializeField]
    private GameObject cellPrefab, wallPrefab;

    private Dictionary<(int, int), Cell> openList;
    private HashSet<Cell> closedList;

    private Maze maze;
    
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

        Cell random = new List<Cell>(openList.Values)[Random.Range(0, openList.Count)];
        closedList.Add(random);
        openList.Remove((random.position.x, random.position.y));
        for (int i = 0; i < 1; i++){
            random = new List<Cell>(openList.Values)[Random.Range(0, openList.Count)];
            HashSet<Cell> path = performRandomWalk(random); 
            foreach (Cell cell in path){
                openList.Remove((cell.position.x, cell.position.y));
                closedList.Add(cell);
            }
        }
        maze.IsGenerated = true;
        maze.displayMaze();
    }

    private HashSet<Cell> performRandomWalk(Cell startCell){
        HashSet<Cell> currentPath = new HashSet<Cell>();
        while (!closedList.Contains(startCell)){
            Cell nextCell = startCell.neighbors[Random.Range(0, startCell.neighbors.Count)];
            if (currentPath.Contains(nextCell)){
                currentPath = eraseLoop(currentPath, nextCell);
                startCell = currentPath.Last();
                continue;
            }
            startCell.setRotation(startCell.position, nextCell.position);
            currentPath.Add(startCell);
            startCell = nextCell;
        }
        currentPath.Add(startCell);
        startCell = currentPath.First();
        HashSet<Cell> newPath = new HashSet<Cell>();
        while (startCell.position != currentPath.Last().position){
            startCell.isVisited = true;
            Vector2Int current = startCell.position + startCell.nextCell;
            newPath.Add(openList[(current.x, current.y)]);
            startCell = newPath.Last();
        }
        return newPath;
    }

    private HashSet<Cell> eraseLoop(HashSet<Cell> path, Cell cell){
        List<Cell> newPath = path.ToList();
        int index = newPath.IndexOf(cell);
        HashSet<Cell> cells = new HashSet<Cell>();
        for (int i = 0; i <= index; i++)
            cells.Add(newPath[i]);
        return cells;
    }
}