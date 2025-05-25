using B3.PlayerSystem;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    int MyWoodNumber;
    int MyBrickNumber;
    int MyWheatNumber;
    int MySheepNumber;
    int MyOreNumber;

    TextMeshPro Wood;
    TextMeshPro Brick;
    TextMeshPro Wheat;
    TextMeshPro Sheep;
    TextMeshPro Ore;

    PlayerBase player;

    void GetResources()
    {
        MyWoodNumber = player.Resources[0];
        MyBrickNumber = player.Resources[1];
        MyWheatNumber = player.Resources[2];
        MySheepNumber = player.Resources[3];
        MyOreNumber = player.Resources[4];
    }
    void SetResources()
    {
        Wood.text = MyWoodNumber.ToString();
        Wood.text = MyBrickNumber.ToString();
        Wood.text = MyWheatNumber.ToString();
        Wood.text = MySheepNumber.ToString();
        Wood.text = MyOreNumber.ToString();
    }
}
