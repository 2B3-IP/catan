using System.Collections;
using B3.DiceSystem;
using B3.ResourcesSystem;
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
            
            //resourceController.AddResources(diceRolls);
            stateMachine.ChangeState<PlayerFreeGameState>();
            yield break;
        }
    }
}