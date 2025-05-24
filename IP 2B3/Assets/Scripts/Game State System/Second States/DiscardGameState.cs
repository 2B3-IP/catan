using System.Collections;
using B3.DiceSystem;
using B3.ResourcesSystem;
using UnityEngine;
using B3.PieceSystem;
using System.Collections.Generic;
using System.Linq;
using B3.BankSystem;
using B3.BoardSystem;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class DiscardGameState : GameStateBase
    {
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            var allPlayers = stateMachine.PlayersManager.players;
            foreach(var player in allPlayers )
            {
               yield return player.StartCoroutine(player.DiscardResourcesCoroutine(10f)); //  The timeout for discarding 
            }
            
            stateMachine.ChangeState<ThiefGameState>();
               
            yield break;
        }
       
        
    }
}