using System;
using B3.GameStateSystem;
using B3.PlayerSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace B3.UI
{
    public class HumanPlayerToggle : MonoBehaviour
    {
        public GameStateMachine gameStateMachine;
        public CanvasGroup canvasGroup;

        private void Start()
        {
            gameStateMachine.onCurrentPlayerChanged.AddListener(UpdateInteractive);
            gameStateMachine.onStateChanged.AddListener(UpdateInteractive);

            UpdateInteractive();
        }

        private void OnDestroy()
        {
            gameStateMachine.onCurrentPlayerChanged.RemoveListener(UpdateInteractive);   
        }

        private void UpdateInteractive()
        {
            canvasGroup.interactable = gameStateMachine.CurrentPlayer is HumanPlayer
                && gameStateMachine.IsInState<PlayerFreeGameState>();
        }
    }
    
}