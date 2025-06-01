using System.Collections;
using B3.BuildingSystem;
using B3.GameStateSystem;
using B3.PlayerSystem;
using UnityEngine;

namespace B3.DevelopmentCardSystem
{
    [System.Serializable]
    public sealed class RoadBuildingDevelopmentCard : DevelopmentCardBase
    {
        [SerializeField] private BuildingControllerBase buildingController;

        public override IEnumerator UseCard(PlayerBase player, CanvasGroup actions)
        {
            actions.interactable = false;
            
            yield return buildingController.BuildRoad(player);
            yield return buildingController.BuildRoad(player);
            
            actions.interactable = true;
        }
    }
}