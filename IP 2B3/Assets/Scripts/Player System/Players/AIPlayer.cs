using System.Collections;
using B3.BoardSystem;
using B3.ThiefSystem;
using UnityEngine;

namespace B3.PlayerSystem
{
    public sealed class AIPlayer : PlayerBase
    {
        [SerializeField] private BoardController boardController;

        public override IEnumerator ThrowDiceCoroutine()
        {
            //DiceSum = Random.Range(MIN_DICE_THROW_FORCE, MAX_DICE_THROW_FORCE); //TODO: TEMP
            yield break;
        }

        public override IEnumerator MoveThiefCoroutine(ThiefControllerBase thiefController)
        {
            yield break; // cea mai buna pozitie pt thief
        }

        public override void OnTradeAndBuildUpdate()
        {
            // verifica daca are de contruit + de dat trade
            // daca nu mai are IsTurnEnded = false;
        }

        public override IEnumerator BuildHouseCoroutine()
        {
            var housePosition = AI.GetHousePosition();
            var boardGrid = boardController.BoardGrid;

            yield return new WaitForSeconds(1f);

            var settlementController = boardGrid.GetVertex(housePosition.Item1, housePosition.Item2);
            SelectedHouse = settlementController;
        }

        public override IEnumerator UpgradeToCityCoroutine()
        {
            var housePosition = AI.GetCityPosition();
            var boardGrid = boardController.BoardGrid;

            yield return new WaitForSeconds(1f);

            var settlementController = boardGrid.GetVertex(housePosition.Item1, housePosition.Item2);
            SelectedHouse = settlementController;
        }

        public override IEnumerator BuildRoadCoroutine()
        {
            var housePosition = AI.GetRoadPosition();
            var boardGrid = boardController.BoardGrid;

            yield return new WaitForSeconds(1f);

            var pathController = boardGrid.GetEdge(housePosition.Item1, housePosition.Item2);
            SelectedPath = pathController;
        }
    }
}