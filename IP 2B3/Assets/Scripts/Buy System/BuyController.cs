using System.Collections;
using B3.BankSystem;
using B3.BuildingSystem;
using B3.DevelopmentCardSystem;
using B3.GameStateSystem;
using B3.PlayerInventorySystem;
using B3.PlayerSystem;
using UnityEngine;

namespace B3.BuySystem
{
    internal sealed class BuyController : BuyControllerBase
    {
        [SerializeField] private BuildingController buildingController;
        
        public override IEnumerator BuyHouse(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.House))
                yield break;
            
            Debug.Log("Buying house for: " + player.name);
            yield return buildingController.BuildHouse(player);
            RemoveResources(player, BuyItemType.House);
        }

        public override IEnumerator BuyRoad(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.Road))
                yield break;
            
            yield return buildingController.BuildRoad(player);
            RemoveResources(player, BuyItemType.Road);
        }

        public override IEnumerator BuyCity(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.City))
                yield break;
            
            Debug.Log("Buying city for: " + player.name);
            yield return buildingController.BuildCity(player);
            RemoveResources(player, BuyItemType.City);
        }

        public override DevelopmentCardType? BuyDevelopmentCard(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.DevelopmentCard))
                return null;
            
            var cardType = bankController.BuyDevelopmentCard();
            if (cardType is null) 
                return null;
            
            var inventory = player.GetComponent<PlayerInventoryController>();
            inventory.AddItem(cardType.Value);
            
            RemoveResources(player, BuyItemType.DevelopmentCard);
            return cardType;
        }
        
        
    }
}