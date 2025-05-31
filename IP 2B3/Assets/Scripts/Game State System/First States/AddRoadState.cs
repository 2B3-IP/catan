using B3.BuildingSystem;
using System.Collections;
using B3.PlayerSystem;
using UnityEngine;

namespace B3.GameStateSystem
{
    [System.Serializable]
    public class AddRoadState : GameStateBase
    {
        [SerializeField] private BuildingControllerBase buildingController;

        private bool _isInverseOrder;

        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            var currentPlayer = stateMachine.CurrentPlayer;
            yield return buildingController.BuildRoad(currentPlayer);

            if (stateMachine.IsLastPlayer && !_isInverseOrder)
            {
                _isInverseOrder = true;
                stateMachine.ChangeState<AddHouseState>();
                yield break;
            }
            if (stateMachine.IsFirstPlayer && _isInverseOrder)
            {
                stateMachine.ChangeState<PlayerDiceGameState>();
                yield break;
            }

            stateMachine.ChangePlayer(_isInverseOrder);
            stateMachine.ChangeState<AddHouseState>();
        }
    }
}