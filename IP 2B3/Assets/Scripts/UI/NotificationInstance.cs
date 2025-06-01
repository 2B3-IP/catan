using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace B3.UI
{
    public class NotificationInstance : MonoBehaviour
    {
        public TMP_Text text;
        public CanvasGroup canvasGroup;
        public Button button;

        public float animDuration = 1f;
        
        public void DestroyNotification()
        {
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, Vector3.zero, animDuration).setFrom(Vector3.one)
                .setEase(LeanTweenType.easeInCubic)
                .setOnComplete(() => Destroy(gameObject));
            LeanTween.alphaCanvas(canvasGroup, 0f, animDuration).setFrom(1f).setEase(LeanTweenType.easeInCubic);
        }
    }
}