using B3.GameStateSystem;
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
        [SerializeField] public TMP_Text countText;
        
        public int Count { get; private set; }

        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            if(player != null && player.Resources[(int)ResourceType] <= Count)
                return;
            
            Count++;
            countText.SetText(Count.ToString());
        }

        public void Reset()
        {
            Count = 0;
            countText.SetText("0");
        }
    }
}