using UnityEngine;
using TMPro;
using B3.PieceSystem;
using NaughtyAttributes;
using System.Linq;
using B3.BuildingSystem;
using B3.SettlementSystem;
using Random = UnityEngine.Random;
using B3.ResourcesSystem;
using B3.PortSystem;

namespace B3.BoardSystem
{
    public class BoardController : MonoBehaviour
    {
        private const int GRID_WIDTH = 6;
        private const int GRID_HEIGHT = 7;
        
        [SerializeField] private BoardPiece[] pieces;
        [SerializeField] private BoardPiece[] ports;
        
        [SerializeField] private SettlementController settlementPrefab;
        
        [SerializeField] private GameObject pieceTextPrefab;

        private ResourceType?[] _piecesResources = new ResourceType?[19];
        private int[] _piecesNumber = new int[19];
        private ResourceType?[] _portsResources = new ResourceType?[9];

        private readonly int[] _numberPoll = new int[]
        {
            2, 3, 3, 4, 4, 5, 5, 6, 6,
            8, 8, 9, 9, 10, 10, 11, 11, 12
        };

        private int _currentPieceIndex, _currentPortIndex, _currentNumberIndex;

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
            ShuffleNumberPoll();

            _currentPieceIndex = 0;
            _currentPortIndex = 0;
            _currentNumberIndex = 0;

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

            AI.PassPiecesResources(_piecesResources);
            AI.PassPiecesNumber(_piecesNumber);
            AI.PassPortsResources(_portsResources);
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

            if (!arePortPieces)
            {
                bool isDesertPiece = pieceController.IsDesert;

                int number = isDesertPiece ? -1 : _numberPoll[_currentNumberIndex++];
                pieceController.Number = number;

                _piecesNumber[_currentPieceIndex] = number;
                _piecesResources[_currentPieceIndex++] = isDesertPiece ? null : pieceController.ResourceType;
            }
            else
            {
                var portController = pieceController.GetComponent<PortController>();
                _portsResources[_currentPortIndex++] = portController.ResourceType;
            }

            var debugText = Instantiate
            (
                pieceTextPrefab, 
                new Vector3(worldPosition.x, 0.2f, worldPosition.y), 
                Quaternion.identity, 
                transform
            );
                
            debugText.GetComponentInChildren<TMP_Text>()
                .SetText(i + " " + j + " number: " + pieceController.Number);
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

        private void ShuffleNumberPoll()
        {
            for (int i = _numberPoll.Length - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (_numberPoll[i], _numberPoll[j]) = (_numberPoll[j], _numberPoll[i]);
            }
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
