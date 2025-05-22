using System.Collections;
using B3.BankSystem;
using B3.GameStateSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;
using UnityEngine;

namespace B3.DevelopmentCardSystem
{
    [System.Serializable]
    public sealed class YearOfPlentyDevelopmentCard : DevelopmentCardBase
    {
        [SerializeField] private BankController bankController;
        private bool resourceWasSelected = false;
        private ResourceType selectedResourceType;
        
        public override IEnumerator UseCard(PlayerBase player)
        {
            resourceWasSelected = false;
            UISelectResource.OnSelectResource += OnChosenResource;
            while (!resourceWasSelected)
                yield return null;
            
            
            bankController.GetResources(selectedResourceType, 2);
            player.AddResource(selectedResourceType, 2);
            
            UISelectResource.OnSelectResource -= OnChosenResource;
        }

        private void OnChosenResource(ResourceType chosenResource)
        {
            resourceWasSelected = true;
            selectedResourceType = chosenResource;
        }
    }
}