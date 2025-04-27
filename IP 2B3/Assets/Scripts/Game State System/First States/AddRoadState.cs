using B3.BuildingSystem;
using System.Collections;
using UnityEngine;

namespace B3.GameStateSystem
{
    [System.Serializable]
    public class AddRoadState : GameStateBase
    {
        [SerializeField] private BuildingControllerBase buildingController;
        [SerializeField] private int repeatTimes = 2;
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            var currentPlayer = stateMachine.CurrentPlayer;
            yield return buildingController.BuildRoad(currentPlayer);
       
            bool isFirstPlayer = stateMachine.ChangePlayer();

            if (!isFirstPlayer)
                yield break;

            repeatTimes--;

            if (repeatTimes == 0) stateMachine.ChangeState<PlayerDiceGameState>();
            else stateMachine.ChangeState<AddHouseState>();
        }
    }
}

