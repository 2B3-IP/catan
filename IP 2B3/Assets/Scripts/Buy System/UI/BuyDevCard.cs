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
        public TMP_Text CountText { get; init; }
        public GameObject Go { get; init; }
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
        var entry = displayedCards[cardType];
        if (!inventoryController.UseItem(cardType))
        {
            NotificationManager.Instance.AddNotification("You cannot use this card until next turn ");
            return;
        }

        entry.CountText.text = (--entry.Count).ToString();
        
        if (entry.Count == 0)
        {
            entry.Go.SetActive(false);
            Destroy(entry.Go);
            displayedCards.Remove(cardType);
        }
    }

    private void IncrementDisplayedCards(DevelopmentCardType cardType)
    {
        NotificationManager.Instance.AddNotification($"You bought a <color=red>{cardType.Name()}</color> development card.");
        
        if (displayedCards.TryGetValue(cardType, out DisplayedCard entry))
        {
            entry.Count++;
            entry.CountText.text = entry.Count.ToString();
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
            
            // set title and description
            var hover = go.transform.GetChild(2);
            hover.GetChild(0).GetComponent<TMP_Text>().text = cardType.Name();
            hover.GetChild(1).GetComponent<TMP_Text>().text = cardType.Description();
            
            // get count text component
            var bubble = go.transform.GetChild(1);
            var countText = bubble.GetComponentInChildren<TMP_Text>();
            countText.text = "1";
            
            displayedCards.Add(cardType, new DisplayedCard
            {
                Count = 1,
                Go = go,
                CountText = countText
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
