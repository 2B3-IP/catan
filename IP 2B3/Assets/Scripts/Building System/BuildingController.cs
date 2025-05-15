using System.Collections;
using System.Collections.Generic;
using B3.GameStateSystem;
using UnityEngine;
using System.Linq;
using B3.SettlementSystem;
using B3.PlayerSystem;
using UnityEngine.InputSystem;

namespace B3.BuildingSystem
{
    internal sealed class BuildingController : BuildingControllerBase
    {  
        [SerializeField] private InputActionReference clickButton;
        [SerializeField] private LayerMask settlementLayerMask;
        [SerializeField] private SettlementController settlementController;
        
        private readonly Camera _playerCamera = Camera.main;
        private readonly RaycastHit[] _hits = new RaycastHit[5];
        
        private bool _housePlaced = false;
        private SettlementController _selectedSettlement;
        
        private SettlementController[] _settlements;
        private Path[] _allPaths;

        private void Awake()
        {
            _settlements = FindObjectsByType<SettlementController>(FindObjectsSortMode.None);
            _allPaths = FindObjectsByType<Path>(FindObjectsSortMode.None);
        }

        public override IEnumerator BuildHouse(PlayerBase player)
        {
            yield return player.BuildHouseCoroutine();

            Vector2? closestCorner = player.ClosestCorner;

            if (closestCorner.HasValue)
            {
                Vector3 position = new Vector3(closestCorner.Value.x, 0f, closestCorner.Value.y);
        
                var house = Instantiate(settlementController, position, Quaternion.identity);

                house.SetOwner(player);
                player.Settlements.Add(house);
        
                Debug.Log($"House built at {position} by {player.name}");
            }
            else
            {
                Debug.LogWarning("No valid closest corner found. House not built.");
            }
        }
        
        private void ShowAvailableSettlements(SettlementController[] availableSettlements)
        {
            foreach (var settlement in availableSettlements)
            {
                settlement.Highlight(true);
                settlement.AllowSelection(true);
            }
        }

        private void OnSettlementSelected(SettlementController settlement)
        {
            if (_housePlaced)
                return;
            _selectedSettlement = settlement;
            _housePlaced = true;
            foreach (var s in _settlements)
            {
                s.Highlight(false);
                s.AllowSelection(false);
            }
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
        public override IEnumerator BuildRoad(PlayerBase player)
        {
            if (!CanBuildRoad(player))
                yield break;
            
            //daca drumul nu este ocupat si unul din capete este owned de un player
            //sau daca este conectat de un alt drum al playerului atunci putem construi drumul
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
            Path selectedPath = null;
            bool roadPlaced = false;

            clickButton.action.Enable();
            //daca obiectul (adica aici drumul) pe care a dat click playerul este printre pathurile available setam drept Owner playerul 
            while (!roadPlaced)
            {
                if (clickButton.action.WasPressedThisFrame())
                {
                    var ray = _playerCamera.ScreenPointToRay(Mouse.current.position.value);
                    if (Physics.Raycast(ray, out RaycastHit hit, 100f))
                    {
                        var path = hit.transform.GetComponent<Path>();
                        if (path != null && availablePaths.Contains(path))
                        {
                            path.Owner = player;
                            player.Paths.Add(path);
                            Debug.Log($"Road built between {path.SettlementA?.name} and {path.SettlementB?.name} by {player.name}");
                            roadPlaced = true;
                            selectedPath = path;
                        }
                    }
                }
                yield return null;
            }

            HighlightPaths(availablePaths, false);
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
            
            var action = clickButton.action;
            while(!action.WasPressedThisFrame())
                yield return null;
            bool cityDone = false;
            while (!cityDone)
            {
                int hitCount = 0;
                while(hitCount == 0)
                {
                    var ray = _playerCamera.ScreenPointToRay(Mouse.current.position.value);
                    hitCount = Physics.RaycastNonAlloc(ray, _hits, float.MaxValue, settlementLayerMask);
                }
                var closestHit = _hits[0];
                for (int i = 1; i < hitCount; i++)
                {
                    var hit = _hits[i];
                
                    if(closestHit.distance > hit.distance)
                        closestHit = hit;
                }
                var settlement = closestHit.transform.GetComponent<SettlementController>();
                if (settlement == null)
                    continue;
                if(!settlement.HasOwner || settlement.Owner!= player)
                    continue;
                if (settlement.IsCity)
                    continue;
                settlement.UpgradeToCity();
                cityDone = true;
            }
        }
    }
}