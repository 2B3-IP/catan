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
            var playerBuffs = player.PlayerBuffs;
            int resourceTypeCount = resourcesGiven.Length;

            int totalBatches = 0;
            for (int i = 0; i < resourceTypeCount; i++)
            {
                int given = resourcesGiven[i];
                if (given > 0)
                {
                    var resourceType = (ResourceType)i;
                    int batchSize = playerBuffs.GetResourceAmount(resourceType);
                    if (batchSize == 0)
                        batchSize = 4;
                    int batches = given / batchSize;
                    totalBatches += batches;
                }
            }

            int totalWanted = 0;
            for (int i = 0; i < resourceTypeCount; i++)
            {
                if (resourcesWanted[i] > 0)
                    totalWanted += resourcesWanted[i];
            }

            if (totalWanted > totalBatches)
            {
                Debug.Log("Player wants more resources than allowed by the resources traded!");
                return;
            }

            for (int i = 0; i < resourceTypeCount; i++)
            {
                if (resourcesGiven[i] > 0)
                {
                    var resourceType = (ResourceType)i;
                    if (player.GetResourceAmount(resourceType) < resourcesGiven[i])
                    {
                        Debug.Log($"Player does not have enough {resourceType} to trade!");
                        return;
                    }
                }
            }

            for (int i = 0; i < resourceTypeCount; i++)
            {
                if (resourcesGiven[i] > 0)
                {
                    var resourceType = (ResourceType)i;
                    player.RemoveResource(resourceType, resourcesGiven[i]);
                    bankController.GiveResources(resourceType, resourcesGiven[i]);
                }
            }

            for (int i = 0; i < resourceTypeCount; i++)
            {
                if (resourcesWanted[i] > 0)
                {
                    var resourceType = (ResourceType)i;
                    bankController.GetResources(resourceType, resourcesWanted[i]);
                    player.AddResource(resourceType, resourcesWanted[i]);
                }
            }
        }
    }
}
