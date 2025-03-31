using System.Collections;
using B3.GameStateSystem;
using UnityEngine;

namespace B3.BoardSystem
{
    public class BoardController : MonoBehaviour
    {
        [SerializeField] private float pieceSpawnDelay = 0.5f;
        
        [SerializeField] private BoardLine[] lines;
        [SerializeField] private BoardPiece[] pieces;

        private void Start() =>
            Generate();            
        
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
                    piece.Spawn(spawnPosition, endPosition.position);

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
    }
}
