using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using B3.BankSystem;
using B3.ResourcesSystem;

public class BankControllerTests
{
    private BankController _bankController;

    [SetUp]
    public void Setup()
    {
        var gameObject = new GameObject();
        _bankController = gameObject.AddComponent<BankController>();
    }

    [Test]
    public void HasResources_WhenEnoughResources_ReturnsTrue()
    {
        Assert.IsTrue(_bankController.HasResources(ResourceType.Wood, 5));
    }

    [Test]
    public void GiveResources_IncreasesResourceCount()
    {
        int extraAmount = 3;
        _bankController.GiveResources(ResourceType.Brick, extraAmount);
        Assert.IsTrue(_bankController.HasResources(ResourceType.Brick, 19 + extraAmount));
    }

    [Test]
    public void GetResources_WhenEnoughResources_DecreasesResourceCount()
    {
        _bankController.GetResources(ResourceType.Wheat, 5);
        Assert.IsTrue(_bankController.HasResources(ResourceType.Wheat, 14));
    }

    [Test]
    public void GetResources_WhenNotEnoughResources_DoesNotCrash()
    {
        _bankController.GetResources(ResourceType.Ore, 50);
        Assert.IsTrue(_bankController.HasResources(ResourceType.Ore, 19));
    }

    [Test]
    public void BuyDevelopmentCard_WhenAvailable_ReturnsCard()
    {
        var card = _bankController.BuyDevelopmentCard();
        Assert.IsNotNull(card);
    }

    [Test]
    public void BuyDevelopmentCard_WhenNoneAvailable_ReturnsNull()
    {
        // Epuizăm toate cărțile de dezvoltare
        for (int i = 0; i < 25; i++)
        {
            _bankController.BuyDevelopmentCard();
        }

        var card = _bankController.BuyDevelopmentCard();
        Assert.IsNull(card);
    }
}
