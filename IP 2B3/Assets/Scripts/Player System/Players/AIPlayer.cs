using System.Collections;
using B3.ThiefSystem;
using UnityEngine;

namespace B3.PlayerSystem
{
    public sealed class AIPlayer : PlayerBase
    {
        public override IEnumerator DiceThrowForceCoroutine()
        {
            DiceThrowForce = Random.Range(MIN_DICE_THROW_FORCE, MAX_DICE_THROW_FORCE); //TODO: TEMP
            yield break;
        }

        public override IEnumerator MoveThiefCoroutine(ThiefController thiefController)
        {
            yield break; // cea mai buna pozitie pt thief
        }

        public override void OnTradeAndBuildUpdate()
        {
            // verifica daca are de contruit + de dat trade
            // daca nu mai are IsTurnEnded = false;
        }
    }
}