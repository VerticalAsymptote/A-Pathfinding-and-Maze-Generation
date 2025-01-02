using System.Collections.Generic;
using UnityEngine;

public struct Cell{
    public Vector2Int position {get; private set;}
    public Quaternion direction {get; set;}
    public bool isVisited {get; set;}
    public List<Cell> neighbors {get; set;}
    public Cell(Vector2Int position){
        this.position = position;
        direction = Quaternion.Euler(90f, 90f, 90f);
        isVisited = false;
        neighbors = new List<Cell>(4);
    }
    
    public void setRotation(Vector2Int a, Vector2Int b){
        Vector2Int result = b - a;
        switch (result.x, result.y){
            case (0, 1):
                direction = Quaternion.Euler(0f, 90f, 0f);
                break;
            case (1, 0):
                direction = Quaternion.Euler(0f, 0f, 0f);
                break;
            case (0, -1):
                direction = Quaternion.Euler(0f, 180f, 0f);
                break;
            case (-1, 0):
                direction = Quaternion.Euler(0f, 270f, 0f);                
                break;
        }
    }
}