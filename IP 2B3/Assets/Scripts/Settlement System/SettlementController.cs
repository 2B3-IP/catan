using B3.BoardSystem;
using B3.GameStateSystem;
using B3.PlayerSystem;
using B3.PortSystem;
using UnityEngine;

namespace B3.SettlementSystem
{
    public sealed class SettlementController : MonoBehaviour
    {
        [SerializeField] private GameObject houseObject;
        [SerializeField] private GameObject cityObject;
        [SerializeField] private Material highlightMaterial;
        
        [SerializeField] private LeanTweenType easing;
        [SerializeField] private float animLength = 2f;

        public PortController ConnectedPortController { get; set; }
        public HexPosition HexPosition { get; set; }
        public HexVertexDir VertexDir { get; set; }
        
        public static event System.Action<SettlementController> OnSettlementSelected;
        
        public PlayerBase Owner { get; set; }
        public bool IsCity { get; private set; }
        
        public bool HasOwner => Owner != null;

        public int ResourceAmount => IsCity ? 2 : 1;
        
        private bool _selectable = false;
        private Material _originalMaterial;
        private Renderer _renderer;
        
        private void Awake()
        {
            houseObject.SetActive(false);
            cityObject.SetActive(false);
            
            _renderer = houseObject.GetComponent<Renderer>();
            if (_renderer != null)
            {
                _originalMaterial = _renderer.material;
            }
        }
        
        private void OnMouseDown()
        {
            if (_selectable && Owner == null && !IsCity)
            {
                OnSettlementSelected?.Invoke(this);
            }
        }

        public void SetSelectable(bool value)
        {
            _selectable = value;
        }
        
        public void BuildHouse()
        {
            if (IsCity)
                return;
            
            houseObject.transform.position = new Vector3(transform.position.x, transform.position.y + 5f, transform.position.z);
            houseObject.transform.localScale = Vector3.zero;
            
            LeanTween.scale(houseObject, Vector3.one, animLength).setFrom(Vector3.zero);
            LeanTween.moveLocalY(houseObject, houseObject.transform.position.y - 5f, animLength);
            
            houseObject.SetActive(true);
            cityObject.SetActive(false);
        }
        
        public void Highlight(bool value)
        {
            if (_renderer == null || highlightMaterial == null) return;
            
            houseObject.SetActive(value);
            _renderer.material = value ? highlightMaterial : _originalMaterial;
        }
        
        public void AllowSelection(bool value)
        {
            _selectable = value;
        }

        public void UpgradeToCity()
        {
            if (IsCity) 
                return;
            
            IsCity = true;
            
            LeanTween.rotateAroundLocal(houseObject, Vector3.up, 360f, animLength);
            LeanTween.scale(houseObject, Vector3.zero, animLength).setFrom(Vector3.one).setOnComplete(() =>
                {
                    LeanTween.rotateAroundLocal(cityObject, Vector3.up, 360f, animLength * 1.5f);
                    LeanTween.scale(cityObject, Vector3.one, animLength * 1.5f).setFrom(Vector3.zero);
                    houseObject.SetActive(false);
                    cityObject.SetActive(true);
                } 
            );
            
        }
    }
}