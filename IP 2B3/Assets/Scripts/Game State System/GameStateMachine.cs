using UnityEngine;

namespace B3.GameStateSystem
{
    public sealed class GameStateMachine : MonoBehaviour
    {
        [SerializeReference] private GameStateBase[] gameStates;
        
        private Player[] _players;
        private GameStateBase _currentState;

        private int _currentPlayerIndex;
        
        internal Player CurrentPlayer => _players[_currentPlayerIndex];

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
            _currentPlayerIndex = (_currentPlayerIndex + 1) % _players.Length;
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