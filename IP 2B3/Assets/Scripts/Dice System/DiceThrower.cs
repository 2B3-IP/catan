using System.Collections;
using UnityEngine;

namespace B3.DiceSystem
{
    internal sealed class DiceThrower : MonoBehaviour
    {
        private DiceController[] _diceControllers;
        
        public int DiceRolls { get; private set; }

        private void Awake() =>
            _diceControllers = GetComponentsInChildren<DiceController>();

        [ContextMenu("a")]
        public void a()
        {
            StartCoroutine(ThrowCoroutine(transform.position, 5f));
        }
        
        public IEnumerator ThrowCoroutine(Vector3 startPosition, float throwForce)
        {
            var firstDice = _diceControllers[0];
            var secondDice = _diceControllers[1];
            
            var firstThrow = StartCoroutine(firstDice.ThrowCoroutine(startPosition, throwForce));
            var secondThrow = StartCoroutine(secondDice.ThrowCoroutine(startPosition, throwForce));
            
            yield return firstThrow;
            yield return secondThrow;
            
            DiceRolls = firstDice.DiceRoll + secondDice.DiceRoll;
        }
    }
}