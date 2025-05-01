using System.Collections;
using UnityEngine;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class PlayerFreeGameState : GameStateBase
    {
        [SerializeField] private float _waitTimeRound = 10f;
        
        private bool _isButtonPressed = false;
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            float elapsedTime = 0f;
            _isButtonPressed = false;
            
            // TODO(front/back): trade + build, yield return astepti doar dupa end turn button/trece timpu
            
            var currentPlayer = stateMachine.CurrentPlayer;
            
            UIEndPlayerButton.OnEndButtonPressed += OnEndTurnPressed;
            
            while (!_isButtonPressed && elapsedTime < _waitTimeRound) {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            UIEndPlayerButton.OnEndButtonPressed -= OnEndTurnPressed;
            _isButtonPressed = false;
            elapsedTime = 0f; 
            
            yield return currentPlayer.EndTurnCoroutine();
            stateMachine.ChangeState<PlayerEndGameState>();
        }
        
        private void OnEndTurnPressed()
        {
            _isButtonPressed = true;
        }
    }
}