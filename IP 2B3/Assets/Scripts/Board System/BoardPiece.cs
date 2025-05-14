using UnityEngine;
using B3.PieceSystem;
using B3.ResourcesSystem;

namespace B3.BoardSystem
{
    [System.Serializable]
    internal sealed class BoardPiece
    {
        [field:SerializeField] private int maxSpawnCount;
        [field:SerializeField] private PieceController piecePrefab;
        
        private int _currentSpawnCount;
        public bool CanSpawn => _currentSpawnCount < maxSpawnCount;
        
        public PieceController Spawn(Vector3 position, Transform parent = null)
        {
            var piece = Object.Instantiate(piecePrefab, position, Quaternion.identity, parent);
            _currentSpawnCount++;
            
            return piece;
        }
    }
}