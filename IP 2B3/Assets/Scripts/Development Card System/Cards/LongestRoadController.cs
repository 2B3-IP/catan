using System.Collections.Generic;
using System.Linq;
using B3.BoardSystem;
using B3.BuildingSystem;
using B3.PlayerSystem;
using B3.SettlementSystem;
using UnityEngine;

namespace B3.DevelopmentCardSystem
{
    public class LongestRoadController : MonoBehaviour
    {
        private PlayerBase _currentLongestRoadOwner;
        private int _currentLongestRoadLength = 4; // minim 5 pentru a primi cardul
        
        public void CheckLongestRoadAfterBuild(PlayerBase player)
        {
            int playerLongestRoad = CalculateLongestRoad(player);
            
            // verificam daca jucatorul are drumul cel mai lung (minim 5)
            if (playerLongestRoad >= 5 && playerLongestRoad > _currentLongestRoadLength)
            {
                // transferam cardul la noul jucator
                TransferLongestRoadCard(player, playerLongestRoad);
            }
            // daca jucatorul curent inca are cel mai lung drum, verificam daca si-a marit drumul
            else if (_currentLongestRoadOwner == player && playerLongestRoad > _currentLongestRoadLength)
            {
                _currentLongestRoadLength = playerLongestRoad;
                Debug.Log($"{player.name} si-a marit drumul cel mai lung la {playerLongestRoad}");
            }
        }
        
        private void TransferLongestRoadCard(PlayerBase newOwner, int newLength)
        {
            // ii luam punctele celui care avea cardul inainte
            if (_currentLongestRoadOwner != null)
            {
                _currentLongestRoadOwner.RemoveVictoryPoints(2);
                Debug.Log($"{_currentLongestRoadOwner.name} a pierdut cardul 'Longest Road' (-2 puncte)");
            }
            
            // ii dam punctele noului jucator
            _currentLongestRoadOwner = newOwner;
            _currentLongestRoadLength = newLength;
            newOwner.AddVictoryPoints(2);
            
            Debug.Log($"{newOwner.name} a primit cardul 'Longest Road' (+2 puncte) cu un drum de {newLength}");
        }
        
        private int CalculateLongestRoad(PlayerBase player)
        {
            var playerRoads = player.Paths.Where(p => p.IsBuilt).ToList();
            
            if (playerRoads.Count == 0)
                return 0;
            
            int maxLength = 0;
            
            // incercam sa gasim cel mai lung drum pornind din fiecare capat posibil
            foreach (var startRoad in playerRoads)
            {
                var visited = new HashSet<PathController>();
                int length = DFSLongestPath(startRoad, player, visited);
                maxLength = Mathf.Max(maxLength, length);
            }
            
            return maxLength;
        }
        
        private int DFSLongestPath(PathController currentRoad, PlayerBase player, HashSet<PathController> visited)
        {
            visited.Add(currentRoad);
            
            int maxLength = 1; // drumul curent
            
            // gasim toate drumurile conectate la drumul curent
            var connectedRoads = GetConnectedRoads(currentRoad, player);
            
            foreach (var connectedRoad in connectedRoads)
            {
                if (!visited.Contains(connectedRoad))
                {
                    int pathLength = 1 + DFSLongestPath(connectedRoad, player, visited);
                    maxLength = Mathf.Max(maxLength, pathLength);
                }
            }
            
            visited.Remove(currentRoad);
            return maxLength;
        }
        
        private List<PathController> GetConnectedRoads(PathController road, PlayerBase player)
        {
            var connectedRoads = new List<PathController>();
            var roadEndpoints = GetExactEdgeEndpoints(road.HexPosition, road.EdgeDir);
            
            // pentru fiecare capat al drumului
            foreach (var (vertexPos, vertexDir) in roadEndpoints)
            {
                // verificam daca la acest vertex se conecteaza alte drumuri ale playerului
                foreach (var otherRoad in player.Paths)
                {
                    if (otherRoad != road && otherRoad.IsBuilt)
                    {
                        var otherEndpoints = GetExactEdgeEndpoints(otherRoad.HexPosition, otherRoad.EdgeDir);
                        
                        foreach (var (otherVertexPos, otherVertexDir) in otherEndpoints)
                        {
                            if (vertexPos.X == otherVertexPos.X && 
                                vertexPos.Y == otherVertexPos.Y && 
                                vertexDir == otherVertexDir)
                            {
                                // verificam daca la acest vertex NU exista o asezare a altui jucator
                                // (asezarile intrerup drumul pentru calculul longest road)
                                if (!HasOtherPlayerSettlement(vertexPos, vertexDir, player))
                                {
                                    connectedRoads.Add(otherRoad);
                                }
                            }
                        }
                    }
                }
            }
            
            return connectedRoads;
        }
        
        private bool HasOtherPlayerSettlement(HexPosition vertexPos, HexVertexDir vertexDir, PlayerBase player)
        {
            var boardController = FindObjectOfType<BoardController>();
            var vertex = boardController.BoardGrid.GetVertex(vertexPos, vertexDir);
            
            if (vertex is SettlementController settlement && settlement.HasOwner)
            {
                // drumul se intrerupe DOAR daca asezarea apartine unui ALT jucator
                return settlement.Owner != player;
            }
            
            return false;
        }
        
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
        
        public int GetPlayerLongestRoad(PlayerBase player)
        {
            return CalculateLongestRoad(player);
        }
        
        public PlayerBase GetCurrentLongestRoadOwner()
        {
            return _currentLongestRoadOwner;
        }
        
        public int GetCurrentLongestRoadLength()
        {
            return _currentLongestRoadLength;
        }
    }
}