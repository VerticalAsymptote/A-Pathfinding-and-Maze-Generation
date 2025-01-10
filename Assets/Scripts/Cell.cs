using System.Collections.Generic;
using UnityEngine;

public class Cell{
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
    
    public void setRotation(Vector2Int v){
        Vector2Int result = v - position;
        nextCell = new Vector2Int(result.x, result.y);
    }

    public Quaternion getRotation(){
        int magnitude = (int)nextCell.magnitude;
        if (magnitude != 0)
            switch(nextCell.x / magnitude, nextCell.y / magnitude){
                case(0, 1):
                    return Quaternion.Euler(0f, 0f, 0f);
                case(1, 0):
                    return Quaternion.Euler(0f, 90f, 0f);
                case(0, -1):
                    return Quaternion.Euler(0f, 180f, 0f);
                case(-1, 0):
                    return Quaternion.Euler(0f, 270f, 0f);
            }
        return Quaternion.Euler(180f, 0f, 0f);
    }
}
