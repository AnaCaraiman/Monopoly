using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;

public class MessageSystemEditModeTest
{
    private MessageSystemMock messageSystem;
    private TMP_Text messageText;

    [SetUp]
    public void Setup()
    {
        GameObject messageSystemObject = new GameObject("MessageSystem");
        messageSystem = messageSystemObject.AddComponent<MessageSystemMock>();

        GameObject messageTextObject = new GameObject("MessageText");
        messageText = messageTextObject.AddComponent<TextMeshProUGUI>();
        messageSystem.messageText = messageText;
    }

    [Test]
    public void ReceiveMessage_UpdatesMessageText()
    {
        // Arrange
        string testMessage = "Test Message";

        // Act
        messageSystem.ReceiveMessage(testMessage);

        // Assert
        Assert.AreEqual(testMessage, messageText.text);
    }

    [Test]
    public void ClearMessage_ClearsMessageText()
    {
        // Arrange
        messageSystem.ReceiveMessage("Test Message");

        // Act
        messageSystem.ClearMessage();

        // Assert
        Assert.AreEqual(string.Empty, messageText.text);
    }

    [Test]
    public void OnEnable_SubscribesToEvents()
    {
        // Arrange
        string testMessage = "Test Message";
        messageSystem.OnEnable();

        // Act
        Player.OnUpdateMessage?.Invoke(testMessage);

        // Assert
        Assert.AreEqual(testMessage, messageText.text);
    }

    [Test]
    public void OnDisable_UnsubscribesFromEvents()
    {
        // Arrange
        string testMessage = "Test Message";
        messageSystem.OnEnable();
        messageSystem.OnDisable();

        // Act
        Player.OnUpdateMessage?.Invoke(testMessage);

        // Assert
        Assert.AreEqual(string.Empty, messageText.text);
    }
}