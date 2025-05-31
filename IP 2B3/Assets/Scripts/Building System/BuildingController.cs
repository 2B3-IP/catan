using System.Collections;
using UnityEngine;
using B3.SettlementSystem;
using B3.PlayerSystem;
using B3.BoardSystem;
using B3.DevelopmentCardSystem;
using B3.GameStateSystem;
using B3.PieceSystem;
using UnityEngine.InputSystem;
using B3.UI;

namespace B3.BuildingSystem
{
    internal sealed class BuildingController : BuildingControllerBase
    {
        [SerializeField] private InputActionReference clickButton;
        [SerializeField] private SettlementController settlementPrefab;
        [SerializeField] private LongestRoadController longestRoadController;

        private PathController[] _allPaths;
        private bool _isFirstStates = true;
        private int countFirstStates = 0;
        
        public bool HasBuilt { get; private set; }

        private void Awake()
        {
            _allPaths = FindObjectsByType<PathController>(FindObjectsSortMode.None);
    
            Debug.Log($"Found {_allPaths.Length} total paths in Awake");
    
            // Debug fiecare drum
            foreach (var path in _allPaths)
            {
                Debug.Log($"Path: Owner={path.Owner?.name ?? "NULL"}, IsBuilt={path.IsBuilt}, HexPos=({path.HexPosition.X},{path.HexPosition.Y}), EdgeDir={path.EdgeDir}");
        
                if (path.Owner != null)
                {
                    path.IsBuilt = true; // Acum merge direct
                    Debug.Log($"Set IsBuilt=true for path at {path.HexPosition.X},{path.HexPosition.Y} {path.EdgeDir}");
                }
            }
    
            // Verificăm din nou după fix
            int totalBuiltRoads = 0;
            foreach (var path in _allPaths)
            {
                if (path.IsBuilt && path.Owner != null)
                {
                    totalBuiltRoads++;
                    Debug.Log($"Built road found: {path.HexPosition.X},{path.HexPosition.Y} {path.EdgeDir} owned by {path.Owner.name}");
                }
            }
            Debug.Log($"Total built roads after fix: {totalBuiltRoads}");
        }

        private void OnEnable() =>
            PlayerDiceGameState.OnDiceGameState += OnDiceGameState;
        
        private void OnDisable() =>
            PlayerDiceGameState.OnDiceGameState -= OnDiceGameState;

        public override IEnumerator BuildHouse(PlayerBase player)
        {
            if (!_isFirstStates && !CanBuildHouse(player))
                yield break;

            if (_isFirstStates)
                countFirstStates++;

            SettlementController selectedHouse = null;
            var instructionNotif = NotificationManager.Instance
                .AddNotification("Select a vertex to build a house", float.PositiveInfinity, false);
            while (selectedHouse == null)
            {
                yield return player.BuildHouseCoroutine();
                selectedHouse = player.SelectedHouse;

                if (selectedHouse == null) 
                    continue;
                
                bool canBuild = CanBuildHouse(selectedHouse, player);
                Debug.Log($"Checking if can build house at {selectedHouse.HexPosition.X},{selectedHouse.HexPosition.Y} {selectedHouse.VertexDir}: {canBuild}");

                if (canBuild) 
                    continue;
                
                Debug.Log("Cannot build house here - resetting selection");
                HasBuilt = false;
                selectedHouse = null;
            }
            instructionNotif.Destroy();
            Debug.Log("Building house successfully!");
            
            Debug.Log($"BEFORE: Settlement at ({selectedHouse.HexPosition.X},{selectedHouse.HexPosition.Y} {selectedHouse.VertexDir}) - HasOwner: {selectedHouse.HasOwner}, Owner: {selectedHouse.Owner?.name ?? "NULL"}");

            if (_isFirstStates && countFirstStates == 4)//TREBUIE VAZUT CAND ADAUGAM RESURSELE
            {
                AddResourcesForSettlement(selectedHouse, player);
            }

            selectedHouse.Owner = player;
            selectedHouse.BuildHouse();
            player.Settlements.Add(selectedHouse);
            player.AddVictoryPoints(1);
            
            HasBuilt = true;
            
            Debug.Log($"AFTER: Settlement at ({selectedHouse.HexPosition.X},{selectedHouse.HexPosition.Y} {selectedHouse.VertexDir}) - HasOwner: {selectedHouse.HasOwner}, Owner: {selectedHouse.Owner?.name ?? "NULL"}");
            
            AddPortBuffForSettlement(selectedHouse, player);
            
            var message = $"House built at {selectedHouse.HexPosition.X} {selectedHouse.HexPosition.Y}, {selectedHouse.VertexDir} by {player.name}";
            Debug.Log(message);

            if(player is HumanPlayer)
                AI.SendMove(message);
        }


