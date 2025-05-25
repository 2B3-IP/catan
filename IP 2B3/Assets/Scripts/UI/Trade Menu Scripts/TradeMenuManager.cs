using B3.BankSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;
using B3.TradeSystem;
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
    public PlayerBase CurrentPlayer;
    public TradeController TradeController;
    public PlayerBase OtherPlayer;

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
        for (int i = 0; i < CardManagerMine.size; i++)
        {
            if (CardManagerMine.CardTypes[i] != CardManagerMine.CardTypes[0]) PresumedTradeType.MyType = TradeType.type.any;
        }
        for (int i = 0; i < CardManagerTheirs.size; i++)
        {
            if (CardManagerTheirs.CardTypes[i] != CardManagerTheirs.CardTypes[0]) PresumedTradeType.TheirType = TradeType.type.any;
        }

        bool ok = true;

        ResourceType tempresourceToTrade = ResourceType.Wood;//Default e wood
        
        if (PresumedTradeType.MyType == TradeType.type.wood) tempresourceToTrade = ResourceType.Wood;
        if (PresumedTradeType.MyType == TradeType.type.brick) tempresourceToTrade = ResourceType.Brick;
        if (PresumedTradeType.MyType == TradeType.type.wheat) tempresourceToTrade = ResourceType.Wheat;
        if (PresumedTradeType.MyType == TradeType.type.sheep) tempresourceToTrade = ResourceType.Sheep;
        if (PresumedTradeType.MyType == TradeType.type.ore) tempresourceToTrade = ResourceType.Ore;

        //var playerBuffs = CurrentPlayer.PlayerBuffs;
        //int resourceCount = playerBuffs.GetResourceAmount(tempresourceToTrade);
        //TradeType.MyResourceNumber = resourceCount;

        if (!PlayerMenu.activeSelf)
        {
            if (PresumedTradeType.MyType == TradeType.type.any) ok = false;
            //if (PresumedTradeType.MyType != TradeType.MyType) ok = false;
            if (PresumedTradeType.TheirType != TradeType.TheirType && TradeType.TheirType != TradeType.type.any) ok = false;
            if (PresumedTradeType.MyResourceNumber != TradeType.MyResourceNumber) ok = false;
            if (PresumedTradeType.TheirResourceNumber != TradeType.TheirResourceNumber) ok = false;
        }
        else
        {
            if (OtherPlayer == CurrentPlayer) ok = false;
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

                for (int i = 0; i < CardManagerMine.size; i++)
                {

                    if (CardManagerMine.CardTypes[i] == TradeType.type.wood)
                    {
                        TradeOfferCardManagerMine.AddCard(CardManagerMine.WoodPrefab, CardManagerMine.MyWood);
                        TradeOfferCardManagerMine.CardTypes[CardManagerMine.size] = TradeType.type.wood;
                    }
                    if (CardManagerMine.CardTypes[i] == TradeType.type.brick)
                    {
                        TradeOfferCardManagerMine.AddCard(CardManagerMine.BrickPrefab, CardManagerMine.MyBrick);
                        TradeOfferCardManagerMine.CardTypes[CardManagerMine.size] = TradeType.type.brick;
                    }
                    if (CardManagerMine.CardTypes[i] == TradeType.type.wheat)
                    {
                        TradeOfferCardManagerMine.AddCard(CardManagerMine.WheatPrefab, CardManagerMine.MyWheat);
                        TradeOfferCardManagerMine.CardTypes[CardManagerMine.size] = TradeType.type.wheat;
                    }
                    if (CardManagerMine.CardTypes[i] == TradeType.type.sheep)
                    {
                        TradeOfferCardManagerMine.AddCard(CardManagerMine.SheepPrefab, CardManagerMine.MySheep);
                        TradeOfferCardManagerMine.CardTypes[CardManagerMine.size] = TradeType.type.sheep;
                    }
                    if (CardManagerMine.CardTypes[i] == TradeType.type.ore)
                    {
                        TradeOfferCardManagerMine.AddCard(CardManagerMine.OrePrefab, CardManagerMine.MyOre);
                        TradeOfferCardManagerMine.CardTypes[CardManagerMine.size] = TradeType.type.ore;
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
                        TradeOfferCardManagerTheirs.CardTypes[CardManagerTheirs.size] = TradeType.type.wood;
                    }
                    if (CardManagerTheirs.CardTypes[i] == TradeType.type.brick)
                    {
                        TradeOfferCardManagerTheirs.AddCard(CardManagerTheirs.BrickPrefab, CardManagerTheirs.MyBrick);
                        TradeOfferCardManagerTheirs.CardTypes[CardManagerTheirs.size] = TradeType.type.brick;
                    }
                    if (CardManagerTheirs.CardTypes[i] == TradeType.type.wheat)
                    {
                        TradeOfferCardManagerTheirs.AddCard(CardManagerTheirs.WheatPrefab, CardManagerTheirs.MyWheat);
                        TradeOfferCardManagerTheirs.CardTypes[CardManagerTheirs.size] = TradeType.type.wheat;
                    }
                    if (CardManagerTheirs.CardTypes[i] == TradeType.type.sheep)
                    {
                        TradeOfferCardManagerTheirs.AddCard(CardManagerTheirs.SheepPrefab, CardManagerTheirs.MySheep);
                        TradeOfferCardManagerTheirs.CardTypes[CardManagerTheirs.size] = TradeType.type.sheep;
                    }
                    if (CardManagerTheirs.CardTypes[i] == TradeType.type.ore)
                    {
                        TradeOfferCardManagerTheirs.AddCard(CardManagerTheirs.OrePrefab, CardManagerTheirs.MyOre);
                        TradeOfferCardManagerTheirs.CardTypes[CardManagerTheirs.size] = TradeType.type.ore;
                    }
                }

           

                TradeOffer.SetActive(true);
            }
            if (BankMenu.activeSelf)//Trade successful cu banca
            {
                ResourceType resourceToTrade = ResourceType.Wood;//Default e wood
                ResourceType resourceToGet = ResourceType.Wood;//Default e wood;

                if (PresumedTradeType.MyType == TradeType.type.wood) resourceToTrade = ResourceType.Wood;
                if (PresumedTradeType.MyType == TradeType.type.brick) resourceToTrade = ResourceType.Brick;
                if (PresumedTradeType.MyType == TradeType.type.wheat) resourceToTrade = ResourceType.Wheat;
                if (PresumedTradeType.MyType == TradeType.type.sheep) resourceToTrade = ResourceType.Sheep;
                if (PresumedTradeType.MyType == TradeType.type.ore) resourceToTrade = ResourceType.Ore;

                if (CardManagerTheirs.CardTypes[0] == TradeType.type.wood) resourceToGet = ResourceType.Wood;
                if (CardManagerTheirs.CardTypes[0] == TradeType.type.brick) resourceToGet = ResourceType.Brick;
                if (CardManagerTheirs.CardTypes[0] == TradeType.type.wheat) resourceToGet = ResourceType.Wheat;
                if (CardManagerTheirs.CardTypes[0] == TradeType.type.sheep) resourceToGet = ResourceType.Sheep;
                if (CardManagerTheirs.CardTypes[0] == TradeType.type.ore) resourceToGet = ResourceType.Ore;

                CurrentPlayer.RemoveResource(resourceToTrade, 4);
                BankController.GiveResources(resourceToTrade, 4);
                print("Gaga");
                BankController.GetResources(resourceToGet, 1);
                CurrentPlayer.AddResource(resourceToGet, 1);
            }
            if (HarbourMenu.activeSelf)
            {
                ResourceType resourceToTrade = ResourceType.Wood;//Default e wood
                ResourceType resourceToGet = ResourceType.Wood;//Default e wood;

                if (PresumedTradeType.MyType == TradeType.type.wood) resourceToTrade = ResourceType.Wood;
                if (PresumedTradeType.MyType == TradeType.type.brick) resourceToTrade = ResourceType.Brick;
                if (PresumedTradeType.MyType == TradeType.type.wheat) resourceToTrade = ResourceType.Wheat;
                if (PresumedTradeType.MyType == TradeType.type.sheep) resourceToTrade = ResourceType.Sheep;
                if (PresumedTradeType.MyType == TradeType.type.ore) resourceToTrade = ResourceType.Ore;

                if (CardManagerTheirs.CardTypes[0] == TradeType.type.wood) resourceToGet = ResourceType.Wood;
                if (CardManagerTheirs.CardTypes[0] == TradeType.type.brick) resourceToGet = ResourceType.Brick;
                if (CardManagerTheirs.CardTypes[0] == TradeType.type.wheat) resourceToGet = ResourceType.Wheat;
                if (CardManagerTheirs.CardTypes[0] == TradeType.type.sheep) resourceToGet = ResourceType.Sheep;
                if (CardManagerTheirs.CardTypes[0] == TradeType.type.ore) resourceToGet = ResourceType.Ore;


                TradeController.TradeResources(CurrentPlayer, resourceToTrade, resourceToGet);
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

        

    }
    public void AcceptOffer()
    {
        print("Am ajuns aici!");
        if (TradeOffer.activeSelf)
        {
            int[] ResourcesToTrade = new int[5];
            for (int i=0; i<5; i++)
            {
                ResourcesToTrade[i] = 0;   
            }
            for (int i=0; i< TradeOfferCardManagerMine.size; i++)
            {
                //print(TradeOfferCardManagerMine.size);
                if (CardManagerMine.CardTypes[i] == TradeType.type.wood) { ResourcesToTrade[2]++; print("Lemn"); }
                if (CardManagerMine.CardTypes[i] == TradeType.type.brick) ResourcesToTrade[3]++;
                if (CardManagerMine.CardTypes[i] == TradeType.type.wheat) ResourcesToTrade[1]++;
                if (CardManagerMine.CardTypes[i] == TradeType.type.sheep) ResourcesToTrade[4]++;
                if (CardManagerMine.CardTypes[i] == TradeType.type.ore) ResourcesToTrade[0]++;
            }
            for (int i = 0; i < TradeOfferCardManagerTheirs.size; i++)
            {
                if (CardManagerTheirs.CardTypes[i] == TradeType.type.wood) ResourcesToTrade[2]--;
                if (CardManagerTheirs.CardTypes[i] == TradeType.type.brick) ResourcesToTrade[3]--;
                if (CardManagerTheirs.CardTypes[i] == TradeType.type.wheat) ResourcesToTrade[1]--;
                if (CardManagerTheirs.CardTypes[i] == TradeType.type.sheep) ResourcesToTrade[4]--;
                if (CardManagerTheirs.CardTypes[i] == TradeType.type.ore) ResourcesToTrade[0]--;
            }
            
            CurrentPlayer.Resources[0] -= ResourcesToTrade[2];
            CurrentPlayer.Resources[1] -= ResourcesToTrade[3];
            CurrentPlayer.Resources[2] -= ResourcesToTrade[1];
            CurrentPlayer.Resources[3] -= ResourcesToTrade[4];
            CurrentPlayer.Resources[4] -= ResourcesToTrade[0];

            OtherPlayer.Resources[0] += ResourcesToTrade[2];
            OtherPlayer.Resources[1] += ResourcesToTrade[3];
            OtherPlayer.Resources[2] += ResourcesToTrade[1];
            OtherPlayer.Resources[3] += ResourcesToTrade[4];
            OtherPlayer.Resources[4] += ResourcesToTrade[0];

            //TradeController.TradeResources(CurrentPlayer, OtherPlayer, ResourcesToTrade);
            print("Am ajuns aici!");
            TradeOfferCardManagerMine.EraseCards();
            TradeOfferCardManagerTheirs.EraseCards();
            CardManagerMine.EraseCards();
            CardManagerTheirs.EraseCards();
        }
        TradeOffer.SetActive(false);
    }
    public void DeclineOffer()
    {
        if (TradeOffer.activeSelf)
        {
            TradeOffer.SetActive(false);

            TradeOfferCardManagerMine.EraseCards();
            TradeOfferCardManagerTheirs.EraseCards();
            CardManagerMine.EraseCards();
            CardManagerTheirs.EraseCards();
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
