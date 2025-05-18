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
        [SerializeField] private BoardController boardController;
        
        private PieceController _pieceController;

        protected override void Awake()
        {
            base.Awake();
            _pieceController = GetComponent<PieceController>();
        }

        public abstract ResourceType? ResourceType { get; }

        public abstract void AddPlayerBuff(PlayerBase player);

        public bool IsSettlementPosition(SettlementController settlement)
        {
            // luam vertexul de pe partea opusa pe ideea ca 
            //locul portului de pe hexul de port este in oglinda cu locul hexului
            //de pe hexul tile
            HexVertexDir settlementHexDir = settlement.VertexDir.Opposite();

            var hexPosition = _pieceController.HexPosition;
            switch (hexPosition.X, hexPosition.Y)
            {
                case (-1, 3):
                {
                    if (settlementHexDir.Equals(HexVertexDir.BottomLeft) ||
                        settlementHexDir.Equals(HexVertexDir.BottomRight))
                        return true;
                    break;
                }
                case (1, 2):
                {
                    if (settlementHexDir.Equals(HexVertexDir.BottomLeft) ||
                        settlementHexDir.Equals(HexVertexDir.BottomRight))
                        return true;
                    break;
                }
                case (3, 0):
                {
                    if (settlementHexDir.Equals(HexVertexDir.BottomLeft) ||
                        settlementHexDir.Equals(HexVertexDir.Left))
                        return true;
                    break;
                }
                case (3, -2):
                {
                    if (settlementHexDir.Equals(HexVertexDir.TopLeft) ||
                        settlementHexDir.Equals(HexVertexDir.Left))
                        return true;
                    break;
                }
                case (2, -3):
                {
                    if (settlementHexDir.Equals(HexVertexDir.TopLeft) ||
                        settlementHexDir.Equals(HexVertexDir.Left))
                        return true;
                    break;
                }
                case (0, -3):
                {
                    if (settlementHexDir.Equals(HexVertexDir.TopLeft) ||
                        settlementHexDir.Equals(HexVertexDir.TopRight))
                        return true;
                    break;
                }
                case (-2, -1):
                {
                    if (settlementHexDir.Equals(HexVertexDir.TopRight) ||
                        settlementHexDir.Equals(HexVertexDir.Right))
                        return true;
                    break;
                }
                case (-3, 1):
                {
                    if (settlementHexDir.Equals(HexVertexDir.TopRight) ||
                        settlementHexDir.Equals(HexVertexDir.Right))
                        return true;
                    break;
                }
                case (-3, 3):
                {
                    if (settlementHexDir.Equals(HexVertexDir.BottomRight) ||
                        settlementHexDir.Equals(HexVertexDir.Right))
                        return true;
                    break;
                }
                default:
                    return false;
            }
            return false;
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