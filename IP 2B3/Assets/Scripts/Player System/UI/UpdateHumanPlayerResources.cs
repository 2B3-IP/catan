using B3.PlayerSystem;
using TMPro;
using UnityEngine;

public class UpdateHumanPlayerResources : MonoBehaviour
{
    [SerializeField] PlayerBase humanPlayer;
    public TMP_Text victoryPointsText;
    public TMP_Text longestRoadText;
    public TMP_Text largestArmyText;
    public TMP_Text woodResourcesText;
    public TMP_Text brickResourcesText;
    public TMP_Text wheatResourcesText;
    public TMP_Text sheepResourcesText;
    public TMP_Text oreResourcesText;

    void FixedUpdate()
    {
        victoryPointsText.text = humanPlayer.VictoryPoints.ToString();
        woodResourcesText.text = humanPlayer.Resources[2].ToString();
        brickResourcesText.text = humanPlayer.Resources[3].ToString();
        wheatResourcesText.text = humanPlayer.Resources[1].ToString();
        sheepResourcesText.text = humanPlayer.Resources[0].ToString();
        oreResourcesText.text = humanPlayer.Resources[4].ToString();
    }
    
}
