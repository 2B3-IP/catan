using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using B3.SettlementSystem;
using B3.PlayerSystem;
using B3.BoardSystem;
using B3.DevelopmentCardSystem;
using B3.GameStateSystem;
using B3.PieceSystem;
using B3.PlayerBuffSystem;
using B3.PortSystem;
using UnityEngine.InputSystem;
using System;
using NUnit.Framework;

namespace B3.BuildingSystem
{
    internal sealed class BuildingController : BuildingControllerBase
    {
        [SerializeField] private InputActionReference clickButton;
        [SerializeField] private SettlementController settlementPrefab;
        [SerializeField] private BoardController boardController;
        [SerializeField] private LongestRoadController longestRoadController;

        private PathController[] _allPaths;
        private bool _isFirstStates = true;
        
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
            if (!CanBuildHouse(player))
                yield break;

            SettlementController selectedHouse = null;

            while (selectedHouse == null)
            {
                yield return player.BuildHouseCoroutine();
                selectedHouse = player.SelectedHouse;

                if (!_isFirstStates && selectedHouse != null)
                {
                    bool canBuild = CanBuildHouse(selectedHouse, player);
                    Debug.Log($"Checking if can build house at {selectedHouse.HexPosition.X},{selectedHouse.HexPosition.Y} {selectedHouse.VertexDir}: {canBuild}");
    
                    if (!canBuild)
                    {
                        Debug.Log("Cannot build house here - resetting selection");
                        HasBuilt = false;
                        selectedHouse = null;
                    }
                }
            }

            Debug.Log("Building house successfully!");
            
            Debug.Log($"BEFORE: Settlement at ({selectedHouse.HexPosition.X},{selectedHouse.HexPosition.Y} {selectedHouse.VertexDir}) - HasOwner: {selectedHouse.HasOwner}, Owner: {selectedHouse.Owner?.name ?? "NULL"}");
    
            selectedHouse.Owner = player;
            selectedHouse.BuildHouse();
            player.Settlements.Add(selectedHouse);
            
            HasBuilt = true;
            
            Debug.Log($"AFTER: Settlement at ({selectedHouse.HexPosition.X},{selectedHouse.HexPosition.Y} {selectedHouse.VertexDir}) - HasOwner: {selectedHouse.HasOwner}, Owner: {selectedHouse.Owner?.name ?? "NULL"}");
            
            AddPortBuffForSettlement(selectedHouse, player);
            
            var message = $"BUILD House {selectedHouse.HexPosition.X} {selectedHouse.HexPosition.Y} {(int)selectedHouse.VertexDir} by {player.name}";

            if(player is HumanPlayer)
                AI.SendMove(message);
        }


        public override IEnumerator BuildRoad(PlayerBase player)
        {
            Debug.Log($"=== BUILD ROAD STARTED for {player.name} ===");
    
            if (!CanBuildRoad(player))
            {
                Debug.Log("CanBuildRoad(player) returned false - exiting");
                yield break;
            }
    
            PathController selectedPath = null;
    
            while (selectedPath == null)
            {
                Debug.Log("Waiting for player to select a path...");
                yield return player.BuildRoadCoroutine();
                selectedPath = player.SelectedPath;
        
                if (selectedPath != null)
                {
                    Debug.Log($"=== PLAYER SELECTED PATH ===");
                    Debug.Log($"Selected path: {selectedPath.HexPosition.X},{selectedPath.HexPosition.Y} {selectedPath.EdgeDir}");
                    Debug.Log($"Path state: IsBuilt={selectedPath.IsBuilt}, Owner={selectedPath.Owner?.name ?? "NULL"}");
            
                    // Debug detailat pentru validare
                    bool canBuild = CanBuildRoad(player, selectedPath);
                    Debug.Log($"CanBuildRoad result: {canBuild}");
            
                    if (!canBuild)
                    {
                        Debug.Log($"❌ Cannot build road at {selectedPath.HexPosition.X},{selectedPath.HexPosition.Y} {selectedPath.EdgeDir} - resetting selection");
                        HasBuilt = false;
                        selectedPath = null;
                    }
                    else
                    {
                        Debug.Log($"✅ Can build road - proceeding with construction");
                    }
                }
                else
                {
                    Debug.Log("No path selected by player");
                }
            }

            var message = $"BUILD road {selectedPath.HexPosition.X} {selectedPath.HexPosition.Y} {(int)selectedPath.EdgeDir} by {player.name}";

            if(player is HumanPlayer)
                AI.SendMove(message);

    
            HasBuilt = true;
            selectedPath.Owner = player;
            selectedPath.BuildRoad();
    
            player.Paths.Add(selectedPath);
    
            Debug.Log($"Road built successfully! Player now has {player.Paths.Count} roads");
    
            if (longestRoadController != null)
            {
                longestRoadController.CheckLongestRoadAfterBuild(player);
            }
        }

