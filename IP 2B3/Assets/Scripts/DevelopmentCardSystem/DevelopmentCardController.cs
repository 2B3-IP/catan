using System.Collections.Generic;
using B3.GameStateSystem;
using B3.PlayerSystem;
using NaughtyAttributes;
using UnityEngine;
namespace B3.DevelopmentCardSystem
{
    public sealed class DevelopmentCardController : MonoBehaviour
    {
        [SerializeReference] public List<DevelopmentCardBase> cards;
        
        public void UseCard(PlayerBase player, DevelopmentCardType cardType)
        {
            cards[(int)cardType].UseCard(player);
        }

        [Button]
        void AddClasses()
        {
            cards.Clear();
            cards.Add(new KnightDevelopmentCard());
            cards.Add(new RoadBuildingDevelopmentCard());
            cards.Add(new MonopolyDevelopmentCard());
            cards.Add(new YearOfPlentyDevelopmentCard());
            cards.Add(new VictoryPointDevelopmentCard());
        }
    }
}