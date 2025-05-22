using System.Linq;
using B3.DevelopmentCardSystem;
using UnityEngine;
using B3.ResourcesSystem;

namespace B3.BankSystem
{
    public sealed class BankController : MonoBehaviour
    {
        private readonly int[] _currentResources = {19, 19, 19, 19, 19};
        private readonly int[] _currentDevelopmentCards = {14, 2, 2, 2, 5};
        
        public int[] CurrentResources => _currentResources;
        public int DevCardsCount => _currentDevelopmentCards.Sum();
        
        public bool HasResources(ResourceType resource, int amount) =>
            _currentResources[(int)resource] >= amount;
        
        public void GiveResources(ResourceType resource, int amount) =>
            _currentResources[(int)resource] += amount;
        
        public void GetResources(ResourceType resource, int amount)
        {
            if (!HasResources(resource, amount))
            {
                Debug.Log("Not enough resources");                
                return;
            }
            
            _currentResources[(int)resource] -= amount;
        }
        
        public DevelopmentCardType? BuyDevelopmentCard()
        {
            if(!HasDevelopmentCards())
                return null;
            
            int cardIndex = GetRandomDevelopmentCard();
            
            _currentDevelopmentCards[cardIndex]--;
            return (DevelopmentCardType)cardIndex;
        }

        private bool HasDevelopmentCards()
        {
            foreach (var developmentCard in _currentDevelopmentCards)
            {
                if (developmentCard > 0)
                    return true;
            }

            return false;
        }
        
        private int GetRandomDevelopmentCard()
        {
            int index;
            
            do
            {
                index = Random.Range(0, _currentDevelopmentCards.Length);
            } 
            while (_currentDevelopmentCards[index] == 0);
            
            return index;
        }
    }
}