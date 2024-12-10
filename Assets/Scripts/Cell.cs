using UnityEngine;

public class Cell : MonoBehaviour{
    public int x, y;
    public float reward = 0f;
    public bool IsWalkable => !IsWall;
    public bool IsVisited = false;
    public bool IsWall = true;
    public bool IsRevealed = false;

    public float hiddenReward = 0f;

    public void Reveal(){
        IsRevealed = true;
        UpdateCellVisual();
    }

    public void UpdateCellVisual(){
        Renderer renderer = GetComponent<Renderer>();
        if (IsRevealed){
            if (hiddenReward > 0)
                renderer.material.color = Color.green;
            else if (hiddenReward < 0)
                renderer.material.color = Color.red;
            else
                renderer.material.color = Color.gray;
        }
        else
            renderer.material.color = Color.white;
    }
}
