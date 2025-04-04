using System;
using System.Collections;
using UnityEngine;
using Utility.Language;
using Random = UnityEngine.Random;

namespace B3.DiceSystem
{
    internal sealed class DiceController : MonoBehaviour
    {
        private const float LAND_DELAY = 0.5f;
        private static readonly WaitForFixedUpdate WAIT_FIXED = new();
        
        [SerializeField] private Vector3 throwDirection = new(1f, 1f, 0f);
        [SerializeField] private float landThreshold = 0.1f;

        private Transform _transform;
        private Rigidbody _rigidbody;

        public int DiceRoll { get; private set; }


        private void Awake()
        {
            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
        }

        public IEnumerator ThrowCoroutine(Vector3 startPosition, float throwForce)
        {
            _transform.SetPositionAndRotation(startPosition, Random.rotation);
            _rigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);

            yield return WAIT_FIXED;
            
            while (_rigidbody.linearVelocity.magnitude > landThreshold || 
                   _rigidbody.angularVelocity.magnitude > landThreshold)
                yield return WAIT_FIXED;

            yield return LAND_DELAY.ToWait();

            SetDiceValue();
        }

        private void SetDiceValue()
        {
            Span<Vector3> faceDirection = stackalloc Vector3[6];
            faceDirection[0] = _transform.up;
            faceDirection[1] = _transform.forward;
            faceDirection[2] = _transform.right;
            faceDirection[3] = -faceDirection[2];
            faceDirection[4] = -faceDirection[1];
            faceDirection[5] = -faceDirection[0];

            float closestDot = -1f;
            int faceIndex = 0; 

            for(int i = 0; i < faceDirection.Length; i++)
            {
                float dotFromUp = Vector3.Dot(Vector3.up, faceDirection[i]);
                if(dotFromUp <= closestDot)
                    continue;
   
                closestDot = dotFromUp;
                faceIndex = i;
            }

            DiceRoll = faceIndex + 1;
            Debug.Log(DiceRoll);
        }
    }
}