using System;
using UnityEngine;

namespace TheBlindEye.ObjectPoolSystem
{
    public abstract class PoolObject<T> : MonoBehaviour
    {
        private Action<T> OnObjectReturn;

        public void Register(Action<T> listener) => OnObjectReturn = listener;
        protected void Return(T objectReference) => OnObjectReturn.Invoke(objectReference);
    }
}