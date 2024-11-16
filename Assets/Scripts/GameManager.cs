using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private MonopolyBoard gameBoard;
    [SerializeField] private List<Player> playerList = new List<Player>();
    [SerializeField] private int currentPlayerIndex;

    [Header("Globla Game Settings")] [SerializeField]
    private int maxTurnsInJail = 3;
    [SerializeField] private int startMoney = 1500;
    [SerializeField] private int goMoney = 500;

    [Header("Player Info")] [SerializeField]
    GameObject playerInfoPrefab;
    [SerializeField] private Transform playerPanel; // Where the player info will be displayed
    [SerializeField] private List<GameObject> playerTokenList = new List<GameObject>();

    private int[] rolledDice;
    private bool rolledADouble;
    private int doubleRollCount;
    //tax pool
    int taxPool = 0;

    public int GetGoMoney => goMoney;
    

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Initialize();
        if(playerList[currentPlayerIndex].playerType == Player.PlayerType.AI) {
            RollDice();
        }
        else {
            //
        }
    }

    void Initialize()
    {
        // Initialize the players
        for (int i = 0; i < playerList.Count; i++)
        {
            GameObject playerInfo = Instantiate(playerInfoPrefab, playerPanel, false);
            PlayerInfo playerInfoComponent = playerInfo.GetComponent<PlayerInfo>();

            // Randomize the color of the player token
            int randomIndex = Random.Range(0, playerTokenList.Count);
            GameObject newToken = Instantiate(playerTokenList[randomIndex], gameBoard.route[0].transform.position,
                Quaternion.identity);
            playerList[i].InitializePlayer(gameBoard.route[0], startMoney, playerInfoComponent, newToken);
        }
    }

    public void RollDice()
    {
        bool allowedToMove = true;
        //reset last roll
        rolledDice = new int[2];
        rolledDice[0] = Random.Range(1, 7);
        rolledDice[1] = Random.Range(1, 7);
        //chech for double
        rolledADouble = rolledDice[0] == rolledDice[1];
        //throw 3 times in a row(double) -> JAIL ANYHOW -> END TURN

        //IS IN JAIL ALREADY
        if (playerList[currentPlayerIndex].IsInJail)
        {
            if(rolledADouble)
            {
                playerList[currentPlayerIndex].SetOutOfJail();
                //MOVE THE PLAYER

            }
            else
            {
                allowedToMove = false;
            }
        }
        else
        {
            //RESET DOUBLE ROLLS
            if(!rolledADouble)
            {
                doubleRollCount = 0;
            }
            else
            {
                doubleRollCount++;
                if(doubleRollCount >= 3)
                {
                    //MOVE TO JAIL
                    
                    return;
                }
            }
        }

        //CAN LEAVE JAIL

        //test dice
        rolledDice[0] = 15;
        rolledDice[1] = 15;
        if(allowedToMove)
        {
            StartCoroutine(DelayBeforMove(rolledDice[0] + rolledDice[1]));
        }
        else
        {
            //SWITCH PLAYER
        }

        Debug.Log($"Rolled dice: {rolledDice[0]} and {rolledDice[1]}");

    }

    IEnumerator DelayBeforMove(int rolledDice)
    {
        yield return new WaitForSeconds(2f);

        gameBoard.MovePlayerToken(rolledDice, playerList[currentPlayerIndex]);
    }

    public void SwitchPlayers(){
        currentPlayerIndex++;
        //rolled double
        doubleRollCount = 0;

        if(currentPlayerIndex >= playerList.Count) {
            currentPlayerIndex = 0;
        }

        if(playerList[currentPlayerIndex].playerType == Player.PlayerType.AI) {
            RollDice();
        }
    }

    public int[] LastRolledDice => rolledDice;

    public void AddTaxToPool(int amount)
    {
        taxPool += amount;
    }

    public int GetTaxPool()
    {
        int currentTaxCollected = taxPool;
        taxPool = 0;
        return currentTaxCollected;
    }
   
}