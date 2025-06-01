using UnityEngine;
using B3.ResourcesSystem;
using TMPro;
using B3.PlayerSystem;
using UnityEngine.UI;
public class ResourceDiscard : MonoBehaviour
{ 
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Button button;
    
    private ResourceType _resourceType;
    private int _playerAmount;
    private int _currentAmount;
    private System.Action<ResourceType, int> _onResourceChanged;
    
    public void Initialize(ResourceType resourceType, int playerAmount, System.Action<ResourceType, int> onResourceChanged)
    {
        _resourceType = resourceType;
        _playerAmount = playerAmount;
        _currentAmount = 0;
        _onResourceChanged = onResourceChanged;
        button.onClick.RemoveAllListeners();
        
        button.onClick.AddListener(onClickButton);
        UpdateUI();
    }

    private void onClickButton()
    {
        if (_currentAmount < _playerAmount)
        {
            _currentAmount++;
            _onResourceChanged?.Invoke(_resourceType, 1);
        }
        else
        {
            _onResourceChanged?.Invoke(_resourceType, -_playerAmount);
            _currentAmount = 0;
        }
        UpdateUI();
    }

    private void UpdateUI()
    {
        countText.text = _currentAmount.ToString();
        button.interactable = (_currentAmount < _playerAmount);
    }

    public void ResetSelection()
    {
        if (_currentAmount > 0)
        {
            _onResourceChanged?.Invoke(_resourceType, -_currentAmount);
            _currentAmount = 0;
            UpdateUI();
        }
    }
}
