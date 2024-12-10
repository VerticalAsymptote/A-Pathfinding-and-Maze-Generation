using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour{
    [SerializeField]
    GameObject floorPrefab, wallPrefab;

    public int width, height;

    public Cell[,] grid;
    public bool isGenerated;
    public MazeStateManager mazeStateManager;
    private List<Vector2Int> wallList;

    void Start(){
        mazeStateManager = FindAnyObjectByType<MazeStateManager>();
        InitializeGrid();
        GenerateMaze();
        DrawMaze();

        Camera.main.transform.position = new Vector3(width / 2, height, width / 2);
        GetComponent<Pathfinding>().Begin();
    }

    void InitializeGrid(){
        grid = new Cell[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++){
                Cell temp = new Cell();
                temp.x = x;
                temp.y = y;
                grid[x, y] = temp;
                mazeStateManager.RegisterCell(temp);
            }
    }

    void GenerateMaze(){
        wallList = new List<Vector2Int>();

        int startX = Random.Range(1, width - 1);
        int startY = Random.Range(1, height - 1);

        startX = startX % 2 == 0 ? startX - 1 : startX;
        startY = startY % 2 == 0 ? startY - 1 : startY;

        grid[startX, startY].IsVisited = true;
        grid[startX, startY].IsWall = false;

        AddWallsToList(startX, startY);

        while (wallList.Count > 0){
            int randomIndex = Random.Range(0, wallList.Count);
            Vector2Int wall = wallList[randomIndex];
            wallList.RemoveAt(randomIndex);

            ProcessWall(wall);
        }

        mazeStateManager.width = width;
        mazeStateManager.height = height;
    }

    void AddWallsToList(int x, int y){
        if (x - 2 > 0)
            wallList.Add(new Vector2Int(x - 1, y));
        if (x + 2 < width - 1)
            wallList.Add(new Vector2Int(x + 1, y));
        if (y - 2 > 0)
            wallList.Add(new Vector2Int(x, y - 1));
        if (y + 2 < height - 1)
            wallList.Add(new Vector2Int(x, y + 1));
    }

    void ProcessWall(Vector2Int wall){
        int x = wall.x;
        int y = wall.y;

        List<Vector2Int> neighbors = GetNeighbors(x, y);

        if (neighbors.Count == 2){
            Cell cell1 = grid[neighbors[0].x, neighbors[0].y];
            Cell cell2 = grid[neighbors[1].x, neighbors[1].y];

            if (cell1.IsVisited != cell2.IsVisited){
                grid[x, y].IsWall = false;

                if (!cell1.IsVisited){
                    cell1.IsVisited = true;
                    cell1.IsWall = false;
                    AddWallsToList(neighbors[0].x, neighbors[0].y);
                }
                else{
                    cell2.IsVisited = true;
                    cell2.IsWall = false;
                    AddWallsToList(neighbors[1].x, neighbors[1].y);
                }
            }
        }
    }

    List<Vector2Int> GetNeighbors(int x, int y){

    List<Vector2Int> neighbors = new List<Vector2Int>();

    if (x % 2 == 1){
        if (y - 1 >= 0) neighbors.Add(new Vector2Int(x, y - 1));
        if (y + 1 < height) neighbors.Add(new Vector2Int(x, y + 1));
    }
    else if (y % 2 == 1){
        if (x - 1 >= 0) neighbors.Add(new Vector2Int(x - 1, y));
        if (x + 1 < width) neighbors.Add(new Vector2Int(x + 1, y));
    }

    return neighbors;

    }

    void DrawMaze(){
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++){
                Vector3 position = new Vector3(x, 0, y);
                if (grid[x, y].IsWall)
                    Instantiate(wallPrefab, position, Quaternion.identity, transform);
                else
                    Instantiate(floorPrefab, position, Quaternion.identity, transform);
            }
    }

}