using B3.BuildingSystem;
using System.Collections;
using UnityEngine;

namespace B3.GameStateSystem
{
    public class AddRoadState : RepeatedGameStateBase
    {
        [SerializeField] private BuildingControllerBase buildingController;

        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            var currentPlayer = stateMachine.CurrentPlayer;
            yield return buildingController.BuildRoad(currentPlayer);
       
            bool isFirstPlayer = stateMachine.ChangePlayer();

            if (!isFirstPlayer)
                yield break;

            RepeatTimes--;

            if (RepeatTimes == 0) stateMachine.ChangeState<PlayerDiceGameState>();
            else stateMachine.ChangeState<AddHouseState>();
        }
    }
}

