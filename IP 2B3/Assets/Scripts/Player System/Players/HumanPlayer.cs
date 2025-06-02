using System.Collections;
using System.Collections.Generic;
using System.Linq;
using B3.BoardSystem;
using B3.BuildingSystem;
using B3.DiceSystem;
using B3.GameStateSystem;
using B3.PieceSystem;
using B3.PlayerSystem.UI;
using B3.PortSystem;
using B3.SettlementSystem;
using B3.ThiefSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using B3.UI;
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
        [SerializeField] private DiceThrower diceThrower;
        
        private readonly RaycastHit[] _hits = new RaycastHit[5];
        
        private RaycastHit _closestHit;
        private Camera _playerCamera;
        private bool _hasInputClicked, _hasEndClicked, _hasDiceClick;

        protected override void Awake()
        {
            base.Awake();
            _playerCamera = Camera.main;
        }

        private void OnEnable()
        {
            clickButton.action.performed += OnClickPerformed;
            UIDiceButton.OnButtonClick += OnDiceButtonClick;
            UIEndButton.OnButtonClick += OnEndPlayerButtonPress;
        }

        private void OnDisable()
        {
            clickButton.action.performed -= OnClickPerformed;
            UIDiceButton.OnButtonClick -= OnDiceButtonClick;
            UIEndButton.OnButtonClick -= OnEndPlayerButtonPress;
        }
        
        public override IEnumerator ThrowDiceCoroutine()
        {
            _hasDiceClick = false;
            var instructionNotif = NotificationManager.Instance
                .AddNotification("Click on the dice button", float.PositiveInfinity, false);
            while (!_hasDiceClick)
                yield return null;


            yield return diceThrower.ThrowCoroutine(); 
            DiceSum = 7;
            while(DiceSum==7)
                DiceSum = Random.Range(1, 7) + Random.Range(1,7); // Simulate a dice roll for the sake of example
            AI.SendDice(DiceSum);
            
            _hasDiceClick = false;
            instructionNotif.Destroy();
        }

        public override IEnumerator MoveThiefCoroutine(ThiefControllerBase thiefController)
        {
            SelectedThiefPiece = null;
            var instructionNotif = NotificationManager.Instance
                .AddNotification("Select a tile to move the thief to", float.PositiveInfinity, false);
            while (SelectedThiefPiece == null)
            {
                yield return RayCastCoroutine(pieceLayerMask);
                SelectedThiefPiece = _closestHit.transform.GetComponentInParent<PieceController>();

                if (SelectedThiefPiece.IsBlocked || SelectedThiefPiece.GetComponentInParent<PortController>())
                {
                    SelectedThiefPiece = null;
                }
            }
            instructionNotif.Destroy();
        }

        public override void OnTradeAndBuildUpdate()
        {
           // Debug.Log("waiting to end");
            if(!_hasEndClicked)
                return;
            Debug.Log("buton apasat");
            AI.SendMove("END_TURN");    
            IsTurnEnded = true;
            _hasEndClicked = false;
        }

        public override IEnumerator BuildHouseCoroutine()
        {
            SelectedHouse = null;
            var instructionNotif = NotificationManager.Instance
                .AddNotification("Select a vertex to build a house", float.PositiveInfinity, false);
            while (SelectedHouse == null)
            {
                yield return RayCastCoroutine(pieceLayerMask);
                var pieceController = _closestHit.transform.GetComponentInParent<PieceController>();

                var hexPosition = pieceController.HexPosition;
                SelectedHouse = GetClosestCorner(hexPosition, _closestHit.point);

                if (SelectedHouse == null)
                {
                    Debug.Log("null house");
                    NotificationManager.Instance
                        .AddNotification("Invalid position for a house", 5, true);
                }
                else Debug.Log("selected " + SelectedHouse.Owner?.name + " " + SelectedHouse?.name);

                if (SelectedHouse != null && SelectedHouse.Owner != null)
                {
                    NotificationManager.Instance
                        .AddNotification("There is already a house in that vertex", 5, true);
                    SelectedHouse = null;
                }
            }
            instructionNotif.Destroy();
        }
        
        public override IEnumerator BuildRoadCoroutine()
        {
            SelectedPath = null;
            var instructionNotif = NotificationManager.Instance
                .AddNotification("Select an edge to build a road to", float.PositiveInfinity, false);
            while (SelectedPath == null)
            {
                yield return RayCastCoroutine(pieceLayerMask);
                var pieceController = _closestHit.transform.GetComponentInParent<PieceController>();

                var hexPosition = pieceController.HexPosition;
                SelectedPath = GetClosestEdge(hexPosition, _closestHit.point);
                if (SelectedPath == null)
                {
                    NotificationManager.Instance
                        .AddNotification("Invalid position for a road", 5, true);
                }
                if (SelectedPath != null && SelectedPath.IsBuilt)
                {
                     NotificationManager.Instance
                        .AddNotification("There is a road already there", 5, true);
                    SelectedPath = null;
                    
                }
            }
            instructionNotif.Destroy();
        }
        
        public override IEnumerator UpgradeToCityCoroutine()
        { 
            SelectedHouse = null;
            var instructionNotif = NotificationManager.Instance
                .AddNotification("Select a house to upgrade to city", float.PositiveInfinity, false);
            while (SelectedHouse == null)
            {
                yield return RayCastCoroutine(settlementLayerMask);
                SelectedHouse = _closestHit.transform.GetComponentInParent<SettlementController>();
                Debug.Log(SelectedHouse, SelectedHouse);
                Debug.Log(SelectedHouse != null && (SelectedHouse.IsCity || SelectedHouse.Owner != this));
                if (SelectedHouse != null && (SelectedHouse.IsCity || SelectedHouse.Owner != this))
                {
                     NotificationManager.Instance
                        .AddNotification("Can't build a city there", 5, true);
                    SelectedHouse = null;
                }
                    
            }
            instructionNotif.Destroy();
        }
        
        public override IEnumerator DiscardResourcesCoroutine(float timeout)
        {
            int resourcesToDiscard = TotalResources() / 2;
            DiscardMenu discardMenu = FindObjectOfType<DiscardMenu>(true);
            bool isComplete = false;

            DiscardResources = null;
            
            discardMenu.Initialize(resourcesToDiscard, (selectedResources) => 
            {
                DiscardResources = selectedResources; 
                isComplete = true;
                discardMenu.gameObject.SetActive(false); 
            });
    
            yield return new WaitUntil(() => isComplete);         
        }
        
        private IEnumerator RayCastCoroutine(LayerMask layerMask)
        {
            _hasInputClicked = false;
            int hitCount = 0;
            Debug.Log("raycasting");
            while(hitCount == 0)
            {
                while (!_hasInputClicked)
                {
                    //Debug.Log("waiting");
                    yield return null;
                }

                _hasInputClicked = false;
                
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
        
        private void OnClickPerformed(InputAction.CallbackContext context) =>
            _hasInputClicked = context.ReadValueAsButton();
        
        private void OnDiceButtonClick() =>
            _hasDiceClick = true;

        private void OnEndPlayerButtonPress() 
        {
            Debug.Log("Button pressed");
            _hasEndClicked = true;
        }
    }
}