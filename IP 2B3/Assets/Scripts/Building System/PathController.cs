using B3.PlayerSystem;
using B3.BoardSystem;
using UnityEngine;

namespace B3.BuildingSystem
{
    public class PathController : MonoBehaviour 
    {
        [SerializeField] private Transform roadPrefab;
        
        public PlayerBase Owner { get; set; }
        public bool IsBuilt { get; set; } = false; // Schimbă din private set în set
        
        public HexPosition HexPosition { get; set; }
        public HexEdgeDir EdgeDir { get; set; }

        private void Awake()
        {
            roadPrefab.gameObject.SetActive(false);
        }

        public void BuildRoad()
        {
            if (IsBuilt)
                return;
            
            IsBuilt = true;
            roadPrefab.gameObject.SetActive(true);
        }
        
        public override string ToString()
        {
            return $"Road[{HexPosition.X},{HexPosition.Y} {EdgeDir}]";
        }
    }
}