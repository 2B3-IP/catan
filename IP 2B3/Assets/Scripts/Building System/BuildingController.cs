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

                if (selectedHouse != null)
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
            foreach (var (settlement, pos, dir) in neighbouringVertices)
            {
                if (settlement.HasOwner)
                {
                    Debug.Log("DISTANCE OF 2 NOT RESPECTED");
                    return false;
                }
            }
            if (_isFirstStates)
            {
                return true;
            }
            Debug.Log("INAINTE DE FOR ");
            int index = 0;

            bool canBePlaced = false;
            foreach (var (settlement, pos, dir) in neighbouringVertices)
            {
                /*// am o casa vecina deja construita
                if (settlement.HasOwner)
                    return false;*/

                HexVertexDir a = houseDir switch
                {
                    HexVertexDir.TopLeft => housePosition.X > pos.X ? HexVertexDir.Right : HexVertexDir.BottomLeft,
                    HexVertexDir.TopRight => housePosition.X < pos.X ? HexVertexDir.Left : HexVertexDir.BottomRight,
                    HexVertexDir.Right => housePosition.Y > pos.Y ? HexVertexDir.TopLeft : HexVertexDir.BottomLeft,
                    HexVertexDir.BottomRight => housePosition.X < pos.X ? HexVertexDir.Left : HexVertexDir.TopRight,
                    HexVertexDir.BottomLeft => housePosition.X > pos.X ? HexVertexDir.Right : HexVertexDir.TopLeft,
                    HexVertexDir.Left => housePosition.Y < pos.Y ? HexVertexDir.BottomRight : HexVertexDir.TopRight,
                    _ => throw new ArgumentOutOfRangeException()
                };

                HexEdgeDir edgeDir = index switch
                {
                    0 => GetHexDir(houseDir, dir),
                    1 => GetHexDir(dir, houseDir),
                    _ => GetHexDir(dir, a)
                };

                Debug.Log("dir: " + dir+" house dir: " + houseDir +" edge dir: " + edgeDir + " house opp dir: " + houseDir.Opposite());
                index++;
                
                var path = boardGrid.GetEdge(pos, edgeDir);
                Debug.Log("AICI E PATH ", path);

                if (path == null)
                    continue;

                if (path.IsBuilt && path.Owner == player)
                    canBePlaced = true;
            }
            Debug.Log("DUPA DE FOR " + canBePlaced);

            return canBePlaced;
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
                
                _ => HexEdgeDir.Top
            };
        }
      
        
        protected override bool CanBuildRoad(PlayerBase player, PathController targetPath)
        {
            return true;
            var housePosition = targetPath.HexPosition;
            var houseDir = targetPath.EdgeDir;

            var boardGrid = boardController.BoardGrid;
            var neighbouringVertices = boardGrid.GetNeighbouringEdges(housePosition, houseDir);
            Debug.Log("INAINTE DE FOR ");

            bool canBePlaced = false;
            foreach (var (settlement, pos, dir) in neighbouringVertices)
            {
                
            }
            
            return canBePlaced;
        }
        
        /*
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
        }*/

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