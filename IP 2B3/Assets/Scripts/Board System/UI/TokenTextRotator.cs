using System;
using UnityEngine;

namespace B3.BoardSystem.UI
{
    public class TokenTextRotator : MonoBehaviour
    {
        private Camera cam;

        public void Start()
        {
            cam = Camera.main;
            if (cam == null) Destroy(this);
        }
        
        public void LateUpdate()
        {
            transform.rotation = Quaternion.Euler(90f, cam.transform.parent.eulerAngles.y, 0f);
        }
    }
}