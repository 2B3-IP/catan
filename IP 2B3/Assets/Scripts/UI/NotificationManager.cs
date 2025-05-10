using System;
using System.Collections;
using JetBrains.Annotations;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace B3.UI
{
    public class NotificationManager : MonoBehaviour
    {
        private static NotificationManager _instance = null;
        [CanBeNull]
        public static NotificationManager Instance
        {
            get
            {
                if (_instance == null) Debug.LogError("NotificationManager has not been added to a scene");
                return _instance;
            }
        }

        void Awake() {
            if (_instance != null) {
                Destroy(transform.parent.gameObject);
                transform.parent.gameObject.SetActive(false);
            }
            else
            {
                DontDestroyOnLoad(transform.parent);
                _instance = this;
            }
        }
     
        [SerializeField] private GameObject notificationPrefab;
        [SerializeField] private float animDuration;
        
        // btw: https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/RichText.html
        public void AddNotification(string message, float durationSeconds)
        {
            if (durationSeconds < 2f)
                Debug.LogWarning("Notification duration should be at least 2 seconds so the animation displays properly");
            
            var notification = (GameObject) Instantiate(notificationPrefab, transform);
            var notificationInstance = notification.GetComponent<NotificationInstance>();
            
            notificationInstance.text.text = message;
            
            LeanTween.scale(notification, Vector3.one, animDuration).setFrom(Vector3.zero)
                .setEase(LeanTweenType.easeOutCubic);
            LeanTween.alphaCanvas(notificationInstance.canvasGroup, 1f, animDuration).setFrom(0f).setEase(LeanTweenType.easeOutCubic);
            
            LeanTween.alphaCanvas(notificationInstance.canvasGroup, 0.5f, 0.5f).setFrom(1)
                .setLoopPingPong(4)
                .setDelay(durationSeconds - 2f)
                .setOnComplete(() => notificationInstance.DestroyNotification() );
            
        }
        
        [Button]
        void TestNotification()
        {
            AddNotification("<color=red> RED </color> built a road", 5f);
        }
    }
}