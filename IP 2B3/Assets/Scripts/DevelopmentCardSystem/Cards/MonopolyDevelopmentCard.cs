using System.Collections;
using B3.GameStateSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;
using UnityEngine;

namespace B3.DevelopmentCardSystem
{
    [System.Serializable]
    public sealed class MonopolyDevelopmentCard : DevelopmentCardBase
    {
        [SerializeField] private PlayersManager playerManager;
        
        public override IEnumerator UseCard(PlayerBase player, CanvasGroup actions)
        {
            ResourceType? selectedResourceType = null;
            actions.interactable = false;
            yield return UISelectResource.SelectResourceType(resType => selectedResourceType = resType);
            Debug.Assert(selectedResourceType.HasValue);            

            foreach (var otherPlayer in playerManager.players)
            {
                if (otherPlayer == player)
                    continue;
                
                int amount = otherPlayer.Resources[(int)selectedResourceType.Value];
                otherPlayer.RemoveResource(selectedResourceType.Value, amount);
                
                player.AddResource(selectedResourceType.Value, amount);
            }
            actions.interactable = true;
        }
    }
}