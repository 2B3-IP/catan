using System;
using UnityEngine;
using UnityEngine.UI;

namespace B3.GameStateSystem
{
    public class UIEndPlayerButton : MonoBehaviour
    {
        public static event Action OnEndButtonPressed;

        [SerializeField] private Button endTurnButton;

        private void OnEnable() =>
            endTurnButton.onClick.AddListener(HandleButtonClick);

        private void OnDisable() =>
            endTurnButton.onClick.RemoveListener(HandleButtonClick);
        
        private void HandleButtonClick()
        {
            OnEndButtonPressed?.Invoke();
        }
    }
}