using System.Collections;
using UnityEngine;

namespace B3.DiceSystem
{
    public sealed class DiceThrower : MonoBehaviour
    {
        [SerializeField] private DiceController[] _diceControllers;
        
        public int DiceRolls { get; private set; }

        public IEnumerator ThrowCoroutine()
        {
            var startPosition = transform.position;
            float throwForce = Random.Range(1f, 10f);
            
            var firstDice = _diceControllers[0];
            var secondDice = _diceControllers[1];
            
            firstDice.gameObject.SetActive(true);
            secondDice.gameObject.SetActive(true);
            
            var firstThrow = StartCoroutine(firstDice.ThrowCoroutine(startPosition, throwForce));
            var secondThrow = StartCoroutine(secondDice.ThrowCoroutine(startPosition, throwForce));
            
            yield return firstThrow;
            yield return secondThrow;
            
            DiceRolls = firstDice.DiceRoll + secondDice.DiceRoll;
            
            // AI.SendDice(DiceRolls);
        }
    }
}