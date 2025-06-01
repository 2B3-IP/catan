using System.Collections;
using B3.DiceSystem;
using B3.ResourcesSystem;
using UnityEngine;
using B3.PieceSystem;
using System.Collections.Generic;
using System.Linq;
using B3.BankSystem;
using B3.BoardSystem;
using B3.BuildingSystem;
using B3.PlayerSystem;
using B3.SettlementSystem;
using B3.UI;
using UnityEngine.Rendering;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class ResourceGameState : GameStateBase
    {
        [SerializeField] private DiceThrower diceThrower;
        [SerializeField] private BankController bankController;
        [SerializeField] private BoardController boardController;
        
        private PieceController[] allPieces;
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            Debug.Log("Resource");
            
            allPieces ??= Object.FindObjectsByType<PieceController>(FindObjectsSortMode.None);
            
            int diceRolls = stateMachine.CurrentPlayer.DiceSum;
            Debug.Log(diceRolls);
            var matchedPieces = allPieces.Where(piece => piece.Number == diceRolls && 
                                                         !piece.IsBlocked && !piece.IsDesert);
            
            var boardGrid = boardController.BoardGrid;
            // todo (front): resource notifications
            foreach (var piece in matchedPieces)
            {
                foreach (var vertex in boardGrid.GetHexVertices(piece.HexPosition))
                {
                    var settlement = vertex.Item1;
                    var owner = settlement.Owner;
                    if(owner == null)
                        continue;
                    var resourceType = piece.ResourceType;
                    int amount = settlement.ResourceAmount;
                  
                    owner.AddResource(resourceType, amount);
                    Debug.Log($"Resource {resourceType} Added to {owner.name}  with amount :{amount}");
                    NotificationManager.Instance.AddNotification($"{owner.colorTag}{owner.playerName}</color> got {amount} {resourceType}.");
                    bankController.GetResources(resourceType, amount);
                }
            }
            
            stateMachine.ChangeState<PlayerFreeGameState>();
            yield break;
        }
    }
}