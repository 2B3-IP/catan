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
        private const float WATER_SIZE = 22.86f;
        
        [SerializeField] private int waterGridSize = 10;
        [SerializeField] private GameObject waterPrefab;
        
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private BoardController boardController;
        [SerializeField] private PlayersManager playersManager;
        [SerializeField] private ThiefController thiefController;
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {  
            if (gameSettings.autoGenerateBoard)
                boardController.Generate();

            SpawnWater();
            playersManager.Initialize(gameSettings.numberOfPlayers);
            
            var allPieces = Object.FindObjectsByType<PieceController>(FindObjectsSortMode.None);
            foreach (var piece in allPieces)
            {
                if (!piece.IsDesert)
                    continue;

                Debug.Log(piece, piece);
                yield return thiefController.MoveThief(piece.ThiefPivot.position);
                break;
            }
            
            stateMachine.ChangeState<AddHouseState>();
        }

        private void SpawnWater()
        {
            int half = waterGridSize / 2;

            for (int x = -half; x < half; x++)
            {
                for (int z = -half; z < half; z++)
                {
                    Vector3 position = new Vector3(x * WATER_SIZE, -6.3f, z * WATER_SIZE);
                    Object.Instantiate(waterPrefab, position, Quaternion.identity);
                }
            }
        }
    }
}