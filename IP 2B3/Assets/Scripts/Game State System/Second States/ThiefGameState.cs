using System.Collections;
using B3.ThiefSystem;
using UnityEngine;

namespace B3.GameStateSystem
{
    [System.Serializable]
    public sealed class ThiefGameState : GameStateBase
    {
        [SerializeField] private ThiefControllerBase thief;
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            
            var currentPlayer = stateMachine.CurrentPlayer;
            yield return currentPlayer.MoveThiefCoroutine(thief);
            var currentPiece = currentPlayer.SelectedThiefPiece;
            thief.BlockPiece(currentPiece);
            thief.StealFromRandomPlayer(currentPlayer);
            
            stateMachine.ChangeState<PlayerFreeGameState>();
        }
    }
}