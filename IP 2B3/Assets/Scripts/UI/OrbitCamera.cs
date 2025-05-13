using UnityEngine;

    public class CameraOrbit : MonoBehaviour
    {
        public GameObject target;
        public float distance = 10.0f;
        public float minDistance = 25.0f;
        public float maxDistance = 50.0f;
        
        public float xSpeed = 365.0f;
        public float ySpeed = 200.0f;
        
        public float yMinLimit = -20f;
        public float yMaxLimit = 80f;

        private float x = 0.0f;
        private float y = 0.0f;

        void Start()
        {
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
            
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
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
                distance = Mathf.Clamp(distance - scroll * 10f, minDistance, maxDistance);
                if (distance > maxDistance)
                {
                    distance = maxDistance;
                }
                else if (distance < minDistance)
                {
                    distance = minDistance;
                }
                UpdateCameraPosition();
            }

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
                y -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime;
                y = ClampAngle(y, yMinLimit, yMaxLimit);
                
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