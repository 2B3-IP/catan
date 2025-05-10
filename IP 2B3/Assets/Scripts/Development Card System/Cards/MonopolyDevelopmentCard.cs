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
        private bool resourceWasSelected = false;
        private ResourceType chosenResourceType;
        
        public override IEnumerator UseCard(PlayerBase player)
        {   
            resourceWasSelected = false;
            UISelectResource.OnSelectResource += OnChosenResource;
            while (!resourceWasSelected)
                yield return null;

            foreach (var otherPlayer in playerManager.players)
            {
                if (otherPlayer == player)
                    continue;
                
                int amount = otherPlayer.Resources[(int)chosenResourceType];
                otherPlayer.RemoveResource(chosenResourceType, amount);
                
                player.AddResource(chosenResourceType, amount);
            }
            UISelectResource.OnSelectResource -= OnChosenResource;
        }

        private void OnChosenResource(ResourceType chosenResource)
        {
            resourceWasSelected = true;
            this.chosenResourceType = chosenResource;
            
        }
    }
}