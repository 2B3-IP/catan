using System.Collections;
using UnityEngine;

namespace B3.PieceSystem
{
    internal sealed class PieceController : MonoBehaviour
    {
        [SerializeField] private float spawnDuration = 1f;
        
        private Transform _transform;

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