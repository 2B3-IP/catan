using B3.PlayerInventorySystem;
using B3.PlayerSystem;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UpdateAIPlayersResources : MonoBehaviour
{
    [SerializeField] PlayerBase chatGPT;
    public PlayerInventoryController playerInventoryController;
    public TMP_Text victoryPointsText;
    public TMP_Text longestRoadText;
    public TMP_Text largestArmyText;
    public TMP_Text resourcesCountText;
    public TMP_Text developmentCardsCountText;

    void FixedUpdate()
    {
        if (chatGPT.gameObject.IsDestroyed())
            Destroy(gameObject);
        
        victoryPointsText.text = chatGPT.VictoryPoints.ToString();
        
        int sum = 0;
        for (int i = 0; i < 5; i++)
        {
            sum += chatGPT.Resources[i];
        }
        resourcesCountText.text = sum.ToString();
        developmentCardsCountText.text = playerInventoryController.ItemCount.ToString();    
    }
}
