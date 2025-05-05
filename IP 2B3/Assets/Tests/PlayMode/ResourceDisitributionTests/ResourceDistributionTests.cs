using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using B3.PlayerSystem;
using B3.SettlementSystem;
using B3.PieceSystem;
using B3.ResourcesSystem;
using B3.BankSystem;
using B3.BoardSystem;
using B3.ThiefSystem;

public class ResourceFullDistributionTest
{
    private class TestPlayer : PlayerBase
    {
        public override IEnumerator DiceThrowForceCoroutine() => null;
        public override IEnumerator MoveThiefCoroutine(ThiefController thiefController) => null;
        public override void OnTradeAndBuildUpdate() {}
        public int GetResourceCount(ResourceType type) => Resources[(int)type];
    }

    private TestPlayer player;
    private BoardController board;

    [UnitySetUp]
    public IEnumerator LoadScene()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;
    }

    [UnityTest]
    public IEnumerator PlayerReceivesResources_FromMultipleHexes()
    {
        player = new GameObject("Player").AddComponent<TestPlayer>();

        // Găsim două settlement-uri libere din scenă
        var freeSettlements = Object.FindObjectsOfType<SettlementController>()
            .Where(s => !s.HasOwner).Take(2).ToList();
        Assert.AreEqual(2, freeSettlements.Count, "Trebuie cel puțin 2 settlement-uri libere în scenă.");

        foreach (var s in freeSettlements)
        {
            s.SetOwner(player);

            // Injectăm dummy GameObjects pentru houseObject și cityObject
            var dummyHouse = new GameObject("DummyHouse");
            var dummyCity = new GameObject("DummyCity");

            typeof(SettlementController).GetField("houseObject", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(s, dummyHouse);
            typeof(SettlementController).GetField("cityObject", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(s, dummyCity);
        }

        // Hex 1 - Wood
        var piece1 = new GameObject("Piece1").AddComponent<PieceController>();
        piece1.Number = 9;
        piece1.ResourceType = ResourceType.Wood;
        piece1.Settlements = new List<SettlementController> { freeSettlements[0] };

        // Hex 2 - Brick
        var piece2 = new GameObject("Piece2").AddComponent<PieceController>();
        piece2.Number = 9;
        piece2.ResourceType = ResourceType.Brick;
        piece2.Settlements = new List<SettlementController> { freeSettlements[1] };

        // Board + Bank setup
        var bank = new GameObject("Bank").AddComponent<BankController>();
        board = new GameObject("Board").AddComponent<BoardController>();
        board._pieceControllers[0] = piece1;
        board._pieceControllers[1] = piece2;

        typeof(BoardController).GetField("bankController", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(board, bank);

        // Simulăm aruncarea zarului cu 9
        board.GiveResources(9);

        // Verificări
        Assert.AreEqual(1, player.GetResourceCount(ResourceType.Wood), "Playerul nu a primit Wood.");
        Assert.AreEqual(1, player.GetResourceCount(ResourceType.Brick), "Playerul nu a primit Brick.");

        yield return null;
    }
}
