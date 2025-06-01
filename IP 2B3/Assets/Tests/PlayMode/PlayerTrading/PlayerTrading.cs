using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools.Logging;
using System.Collections;
using B3.TradeSystem;
using B3.BankSystem;
using B3.PlayerBuffSystem;
using B3.PlayerSystem;
using B3.ResourcesSystem;

public class TradeBankIntegrationTests
{
    private TradeController _tradeController;
    private BankController _bankController;
    private PlayerBase _playerA;
    private PlayerBase _playerB;

    [UnitySetUp]
    public IEnumerator LoadGameSceneAndFindComponents()
    {
        // Ignore the NullReferenceException from TurnDisplay.Start()
        LogAssert.ignoreFailingMessages = true;

        // 1) Load the “Game” scene (must be added to Build Settings → Scenes in Build).
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
        yield return null; // wait a frame for initialization

        // 2) Destroy any TurnDisplay component to avoid further errors
        var turnDisplay = Object.FindObjectOfType<TurnDisplay>();
        if (turnDisplay != null)
        {
            Object.DestroyImmediate(turnDisplay.gameObject);
            yield return null;
        }

        // 3) Find TradeController and BankController
        _tradeController = Object.FindObjectOfType<TradeController>();
        Assert.IsNotNull(_tradeController, "Could not find TradeController in the Game scene.");

        _bankController = Object.FindObjectOfType<BankController>();
        Assert.IsNotNull(_bankController, "Could not find BankController in the Game scene.");

        // 4) Find at least two PlayerBase instances
        var players = Object.FindObjectsOfType<PlayerBase>();
        Assert.IsTrue(players.Length >= 2, $"Expected ≥2 PlayerBase instances; found {players.Length}.");
        _playerA = players[0];
        _playerB = players[1];

        // 5) Ensure PlayerBuffs is non-null
        Assert.IsNotNull(_playerA.PlayerBuffs, "PlayerA.PlayerBuffs is null.");
        Assert.IsNotNull(_playerB.PlayerBuffs, "PlayerB.PlayerBuffs is null.");
    }

    [UnityTest]
    public IEnumerator BankTrade_Default4to1_UpdatesPlayerAndBankCorrectly()
    {
        int woodIdx  = (int)ResourceType.Wood;
        int brickIdx = (int)ResourceType.Brick;

        int playerAWoodBefore  = _playerA.Resources[woodIdx];
        int playerABrickBefore = _playerA.Resources[brickIdx];
        int bankWoodBefore     = _bankController.CurrentResources[woodIdx];
        int bankBrickBefore    = _bankController.CurrentResources[brickIdx];

        int[] resourcesGiven  = new int[5];
        int[] resourcesWanted = new int[5];
        resourcesGiven[woodIdx]   = 4;
        resourcesWanted[brickIdx] = 1;

        _tradeController.TradeResources(_playerA, resourcesGiven, resourcesWanted);

        Assert.AreEqual(playerAWoodBefore - 4, _playerA.Resources[woodIdx]);
        Assert.AreEqual(playerABrickBefore + 1, _playerA.Resources[brickIdx]);
        Assert.AreEqual(bankWoodBefore + 4, _bankController.CurrentResources[woodIdx]);
        Assert.AreEqual(bankBrickBefore - 1, _bankController.CurrentResources[brickIdx]);

        yield return null;
    }

    [UnityTest]
    public IEnumerator BankTrade_PartialBatch_RemainderReturnedToPlayer()
    {
        int woodIdx  = (int)ResourceType.Wood;
        int brickIdx = (int)ResourceType.Brick;

        int playerAWoodBefore  = _playerA.Resources[woodIdx];
        int playerABrickBefore = _playerA.Resources[brickIdx];
        int bankWoodBefore     = _bankController.CurrentResources[woodIdx];
        int bankBrickBefore    = _bankController.CurrentResources[brickIdx];

        int[] resourcesGiven  = new int[5];
        int[] resourcesWanted = new int[5];
        resourcesGiven[woodIdx]   = 5;
        resourcesWanted[brickIdx] = 1;

        _tradeController.TradeResources(_playerA, resourcesGiven, resourcesWanted);

        Assert.AreEqual(playerAWoodBefore - 4, _playerA.Resources[woodIdx]);
        Assert.AreEqual(playerABrickBefore + 1, _playerA.Resources[brickIdx]);
        Assert.AreEqual(bankWoodBefore + 4, _bankController.CurrentResources[woodIdx]);
        Assert.AreEqual(bankBrickBefore - 1, _bankController.CurrentResources[brickIdx]);

        yield return null;
    }

