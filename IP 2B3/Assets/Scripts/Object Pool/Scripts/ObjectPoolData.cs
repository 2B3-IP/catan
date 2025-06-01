using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace TheBlindEye.ObjectPoolSystem
{
    public abstract class ObjectPoolData<T> : ScriptableObject where T : Component
    {
        [SerializeField] private T poolingObject;
        
        [SerializeField] private int defaultCapacity = 10;

        private ObjectPool<T> _objectPool;
        private readonly List<T> _polledObjects = new();

        private GameObject _parentObject;

        private void Awake() => SceneManager.sceneLoaded += OnSceneLoaded;
        private void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

        public T Get(Vector3 newPosition, Quaternion newRotation)
        {
            if (_objectPool is null)
                Initialize();
            
            var poolObject = _objectPool.Get();
            
            var transform = poolObject.transform;
            transform.position = newPosition; transform.rotation = newRotation;
            
            _polledObjects.Add(poolObject);
            return poolObject;

            void Initialize()
            {
                _objectPool = new ObjectPool<T>(OnPoolCreate, OnPoolGet, OnPoolRelease, OnPoolDestroy,
                    false, defaultCapacity);

                SceneManager.sceneLoaded += OnSceneLoaded;
            }
        }

        public void Release(T  poolObject)
        {
            _polledObjects.Remove(poolObject); _objectPool.Release(poolObject);
        }

        private T OnPoolCreate()
        {
            var poolObject = Instantiate(poolingObject);
            DontDestroyOnLoad(poolObject);

            return poolObject;
        }
        
        private void OnPoolGet(T poolObject) => poolObject.gameObject.SetActive(true);
        private void OnPoolRelease(T poolObject) => poolObject.gameObject.SetActive(false);
        private void OnPoolDestroy(T poolObject) => Destroy(poolObject.gameObject);

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            foreach (var polledObject in _polledObjects)
                _objectPool.Release(polledObject);
            
            _polledObjects.Clear();
        }
    }
}
