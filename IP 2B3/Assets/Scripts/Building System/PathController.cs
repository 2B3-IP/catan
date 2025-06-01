using System;
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
            roadPrefab.rotation = Quaternion.Euler(0f, EdgeDir switch
            {
                HexEdgeDir.Top => 0f,
                HexEdgeDir.TopRight => 60f,
                HexEdgeDir.BottomRight => 120f,
                HexEdgeDir.Bottom => 0f,
                HexEdgeDir.BottomLeft => 60f,
                HexEdgeDir.TopLeft => 120f,
                _ => throw new ArgumentOutOfRangeException()
            }, 0f);
            
            LeanTween.scale(roadPrefab.gameObject, Vector3.one, animLength).setFrom(Vector3.zero).setEase(easing);
            
            LeanTween.moveLocalY(roadPrefab.gameObject, roadPrefab.position.y - 5f, animLength).setEase(easing);
            
            roadPrefab.gameObject.SetActive(true);
        }
        
        public override string ToString()
        {
            return $"Road[{HexPosition.X},{HexPosition.Y} {EdgeDir}]";
        }
    }
}