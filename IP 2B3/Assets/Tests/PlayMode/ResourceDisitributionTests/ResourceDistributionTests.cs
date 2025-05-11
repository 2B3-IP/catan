using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Collections;
using System.Linq;
using System.Reflection;
using B3.BankSystem;
using B3.BoardSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;
using B3.SettlementSystem;
using B3.PieceSystem;

public class SimpleResourceDistributionTest
{
    private BoardController board;
    private PlayerBase player;
    private BankController _bankController;
    [UnitySetUp]
    public IEnumerator LoadScene()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;
        yield return new WaitForSeconds(10f); // așteptăm să se genereze tot
    }

    [UnityTest]
    public IEnumerator PlayerReceivesResources_WhenSettlementIsManuallyLinked()
    {
        board = Object.FindObjectOfType<BoardController>();
        Assert.IsNotNull(board, "BoardController not found.");
        var gameObject = new GameObject(); _bankController = gameObject.AddComponent<BankController>();
        //  Găsește o piesă validă
        var piece = board._pieceControllers
            .FirstOrDefault(p => p != null && !p.IsBlocked);

        Assert.IsNotNull(piece, "Nu s-a găsit un PieceController valid.");

        //  Găsește un settlement din scenă
        var settlement = Object.FindObjectsOfType<SettlementController>()
            .FirstOrDefault(s => s != null && s.Owner == null);

        Assert.IsNotNull(settlement, "Nu s-a găsit niciun SettlementController liber.");

        //  Creează un jucător de test
        var playerGO = new GameObject("TestPlayer");
        player = playerGO.AddComponent<FakePlayer>();

        //  Leagă settlement-ul de player și piesă
        settlement.SetOwner(player);
        piece.Settlements.Add(settlement);
        Debug.Log($"[DEBUG] Settlement linked to Piece: {piece.name}");
        Debug.Log($"[DEBUG] Piece Number: {piece.Number}, ResourceType: {piece.ResourceType}, Blocked: {piece.IsBlocked}");
        Debug.Log($"[DEBUG] Settlement.Owner is null? {settlement.Owner == null}");
        Debug.Log($"[DEBUG] Player initial resource count: {player.Resources[(int)piece.ResourceType]}");
        int resourceIndex = (int)piece.ResourceType;
        int before = player.Resources[resourceIndex];

        Debug.Log($"[TEST] Resursa {piece.ResourceType} înainte: {before} — DiceNumber: {piece.Number}");

        board.GiveResources(piece.Number);
        yield return null;

        int after = player.Resources[resourceIndex];
        Debug.Log($"[TEST] Resursa {piece.ResourceType} după: {after}");

        Assert.Greater(after, before, "❌ Playerul nu a primit resursa după GiveResources.");
    }
    [UnityTest]
    public IEnumerator CitySettlement_ReceivesTwoResources()
    {
        board = Object.FindObjectOfType<BoardController>();
        Assert.IsNotNull(board, "❌ BoardController not found.");

        // Injectează BankController
        var bankGO = new GameObject("FakeBank");
         _bankController = bankGO.AddComponent<BankController>();
        typeof(BoardController)
            .GetField("bankController", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(board, _bankController);

        // Găsește o piesă validă
        var piece = board._pieceControllers
            .FirstOrDefault(p => p != null && !p.IsBlocked);
        Assert.IsNotNull(piece, "No valid PieceController found.");

        // Găsește un settlement
        var settlement = Object.FindObjectsOfType<SettlementController>()
            .FirstOrDefault(s => s != null && s.Owner == null);
        Assert.IsNotNull(settlement, "No available SettlementController.");

        // Creează player + resources
        var playerGO = new GameObject("CityPlayer");
        var cityPlayer = playerGO.AddComponent<FakePlayer>();

        // Leagă settlement și upgrade la oraș
        settlement.SetOwner(cityPlayer);
        settlement.UpgradeToCity();
        piece.Settlements.Add(settlement);

        int resourceIndex = (int)piece.ResourceType;
        int before = cityPlayer.Resources[resourceIndex];

        Debug.Log($"[TEST] City - Before: {before} resurse {piece.ResourceType}");

        board.GiveResources(piece.Number);
        yield return null;

        int after = cityPlayer.Resources[resourceIndex];
        Debug.Log($"[TEST] City - After: {after} resurse {piece.ResourceType}");

        Assert.AreEqual(before + 2, after, "Orașul nu a primit 2 resurse!");
    }
    [UnityTest]
    public IEnumerator BlockedPiece_DoesNotGiveResources()
    {
        board = Object.FindObjectOfType<BoardController>();
        Assert.IsNotNull(board, "❌ BoardController not found.");

        // Injectează BankController
        var bankGO = new GameObject("FakeBank");
        var bank = bankGO.AddComponent<BankController>();
        typeof(BoardController)
            .GetField("bankController", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(board, bank);

        // Găsește o piesă validă
        var piece = board._pieceControllers
            .FirstOrDefault(p => p != null);
        Assert.IsNotNull(piece, "No PieceController found.");

        piece.IsBlocked = true; // 🔒 blocăm piesa

        // Găsește un settlement
        var settlement = Object.FindObjectsOfType<SettlementController>()
            .FirstOrDefault(s => s != null && s.Owner == null);
        Assert.IsNotNull(settlement, "No available SettlementController.");

        // Creează player
        var playerGO = new GameObject("BlockedPlayer");
        var blockedPlayer = playerGO.AddComponent<FakePlayer>();

        // Leagă settlement la piesă
        settlement.SetOwner(blockedPlayer);
        piece.Settlements.Add(settlement);

        int resourceIndex = (int)piece.ResourceType;
        int before = blockedPlayer.Resources[resourceIndex];

        Debug.Log($"[TEST] Blocked piece: Resource {piece.ResourceType}, Before: {before}");

        board.GiveResources(piece.Number);
        yield return null;

        int after = blockedPlayer.Resources[resourceIndex];
        Debug.Log($"[TEST] Blocked piece: After: {after}");

        Assert.AreEqual(before, after, "Resursele au fost distribuite deși piesa era blocată.");
    }


    private class FakePlayer : PlayerBase
    {
        public override IEnumerator DiceThrowForceCoroutine() => null;
        public override IEnumerator MoveThiefCoroutine(B3.ThiefSystem.ThiefController _) => null;
        public override void OnTradeAndBuildUpdate() { }
    }
}
