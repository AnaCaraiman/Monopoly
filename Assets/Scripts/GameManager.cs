using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [SerializeField] private MonopolyBoard gameBoard;
    [SerializeField] private List<Player> playerList = new List<Player>();
    [SerializeField] private int currentPlayerIndex;
    [SerializeField] private int maxTurnsInJail = 3;
    [SerializeField] private int startMoney = 1500;
    [SerializeField] private int goMoney = 500;
    
    [SerializeField] GameObject playerInfoPrefab;
    [SerializeField] private Transform playerPanel; // Where the player info will be displayed
    void Awake()
    {
        // if(instance == null)
        // {
            instance = this;
        // }
        // else
        // {
        //     Destroy(this);
        // }
    }

    private void Start()
    {
        // Initialize the players
        for (int i = 0; i < playerList.Count; i++)
        {
            GameObject playerInfo = Instantiate(playerInfoPrefab, playerPanel, false);
            PlayerInfo playerInfoComponent = playerInfo.GetComponent<PlayerInfo>();
            playerList[i].InitializePlayer(gameBoard.route[0], startMoney, playerInfoComponent);
        }
        
        // Display the player info
        // for (int i = 0; i < playerList.Count; i++)
        // {
        //     GameObject playerInfo = Instantiate(playerInfoPrefab, playerPanel);
        //     playerInfo.GetComponent<PlayerInfo>().SetPlayerNameAndCash(playerList[i].name, playerList[i].Money);
        // }
    }
}

