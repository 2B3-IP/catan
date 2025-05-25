using B3.PlayerSystem;
using TMPro;
using UnityEngine;
using ResourceType = B3.ResourcesSystem.ResourceType;

public class UpdateHumanPlayerResources : MonoBehaviour
{
    [SerializeField] PlayerBase humanPlayer;
    public TMP_Text victoryPointsText;
    public TMP_Text longestRoadText;
    public TMP_Text largestArmyText; // todo: make the card shiny or something if you have the largest army
    public TMP_Text woodResourcesText;
    public TMP_Text brickResourcesText;
    public TMP_Text wheatResourcesText;
    public TMP_Text sheepResourcesText;
    public TMP_Text oreResourcesText;

    void Start()
    {
        humanPlayer.onResourcesChanged.AddListener(UpdateResources);    
        humanPlayer.onVPChanged.AddListener(() => victoryPointsText.text = humanPlayer.VictoryPoints.ToString());
        humanPlayer.onUsedKnightsChanged.AddListener(
            () => largestArmyText.text = humanPlayer.UsedKnightCards.ToString());
        
        UpdateResources();
        victoryPointsText.text = 0.ToString();
        largestArmyText.text = 0.ToString();
        // longestRoadText.text = 0.ToString();
    }

    void UpdateResources()
    {
        woodResourcesText.text = humanPlayer.Resources[(int)  ResourceType.Wood].ToString();
        brickResourcesText.text = humanPlayer.Resources[(int) ResourceType.Brick].ToString();
        wheatResourcesText.text = humanPlayer.Resources[(int) ResourceType.Wheat].ToString();
        sheepResourcesText.text = humanPlayer.Resources[(int) ResourceType.Sheep].ToString();
        oreResourcesText.text = humanPlayer.Resources[(int) ResourceType.Ore].ToString();
    }
    
}
