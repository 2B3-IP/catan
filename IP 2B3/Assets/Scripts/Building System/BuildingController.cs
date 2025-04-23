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
            var availableSettlements =
                _settlements.Where(s => !s.HasOwner && CanBuildHouse(s, player, _allPaths)).ToArray();
            
            if (availableSettlements.Length == 0)
            {
                Debug.Log("No available settlements to build a house.");
                yield break;
            }
            
            ShowAvailableSettlements(availableSettlements);

            _housePlaced = false;
            SettlementController.OnSettlementSelected += OnSettlementSelected;
            
            while (!_housePlaced)
            {
                yield return null;
            }
            
            if (_selectedSettlement != null)
            {
                _selectedSettlement.SetOwner(player);
                _selectedSettlement.BuildHouse();
                Debug.Log($"House built at {_selectedSettlement.name} by {player.name}");
            }
            SettlementController.OnSettlementSelected -= OnSettlementSelected;
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

        private bool CanBuildHouse(SettlementController targetSettlement, PlayerBase player, Path[] allPaths)
        {
            bool hasConnectedRoad = allPaths.Any(p => p.Owner == player && p.ConnectsTo(targetSettlement));
            
            if (!hasConnectedRoad)
                return false;

            Queue<(SettlementController settlement, int distance)> queue = new();
            HashSet<SettlementController> visited = new();
            queue.Enqueue((targetSettlement,0));
            visited.Add(targetSettlement);

            while (queue.Count > 0)
            {
                var (current, distance) = queue.Dequeue();
                if (distance > 0 && current.HasOwner)
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
        public override IEnumerator BuildRoad(PlayerBase player)
        {
            yield break;
        }

        public override IEnumerator BuildCity(PlayerBase player)
        {
          
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