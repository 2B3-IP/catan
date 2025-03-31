using System.Collections;
using UnityEngine;
using B3.DiceSystem;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class PlayerDiceGameState : GameStateBase
    {
        private const int THIEF_ROLL = 7;
        
        [SerializeField] private DiceThrower diceThrower;

        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            // TODO(back): inlocuieste Vector3.zero cu pozitia camerei playerului curent
            // + trb sa astepti ca playerul ca tina apasat pe un buton iar apoi sa i pasezi forta la coroutina
            // adica, cu cat mai mult tine apasat pe buton cu atat mai tare arunca zaru
            float throwForce = 1f;
            
            yield return diceThrower.ThrowCoroutine(Vector3.zero, throwForce); 
            
            int diceRolls = diceThrower.DiceRolls;
            
            if(diceRolls == THIEF_ROLL) stateMachine.ChangeState<ThiefGameState>();
            else stateMachine.ChangeState<ResourceGameState>();
        }
    }
}