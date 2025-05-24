using System.Collections;
using B3.BuySystem;
using B3.PlayerSystem;
using UnityEngine;
using UnityEngine.UI;

namespace B3.BankSystem.UI
{
    internal sealed class UIBuyButton : MonoBehaviour
    {
        [SerializeField] private BuyController buyController;
        [SerializeField] private PlayerBase temp;
        [SerializeField] private BuyItemType itemType;
        
        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            var coroutine =  itemType switch
            {
                BuyItemType.House => buyController.BuyHouse(temp),
                BuyItemType.Road => buyController.BuyRoad(temp),
                BuyItemType.City => buyController.BuyCity(temp),
                _ => null
            };

            StartCoroutine(coroutine);
        }
    }
}