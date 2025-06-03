using UnityEngine;
using B3.BankSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;
using B3.UI;
using System.Linq;
namespace B3.TradeSystem
{
    public class TradeController : MonoBehaviour
    {
        [SerializeField] private BankController bankController;
        
        // +value = daca oferi
        // -value = daca primesti
        public void TradeResources(PlayerBase player, PlayerBase otherPlayer, int[] resourcesToGive, int[] resourcesToGet)
        {
            // if(otherPlayer is AIPlayer)
            // {
            // ai.sendTurn
            // while(false)
            // yield return null;
            
            // if(!ai.accepted)
            //     return;
            // }
            
            for (int i = 0; i < resourcesToGive.Length; i++)
            {
                var resource = (ResourceType)i;

                player.RemoveResource(resource, resourcesToGive[i]);
                otherPlayer.AddResource(resource, resourcesToGive[i]);
            }
            
            for (int i = 0; i < resourcesToGive.Length; i++)
            {
                var resource = (ResourceType)i;

                player.AddResource(resource, resourcesToGet[i]);
                otherPlayer.RemoveResource(resource, resourcesToGet[i]);
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

                    if (given % batchSize != 0)
                    {
                        int returnAmount = given % batchSize;
                        resourcesGiven[i] -= returnAmount;
                    }
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
                NotificationManager.Instance.AddNotification($"You selected more resources than allowed by the resources traded!",5,true);
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
                        NotificationManager.Instance.AddNotification($"You do not have enough {resourceType} to trade!",5,true);
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
    
// public bool TryTradeWithJava(PlayerBase player, PlayerBase otherPlayer, int[] trade)
// {
//     // construim mesajul
//     string[] resourceNames = { "brick", "lumber", "grain", "ore", "wool" };
//     string give = "", get = "";
//     for (int i = 0; i < trade.Length; i++)
//     {
//         int val = trade[i];
//         if (val > 0) give += resourceNames[i] + ":" + val + ",";
//         if (val < 0) get += resourceNames[i] + ":" + (-val) + ",";
//     }
//     give = give.TrimEnd(',');
//     get = get.TrimEnd(',');

//     int from = player.PlayerIndex;
//     int to = otherPlayer.PlayerIndex;
//     string toStr = string.Join(",", Enumerable.Range(0, 4).Select(i => i == to ? "true" : "false"));

//     string message = $"TRADE_OFFER from={from} to={toStr} give={give} get={get}";
//     bool accepted = AI.SendTradeOffer(message);

//     if (accepted)
//     {
//         TradeResources(player, otherPlayer, trade);  // aplicăm trade-ul local
//         Debug.Log("✅ Trade executat (Java a aprobat)");
//         return true;
//     }

//     Debug.Log("❌ Trade respins de bot (Java)");
//     return false;
// }
    }
}
