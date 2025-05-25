using System.Collections;
using B3.ThiefSystem;
using UnityEngine;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class ThiefGameState : GameStateBase
    {
        [SerializeField] private ThiefControllerBase thiefController;
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            Debug.Log("Thief state");
            
            var currentPlayer = stateMachine.CurrentPlayer;
            yield return currentPlayer.MoveThiefCoroutine(thiefController);

            var pieceController = currentPlayer.SelectedThiefPiece;
            var thiefPivot = pieceController.ThiefPivot;
            
            yield return thiefController.MoveThief(thiefPivot.position);
            
            thiefController.BlockPiece(pieceController);
            thiefController.StealFromRandomPlayer(currentPlayer);
            
            stateMachine.ChangeState<PlayerFreeGameState>();
        }
    }
}