using B3.SettlementSystem;
using B3.PlayerSystem;
using UnityEngine;

namespace B3.BuildingSystem
{
    public class Path : MonoBehaviour
    {
        public SettlementController SettlementA;
        public SettlementController SettlementB;
        public PlayerBase Owner;

        public bool ConnectsTo(SettlementController settlement)
        {
            return SettlementA == settlement || SettlementB == settlement;
        }

        public SettlementController GetOtherSettlement(SettlementController from)
        {
            if (SettlementA == from) return SettlementB;
            if (SettlementB == from) return SettlementA;
            return null;
        }
    }
}