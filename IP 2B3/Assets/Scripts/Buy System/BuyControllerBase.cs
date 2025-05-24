using System.Collections;
using B3.BankSystem;
using B3.DevelopmentCardSystem;
using UnityEngine;
using B3.GameStateSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;

namespace B3.BuySystem
{
    public abstract class BuyControllerBase : MonoBehaviour
    {
        [SerializeField] protected BankController bankController;

        private readonly int[] _inversedItemsCosts =
        {
            22221, // house
            22111, // road
            11314, // city
            11222 // development card
        };

        public abstract IEnumerator BuyHouse(PlayerBase player);
        public abstract IEnumerator BuyRoad(PlayerBase player);
        public abstract IEnumerator BuyCity(PlayerBase player);
        public abstract DevelopmentCardType? BuyDevelopmentCard(PlayerBase player);

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

            var resources = player.Resources;
            for (int i = 0; i < resources.Length; i++)
            {
                int resourceCost = itemCost % 10 - 1;
                if (resourceCost == 0)
                    continue;

                player.RemoveResource((ResourceType)i, resourceCost);
                bankController.GiveResources((ResourceType)i, resourceCost);

                itemCost /= 10;
            }
        }
    }
}