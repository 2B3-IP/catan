using UnityEngine;
using B3.BankSystem;
using B3.GameStateSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;
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
        public void TradeResources(PlayerBase player, ResourceType resourceToTrade, ResourceType resourceToGet)
        {
            var playerBuffs = player.PlayerBuffs;
            int resourceCount = playerBuffs.GetResourceAmount(resourceToTrade);
            
            player.RemoveResource(resourceToTrade, resourceCount);
            bankController.GiveResources(resourceToTrade, resourceCount);
            
            bankController.GetResources(resourceToGet, 1);
            player.AddResource(resourceToGet, 1);
        }
    
public bool TryTradeWithJava(PlayerBase player, PlayerBase otherPlayer, int[] trade)
{
    // construim mesajul
    string[] resourceNames = { "brick", "lumber", "grain", "ore", "wool" };
    string give = "", get = "";
    for (int i = 0; i < trade.Length; i++)
    {
        int val = trade[i];
        if (val > 0) give += resourceNames[i] + ":" + val + ",";
        if (val < 0) get += resourceNames[i] + ":" + (-val) + ",";
    }
    give = give.TrimEnd(',');
    get = get.TrimEnd(',');

    int from = player.PlayerIndex;
    int to = otherPlayer.PlayerIndex;
    string toStr = string.Join(",", Enumerable.Range(0, 4).Select(i => i == to ? "true" : "false"));

    string message = $"TRADE_OFFER from={from} to={toStr} give={give} get={get}";
    bool accepted = AI.SendTradeOffer(message);

    if (accepted)
    {
        TradeResources(player, otherPlayer, trade);  // aplicăm trade-ul local
        Debug.Log("✅ Trade executat (Java a aprobat)");
        return true;
    }

    Debug.Log("❌ Trade respins de bot (Java)");
    return false;
}
    }
}
