using System.Collections;
using B3.BuildingSystem;
using B3.GameStateSystem;
using B3.PlayerSystem;
using UnityEngine;

namespace B3.DevelopmentCardSystem
{
    internal sealed class RoadBuildingDevelopmentCard : DevelopmentCardBase
    {
        [SerializeField] private BuildingControllerBase buildingController;
        
        public override IEnumerator UseCard(PlayerBase player) => 
            UseCardCoroutine(player);

        private IEnumerator UseCardCoroutine(PlayerBase player)
        {
            yield return buildingController.BuildRoad(player);
            yield return buildingController.BuildRoad(player);
        }
    }
}