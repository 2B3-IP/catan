using System;
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
        [SerializeField] private BoardController boardController;
        
        private PlayerBase _currentLongestRoadOwner;
        private int _currentLongestRoadLength = 4; // minim 5 pentru a primi cardul
        
        public void CheckLongestRoadAfterBuild(PlayerBase player, PathController placedRoad)
        {
            Debug.Log("========[INLONGESTROAD]=========");
            int playerLongestRoad = CalculateLongestRoad(player, placedRoad);
            Debug.Log("LONGERS ROAD: " + playerLongestRoad);
            
            // daca jucatorul curent inca are cel mai lung drum, verificam daca si-a marit drumul
            if (_currentLongestRoadOwner == player && playerLongestRoad > _currentLongestRoadLength)
            {
                _currentLongestRoadLength = playerLongestRoad;
                Debug.Log($"{player.name} si-a marit drumul cel mai lung la {playerLongestRoad}");
            }
            // verificam daca jucatorul are drumul cel mai lung (minim 5)
            else if (playerLongestRoad >= 5 && playerLongestRoad > _currentLongestRoadLength)
            {
                // transferam cardul la noul jucator
                TransferLongestRoadCard(player, playerLongestRoad);
            }

            player.LongestRoad = playerLongestRoad;
            
            Debug.Log("========[OUTLONGESTROAD]=========");
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
        
        private int CalculateLongestRoad(PlayerBase player, PathController placedRoad)
        {
            var playerRoads = player.Paths.Where(p => p.IsBuilt).ToList();
            if (playerRoads.Count == 0)
                return 0;
            
            // incercam sa gasim cel mai lung drum pornind din fiecare capat posibil
            int length = DFSLongestPath(placedRoad, player);
            
            return length;
        }
        
        private int DFSLongestPath(PathController currentRoad, PlayerBase player)
        {
            var roadPosition = currentRoad.HexPosition;
            var roadDir = currentRoad.EdgeDir;
            
            var boardGrid = boardController.BoardGrid;
            var (roadVertex1, roadVertex2) = roadDir.GetVertexDirs();
            Debug.Log("=====================DFS=======================");
            var vertex1 = boardGrid.GetVertex(roadPosition, roadVertex1);
            var vertex2 = boardGrid.GetVertex(roadPosition, roadVertex2);

            Debug.Log("VERTEX1", vertex1);
            Debug.Log("VERTEX2", vertex2);
            
            visited = new HashSet<SettlementController>{vertex1,vertex2};
            int length1 = TraverseVertex(player, vertex1, vertex1, vertex2);
            int length2 = TraverseVertex(player, vertex2, vertex1, vertex2);
            Debug.Log("LENGTH: " + length1 + " " + length2);
            return length1 + length2 + 1;
        }
        private HashSet<SettlementController> visited;

        private int TraverseVertex(PlayerBase player, SettlementController currentSettlement, SettlementController v1 = null, SettlementController v2 = null)
        {
            Debug.Log("=================TRAVERSARE===========================");
            Debug.Log("VERTEX tra", currentSettlement);

            var boardGrid = boardController.BoardGrid;

            var vertex = currentSettlement.VertexDir;
            var position = currentSettlement.HexPosition;

            var neighbouringVertices =
                boardGrid.GetNeighbouringVertices(position, vertex);

            int maxLength = 0;

            int index = 0;
            
            foreach (var (settlement, pos, dir) in neighbouringVertices)
            {
                if (visited.Contains(settlement))
                {
                    Debug.Log("VERTEX deja vizitat", settlement);
                    continue;
                }
                Debug.Log("VERTEX dupa if", settlement);

                var edgeDir = index switch
                {
                    1 => HexEdgeDirExt.GetHexDir(vertex.GetVertexDirBasedOnStartDir(position, pos), dir),
                    _ => HexEdgeDirExt.GetHexDir(dir, vertex)
                };

                index++;

                var path = boardGrid.GetEdge(pos, edgeDir);
                if (path == null || path.Owner != player)
                    continue;
                
                visited.Add(settlement);
                Debug.Log("VERTEX adaugat", settlement);
                int length = 1 + TraverseVertex(player, settlement, v1, v2);

                if (maxLength < length)
                    maxLength = length;
            }
            
            Debug.Log("MAX LENGTH: " + maxLength);
            return maxLength;
        }
    }
}