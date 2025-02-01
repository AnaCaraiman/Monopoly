using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;

public class PlayerInfoEditModeTest
{
    private PlayerInfoMock playerInfo;
    private TMP_Text playerNameText;
    private TMP_Text playerCashText;
    private GameObject activePlayerArrow;

    [SetUp]
    public void Setup()
    {
        GameObject playerInfoObject = new GameObject("PlayerInfo");
        playerInfo = playerInfoObject.AddComponent<PlayerInfoMock>();

        GameObject playerNameTextObject = new GameObject("PlayerNameText");
        playerNameText = playerNameTextObject.AddComponent<TextMeshProUGUI>();
        playerInfo.playerNameText = playerNameText;

        GameObject playerCashTextObject = new GameObject("PlayerCashText");
        playerCashText = playerCashTextObject.AddComponent<TextMeshProUGUI>();
        playerInfo.playerCashText = playerCashText;

        activePlayerArrow = new GameObject("ActivePlayerArrow");
        playerInfo.activePlayerArrow = activePlayerArrow;
    }

    [Test]
    public void SetPlayerName_UpdatesPlayerNameText()
    {
        string testName = "Test Player";
        playerInfo.SetPlayerName(testName);
        Assert.AreEqual(testName, playerNameText.text);
    }

    [Test]
    public void SetPlayerCash_UpdatesPlayerCashText()
    {
        int testCash = 1500;
        playerInfo.SetPlayerCash(testCash);
        Assert.AreEqual($"{testCash} RON", playerCashText.text);
    }

    [Test]
    public void SetPlayerNameAndCash_UpdatesPlayerNameAndCashText()
    {
        string testName = "Test Player";
        int testCash = 1500;
        playerInfo.SetPlayerNameAndCash(testName, testCash);
        Assert.AreEqual(testName, playerNameText.text);
        Assert.AreEqual($"{testCash} RON", playerCashText.text);
    }

    [Test]
    public void ActivateArrow_SetsActivePlayerArrow()
    {
        playerInfo.ActivateArrow(true);
        Assert.IsTrue(activePlayerArrow.activeSelf);
        playerInfo.ActivateArrow(false);
        Assert.IsFalse(activePlayerArrow.activeSelf);
    }
}