using UnityEngine;

public class Cell{
    public Vector2Int position;
    public bool isWall;
    public bool isVisited;
    public Cell nextCell;
    public Cell(Vector2Int position){
        this.position = position;
        isWall = true;
        isVisited = false;
        nextCell = null;
    }
    public Quaternion getDirection(){
        if (nextCell == null)
            return Quaternion.Euler(90f, 0f, 0f);
        Vector2Int direction = nextCell.position - position;
        switch(direction){
            case Vector2Int v when v.Equals(Vector2Int.up):
                return Quaternion.identity;
            case Vector2Int v when v.Equals(Vector2Int.down):
                return Quaternion.Euler(0f, 180f, 0f);
            case Vector2Int v when v.Equals(Vector2Int.left):
                return Quaternion.Euler(0f, -90f, 0f);
            case Vector2Int v when v.Equals(Vector2Int.right):
                return Quaternion.Euler(0f, 90f, 0f);
            default:
                return Quaternion.identity;
        }
    }
}