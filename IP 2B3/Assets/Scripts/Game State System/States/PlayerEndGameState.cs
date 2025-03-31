using System.Collections;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class PlayerEndGameState : GameStateBase
    {
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            
            stateMachine.StartMachineWithOtherPlayer();
            yield break;
        }
    }
}