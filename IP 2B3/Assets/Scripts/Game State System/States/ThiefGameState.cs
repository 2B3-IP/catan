using System.Collections;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class ThiefGameState : GameStateBase
    {
        //[SerializeField] private Theif theif;
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            // TODO(front): steal from players, move thief, etc
            // theif.move(position)
            stateMachine.ChangeState<PlayerFreeGameState>();
            yield break;
        }
    }
}