using System.Collections;
using UnityEngine;
using B3.DiceSystem;

namespace B3.GameStateSystem
{
    [System.Serializable]
    public sealed class PlayerDiceGameState : GameStateBase
    {
        private const int THIEF_ROLL = 7;
        
        [SerializeField] private DiceThrower diceThrower;

        private Camera _camera;

        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            Debug.Log("[State] Entered PlayerDiceGameState");

            _camera ??= Camera.main;

            var currentPlayer = stateMachine.CurrentPlayer;
            Debug.Log($"[State] CurrentPlayer: {currentPlayer?.name}");

            yield return currentPlayer.DiceThrowForceCoroutine();
            Debug.Log("[State] DiceThrowForceCoroutine DONE");
            
            float throwForce = currentPlayer.DiceThrowForce;
            Debug.Log($"[State] Force = {throwForce}");

            var startPosition = _camera.transform.forward+ new Vector3(0, 2, -15);//nu era ok cum era inainte
            Debug.Log($"[State] Throwing from position {startPosition} with force {throwForce}");
            yield return diceThrower.ThrowCoroutine(startPosition, throwForce);
            Debug.Log($"[State] ThrowCoroutine DONE");

            int diceRolls = diceThrower.DiceRolls;
            Debug.Log($"[State] Dice rolled: {diceRolls}");

            if(diceRolls == 7)
                stateMachine.ChangeState<ThiefGameState>();
            else
                stateMachine.ChangeState<ResourceGameState>();

            Debug.Log("[State] ChangeState called");
        }
    }
}