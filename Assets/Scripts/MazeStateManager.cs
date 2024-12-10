using System.Collections.Generic;
using UnityEngine;

public class MazeStateManager : MonoBehaviour{
    public Dictionary<(int, int), Cell> cellStates = new Dictionary<(int, int), Cell>();
    public int width, height;

    public void RegisterCell(Cell cell){
        cellStates[(cell.x, cell.y)] = cell;
    }

    public Cell GetNextState(Cell current, Vector2Int direction){
        int nx = current.x + direction.x;
        int ny = current.y + direction.y;

        if (nx < 0 || nx >= width || ny < 0 || ny >= height)
            return current;
        
        Cell nextState = cellStates[(nx, ny)];
        if (!nextState.IsWalkable)
            return current;

        return nextState;
    }
}
