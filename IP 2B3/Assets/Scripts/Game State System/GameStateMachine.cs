using B3.PlayerSystem;
using UnityEngine;

namespace B3.GameStateSystem
{
    public sealed class GameStateMachine : MonoBehaviour
    {
        [SerializeField] private int startStateIndex;
        [SerializeReference] private GameStateBase[] gameStates;
        [SerializeField] private PlayersManager playersManager;

        private GameStateBase _currentState;

        public int _currentPlayerIndex;

        internal PlayerBase CurrentPlayer => playersManager.players[_currentPlayerIndex];
        internal int PlayerCount => playersManager.players.Count;
        internal bool IsLastPlayer => _currentPlayerIndex == PlayerCount - 1;
        internal bool IsFirstPlayer => _currentPlayerIndex == 0;

        private void Start() =>
            StartMachine();

        public void StartMachine()
        {
            var gameState = gameStates[startStateIndex];
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
            StartMachine();
        }

        internal bool ChangePlayer(bool inversedOrder = false)
        {
            CurrentPlayer.IsTurnEnded = true;

            int amount = inversedOrder ? -1 : 1;
            _currentPlayerIndex = (_currentPlayerIndex + amount) % PlayerCount;

            CurrentPlayer.IsTurnEnded = false;
            return _currentPlayerIndex == 0;
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