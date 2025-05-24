using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B3.SettlementSystem;
using B3.PlayerSystem;
using B3.BoardSystem;
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
                
                //if (!_isFirstStates && !CanBuildHouse(selectedHouse, player))
                //    selectedHouse = null;
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
                //if (!CanBuildRoad(player, selectedPath))
                //    selectedPath = null;
            }
            
            selectedPath.Owner = player;
            selectedPath.BuildRoad();
            
            player.Paths.Add(selectedPath);
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
            // mai are playerul roaduri disponibile?
            if (!base.CanBuildRoad(player))
                return false;

            // este deja construit?
            if (targetPath.IsBuilt || targetPath.Owner != null)
                return false;

            HexPosition hexPosition = targetPath.HexPosition;
            HexEdgeDir edgeDir = targetPath.EdgeDir;

            var neighbouringEdges =
                boardController.BoardGrid.GetNeighbouringEdges(hexPosition, edgeDir);

            bool hasOwnedSettlement = false;

            foreach (var (vertex, vertexPos, vertexDir) in neighbouringEdges)
            {
                if (vertex is SettlementController settlement && settlement.HasOwner && settlement.Owner == player)
                {
                    hasOwnedSettlement = true;
                    break;
                }
            }

            bool isConnectedToOwnedRoad = false;
            foreach (var (vertex, vertexPos, vertexDir) in neighbouringEdges)
            {
                if (vertex is SettlementController settlement)
                {
                    var connectedEdges = boardController.BoardGrid
                        .GetNeighbouringEdges(vertexPos, VertexDirToEdgeDir(vertexDir));

                    foreach (var (otherVertex, otherPos, otherDir) in connectedEdges)
                    {
                        foreach (var path in _allPaths)
                        {
                            if (path != targetPath && path.IsBuilt && path.Owner == player)
                            {
                                if ((path.HexPosition.X == vertexPos.X && path.HexPosition.Y == vertexPos.Y) ||
                                    (path.HexPosition.X == otherPos.X && path.HexPosition.Y == otherPos.Y))
                                {
                                    isConnectedToOwnedRoad = true;
                                    break;
                                }
                            }
                        }

                        if (isConnectedToOwnedRoad)
                            break;
                    }
                }

                if (isConnectedToOwnedRoad)
                    break;
            }

            return hasOwnedSettlement || isConnectedToOwnedRoad;
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
            if(settlement.ConnectedPortController!=null) settlement.ConnectedPortController.AddPlayerBuff(player);
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