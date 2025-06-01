using System;
using System.Collections.Generic;
using B3.BoardSystem;
using B3.BuildingSystem;
using B3.GameStateSystem;
using B3.PieceSystem;
using B3.PlayerBuffSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;
using B3.SettlementSystem;
using UnityEngine;

namespace B3.PortSystem
{
    public abstract class PortController : MovingPieceController
    {
        [SerializeField] public BoardController boardController;

        private PieceController _pieceController;

        protected void Awake()
        {
            _pieceController = GetComponent<PieceController>();
        }

        public abstract ResourceType? ResourceType { get; }

        public abstract void AddPlayerBuff(PlayerBase player);

        public bool SetSettlementPosition()
        {
            var hexPosition = _pieceController.HexPosition;
            
            switch (hexPosition.X, hexPosition.Y)
            {
                case (-1, 3):
                {
                    var settlement1 = boardController.BoardGrid.GetVertex(hexPosition.Bottom, HexVertexDir.TopLeft);
                    var settlement2 = boardController.BoardGrid.GetVertex(hexPosition.Bottom, HexVertexDir.TopRight);
                    settlement1.ConnectedPortController = this;
                    settlement2.ConnectedPortController = this;
                    return true;
                    break;
                }
                case (1, 2):
                {
                    var settlement1 = boardController.BoardGrid.GetVertex(hexPosition.Bottom, HexVertexDir.TopLeft);
                    var settlement2 = boardController.BoardGrid.GetVertex(hexPosition.Bottom, HexVertexDir.TopRight);
                    var settlement3 = boardController.BoardGrid.GetVertex(hexPosition.BottomLeft, HexVertexDir.Right);
                    settlement1.ConnectedPortController = this;
                    settlement2.ConnectedPortController = this;
                    settlement3.ConnectedPortController = this;
                    return true;
                    break;
                }
                case (3, 0):
                {
                    var settlement1 = boardController.BoardGrid.GetVertex(hexPosition.BottomLeft, HexVertexDir.Right);
                    var settlement2 = boardController.BoardGrid.GetVertex(hexPosition.BottomLeft, HexVertexDir.TopRight);
                    settlement1.ConnectedPortController = this;
                    settlement2.ConnectedPortController = this;
                    return true;
                    break;
                }
                case (3, -2):
                {
                    var settlement1 = boardController.BoardGrid.GetVertex(hexPosition.TopLeft, HexVertexDir.Right);
                    var settlement2 = boardController.BoardGrid.GetVertex(hexPosition.TopLeft, HexVertexDir.BottomRight);
                    var settlement3 = boardController.BoardGrid.GetVertex(hexPosition.BottomLeft, HexVertexDir.Right);
                    settlement1.ConnectedPortController = this;
                    settlement2.ConnectedPortController = this;
                    settlement3.ConnectedPortController = this;
                    return true;
                    break;
                }
                case (2, -3):
                {
                    var settlement1 = boardController.BoardGrid.GetVertex(hexPosition.TopLeft, HexVertexDir.Right);
                    var settlement2 = boardController.BoardGrid.GetVertex(hexPosition.TopLeft, HexVertexDir.BottomRight);
                    var settlement3 = boardController.BoardGrid.GetVertex(hexPosition.Top, HexVertexDir.BottomLeft);
                    settlement1.ConnectedPortController = this;
                    settlement2.ConnectedPortController = this;
                    settlement3.ConnectedPortController = this;
                    return true;
                    break;
                }
                case (0, -3):
                {
                    var settlement1 = boardController.BoardGrid.GetVertex(hexPosition.Top, HexVertexDir.BottomLeft);
                    var settlement2 = boardController.BoardGrid.GetVertex(hexPosition.Top, HexVertexDir.BottomRight);
                    settlement1.ConnectedPortController = this;
                    settlement2.ConnectedPortController = this;
                    return true;
                    break;
                }
                case (-2, -1):
                {
                    var settlement1 = boardController.BoardGrid.GetVertex(hexPosition.TopRight, HexVertexDir.Left);
                    var settlement2 = boardController.BoardGrid.GetVertex(hexPosition.TopRight, HexVertexDir.BottomLeft);
                    var settlement3 = boardController.BoardGrid.GetVertex(hexPosition.Top, HexVertexDir.BottomRight);
                    settlement1.ConnectedPortController = this;
                    settlement2.ConnectedPortController = this;
                    settlement3.ConnectedPortController = this;
                    return true;
                    break;
                }
                case (-3, 1):
                {
                    var settlement1 = boardController.BoardGrid.GetVertex(hexPosition.TopRight, HexVertexDir.Left);
                    var settlement2 = boardController.BoardGrid.GetVertex(hexPosition.TopRight, HexVertexDir.BottomLeft);
                    var settlement3 = boardController.BoardGrid.GetVertex(hexPosition.BottomRight, HexVertexDir.BottomRight);
                    settlement1.ConnectedPortController = this;
                    settlement2.ConnectedPortController = this;
                    settlement3.ConnectedPortController = this;
                    return true;
                    break;
                }
                case (-3, 3):
                {
                    var settlement1 = boardController.BoardGrid.GetVertex(hexPosition.BottomRight, HexVertexDir.Left);
                    var settlement2 = boardController.BoardGrid.GetVertex(hexPosition.BottomRight, HexVertexDir.TopLeft);
                    settlement1.ConnectedPortController = this;
                    settlement2.ConnectedPortController = this;
                    return true;
                    break;
                }
                default:
                    return false;
            }
        }

        //nu cred ca mai trebuie asta
        private HashSet<SettlementController> GetNearbyCornerSettlements()
        {
            var hexGrid = boardController.BoardGrid;
            var portWorldPos = new Vector2(transform.position.x, transform.position.z);
            float radius = hexGrid.DistanceFromCenter;

            Vector2[] portCorners = GetHexCorners(portWorldPos, radius);
            const float threshold = 0.4f;

            HashSet<SettlementController> nearbySettlements = new();
/*
            foreach (var corner in portCorners)
            {
                // Transforma coltul intr-o pozitie de hex
                HexPosition cornerHexPos = hexGrid.FromWorldPosition(corner);
                var piece = hexGrid[cornerHexPos];

                if (piece == null) continue;

                var settlements = piece.GetComponentsInChildren<SettlementController>();
                foreach (var settlement in settlements)
                {
                    var settlementPos = new Vector2(settlement.transform.position.x, settlement.transform.position.z);

                    if (Vector2.Distance(settlementPos, corner) <= threshold)
                    {
                        nearbySettlements.Add(settlement);
                    }
                }
            }*/

            return nearbySettlements;
        }

        private Vector2[] GetHexCorners(Vector2 center, float radius)
        {
            Vector2[] corners = new Vector2[6];

            for (int i = 0; i < 6; i++)
            {
                float angleRad = Mathf.PI / 3 * i;
                float x = center.x + radius * Mathf.Cos(angleRad);
                float y = center.y + radius * Mathf.Sin(angleRad);
                corners[i] = new Vector2(x, y);
            }

            return corners;
        }
    }
}