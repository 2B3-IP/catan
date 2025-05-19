using System;
using System.Collections;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class PlayerEndGameState : GameStateBase
    {
        public static event Action OnPlayerEnd;
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            OnPlayerEnd?.Invoke();
            stateMachine.StartMachineWithOtherPlayer();
            
            AI.SendMove("end round");
            yield break;
        }
    }
}