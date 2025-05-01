using B3.PlayerSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UIPlayerInfo
{
    public class PlayerInfo : MonoBehaviour
    {
        [SerializeField] HumanPlayer humanPlayer;
        [SerializeField] TextMeshProUGUI victoryPointsText;
        [SerializeField] TextMeshProUGUI longestRoadText;
        [SerializeField] TextMeshProUGUI largestArmyText;
        [SerializeField] TextMeshProUGUI woodText;
        [SerializeField] TextMeshProUGUI oreText;
        [SerializeField] TextMeshProUGUI sheepText;
        [SerializeField] TextMeshProUGUI brickText;
        [SerializeField] TextMeshProUGUI wheatText;

        void Start()
        {
            humanPlayer = FindObjectOfType<HumanPlayer>();
            if (humanPlayer == null)
            {
                return;
            }

            victoryPointsText.text = humanPlayer.VictoryPoints.ToString();
            woodText.text = humanPlayer.Resources[0].ToString();
            brickText.text = humanPlayer.Resources[1].ToString();
            wheatText.text = humanPlayer.Resources[2].ToString();
            sheepText.text = humanPlayer.Resources[3].ToString();
            oreText.text = humanPlayer.Resources[4].ToString();
        }

        void Update()
        {
            
        }
    }
}