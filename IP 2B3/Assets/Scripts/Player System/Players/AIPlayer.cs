using System.Collections;
using B3.BoardSystem;
using B3.BuySystem;
using B3.ThiefSystem;
using B3.TradeSystem;
using UnityEngine;

namespace B3.PlayerSystem
{
    public sealed class AIPlayer : PlayerBase
    {
        [SerializeField] private BoardController boardController;
        [SerializeField] private BuyController buyController;
        [SerializeField] private TradeSystem tradeSystem;
        
        public override IEnumerator ThrowDiceCoroutine()
        {
            //DiceSum = Random.Range(MIN_DICE_THROW_FORCE, MAX_DICE_THROW_FORCE); //TODO: TEMP
            yield break;
        }

        public override IEnumerator MoveThiefCoroutine(ThiefControllerBase thiefController)
        {
            var thiefPosition = AI.GetThiefPostion();
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
                    break;

                case "buy city":
                    StartCoroutine(buyController.BuyCity(this));
                    break;

                case "buy road":
                    StartCoroutine(buyController.BuyRoad(this));
                    break;

                case "buy card":
                    buyController.BuyDevelopmentCard(this);
                    break;
                
                case "trade bank":
                    var resourcesToGive = AI.GetBankTradeInfo().Item1;
                    var resourcesToTake = AI.GetBankTradeInfo().Item2;

                    tradeSystem.TradeResources(this, resourcesToGive, resourcesToTake);
                    break;

                case "trade player":
                    var player = AI.GetPlayerTradeInfo().Item1;
                    var resourcesToTrade = AI.GetPlayerTradeInfo().Item2;

                    tradeSystem.TradeResources(player, resourcesToTrade);
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
            /*var housePosition = AI.GetHousePosition();
            var boardGrid = boardController.BoardGrid;

            //yield return new WaitForSeconds(1f);

            var settlementController = boardGrid.GetVertex(housePosition.Item1, housePosition.Item2);
            SelectedHouse = settlementController;*/
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
            /*var housePosition = AI.GetRoadPosition();
            var boardGrid = boardController.BoardGrid;

            //yield return new WaitForSeconds(1f);

            var pathController = boardGrid.GetEdge(housePosition.Item1, housePosition.Item2);
            SelectedPath = pathController;*/
            yield break;
        }

        public override IEnumerator DiscardResourcesCoroutine(float timeout)
        {
            yield return new WaitForSeconds(1f);
            var discardedResources = AI.GetDiscardedResources();
            
            yield break;
        }
    }
}