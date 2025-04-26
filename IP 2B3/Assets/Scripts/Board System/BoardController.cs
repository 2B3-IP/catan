using System.Collections;
using B3.BankSystem;
using UnityEngine;
using B3.PieceSystem;
using B3.PortSystem;

namespace B3.BoardSystem
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private float pieceSpawnDelay = 0.5f;
        
        [SerializeField] private BoardLine[] lines;
        [SerializeField] private BoardPiece[] pieces;
        [SerializeField] private PortPiece[] ports;
        [SerializeField] private BoardLine[] portLines;
        
        [SerializeField] private BankController bankController;
        
        private int _currentIndex;
        
        public PieceController[] _pieceControllers { get; } = new PieceController[19];
        public PortController[] _portControllers { get; } = new PortController[9];
        private void Start() =>
            Generate();
        
        public void GiveResources(int pieceNumber)
        {
            foreach (var piece in _pieceControllers)
            {
                if (piece.Number != pieceNumber)
                    continue;

                foreach (var settlement in piece.Settlements)
                {
                    var resourceType = piece.ResourceType;
                    
                    var player = settlement.Owner;
                    int resourceAmount = settlement.ResourceAmount;
                    
                    bankController.GetResources(resourceType, resourceAmount);
                    player.AddResource(resourceType, resourceAmount);
                }
            }
        }
        
        private void Generate() =>
            StartCoroutine(GenerateCoroutine());

        private IEnumerator GenerateCoroutine()
        {
            var wait = new WaitForSeconds(pieceSpawnDelay);
            
            foreach (var line in lines)
            {
                var spawnPosition = line.SpawnPosition.position;
                
                foreach (var endPosition in line.EndPositions)
                {
                    var piece = GetRandomPiece();
                    var instance = piece.Spawn(spawnPosition, endPosition.position);
                    
                    _pieceControllers[_currentIndex++] = instance;
                    yield return wait;
                }
            }

            foreach (var portLine in portLines)
            {
                var spawnPosition = portLine.SpawnPosition.position;

                foreach (var endPosition in portLine.EndPositions)
                {
                    var piece = GetRandomPortPiece();
                    var instance = piece.Spawn(spawnPosition, endPosition.position);
                    
                    _portControllers[_currentIndex++] = instance;
                    yield return wait;
                }
            }

        }
        
        private BoardPiece GetRandomPiece()
        {
            BoardPiece piece;
            
            do
            {
                int index = Random.Range(0, pieces.Length);
                piece = pieces[index];
                
            } while (!piece.CanSpawn);
            
            return piece;
        }

        private PortPiece GetRandomPortPiece()
        {
            PortPiece piece;
            do
            {
                int index = Random.Range(0, _portControllers.Length);
                piece=ports[index];
            } while (!piece.CanSpawn);
            
            return piece;
        }
    }
}
