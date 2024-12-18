using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour{
    [SerializeField]
    Vector2Int startPositon, endPosition;

    private Node[,] nodes;
    private List<Node> openList;
    private HashSet<Node> closedList;

    private MazeGenerator mazeGenerator;

    public void Begin(){
        mazeGenerator = GetComponent<MazeGenerator>();
        InitializeNodes();
        FindPath(startPositon, endPosition);    
    }
    
    void InitializeNodes(){
        int width = mazeGenerator.width;
        int height = mazeGenerator.height;
        nodes = new Node[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++){
                bool isWalkable = !mazeGenerator.grid[x, y].IsWall;
                nodes[x, y] = new Node(new Vector2Int(x, y), isWalkable);
            }
    }

    void FindPath(Vector2Int startPosition, Vector2Int endPosition){
        Node startNode = nodes[startPosition.x, startPosition.y];
        Node endNode = nodes[endPosition.x, endPosition.y];

        openList = new List<Node>{startNode};
        closedList = new HashSet<Node>();

        startNode.GCost = 0;
        startNode.HCost = GetHeuristic(startNode, endNode);

        while (openList.Count > 0){
            Node currentNode = GetLowestFCostNode(openList);

            if (currentNode == endNode){
                RetracePath(startNode, endNode);
                return;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (Node neighbor in GetNeighbors(currentNode)){
                if (!neighbor.IsWalkable || closedList.Contains(neighbor))
                    continue;
                
                int tentativeGCost = currentNode.GCost + GetDistance(currentNode, neighbor);
                if (tentativeGCost < neighbor.GCost || !openList.Contains(neighbor)){
                    neighbor.GCost = tentativeGCost;
                    neighbor.HCost = GetHeuristic(neighbor, endNode);
                    neighbor.ParentNode = currentNode;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }
    }

    int GetHeuristic(Node a, Node b){
        int dx = Mathf.Abs(a.GridPosition.x - b.GridPosition.x);
        int dy = Mathf.Abs(a.GridPosition.y - b.GridPosition.y);
        return dx + dy;
    }

    int GetDistance(Node a, Node b){
        return 1;
    }

    Node GetLowestFCostNode(List<Node> nodeList){
        Node lowestFCostNode = nodeList[0];
        
        foreach (Node node in nodeList){
            if (node.FCost < lowestFCostNode.FCost || node.FCost == lowestFCostNode.FCost && node.HCost < lowestFCostNode.HCost)
                lowestFCostNode = node;
        }
        return lowestFCostNode;
    }

    List<Node> GetNeighbors(Node node){
        List<Node> neighbors = new List<Node>();
        int x = node.GridPosition.x;
        int y = node.GridPosition.y;

        if (x - 1 >= 0) neighbors.Add(nodes[x - 1, y]);
        if (x + 1 < mazeGenerator.width) neighbors.Add(nodes[x + 1, y]);
        if (y - 1 >= 0) neighbors.Add(nodes[x, y - 1]);
        if (y + 1 < mazeGenerator.height) neighbors.Add(nodes[x, y + 1]);

        return neighbors;
    }

    void RetracePath(Node startNode, Node endNode){
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode){
            path.Add(currentNode);
            currentNode = currentNode.ParentNode;
        }

        path.Reverse();

        StartCoroutine(DrawPath(path));
    }

    System.Collections.IEnumerator DrawPath(List<Node> path){
        foreach (Node node in path){
            Vector3 position = new Vector3(node.GridPosition.x, 1f, node.GridPosition.y);
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = position;
            sphere.transform.localScale = Vector3.one * 0.5f;
            sphere.GetComponent<Renderer>().material.color = Color.red;
            yield return new WaitForSeconds(0.05f);
        }
    }

}
