using System.Collections;
using B3.BankSystem;
using B3.GameStateSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;
using UnityEngine;

namespace B3.DevelopmentCardSystem
{
    internal sealed class YearOfPlentyDevelopmentCard : DevelopmentCardBase
    {
        [SerializeField] private BankController bankController;
        
        public override IEnumerator UseCard(PlayerBase player)
        {
            var resource = ResourceType.Brick;
            yield return null; //TODO: TEMP, alege o resursa pe care vrei sa o furi
            
            bankController.GetResources(resource, 2);
            player.AddResource(resource, 2);
        }
    }
}