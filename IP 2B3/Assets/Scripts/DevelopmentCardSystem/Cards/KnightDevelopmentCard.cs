using System.Collections;
using B3.PieceSystem;
using B3.PlayerSystem;
using B3.ThiefSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace B3.DevelopmentCardSystem
{
    internal sealed class KnightDevelopmentCard : DevelopmentCardBase
    {
        [SerializeField] private ThiefController thiefController;
        [SerializeField] private LayerMask pieceLayerMask;
        [SerializeField] private InputActionReference clickButton;

        private  Camera _playerCamera;
        private RaycastHit[] _hits = new RaycastHit[5];

        public void Awake()
        {
            _playerCamera=Camera.main;
        }
        public override IEnumerator UseCard(PlayerBase player)
        {
            var action = clickButton.action;
            while (!action.WasPressedThisFrame())
                yield return null;

            PieceController pieceController;
            while (true)
            {
                Ray ray = _playerCamera.ScreenPointToRay(Mouse.current.position.value);
                int hitCount = Physics.RaycastNonAlloc(ray, _hits, float.MaxValue, pieceLayerMask);

                if(hitCount == 0)
                    continue;

                var closestHit = _hits[0];
                for (int i = 0; i < hitCount; i++)
                {
                    var hit = _hits[i];

                    if (closestHit.distance > hit.distance)
                        closestHit = hit;
                }

                pieceController = closestHit.transform.GetComponent<PieceController>();
                if (!pieceController.IsBlocked)
                    break;
            }

            var thiefPivot = pieceController.ThiefPivot;
            yield return thiefController.MoveThief(thiefPivot.position);
        }
    }
}