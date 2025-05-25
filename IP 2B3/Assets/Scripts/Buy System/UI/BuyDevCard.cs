using System;
using System.Collections.Generic;
using B3.BuySystem;
using B3.DevelopmentCardSystem;
using B3.PlayerInventorySystem;
using B3.PlayerSystem;
using B3.UI;
using NaughtyAttributes;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class BuyDevCard : MonoBehaviour
{
    public GameObject developmentCardPrefab;
    [SerializeField] private BuyController buyController;
    public PlayerBase player;

    [HorizontalLine]
    public Sprite knightSprite;     
    public Sprite roadBuildingSprite;    
    public Sprite monopolySprite;    
    public Sprite yearOfPlentySprite;    
    public Sprite victoryPointSprite;

    private PlayerInventoryController inventoryController;
    private Dictionary<DevelopmentCardType, DisplayedCard> displayedCards = new();
    private Transform buyCard; // reordered after inserting a new card type
    
    class DisplayedCard
    {
        public int Count { get; set; }
        public TMP_Text Text { get; init; }
    }
    
    public void Start()
    {
        inventoryController = player.GetComponent<PlayerInventoryController>(); 
        buyCard = transform.GetChild(0);
    }
    
    public void Buy()
    {
        var cardType = buyController.BuyDevelopmentCard(player);
        if (cardType is null) return;
        IncrementDisplayedCards(cardType.Value);
    }

    void UseCard(DevelopmentCardType cardType)
    {
        var entry = displayedCards;
        if (!inventoryController.UseItem(cardType))
        {
            NotificationManager.Instance.AddNotification("You cannot use this card until next turn ");
            return;
        }
    }

    private void IncrementDisplayedCards(DevelopmentCardType cardType)
    {
        NotificationManager.Instance.AddNotification($"You bought a <color=red>{cardType.Name()}</color> development card.");
        
        if (displayedCards.TryGetValue(cardType, out DisplayedCard entry))
        {
            entry.Count++;
            entry.Text.text = entry.Count.ToString();
        }
        else
        {
            var go = Instantiate(developmentCardPrefab, transform);
            
            // set image
            go.GetComponentInChildren<Image>().sprite = cardType switch
            {
                DevelopmentCardType.Knight => knightSprite,
                DevelopmentCardType.RoadBuilding => roadBuildingSprite,
                DevelopmentCardType.Monopoly => monopolySprite,
                DevelopmentCardType.YearOfPlenty => yearOfPlentySprite,
                DevelopmentCardType.VictoryPoint => victoryPointSprite,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            // set handler
            var button = go.GetComponentInChildren<Button>();
            button.onClick.AddListener(() => UseCard(cardType));
            
            // get text component
            var bubble = go.transform.GetChild(1);
            var textComponent = bubble.GetComponentInChildren<TMP_Text>();
            textComponent.text = "1";
            
            displayedCards.Add(cardType, new DisplayedCard
            {
                Count = 1,
                Text = textComponent
            });
            
            buyCard.SetAsLastSibling();
        }
    }
    
    [Button]
    private void AddKnightCard()
    {
        if (Application.isPlaying)
        {
            inventoryController.AddItem(DevelopmentCardType.Knight);
            IncrementDisplayedCards(DevelopmentCardType.Knight);           
        }
    }
    
    [Button]
    private void AddAllCards()
    {
        if (Application.isPlaying)
        {
            inventoryController.AddItem(DevelopmentCardType.Knight);
            IncrementDisplayedCards(DevelopmentCardType.Knight);
            inventoryController.AddItem(DevelopmentCardType.Monopoly);
            IncrementDisplayedCards(DevelopmentCardType.Monopoly);
            inventoryController.AddItem(DevelopmentCardType.RoadBuilding);
            IncrementDisplayedCards(DevelopmentCardType.RoadBuilding);
            inventoryController.AddItem(DevelopmentCardType.VictoryPoint);
            IncrementDisplayedCards(DevelopmentCardType.VictoryPoint);
            inventoryController.AddItem(DevelopmentCardType.YearOfPlenty);
            IncrementDisplayedCards(DevelopmentCardType.YearOfPlenty);
        }
    }
}
