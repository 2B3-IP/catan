using UnityEngine;

public class AIManager : MonoBehaviour
{
    void Start()
    {
        return;
        AI.StartMoveListener();
    }

    void Update()
    {
        return;
        string move = AI.PollMove();
        if (move != "NONE")
        {
            // Debug.Log("Received move in Update: " + move);
        
            // Do something with the move
        }
    }

    void OnApplicationQuit()
    {
        return;
        AI.StopMoveListener();
    }

}