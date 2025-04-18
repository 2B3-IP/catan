using System.Collections;
using System.Collections.Generic;
using B3.ResourcesSystem;
using B3.SettlementSystem;
using UnityEngine;

namespace B3.PieceSystem
{
    public sealed class PieceController : MonoBehaviour
    {
        [field:SerializeField] public ResourceType ResourceType { get; private set; }
        [field:SerializeField] public bool IsBlocked { get; set; }
        [field:SerializeField] public Transform ThiefPivot { get; private set; }
        
        [SerializeField] private float spawnDuration = 1f;
        
        private Transform _transform;

        public List<SettlementController> Settlements { get; } = new();
        
        public int Number { get; set; }

        private void Awake() =>
            _transform = transform;

        public void OnSpawn(Vector3 endPosition)
        {
            // TODO(front): puteti inlocui cu dotween
            StartCoroutine(LerpPosition(endPosition, spawnDuration));
        }
        
        private IEnumerator LerpPosition(Vector3 endPosition, float duration)
        {
            float elapsedTime = 0f;
            var startPosition = _transform.position;

            while (elapsedTime < duration)
            {
                _transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                
                yield return null;
            }
            
            _transform.position = endPosition;
        }
    }
}