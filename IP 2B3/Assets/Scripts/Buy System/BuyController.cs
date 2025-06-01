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
                var warningNotif = NotificationManager.Instance
                    .AddNotification("Player "+player.playerName+" doesn't have enough resources for a house", 5, false);
                yield break;
            }

            Debug.Log("Buying house for: " + player.name);
            yield return buildingController.BuildHouse(player);
            
            if(buildingController.HasBuilt)
                RemoveResources(player, BuyItemType.House);
        }

        public override IEnumerator BuyRoad(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.Road))
            {
                var warningNotif = NotificationManager.Instance
                    .AddNotification("Player "+player.playerName+" doesn't have enough resources for a road", 5, false);
                yield break;
            }

            yield return buildingController.BuildRoad(player);

            if (buildingController.HasBuilt)
            {
                RemoveResources(player, BuyItemType.Road);
            }
        }

        public override IEnumerator BuyCity(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.City))
            {
                var warningNotif = NotificationManager.Instance
                    .AddNotification("Player "+player.playerName+" doesn't have enough resources for a city", 5, false);
                yield break;
            }

            Debug.Log("Buying city for: " + player.name);
            yield return buildingController.BuildCity(player);
            
            if(buildingController.HasBuilt)
                RemoveResources(player, BuyItemType.City);
        }

        public override DevelopmentCardType? BuyDevelopmentCard(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.DevelopmentCard))
            {
                var warningNotif = NotificationManager.Instance
                    .AddNotification("Player "+player.playerName+" doesn't have enough resources for a dev card", 5, false);
                return null;
            }

            var cardType = bankController.BuyDevelopmentCard();
            if (cardType is null)
            {
                var warningNotif = NotificationManager.Instance
                    .AddNotification("there aren't any dev cards in the bank", 5, false);
                return null;
            }

            var inventory = player.GetComponent<PlayerInventoryController>();
            inventory.AddItem(cardType.Value);
            
            RemoveResources(player, BuyItemType.DevelopmentCard);
            return cardType;
        }
        
        
    }
}