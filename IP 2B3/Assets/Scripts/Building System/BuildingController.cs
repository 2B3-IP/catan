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

            Debug.Log("Building house successfully!");
            
            Debug.Log($"BEFORE: Settlement at ({selectedHouse.HexPosition.X},{selectedHouse.HexPosition.Y} {selectedHouse.VertexDir}) - HasOwner: {selectedHouse.HasOwner}, Owner: {selectedHouse.Owner?.name ?? "NULL"}");
    
            selectedHouse.Owner = player;
            selectedHouse.BuildHouse();
            player.Settlements.Add(selectedHouse);
            
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
    
            Debug.Log($"Building road at {selectedPath.HexPosition.X},{selectedPath.HexPosition.Y} {selectedPath.EdgeDir}");
    
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
                    0 => GetHexDir(houseDir, dir),
                    1 => GetHexDir(dir, houseDir),
                    _ => GetHexDir(dir, GetVertexDirBasedOnStartDir(houseDir, housePosition, pos))
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

        private HexVertexDir GetVertexDirBasedOnStartDir(HexVertexDir startDir, HexPosition startPos, HexPosition endPos)
        {
            return startDir switch
            {
                HexVertexDir.TopLeft => startPos.X == endPos.X ? HexVertexDir.BottomLeft : HexVertexDir.Right,
                HexVertexDir.TopRight => startPos.Y == endPos.Y ? HexVertexDir.Left : HexVertexDir.BottomRight,
                HexVertexDir.Right => startPos.Y == endPos.Y ? HexVertexDir.BottomLeft : HexVertexDir.TopLeft,
                HexVertexDir.BottomRight => startPos.X == endPos.X ? HexVertexDir.TopRight : HexVertexDir.Left,
                HexVertexDir.BottomLeft => startPos.X == endPos.X ? HexVertexDir.TopLeft : HexVertexDir.Right,
                HexVertexDir.Left => startPos.Y == endPos.Y ? HexVertexDir.TopRight : HexVertexDir.BottomRight,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        private HexEdgeDir GetHexDir(HexVertexDir vertex1, HexVertexDir vertex2)
        {
            return (vertex1, vertex2) switch
            {
                (HexVertexDir.TopLeft, HexVertexDir.TopRight) => HexEdgeDir.Top,
                (HexVertexDir.TopRight, HexVertexDir.Right) => HexEdgeDir.TopRight,
                (HexVertexDir.Right, HexVertexDir.BottomRight) => HexEdgeDir.BottomRight,
                (HexVertexDir.BottomRight, HexVertexDir.BottomLeft) => HexEdgeDir.Bottom,
                (HexVertexDir.BottomLeft, HexVertexDir.Left) => HexEdgeDir.BottomLeft,
                (HexVertexDir.Left, HexVertexDir.TopLeft) => HexEdgeDir.TopLeft,
                
                (HexVertexDir.TopRight, HexVertexDir.TopLeft) => HexEdgeDir.Top,
                (HexVertexDir.Right, HexVertexDir.TopRight) => HexEdgeDir.TopRight,
                (HexVertexDir.BottomRight, HexVertexDir.Right) => HexEdgeDir.BottomRight,
                (HexVertexDir.BottomLeft, HexVertexDir.BottomRight) => HexEdgeDir.Bottom,
                (HexVertexDir.Left, HexVertexDir.BottomLeft) => HexEdgeDir.BottomLeft,
                (HexVertexDir.TopLeft, HexVertexDir.Left) => HexEdgeDir.TopLeft,
                
                _ => throw new ArgumentException($"Invalid vertex directions: {vertex1}, {vertex2}"), 
            };
        }
      
        private (HexVertexDir, HexVertexDir) GetVertexDirs(HexEdgeDir edgeDir)
        {
            return edgeDir switch
            {
                HexEdgeDir.Top => (HexVertexDir.TopLeft, HexVertexDir.TopRight),
                HexEdgeDir.TopRight => (HexVertexDir.TopRight, HexVertexDir.Right),
                HexEdgeDir.BottomRight => (HexVertexDir.Right, HexVertexDir.BottomRight),
                HexEdgeDir.Bottom => (HexVertexDir.BottomRight, HexVertexDir.BottomLeft),
                HexEdgeDir.BottomLeft => (HexVertexDir.BottomLeft, HexVertexDir.Left),
                HexEdgeDir.TopLeft => (HexVertexDir.Left, HexVertexDir.TopLeft)
            };
        }
        
        protected override bool CanBuildRoad(PlayerBase player, PathController targetPath)
        {
            if (targetPath.IsBuilt)
                return false;
            
            var roadPosition = targetPath.HexPosition;
            var roadDir = targetPath.EdgeDir;

            var boardGrid = boardController.BoardGrid;
            
            var (roadVertex1, roadVertex2) = GetVertexDirs(roadDir);
            
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
                    if(settlement == vertex2 || settlement == vertex1)
                        continue;

                    var edgeDir = index switch
                    {
                        1 => GetHexDir(GetVertexDirBasedOnStartDir(vertex, roadPosition, pos), dir),
                        _ => GetHexDir(dir, vertex)
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