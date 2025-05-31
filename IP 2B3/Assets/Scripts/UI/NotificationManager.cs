using System;
using System.Collections;
using JetBrains.Annotations;
using NaughtyAttributes;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
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

        public class NotificationHandle
        {
            private NotificationInstance _instance;

            internal NotificationHandle(NotificationInstance instance)
            {
                _instance = instance;
            }

            public void SetText(string text)
            {
                _instance.text.text = text;
            }

            public void AddOnClickListener(UnityAction listener) => _instance.button.onClick.AddListener(listener);
            
            public void Destroy()
            {
                _instance?.DestroyNotification();
                _instance = null;
            }
        }
        
        // btw: https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/manual/RichText.html
        public NotificationHandle AddNotification(string message, float durationSeconds = 4, bool destroyOnClick = true)
        {
            if (durationSeconds < 2f)
                Debug.LogWarning("Notification duration should be at least 2 seconds so the animation displays properly");
            
            var notification = (GameObject) Instantiate(notificationPrefab, transform);
            var notificationInstance = notification.GetComponent<NotificationInstance>();
            
            notificationInstance.text.text = message;
            if (destroyOnClick)
                notificationInstance.button.onClick.AddListener(() => notificationInstance.DestroyNotification());
            
            LeanTween.scale(notification, Vector3.one, animDuration).setFrom(Vector3.zero)
                .setEase(LeanTweenType.easeOutCubic);
            LeanTween.alphaCanvas(notificationInstance.canvasGroup, 1f, animDuration).setFrom(0f).setEase(LeanTweenType.easeOutCubic);
            
            LeanTween.alphaCanvas(notificationInstance.canvasGroup, 0.5f, 0.5f).setFrom(1)
                .setLoopPingPong(4)
                .setDelay(durationSeconds - 2f)
                .setOnComplete(() => notificationInstance.DestroyNotification() );

            return new NotificationHandle(notificationInstance);
        }
        
        [Button]
        void TestNotification()
        {
            AddNotification("<color=red> RED </color> built a road", 5f);
        }
    }
}