using System;
using System.Collections;
using UnityEngine;

namespace B3.GameStateSystem
{
    [Serializable]
    internal sealed class PlayerEndGameState : GameStateBase
    {
        public static event Action OnPlayerEnd;
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            Debug.Log("PlayerEndGameState OnEnter");
            OnPlayerEnd?.Invoke();
            
            AI.SendMove("end round");
            
            stateMachine.StartMachineWithOtherPlayer();
            yield break;
        }
    }
}