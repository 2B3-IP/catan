using System;
using System.Collections;
using UnityEngine;
using B3.DiceSystem;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class PlayerDiceGameState : GameStateBase
    {
        public static event Action OnDiceGameState;
        private const int THIEF_ROLL = 7;
        
        [SerializeField] private DiceThrower diceThrower;

        private Transform _cameraTransform;

        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            OnDiceGameState?.Invoke();
            _cameraTransform ??= Camera.main.transform;
            
            // TODO(front): inlocuieste Vector3.zero cu pozitia camerei playerului curent
            // + trb sa astepti ca playerul ca tina apasat pe un buton iar apoi sa i pasezi forta la coroutina
            // adica, cu cat mai mult tine apasat pe buton cu atat mai tare arunca zaru

            var currentPlayer = stateMachine.CurrentPlayer;
            
            yield return currentPlayer.ThrowDiceCoroutine();
            
            
            //float throwForce = currentPlayer.DiceSum;
            // player - de la input
            // ai - random

            //var startPosition = _cameraTransform.forward;
            //yield return diceThrower.ThrowCoroutine(startPosition, throwForce);
            
            int diceRolls = currentPlayer.DiceSum;
            Debug.Log("dice: " + diceRolls + " " + (diceRolls == THIEF_ROLL));
            // AI.SendDice(diceRolls);


            if (diceRolls == THIEF_ROLL) stateMachine.ChangeState<DiscardGameState>();
            else stateMachine.ChangeState<ResourceGameState>();
        }
    }
}