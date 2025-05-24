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
using B3.PortSystem;
using UnityEngine.InputSystem;

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

        private void Awake()
        {
            _allPaths = FindObjectsByType<PathController>(FindObjectsSortMode.None);
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
                
                if (!_isFirstStates && !CanBuildHouse(selectedHouse, player))
                    selectedHouse = null;
            }
            
            selectedHouse.SetOwner(player);
            selectedHouse.BuildHouse();
            player.Settlements.Add(selectedHouse);
            
            TryAddPortBuffForSettlement(selectedHouse, player);
            
            var message = $"House built at {selectedHouse.HexPosition.X} {selectedHouse.HexPosition.Y}, {selectedHouse.VertexDir} by {player.name}";
            Debug.Log(message);
            
            if(player is HumanPlayer)
                AI.SendMove(message);
        }

        public override IEnumerator BuildRoad(PlayerBase player)
        {
            if (!CanBuildRoad(player))
                yield break;
            
            PathController selectedPath = null;
            
            while (selectedPath == null)
            {
                yield return player.BuildRoadCoroutine();
                selectedPath = player.SelectedPath;
                
                // pusca daca apesi pe un road care nu e bun ( lipit de o casa)
                if (!CanBuildRoad(player, selectedPath))
                    selectedPath = null;
            }
            
            selectedPath.Owner = player;
            selectedPath.BuildRoad();
            
            player.Paths.Add(selectedPath);
            
            if (longestRoadController != null)
            {
                longestRoadController.CheckLongestRoadAfterBuild(player);
            }
        }

        protected override bool CanBuildHouse(SettlementController targetSettlement, PlayerBase player)
        {
            // este asezarea ocupata?
            if (targetSettlement.HasOwner)
                return false;

            // obt poz si dir vertexului pentru asezarea tintă
            HexPosition hexPosition = targetSettlement.HexPosition;
            HexVertexDir vertexDir = targetSettlement.VertexDir;

            // vf nodurile vecine
            bool isConnectedToPlayerRoad = false;

            //vecinii vertexului curent
            var neighbouringVertices = boardController.BoardGrid
                .GetNeighbouringVertices(hexPosition, vertexDir);

            // vf dacă exista un drum construit de player care este conectat la aceasta asezare
            foreach (var (neighbourVertex, neighbourPos, neighbourDir) in neighbouringVertices)
            {
                foreach (var path in _allPaths)
                {
                    if (path.IsBuilt && path.Owner == player)
                    {
                        if ((path.HexPosition.X == hexPosition.X && path.HexPosition.Y == hexPosition.Y) ||
                            (path.HexPosition.X == neighbourPos.X && path.HexPosition.Y == neighbourPos.Y))
                        {
                            isConnectedToPlayerRoad = true;
                            break;
                        }
                    }
                }

                if (isConnectedToPlayerRoad)
                    break;
            }

            if (!isConnectedToPlayerRoad)
                return false;

            // vf regula distantei de 2 folosind DFS cu lungime maxima 2
            HashSet<SettlementController> visited = new HashSet<SettlementController>();
            Queue<(SettlementController settlement, int distance)> queue = new Queue<(SettlementController, int)>();

            queue.Enqueue((targetSettlement, 0));
            visited.Add(targetSettlement);

            while (queue.Count > 0)
            {
                var (currentSettlement, distance) = queue.Dequeue();
                if (distance > 0 && distance < 2 && currentSettlement.HasOwner)
                    return false;

                if (distance >= 2)
                    continue;
                HexPosition currentPos = currentSettlement.HexPosition;
                HexVertexDir currentDir = currentSettlement.VertexDir;

                var currentNeighbours = boardController.BoardGrid
                    .GetNeighbouringVertices(currentPos, currentDir);

                foreach (var (neighbourVertex, neighbourPos, neighbourDir) in currentNeighbours)
                {
                    if (neighbourVertex is SettlementController neighbourSettlement &&
                        !visited.Contains(neighbourSettlement))
                    {
                        visited.Add(neighbourSettlement);
                        queue.Enqueue((neighbourSettlement, distance + 1));
                    }
                }
            }

            return true;
        }
        
        protected override bool CanBuildRoad(PlayerBase player, PathController targetPath)
        {
            if (!base.CanBuildRoad(player))
                return false;

            if (targetPath.IsBuilt || targetPath.Owner != null)
                return false;

            // un drum poate fi construit doar daca se conecteaza direct 
            // prin unul din capetele sale la o asezare sau drum al playerului
            return IsDirectlyConnectedToPlayerAssets(targetPath, player);
        }

        private bool IsDirectlyConnectedToPlayerAssets(PathController targetPath, PlayerBase player)
        {
            // obtin cele 2 vertexuri exacte de la capetele drumului
            var edgeEndpoints = GetExactEdgeEndpoints(targetPath.HexPosition, targetPath.EdgeDir);
            
            foreach (var (vertexPos, vertexDir) in edgeEndpoints)
            {
                // verific daca la acest vertex exista o asezare a playerului
                var vertex = boardController.BoardGrid.GetVertex(vertexPos, vertexDir);
                if (vertex is SettlementController settlement && 
                    settlement.HasOwner && settlement.Owner == player)
                {
                    return true;
                }
                
                // verific daca la acest vertex se conecteaza un alt drum al playerului
                if (HasPlayerRoadAtVertex(vertexPos, vertexDir, player, targetPath))
                {
                    return true;
                }
            }
            
            return false;
        }

        private bool HasPlayerRoadAtVertex(HexPosition vertexPos, HexVertexDir vertexDir, PlayerBase player, PathController excludePath)
        {
            // verific toate drumurile playerului pentru a vedea daca vreunul se conecteaza la acest vertex
            foreach (var existingPath in _allPaths)
            {
                if (existingPath != excludePath && existingPath.IsBuilt && existingPath.Owner == player)
                {
                    var existingEndpoints = GetExactEdgeEndpoints(existingPath.HexPosition, existingPath.EdgeDir);
                    
                    foreach (var (existingVertexPos, existingVertexDir) in existingEndpoints)
                    {
                        if (existingVertexPos.X == vertexPos.X && 
                            existingVertexPos.Y == vertexPos.Y && 
                            existingVertexDir == vertexDir)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //obtine exact cele 2 vertexuri de la capetele unui edge
        private List<(HexPosition, HexVertexDir)> GetExactEdgeEndpoints(HexPosition hexPos, HexEdgeDir edgeDir)
        {
            var endpoints = new List<(HexPosition, HexVertexDir)>();
            
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
            }
            
            return endpoints;
        }
        private HexEdgeDir VertexDirToEdgeDir(HexVertexDir vertexDir)
        {
            switch (vertexDir)
            {
                case HexVertexDir.TopRight: return HexEdgeDir.TopRight;
                case HexVertexDir.Right: return HexEdgeDir.BottomRight;
                case HexVertexDir.BottomRight: return HexEdgeDir.Bottom;
                case HexVertexDir.BottomLeft: return HexEdgeDir.BottomLeft;
                case HexVertexDir.Left: return HexEdgeDir.TopLeft;
                case HexVertexDir.TopLeft: return HexEdgeDir.Top;
                default: return HexEdgeDir.Top;
            }
        }

        private void TryAddPortBuffForSettlement(SettlementController settlement, PlayerBase player)
        {
            if (settlement.ConnectedPortController != null) settlement.ConnectedPortController.AddPlayerBuff(player);
            
        }

        /*private bool IsConnectedToOwnedRoad(Path path, PlayerBase player)
        {
            return _allPaths.Any(p => p.Owner == player &&
                                      (p.ConnectsTo(path.SettlementA) || p.ConnectsTo(path.SettlementB)));
        }*/

        public override IEnumerator BuildCity(PlayerBase player)
        {
            if (!CanBuildCity(player))
                yield break;

            yield return player.UpgradeToCityCoroutine();

            var closestCorner = player.SelectedHouse;

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