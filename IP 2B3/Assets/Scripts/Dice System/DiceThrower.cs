using System.Collections;
using UnityEngine;

namespace B3.DiceSystem
{
    public sealed class DiceThrower : MonoBehaviour
    {
        private DiceController[] _diceControllers;

        public int DiceRolls { get; private set; }

        private void Awake()
        {
            _diceControllers = GetComponentsInChildren<DiceController>();

            Debug.Log($"Found {_diceControllers.Length} DiceControllers.");
            if (_diceControllers == null || _diceControllers.Length < 2)
            {
                //Debug.LogError("Not enough DiceControllers attached to the DiceThrower object.");
            }
        }
        public IEnumerator ThrowAndWait(Vector3 startPosition, float throwForce)
        {
            Debug.Log($"Throwing from position {startPosition} with force {throwForce}");

            yield return StartCoroutine(ThrowCoroutine(startPosition, throwForce));

            Debug.Log($"Final dice result: {DiceRolls}");
        }

        [ContextMenu("a")]
        public void a()
        {
            Debug.Log("Starting the dice throw.");
            StartCoroutine(ThrowCoroutine(transform.position, 5f));
        }

        public IEnumerator ThrowCoroutine(Vector3 startPosition, float throwForce)
        {
            if (_diceControllers == null || _diceControllers.Length < 2)
            {
               // Debug.LogError("Cannot throw dice: Not enough DiceControllers.");
                yield break;
            }

            var firstDice = _diceControllers[0];
            var secondDice = _diceControllers[1];
            Debug.Log($"Throwing dice with force {throwForce}. Dice count: {_diceControllers.Length}");

            var firstThrow = StartCoroutine(firstDice.ThrowCoroutine(startPosition, throwForce));
            var secondThrow = StartCoroutine(secondDice.ThrowCoroutine(startPosition, throwForce));

            yield return firstThrow;
            yield return secondThrow;

            DiceRolls = firstDice.DiceRoll + secondDice.DiceRoll;

            Debug.Log($"Dice rolled. Total: {DiceRolls} (first: {firstDice.DiceRoll}, second: {secondDice.DiceRoll})");
        }
    }
}