using UnityEngine;
using UnityEngine.Serialization;

public class TradeMenuManager : MonoBehaviour
{
    bool CevaEsteActiv = false;
    public GameObject SelectMenu;
    public GameObject BankMenu;

    public void OnTradeButtonClicked()
    {
        if (CevaEsteActiv)
        {
            SelectMenu.SetActive(false);
            BankMenu.SetActive(false);
            CevaEsteActiv=false;
        }
        else
        {
            SelectMenu.SetActive(true);
            CevaEsteActiv = true;
        }

    }

    public void OnBankButtonClicked()
    {
        SelectMenu.SetActive(false);
        BankMenu.SetActive(true);
    }



}
