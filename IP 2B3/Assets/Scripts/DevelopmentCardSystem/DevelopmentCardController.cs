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
        [SerializeField] private PlayerBase temp;
        [SerializeField] private CanvasGroup canvasGroup;
        
        public void UseCard(PlayerBase player, DevelopmentCardType cardType)
        {
            var developmentCard = cards[(int)cardType];
            StartCoroutine(developmentCard.UseCard(player, canvasGroup));
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
        
        [Button]
        private void UseKnight()
        {
            Debug.Log("MERGE");
            UseCard(temp, DevelopmentCardType.Knight);
        }
        
        [Button]
        private void UseMonopoly()
        {
            UseCard(temp, DevelopmentCardType.Monopoly);
        }
        
        [Button]
        private void UseRoadBuilding()
        {
            UseCard(temp, DevelopmentCardType.RoadBuilding);
        }
        
        [Button]
        private void UseVictoryPoint()
        {
            UseCard(temp, DevelopmentCardType.VictoryPoint);
        }
        
        [Button]
        private void UseYearOfPlenty()
        {
            UseCard(temp, DevelopmentCardType.YearOfPlenty);
        }
    }
}