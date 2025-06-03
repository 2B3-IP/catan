using System;
using System.Collections;
using UnityEngine;
using B3.PlayerSystem;
namespace B3.GameStateSystem
{
    [Serializable]
    internal sealed class PlayerEndGameState : GameStateBase
    {
        public static event Action OnPlayerEnd;
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            Debug.Log("PlayerEndGameState OnEnter "+ stateMachine.CurrentPlayer.playerName);
            OnPlayerEnd?.Invoke();
            if(stateMachine.CurrentPlayer is HumanPlayer)
            AI.SendMove("END_TURN");    

            
            stateMachine.StartMachineWithOtherPlayer();
            yield break;
        }
    }
}