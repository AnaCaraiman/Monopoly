using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfoMock : MonoBehaviour
{
    public TMP_Text playerNameText;
    public TMP_Text playerCashText;
    public GameObject activePlayerArrow;

    public void SetPlayerName(string playerName)
    {
        playerNameText.text = playerName;
    }

    public void SetPlayerCash(int currentCash)
    {
        playerCashText.text = $"{currentCash} RON";
    }

    public void SetPlayerNameAndCash(string playerName, int currentCash)
    {
        SetPlayerName(playerName);
        SetPlayerCash(currentCash);
    }

    public void ActivateArrow(bool active)
    {
        activePlayerArrow.SetActive(active);
    }
}