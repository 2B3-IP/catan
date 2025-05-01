using System.Collections;
using B3.DiceSystem;
using B3.ResourcesSystem;
using UnityEngine;
using B3.PieceSystem;
using System.Collections.Generic;
using System.Linq;

namespace B3.GameStateSystem
{
    [System.Serializable]
    internal sealed class ResourceGameState : GameStateBase
    {
        [SerializeField] private DiceThrower diceThrower;
        [SerializeField] private List<PieceController> allPieces;
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            float diceRolls = diceThrower.DiceRolls;

            var macthedPieces = allPieces.Where(piece => piece.Number == (int)diceRolls && !piece.IsBlocked);

            foreach (var piece in macthedPieces)
            {
                foreach (var settlement in piece.Settlements)
                {
                    if (!settlement.HasOwner)
                        continue;
                    var owner = settlement.Owner;
                    var resourceType = piece.ResourceType;
                    int amount = settlement.ResourceAmount;
                    
                    owner.AddResource(resourceType, amount);
                }
            }
            //resourceController.AddResources(diceRolls);
            stateMachine.ChangeState<PlayerFreeGameState>();
            yield break;
        }
    }
}