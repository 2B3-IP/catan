using B3.PlayerSystem;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace B3.GameStateSystem
{
    public sealed class GameStateMachine : MonoBehaviour
    {
        [SerializeField] private int firstStateIndex;
        [SerializeField] private int secondStateIndex;
        
        [SerializeReference] private GameStateBase[] gameStates;
        [SerializeField] private PlayersManager playersManager;
        
        public UnityEvent onCurrentPlayerChanged = new();
        public UnityEvent onStateChanged = new();
        
        private GameStateBase _currentState;

        private int _currentPlayerIndex;

        internal PlayerBase CurrentPlayer => playersManager.players[_currentPlayerIndex];
        internal int PlayerCount => playersManager.players.Count;
        internal bool IsLastPlayer => _currentPlayerIndex == PlayerCount - 1;
        internal bool IsFirstPlayer => _currentPlayerIndex == 0;
        internal PlayersManager PlayersManager => playersManager;

        private void Start() => StartMachine(firstStateIndex);

        public void StartMachine(int index)
        {
            var gameState = gameStates[index];
            ChangeState(gameState);
        }

        public T GetState<T>() where T : GameStateBase
        {
            foreach (var state in gameStates)
            {
                if (state is T t)
                    return t;
            }
            return null;
        }

        public bool IsInState<T>() where T : GameStateBase => _currentState is T;
        
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
            Debug.Log("START MACHINE WITH PLAYER: " + _currentPlayerIndex);
        }

        internal void ChangePlayer(bool inversedOrder = false)
        {
            CurrentPlayer.IsTurnEnded = false;
            
            int amount = inversedOrder ? -1 : 1;
            Debug.Log("[SM] current: " + _currentPlayerIndex + " amount: " + amount + " PlayerCount: " + PlayerCount);
            _currentPlayerIndex = (_currentPlayerIndex + amount) % PlayerCount;
            onCurrentPlayerChanged?.Invoke();
            
            Debug.Log("[SM]Player: " + _currentPlayerIndex);
        }

       
        private void ChangeState(GameStateBase state)
        {
            if (_currentState == state)
                return;

            _currentState = state;
            
            StopAllCoroutines();
            StartCoroutine(_currentState.OnEnter(this));
            onStateChanged?.Invoke();
        }
    }
}