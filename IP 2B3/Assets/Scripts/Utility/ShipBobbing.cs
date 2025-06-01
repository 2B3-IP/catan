using NaughtyAttributes;
using UnityEngine;

namespace Utility
{
    public class ShipBobbing : MonoBehaviour
    {
        public float height;
        public LeanTweenType easing;
        public float duration;
        
        private void Start()
        {
            LeanTween.moveLocalY(gameObject, transform.localPosition.y + height, duration)
                .setFrom(transform.localPosition.y)
                .setLoopPingPong(-1)
                .setEase(easing)
                .setDelay(Random.Range(0f, duration));
        }

        [Button]
        private void Reset()
        {
            LeanTween.cancel(gameObject);
            Start();
        }
    }
}