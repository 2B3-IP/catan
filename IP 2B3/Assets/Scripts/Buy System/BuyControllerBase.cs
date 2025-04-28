using B3.BankSystem;
using UnityEngine;
using B3.GameStateSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;

namespace B3.BuySystem
{
    public abstract class BuyControllerBase : MonoBehaviour
    {
        [SerializeField] private BankController bankController;

        private readonly int[] _inversedItemsCosts =
        {
            22221, // house
            22111, // road
            11314, // city
            11222 // development card
        };

        public abstract bool BuyHouse(PlayerBase player);
        public abstract bool BuyRoad(PlayerBase player);
        public abstract bool BuyCity(PlayerBase player);
        public abstract bool BuyDevelopmentCard(PlayerBase player);

        protected bool HasEnoughResources(PlayerBase player, BuyItemType itemType)
        {
            int itemCost = _inversedItemsCosts[(int)itemType];

            foreach (var resources in player.Resources)
            {
                int resourceCost = itemCost % 10 - 1;
                if (resources < resourceCost)
                    return false;

                itemCost /= 10;
            }

            return true;
        }

        protected void RemoveResources(PlayerBase player, BuyItemType itemType)
        {
            int itemCost = _inversedItemsCosts[(int)itemType];

            foreach (var resource in player.Resources)
            {
                int resourceCost = itemCost % 10 - 1;
                if (resourceCost == 0)
                    continue;

                player.RemoveResource((ResourceType)resource, resourceCost);
                bankController.GiveResources((ResourceType)resource, resourceCost);

                itemCost /= 10;
            }

        }
    }
}