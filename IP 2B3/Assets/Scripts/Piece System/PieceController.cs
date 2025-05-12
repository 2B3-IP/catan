using System.Collections.Generic;
using B3.ResourcesSystem;
using B3.SettlementSystem;
using UnityEngine;

namespace B3.PieceSystem
{
    public sealed class PieceController : MovingPieceController
    {
        [field:SerializeField] public ResourceType ResourceType { get; private set; }
        [field:SerializeField] public bool IsBlocked { get; set; }
        [field:SerializeField] public Transform ThiefPivot { get; private set; }
        public List<SettlementController> Settlements { get; } = new();
        public int Number { get; set; }
        
    }
}