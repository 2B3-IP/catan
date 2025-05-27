using System;
using B3.GameStateSystem;
using B3.PlayerSystem;
using TMPro;
using UnityEngine;

public class TurnDisplay : MonoBehaviour
{
    public TMP_Text turnText;
    public GameStateMachine gameStateMachine;
    public float timeRemainingTextSize = 20f;

    private float timeRemaining = float.NaN;
    
    private void Start()
    {
        gameStateMachine.onCurrentPlayerChanged.AddListener(UpdateText);
        gameStateMachine.onStateChanged.AddListener(() =>
        {
            timeRemaining = float.NaN;
            UpdateText();
        });
        
        gameStateMachine.GetState<PlayerFreeGameState>().timeRemainingEvent.AddListener(t =>
        {
            timeRemaining = t;
            UpdateText();
        });
        
        UpdateText();
    }

    private void UpdateText()
    {
        var currentPlayer = gameStateMachine.CurrentPlayer;
        turnText.text = $"{currentPlayer.colorTag}{currentPlayer.playerName}</color>'s turn\n";

        if (!float.IsNaN(timeRemaining))
        {
            var minutes = (int)timeRemaining / 60;
            var seconds = (int)(timeRemaining % 60);
        turnText.text += $"<size={timeRemainingTextSize}>{minutes}:{seconds, 2}</size>";
        }
    }
}
