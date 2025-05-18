using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using B3.SettlementSystem;
using B3.PlayerSystem;
using B3.BoardSystem;
using B3.PieceSystem;
using B3.PortSystem;
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
            //vedem daca playerul mai are case la dispozitie
            if (!CanBuildHouse(player))
                return false;
            //vf daca asezarea este deja ocupata
            if (targetSettlement.HasOwner)
                return false;

            bool isConnectedToOwnRoad = false;

            HashSet<SettlementController> visited = new HashSet<SettlementController>();
            Stack<SettlementController> stack = new Stack<SettlementController>();

            foreach (var settlement in player.Settlements)
            {
                stack.Push(settlement);
                visited.Add(settlement);
            }

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                foreach (var path in allPaths)
                {
                    //daca drumul apartine playerului
                    if (path.Owner == player)
                    {
                        if (path.ConnectsTo(current))
                        {
                            //vf daca drumul este conectat la asezarea curenta
                            var connectedSettlement = path.GetOtherSettlement(current);
                            if (connectedSettlement == targetSettlement)
                            {
                                //obtinem cealalta asezarea de la capatul drumului
                                isConnectedToOwnRoad = true;
                                break;
                            }
                            //daca este asezarea tinta am gasit o conexiune
                            if (connectedSettlement != null && !visited.Contains(connectedSettlement))
                            {
                                visited.Add(connectedSettlement);
                                stack.Push(connectedSettlement);
                            }
                        }
                    }
                }

                if (isConnectedToOwnRoad)
                    break;
            }

            if (!isConnectedToOwnRoad)
                return false;
            
            //vf regula distantei de 2 intre oricare doua asezari
            visited.Clear();
            Queue<(SettlementController settlementController, int distance)> queue = new Queue<(SettlementController, int)>();
            queue.Enqueue((targetSettlement, 0));
            visited.Add(targetSettlement);

            while (queue.Count > 0)
            {
                var (current, distance) = queue.Dequeue();
                if (distance > 0 && distance < 2 && current.HasOwner)
                    return false;
                if (distance >= 2)
                    continue;
                foreach (var path in allPaths)
                {
                    if (path.ConnectsTo(current))
                    {
                        var neighbor = path.GetOtherSettlement(current);
                        if (neighbor != null && !visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            queue.Enqueue((neighbor, distance + 1));
                            
                        }
                    }
                }
            }

            return true;
        }

        protected override bool CanBuildRoad(PlayerBase player, Path targetPath, Path[] allPaths)
        {
            //vf daca mai avem drumuri la dispozitie
            if (!base.CanBuildRoad(player))
                return false;
            //vf daca drumul e deja ocupat
            if (targetPath.Owner != null)
                return false;
            
            //un drum poate fi construit daca unul din capete are o asezare a playerului
            bool hasOwnedSettlement = (targetPath.SettlementA != null && targetPath.SettlementA.HasOwner &&
                                       targetPath.SettlementA.Owner == player) ||
                                      (targetPath.SettlementB != null && targetPath.SettlementB.HasOwner &&
                                       targetPath.SettlementB.Owner == player);
            
            //sau daca este conectat la un alt drum al sau
            bool isConnectedToOwnedRoad = false;
            foreach (var path in allPaths)
            {
                if (path.Owner == player)
                {
                    if ((path.ConnectsTo(targetPath.SettlementA) && targetPath.SettlementA != null) ||
                        (path.ConnectsTo(targetPath.SettlementB) && targetPath.SettlementB != null))
                    {
                        isConnectedToOwnedRoad = true;
                        break;
                    }
                }
            }

            return hasOwnedSettlement || isConnectedToOwnedRoad;
        }
        
        private void AddPortBuffForSettlement(SettlementController settlement, PlayerBase player)
        {
            var piece = settlement.GetComponentInParent<PieceController>();
            if (piece == null) return;
            
            var neighbors = piece.HexPosition.GetNeighbours();
            
            BoardController board = FindObjectOfType<BoardController>();
            if (board == null) return;

            foreach (var neighborPos in neighbors)
            {
                PortController portPiece = board.GetComponentAt<PortController>(neighborPos);
                if (portPiece == null) continue;
                
                portPiece.AddPlayerBuff(player);
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

        private bool IsConnectedToOwnedRoad(Path path, PlayerBase player)
        {
            return _allPaths.Any(p => p.Owner == player &&
                                      (p.ConnectsTo(path.SettlementA) || p.ConnectsTo(path.SettlementB)));
        }

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