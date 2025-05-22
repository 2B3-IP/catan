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
        public override bool BuyHouse(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.House))
                return false;
            
            buildingController.BuildHouse(player);
            RemoveResources(player, BuyItemType.House);
            return true;
        }

        public override bool BuyRoad(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.Road))
                return false;
            buildingController.BuildRoad(player);
            RemoveResources(player, BuyItemType.Road);
            return true;
        }

        public override bool BuyCity(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.City))
                return false;
            buildingController.BuildCity(player);
            RemoveResources(player, BuyItemType.City);
            return true;
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