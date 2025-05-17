using UnityEngine;
using TMPro;
using B3.PieceSystem;
using NaughtyAttributes;
using System.Linq;
using B3.BuildingSystem;
using B3.SettlementSystem;
using Random = UnityEngine.Random;

namespace B3.BoardSystem
{
    public class BoardController : MonoBehaviour
    {
        private const int GRID_WIDTH = 6;
        private const int GRID_HEIGHT = 7;
        
        [SerializeField] private BoardPiece[] pieces;
        [SerializeField] private BoardPiece[] ports;
        
        [SerializeField] private SettlementController settlementPrefab;
        
        [SerializeField] private GameObject debugTextPrefab;

        private int _currentIndex;
        public FullHexGrid<PieceController, SettlementController, Path> BoardGrid { get; private set; }

        private void Awake()
        {
            BoardGrid = new FullHexGrid<PieceController, SettlementController, Path>
                (GRID_WIDTH, GRID_HEIGHT, VertexFactory, EdgeFactory);
        }

        private Path EdgeFactory(PieceController cell, HexPosition hex, HexEdgeDir dir)
        {
            return null;
        }

        private SettlementController VertexFactory(PieceController cell, HexPosition hex, HexVertexDir dir)
        {
            var cornerPosition = BoardGrid.GetHexCorner(dir, hex);

            var settlementPosition = new Vector3(cornerPosition.x, 0, cornerPosition.y);
            var instance = Instantiate(settlementPrefab, settlementPosition, Quaternion.identity);
            
            instance.HexPosition = hex;
            instance.VertexDir = dir;
            
            return instance;
        }
        
        [Button]
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
        
        public Vector3 GetClosestEdgeMidpoint(PieceController piece, Vector3 clickPosition)
        {
            Vector3[] edgeMidpoints = piece.GetEdgeMidpoints();
            return edgeMidpoints.OrderBy(p => Vector3.Distance(p, clickPosition)).First();
        }
        
        public PieceController GetPieceAt(Vector3 position)
        {
            //Vector2 axialCoords = BoardGrid.WorldToAxial(position);
            //PieceController piece = BoardGrid.GetCellAtAxial(axialCoords);
            //return piece;
            return null;
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
                return;

            var worldPosition = BoardGrid.ToWorldPosition(position);
            var pieceController = boardPiece.Spawn(new Vector3(worldPosition.x, 0, worldPosition.y), transform);

            pieceController.HexPosition = position;
            BoardGrid[position] = pieceController;
                
            var debugText = Instantiate(
                debugTextPrefab, 
                new Vector3(worldPosition.x, 2, worldPosition.y), 
                Quaternion.identity, 
                transform);
                
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
        
        // private void Start()
        // {
        //     if (gameSettings == null)
        //     {
        //         Debug.LogError("gameSettings is not assigned!");
        //         return;
        //     }
        //
        //     if (gameSettings.autoGenerateBoard)
        //     {
        //         Generate();
        //     }
        // }
    }
}
