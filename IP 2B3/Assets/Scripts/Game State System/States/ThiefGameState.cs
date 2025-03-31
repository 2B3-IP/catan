using System.Collections;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class ThiefGameState : GameStateBase
    {
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            // TODO(back): steal from players, move thief, etc
            
            stateMachine.ChangeState<PlayerFreeGameState>();
            yield break;
        }
    }
}