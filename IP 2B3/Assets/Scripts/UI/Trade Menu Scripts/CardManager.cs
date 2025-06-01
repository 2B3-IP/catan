using System;
using System.Collections;
using UnityEngine;

public class CardManager : MonoBehaviour
{

    [SerializeField] public float AnimationDuration;
    [SerializeField] public LeanTweenType EaseType;
    [SerializeField] public int spacing;

    public GameObject TradeOffer;

    public GameObject MyWood;
    public GameObject MyBrick;
    public GameObject MyWheat;
    public GameObject MySheep;
    public GameObject MyOre;


    public GameObject WoodPrefab;
    public GameObject BrickPrefab;
    public GameObject WheatPrefab;
    public GameObject SheepPrefab;
    public GameObject OrePrefab;

    public GameObject WhatIGive;

    public int size;
    public GameObject[] Cards = new GameObject[7];
    public Vector3[] CardLocations = new Vector3[7];
    public TradeType.type[] CardTypes = new TradeType.type[7]; 

    public void MoveCards()
    {
        for (int i = 0; i < size; i++)
        {
            Vector3 v = new Vector3(CardLocations[i].x, CardLocations[i].y, 0);
            Cards[i].LeanMove(v, AnimationDuration).setEase(EaseType);
        }
    }

    public void AddCard(GameObject prefab, GameObject start)
    {
        if (size < 7)
        {
            GameObject clone = Instantiate(prefab);
            clone.transform.parent = WhatIGive.transform;
            clone.transform.position = start.transform.position;

            Cards[size] = clone;
            size++;
            Vector3 v = WhatIGive.transform.position;

            if (size > 3) spacing = 25;
            //if (size > 5 && TradeOffer.activeSelf) spacing = -25;

            for (int i = 0; i < size; i++)
            {
                CardLocations[i].y = WhatIGive.transform.position.y;
                CardLocations[i].x = WhatIGive.transform.position.x + spacing * (2 * i - size + 1);
            }

            //CardLocations[size] = WhatIGive.transform.position;

            MoveCards();
        }
        else print("Este prea multe carti ba");
    }
    
    public void EraseCards()
    {
        for (int i = 0; i < size; i++)
        {
            CardLocations[i].y = 0;
            CardLocations[i].x = 0;
            CardLocations[i].z = 0;
            CardTypes[i] = TradeType.type.any;
            Destroy(Cards[i]);
        }
        size = 0;
    }
}