using System.Collections;
using B3.GameStateSystem;
using B3.PieceSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;
using UnityEngine;
using B3.UI;

namespace B3.ThiefSystem
{
    public abstract class ThiefControllerBase : MonoBehaviour
    {
        private PieceController _currentPiece;
        
        public abstract IEnumerator MoveThief(Vector3 endPosition);

        public void BlockPiece(PieceController piece)
        {
            if(_currentPiece != null)
                _currentPiece.IsBlocked = false;
            
            piece.IsBlocked = true;
            _currentPiece = piece;
        }

        public void StealFromRandomPlayer(PlayerBase currentPlayer)
        {
            var settlements = _currentPiece.Settlements;
            if (settlements.Count == 0)
                return;
            
            int randomIndex = Random.Range(0, settlements.Count);
            
            var randomSettlement = settlements[randomIndex];
            StealFromPlayer(randomSettlement.Owner, currentPlayer);
        }

        private void StealFromPlayer(PlayerBase stealFrom, PlayerBase giveTo)
        {
            var resources = stealFrom.Resources;
            int index;
            bool hasAnyResource = false;
            
            foreach (var r in resources)
            {
                if (r > 0)
                {
                    hasAnyResource = true;
                    break;
                }
            }

            if (!hasAnyResource)
                return;
            
            do
            {
                index = Random.Range(0, resources.Length);
            } 
            while (resources[index] == 0);
            
            var resource = (ResourceType)index;
            NotificationManager.Instance.AddNotification($"{giveTo.colorTag}{giveTo.playerName}</color> stole 1 {resource} from {stealFrom.colorTag}{stealFrom.playerName}</color>.");
            stealFrom.RemoveResource(resource, 1);
            giveTo.AddResource(resource, 1);
        }
    }
}