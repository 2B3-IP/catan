using System;
using B3.GameStateSystem;
using TMPro;
using UnityEngine;

public class TurnDisplay : MonoBehaviour
{
    public TMP_Text turnText;
    public GameStateMachine gameStateMachine;

    private void FixedUpdate()
    {
        var currentPlayer = gameStateMachine.CurrentPlayer;
        turnText.text = $"{currentPlayer.colorTag}{currentPlayer.playerName}</color>'s turn";
    }
}
