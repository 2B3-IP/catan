﻿using System.Collections.Generic;
using B3.DevelopmentCardSystem;
using B3.GameStateSystem;
using B3.PlayerSystem;
using UnityEngine;

namespace B3.PlayerInventorySystem
{
    public sealed class PlayerInventoryController : MonoBehaviour
    {
        private readonly List<PlayerItem> _playerItems = new();
        
        private DevelopmentCardController _developmentCardController;
        private PlayerBase _player;
        
        private void Awake() =>
            _player = GetComponent<PlayerBase>();
        
        private void OnEnable() =>
            PlayerEndGameState.OnPlayerEnd += OnPlayerEndGameState;
        
        private void OnDisable() =>
            PlayerEndGameState.OnPlayerEnd -= OnPlayerEndGameState;
        
        public void Initialize(DevelopmentCardController developmentCardController) =>
            _developmentCardController = developmentCardController;

        public void AddItem(DevelopmentCardType cardType)
        {
            var playerItem = new PlayerItem(cardType);
            _playerItems.Add(playerItem);
        }

        public void UseItem(DevelopmentCardType cardType)
        {
            if (HasCard(cardType))
                _developmentCardController.UseCard(_player, cardType);
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