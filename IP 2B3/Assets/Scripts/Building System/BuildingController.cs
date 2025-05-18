using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using B3.SettlementSystem;
using B3.PlayerSystem;
using B3.BoardSystem;
using B3.PieceSystem;
using UnityEngine.InputSystem;

namespace B3.BuildingSystem
{
    internal sealed class BuildingController : BuildingControllerBase
    {  
        [SerializeField] private InputActionReference clickButton;
        [SerializeField] private SettlementController settlementPrefab;
        
        private Camera _playerCamera;
        private readonly RaycastHit[] _hits = new RaycastHit[5];
        
        private SettlementController[] _settlements;
        private Path[] _allPaths;

        private void Awake()
        {
            _playerCamera = Camera.main;
                
            _settlements = FindObjectsByType<SettlementController>(FindObjectsSortMode.None);
            _allPaths = FindObjectsByType<Path>(FindObjectsSortMode.None);
        }

        public override IEnumerator BuildHouse(PlayerBase player)
        {
            yield return player.BuildHouseCoroutine();
            
            var closestCorner = player.ClosestCorner;

            closestCorner.SetOwner(player);
            closestCorner.BuildHouse();
            player.Settlements.Add(closestCorner);
    
            Debug.Log($"House built at {closestCorner.transform.position} by {player.name}");
        }
        
        public override IEnumerator BuildRoad(PlayerBase player)
        {
            if (!CanBuildRoad(player))
                yield break;

            
            var availablePaths = _allPaths
                .Where(p => p.Owner == null && (
                    (p.SettlementA != null && p.SettlementA.HasOwner && p.SettlementA.Owner == player) ||
                    (p.SettlementB != null && p.SettlementB.HasOwner && p.SettlementB.Owner == player) ||
                    IsConnectedToOwnedRoad(p, player)
                )).ToList();

            if (availablePaths.Count == 0)
            {
                Debug.Log("No available paths to build a road.");
                yield break;
            }

            HighlightPaths(availablePaths, true);
            bool roadPlaced = false;

            clickButton.action.Enable();

            BoardController board = FindObjectOfType<BoardController>();

            while (!roadPlaced)
            {
                if (clickButton.action.WasPressedThisFrame())
                {
                    var ray = _playerCamera.ScreenPointToRay(Mouse.current.position.value);
                    if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                    {
                        Vector3 hitPoint = hit.point;

                        PieceController piece = board.GetPieceAt(hitPoint);
                        if (piece == null)
                            continue;

                        Vector3 edgeMidpoint = board.GetClosestEdgeMidpoint(piece, hitPoint);

                        foreach (var path in availablePaths)
                        {
                            if (path.IsNearEdge(edgeMidpoint))
                            {
                                path.Owner = player;
                                player.Paths.Add(path);
                                Debug.Log($"Road built along edge near {edgeMidpoint} by {player.name}");
                                roadPlaced = true;
                                break;
                            }
                        }
                    }
                }

                yield return null;
            }

            HighlightPaths(availablePaths, false);
        }

        protected override bool CanBuildHouse(SettlementController targetSettlement, PlayerBase player, Path[] allPaths)
        {
            // jucatorul mai are case disponibile?
            if (!CanBuildHouse(player))
                return false;
                
            // este asezarea ocupata?
            if (targetSettlement.HasOwner)
                return false;
            
            // obt poz si dir vertexului pentru asezarea tintă
            HexPosition hexPosition = targetSettlement.HexPosition;
            HexVertexDir vertexDir = targetSettlement.VertexDir;
            
            // vf nodurile vecine
            bool isConnectedToPlayerRoad = false;
            
            //vecinii vertexului curent
            var neighbouringVertices = FindObjectOfType<BoardController>().BoardGrid.GetNeighbouringVertices(hexPosition, vertexDir);
            
            // vf dacă exista un drum construit de player care este conectat la aceasta asezare
            foreach (var (neighbourVertex, neighbourPos, neighbourDir) in neighbouringVertices)
            {
                foreach (var path in allPaths)
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
                
                var currentNeighbours = FindObjectOfType<BoardController>().BoardGrid.GetNeighbouringVertices(currentPos, currentDir);
                
                foreach (var (neighbourVertex, neighbourPos, neighbourDir) in currentNeighbours)
                {
                    if (neighbourVertex is SettlementController neighbourSettlement && !visited.Contains(neighbourSettlement))
                    {
                        visited.Add(neighbourSettlement);
                        queue.Enqueue((neighbourSettlement, distance + 1));
                    }
                }
            }
            return true;
        }
        
        
        protected override bool CanBuildRoad(PlayerBase player, Path targetPath, Path[] allPaths)
        {
            // mai are playerul roaduri disponibile?
            if (!base.CanBuildRoad(player))
                return false;
                
            // este deja construit?
            if (targetPath.IsBuilt || targetPath.Owner != null)
                return false;

            HexPosition hexPosition = targetPath.HexPosition;
            HexEdgeDir edgeDir = targetPath.EdgeDir;
            
            var neighbouringEdges = FindObjectOfType<BoardController>().BoardGrid.GetNeighbouringEdges(hexPosition, edgeDir);
            
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
                    var connectedEdges = FindObjectOfType<BoardController>().BoardGrid.GetNeighbouringEdges(vertexPos, VertexDirToEdgeDir(vertexDir));
                    
                    foreach (var (otherVertex, otherPos, otherDir) in connectedEdges)
                    {
                        foreach (var path in allPaths)
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
        private void HighlightPaths(List<Path> paths, bool highlight)
        {
            foreach (var path in paths)
            {
                //to do front: de schimbat culoare, scale, etc
                path.gameObject.GetComponent<Renderer>().material.color = highlight ? Color.green : Color.white;
            }
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
            
         
            bool cityDone = false;
            yield return player.UpgradeToCityCoroutine();
            var settlement=player.SelectedSettlement;
            
                if(!settlement.HasOwner || settlement.Owner!= player)
                    Debug.Log("Not your settlement");
                else if (settlement.IsCity)
                    Debug.Log("Already a city.");
                else
                {
                    settlement.UpgradeToCity();
                    cityDone = true;
                }
        
        }
    }
}