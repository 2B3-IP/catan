using UnityEngine;

public class AIManager : MonoBehaviour
{
    void Start()
    {
        AI.StartMoveListener();
    }

    void Update()
    {
        string move = AI.PollMove();
        if (move != "NONE")
        {
            // Debug.Log("Received move in Update: " + move);
        
            // Do something with the move
        }
    }

    void OnApplicationQuit()
    {
        AI.StopMoveListener();
    }

}