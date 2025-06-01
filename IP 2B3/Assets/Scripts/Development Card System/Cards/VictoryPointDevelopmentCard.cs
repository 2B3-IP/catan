using System.Collections;
using B3.GameStateSystem;
using B3.PlayerSystem;
using UnityEngine;

namespace B3.DevelopmentCardSystem
{
    [System.Serializable]
    public sealed class VictoryPointDevelopmentCard : DevelopmentCardBase
    {
        public override IEnumerator UseCard(PlayerBase player, CanvasGroup actions)
        {
            player.AddVictoryPoints(1);
            yield break;
        }
    }
}