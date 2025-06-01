using B3.GameStateSystem;
using UnityEngine;

namespace B3.TradeSystem
{
    internal sealed class UIDisable : MonoBehaviour
    {
        private void OnEnable()
        {
            PlayerEndGameState.OnPlayerEnd += OnEndState;
        }
        
        private void OnDisable()
        {
            PlayerEndGameState.OnPlayerEnd -= OnEndState;
        }

        private void OnEndState()
        {
            gameObject.SetActive(false);
        }
    }
}