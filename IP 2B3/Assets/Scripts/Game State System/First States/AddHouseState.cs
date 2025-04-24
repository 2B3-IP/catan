using System.Collections;
using UnityEngine;
using B3.BuildingSystem;

namespace B3.GameStateSystem
{
    [System.Serializable]
    public class AddHouseState : GameStateBase
    {
        [SerializeField] private BuildingControllerBase buildingController;
        
        public override IEnumerator OnEnter(GameStateMachine stateMachine)
        {
            var currentPlayer = stateMachine.CurrentPlayer;
            yield return buildingController.BuildCity(currentPlayer);
            
            bool isFirstPlayer = stateMachine.ChangePlayer();
            
            if (isFirstPlayer)
                stateMachine.ChangeState<AddRoadState>();
        }
    }
}