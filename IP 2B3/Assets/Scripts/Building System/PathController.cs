using B3.PlayerSystem;
using B3.BoardSystem;
using UnityEngine;

namespace B3.BuildingSystem
{
    public class PathController : MonoBehaviour 
    {
        [SerializeField] private Transform roadPrefab;
        
        [SerializeField] private LeanTweenType easing;
        [SerializeField] private float animLength = 2f;
        
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
            roadPrefab.position = new Vector3(transform.position.x, transform.position.y + 5f, transform.position.z);
            roadPrefab.localScale = Vector3.zero;
            
            LeanTween.scale(roadPrefab.gameObject, Vector3.one, animLength).setFrom(Vector3.zero);
            
            LeanTween.moveLocalY(roadPrefab.gameObject, roadPrefab.position.y - 5f, animLength);
            
            roadPrefab.gameObject.SetActive(true);
        }
        
        public override string ToString()
        {
            return $"Road[{HexPosition.X},{HexPosition.Y} {EdgeDir}]";
        }
    }
}