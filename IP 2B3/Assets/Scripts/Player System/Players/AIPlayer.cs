using System.Collections;
using B3.BoardSystem;
using B3.BuySystem;
using B3.DiceSystem;
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
        [SerializeField] private DiceThrower diceThrower;
        public int PlayerIndex { get; set; }
        public override IEnumerator ThrowDiceCoroutine()
        {
            yield return diceThrower.ThrowCoroutine();
            // DiceSum = 7;
             DiceSum = diceThrower.DiceRolls;
            AI.SendDice(DiceSum);

        }

        public override IEnumerator MoveThiefCoroutine(ThiefControllerBase thiefController)
        {
            yield return AI.GetThiefPosition();
       
            var pieceController = boardController.BoardGrid[new HexPosition(AI.robberX,AI.robberY)];
            
            yield return new WaitForSeconds(1f);

            SelectedThiefPiece = pieceController;
            
            var thiefPivot = pieceController.ThiefPivot;
            yield return thiefController.MoveThief(thiefPivot.position);
        }

        public override void OnTradeAndBuildUpdate()
        {
            string command="";
       
            if(AI.freeStateReady){
                command = AI.GetFreeStateCommand();
                AI.freeStateReady = false;
                AI.freeState = "";
            }

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
                    
                    //tradeSystem.TradeResources(this, player, playerTradeInfo.Item2);
                    break;

                case "end turn":
                Debug.Log("[Unity1] Ending turn: ");
                    IsTurnEnded = true;
                    
                    break;

                default:
                if(command != "")
                    Debug.Log("AI unknown command " + "{"+ command +"}");
                    break;
            }
        }

        public override IEnumerator BuildHouseCoroutine()
        {
            HexPosition housePosition = new HexPosition(0,0);
            HexVertexDir houseDirection = HexVertexDir.Left;
            yield return AI.GetHousePosition((pos,dir) =>
            {
                housePosition = pos;
                houseDirection = dir;
            });
            var boardGrid = boardController.BoardGrid;
            Debug.Log($"AI building house at {housePosition.X} {housePosition.Y}, {houseDirection}");

            //yield return new WaitForSeconds(1f);

            var settlementController = boardGrid.GetVertex(housePosition, houseDirection);
            SelectedHouse = settlementController;
            yield break;
        }

        public override IEnumerator UpgradeToCityCoroutine()
        {
            HexPosition housePosition = new HexPosition(0,0);
            HexVertexDir houseDirection = HexVertexDir.Left;
            yield return AI.GetCityPosition((pos,dir) =>
            {
                housePosition = pos;
                houseDirection = dir;
            });
            var boardGrid = boardController.BoardGrid;
            Debug.Log($"AI building city at {housePosition.X} {housePosition.Y}, {houseDirection}");

            //yield return new WaitForSeconds(1f);

            var settlementController = boardGrid.GetVertex(housePosition, houseDirection);
            SelectedHouse = settlementController;
            yield break;
        }

        public override IEnumerator BuildRoadCoroutine()
        {
            // initialize to a safe default
            HexPosition roadPosition = new HexPosition(0, 0);
            HexEdgeDir roadDirection = HexEdgeDir.Top;
        
            // wait for the AI to supply a road position
            yield return AI.GetRoadPosition((pos, dir) =>
            {
                roadPosition = pos;
                roadDirection = dir;
            });
        
            var boardGrid = boardController.BoardGrid;
            Debug.Log($"AI building road at {roadPosition.X} {roadPosition.Y}, {roadDirection}");
        
            var pathController = boardGrid.GetEdge(roadPosition, roadDirection);
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