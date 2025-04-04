using UnityEngine;

namespace DefaultNamespace
{
    internal sealed class test2 : MonoBehaviour
    {
        private void OnEnable()
        {
            Debug.Log("enable");
            test.onTest += ChangeMaterial;
        }

        private void OnDisable()
        {
            Debug.Log("disable");
            test.onTest -= ChangeMaterial;
        }

        private new Renderer renderer;
        
        private void Start()
        {
            renderer = GetComponent<Renderer>();
        }

        private void ChangeMaterial(int arg1, int arg2, int arg3)
        {
            renderer.material.color = new Color(arg1 / 255f, arg2 / 255f, arg3 / 255f);
        }
    }
}