using System;
using B3.PlayerSystem;
using Mono.Cecil;
using TMPro;
using UnityEngine;
using ResourceType = B3.ResourcesSystem.ResourceType;

namespace B3.BankSystem.UI
{
    public class UpdateBankResources : MonoBehaviour
    {
        public BankController bankController;
        public TMP_Text woodResourcesText;
        public TMP_Text brickResourcesText;
        public TMP_Text wheatResourcesText;
        public TMP_Text sheepResourcesText;
        public TMP_Text oreResourcesText;
        public TMP_Text developmentCardsCountText;

        private void FixedUpdate()
        {
            woodResourcesText.text = bankController.CurrentResources[(int) ResourceType.Wood].ToString();
            brickResourcesText.text = bankController.CurrentResources[(int) ResourceType.Brick].ToString();
            wheatResourcesText.text = bankController.CurrentResources[(int) ResourceType.Wheat].ToString();
            sheepResourcesText.text = bankController.CurrentResources[(int) ResourceType.Sheep].ToString();
            oreResourcesText.text = bankController.CurrentResources[(int) ResourceType.Ore].ToString();
            developmentCardsCountText.text = bankController.DevCardsCount.ToString();
        }
        
    }   
}