        protected override bool CanBuildHouse(SettlementController targetSettlement, PlayerBase player)
        {
            Debug.Log($"CanBuildHouse called for {targetSettlement.HexPosition.X},{targetSettlement.HexPosition.Y} {targetSettlement.VertexDir}");
            
            if (targetSettlement.HasOwner)
            {
                Debug.Log("Settlement is already occupied");
                return false;
            }
            
            if (_isFirstStates)
            {
                Debug.Log("First states - only checking distance rule");
                bool distanceOk = CheckSettlementDistanceRule(targetSettlement);
                Debug.Log($"Distance rule result: {distanceOk}");
                return distanceOk;
            }

            Debug.Log("Checking road connectivity...");
            bool isConnectedToPlayerRoad = IsSettlementConnectedToPlayerRoad(targetSettlement, player);
            Debug.Log($"Connected to player road: {isConnectedToPlayerRoad}");
    
            if (!isConnectedToPlayerRoad)
                return false;
            
            bool distanceOk2 = CheckSettlementDistanceRule(targetSettlement);
            Debug.Log($"Distance rule result: {distanceOk2}");
            return distanceOk2;
        }

        private bool IsSettlementConnectedToPlayerRoad(SettlementController targetSettlement, PlayerBase player)
        {
            HexPosition hexPosition = targetSettlement.HexPosition;
            HexVertexDir vertexDir = targetSettlement.VertexDir;

            Debug.Log($"Looking for roads connected to settlement at {hexPosition.X},{hexPosition.Y} {vertexDir}");
            Debug.Log($"Player has {player.Paths.Count} paths total");

            int builtRoads = 0;
            foreach (var path in player.Paths)
            {
                if (path != null)
                {
                    builtRoads++;
                    Debug.Log($"Checking PLAYER road at {path.HexPosition.X},{path.HexPosition.Y} {path.EdgeDir}");
                    
                    var roadEndpoints = GetExactEdgeEndpoints(path.HexPosition, path.EdgeDir);
    
                    Debug.Log($"Road endpoints:");
                    foreach (var endpoint in roadEndpoints)
                    {
                        Debug.Log($"  - {endpoint.Item1.X},{endpoint.Item1.Y} {endpoint.Item2}");
                    }
                    
                    foreach (var (roadVertexPos, roadVertexDir) in roadEndpoints)
                    {
                        if (roadVertexPos.X == hexPosition.X && 
                            roadVertexPos.Y == hexPosition.Y && 
                            roadVertexDir == vertexDir)
                        {
                            Debug.Log($"MATCH FOUND! Road connects to settlement");
                            return true;
                        }
                    }
                }
            }

            Debug.Log($"Player has {builtRoads} roads in their list, but none connect to the target settlement");
            return false;
        }
        private bool CheckSettlementDistanceRule(SettlementController targetSettlement)
        {
            HexPosition hexPosition = targetSettlement.HexPosition;
            HexVertexDir vertexDir = targetSettlement.VertexDir;

            Debug.Log($"=== DISTANCE CHECK ===");
            Debug.Log($"Checking settlement at {hexPosition.X},{hexPosition.Y} {vertexDir}");

            var allSettlements = FindObjectsOfType<SettlementController>();

            foreach (var existingSettlement in allSettlements)
            {
                if (existingSettlement.HasOwner && existingSettlement != targetSettlement)
                {
                    Debug.Log($"\nChecking against existing settlement: ({existingSettlement.HexPosition.X},{existingSettlement.HexPosition.Y} {existingSettlement.VertexDir}) owned by {existingSettlement.Owner.name}");
                    
                    if (hexPosition.X == existingSettlement.HexPosition.X && 
                        hexPosition.Y == existingSettlement.HexPosition.Y)
                    {
                        Debug.Log("Same hex - checking vertex distance...");
                        
                        int vertex1 = (int)vertexDir;
                        int vertex2 = (int)existingSettlement.VertexDir;
                        int diff = Math.Abs(vertex1 - vertex2);
                        
                        bool areConsecutive = (diff == 1) || (diff == 5);
                        
                        Debug.Log($"Vertex {vertex1} vs {vertex2}, diff={diff}, consecutive={areConsecutive}");
                        
                        if (areConsecutive)
                        {
                            Debug.Log($"❌ BLOCKED: Consecutive vertices on same hex!");
                            return false;
                        }
                    }
                    else
                    {
                        Debug.Log("Different hex - checking adjacent vertices...");
                        
                        bool areAdjacent = AreVerticesAdjacent(hexPosition, vertexDir, existingSettlement.HexPosition, existingSettlement.VertexDir);
                        
                        if (areAdjacent)
                        {
                            Debug.Log($"❌ BLOCKED: Adjacent vertices across hexes!");
                            return false;
                        }
                    }
                }
            }

            Debug.Log("✅ DISTANCE OK: No blocking settlements found");
            return true;
        }
        private bool AreVerticesAdjacent(HexPosition pos1, HexVertexDir dir1, HexPosition pos2, HexVertexDir dir2)
        {
            Debug.Log($"Checking if ({pos1.X},{pos1.Y} {dir1}) and ({pos2.X},{pos2.Y} {dir2}) are adjacent");
            
            int deltaX = pos2.X - pos1.X;
            int deltaY = pos2.Y - pos1.Y;
            
            Debug.Log($"Hex delta: ({deltaX},{deltaY})");
            
            bool areNeighboringHexes = (Math.Abs(deltaX) <= 1 && Math.Abs(deltaY) <= 1 && !(deltaX == 0 && deltaY == 0));
            
            if (!areNeighboringHexes)
            {
                Debug.Log("Hexes are not neighbors - vertices cannot be adjacent");
                return false;
            }
            
            return CheckSpecificAdjacentVertices(pos1, dir1, pos2, dir2);
        }

