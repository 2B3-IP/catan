using UnityEngine;

namespace B3.Utility
{
    internal sealed class DestroyOnAwake : MonoBehaviour
    {
        #if UNITY_EDITOR
            private void Awake() =>
                Destroy(gameObject);
        #endif
    }
}