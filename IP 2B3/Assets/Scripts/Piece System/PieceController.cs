using System.Collections.Generic;
using B3.BoardSystem;
using B3.ResourcesSystem;
using B3.SettlementSystem;
using UnityEngine;

namespace B3.PieceSystem
{
    public sealed class PieceController : MovingPieceController
    {
        [field:SerializeField] public ResourceType ResourceType { get; private set; }
        [field:SerializeField] public bool IsBlocked { get; set; }
        [field: SerializeField] public bool IsDesert { get; set; }

        [field:SerializeField] public Transform ThiefPivot { get; private set; }
        public List<SettlementController> Settlements { get; } = new();
        public int Number { get; set; }
        public HexPosition HexPosition { get; set; }
        
        public float radius = 1f;

        public Vector3[] GetEdgeMidpoints()
        {
            Vector3 center = transform.position;
            Vector3[] midpoints = new Vector3[6];

            for (int i = 0; i < 6; i++)
            {
                float angle_deg1 = 60 * i - 30;
                float angle_deg2 = 60 * (i + 1) - 30;
                float angle_rad1 = Mathf.Deg2Rad * angle_deg1;
                float angle_rad2 = Mathf.Deg2Rad * angle_deg2;

                Vector3 p1 = new Vector3(Mathf.Cos(angle_rad1), 0, Mathf.Sin(angle_rad1)) * radius + center;
                Vector3 p2 = new Vector3(Mathf.Cos(angle_rad2), 0, Mathf.Sin(angle_rad2)) * radius + center;

                midpoints[i] = (p1 + p2) / 2;
            }

            return midpoints;
        }
    }
}