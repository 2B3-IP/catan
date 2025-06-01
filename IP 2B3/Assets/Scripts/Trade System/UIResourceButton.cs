using B3.PlayerSystem;
using B3.ResourcesSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace B3.TradeSystem
{
    internal sealed class UIResourceButton : MonoBehaviour
    {
        [field: SerializeField] public ResourceType ResourceType { get; private set; }
        [SerializeField] private HumanPlayer player;
        
        private TMP_Text _countText;
        public int Count { get; private set; }

        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClick);

            _countText = GetComponentInChildren<TMP_Text>();
            Reset();
        }

        private void OnButtonClick()
        {
            if(player.Resources[(int)ResourceType] <= Count)
                return;
            
            Count++;
            _countText.SetText(Count.ToString());
        }

        public void Reset()
        {
            _countText.SetText("0");
        }
    }
}