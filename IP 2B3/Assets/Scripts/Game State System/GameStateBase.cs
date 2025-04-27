using System.Collections;

namespace B3.GameStateSystem
{
    [System.Serializable]
    public abstract class GameStateBase
    {
        public abstract IEnumerator OnEnter(GameStateMachine stateMachine);
    }
}