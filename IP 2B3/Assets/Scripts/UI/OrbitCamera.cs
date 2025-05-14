using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraOrbit : MonoBehaviour
    {
        public GameObject target;
        public float distance = 10.0f;
        
        public float xSpeed = 365.0f;
        public float ySpeed = 200.0f;
        public float scrollSpeed = 20.0f;

        [MinMaxSlider(0f, 90f)]
        public Vector2 yLimits;
        [MinMaxSlider(0f, 200f)]
        public Vector2 zoomLimits;

        private float x = 0.0f;
        private float y = 0.0f;

        void Start()
        {
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
            
            distance = Mathf.Clamp(distance, zoomLimits.x, zoomLimits.y);
            UpdateCameraPosition();
        }

        void UpdateCameraPosition()
        {
            if (!target) return;

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            transform.rotation = rotation;
            transform.position = target.transform.position;
            
            Camera.main.transform.localPosition = new Vector3(0, 0, -distance);
            Camera.main.transform.LookAt(target.transform.position);
        }

        void LateUpdate()
        {
            if (!target) return;

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                distance = Mathf.Clamp(distance - scroll * scrollSpeed, zoomLimits.x, zoomLimits.y);
                UpdateCameraPosition();
            }

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
                y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
                y = ClampAngle(y, yLimits.x, yLimits.y);
                
                UpdateCameraPosition();
            }
        }

        static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360f) angle += 360f;
            if (angle > 360f) angle -= 360f;
            return Mathf.Clamp(angle, min, max);
        }
    }