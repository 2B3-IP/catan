using UnityEngine;

public class AIManager : MonoBehaviour
{
    void Start()
    {
        AI.StartMoveListener();
    }
    public bool a = true;
    void Update()
    {
        if(a)
        Time.timeScale= 3f; 
        else
        Time.timeScale= 1f;
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