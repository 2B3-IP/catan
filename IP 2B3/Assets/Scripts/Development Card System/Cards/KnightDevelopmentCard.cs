using System.Collections;
using B3.PlayerSystem;
using B3.ThiefSystem;
using UnityEngine;

namespace B3.DevelopmentCardSystem
{
    [System.Serializable]
    internal sealed class KnightDevelopmentCard : DevelopmentCardBase
    {
        [SerializeField] private ThiefControllerBase thiefController;

        private int _maxCardsUsed = 2;
        private PlayerBase _currentPlayerHolder;
        
        public override IEnumerator UseCard(PlayerBase player)
        {
            // Debug.Log("IN KNIGTH");
            player.AddUsedKnight();

            if (player.UsedKnightCards > _maxCardsUsed)
            {
                if(_currentPlayerHolder != null)
                    _currentPlayerHolder.RemoveVictoryPoints(2);
                
                player.AddVictoryPoints(2);
                _currentPlayerHolder = player;

                _maxCardsUsed++;
            }
            
            yield return player.MoveThiefCoroutine(thiefController);
            
            var thiefPivot = player.SelectedThiefPiece.transform;
            yield return thiefController.MoveThief(thiefPivot.position);
        }
    }
}