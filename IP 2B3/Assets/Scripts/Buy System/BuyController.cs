using B3.GameStateSystem;
using B3.PlayerSystem;
using UnityEngine;

namespace B3.BuySystem
{
    internal sealed class BuyController : BuyControllerBase
    {
        public override bool BuyHouse(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.House))
                return false;
            
            RemoveResources(player, BuyItemType.House);
            return true;
        }

        public override bool BuyRoad(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.Road))
                return false;

            RemoveResources(player, BuyItemType.Road);
            return true;
        }

        public override bool BuyCity(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.City))
                return false;

            RemoveResources(player, BuyItemType.City);
            return true;
        }

        public override bool BuyDevelopmentCard(PlayerBase player)
        {
            if (!HasEnoughResources(player, BuyItemType.DevelopmentCard))
                return false;

            RemoveResources(player, BuyItemType.DevelopmentCard);
            return true;
        }
    }
}