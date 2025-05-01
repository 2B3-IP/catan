using System.Collections;
using B3.GameStateSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;
using UnityEngine;

namespace B3.DevelopmentCardSystem
{
    internal sealed class MonopolyDevelopmentCard : DevelopmentCardBase
    {
        [SerializeField] private PlayersManager playerManager;
        
        public override IEnumerator UseCard(PlayerBase player)
        {
            var resource = ResourceType.Brick;
            yield return null; //TODO: TEMP, alege o resursa pe care vrei sa o furi

            foreach (var otherPlayer in playerManager.ActivePlayers)
            {
                if (otherPlayer == player)
                    continue;
                
                int amount = otherPlayer.Resources[(int)resource];
                otherPlayer.RemoveResource(resource, amount);
                
                player.AddResource(resource, amount);
            }
        }
    }
}