        private bool CheckSpecificAdjacentVertices(HexPosition pos1, HexVertexDir dir1, HexPosition pos2, HexVertexDir dir2)
        {
            int deltaX = pos2.X - pos1.X;
            int deltaY = pos2.Y - pos1.Y;
            
            Debug.Log($"Checking specific adjacency for delta ({deltaX},{deltaY}) with vertices {dir1} and {dir2}");
            
            // Vecinul din dreapta (deltaX=1, deltaY=0)
            if (deltaX == 1 && deltaY == 0)
            {
                bool adjacent = (dir1 == HexVertexDir.TopRight && dir2 == HexVertexDir.TopLeft) ||
                               (dir1 == HexVertexDir.Right && dir2 == HexVertexDir.Left) ||
                               (dir1 == HexVertexDir.BottomRight && dir2 == HexVertexDir.BottomLeft);
                
                Debug.Log($"Right neighbor check: {adjacent}");
                return adjacent;
            }
            
            // Vecinul din stânga (deltaX=-1, deltaY=0)
            if (deltaX == -1 && deltaY == 0)
            {
                bool adjacent = (dir1 == HexVertexDir.TopLeft && dir2 == HexVertexDir.TopRight) ||
                               (dir1 == HexVertexDir.Left && dir2 == HexVertexDir.Right) ||
                               (dir1 == HexVertexDir.BottomLeft && dir2 == HexVertexDir.BottomRight);
                
                Debug.Log($"Left neighbor check: {adjacent}");
                return adjacent;
            }
            
            // Vecinul de sus-dreapta (deltaX=1, deltaY=1)
            if (deltaX == 1 && deltaY == 1)
            {
                bool adjacent = (dir1 == HexVertexDir.TopRight && dir2 == HexVertexDir.Left) ||
                               (dir1 == HexVertexDir.Right && dir2 == HexVertexDir.TopLeft);
                
                Debug.Log($"Top-right neighbor check: {adjacent}");
                return adjacent;
            }
            
            // Vecinul de jos-stânga (deltaX=-1, deltaY=-1)
            if (deltaX == -1 && deltaY == -1)
            {
                bool adjacent = (dir1 == HexVertexDir.Left && dir2 == HexVertexDir.TopRight) ||
                               (dir1 == HexVertexDir.TopLeft && dir2 == HexVertexDir.Right);
                
                Debug.Log($"Bottom-left neighbor check: {adjacent}");
                return adjacent;
            }
            
            // Vecinul de sus (deltaX=0, deltaY=1)
            if (deltaX == 0 && deltaY == 1)
            {
                bool adjacent = (dir1 == HexVertexDir.TopLeft && dir2 == HexVertexDir.BottomLeft) ||
                               (dir1 == HexVertexDir.TopRight && dir2 == HexVertexDir.BottomRight);
                
                Debug.Log($"Top neighbor check: {adjacent}");
                return adjacent;
            }
            
            // Vecinul de jos (deltaX=0, deltaY=-1)
            if (deltaX == 0 && deltaY == -1)
            {
                bool adjacent = (dir1 == HexVertexDir.BottomLeft && dir2 == HexVertexDir.TopLeft) ||
                               (dir1 == HexVertexDir.BottomRight && dir2 == HexVertexDir.TopRight);
                
                Debug.Log($"Bottom neighbor check: {adjacent}");
                return adjacent;
            }
            
            // Vecinul de sus-stânga (deltaX=-1, deltaY=1) 
            if (deltaX == -1 && deltaY == 1)
            {
                bool adjacent = (dir1 == HexVertexDir.TopLeft && dir2 == HexVertexDir.Right) ||
                               (dir1 == HexVertexDir.Left && dir2 == HexVertexDir.BottomRight);
                
                Debug.Log($"Top-left neighbor check: {adjacent}");
                return adjacent;
            }
            
            // Vecinul de jos-dreapta (deltaX=1, deltaY=-1)
            if (deltaX == 1 && deltaY == -1)
            {
                bool adjacent = (dir1 == HexVertexDir.Right && dir2 == HexVertexDir.TopLeft) ||
                               (dir1 == HexVertexDir.BottomRight && dir2 == HexVertexDir.Left);
                
                Debug.Log($"Bottom-right neighbor check: {adjacent}");
                return adjacent;
            }
            
            Debug.Log($"No adjacency rule found for delta ({deltaX},{deltaY})");
            return false;
        }


        
        protected override bool CanBuildRoad(PlayerBase player, PathController targetPath)
        {
            Debug.Log($"\n=== CAN BUILD ROAD CHECK ===");
            Debug.Log($"Player: {player.name}");
            Debug.Log($"Target path: {targetPath?.HexPosition.X},{targetPath?.HexPosition.Y} {targetPath?.EdgeDir}");
            
            if (targetPath == null)
            {
                Debug.Log("❌ CanBuildRoad: targetPath is null");
                return false;
            }
            
            if (!base.CanBuildRoad(player))
            {
                Debug.Log("❌ CanBuildRoad: base.CanBuildRoad failed");
                return false;
            }
            
            if (targetPath.IsBuilt)
            {
                Debug.Log($"❌ CanBuildRoad: Path already built. IsBuilt={targetPath.IsBuilt}");
                return false;
            }
            
            if (targetPath.Owner != null)
            {
                Debug.Log($"❌ CanBuildRoad: Path already owned by {targetPath.Owner.name}");
                return false;
            }
            
            SyncPlayerSettlements(player);
            
            Debug.Log($"\n--- PLAYER ROADS DEBUG ---");
            Debug.Log($"Player {player.name} has {player.Paths.Count} roads in Paths list:");
            
            for (int i = 0; i < player.Paths.Count; i++)
            {
                var road = player.Paths[i];
                if (road != null)
                {
                    Debug.Log($"  [{i}] Road: {road.HexPosition.X},{road.HexPosition.Y} {road.EdgeDir} (IsBuilt: {road.IsBuilt}, Owner: {road.Owner?.name ?? "NULL"})");
                }
                else
                {
                    Debug.Log($"  [{i}] Road: NULL");
                }
            }
            Debug.Log($"\nChecking _allPaths for player roads:");
            int foundInAllPaths = 0;
            foreach (var road in _allPaths)
            {
                if (road != null && road.Owner == player && road.IsBuilt)
                {
                    foundInAllPaths++;
                    Debug.Log($"  _allPaths Road: {road.HexPosition.X},{road.HexPosition.Y} {road.EdgeDir}");
                    
                    // Verifică dacă e în player.Paths
                    if (!player.Paths.Contains(road))
                    {
                        Debug.LogWarning($"  ⚠️ Road found in _allPaths but NOT in player.Paths! ADDING IT!");
                        player.Paths.Add(road);
                    }
                }
            }
            Debug.Log($"Found {foundInAllPaths} roads in _allPaths for player {player.name}");
            
            Debug.Log($"\n--- CONNECTIVITY CHECK ---");
            bool isConnected = IsDirectlyConnectedToPlayerAssets(targetPath, player);
            Debug.Log($"IsDirectlyConnectedToPlayerAssets result: {isConnected}");
            
            if (!isConnected)
            {
                Debug.Log("❌ CanBuildRoad: Not connected to player assets");
                return false;
            }
            
            Debug.Log("✅ CanBuildRoad: All checks passed");
            return true;
        }

