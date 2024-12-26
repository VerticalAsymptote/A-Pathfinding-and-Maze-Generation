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
    public string getDirection(){
        Vector2Int direction = nextCell.position - position;
        switch(direction){
            case Vector2Int v when v.Equals(Vector2Int.up) || v.Equals(Vector2Int.down):
                return "Vertical";
            case Vector2Int v when v.Equals(Vector2Int.left) || v.Equals(Vector2Int.right):
                return "Horizontal";
            default:
                return "None";
        }
    }
}