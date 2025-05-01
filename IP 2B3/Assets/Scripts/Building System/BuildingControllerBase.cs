using System.Collections;
using B3.GameStateSystem;
using B3.PlayerSystem;
using B3.SettlementSystem;
using UnityEngine;

namespace B3.BuildingSystem
{
    public abstract class BuildingControllerBase : MonoBehaviour
    {
        // private Paths
        
        public abstract IEnumerator BuildHouse(PlayerBase player); // ii arati playerului care sunt spatiile ramase libere
                                                                   // , dupa player alege unde vrea sa construiasca,
                                                                   // ii construiecti casa
        public abstract IEnumerator BuildRoad(PlayerBase player);
        public abstract IEnumerator BuildCity(PlayerBase player);
    }
}