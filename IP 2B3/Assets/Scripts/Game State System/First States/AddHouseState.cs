using System.Collections;
using B3.BankSystem;
using B3.BoardSystem;
using UnityEngine;
using B3.BuildingSystem;
using B3.PieceSystem;
using B3.PlayerSystem;
using B3.SettlementSystem;

namespace B3.GameStateSystem
{
    [System.Serializable]
    public class AddHouseState : GameStateBase
    {
        [SerializeField] private BuildingControllerBase buildingController;
        [SerializeField] private BoardController boardController;
        [SerializeField] private BankController bankController;

        private bool _giveResources;
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            var currentPlayer = stateMachine.CurrentPlayer;
            yield return buildingController.BuildHouse(currentPlayer);

            if (stateMachine.IsLastPlayer)
                _giveResources = true;
            
            if(_giveResources)
                AddResourcesForSettlement(currentPlayer.SelectedHouse, currentPlayer);

            stateMachine.ChangeState<AddRoadState>();
        }
        
        private void AddResourcesForSettlement(SettlementController selectedHouse, PlayerBase player)
        {
            var boardGrid = boardController.BoardGrid;
            var vertexPosition = selectedHouse.HexPosition;
            var vertexDirection = selectedHouse.VertexDir;
            
            var pieceController = boardGrid[vertexPosition];
            if (pieceController != null)
            {
                player.AddResource(pieceController.ResourceType, 1);
                bankController.GetResources(pieceController.ResourceType, 1);
            }
            
            switch (vertexDirection)
            {
                case HexVertexDir.TopRight :
                {
                    AddResourcesToPlayer(HexEdgeDir.TopRight, HexEdgeDir.Top, vertexPosition, player);
                    break;
                }
                
                case HexVertexDir.Right:
                {
                    AddResourcesToPlayer(HexEdgeDir.TopRight,HexEdgeDir.BottomRight,vertexPosition,player);
                    break;
                }
                
                case HexVertexDir.BottomRight:
                {
                    AddResourcesToPlayer(HexEdgeDir.BottomRight, HexEdgeDir.Bottom, vertexPosition, player);
                    break;
                }
                
                case HexVertexDir.BottomLeft:
                {
                    AddResourcesToPlayer(HexEdgeDir.BottomLeft, HexEdgeDir.Bottom, vertexPosition, player);
                    break;
                }
                
                case HexVertexDir.Left:
                {
                    AddResourcesToPlayer(HexEdgeDir.BottomLeft, HexEdgeDir.TopLeft, vertexPosition, player);
                    break;
                }
                
                case HexVertexDir.TopLeft:
                {
                    AddResourcesToPlayer(HexEdgeDir.TopLeft, HexEdgeDir.Top, vertexPosition, player);
                    break;
                }
            }
        }
        
        private void AddResourcesToPlayer(HexEdgeDir dir1, HexEdgeDir dir2, HexPosition vertexPosition, 
            PlayerBase player)
        {
            var boardGrid = boardController.BoardGrid;
            
            var hex1 = vertexPosition.GetNeighbour(dir1);
            var hex2 = vertexPosition.GetNeighbour(dir2);

            var pieceController1 = boardGrid[hex1];
            var pieceController2 = boardGrid[hex2];

            if (pieceController1 != null)
            {
                player.AddResource(pieceController1.ResourceType, 1);
                bankController.GetResources(pieceController1.ResourceType, 1);
            }

            if (pieceController2 != null)
            {
                player.AddResource(pieceController2.ResourceType, 1);
                bankController.GetResources(pieceController2.ResourceType, 1);
            }
        }
    }
}