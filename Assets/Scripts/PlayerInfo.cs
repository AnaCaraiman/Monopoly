using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerCashText;

    public void SetPlayerName(string playerName)
    {
        playerNameText.text = playerName;
    }

    public void SetPlayerCash(int currentCash)
    {
        playerCashText.text = $"$ {currentCash}";
    }
    
    public void SetPlayerNameAndCash(string playerName, int currentCash)
    {
        SetPlayerName(playerName);
        SetPlayerCash(currentCash);
    }
    
}