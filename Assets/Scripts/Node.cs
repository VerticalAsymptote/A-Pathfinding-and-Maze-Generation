using UnityEngine;

public class Node{
    public Vector2Int GridPosition;
    public bool IsWalkable;
    public int GCost;
    public int HCost;
    public int FCost => GCost + HCost;
    public Node ParentNode;

    public Node(Vector2Int position, bool isWalkable){
        GridPosition = position;
        IsWalkable = isWalkable;
    }
}
