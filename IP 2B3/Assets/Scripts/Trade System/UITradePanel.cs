using B3.PlayerSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace B3.TradeSystem
{
    internal sealed class UITradePanel : MonoBehaviour
    {
        [SerializeField] private UIResourceButton[] topPlayerButtons;
        [SerializeField] private UIResourceButton[] bottomPlayerButtons;
        [SerializeField] private TradeController tradeController;
        [SerializeField] private HumanPlayer player;
        [SerializeField] private bool isBank;
        [SerializeField] private TMP_Dropdown dropDown;
        [SerializeField] private PlayersManager playersManager;

        public void AcceptTrade()
        {
            var resourcesToGive = new int[5];
            foreach (var button in topPlayerButtons)
                resourcesToGive[(int)button.ResourceType] = button.Count;

            var resourcesToGet = new int[5];
            foreach (var button in bottomPlayerButtons)
                resourcesToGet[(int)button.ResourceType] = button.Count;

            if (isBank)
            {
                tradeController.TradeResources(player, resourcesToGive, resourcesToGet);
                return;
            }

            var otherPlayer = playersManager.players[dropDown.value  + 1];
            tradeController.TradeResources(player, otherPlayer, resourcesToGive, resourcesToGet);
        }

        public void Reset()
        {
            foreach (var button in topPlayerButtons)
                button.Reset();
            
            foreach (var button in topPlayerButtons)
                button.Reset();
        }
    }
}