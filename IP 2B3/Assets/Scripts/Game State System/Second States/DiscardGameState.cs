using System.Collections;
using B3.DiceSystem;
using B3.ResourcesSystem;
using UnityEngine;
using B3.PieceSystem;
using System.Collections.Generic;
using System.Linq;
using B3.BankSystem;
using B3.BoardSystem;
using B3.PlayerSystem;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class DiscardGameState : GameStateBase
    {
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            Debug.Log("discard state");
            stateMachine.ChangeState<ThiefGameState>();
            yield break;
            
            var allPlayers = stateMachine.PlayersManager.players;
            // list<cortoutine> l;
            foreach(var player in allPlayers)
            {
                //if(player is not AIPlayer) // temp
                // l.add(stateMachine.StartCoroutine(player.StartCoroutine(player.DiscardResourcesCoroutine(10f))); //  The timeout for discarding 
            }
            
            // while(ewlapse < ceve)
            //{}
            // for(player) if(*player.discardResources != null) playe.resiurce[i] -= amount; bank.give(i, amount);
            // for(l1 : l) stateMACHINE.stoPCoriutine(l1);
        }
    }
}