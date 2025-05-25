using B3.BankSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class TradeMenuManager : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    [SerializeField] float duration;
    [SerializeField] LeanTweenType easeType;

    bool CevaEsteActiv = false;

    public CardManager CardManagerMine;
    public CardManager CardManagerTheirs;

    public BankController BankController;

    public CardManager TradeOfferCardManagerTheirs;
    public CardManager TradeOfferCardManagerMine;

    public TradeType TradeType;
    public TradeType PresumedTradeType;

    public GameObject SelectMenu;
    public GameObject BankMenu;
    public GameObject PlayerMenu;
    public GameObject HarbourMenu;
    public GameObject WaitingScreen;
    public GameObject TradeOffer;

    public GameObject MyWood;
    public GameObject MyBrick;
    public GameObject MyWheat;
    public GameObject MySheep;
    public GameObject MyOre;

    public GameObject TheirWood;
    public GameObject TheirBrick;
    public GameObject TheirWheat;
    public GameObject TheirSheep;
    public GameObject TheirOre;

    // Functii pt meniuri
    public void OnTradeButtonClicked()// Butonul de jos, nu cel verde pt trade
    {
        if (CevaEsteActiv)
        {
            SelectMenu.SetActive(false);
            BankMenu.SetActive(false);
            HarbourMenu.SetActive(false);
            PlayerMenu.SetActive(false);
            WaitingScreen.SetActive(false);
            TradeOffer.SetActive(false);
            CevaEsteActiv =false;

            PresumedTradeType.MyResourceNumber = 0;
            PresumedTradeType.TheirResourceNumber = 0;

            //if(BankMenu is )
        }
        else
        {
            SelectMenu.SetActive(true);
            CevaEsteActiv = true;

            PresumedTradeType.MyType = TradeType.type.any;
            PresumedTradeType.TheirType = TradeType.type.any;
            PresumedTradeType.MyResourceNumber = 0;
            PresumedTradeType.TheirResourceNumber = 0;
        }

    }

    public void OnBankButtonClicked()
    {
        SelectMenu.SetActive(false);
        BankMenu.SetActive(true);
    }

    public void OnHarbourButtonClicked()
    {
        SelectMenu.SetActive(false);
        HarbourMenu.SetActive(true);
    }

    public void OnPlayerButtonClicked()
    {
        SelectMenu.SetActive(false);
        PlayerMenu.SetActive(true);
    }

    public void OnOkTradeButtonClicked()// Butonul verde pt trade
    {
        for (int i = 0; i< CardManagerMine.size; i++)
        {
            if (CardManagerMine.CardTypes[i] != PresumedTradeType.MyType) PresumedTradeType.MyType = TradeType.type.any;
        }
        for (int i = 0; i < CardManagerTheirs.size; i++)
        {
            if (CardManagerTheirs.CardTypes[i] != PresumedTradeType.TheirType) PresumedTradeType.TheirType = TradeType.type.any;
        }

        bool ok = true;
        if(!PlayerMenu.activeSelf)
        {
            if (PresumedTradeType.MyType == TradeType.type.any) ok = false;
            //if (PresumedTradeType.MyType != TradeType.MyType) ok = false;
            if (PresumedTradeType.TheirType != TradeType.TheirType && TradeType.TheirType != TradeType.type.any) ok = false;
            if (PresumedTradeType.MyResourceNumber != TradeType.MyResourceNumber) ok = false;
            if (PresumedTradeType.TheirResourceNumber != TradeType.TheirResourceNumber) ok = false;
        }


        if (PresumedTradeType.MyResourceNumber == 0) ok = false;
        if (PresumedTradeType.TheirResourceNumber == 0) ok = false;

        if (ok == true)
        {
            print("Trade successful");

            if (PlayerMenu.activeSelf)//Daca e vorba de un player trade
            {
                //WaitingScreen.SetActive(true);
                TradeOfferCardManagerMine.AnimationDuration = 0;
                if (TradeOfferCardManagerMine.size > 5) TradeOfferCardManagerMine.spacing = -25;
                else TradeOfferCardManagerMine.spacing = 25;

                for (int i = 0; i < CardManagerMine.size;  i++)
                {

                    if (CardManagerMine.CardTypes[i] == TradeType.type.wood)
                    {
                        TradeOfferCardManagerMine.AddCard(CardManagerMine.WoodPrefab, CardManagerMine.MyWood);
                    }
                    if (CardManagerMine.CardTypes[i] == TradeType.type.brick)
                    {
                        TradeOfferCardManagerMine.AddCard(CardManagerMine.BrickPrefab, CardManagerMine.MyBrick);
                    }
                    if (CardManagerMine.CardTypes[i] == TradeType.type.wheat)
                    {
                        TradeOfferCardManagerMine.AddCard(CardManagerMine.WheatPrefab, CardManagerMine.MyWheat);
                    }
                    if (CardManagerMine.CardTypes[i] == TradeType.type.sheep)
                    {
                        TradeOfferCardManagerMine.AddCard(CardManagerMine.SheepPrefab, CardManagerMine.MySheep);
                    }
                    if (CardManagerMine.CardTypes[i] == TradeType.type.ore)
                    {
                        TradeOfferCardManagerMine.AddCard(CardManagerMine.OrePrefab, CardManagerMine.MyOre);
                    }
                }

                TradeOfferCardManagerTheirs.AnimationDuration = 0;
                if (TradeOfferCardManagerTheirs.size > 5) TradeOfferCardManagerTheirs.spacing = -25;
                else TradeOfferCardManagerTheirs.spacing = 25;

                for (int i = 0; i < CardManagerTheirs.size; i++)
                {
                   
                    if (CardManagerTheirs.CardTypes[i] == TradeType.type.wood)
                    {
                        TradeOfferCardManagerTheirs.AddCard(CardManagerTheirs.WoodPrefab, CardManagerTheirs.MyWood);
                    }
                    if (CardManagerTheirs.CardTypes[i] == TradeType.type.brick)
                    {
                        TradeOfferCardManagerTheirs.AddCard(CardManagerTheirs.BrickPrefab, CardManagerTheirs.MyBrick);
                    }
                    if (CardManagerTheirs.CardTypes[i] == TradeType.type.wheat)
                    {
                        TradeOfferCardManagerTheirs.AddCard(CardManagerTheirs.WheatPrefab, CardManagerTheirs.MyWheat);
                    }
                    if (CardManagerTheirs.CardTypes[i] == TradeType.type.sheep)
                    {
                        TradeOfferCardManagerTheirs.AddCard(CardManagerTheirs.SheepPrefab, CardManagerTheirs.MySheep);
                    }
                    if (CardManagerTheirs.CardTypes[i] == TradeType.type.ore)
                    {
                        TradeOfferCardManagerTheirs.AddCard(CardManagerTheirs.OrePrefab, CardManagerTheirs.MyOre);
                    }
                }

                TradeOffer.SetActive(true);
            }

            BankMenu.SetActive(false);
            HarbourMenu.SetActive(false);
            PlayerMenu.SetActive(false);
        }
        else
        {
            print("Wrong trade format dummy");
            
        }

        PresumedTradeType.MyResourceNumber = 0;
        PresumedTradeType.TheirResourceNumber = 0;
        PresumedTradeType.MyType = TradeType.type.any;
        PresumedTradeType.TheirType = TradeType.type.any;

        CardManagerMine.EraseCards();
        CardManagerTheirs.EraseCards();

    }
    public void AcceptOffer()
    {
        if(TradeOffer.activeSelf)
        {

            TradeOffer.SetActive(false);

            TradeOfferCardManagerMine.EraseCards();
            TradeOfferCardManagerTheirs.EraseCards();
        }
    }
    public void DeclineOffer()
    {
        if (TradeOffer.activeSelf)
        {
            TradeOffer.SetActive(false);

            TradeOfferCardManagerMine.EraseCards();
            TradeOfferCardManagerTheirs.EraseCards();
        }
    }
    // Functii pt cartile-butoane de jos, pe care probabil tr sa le fac cu un template
    public void OnMyWoodClicked()
    {
        CardManagerMine.AddCard(CardManagerMine.WoodPrefab, CardManagerMine.MyWood);
        if (CardManagerMine.size < 7)
        {
            PresumedTradeType.MyResourceNumber++;
            PresumedTradeType.MyType = TradeType.type.wood;
        }
        CardManagerMine.CardTypes[CardManagerMine.size - 1] = TradeType.type.wood;
    }

    public void OnMyBrickClicked()
    {
        CardManagerMine.AddCard(CardManagerMine.BrickPrefab, CardManagerMine.MyBrick);
        if (CardManagerMine.size < 7)
        {
            PresumedTradeType.MyResourceNumber++;
            PresumedTradeType.MyType = TradeType.type.brick;
        }
        CardManagerMine.CardTypes[CardManagerMine.size - 1] = TradeType.type.brick;
    }

    public void OnMyWheatClicked()
    {
        CardManagerMine.AddCard(CardManagerMine.WheatPrefab, CardManagerMine.MyWheat);
        if (CardManagerMine.size < 7)
        {
            PresumedTradeType.MyResourceNumber++;
            PresumedTradeType.MyType = TradeType.type.wheat;
        }
        CardManagerMine.CardTypes[CardManagerMine.size - 1] = TradeType.type.wheat;
    }

    public void OnMySheepClicked()
    {
        CardManagerMine.AddCard(CardManagerMine.SheepPrefab, CardManagerMine.MySheep);
        if (CardManagerMine.size < 7)
        {
            PresumedTradeType.MyResourceNumber++;
            PresumedTradeType.MyType = TradeType.type.sheep;
        }
        CardManagerMine.CardTypes[CardManagerMine.size - 1] = TradeType.type.sheep;
    }

    public void OnMyOreClicked()
    {
        CardManagerMine.AddCard(CardManagerMine.OrePrefab, CardManagerMine.MyOre);
        if (CardManagerMine.size < 7)
        {
            PresumedTradeType.MyResourceNumber++;
            PresumedTradeType.MyType = TradeType.type.ore;
        }
        CardManagerMine.CardTypes[CardManagerMine.size - 1] = TradeType.type.ore;
    }


    // Functii pt cartile-butoane de sus, pe care probabil tr sa le fac cu un template
    public void OnTheirWoodClicked()
    {
        CardManagerTheirs.AddCard(CardManagerTheirs.WoodPrefab, CardManagerTheirs.MyWood);
        if (CardManagerTheirs.size < 7)
        {
            PresumedTradeType.TheirResourceNumber++;
            PresumedTradeType.TheirType = TradeType.type.wood;
        }
        CardManagerTheirs.CardTypes[CardManagerTheirs.size - 1] = TradeType.type.wood;
    }
    public void OnTheirBrickClicked()
    {
        CardManagerTheirs.AddCard(CardManagerTheirs.BrickPrefab, CardManagerTheirs.MyBrick);
        if (CardManagerTheirs.size < 7)
        {
            PresumedTradeType.TheirResourceNumber++;
            PresumedTradeType.TheirType = TradeType.type.brick;
        }
        CardManagerTheirs.CardTypes[CardManagerTheirs.size - 1] = TradeType.type.brick;
    }
    public void OnTheirWheatClicked()
    {
        CardManagerTheirs.AddCard(CardManagerTheirs.WheatPrefab, CardManagerTheirs.MyWheat);
        if (CardManagerTheirs.size < 7)
        {
            PresumedTradeType.TheirResourceNumber++;
            PresumedTradeType.TheirType = TradeType.type.wheat;
        }
        CardManagerTheirs.CardTypes[CardManagerTheirs.size - 1] = TradeType.type.wheat;
    }
    public void OnTheirSheepClicked()
    {
        CardManagerTheirs.AddCard(CardManagerTheirs.SheepPrefab, CardManagerTheirs.MySheep);
        if (CardManagerTheirs.size < 7)
        {
            PresumedTradeType.TheirResourceNumber++;
            PresumedTradeType.TheirType = TradeType.type.sheep;
        }
        CardManagerTheirs.CardTypes[CardManagerTheirs.size - 1] = TradeType.type.sheep;
    }
    public void OnTheirOreClicked()
    {
        CardManagerTheirs.AddCard(CardManagerTheirs.OrePrefab, CardManagerTheirs.MyOre);
        if (CardManagerTheirs.size < 7)
        {
            PresumedTradeType.TheirResourceNumber++;
            PresumedTradeType.TheirType = TradeType.type.ore;
        }
        CardManagerTheirs.CardTypes[CardManagerTheirs.size - 1] = TradeType.type.ore;
    }
}
