using NUnit.Framework;
using UnityEngine;
using B3.TradeSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;
using B3.ThiefSystem;
using System.Collections;

public class PlayerMutualTradeTest
{
    private class TestPlayer : PlayerBase
    {
        public override IEnumerator DiceThrowForceCoroutine() => null;
        public override IEnumerator MoveThiefCoroutine(ThiefController thiefController) => null;
        public override void OnTradeAndBuildUpdate() {}
    }

    private class TestTradeSystem : TradeSystem {}

    private TestPlayer playerA;
    private TestPlayer playerB;
    private TradeSystem tradeSystem;

    [SetUp]
    public void SetUp()
    {
        playerA = new GameObject("PlayerA").AddComponent<TestPlayer>();
        playerB = new GameObject("PlayerB").AddComponent<TestPlayer>();
        tradeSystem = new GameObject("TradeSystem").AddComponent<TestTradeSystem>();

        // Player A oferă 2 Brick, 1 Ore
        playerA.AddResource(ResourceType.Brick, 2);
        playerA.AddResource(ResourceType.Ore, 1);

        // Player B oferă 1 Wheat
        playerB.AddResource(ResourceType.Wheat, 1);
    }

    [Test]
    public void MutualTradeBetweenPlayers_TransfersAllResourcesCorrectly()
    {
        int[] offerFromA = new int[5];
        offerFromA[(int)ResourceType.Brick] = 2;
        offerFromA[(int)ResourceType.Ore] = 1;

        int[] offerFromB = new int[5];
        offerFromB[(int)ResourceType.Wheat] = 1;

        // A dă lui B
        CallTrade(tradeSystem, playerA, playerB, offerFromA);

        // B dă lui A
        CallTrade(tradeSystem, playerB, playerA, offerFromB);

        // Verificări
        Assert.AreEqual(1, playerA.Resources[(int)ResourceType.Wheat], "A ar trebui să primească Wheat de la B.");
        Assert.AreEqual(2, playerB.Resources[(int)ResourceType.Brick], "B ar trebui să primească Brick de la A.");
        Assert.AreEqual(1, playerB.Resources[(int)ResourceType.Ore], "B ar trebui să primească Ore de la A.");
    }

    private void CallTrade(TradeSystem system, PlayerBase from, PlayerBase to, int[] offer)
    {
        typeof(TradeSystem)
            .GetMethod("TradeResources", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new[] { typeof(PlayerBase), typeof(PlayerBase), typeof(int[]) }, null)
            ?.Invoke(system, new object[] { from, to, offer });
    }
}
