using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageSystemMock : MonoBehaviour
{
    public TMP_Text messageText;

    public void OnEnable()
    {
        ClearMessage();
        GameManager.OnUpdateMessage += ReceiveMessage;
        Player.OnUpdateMessage += ReceiveMessage;
        MonopolyNode.OnUpdateMessage += ReceiveMessage;
        TradingSystem.OnUpdateMessage += ReceiveMessage;
    }

    public void OnDisable()
    {
        GameManager.OnUpdateMessage -= ReceiveMessage;
        Player.OnUpdateMessage -= ReceiveMessage;
        MonopolyNode.OnUpdateMessage -= ReceiveMessage;
        TradingSystem.OnUpdateMessage -= ReceiveMessage;
    }

    public void ReceiveMessage(string message)
    {
        messageText.text = message;
    }

    public void ClearMessage()
    {
        messageText.text = string.Empty;
    }
}