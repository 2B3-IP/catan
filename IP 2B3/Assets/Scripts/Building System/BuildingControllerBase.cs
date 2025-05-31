using System.Collections;
using B3.BoardSystem;
using B3.GameStateSystem;
using B3.PieceSystem;
using B3.PlayerSystem;
using B3.SettlementSystem;
using UnityEngine;

namespace B3.BuildingSystem
{
    public abstract class BuildingControllerBase : MonoBehaviour
    {
        private const int MAX_HOUSES_PER_PLAYER = 5;
        private const int MAX_CITIES_PER_PLAYER = 4;
        private const int MAX_ROADS_PER_PLAYER = 15;
        
        [SerializeField] protected BoardController boardController;
        
        public abstract IEnumerator BuildHouse(PlayerBase player); // ii arati playerului care sunt spatiile ramase libere
                                                                   // , dupa player alege unde vrea sa construiasca,
                                                                   // ii construiecti casa
        public abstract IEnumerator BuildRoad(PlayerBase player);
        public abstract IEnumerator BuildCity(PlayerBase player);

        protected abstract bool CanBuildHouse(SettlementController targetSettlement, PlayerBase player);
        protected abstract bool CanBuildRoad(PlayerBase player, PathController targetPath);
        

        protected bool CanBuildHouse(PlayerBase player)
        {
            var boardGrid = boardController.BoardGrid;
            bool canBuildHouse = false;
            foreach (var path in player.Paths)
            {
                var roadPosition = path.HexPosition;
                var roadDir = path.EdgeDir;

                var (roadVertex1, roadVertex2) = roadDir.GetVertexDirs();
                
                var settlement1 = boardGrid.GetVertex(roadPosition, roadVertex1);
                var settlement2 = boardGrid.GetVertex(roadPosition, roadVertex2);

                if (settlement1.Owner != null && CanBuildHouse(settlement1, player))
                {
                    canBuildHouse = true;
                    break;
                }
                if (settlement2.Owner != null && CanBuildHouse(settlement2, player))
                {
                    canBuildHouse = true;
                    break;   
                }
            }

            int housesCount = player.GetHousesCount();
            return housesCount < MAX_HOUSES_PER_PLAYER && canBuildHouse;
        }
        
        protected bool CanBuildCity(PlayerBase player)
        {
            int citiesCount = player.GetCitiesCount();
            if (citiesCount == player.Settlements.Count)
                return false;
            
            return citiesCount < MAX_CITIES_PER_PLAYER;
        }
        
        protected bool CanBuildRoad(PlayerBase player)
        {
            var boardGrid = boardController.BoardGrid;
            bool canBuildRoad = false;
            foreach (var settlement in player.Settlements)
            {
                int index = 0;
                var position = settlement.HexPosition;
                var roadVertex = settlement.VertexDir;
                var neighbouringVertices = boardGrid.GetNeighbouringVertices(position, roadVertex);  
                
                foreach (var (set, pos, dir) in neighbouringVertices)
                {
                    var edgeDir = index switch
                    {
                        0 => HexEdgeDirExt.GetHexDir(roadVertex, dir),
                        1 => HexEdgeDirExt.GetHexDir(dir, roadVertex),
                        _ => HexEdgeDirExt.GetHexDir(dir, roadVertex.GetVertexDirBasedOnStartDir(position, pos))
                    };
                    
                    var road = boardGrid.GetEdge(pos, edgeDir);
                    if (road.Owner == null)
                    {
                        canBuildRoad = true;
                        break;
                    }
                    index++;
                }
            }

            if (!canBuildRoad)
            {
                foreach (var path in player.Paths)
                {
                    var roadPosition = path.HexPosition;
                    var roadDir = path.EdgeDir;

                    var (roadVertex1, roadVertex2) = roadDir.GetVertexDirs();

                    var settlement1 = boardGrid.GetVertex(roadPosition, roadVertex1);
                    var settlement2 = boardGrid.GetVertex(roadPosition, roadVertex2);

                    canBuildRoad = CanBuildRoadNeighbouring(settlement1, boardGrid);
                    if (canBuildRoad)
                        break;
                    canBuildRoad = CanBuildRoadNeighbouring(settlement2, boardGrid);
                    if (canBuildRoad)
                        break;
                }
            }

            int roadsCount = player.Paths.Count;
            return roadsCount < MAX_ROADS_PER_PLAYER && canBuildRoad;
        }

        private bool CanBuildRoadNeighbouring(SettlementController settlement1,FullHexGrid<PieceController, SettlementController, PathController> boardGrid)
        {
            int index = 0;
            
            var position1 = settlement1.HexPosition;
            var roadVertex1 = settlement1.VertexDir;
            var neighbouringVertices = boardGrid.GetNeighbouringVertices(position1, roadVertex1);
            
            foreach (var (settlement, pos, dir) in neighbouringVertices)
            {
                var edgeDir = index switch
                {
                    0 => HexEdgeDirExt.GetHexDir(roadVertex1, dir),
                    1 => HexEdgeDirExt.GetHexDir(dir, roadVertex1),
                    _ => HexEdgeDirExt.GetHexDir(dir, roadVertex1.GetVertexDirBasedOnStartDir(position1, pos))
                };
                var road = boardGrid.GetEdge(pos, edgeDir);
                if (road.Owner == null)
                {
                    return true;
                }
                index++;
            }
            
            return false;
        }
    }
}