using System.Collections;
using B3.ThiefSystem;
using UnityEngine;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class ThiefGameState : GameStateBase
    {
        [SerializeField] private ThiefController thief;
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            // TODO(front): steal from players, move thief, etc
            
            var currentPlayer = stateMachine.CurrentPlayer;
            yield return currentPlayer.MoveThiefCoroutine(thief);
            
            
            
            stateMachine.ChangeState<PlayerFreeGameState>();
        }
    }
}