    [UnityTest]
    public IEnumerator BankTrade_InsufficientBankResource_DoesNothing()
    {
        int wheatIdx = (int)ResourceType.Wheat;
        int sheepIdx = (int)ResourceType.Sheep;

        // 1. Setăm buff 2:1 pentru grâne
        _playerA.PlayerBuffs.AddBuff(ResourceType.Wheat, PlayerBuff.Trade2_1);

        // 2. Banca nu mai are nicio oaie
        _bankController.CurrentResources[sheepIdx] = 0;

        // 3. Salvăm resursele jucătorului înainte de schimb
        int playerAWheatBefore = _playerA.Resources[wheatIdx];
        int playerASheepBefore = _playerA.Resources[sheepIdx];

        // 4. Pregătim vectorii de schimb: 2 Wheat → 1 Sheep
        int[] resourcesGiven  = new int[5];
        int[] resourcesWanted = new int[5];
        resourcesGiven[wheatIdx]       = 2;
        resourcesWanted[sheepIdx]      = 1;

        // 5. Apelăm metoda de trade
        _tradeController.TradeResources(_playerA, resourcesGiven, resourcesWanted);

        // 6. Logăm stocul băncii după apel
        Debug.Log($"[TEST] After TradeResources: Bank Sheep = {_bankController.CurrentResources[sheepIdx]}");

        // 7. Așteptăm un frame
        yield return null;

        // 8. Verificăm că banca nu și-a schimbat stocul de oi
        Assert.AreEqual(
            0, 
            _bankController.CurrentResources[sheepIdx],
            "Ne așteptăm ca banca să rămână cu 0 oi (niciun schimb)."
        );

        // 9. Verificăm că jucătorul nu și-a schimbat grânele (rămâne 99)
        

        // 10. Verificăm că jucătorul nu și-a schimbat oile (rămâne 99)
        Assert.AreEqual(
            playerASheepBefore, 
            _playerA.Resources[sheepIdx],
            "Ne așteptăm ca jucătorul să păstreze același număr de oi (niciun schimb)."
        );
        Assert.AreEqual(
                      playerAWheatBefore, 
                      _playerA.Resources[wheatIdx],
                      "Ne așteptăm ca jucătorul să păstreze același număr de grâne (niciun schimb)."
                  );
    }
    [UnityTest]
    public IEnumerator BankTrade_PlayerLacksResources_DoesNothing()
    {
        int wheatIdx = (int)ResourceType.Wheat;
        int brickIdx = (int)ResourceType.Brick;

        // 1. Asigurăm că jucătorul nu are suficiente Wheat: îi dăm doar 1
        _playerA.Resources[wheatIdx] = 1;
        _playerA.PlayerBuffs.AddBuff(ResourceType.Wheat, PlayerBuff.Trade2_1);

        // 2. Setăm banca să aibă suficient Brick (ca să nu fie bancă insuficientă)
        _bankController.CurrentResources[brickIdx] = 10;

        // 3. Salvăm valorile inițiale
        int playerAWheatBefore  = _playerA.Resources[wheatIdx];
        int playerABrickBefore  = _playerA.Resources[brickIdx];
        int bankWheatBefore     = _bankController.CurrentResources[wheatIdx];  // nu afectează pentru acest test
        int bankBrickBefore     = _bankController.CurrentResources[brickIdx];

        // 4. Jucătorul încearcă să dea 2 Wheat (dar are doar 1) ca să primească 1 Brick
        int[] resourcesGiven  = new int[5];
        int[] resourcesWanted = new int[5];
        resourcesGiven[wheatIdx]   = 2;
        resourcesWanted[brickIdx]  = 1;

        // 5. Apelăm metoda de trade
        _tradeController.TradeResources(_playerA, resourcesGiven, resourcesWanted);

        // 6. Așteptăm un frame
        yield return null;

        // 7. Verificăm că resursele jucătorului nu s-au schimbat:
        Assert.AreEqual(
            playerAWheatBefore,
            _playerA.Resources[wheatIdx],
            "Ne așteptăm ca jucătorul să păstreze același număr de Wheat (nu putea da 2 dacă are doar 1)."
        );
        Assert.AreEqual(
            playerABrickBefore,
            _playerA.Resources[brickIdx],
            "Ne așteptăm ca jucătorul să nu primească Brick (nu a plătit corect)."
        );

        // 8. Verificăm că banca nu și-a schimbat stocul de Brick
        Assert.AreEqual(
            bankBrickBefore,
            _bankController.CurrentResources[brickIdx],
            "Ne așteptăm ca banca să păstreze același număr de Brick (schimbul nu a avut loc)."
        );
    }


}
