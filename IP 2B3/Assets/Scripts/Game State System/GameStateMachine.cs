using B3.PlayerSystem;
using UnityEngine;

namespace B3.GameStateSystem
{
    public sealed class GameStateMachine : MonoBehaviour
    {
        [SerializeReference] private GameStateBase[] gameStates;
        [SerializeField] private PlayersManager playersManager;
        
        private GameStateBase _currentState;

        private int _currentPlayerIndex;
        
        internal PlayerBase CurrentPlayer => playersManager.ActivePlayers[_currentPlayerIndex];
        internal int PlayerCount => playersManager.ActivePlayers.Count;

        public void StartMachine() =>
            ChangeState<PlayerDiceGameState>();
        
        internal void ChangeState<T>()
        {
            foreach (var state in gameStates)
            {
                if (state is not T)
                    continue;
                
                ChangeState(state);
                break;
            }
        }
        
        internal void StartMachineWithOtherPlayer()
        {
            CurrentPlayer.IsTurnEnded = true;
            _currentPlayerIndex = (_currentPlayerIndex + 1) % PlayerCount;
            
            CurrentPlayer.IsTurnEnded = false;
            StartMachine();
        }
        
        private void ChangeState(GameStateBase state)
        {
            if (_currentState == state)
                return;
            
            _currentState = state;
            StartCoroutine(_currentState.OnEnter(this));
        }
    }
}