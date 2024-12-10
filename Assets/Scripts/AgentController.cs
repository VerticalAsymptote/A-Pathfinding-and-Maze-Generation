using UnityEngine;

public class AgentController : MonoBehaviour{
    public MazeStateManager mazeStateManager;
    private Cell currentState;

    void Start(){
        if (mazeStateManager == null)
            mazeStateManager = FindAnyObjectByType<MazeStateManager>();

        var startCoords = (0, 0);
        if (mazeStateManager.cellStates.ContainsKey(startCoords)){
            currentState = mazeStateManager.cellStates[startCoords];
            transform.position = new Vector3(currentState.x, 0, currentState.y);
        }
        else
            Debug.LogError("Start state not found!");
    }
}