        private bool IsDirectlyConnectedToPlayerAssets(PathController targetPath, PlayerBase player)
        {
            Debug.Log($"\n=== CONNECTIVITY CHECK DETAILED ===");
            Debug.Log($"Target path: {targetPath.HexPosition.X},{targetPath.HexPosition.Y} {targetPath.EdgeDir}");
            
            Debug.Log($"\n--- PLAYER SETTLEMENTS DEBUG ---");
            Debug.Log($"Player {player.name} has {player.Settlements.Count} settlements:");
            foreach (var settlement in player.Settlements)
            {
                if (settlement != null)
                {
                    Debug.Log($"  Settlement: {settlement.HexPosition.X},{settlement.HexPosition.Y} {settlement.VertexDir} (HasOwner: {settlement.HasOwner}, Owner: {settlement.Owner?.name ?? "NULL"})");
                }
            }
            
            Debug.Log($"\nChecking ALL settlements on board for player {player.name}:");
            var allSettlements = FindObjectsOfType<SettlementController>();
            foreach (var settlement in allSettlements)
            {
                if (settlement.HasOwner && settlement.Owner == player)
                {
                    Debug.Log($"  Board Settlement: {settlement.HexPosition.X},{settlement.HexPosition.Y} {settlement.VertexDir}");
                }
            }
            
            var targetEndpoints = GetExactEdgeEndpoints(targetPath.HexPosition, targetPath.EdgeDir);
            
            Debug.Log($"Target path has {targetEndpoints.Count} endpoints:");
            foreach (var endpoint in targetEndpoints)
            {
                Debug.Log($"  - {endpoint.Item1.X},{endpoint.Item1.Y} {endpoint.Item2}");
            }
            
            foreach (var (vertexPos, vertexDir) in targetEndpoints)
            {
                Debug.Log($"--- Checking endpoint {vertexPos.X},{vertexPos.Y} {vertexDir} ---");
                
                Debug.Log("Checking player.Settlements list for this vertex...");
                foreach (var settlement in player.Settlements)
                {
                    if (settlement != null)
                    {
                        Debug.Log($"  Comparing with settlement at {settlement.HexPosition.X},{settlement.HexPosition.Y} {settlement.VertexDir}");
                        
                        if (settlement.HexPosition.X == vertexPos.X && 
                            settlement.HexPosition.Y == vertexPos.Y && 
                            settlement.VertexDir == vertexDir)
                        {
                            Debug.Log($"✅ MANUAL MATCH! Found player settlement in Settlements list");
                            return true;
                        }
                    }
                }
                
                Debug.Log("Checking boardController.BoardGrid.GetVertex...");
                try
                {
                    var vertex = boardController.BoardGrid.GetVertex(vertexPos, vertexDir);
                    
                    if (vertex != null)
                    {
                        Debug.Log($"  GetVertex returned: {vertex.GetType().Name}");
                        
                        if (vertex is SettlementController settlement)
                        {
                            Debug.Log($"  Settlement found: HasOwner={settlement.HasOwner}, Owner={settlement.Owner?.name ?? "NULL"}");
                            Debug.Log($"  Settlement position: {settlement.HexPosition.X},{settlement.HexPosition.Y} {settlement.VertexDir}");
                            
                            if (settlement.HasOwner && settlement.Owner == player)
                            {
                                Debug.Log($"✅ BOARDGRID MATCH! Found player settlement via BoardGrid");
                                return true;
                            }
                        }
                        else
                        {
                            Debug.Log($"  Vertex is not a SettlementController, it's a {vertex.GetType().Name}");
                        }
                    }
                    else
                    {
                        Debug.Log($"  GetVertex returned NULL");
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"  Error calling GetVertex: {e.Message}");
                }
                
                Debug.Log("Checking via FindObjectsOfType...");
                foreach (var settlement in allSettlements)
                {
                    if (settlement.HexPosition.X == vertexPos.X && 
                        settlement.HexPosition.Y == vertexPos.Y && 
                        settlement.VertexDir == vertexDir)
                    {
                        Debug.Log($"  Found settlement at this vertex: HasOwner={settlement.HasOwner}, Owner={settlement.Owner?.name ?? "NULL"}");
                        
                        if (settlement.HasOwner && settlement.Owner == player)
                        {
                            Debug.Log($"✅ FINDOBJECTS MATCH! Found player settlement via FindObjectsOfType");
                            return true;
                        }
                    }
                }
                Debug.Log("Checking player.Paths for connecting roads...");
                foreach (var existingPath in player.Paths)
                {
                    if (existingPath != null && existingPath != targetPath && existingPath.IsBuilt)
                    {
                        Debug.Log($"  Checking road: {existingPath.HexPosition.X},{existingPath.HexPosition.Y} {existingPath.EdgeDir}");
                        
                        var existingEndpoints = GetExactEdgeEndpoints(existingPath.HexPosition, existingPath.EdgeDir);
                        foreach (var (existingVertexPos, existingVertexDir) in existingEndpoints)
                        {
                            Debug.Log($"    Road endpoint: {existingVertexPos.X},{existingVertexPos.Y} {existingVertexDir}");
                            
                            if (existingVertexPos.X == vertexPos.X && 
                                existingVertexPos.Y == vertexPos.Y && 
                                existingVertexDir == vertexDir)
                            {
                                Debug.Log($"✅ CONNECTED via road in player.Paths: {existingPath.HexPosition.X},{existingPath.HexPosition.Y} {existingPath.EdgeDir}");
                                return true;
                            }
                        }
                    }
                }

                Debug.Log("Checking _allPaths for connecting roads...");
                foreach (var existingPath in _allPaths)
                {
                    if (existingPath != null && existingPath != targetPath && 
                        existingPath.IsBuilt && existingPath.Owner == player)
                    {
                        Debug.Log($"  Checking _allPaths road: {existingPath.HexPosition.X},{existingPath.HexPosition.Y} {existingPath.EdgeDir}");
                        
                        var existingEndpoints = GetExactEdgeEndpoints(existingPath.HexPosition, existingPath.EdgeDir);
                        foreach (var (existingVertexPos, existingVertexDir) in existingEndpoints)
                        {
                            if (existingVertexPos.X == vertexPos.X && 
                                existingVertexPos.Y == vertexPos.Y && 
                                existingVertexDir == vertexDir)
                            {
                                Debug.Log($"✅ CONNECTED via road in _allPaths: {existingPath.HexPosition.X},{existingPath.HexPosition.Y} {existingPath.EdgeDir}");
                                
                                // Sync cu player.Paths
                                if (!player.Paths.Contains(existingPath))
                                {
                                    Debug.LogWarning($"Adding missing road to player.Paths");
                                    player.Paths.Add(existingPath);
                                }
                                
                                return true;
                            }
                        }
                    }
                }
                
                Debug.Log($"No connections found for endpoint {vertexPos.X},{vertexPos.Y} {vertexDir}");
            }
            
            Debug.Log("❌ NO CONNECTIONS FOUND - Cannot build road");
            return false;
        }
        private List<(HexPosition, HexVertexDir)> GetExactEdgeEndpoints(HexPosition hexPos, HexEdgeDir edgeDir)
        {
            var endpoints = new List<(HexPosition, HexVertexDir)>();

            Debug.Log($"Calculating endpoints for edge at {hexPos.X},{hexPos.Y} {edgeDir}");

            switch (edgeDir)
            {
                case HexEdgeDir.Top:
                    endpoints.Add((hexPos, HexVertexDir.TopLeft));
                    endpoints.Add((hexPos, HexVertexDir.TopRight));
                    break;
                case HexEdgeDir.TopRight:
                    endpoints.Add((hexPos, HexVertexDir.TopRight));
                    endpoints.Add((hexPos, HexVertexDir.Right));
                    break;
                case HexEdgeDir.BottomRight:
                    endpoints.Add((hexPos, HexVertexDir.Right));
                    endpoints.Add((hexPos, HexVertexDir.BottomRight));
                    break;
                case HexEdgeDir.Bottom:
                    endpoints.Add((hexPos, HexVertexDir.BottomRight));
                    endpoints.Add((hexPos, HexVertexDir.BottomLeft));
                    break;
                case HexEdgeDir.BottomLeft:
                    endpoints.Add((hexPos, HexVertexDir.BottomLeft));
                    endpoints.Add((hexPos, HexVertexDir.Left));
                    break;
                case HexEdgeDir.TopLeft:
                    endpoints.Add((hexPos, HexVertexDir.Left));
                    endpoints.Add((hexPos, HexVertexDir.TopLeft));
                    break;
                default:
                    Debug.LogError($"Unknown edge direction: {edgeDir}");
                    break;
            }

            Debug.Log($"Calculated endpoints:");
            foreach (var endpoint in endpoints)
            {
                Debug.Log($"  - {endpoint.Item1.X},{endpoint.Item1.Y} {endpoint.Item2}");
            }

            return endpoints;
        }
        private void SyncPlayerSettlements(PlayerBase player)
        {
            Debug.Log($"=== SYNCING PLAYER SETTLEMENTS ===");
    
            // Găsește toate așezările de pe board care aparțin playerului
            var allSettlements = FindObjectsOfType<SettlementController>();
            var foundSettlements = new List<SettlementController>();
    
            foreach (var settlement in allSettlements)
            {
                if (settlement.HasOwner && settlement.Owner == player)
                {
                    foundSettlements.Add(settlement);
                    Debug.Log($"Found board settlement: {settlement.HexPosition.X},{settlement.HexPosition.Y} {settlement.VertexDir}");
            
                    // Verifică dacă e în player.Settlements
                    if (!player.Settlements.Contains(settlement))
                    {
                        Debug.LogWarning($"Settlement NOT in player.Settlements - ADDING IT!");
                        player.Settlements.Add(settlement);
                    }
                }
            }
    
            Debug.Log($"Player {player.name} now has {player.Settlements.Count} settlements in list");
        }

        private void AddPortBuffForSettlement(SettlementController settlement, PlayerBase player)
        {
            if (settlement.ConnectedPortController != null) settlement.ConnectedPortController.AddPlayerBuff(player);
        }
        
        public override IEnumerator BuildCity(PlayerBase player)
        {
            if (!CanBuildCity(player))
                yield break;

            Debug.Log("Building city for:" + player.name);
            yield return player.UpgradeToCityCoroutine();

            var closestCorner = player.SelectedHouse;

            Debug.Log(closestCorner.Owner.name + " vs " + player.name);
            if (!closestCorner.HasOwner || closestCorner.Owner != player)
                Debug.Log("Not your settlement");
            else
            {
                closestCorner.UpgradeToCity();
            }
        }
        
        private void OnDiceGameState() =>
            _isFirstStates = false;
    }
}