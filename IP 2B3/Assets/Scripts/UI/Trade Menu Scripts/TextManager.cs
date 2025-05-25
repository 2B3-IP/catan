using B3.PlayerSystem;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    public int MyWoodNumber;
    public int MyBrickNumber;
    public int MyWheatNumber;
    public int MySheepNumber;
    public int MyOreNumber;

    public TMP_Text Wood;
    public TMP_Text Brick;
    public TMP_Text Wheat;
    public TMP_Text Sheep;
    public TMP_Text Ore;

    public PlayerBase player;

    void GetResources()//Coaie de ce ati facut in alta ordine smh
    {
        //MyWoodNumber = player.Resources[2];
        //MyBrickNumber = player.Resources[3];
        //MyWheatNumber = player.Resources[1];
        //MySheepNumber = player.Resources[4];
        //MyOreNumber = player.Resources[0];

        MyWoodNumber = player.Resources[0];
        MyBrickNumber = player.Resources[1];
        MyWheatNumber = player.Resources[2];
        MySheepNumber = player.Resources[3];
        MyOreNumber = player.Resources[4];
    }
    void SetResources()
    {
        Wood.text = MyWoodNumber.ToString();
        Brick.text = MyBrickNumber.ToString();
        Wheat.text = MyWheatNumber.ToString();
        Sheep.text = MySheepNumber.ToString();
        Ore.text = MyOreNumber.ToString();
    }

    private void Update()
    {
        GetResources();
        SetResources();
    }
}
