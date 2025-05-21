using System.Collections;
using UnityEngine;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class PlayerFreeGameState : GameStateBase
    {
        [SerializeField] private float _waitTimeRound = 10f;
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            // TODO(front/back): trade + build, yield return astepti doar dupa end turn button/trece timpu
            var currentPlayer = stateMachine.CurrentPlayer;
            var endTurnCoroutine = currentPlayer.StartCoroutine(currentPlayer.EndTurnCoroutine());
            float elapsedTime = 0f; 
            while (elapsedTime < _waitTimeRound) 
            {
                elapsedTime += Time.deltaTime;
                if (currentPlayer.IsTurnEnded)
                    break;
                yield return null;
            }

            Debug.Log("aici");
            currentPlayer.StopAllCoroutines();
            
            stateMachine.ChangeState<PlayerEndGameState>();
        }
    }
}