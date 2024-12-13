using UnityEngine;

public class Cell{
    public Vector2Int position;
    public bool isWall = true;
    public bool isVisited = false;
    public Cell nextCell = null;
}