        public override IEnumerator BuildRoad(PlayerBase player)
        {
            if (!_isFirstStates && !CanBuildRoad(player))
            {
                Debug.Log("CanBuildRoad(player) returned false - exiting");
                yield break;
            }
    
            PathController selectedPath = null;
            var instructionNotif = NotificationManager.Instance
                .AddNotification("Select an edge to build a road", float.PositiveInfinity, false);
            while (selectedPath == null)
            {
                Debug.Log("Waiting for player to select a path...");
                yield return player.BuildRoadCoroutine();
                selectedPath = player.SelectedPath;
        
                if (selectedPath != null)
                {
                     bool canBuild = CanBuildRoad(player, selectedPath);
                    Debug.Log($"CanBuildRoad result: {canBuild}");
            
                    if (!canBuild)
                    {
                        Debug.Log($"Cannot build road at {selectedPath.HexPosition.X},{selectedPath.HexPosition.Y} {selectedPath.EdgeDir} - resetting selection");
                        HasBuilt = false;
                        selectedPath = null;
                    }
                    else {
                        Debug.Log($"Can build road - proceeding with construction");
                    }
                }
                else
                {
                    Debug.Log("No path selected by player");
                }
            }
            instructionNotif.Destroy();
            Debug.Log($"Building road at {selectedPath.HexPosition.X},{selectedPath.HexPosition.Y} {selectedPath.EdgeDir}");
    
            HasBuilt = true;
            selectedPath.Owner = player;
            selectedPath.BuildRoad();
    
            player.Paths.Add(selectedPath);
    
            Debug.Log($"Road built successfully! Player now has {player.Paths.Count} roads");
    
            if (longestRoadController != null)
                longestRoadController.CheckLongestRoadAfterBuild(player, selectedPath);;
        }

        protected override bool CanBuildHouse(SettlementController targetSettlement, PlayerBase player)
        {
            var housePosition = targetSettlement.HexPosition;
            var houseDir = targetSettlement.VertexDir;

            var boardGrid = boardController.BoardGrid;
            var neighbouringVertices = boardGrid.GetNeighbouringVertices(housePosition, houseDir);
            
            int index = 0;

            bool canBePlaced = _isFirstStates;
            foreach (var (settlement, pos, dir) in neighbouringVertices)
            {
                // am o casa vecina deja construita
                if (settlement.HasOwner)
                    return false;

                if (_isFirstStates)
                    continue;
                
                var edgeDir = index switch
                {
                    0 => HexEdgeDirExt.GetHexDir(houseDir, dir),
                    1 => HexEdgeDirExt.GetHexDir(dir, houseDir),
                    _ => HexEdgeDirExt.GetHexDir(dir, houseDir.GetVertexDirBasedOnStartDir(housePosition, pos))
                };

                index++;
                
                var path = boardGrid.GetEdge(pos, edgeDir);
                if (path == null)
                    continue;

                if (path.IsBuilt && path.Owner == player)
                    canBePlaced = true;
            }

            return canBePlaced;
        }
        
