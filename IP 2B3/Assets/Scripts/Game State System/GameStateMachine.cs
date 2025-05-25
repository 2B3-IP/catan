using B3.PlayerSystem;
using UnityEngine;

namespace B3.GameStateSystem
{
    public sealed class GameStateMachine : MonoBehaviour
    {
        [SerializeField] private int firstStateIndex;
        [SerializeField] private int secondStateIndex;
        
        [SerializeReference] private GameStateBase[] gameStates;
        [SerializeField] private PlayersManager playersManager;

        private GameStateBase _currentState;

        private int _currentPlayerIndex;

        internal PlayerBase CurrentPlayer => playersManager.players[_currentPlayerIndex];
        internal int PlayerCount => playersManager.players.Count;
        internal bool IsLastPlayer => _currentPlayerIndex == PlayerCount - 1;
        internal bool IsFirstPlayer => _currentPlayerIndex == 0;
        internal PlayersManager PlayersManager => playersManager;

        private void Start() =>
            StartMachine(firstStateIndex);

        public void StartMachine(int index)
        {
            var gameState = gameStates[index];
            ChangeState(gameState);
        }

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
            ChangePlayer();
            StartMachine(secondStateIndex);
        }

        internal void ChangePlayer(bool inversedOrder = false)
        {
            CurrentPlayer.IsTurnEnded = false;

            int amount = inversedOrder ? -1 : 1;
            _currentPlayerIndex = (_currentPlayerIndex + amount) % PlayerCount;
        }

        private void ChangeState(GameStateBase state)
        {
            if (_currentState == state)
                return;

            _currentState = state;

            StopAllCoroutines();
            StartCoroutine(_currentState.OnEnter(this));
        }
    }
}