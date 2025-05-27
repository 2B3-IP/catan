using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class PlayerFreeGameState : GameStateBase
    {
        [SerializeField] private float _waitTimeRound = 10f;
        [HideInInspector]
        public UnityEvent<float> timeRemainingEvent = new();
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            // TODO(front/back): trade + build, yield return astepti doar dupa end turn button/trece timpu
            Debug.Log("Free");
            var currentPlayer = stateMachine.CurrentPlayer;
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
            
            stateMachine.ChangeState<PlayerEndGameState>();
        }
    }
}