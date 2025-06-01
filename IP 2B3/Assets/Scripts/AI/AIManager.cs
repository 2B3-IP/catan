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
            Debug.Log("Received move in Update: " + move);
            AI.ProcessMove(move); // This is safe
        }
        // print ai.getHoues position with a text
    }

    void OnApplicationQuit()
    {
        AI.StopMoveListener();
    }

}