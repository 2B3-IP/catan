using System;
using UnityEngine;
using UnityEngine.UI;

namespace B3.GameStateSystem
{
    public class UIEndPlayerButton
    {
        public static event Action OnEndButtonPressed;

        [SerializeField] private Button endTurnButton;

        private void Awake()
        {
            if (endTurnButton == null)
            {
                endTurnButton = GetComponent<Button>();
            }

            endTurnButton.onClick.AddListener(HandleButtonClick);
        }

        private T GetComponent<T>()
        {
            throw new NotImplementedException();
        }

        private void HandleButtonClick()
        {
            OnEndButtonPressed?.Invoke();
        }

        private void OnDestroy()
        {
            endTurnButton.onClick.RemoveListener(HandleButtonClick);
        }
    }
}