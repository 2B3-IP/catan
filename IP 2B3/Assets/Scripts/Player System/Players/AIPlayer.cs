using System.Collections;
using B3.BoardSystem;
using B3.BuySystem;
using B3.ThiefSystem;
using B3.TradeSystem;
using B3.UI;
using UnityEngine;

namespace B3.PlayerSystem
{
    public sealed class AIPlayer : PlayerBase
    {
        [SerializeField] private BoardController boardController;
        [SerializeField] private BuyController buyController;
        [SerializeField] private TradeController tradeSystem;
        [SerializeField] private PlayersManager playersManager;

        public override IEnumerator ThrowDiceCoroutine()
        {
            DiceSum = Random.Range(1, 7) + Random.Range(1, 7); //TODO: TEMP
            yield break;
        }

        public override IEnumerator MoveThiefCoroutine(ThiefControllerBase thiefController)
        {
            var thiefPosition = AI.GetThiefPosition();
            var pieceController = boardController.BoardGrid[thiefPosition];
            
            yield return new WaitForSeconds(1f);

            SelectedThiefPiece = pieceController;
            
            var thiefPivot = pieceController.ThiefPivot;
            yield return thiefController.MoveThief(thiefPivot.position);
        }

        public override void OnTradeAndBuildUpdate()
        {
            var command = AI.GetFreeStateCommand();

            switch (command.ToLower())
            {
                case "buy house":
                    StartCoroutine(buyController.BuyHouse(this));
                    NotificationManager.Instance.AddNotification($"{this.colorTag}{this.playerName}</color> built a house.");
                    break;

                case "buy city":
                    StartCoroutine(buyController.BuyCity(this));
                    NotificationManager.Instance.AddNotification($"{this.colorTag}{this.playerName}</color> built a city.");
                    break;

                case "buy road":
                    StartCoroutine(buyController.BuyRoad(this));
                    NotificationManager.Instance.AddNotification($"{this.colorTag}{this.playerName}</color> built a road.");
                    break;

                case "buy card":
                    buyController.BuyDevelopmentCard(this);
                    NotificationManager.Instance.AddNotification($"{this.colorTag}{this.playerName}</color> bought a development card.");
                    break;
                
                case "trade bank":
                    var bankTradeInfo = AI.GetBankTradeInfo();
                    tradeSystem.TradeResources(this, bankTradeInfo.Item1, bankTradeInfo.Item2);
                    break;

                case "trade player":
                    var playerTradeInfo = AI.GetPlayerTradeInfo();
                    var player = playersManager.players[playerTradeInfo.Item1]; 
                    
                    tradeSystem.TradeResources(this, player, playerTradeInfo.Item2);
                    break;

                case "end turn":
                    IsTurnEnded = true;
                    break;

                default:
                    Debug.Log("AI unknown command");
                    break;
            }
        }

        public override IEnumerator BuildHouseCoroutine()
        {
            var housePosition = AI.GetHousePosition();
            var boardGrid = boardController.BoardGrid;

            //yield return new WaitForSeconds(1f);

            var settlementController = boardGrid.GetVertex(housePosition.Item1, housePosition.Item2);
            SelectedHouse = settlementController;
            yield break;
        }

        public override IEnumerator UpgradeToCityCoroutine()
        {
            var housePosition = AI.GetCityPosition();
            var boardGrid = boardController.BoardGrid;

            yield return new WaitForSeconds(1f);

            var settlementController = boardGrid.GetVertex(housePosition.Item1, housePosition.Item2);
            SelectedHouse = settlementController;
        }

        public override IEnumerator BuildRoadCoroutine()
        {
            var housePosition = AI.GetRoadPosition();
            var boardGrid = boardController.BoardGrid;

            //yield return new WaitForSeconds(1f);

            var pathController = boardGrid.GetEdge(housePosition.Item1, housePosition.Item2);
            SelectedPath = pathController;
            yield break;
        }

        public override IEnumerator DiscardResourcesCoroutine(float timeout)
        {
            yield return new WaitForSeconds(1f);
            DiscardResources = AI.GetDiscardedResources();
        }
    }
}