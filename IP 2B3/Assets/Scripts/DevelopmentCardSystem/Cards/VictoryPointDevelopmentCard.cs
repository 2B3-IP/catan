using System.Collections;
using B3.GameStateSystem;
using B3.PlayerSystem;

namespace B3.DevelopmentCardSystem
{
    internal sealed class VictoryPointDevelopmentCard : DevelopmentCardBase
    {
        public override IEnumerator UseCard(PlayerBase player)
        {
            player.AddVictoryPoints(1);
            yield break;
        }
    }
}