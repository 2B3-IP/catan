using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B3.BoardSystem;
using B3.BuildingSystem;
using B3.GameStateSystem;
using B3.PieceSystem;
using B3.SettlementSystem;
using B3.ThiefSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace B3.PlayerSystem
{
    public sealed class HumanPlayer : PlayerBase
    {
        [SerializeField] private InputActionReference throwForceButton;
        [SerializeField] private BoardController boardController;
        [SerializeField] private BuildingController buildingController;
        
        [SerializeField] private InputActionReference clickButton;
        [SerializeField] private LayerMask pieceLayerMask;
        [SerializeField] private int hitDistance = 200;
        
        private readonly RaycastHit[] _hits = new RaycastHit[5];
        private readonly WaitForEndOfFrame _waitForEndFrame = new();
        
        private RaycastHit _closestHit;
        private Camera _playerCamera;
        
        private const float CornerDistanceThreshold = 0.5f;
        private const float EdgeDistanceThreshold = 0.5f;

        protected override void Awake()
        {
            base.Awake();
            _playerCamera = Camera.main;
        }

        private void OnEnable() =>
            UIEndPlayerButton.OnEndButtonPressed += OnPlayerEndButtonPress;
        
        private void OnDisable() =>
            UIEndPlayerButton.OnEndButtonPressed -= OnPlayerEndButtonPress;
        
        public override IEnumerator DiceThrowForceCoroutine()
        {
            var action = throwForceButton.action;
            
            while(!action.WasPressedThisFrame())
                yield return null;

            float throwForce = 0f;
            
            while (action.WasPressedThisFrame())
                throwForce += Mathf.Clamp(Time.fixedDeltaTime, MIN_DICE_THROW_FORCE, MAX_DICE_THROW_FORCE);
            
            DiceThrowForce = throwForce;
        }

        public override IEnumerator MoveThiefCoroutine(ThiefControllerBase thiefController)
        {
            yield return RayCastCoroutine();
            
            var pieceController = _closestHit.transform.GetComponent<PieceController>();
            SelectedThiefPiece = pieceController;
            
            var thiefPivot = pieceController.ThiefPivot;
            
            yield return thiefController.MoveThief(thiefPivot.position);
        }

        public override void OnTradeAndBuildUpdate()
        {
        }

        public override IEnumerator BuildHouseCoroutine()
        {
            SelectedHouse = null;
            
            while (SelectedHouse == null)
            {
                yield return RayCastCoroutine();
                var pieceController = _closestHit.transform.GetComponentInParent<PieceController>();

                var hexPosition = pieceController.HexPosition;
                SelectedHouse = GetClosestCorner(hexPosition, _closestHit.point);

                if (SelectedHouse != null && SelectedHouse.Owner != null)
                    SelectedHouse = null;
            }
        }
        
        public override IEnumerator BuildRoadCoroutine()
        {
            SelectedPath = null;

            while (SelectedPath == null)
            {
                yield return RayCastCoroutine();

                var pieceController = _closestHit.transform.GetComponentInParent<PieceController>();

                var hexPosition = pieceController.HexPosition;
                SelectedPath = GetClosestEdge(hexPosition, _closestHit.point);

                if (SelectedPath != null && SelectedPath.IsBuilt)
                    SelectedPath = null;
            }
        }
        
        public override IEnumerator UpgradeToCityCoroutine()
        { 
            SelectedHouse = null;
            
            while (SelectedHouse == null)
            {
                yield return RayCastCoroutine();
                var pieceController = _closestHit.transform.GetComponentInParent<PieceController>();

                var hexPosition = pieceController.HexPosition;
                SelectedHouse = GetClosestCorner(hexPosition, _closestHit.point);
                
                if (SelectedHouse != null && SelectedHouse.IsCity) 
                    SelectedHouse = null;
            }
        }

        private IEnumerator RayCastCoroutine()
        {
            var action = clickButton.action;
            
            int hitCount = 0;
            while(hitCount == 0)
            {
                // Debug.Log("aaa");
                while (!action.WasPressedThisFrame())
                {
                    // Debug.Log("wait");
                    yield return null;
                }

                var ray = _playerCamera.ScreenPointToRay(Mouse.current.position.value);
                hitCount = Physics.RaycastNonAlloc(ray, _hits, hitDistance, pieceLayerMask);
                // Debug.Log("aaa: " + hitCount);
            }

            _closestHit = _hits[0];
            for (int i = 0; i < hitCount; i++)
            {
                var hit = _hits[i];
                if(_closestHit.distance > hit.distance)
                    _closestHit = hit;
            }
        }
        
        private SettlementController GetClosestCorner(HexPosition hexPosition, Vector3 hitPoint)
        {
            var boardGrid = boardController.BoardGrid;
            var corners = boardGrid.GetHexVertices(hexPosition);
            
            SettlementController closestCorner = null;
            float minDistance = float.MaxValue;

            foreach (var corner in corners)
            {
                var cornerPosition = boardGrid.GetHexCorner(corner.Item2, hexPosition);
                var settlementPosition = new Vector3(cornerPosition.x, 0, cornerPosition.y);
                
                float distance = Vector3.Distance(hitPoint, settlementPosition);
                if (distance <= CornerDistanceThreshold && distance < minDistance)
                {
                    minDistance = distance;
                    closestCorner = corner.Item1;
                }
            }

            return closestCorner;
        }

        private PathController GetClosestEdge(HexPosition hexPosition, Vector3 hitPoint)
        {
            var boardGrid = boardController.BoardGrid;
            var edges = boardGrid.GetHexEdges(hexPosition);
            
            PathController closestEdge = null;
            float minDistance = float.MaxValue;
            
            foreach (var edge in edges)
            {
                var edgePosition = boardGrid.GetHexEdge(edge.Item2, hexPosition);
                var pathPosition = new Vector3(edgePosition.x, 0, edgePosition.y);
                
                float distance = Vector3.Distance(hitPoint, pathPosition);
                if (distance <= EdgeDistanceThreshold && distance < minDistance)
                {
                    minDistance = distance;
                    closestEdge = edge.Item1;
                }
            }
            
            return closestEdge;
        }

        private void OnPlayerEndButtonPress() =>
            IsTurnEnded = true;
        
        
    }
}