using System.Collections;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class PlayerFreeGameState : GameStateBase
    {
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            // TODO(front/back): trade + build
            
            stateMachine.ChangeState<PlayerEndGameState>();
            yield break;
        }
    }
}