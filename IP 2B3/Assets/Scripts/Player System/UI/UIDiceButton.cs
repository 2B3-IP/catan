using System;
using UnityEngine;
using UnityEngine.UI;

namespace B3.PlayerSystem.UI
{
    public class UIDiceButton : MonoBehaviour
    {
        public static event Action OnButtonClick;
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }
        private void OnClick()
        {
            OnButtonClick?.Invoke();
        }
    }
}