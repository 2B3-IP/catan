using UnityEngine;
using B3.BankSystem;
using B3.GameStateSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;

namespace B3.TradeSystem
{
    public abstract class TradeSystem : MonoBehaviour
    {
        [SerializeField] private BankController bankController;
        
        protected void TradeResources(PlayerBase player, PlayerBase otherPlayer, int[] resourcesToTrade)
        {
            for (int i = 0; i < resourcesToTrade.Length; i++)
            {
                var resource = (ResourceType)i;//aici ar trebui ResourceType(i);
                player.RemoveResource(resource, resourcesToTrade[i]);
                otherPlayer.AddResource(resource, resourcesToTrade[i]);
            }
        }

        // 4 : 1 sau 3 : 1 sau 2 : 1
        protected void TradeResources(PlayerBase player, ResourceType resourceToTrade, ResourceType resourceToGet)
        {
            var playerBuffs = player.PlayerBuffs;
            int resourceCount = playerBuffs.GetResourceAmount(resourceToTrade);
            
            player.RemoveResource(resourceToTrade, resourceCount);
            bankController.GiveResources(resourceToTrade, resourceCount);
            
            bankController.GetResources(resourceToGet, 1);
            player.AddResource(resourceToGet, 1);
        }
    }
}