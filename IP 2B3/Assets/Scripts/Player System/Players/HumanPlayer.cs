using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B3.BoardSystem;
using B3.BuildingSystem;
using B3.GameStateSystem;
using B3.PieceSystem;
using B3.PlayerSystem.UI;
using B3.SettlementSystem;
using B3.ThiefSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace B3.PlayerSystem
{
    public sealed class HumanPlayer : PlayerBase
    {
        private const float CORNER_DISTANCE_THRESHOLD = 2f;
        private const float EDGE_DISTANCE_THRESHOLD = 1f;
        
        [SerializeField] private InputActionReference throwForceButton;
        [SerializeField] private BoardController boardController;
        [SerializeField] private BuildingController buildingController;
        
        [SerializeField] private InputActionReference clickButton;
        [SerializeField] private LayerMask pieceLayerMask;
        [SerializeField] private LayerMask settlementLayerMask;
        [SerializeField] private int hitDistance = 200;
        
        private readonly RaycastHit[] _hits = new RaycastHit[5];
        
        private RaycastHit _closestHit;
        private Camera _playerCamera;
        private bool _hasClicked;

        protected override void Awake()
        {
            base.Awake();
            _playerCamera = Camera.main;
        }

        private void OnEnable()
        {
            UIEndPlayerButton.OnEndButtonPressed += OnPlayerEndButtonPress;
            clickButton.action.performed += OnClickPerformed;
        }

        private void OnDisable()
        {
            UIEndPlayerButton.OnEndButtonPressed -= OnPlayerEndButtonPress;
            clickButton.action.performed -= OnClickPerformed;
            UIDiceButton.OnButtonClick += OnDiceButtonClick;
            UIEndButton.OnButtonClick += OnEndPlayerButtonPress;
        }
        
        public override IEnumerator ThrowDiceCoroutine()
        {
            _hasClicked = false;

            while (!_hasClicked)
                yield return null;

            DiceSum = 8;

            _hasClicked = false;
        }

        public override IEnumerator MoveThiefCoroutine(ThiefControllerBase thiefController)
        {
            yield return RayCastCoroutine(pieceLayerMask);
            
            var pieceController = _closestHit.transform.GetComponent<PieceController>();
            SelectedThiefPiece = pieceController;
            
            var thiefPivot = pieceController.ThiefPivot;
            
            yield return thiefController.MoveThief(thiefPivot.position);
        }

        public override void OnTradeAndBuildUpdate()
        {
            Debug.Log("waiting to end");
            if(!_hasClicked)
                return;
            Debug.Log("buton apasat");
            IsTurnEnded = true;
            _hasClicked = false;
        }

        public override IEnumerator BuildHouseCoroutine()
        {
            SelectedHouse = null;
            
            while (SelectedHouse == null)
            {
                yield return RayCastCoroutine(pieceLayerMask);
                var pieceController = _closestHit.transform.GetComponentInParent<PieceController>();

                var hexPosition = pieceController.HexPosition;
                SelectedHouse = GetClosestCorner(hexPosition, _closestHit.point);

                if(SelectedHouse == null) Debug.Log("null house");
                else Debug.Log("selected " + SelectedHouse.Owner?.name + " " + SelectedHouse?.name);
                if (SelectedHouse != null && SelectedHouse.Owner != null)
                    SelectedHouse = null;
            }
        }
        
        public override IEnumerator BuildRoadCoroutine()
        {
            SelectedPath = null;

            while (SelectedPath == null)
            {
                yield return RayCastCoroutine(pieceLayerMask);
                var pieceController = _closestHit.transform.GetComponentInParent<PieceController>();

                var hexPosition = pieceController.HexPosition;
                SelectedPath = GetClosestEdge(hexPosition, _closestHit.point);

                if (SelectedPath != null && SelectedPath.IsBuilt)
                    SelectedPath = null;
            }
        }
        
        public override IEnumerator UpgradeToCityCoroutine()
        { 
            SelectedHouse = null;
            
            while (SelectedHouse == null)
            {
                yield return RayCastCoroutine(settlementLayerMask);
                SelectedHouse = _closestHit.transform.GetComponentInParent<SettlementController>();
                Debug.Log(SelectedHouse, SelectedHouse);
                Debug.Log(SelectedHouse != null && (SelectedHouse.IsCity || SelectedHouse.Owner != this));
                if (SelectedHouse != null && (SelectedHouse.IsCity || SelectedHouse.Owner != this)) 
                    SelectedHouse = null;
            }
        }
        
        public override IEnumerator DiscardResourcesCoroutine(float timeout)
        {
            int total = TotalResources();
            if (total <= 7)
                yield break;

            int toDiscard = total / 2;
            
            bool playerChoseManually = false;
            
            float elapsed = 0f;
            while (elapsed < timeout && !playerChoseManually)
            {   // TODO: front - choose which resources to discard and set playerChoseManually truee
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (!playerChoseManually)
            {
                
                for (int i = 0; i < toDiscard;)
                {
                    int index = UnityEngine.Random.Range(0, Resources.Length);
                    if (Resources[index] > 0)
                    {
                        Resources[index]--;
                        i++;
                    }
                }
                
            }
           

            yield break;
        }
        
        private IEnumerator RayCastCoroutine(LayerMask layerMask)
        {
            _hasClicked = false;
            int hitCount = 0;
            Debug.Log("raycasting");
            while(hitCount == 0)
            {
                while (!_hasClicked)
                {
                    //Debug.Log("waiting");
                    yield return null;
                }

                _hasClicked = false;
                
                var ray = _playerCamera.ScreenPointToRay(Mouse.current.position.value);
                hitCount = Physics.RaycastNonAlloc(ray, _hits, hitDistance, layerMask); 
            }

            _closestHit = _hits[0];
            
            for (int i = 1; i < hitCount; i++)
            {
                var hit = _hits[i];
                if(_closestHit.distance > hit.distance)
                    _closestHit = hit;
            }
            Debug.Log("out of waiting " + _closestHit.transform.name, _closestHit.transform);
        }
        
        private SettlementController GetClosestCorner(HexPosition hexPosition, Vector3 hitPoint)
        {
            var boardGrid = boardController.BoardGrid;
            var corners = boardGrid.GetHexVertices(hexPosition);
            
            SettlementController closestCorner = null;
            float minDistance = float.MaxValue;
            
            foreach (var corner in corners)
            {
                var cornerPosition = boardGrid.GetHexCorner(corner.Item2, hexPosition);
                var settlementPosition = new Vector3(cornerPosition.x, 0, cornerPosition.y);
                
                float distance = Vector3.Distance(hitPoint, settlementPosition);
                if (distance <= CORNER_DISTANCE_THRESHOLD && distance < minDistance)
                {
                    minDistance = distance;
                    closestCorner = corner.Item1;
                }
            }

            return closestCorner;
        }

        private PathController GetClosestEdge(HexPosition hexPosition, Vector3 hitPoint)
        {
            var boardGrid = boardController.BoardGrid;
            var edges = boardGrid.GetHexEdges(hexPosition);
            
            PathController closestEdge = null;
            float minDistance = float.MaxValue;
            
            foreach (var edge in edges)
            {
                var edgePosition = boardGrid.GetHexEdge(edge.Item2, hexPosition);
                var pathPosition = new Vector3(edgePosition.x, 0, edgePosition.y);
                
                float distance = Vector3.Distance(hitPoint, pathPosition);
                if (distance <= EDGE_DISTANCE_THRESHOLD && distance < minDistance)
                {
                    minDistance = distance;
                    closestEdge = edge.Item1;
                }
            }
            
            return closestEdge;
        }

        private void OnPlayerEndButtonPress() =>
            IsTurnEnded = true;
        
        private void OnClickPerformed(InputAction.CallbackContext context) =>
            _hasClicked = context.ReadValueAsButton();
        
        private void OnDiceButtonClick() =>
            _hasClicked = true;

        private void OnEndPlayerButtonPress() 
        {
            Debug.Log("Button pressed");
            _hasClicked = true;
        }
    }
}