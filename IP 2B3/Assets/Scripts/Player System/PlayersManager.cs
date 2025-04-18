using System.Collections.Generic;
using B3.DevelopmentCardSystem;
using B3.PlayerInventorySystem;
using UnityEngine;

namespace B3.PlayerSystem
{
    public class PlayersManager : MonoBehaviour
    {
        [SerializeField] private HumanPlayer humanPlayerPrefab;
        [SerializeField] private AIPlayer aiPlayerPrefab;
        
        [SerializeField] private DevelopmentCardController developmentCardController;

        public List<PlayerBase> ActivePlayers { get; protected set; } = new();

        public void SpawnPlayer(bool isHuman = true)
        {
            PlayerBase playerPrefab = isHuman ? humanPlayerPrefab : aiPlayerPrefab;
            var player = Instantiate(playerPrefab);
            
            var inventoryController = player.GetComponent<PlayerInventoryController>();
            inventoryController.Initialize(developmentCardController);
            
            ActivePlayers.Add(player);
        }
    }
}