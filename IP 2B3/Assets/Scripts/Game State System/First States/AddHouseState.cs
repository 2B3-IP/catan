using System.Collections;
using UnityEngine;
using B3.BuildingSystem;
using B3.PlayerSystem;

namespace B3.GameStateSystem
{
    [System.Serializable]
    public class AddHouseState : GameStateBase
    {
        [SerializeField] private BuildingControllerBase buildingController;

        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            var currentPlayer = stateMachine.CurrentPlayer;
            if (currentPlayer is not AIPlayer)
                yield return buildingController.BuildHouse(currentPlayer);

            stateMachine.ChangeState<AddRoadState>();
        }
    }
}