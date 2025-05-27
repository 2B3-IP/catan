using B3.PlayerSystem;
using UnityEngine;
using UnityEngine.UI;

public class DropdownScript : MonoBehaviour
{
    public TradeMenuManager TradeManager;
    public PlayerBase Red;
    public PlayerBase Blue;
    public PlayerBase Yellow;
    public PlayerBase Green;
    public void HandleInputData(int val)
    {
        if (val == 0)
        {
            TradeManager.OtherPlayer = Red;
        }
        if (val == 1)
        {
            TradeManager.OtherPlayer = Yellow;
        }
        if (val == 2)
        {
            TradeManager.OtherPlayer = Green;
        }
        if (val == 3)
        {
            TradeManager.OtherPlayer = Blue;
        }
    }
}
