using System.Collections;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class PlayerFreeGameState : GameStateBase
    {
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            float elapsedTime = 0f;
            // TODO(front/back): trade + build, yield return astepti doar dupa end turn button/trece timpu
            var currentPlayer = stateMachine.CurrentPlayer;
            
            //while(esteApasat || elapsedTime > timpPerRunda(seriliazeField))
            // elapsedTime += Time.deltaTime;
            // yield return null;
            
            yield return currentPlayer.EndTurnCoroutine();
            stateMachine.ChangeState<PlayerEndGameState>();
        }
    }
}