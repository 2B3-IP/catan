using UnityEngine;
using TMPro;
using B3.PieceSystem;

namespace B3.BoardSystem
{
    public class BoardController : MonoBehaviour
    {
        private const int GRID_WIDTH = 6;
        private const int GRID_HEIGHT = 7;
        
        [SerializeField] private BoardPiece[] pieces;
        [SerializeField] private BoardPiece[] ports;
        
        [SerializeField] private GameObject debugTextPrefab;
        
        private readonly HexGrid<PieceController> _boardGrid = new(GRID_WIDTH, GRID_HEIGHT);
        
        private int _currentIndex;
        
        [ContextMenu("Generate")]
        public void Generate()
        {
            SpawnLine(0, 2, -2, false);
            SpawnLine(-1, 2, -1, false);
            SpawnLine(-2, 2, 0, false);
            SpawnLine(-2, 1, 1, false);
            SpawnLine(-2,0, 2, false);
            
            SpawnPiece(0, -3, true);
            SpawnPiece(2, -3, true);
            SpawnPiece(3, -2, true);
            SpawnPiece(3, 0, true);
            SpawnPiece(1, 2, true);
            SpawnPiece(-1, 3, true);
            SpawnPiece(-3, 3, true);
            SpawnPiece(-3, 1, true);
            SpawnPiece(-2, -1, true);
        }

        private void SpawnLine(int iMin, int iMax, int j, bool arePortPieces)
        {
            for (int i = iMin; i <= iMax; i++)
                SpawnPiece(i, j, arePortPieces);
        }

        private void SpawnPiece(int i, int j, bool arePortPieces)
        {
            var position = new HexPosition(i, j);
                
            var boardPiece = GetRandomPiece(arePortPieces);
            if (boardPiece == null)
            {
                Debug.Log(i + " " + j);
                return;
            }

            var worldPosition = _boardGrid.ToWorldPosition(position);
            var pieceController = boardPiece.Spawn(new Vector3(worldPosition.x, 0, worldPosition.y));
            
            _boardGrid[position] = pieceController;
                
            var debugText = Instantiate(debugTextPrefab, 
                new Vector3(worldPosition.x, 2, worldPosition.y), Quaternion.identity);
                
            debugText.GetComponentInChildren<TMP_Text>()
                .SetText(i + " " + j);
        }
        
        private BoardPiece GetRandomPiece(bool isPortPiece)
        {
            BoardPiece piece;
            
            do
            {
                int length = isPortPiece ? ports.Length : pieces.Length;
                int index = Random.Range(0, length);
                
                piece = isPortPiece ? ports[index] : pieces[index];
                
            } while (!piece.CanSpawn);
            
            return piece;
        }
    }
}
