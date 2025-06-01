using UnityEngine;
using B3.BankSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;
using System.Collections.Generic;
using System.Linq;

namespace B3.TradeSystem
{
    public class TradeController : MonoBehaviour
    {
        [SerializeField] private BankController bankController;
        
        // +value = daca oferi
        // -value = daca primesti
        public void TradeResources(PlayerBase player, PlayerBase otherPlayer, int[] resourcesToTrade)
        {
            // if(otherPlayer is AIPlayer)
            // {
            // ai.sendTurn
            // while(false)
            // yield return null;
            
            // if(!ai.accepted)
            //     return;
            // }
            
            
            for (int i = 0; i < resourcesToTrade.Length; i++)
            {
                //print(i);
                int value = resourcesToTrade[i];
                var resource = (ResourceType)i;

                if (value > 0)
                {
                    player.RemoveResource(resource, resourcesToTrade[i]);
                    otherPlayer.AddResource(resource, resourcesToTrade[i]);
                    //print(i);
                }
                else
                {
                    player.AddResource(resource, -resourcesToTrade[i]);
                    otherPlayer.RemoveResource(resource, -resourcesToTrade[i]);
                }
                
            }
        }

        // 4 : 1 sau 3 : 1 sau 2 : 1
        public void TradeResources(PlayerBase player, int[] resourcesGiven, int[] resourcesWanted)
        {
            /*
            var playerBuffs = player.PlayerBuffs;
            var resourcesGivenDict = new Dictionary<ResourceType, int>();
            foreach (var res in resourcesGiven)
            {
                if (!resourcesGivenDict.ContainsKey(res))
                    resourcesGivenDict[res] = 0;
                resourcesGivenDict[res]++;
            }
            var batchSizeForResource = new Dictionary<ResourceType, int>();
            foreach (var res in resourcesGivenDict.Keys)
            {
                int batchSize = playerBuffs.GetResourceAmount(res);
                if (batchSize == 0)
                    batchSize = 4;
                batchSizeForResource[res] = batchSize;
            }
            int totalBatches = 0;
            foreach (var pair in resourcesGivenDict)
            {
                int batchSize = batchSizeForResource[pair.Key];
                int batches = pair.Value / batchSize;
                totalBatches += batches;
            }
            if (resourcesWanted.Length > totalBatches)
            {
                Debug.Log("Player wants more resources than allowed by the resources traded!");
                return;
            }
            foreach (var pair in resourcesGivenDict)
            {
                if (player.GetResourceAmount(pair.Key) < pair.Value)
                {
                    Debug.Log("Player does not have enough resources to trade!");
                    return;
                }
            }
            foreach (var pair in resourcesGivenDict)
            {
                player.RemoveResource(pair.Key, pair.Value);
                bankController.GiveResources(pair.Key, pair.Value);
            }
            foreach (var res in resourcesWanted)
            {
                bankController.GetResources(res, 1);
                player.AddResource(res, 1);
            }*/
        }
    }
}
