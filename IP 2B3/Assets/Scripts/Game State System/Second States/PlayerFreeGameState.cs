﻿using System.Collections;
using B3.PlayerSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class PlayerFreeGameState : GameStateBase
    {
        [SerializeField] private float _waitTimeRound = 10f;
        [SerializeField] private CanvasGroup endTurnButton;

        [HideInInspector]
        public UnityEvent<float> timeRemainingEvent = new();
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            var currentPlayer = stateMachine.CurrentPlayer;
            if(currentPlayer is HumanPlayer)
                endTurnButton.interactable = true;
            
            // TODO(front/back): trade + build, yield return astepti doar dupa end turn button/trece timpu
            Debug.Log("Free");
            var endTurnCoroutine = currentPlayer.StartCoroutine(currentPlayer.EndTurnCoroutine());
            
            float elapsedTime = 0f; 
            while (elapsedTime < _waitTimeRound) 
            {
                elapsedTime += Time.deltaTime;
                timeRemainingEvent?.Invoke(_waitTimeRound - elapsedTime);
                if (currentPlayer.IsTurnEnded)
                    break;
                
                yield return null;
            }
            
            Debug.Log("aici");
            if(!currentPlayer.IsTurnEnded)
                currentPlayer.StopCoroutine(endTurnCoroutine);
            
            if(currentPlayer is HumanPlayer)
                endTurnButton.interactable = false;
            stateMachine.ChangeState<PlayerEndGameState>();
        }
    }
}