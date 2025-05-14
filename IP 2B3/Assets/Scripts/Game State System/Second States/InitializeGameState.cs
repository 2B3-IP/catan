using System.Collections;
using B3.BoardSystem;
using UnityEngine;
using B3.DiceSystem;
using B3.PlayerSystem;
using Game_Settings;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class InitializeGameState : GameStateBase
    {  
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private BoardController boardController;
        [SerializeField] private PlayersManager playersManager;
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {  

            if (gameSettings.autoGenerateBoard)
            {
                boardController.Generate();
            }
            playersManager.Initialize(gameSettings.numberOfPlayers);
            
            stateMachine.ChangeState<PlayerDiceGameState>();
            yield break;

        }
    }
}