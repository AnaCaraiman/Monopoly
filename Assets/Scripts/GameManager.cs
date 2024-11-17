using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private MonopolyBoard gameBoard;
    [SerializeField] private List<Player> playerList = new List<Player>();
    [SerializeField] private int currentPlayer;

    [Header("Globla Game Settings")] [SerializeField]
    private int maxTurnsInJail = 3;
    [SerializeField] private int startMoney = 1500;
    [SerializeField] private int goMoney = 500;
    [SerializeField] private float secondsBeetweenTurns = 3f;
    
    [Header("Player Info")] [SerializeField]
    GameObject playerInfoPrefab;
    [SerializeField] private Transform playerPanel; // Where the player info will be displayed
    [SerializeField] private List<GameObject> playerTokenList = new List<GameObject>();

    private int[] rolledDice;
    private bool rolledADouble;
    public bool RolledADouble => rolledADouble;
    private int doubleRollCount;
    //tax pool
    int taxPool = 0;

    public int GetGoMoney => goMoney;
    public float SecondsBeetweenTurns => secondsBeetweenTurns;
    

    //DEBUG
    public bool alwaysRollDouble = false;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Initialize();
        if(playerList[currentPlayer].playerType == Player.PlayerType.AI) {
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
        //RESET LAST ROLL
        rolledDice = new int[2];
        rolledDice[0] = Random.Range(1, 7);
        rolledDice[1] = Random.Range(1, 7);

        Debug.Log($"{playerList[currentPlayer].name} Rolled dice: {rolledDice[0]} and {rolledDice[1]}");


        //DEBUG
        if (alwaysRollDouble)
        {
            rolledDice[0] = 2;
            rolledDice[1] = 2;
        }

        //CHECK FOR DOUBLES
        rolledADouble = rolledDice[0] == rolledDice[1];
        //THROW 3 TIMES IN A ROW -> JAIL -> END TURN

        //IS IN JAIL ALREADY
        if(playerList[currentPlayer].IsInJail)
        {
            playerList[currentPlayer].IncreaseNumTurnsInJail();
            if (rolledADouble)
            {
                playerList[currentPlayer].SetOutOfJail();
                doubleRollCount++;
                //MOVE PLAYER
            }
            else if (playerList[currentPlayer].NumTurnsInJail >= maxTurnsInJail)
            {
                //ALLOWED TO LEAVE
                playerList[currentPlayer].SetOutOfJail();
            }
            else
            {
                allowedToMove = false;
            }
        }
        else
        {
            //RESET DOUBLE ROLL COUNT
            if (!rolledADouble)
            {
                doubleRollCount = 0;
            }
            else
            {
                doubleRollCount++;
                if(doubleRollCount >= 3)
                {
                    //MOVE TO JAIL
                    int indexOnBoard = MonopolyBoard.instance.route.IndexOf(playerList[currentPlayer].MyMonopolyNode);
                    playerList[currentPlayer].GoToJail(indexOnBoard);
                    rolledADouble = false;
                    return;
                }
            }
        }

        //LEAVE JAIL

        //MOVE IF ALLOWED
        if(allowedToMove)
        {
            StartCoroutine(DelayBeforMove(rolledDice[0] + rolledDice[1]));
        }
        else
        {
            //SWITCH PLAYER
            Debug.Log("We are not allowed to move");
            StartCoroutine(DeleyBeforeSwitchPlayer());
        }

        //SHOW OR HIDE UI
    }

    IEnumerator DelayBeforMove(int rolledDice)
    {
        yield return new WaitForSeconds(secondsBeetweenTurns);
        gameBoard.MovePlayerToken(rolledDice, playerList[currentPlayer]);
    }
    
    IEnumerator DeleyBeforeSwitchPlayer()
    {
        yield return new WaitForSeconds(secondsBeetweenTurns);
        SwitchPlayers();
    }

    public void SwitchPlayers(){
        currentPlayer++;
        doubleRollCount = 0;
        if (currentPlayer >= playerList.Count) {
            currentPlayer = 0;
        }

        if(playerList[currentPlayer].playerType == Player.PlayerType.AI) {
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