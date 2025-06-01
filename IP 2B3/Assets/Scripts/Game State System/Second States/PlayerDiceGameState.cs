using System;
using System.Collections;
using UnityEngine;
using B3.DiceSystem;
using B3.PlayerSystem;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class PlayerDiceGameState : GameStateBase
    {
        [SerializeField] private CanvasGroup diceButton;
        
        public static event Action OnDiceGameState;
        private const int THIEF_ROLL = 7;
        
        [SerializeField] private DiceThrower diceThrower;

        private Transform _cameraTransform;

        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            var currentPlayer = stateMachine.CurrentPlayer;
            
            if(currentPlayer is HumanPlayer)
                diceButton.interactable = true;
            
            OnDiceGameState?.Invoke();
            _cameraTransform ??= Camera.main.transform;
            
            // TODO(front): inlocuieste Vector3.zero cu pozitia camerei playerului curent
            // + trb sa astepti ca playerul ca tina apasat pe un buton iar apoi sa i pasezi forta la coroutina
            // adica, cu cat mai mult tine apasat pe buton cu atat mai tare arunca zaru

            
            yield return currentPlayer.ThrowDiceCoroutine();
            
            
            //float throwForce = currentPlayer.DiceSum;
            // player - de la input
            // ai - random

            //var startPosition = _cameraTransform.forward;
            //yield return diceThrower.ThrowCoroutine(startPosition, throwForce);
            
            int diceRolls = currentPlayer.DiceSum;
            Debug.Log("dice: " + diceRolls + " " + (diceRolls == THIEF_ROLL));
            // AI.SendDice(diceRolls);

            if(currentPlayer is HumanPlayer)
                diceButton.interactable = false;

            if (diceRolls == THIEF_ROLL) stateMachine.ChangeState<DiscardGameState>();
            else stateMachine.ChangeState<ResourceGameState>();
        }
    }
}