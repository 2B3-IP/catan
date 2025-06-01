using B3.BuildingSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;
using UnityEngine;
using System.Collections.Generic;
using B3.GameStateSystem;
using NaughtyAttributes;
using UnityEngine.UI;

public class DiscardMenu : MonoBehaviour
{
    [SerializeField] private PlayerBase humanPlayer;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private ResourceDiscard[] resources;
    private int[] _selectedResourcesArray = new int[5];
    
    private int _requiredToDiscard ;
    private int _totalSelected;
    private Dictionary<ResourceType, int> _selectedResources = new();
    private System.Action<int[]> _onCompleteCallback;
    private void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirmClicked);
        cancelButton.onClick.AddListener(OnCancelClicked);
        confirmButton.interactable = false;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        DiscardGameState.OnDiscardStateEnd += ForceClose;
    }
    
    private void OnDisable()
    {
        DiscardGameState.OnDiscardStateEnd -= ForceClose;
    }

    
    
    public void Initialize(int requiredToDiscard, System.Action<int[]> onCompleteCallback)
    {
        _requiredToDiscard = requiredToDiscard;
        _totalSelected = 0;
        _selectedResources.Clear();
        
        _onCompleteCallback = onCompleteCallback;
        
        for (int i = 0; i < 5; i++)
        {
            _selectedResourcesArray[i] = 0;
        }

        for (int i = 0; i < resources.Length; i++)
        {
            resources[i].Initialize((ResourceType)i, humanPlayer.Resources[i], OnResourceChanged);
        }
        gameObject.SetActive(true);
    }

    private void OnResourceChanged(ResourceType resourceType, int deltaAmount)
    {
        _selectedResourcesArray[(int)resourceType] += deltaAmount;
        
        
        _totalSelected = 0;
        foreach (int amount in _selectedResourcesArray)
        {
            _totalSelected += amount;
        }
        
        UpdateConfirmButton();
    } 
    private void UpdateConfirmButton()
    {
        confirmButton.interactable = (_totalSelected == _requiredToDiscard);
    }

    private void OnConfirmClicked()
    {
        int[] result = new int[5];
        System.Array.Copy(_selectedResourcesArray, result, 5);
        
        _onCompleteCallback(result);
        gameObject.SetActive(false);
        
    }

    private void OnCancelClicked()
    {
        foreach (var resource in resources)
            resource.ResetSelection();

        for (int i = 0; i < _selectedResources.Count; i++)
        {
            _selectedResourcesArray[i] = 0;
        }
        
        _totalSelected = 0;
        UpdateConfirmButton();
    }

    public void ForceClose()
    {
        OnCancelClicked();
        gameObject.SetActive(false);
        _onCompleteCallback?.Invoke(null);
    }
}
