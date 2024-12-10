public class Cell{
    public int x, y;
    public float reward;
    public bool IsVisited = false;
    public bool IsWall = true;
    public bool IsWalkable => !IsWall;
}
