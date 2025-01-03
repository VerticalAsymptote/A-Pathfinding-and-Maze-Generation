using System.Collections.Generic;
using UnityEngine;

public struct Cell{
    public Vector2Int position {get; private set;}
    public bool isVisited {get; set;}
    public List<Cell> neighbors {get; set;}
    public Vector2Int nextCell {get; set;}
    public Cell(Vector2Int position){
        this.position = position;
        isVisited = false;
        nextCell = new Vector2Int(0, 0);
        neighbors = new List<Cell>(4);
    }
    
    public void setRotation(Vector2Int a, Vector2Int b){
        Vector2Int result = b - a;
        nextCell = new Vector2Int(result.x, result.y);
    }

    public Quaternion getRotation(){
        switch(nextCell.x, nextCell.y){
            case(0, 1):
                return Quaternion.Euler(0f, 0f, 0f);
            case(1, 0):
                return Quaternion.Euler(0f, 90f, 0f);
            case(0, -1):
                return Quaternion.Euler(0f, 180f, 0f);
            case(-1, 0):
                return Quaternion.Euler(0f, 270f, 0f);
            default:
                return Quaternion.identity;
        }
    }
}
