using System;
using B3.ResourcesSystem;
using UnityEngine;
using UnityEngine.UI;

namespace B3.GameStateSystem
{
    public class UISelectResource : MonoBehaviour
    {
        [SerializeField] private  Button selectResourceButton;
        [SerializeField] private ResourceType selectedResourceType;
        
        public static event Action<ResourceType> OnSelectResource;

        private void Awake()
        {
            selectResourceButton.onClick.AddListener(SelectResourceType);
        }

        private void SelectResourceType()
        {
            OnSelectResource?.Invoke(selectedResourceType);
        }
        
    }
}