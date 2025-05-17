using System.Collections.Generic;
using B3.BoardSystem;
using B3.BuildingSystem;
using B3.GameStateSystem;
using B3.PieceSystem;
using B3.PlayerBuffSystem;
using B3.PlayerSystem;
using B3.SettlementSystem;
using UnityEngine;

namespace B3.PortSystem
{
    public abstract class PortController : MovingPieceController
    {
        [SerializeField] private HousePivot[] portTransform;
        [SerializeField] private BoardController boardController;

        protected PlayerBuffs OwnerBuffs
        {
            get
            {
                foreach (var housePivot in portTransform)
                {
                    var owner = housePivot.Owner;

                    if (owner != null)
                        return owner.GetComponent<PlayerBuffs>();
                }

                return null;
            }
        }

        public abstract void AddPlayerBuff(PlayerBase player);
        
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