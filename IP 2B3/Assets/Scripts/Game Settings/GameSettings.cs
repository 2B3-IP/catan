using UnityEngine;

namespace Game_Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Set/Game Settings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [Range(1, 4)]
        public int numberOfPlayers = 4;

        public bool autoGenerateBoard = true;
    }
}