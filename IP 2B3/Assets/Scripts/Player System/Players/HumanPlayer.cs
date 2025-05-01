using System.Collections;
using B3.PieceSystem;
using B3.ThiefSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace B3.PlayerSystem
{
    internal sealed class HumanPlayer : PlayerBase
    {
        [SerializeField] private InputActionReference throwForceButton;
        
        [SerializeField] private InputActionReference clickButton;
        [SerializeField] private LayerMask pieceLayerMask;
        
        private readonly Camera _playerCamera = Camera.main;
        private readonly RaycastHit[] _hits = new RaycastHit[5];
        
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
            var action = clickButton.action;
            while(!action.WasPressedThisFrame())
                yield return null;

            int hitCount = 0;
            while(hitCount == 0)
            {
                var ray = _playerCamera.ScreenPointToRay(Mouse.current.position.value);
                hitCount = Physics.RaycastNonAlloc(ray, _hits, float.MaxValue, pieceLayerMask);
            }

            var closestHit = _hits[0];
            for (int i = 0; i < hitCount; i++)
            {
                var hit = _hits[i];
                
                if(closestHit.distance > hit.distance)
                    closestHit = hit;
            }
            
            var pieceController = closestHit.transform.GetComponent<PieceController>();
            SelectedThiefPiece = pieceController;
            
            var thiefPivot = pieceController.ThiefPivot;
            
            yield return thiefController.MoveThief(thiefPivot.position);
        }

        public override void OnTradeAndBuildUpdate()
        {
        }
    }
}