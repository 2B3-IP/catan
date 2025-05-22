using System.Collections;
using B3.GameStateSystem;
using B3.PlayerSystem;
using B3.SettlementSystem;
using UnityEngine;

namespace B3.BuildingSystem
{
    public abstract class BuildingControllerBase : MonoBehaviour
    {
        private const int MAX_HOUSES_PER_PLAYER = 5;
        private const int MAX_CITIES_PER_PLAYER = 4;
        private const int MAX_ROADS_PER_PLAYER = 15;
        
        public abstract IEnumerator BuildHouse(PlayerBase player); // ii arati playerului care sunt spatiile ramase libere
                                                                   // , dupa player alege unde vrea sa construiasca,
                                                                   // ii construiecti casa
        public abstract IEnumerator BuildRoad(PlayerBase player);
        public abstract IEnumerator BuildCity(PlayerBase player);

        protected abstract bool CanBuildHouse(SettlementController targetSettlement, PlayerBase player,
            Path[] allPaths);
        protected abstract bool CanBuildRoad(PlayerBase player, Path targetPath, Path[] allPaths);
        

        protected bool CanBuildHouse(PlayerBase player)
        {
            int housesCount = player.GetHousesCount();
            return housesCount < MAX_HOUSES_PER_PLAYER;
        }
        
        protected bool CanBuildCity(PlayerBase player)
        {
            int citiesCount = player.GetCitiesCount();
            return citiesCount < MAX_CITIES_PER_PLAYER;
        }
        
        protected bool CanBuildRoad(PlayerBase player)
        {
            int roadsCount = player.Paths.Count;
            return roadsCount < MAX_ROADS_PER_PLAYER;
        }
    }
}