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
        private RaycastHit _closestHit;
        
        private Camera _playerCamera;
        
        private const float CornerDistanceThreshold = 0.5f;
        private const float EdgeDistanceThreshold = 0.5f;

        private void Start() =>
            _playerCamera = Camera.main;

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
            ClosestEdge = null;

            while (ClosestEdge == null)
            {
                yield return RayCastCoroutine();

                var pieceController = _closestHit.transform?.GetComponentInParent<PieceController>();
                if (pieceController == null) yield break;

                var hexPosition = pieceController.HexPosition;
                ClosestEdge = GetClosestEdge(hexPosition, _closestHit.point);
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
                if (SelectedHouse.IsCity) SelectedHouse = null;
              
            }
      
       
            /*
            HexPosition hexPosition = boardController.BoardGrid.FromWorldPosition(_closestHit.point);
            var hexCenter=boardController.BoardGrid.ToWorldPosition(hexPosition);
            this.ClosestCorner = GetClosestCorner(hexCenter, _closestHit.point, boardController.BoardGrid.DistanceFromCenter);
         
            if (!ClosestCorner.HasValue)
            {
                Debug.Log("No corner detected.");
                yield break;
            }
            
           
           SettlementController settlement = null;
           // TODO: De implementat hexboard pt settlements
           
           var TempSettlements = GameObject.FindObjectsOfType<SettlementController>().ToList(); //Array temporar!
           foreach (var s in TempSettlements)
           {
               Vector2 sPosition2D = new Vector2(s.transform.position.x, s.transform.position.z);
               if (Vector2.Distance(sPosition2D, ClosestCorner.Value) < 0.1f)
               {
                   settlement = s;
                   break;
               }
           }

            if (settlement == null)
            {
                Debug.Log("No settlement found.");
                yield break;
            }

            if (settlement.Owner != this)
            {
                Debug.Log("Not your house.");
                yield break;
            }

            if (settlement.IsCity)
            {
                Debug.Log("Already a city.");
                yield break;
            }
            settlement.UpgradeToCity();*/
          

        }

        private IEnumerator RayCastCoroutine()
        {
            var action = clickButton.action;
            
            int hitCount = 0;
            while(hitCount == 0)
            {
                while (!action.WasPressedThisFrame())
                {
                    Debug.Log("wait");
                    yield return null;
                }
                
                var ray = _playerCamera.ScreenPointToRay(Mouse.current.position.value);
                hitCount = Physics.RaycastNonAlloc(ray, _hits, hitDistance, pieceLayerMask);
                Debug.Log("aaa: " + hitCount);
            }

            _closestHit = _hits[0];
            for (int i = 0; i < hitCount; i++)
            {
                var hit = _hits[i];
                if(_closestHit.distance > hit.distance)
                    _closestHit = hit;
            }
        }
        
        private Vector2[] GetHexCorners(Vector2 center, float radius)
        {
            Vector2[] corners = new Vector2[6];

            for (int i = 0; i < 6; i++)
            {
                float angleRad = Mathf.PI / 3 * i; 
                float x = center.x + radius * Mathf.Cos(angleRad);
                float y = center.y + radius * Mathf.Sin(angleRad);
                corners[i] = new Vector2(x, y);
            }

            return corners;
        }

        private SettlementController GetClosestCorner(HexPosition position, Vector3 hitPoint)
        {
            var boardGrid = boardController.BoardGrid;
            var corners = boardGrid.GetHexVertices(position);
            
            SettlementController closestCorner = null;
            float minDistance = float.MaxValue;

            foreach (var corner in corners)
            {
                var cornerPosition = boardGrid.GetHexCorner(corner.Item2, position);
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

        private Path GetClosestEdge(HexPosition hexPosition, Vector3 hitPoint)
        {
            Path closestEdge = null;

            var boardGrid = boardController.BoardGrid;
            float minDistance = float.MaxValue;

            for (int dir = 0; dir < 6; dir++)
            {
                var edgeDir = (HexEdgeDir)dir;
                var edge = boardGrid.GetEdge(hexPosition, edgeDir);
                if (edge == null) continue;

                int cornerAIndex = (dir + 5) % 6;
                int cornerBIndex = (dir + 1) % 6;

                var cornerA = boardGrid.GetHexCorner((HexVertexDir)cornerAIndex, hexPosition);
                var cornerB = boardGrid.GetHexCorner((HexVertexDir)cornerBIndex, hexPosition);

                Vector3 edgeCenter = (new Vector3(cornerA.x, 0, cornerA.y) + new Vector3(cornerB.x, 0, cornerB.y)) / 2f;
                float distance = Vector3.Distance(hitPoint, edgeCenter);

                if (distance <= EdgeDistanceThreshold && distance < minDistance)
                {
                    minDistance = distance;
                    closestEdge = edge;
                }
            }

            return closestEdge;
        }

        private void OnPlayerEndButtonPress() =>
            IsTurnEnded = true;
        
        
    }
}