using System.Collections;
using B3.DiceSystem;
using UnityEngine;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class ResourceGameState : GameStateBase
    {
        [SerializeField] private DiceThrower diceThrower;
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            float diceRolls = diceThrower.DiceRolls;
            
            // TODO(back):Resources.AddResource(diceRolls);
            stateMachine.ChangeState<PlayerFreeGameState>();
            yield break;
        }
    }
}