        protected override bool CanBuildRoad(PlayerBase player, PathController targetPath)
        {
            if (targetPath.IsBuilt)
                return false;
            
            var roadPosition = targetPath.HexPosition;
            var roadDir = targetPath.EdgeDir;

            var boardGrid = boardController.BoardGrid;
            
            var (roadVertex1, roadVertex2) = roadDir.GetVertexDirs();
            
            var vertex1 = boardGrid.GetVertex(roadPosition, roadVertex1);
            var vertex2 = boardGrid.GetVertex(roadPosition, roadVertex2);
            
            // este langa o casa a playerului
            if (vertex1.Owner == player || vertex2.Owner == player)
                return true;

            bool canBePlaced = false;
            
            CheckNeighbourVertexes(roadVertex1);
            CheckNeighbourVertexes(roadVertex2);
            
            return canBePlaced;

            void CheckNeighbourVertexes(HexVertexDir vertex)
            {
                var neighbouringVertices = boardGrid.GetNeighbouringVertices(roadPosition, vertex);

                int index = 0;
                foreach (var (settlement, pos, dir) in neighbouringVertices)
                {
                    // path ul pe care l ai construit chiar acum, deci skip
                    if (settlement == vertex2 || settlement == vertex1)
                        continue;

                    var edgeDir = index switch
                    {
                        1 => HexEdgeDirExt.GetHexDir(vertex.GetVertexDirBasedOnStartDir(roadPosition, pos), dir),
                        _ => HexEdgeDirExt.GetHexDir(dir, vertex)
                    };

                    index++;
                    
                    var path = boardGrid.GetEdge(pos, edgeDir);
                    if (path == null)
                        continue;

                    if (path.Owner == player)
                        canBePlaced = true;
                }
            }
        }
        
        

        private PieceController[] allPieces;
        private void AddPortBuffForSettlement(SettlementController settlement, PlayerBase player)
        {
            if (settlement.ConnectedPortController != null) settlement.ConnectedPortController.AddPlayerBuff(player);
        }
        
        public override IEnumerator BuildCity(PlayerBase player)
        {
            if (!CanBuildCity(player))
                yield break;

            SettlementController closestCorner = null;
            while (closestCorner == null)
            {
                Debug.Log("Building city for:" + player.name);
                yield return player.UpgradeToCityCoroutine();

                closestCorner = player.SelectedHouse;
                
                Debug.Log(closestCorner.Owner.name + " vs " + player.name);
                if (!closestCorner.HasOwner || closestCorner.Owner != player)
                {
                    closestCorner = null;
                    Debug.Log("Not your settlement");
                }
                else
                {
                    closestCorner.UpgradeToCity();
                    player.AddVictoryPoints(1);
                    HasBuilt = true;
                }
            }
        }
        
        private void OnDiceGameState() =>
            _isFirstStates = false;
        
                
         private void AddResourcesForSettlement(SettlementController selectedHouse, PlayerBase player)
        {
            var boardGrid = boardController.BoardGrid;
            var vertexPosition = selectedHouse.HexPosition;
            var vertexDirection = selectedHouse.VertexDir;
            
            var pieceController = boardGrid[vertexPosition];
            player.AddResource(pieceController.ResourceType, 1);
            
            switch (vertexDirection)
            {
                case HexVertexDir.TopRight :
                {
                    AddResourcesToPlayer(HexEdgeDir.TopRight, HexEdgeDir.Top, vertexPosition, player, boardGrid);
                    break;
                }
                case HexVertexDir.Right:
                {
                    AddResourcesToPlayer(HexEdgeDir.TopRight,HexEdgeDir.BottomRight,vertexPosition,player,boardGrid);
                    break;
                }
                case HexVertexDir.BottomRight:
                {
                    AddResourcesToPlayer(HexEdgeDir.BottomRight, HexEdgeDir.Bottom, vertexPosition, player, boardGrid);
                    break;
                }
                case HexVertexDir.BottomLeft:
                {
                    AddResourcesToPlayer(HexEdgeDir.BottomLeft, HexEdgeDir.Bottom, vertexPosition, player, boardGrid);
                    break;
                }
                case HexVertexDir.Left:
                {
                    AddResourcesToPlayer(HexEdgeDir.BottomLeft, HexEdgeDir.TopLeft, vertexPosition, player, boardGrid);
                    break;
                }
                case HexVertexDir.TopLeft:
                {
                    AddResourcesToPlayer(HexEdgeDir.TopLeft, HexEdgeDir.Top, vertexPosition, player, boardGrid);
                    break;
                }
            }
        }

        private void AddResourcesToPlayer(HexEdgeDir dir1, HexEdgeDir dir2, HexPosition vertexPosition, PlayerBase player, FullHexGrid<PieceController, SettlementController, PathController> boardGrid )
        {
            var hex1 = vertexPosition.GetNeighbour(dir1);
            var hex2 = vertexPosition.GetNeighbour(dir2);

            var pieceController1 = boardGrid[hex1];
            var pieceController2 = boardGrid[hex2];
            
            player.AddResource(pieceController1.ResourceType, 1);
            player.AddResource(pieceController2.ResourceType, 1);
        }
    }
    
}