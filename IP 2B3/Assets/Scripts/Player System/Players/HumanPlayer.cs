using System.Collections;
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
        
        
        private readonly RaycastHit[] _hits = new RaycastHit[5];
        private RaycastHit _closestHit;
        
        private const float CornerDistanceThreshold = 0.4f;
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

        public override IEnumerator MoveThiefCoroutine(ThiefController thiefController)
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
            yield return RayCastCoroutine();
            
            HexPosition hexPosition = boardController.BoardGrid.FromWorldPosition(_closestHit.point);
            
            var hexCenter=boardController.BoardGrid.ToWorldPosition(hexPosition);

            this.ClosestCorner = GetClosestCorner(hexCenter, _closestHit.point, boardController.BoardGrid.DistanceFromCenter);
            
        }

        private IEnumerator RayCastCoroutine()
        {
            var action = clickButton.action;
            
            int hitCount = 0;
            while(hitCount == 0)
            {
                while(!action.WasPressedThisFrame())
                    yield return null;
                
                var ray = Camera.main.ScreenPointToRay(Mouse.current.position.value);
                hitCount = Physics.RaycastNonAlloc(ray, _hits, float.MaxValue, pieceLayerMask);
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

        private Vector2? GetClosestCorner(Vector2 center, Vector2 hitPoint, float radius)
        {
            var corners = GetHexCorners(center, radius);
            Vector2? closestCorner = null;
            float minDistance = float.MaxValue;

            foreach (var corner in corners)
            {
                float distance = Vector2.Distance(hitPoint, corner);
                if (distance <= CornerDistanceThreshold && distance < minDistance)
                {
                    minDistance = distance;
                    closestCorner = corner;
                }
            }

            return closestCorner;
        }
        private void OnPlayerEndButtonPress() =>
            IsTurnEnded = true;
        
        
    }
}