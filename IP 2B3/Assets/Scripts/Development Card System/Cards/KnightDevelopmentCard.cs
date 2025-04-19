using System.Collections;
using B3.GameStateSystem;
using B3.PlayerSystem;
using B3.ThiefSystem;
using UnityEngine;

namespace B3.DevelopmentCardSystem
{
    internal sealed class KnightDevelopmentCard : DevelopmentCardBase
    {
        [SerializeField] private ThiefController thiefController;
        
        public override IEnumerator UseCard(PlayerBase player)
        {
            // click on a piece and give position
            //yield return thiefController.MoveThief();
            yield break;
        }
    }
}