using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    internal sealed class test : MonoBehaviour
    {
        public static event Action<int, int, int> onTest; // sfera1 sfera2
        
        [SerializeField] private Transform obje;
        [SerializeField] private test2 tt;

        [ContextMenu("Disable")]
        private void Disable()
        {
            tt.gameObject.SetActive(!tt.gameObject.activeInHierarchy);
        }
        
        [ContextMenu("Change")]
        private void ChangeColor()
        {
            onTest?.Invoke(6, 4, 3);
            /*
            foreach (var sfera)
            {
                sfera.ChangeMaterial(6, 4, 3)
            }*/
        }
        
        private IEnumerator Coroutine(int a)
        {
            // code 1
            
            yield return Coroutine2();
            yield break; // <=> return
            // code 2
        }
        
        private IEnumerator Coroutine2()
        {
            // code 1
            yield return new WaitForSeconds(6);
            // code 2
        }

        
        private void OnMouseDown()
        {
            Debug.Log("down");
        }
    }
}