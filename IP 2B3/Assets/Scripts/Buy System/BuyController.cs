using System.Collections;
using B3.BankSystem;
using B3.BuildingSystem;
using B3.DevelopmentCardSystem;
using B3.GameStateSystem;
using B3.PlayerInventorySystem;
using B3.PlayerSystem;
using UnityEngine;
using UnityEngine.Playables;
using B3.UI;
namespace B3.BuySystem
{
    internal sealed class BuyController : BuyControllerBase
    {
        [SerializeField] private BuildingController buildingController;
        
        public override IEnumerator BuyHouse(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.House))
            {
                NotificationManager.Instance
                    .AddNotification($"{player.colorTag}{player.playerName}</color> doesn't have enough resources for a house", 5, false);
                yield break;
            }

            Debug.Log("Buying house for: " + player.name);
            yield return buildingController.BuildHouse(player);
            
            if(buildingController.HasBuilt)
                RemoveResources(player, BuyItemType.House);


            if (player is HumanPlayer && buildingController.HasBuilt)
                AI.SendMove("BUY HOUSE " + player.SelectedHouse.HexPosition.X + " "+ player.SelectedHouse.HexPosition.Y + " " + (int)player.SelectedHouse.VertexDir);
           
        
        }

        public override IEnumerator BuyRoad(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.Road))
            {
                 NotificationManager.Instance
                    .AddNotification($"{player.colorTag}{player.playerName}</color> doesn't have enough resources for a road", 5, true);
                yield break;
            }

            yield return buildingController.BuildRoad(player);

            if (buildingController.HasBuilt)
            {
                RemoveResources(player, BuyItemType.Road);
            }

             if (player is HumanPlayer && buildingController.HasBuilt)
                AI.SendMove("BUY ROAD " + player.SelectedPath.HexPosition.X + " "+ player.SelectedPath.HexPosition.Y + " " + (int)player.SelectedPath.EdgeDir);
        }

        public override IEnumerator BuyCity(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.City))
            {
                NotificationManager.Instance
                    .AddNotification($"{player.colorTag}{player.playerName}</color> doesn't have enough resources for a city", 5, true);
                yield break;
            }

            Debug.Log("Buying city for: " + player.name);
            yield return buildingController.BuildCity(player);
            
            if(buildingController.HasBuilt)
                RemoveResources(player, BuyItemType.City);


                  if (player is HumanPlayer && buildingController.HasBuilt)
                         AI.SendMove("BUY CITY " + player.SelectedHouse.HexPosition.X + " "+ player.SelectedHouse.HexPosition.Y + " " + (int)player.SelectedHouse.VertexDir);
        }

        public override DevelopmentCardType? BuyDevelopmentCard(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.DevelopmentCard))
            {
                NotificationManager.Instance
                    .AddNotification($"{player.colorTag}{player.playerName}</color> doesn't have enough resources for a dev card", 5, true);
                return null;
            }

            var cardType = bankController.BuyDevelopmentCard();
            if (cardType is null)
            {
                 NotificationManager.Instance
                    .AddNotification("there aren't any dev cards in the bank", 5, true);
                return null;
            }

            var inventory = player.GetComponent<PlayerInventoryController>();
            inventory.AddItem(cardType.Value);
            
            RemoveResources(player, BuyItemType.DevelopmentCard);
            return cardType;
        }
        
        
    }
}