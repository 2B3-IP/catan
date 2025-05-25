using System.Collections;
using System.Linq;
using B3.BoardSystem;
using UnityEngine;
using B3.DiceSystem;
using B3.PieceSystem;
using B3.PlayerSystem;
using B3.ThiefSystem;
using Game_Settings;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class InitializeGameState : GameStateBase
    {  
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private BoardController boardController;
        [SerializeField] private PlayersManager playersManager;
        [SerializeField] private ThiefController thiefController;
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {  
            if (gameSettings.autoGenerateBoard)
                boardController.Generate();
            
            playersManager.Initialize(gameSettings.numberOfPlayers);
            
            stateMachine.ChangeState<AddHouseState>();
            
            var allPieces = Object.FindObjectsByType<PieceController>(FindObjectsSortMode.None);
            foreach (var piece in allPieces)
            {
                if (!piece.IsDesert)
                    continue;

                yield return thiefController.MoveThief(piece.ThiefPivot.position);
                break;
            }
        }
    }
}