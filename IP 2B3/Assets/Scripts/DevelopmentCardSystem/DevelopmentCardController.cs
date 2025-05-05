using B3.GameStateSystem;
using B3.PlayerSystem;
using UnityEngine;
namespace B3.DevelopmentCardSystem
{
    public sealed class DevelopmentCardController : MonoBehaviour
    {
        private readonly DevelopmentCardBase[] _cards =
        {
            new KnightDevelopmentCard(),
            new RoadBuildingDevelopmentCard(),
            new MonopolyDevelopmentCard(),
            new YearOfPlentyDevelopmentCard(),
            new VictoryPointDevelopmentCard()
        };
        
        public void UseCard(PlayerBase player, DevelopmentCardType cardType)
        {
            _cards[(int)cardType].UseCard(player);
        }
    }
}