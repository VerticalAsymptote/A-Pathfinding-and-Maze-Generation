using System;
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
        maze = new Maze(length, width, cellPrefab, wallPrefab);
        openList = new Dictionary<(int, int), Cell>(length * width);
        closedList = new HashSet<Cell>(length * width);
        for (int y = 0; y < length; y++)
            for (int x = 0; x < width; x++)
                openList.Add((x, y), maze.MazeData[x, y]);
            


        Cell random = openList[(UnityEngine.Random.Range(0, length), UnityEngine.Random.Range(0, width))];
        closedList.Add(random);
        openList.Remove((random.position.x, random.position.y));
        //while (openList.Count > 0){
            random = openList[(UnityEngine.Random.Range(0, length), UnityEngine.Random.Range(0, width))];
            transferPath(performRandomWalk(random));
        //}
    }

    private HashSet<Cell> performRandomWalk(Cell startCell){
        HashSet<Cell> currentPath = new HashSet<Cell>();
        while (!closedList.Contains(startCell)){
            Cell nextCell = startCell.neighbors[UnityEngine.Random.Range(0, startCell.neighbors.Count)];
            if (currentPath.Contains(nextCell)){
                currentPath = eraseLoop(currentPath, nextCell);
                startCell = currentPath.Last();
                continue;
            }
            Debug.Log("Start Cell: " + startCell.position);
            Debug.Log("End Cell: " + nextCell.position);
            startCell.setRotation(startCell.position, nextCell.position);
            Debug.Log("Rotation: " + startCell.direction);
            currentPath.Add(startCell);
            startCell = nextCell;
        }
        //return currentPath;

        startCell = currentPath.First();
        HashSet<Cell> newPath = new HashSet<Cell>();
        Vector3 currentPos = new Vector3(startCell.position.x, 0f, startCell.position.y);
        Vector3 endPos = new Vector3(currentPath.Last().position.x, 0f, currentPath.Last().position.y);
        //Debug.Log("End Position: " + endPos);
        while (currentPos != endPos){
            //Debug.Log("Current Position: " + currentPos);
            //Debug.Log("Rotation: " + startCell.direction);
            newPath.Add(openList[(Mathf.RoundToInt(currentPos.x), Mathf.RoundToInt(currentPos.z))]);
            if (startCell.direction != Quaternion.Euler(90f, 90f, 90f))
                currentPos += startCell.direction * new Vector3(1, 0, 0);
            startCell = newPath.Last();
            startCell.isVisited = true;
        }
        return newPath;
    }

    private HashSet<Cell> eraseLoop(HashSet<Cell> path, Cell cell){
        Cell[] array = path.ToArray();
        int index = Array.IndexOf(array, cell);
        return new HashSet<Cell>(new ArraySegment<Cell>(array, 0, index));
    }
    private void transferPath(HashSet<Cell> path){
        foreach (Cell cell in path){
            openList.Remove((cell.position.x, cell.position.y));
            closedList.Add(cell);
        }
    }
}