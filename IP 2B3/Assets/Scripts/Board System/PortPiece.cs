using B3.PortSystem;
using UnityEngine;

namespace B3.BoardSystem
{
    [System.Serializable]
    internal sealed class PortPiece
    {
        [field:SerializeField] private int maxSpawnCount;
        [field:SerializeField] private PortController piecePrefab;
        
        private int _currentSpawnCount;
        
        public bool CanSpawn => _currentSpawnCount < maxSpawnCount;
        
        public PortController Spawn(Vector3 spawnPosition, Vector3 endPosition)
        {
            var piece = Object.Instantiate(piecePrefab, spawnPosition, Quaternion.identity);
            piece.OnSpawn(endPosition);
            
            _currentSpawnCount++;
            return piece;
        }
    }
}