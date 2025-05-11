using System.Collections;
using System.Linq;
using System.Reflection;
using B3.GameStateSystem;
using B3.PlayerSystem;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ResourceGameStateIntegrationTest
{
    private GameStateMachine gsm;
    private PlayerBase player;

    [UnitySetUp]
    public IEnumerator LoadSceneAndSetup()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;
        yield return new WaitForSeconds(10f); // așteptăm scena

        gsm = Object.FindObjectOfType<GameStateMachine>();
        Assert.IsNotNull(gsm, "❌ GameStateMachine not found.");

        // ⚙️ Creează un player
        var playerGO = new GameObject("TestPlayer");
        player = playerGO.AddComponent<TestPlayer>();

        // 🧩 Injectează player-ul în PlayersManager
        var playersManager = gsm.GetType()
            .GetField("playersManager", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(gsm) as PlayersManager;
        Assert.IsNotNull(playersManager, "❌ PlayersManager is null.");
        typeof(PlayersManager)
            .GetProperty("ActivePlayers")
            ?.SetValue(playersManager, new System.Collections.Generic.List<PlayerBase> { player });

        // Asigură-te că e playerul activ
        gsm.GetType()
            .GetField("_currentPlayerIndex", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(gsm, 0);
    }

    [UnityTest]
    public IEnumerator ResourceGameState_TransitionsToPlayerFreeGameState()
    {
        // 🎯 Găsește ResourceGameState din Inspector
        var gameStates = typeof(GameStateMachine)
            .GetField("gameStates", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(gsm) as GameStateBase[];

        Assert.IsNotNull(gameStates, "❌ Nu s-a putut extrage gameStates.");
        var resourceState = gameStates.FirstOrDefault(s => s is ResourceGameState) as ResourceGameState;
        Assert.IsNotNull(resourceState, "❌ ResourceGameState nu a fost găsit în gameStates.");

        // 📥 Setează DiceRolls la o valoare validă
        var diceThrowerField = typeof(ResourceGameState)
            .GetField("diceThrower", BindingFlags.NonPublic | BindingFlags.Instance);
        var thrower = diceThrowerField?.GetValue(resourceState) as B3.DiceSystem.DiceThrower;
        Assert.IsNotNull(thrower, "❌ DiceThrower nu e setat în ResourceGameState.");
        thrower.SetResult(6);

        // ▶️ Rulează OnEnter
        yield return resourceState.OnEnter(gsm);

        // ✅ Verifică dacă starea curentă este PlayerFreeGameState
        var currentState = typeof(GameStateMachine)
            .GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(gsm);

        Assert.IsNotNull(currentState, "❌ _currentState este null după OnEnter.");
        Assert.AreEqual(typeof(PlayerFreeGameState), currentState.GetType(), "❌ Nu s-a trecut în PlayerFreeGameState.");
        Debug.Log("✅ Test trecut: ResourceGameState → PlayerFreeGameState.");
    }
    [UnityTest]
    public IEnumerator PlayerDiceGameState_TransitionsToResourceGameState_WhenDiceIsNotSeven()
    {
        // 🎯 Găsește gameStates[]
        var gameStates = typeof(GameStateMachine)
            .GetField("gameStates", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(gsm) as GameStateBase[];

        Assert.IsNotNull(gameStates, "❌ gameStates nu e setat.");

        // 🧩 Găsește PlayerDiceGameState
        var playerDiceState = gameStates.FirstOrDefault(s => s is PlayerDiceGameState) as PlayerDiceGameState;
        Assert.IsNotNull(playerDiceState, "❌ PlayerDiceGameState nu a fost găsit.");

        // 🎲 Injectează DiceThrower cu rezultat ≠ 7
        var diceThrowerField = typeof(PlayerDiceGameState)
            .GetField("diceThrower", BindingFlags.NonPublic | BindingFlags.Instance);

        var thrower = diceThrowerField?.GetValue(playerDiceState) as B3.DiceSystem.DiceThrower;
        Assert.IsNotNull(thrower, "❌ DiceThrower nu e injectat în PlayerDiceGameState.");
        thrower.SetResult(6); // oricare diferit de 7

        // ▶️ Rulează PlayerDiceGameState
        yield return playerDiceState.OnEnter(gsm);

        // ✅ Verifică tranziția către ResourceGameState
        var currentState = typeof(GameStateMachine)
            .GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(gsm);

        Assert.AreEqual(typeof(PlayerFreeGameState), currentState.GetType(), "❌ Nu s-a trecut în PlayerFreeGameState.");
        Debug.Log("✅ PlayerDiceGameState → ResourceGameState → PlayerFreeGameState");
    }


    private class TestPlayer : PlayerBase
    {
        public override IEnumerator DiceThrowForceCoroutine() => null;
        public override IEnumerator MoveThiefCoroutine(B3.ThiefSystem.ThiefController _) => null;
        public override void OnTradeAndBuildUpdate() { }
        public new int[] Resources { get; set; } = new int[5];
        public float DiceThrowForce => 5f;
    }
}
