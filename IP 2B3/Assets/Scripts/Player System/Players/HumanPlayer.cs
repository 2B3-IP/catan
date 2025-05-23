﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B3.BoardSystem;
using B3.BuildingSystem;
using B3.GameStateSystem;
using B3.PieceSystem;
using B3.PlayerSystem.UI;
using B3.SettlementSystem;
using B3.ThiefSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace B3.PlayerSystem
{
    public sealed class HumanPlayer : PlayerBase
    {
        private const float CORNER_DISTANCE_THRESHOLD = 2f;
        private const float EDGE_DISTANCE_THRESHOLD = 1f;
        
        [SerializeField] private InputActionReference throwForceButton;
        [SerializeField] private BoardController boardController;
        [SerializeField] private BuildingController buildingController;
        
        [SerializeField] private InputActionReference clickButton;
        [SerializeField] private LayerMask pieceLayerMask;
        [SerializeField] private int hitDistance = 200;
        
        private readonly RaycastHit[] _hits = new RaycastHit[5];
        
        private RaycastHit _closestHit;
        private Camera _playerCamera;
        private bool _hasClicked;

        protected override void Awake()
        {
            base.Awake();
            _playerCamera = Camera.main;
        }

        private void OnEnable()
        {
            UIEndPlayerButton.OnEndButtonPressed += OnPlayerEndButtonPress;
            clickButton.action.performed += OnClickPerformed;
        }

        private void OnDisable()
        {
            UIEndPlayerButton.OnEndButtonPressed -= OnPlayerEndButtonPress;
            clickButton.action.performed -= OnClickPerformed;
            UIDiceButton.OnButtonClick += OnDiceButtonClick;
            UIEndButton.OnButtonClick += OnEndPlayerButtonPress;
        }
        
        public override IEnumerator ThrowDiceCoroutine()
        {
            _hasClicked = false;

            while (!_hasClicked)
                yield return null;

            DiceSum = Random.Range(1,7)+Random.Range(1,7); 

            _hasClicked = false;
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
            if(!_hasClicked)
                return;
            
            IsTurnEnded = true;
            _hasClicked = false;
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
            _hasClicked = false;
            int hitCount = 0;
            while(hitCount == 0)
            {
                while (!_hasClicked)
                    yield return null;

                _hasClicked = false;
                
                var ray = _playerCamera.ScreenPointToRay(Mouse.current.position.value);
                hitCount = Physics.RaycastNonAlloc(ray, _hits, hitDistance, pieceLayerMask); 
            }

            _closestHit = _hits[0];
            for (int i = 1; i < hitCount; i++)
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
                if (distance <= CORNER_DISTANCE_THRESHOLD && distance < minDistance)
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
                if (distance <= EDGE_DISTANCE_THRESHOLD && distance < minDistance)
                {
                    minDistance = distance;
                    closestEdge = edge.Item1;
                }
            }
            
            return closestEdge;
        }

        private void OnPlayerEndButtonPress() =>
            IsTurnEnded = true;
        
        private void OnClickPerformed(InputAction.CallbackContext context) =>
            _hasClicked = context.ReadValueAsButton();
        
        private void OnDiceButtonClick() =>
            _hasClicked = true;

        private void OnEndPlayerButtonPress() =>
            _hasClicked = true;
    }
}