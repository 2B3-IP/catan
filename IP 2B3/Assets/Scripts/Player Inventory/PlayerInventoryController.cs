﻿using System.Collections.Generic;
using B3.DevelopmentCardSystem;
using B3.GameStateSystem;
using B3.PlayerSystem;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

namespace B3.PlayerInventorySystem
{
    public sealed class PlayerInventoryController : MonoBehaviour
    {
        private readonly List<PlayerItem> _playerItems = new();
        
        public DevelopmentCardController developmentCardController;
        private PlayerBase _player;
        
        public UnityEvent onItemCountChanged;
        
        public int ItemCount => _playerItems.Count;
        
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
            onItemCountChanged.Invoke();
        }

        public bool UseItem(DevelopmentCardType cardType)
        {
            if (HasCard(cardType))
            {
                developmentCardController.UseCard(_player, cardType);
                onItemCountChanged.Invoke();
                return true;
            }
            
            return false;
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

        [Button("Make all items usable")]
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