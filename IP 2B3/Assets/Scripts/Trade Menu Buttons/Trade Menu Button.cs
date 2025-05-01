using UnityEngine;

public class TradeMenuButton : MonoBehaviour
{
    public GameObject TradeMenu; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WhenButtonClicked()
    {
        if (TradeMenu.activeInHierarchy)
        {
            TradeMenu.SetActive(false);
        }
        else
        {
            TradeMenu.SetActive(true);
        }
    }
}
