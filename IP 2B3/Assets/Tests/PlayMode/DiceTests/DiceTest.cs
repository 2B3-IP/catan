using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using B3.DiceSystem;

public class DiceTest
{
    [UnitySetUp]
    public IEnumerator SetUp()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;
        float timeout = 5f;
        float timer = 0f;
        while (GameObject.FindObjectOfType<B3.DiceSystem.DiceThrower>() == null && timer < timeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Assert.Less(timer, timeout, "Timeout: DiceThrower nu s-a încărcat în timp util.");
    }

    [UnityTest]
    public IEnumerator DiceRolls_AreInValidRange()
    {
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("SampleScene");

        yield return new WaitForSeconds(20f);

        // Caută DiceThrower și apelează metoda `a()`
        var thrower = GameObject.FindObjectOfType<B3.DiceSystem.DiceThrower>();
        Assert.IsNotNull(thrower, "DiceThrower not found");

        thrower.a();

        // Așteaptă să termine aruncarea
        yield return new WaitForSeconds(10f);
        Debug.Log($"Rezultat total zaruri: {thrower.DiceRolls}");
        Assert.IsTrue(thrower.DiceRolls >= 2 && thrower.DiceRolls <= 12, $"Rezultat invalid: {thrower.DiceRolls}");
    }

    [UnityTest]
    [Timeout(2400000)]
    public IEnumerator DiceRoll_DistributionIsStatisticallyReasonable()
    {
        const int rollCount = 100;
        Dictionary<int, int> sumCounts = new();

        for (int sum = 2; sum <= 12; sum++)
            sumCounts[sum] = 0;

        var thrower = GameObject.FindObjectOfType<B3.DiceSystem.DiceThrower>();
        Assert.IsNotNull(thrower, "DiceThrower not found!");

        yield return new WaitForSeconds(20f);
        for (int i = 0; i < rollCount; i++)
        {
            thrower.a();
            yield return new WaitForSeconds(4f);

            // Așteaptă să se oprească zarurile
            yield return WaitUntilDiceStop(10f);

            int sum = thrower.DiceRolls;
            sumCounts[sum]++;
        }

        // Probabilitățile teoretice pentru fiecare sumă
        Dictionary<int, float> expectedProbabilities = new()
        {
            [2] = 1f / 36,
            [3] = 2f / 36,
            [4] = 3f / 36,
            [5] = 4f / 36,
            [6] = 5f / 36,
            [7] = 6f / 36,
            [8] = 5f / 36,
            [9] = 4f / 36,
            [10] = 3f / 36,
            [11] = 2f / 36,
            [12] = 1f / 36
        };

        // Testul Chi-Square
        float chiSquareValue = 0f;
        foreach (var kvp in expectedProbabilities)
        {
            int sum = kvp.Key;
            float expected = kvp.Value * rollCount;
            float actual = sumCounts[sum];
            float diff = Mathf.Abs(expected - actual);

            // Calculul Chi-Square
            chiSquareValue += Mathf.Pow(diff, 2) / expected;
        }

        Debug.Log($"Chi-Square Value: {chiSquareValue}");

        // Valoare critică pentru un nivel de semnificație de 0.05 și 10 grade de libertate (12 sume - 2)
        float criticalValue = 16.919f; // Valoare critică pentru 10 grade de libertate

        Assert.LessOrEqual(chiSquareValue, criticalValue, $"Chi-Square Value prea mare! Valoare calculată: {chiSquareValue}, Valoare critică: {criticalValue}");
    }

    private IEnumerator WaitUntilDiceStop(float timeout = 10f)
    {
        float timer = 0f;
        while (timer < timeout)
        {
            var moving = GameObject.FindObjectsOfType<Rigidbody>()
                .Any(rb => rb.linearVelocity.magnitude > 0.0001f || rb.angularVelocity.magnitude > 0.0001f);

            if (!moving)
                break;

            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
