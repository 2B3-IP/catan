using System.Collections.Generic;
using B3.DevelopmentCardSystem;
using B3.GameStateSystem;
using B3.PlayerSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace B3.PlayerInventorySystem
{
    public sealed class PlayerInventoryController : MonoBehaviour
    {
        private readonly List<PlayerItem> _playerItems = new();
        
        public DevelopmentCardController developmentCardController;
        private PlayerBase _player;
        
        public int PlayerCount => _playerItems.Count;
        
        private void Awake() =>
            _player = GetComponent<PlayerBase>();
        
        private void OnEnable() =>
            PlayerEndGameState.OnPlayerEnd += OnPlayerEndGameState;
        
        private void OnDisable() =>
            PlayerEndGameState.OnPlayerEnd -= OnPlayerEndGameState;
        
        public void Initialize(DevelopmentCardController developmentCardController) =>
            this.developmentCardController = developmentCardController;

        public void AddItem(DevelopmentCardType cardType)
        {
            var playerItem = new PlayerItem(cardType);
            _playerItems.Add(playerItem);
        }

        public void UseItem(DevelopmentCardType cardType)
        {
            if (HasCard(cardType))
                developmentCardController.UseCard(_player, cardType);
        }

        private bool HasCard(DevelopmentCardType cardType)
        {
            for (int i = 0; i < _playerItems.Count; i++)
            {
                var item = _playerItems[i];
                if (item.CardType != cardType)
                    continue;

                if (!item.CanBeUsed)
                    continue;
                
                _playerItems.RemoveAt(i);
                return true;
            }

            return false;
        }

        private void OnPlayerEndGameState()
        {
            for (int i = 0; i < _playerItems.Count; i++)
            {
                var item = _playerItems[i];
                item.CanBeUsed = true;
                
                _playerItems[i] = item;
            }
        }
    }
}