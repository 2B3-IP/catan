using System.Collections.Generic;
using B3.DevelopmentCardSystem;
using B3.PlayerInventorySystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace B3.PlayerSystem
{
    public class PlayersManager : MonoBehaviour
    {
        public List<PlayerBase> players;

        public HumanPlayer humanPlayer => (HumanPlayer) players[0];
        
        public void Initialize(int numberOfPlayers)
        {
            if (numberOfPlayers < 1 || numberOfPlayers > 4)
            {
                Debug.LogError("Number of players must be between 1 and 4");
                numberOfPlayers = 4;
            }

            for (int i = 3; i >= numberOfPlayers; i++)
            {
                Destroy(players[i].gameObject);
                players.RemoveAt(i);
            }
        }
    }
}