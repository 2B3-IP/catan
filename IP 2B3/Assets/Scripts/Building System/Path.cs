using B3.SettlementSystem;
using B3.PlayerSystem;
using B3.BoardSystem;
using UnityEngine;

namespace B3.BuildingSystem
{
    public class Path : MonoBehaviour 
    {
        
        public PlayerBase Owner { get; set; }
        public bool IsBuilt { get; set; } = false;
        
        public HexPosition HexPosition { get; set; }
        public HexEdgeDir EdgeDir { get; set; }
        
        public bool IsNearEdge(Vector3 edgeMidpoint)
        {
            return Vector3.Distance(this.transform.position, edgeMidpoint) <= 0.3f;
        }
    }
}