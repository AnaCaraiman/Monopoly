using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public class GameManagerEditModeTest
{
    private GameObject gmObject;
    private GameManager gameManager;
    private GameObject dice1Object, dice2Object;
    private Dice dice1, dice2;

    [SetUp]
    public void Setup()
    {
        gmObject = new GameObject("GameManager");
        gameManager = gmObject.AddComponent<GameManager>();

        dice1Object = new GameObject("Dice1");
        dice1 = dice1Object.AddComponent<Dice>();

        dice2Object = new GameObject("Dice2");
        dice2 = dice2Object.AddComponent<Dice>();


        typeof(GameManager).GetField("_dice1", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameManager, dice1);
        typeof(GameManager).GetField("_dice2", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameManager, dice2);

        typeof(GameManager).GetField("playerList", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameManager, new List<Player>());
    }

    [TearDown]
    public void Teardown()
    {
        GameObject.DestroyImmediate(gmObject);
        GameObject.DestroyImmediate(dice1Object);
        GameObject.DestroyImmediate(dice2Object);
    }

    [Test]
    public void GameManager_InitializesCorrectly()
    {
        Assert.IsNotNull(gameManager, "GameManager ar trebui să fie inițializat corect.");
    }

    [Test]
    public void RollPhysicalDice_CallsDiceRoll()
    {
        gameManager.RollPhysicalDice();
        
        Assert.IsTrue(dice1.enabled, "Primul zar trebuie să fie activ.");
        Assert.IsTrue(dice2.enabled, "Al doilea zar trebuie să fie activ.");
    }

    [Test]
    public void ReportDiceRolled_AddsDiceValue()
    {
        gameManager.ReportDiceRolled(4);
        gameManager.ReportDiceRolled(3);

        List<int> rolledDice = (List<int>)typeof(GameManager).GetField("rolledDice", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(gameManager);

        Assert.AreEqual(2, rolledDice.Count, "Două valori ar trebui adăugate după două aruncări.");
        Assert.AreEqual(4, rolledDice[0]);
        Assert.AreEqual(3, rolledDice[1]);
    }

    [Test]
    public void ResetRolledADouble_WorksCorrectly()
    {
        typeof(GameManager).GetField("rolledADouble", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(gameManager, true);
        
        gameManager.ResetRolledADouble();
        
        bool rolledADouble = (bool)typeof(GameManager).GetField("rolledADouble", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(gameManager);
        Assert.IsFalse(rolledADouble, "RolledADouble ar trebui să fie resetat la false.");
    }
}
