using System;
using B3.PlayerInventorySystem;
using B3.PlayerSystem;
using Mono.Cecil;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
public class UpdateAIPlayersResources : MonoBehaviour
{
    public PlayerBase chatGPT;
    public PlayerInventoryController playerInventoryController;
    public TMP_Text victoryPointsText;
    public TMP_Text longestRoadText;
    public TMP_Text largestArmyText;
    public TMP_Text resourcesCountText;
    public TMP_Text developmentCardsCountText;


    private void Start()
    {
        chatGPT.onResourcesChanged.AddListener(UpdateResources);
        chatGPT.onUsedKnightsChanged.AddListener(() => largestArmyText.text = chatGPT.UsedKnightCards.ToString());
        chatGPT.onVPChanged.AddListener(() => victoryPointsText.text =  chatGPT.VictoryPoints.ToString());
        playerInventoryController.onItemCountChanged.AddListener(() => 
            developmentCardsCountText.text = playerInventoryController.ItemCount.ToString());
        chatGPT.onLongestRoadChanged.AddListener(() => longestRoadText.text = chatGPT.LongestRoad.ToString());
        UpdateResources();
        victoryPointsText.text = 0.ToString();
        largestArmyText.text = 0.ToString();
        longestRoadText.text = 0.ToString();
    }

    void UpdateResources() => 
        resourcesCountText.text = chatGPT.TotalResources().ToString();
}
