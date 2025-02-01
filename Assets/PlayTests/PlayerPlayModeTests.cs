using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerPlayModeTests
{
    private Player player;
    private PlayerInfo playerInfo;
    private MonopolyNode startingNode;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Reset the dummy singleton instances.
        GameManager.instance = new GameManager();
        MonopolyBoard.instance = new MonopolyBoard();
        TradingSystem.instance = new TradingSystem();
        CommunityChest.instance = new CommunityChest();
        ChanceField.instance = new ChanceField();

        // Create a dummy starting node and token.
        startingNode = new MonopolyNode();
        playerInfo = new PlayerInfo();
        player = new Player();
        player.name = "TestPlayerPlayMode";
        GameObject token = new GameObject("TokenPlayMode");
        player.InitializePlayer(startingNode, 1000, playerInfo, token);

        // Wait one frame so that any UI updates or events (if used) can process.
        yield return null;
    }

    [UnityTearDown]
    public IEnumerator Teardown()
    {
        Object.Destroy(player.MyToken);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestTokenInitialization()
    {
        Assert.IsNotNull(player.MyToken);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestMoneyUIUpdate()
    {
        int initialMoney = player.ReadMoney;
        player.CollectMoney(100);
        yield return null; // wait a frame if UI is updated asynchronously
        Assert.AreEqual(initialMoney + 100, player.ReadMoney);
        Assert.AreEqual(initialMoney + 100, playerInfo.cash);
    }
}