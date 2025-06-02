using System.Collections;
using UnityEngine;

namespace B3.ThiefSystem
{
    internal sealed class ThiefController : ThiefControllerBase
    {
        [SerializeField] private LeanTweenType easing;
        [SerializeField] private float animLength = 2f;

        private bool firstMove = true;
        
        public override IEnumerator MoveThief(Vector3 endPosition)
        {
            LeanTween.cancel(gameObject);

            if (firstMove)
            {
                firstMove = false;
                LeanTween.scale(gameObject, Vector3.one, animLength).setEase(easing);
            }
            
            LeanTween.moveX(gameObject, endPosition.x, animLength).setEase(easing);
            LeanTween.moveZ(gameObject, endPosition.z, animLength).setEase(easing);
            
            LeanTween.moveY(gameObject, endPosition.y + 5f, animLength / 2).setEase(easing)
                .setOnComplete(() => LeanTween.moveY(gameObject, endPosition.y, animLength / 2).setEase(easing));
            
            yield break;
        }
    }
}