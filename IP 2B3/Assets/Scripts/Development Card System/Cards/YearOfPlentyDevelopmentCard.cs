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
        
        public override IEnumerator UseCard(PlayerBase player)
        {
            ResourceType? selectedResourceType = null;
            yield return UISelectResource.SelectResourceType(resType => selectedResourceType = resType);
            Debug.Assert(selectedResourceType.HasValue); 
            
            bankController.GetResources(selectedResourceType.Value, 2);
            player.AddResource(selectedResourceType.Value, 2);
            
        }
    }
}