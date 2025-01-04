using System.Collections.Generic;
using UnityEngine;

public class Cell{
    public Vector2Int position {get; private set;} // Position of the cell in the the world space
    public bool isVisited {get; set;} // If the cell has been visited by the maze algorithm
    public List<Cell> neighbors {get; set;} // List of all neighbors in the cardinal directions of the cell
    public Vector2Int nextCell {get; set;} // Mainly used for debugging purposes, gives vector from current cell to next cell
    public Cell(Vector2Int position){
        this.position = position;
        isVisited = false;
        nextCell = new Vector2Int(0, 0);
        neighbors = new List<Cell>(4);
    }
    
    // Mainly used for debugging purposes, assigns field nextCell
    public void setRotation(Vector2Int v){
        Vector2Int result = v - position;
        nextCell = new Vector2Int(result.x, result.y);
    }

    // Mainly used for debugging purposes, returns rotation of cell to point to the next cell in path
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
                return Quaternion.Euler(180f, 0, 0);
        }
    }
}
