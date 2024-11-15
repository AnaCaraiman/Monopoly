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
    

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Initialize();
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
        rolledDice = new int[2];
        rolledDice[0] = Random.Range(1, 7);
        rolledDice[1] = Random.Range(1, 7);

        rolledADouble = rolledDice[0] == rolledDice[1];
    }

    IEnumerator DelayBeforMove()
    {
        yield return new WaitForSeconds(2f);
    }
}