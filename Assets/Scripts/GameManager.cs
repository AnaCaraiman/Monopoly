using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private MonopolyBoard gameBoard;
    [SerializeField] private List<Player> playerList = new List<Player>();
    [SerializeField] private int currentPlayer;

    [Header("Global Game Settings")]
    [SerializeField]
    private int maxTurnsInJail = 3;

    [SerializeField] private int startMoney = 1500;
    [SerializeField] private int goMoney = 500;
    [SerializeField] private float secondsBeetweenTurns = 3f;

    [Header("Player Info")]
    [SerializeField]
    GameObject playerInfoPrefab;

    [SerializeField] private Transform playerPanel; // Where the player info will be displayed
    [SerializeField] private List<GameObject> playerTokenList = new List<GameObject>();

    [Header("Game Over/ win Info")]
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TMP_Text winnerNameText;

    [Header("Dice")]
    [SerializeField] Dice _dice1;
    [SerializeField] Dice _dice2;

    List<int> rolledDice = new List<int>();
    private bool rolledADouble;
    public bool RolledADouble => rolledADouble;
    public void ResetRolledADouble() => rolledADouble = false;
    private int doubleRollCount;
    private bool hasRolledDice;
    public bool HasRolledDice => hasRolledDice;

    //tax pool
    int taxPool = 0;

    public int GetGoMoney => goMoney;
    public float SecondsBeetweenTurns => secondsBeetweenTurns;
    public List<Player> GetPlayers => playerList;
    public Player GetCurrentPlayer => playerList[currentPlayer];


    //MESSAGE SYSTEM
    public delegate void UpdateMessage(string message);

    public static UpdateMessage OnUpdateMessage;

    //HUMAN INPUT PANEL
    public delegate void ShowHumanPanel(bool activatePanel, bool activateRollDice, bool activateEndTurn, bool hasChanceJailCard, bool hasCommunityJailCard);

    public static ShowHumanPanel OnShowHumanPanel;

    //DEBUG
    [SerializeField] bool alwaysRollDouble = false;
    [SerializeField] bool forceDiceRolls;
    [SerializeField] int dice1;
    [SerializeField] int dice2;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentPlayer = Random.Range(0, playerList.Count);
        gameOverPanel.SetActive(false);
        Initialize();
        CameraSwitcher.instance.switchToTopDown();
        if (playerList[currentPlayer].playerType == Player.PlayerType.AI)
        {
            //RollDice();
            RollPhysicalDice();
        }
        else
        {
            //
            OnShowHumanPanel.Invoke(true, true, false, false, false);
        }
    }

    void Initialize()
    {
        if(GameSettings.settingsList.Count == 0)
        {
            Debug.LogError("Start game from menu");
            return;
        }
        foreach(var setting in GameSettings.settingsList)
        {
            Player player = new Player();
            player.name = setting.playerName;
            player.playerType = (Player.PlayerType)setting.selectedType;
            playerList.Add(player);

            GameObject playerInfo = Instantiate(playerInfoPrefab, playerPanel, false);
            PlayerInfo playerInfoComponent = playerInfo.GetComponent<PlayerInfo>();

             GameObject newToken = Instantiate(playerTokenList[setting.selectedColor], gameBoard.route[0].transform.position,
                Quaternion.identity);
            
            player.InitializePlayer(gameBoard.route[0], startMoney, playerInfoComponent, newToken);
        }
    
        playerList[currentPlayer].ActivateSelector(true);

        if (playerList[currentPlayer].playerType == Player.PlayerType.Human)
        {
            bool jail1 = playerList[currentPlayer].HasChanceJailFreeCard;
            bool jail2 = playerList[currentPlayer].HasCommunityJailFreeCard;
            OnShowHumanPanel.Invoke(true, true, false, jail1, jail2);
        }
        else
        {
            bool jail1 = playerList[currentPlayer].HasChanceJailFreeCard;
            bool jail2 = playerList[currentPlayer].HasCommunityJailFreeCard;
            OnShowHumanPanel.Invoke(false, false, false, jail1, jail2);
        }
    }

    public void RollPhysicalDice()
    {
        CheckForJailFree();
        rolledDice.Clear();
        _dice1.RollDice();
        _dice2.RollDice();
        CameraSwitcher.instance.switchToDice();

        //SHOW OR HIDE UI
        if (playerList[currentPlayer].playerType == Player.PlayerType.Human)
        {
            bool jail1 = playerList[currentPlayer].HasChanceJailFreeCard;
            bool jail2 = playerList[currentPlayer].HasCommunityJailFreeCard;
            OnShowHumanPanel.Invoke(true, false, false, jail1, jail2);
        }
    }

    void CheckForJailFree()
    {
        //JAIL FREE CARD
        if (playerList[currentPlayer].IsInJail && playerList[currentPlayer].playerType == Player.PlayerType.AI)
        {
            if (playerList[currentPlayer].HasChanceJailFreeCard)
            {
                playerList[currentPlayer].UseChanceJailFreeCard();
            }
            else if (playerList[currentPlayer].HasCommunityJailFreeCard)
            {
                playerList[currentPlayer].UseCommunityJailFreeCard();
            }
        }
    }


    public void ReportDiceRolled(int diceValue)
    {
        rolledDice.Add(diceValue);
        if (rolledDice.Count == 2)
        {
            RollDice();
        }
    }

    void RollDice()
    {
        bool allowedToMove = true;
        hasRolledDice = true;

        
        //RESET LAST ROLL
        //rolledDice = new int[2];

        //rolledDice[0] = Random.Range(1, 7);
        //rolledDice[1] = Random.Range(1, 7);
        //rolledDice[0] = 3;
        //rolledDice[1] = 2;

        Debug.Log($"{playerList[currentPlayer].name} Rolled dice: {rolledDice[0]} and {rolledDice[1]}");

        //DEBUG
       // if (alwaysRollDouble)
       // {
       //     rolledDice[0] = 1;
       //     rolledDice[1] = 1;
       // }
//
       // if (forceDiceRolls)
        //{
        //    rolledDice[0] = dice1;
        //    rolledDice[1] = dice2;
        //}

        //CHECK FOR DOUBLES
        rolledADouble = rolledDice[0] == rolledDice[1];
        //THROW 3 TIMES IN A ROW -> JAIL -> END TURN

        //IS IN JAIL ALREADY
        if (playerList[currentPlayer].IsInJail)
        {
            playerList[currentPlayer].IncreaseNumTurnsInJail();
            if (rolledADouble)
            {
                playerList[currentPlayer].SetOutOfJail();
                OnUpdateMessage?.Invoke(
                    $"{playerList[currentPlayer].name} rolled a <b>double</b> and <b><color=green>is out of jail!</color></b>");
                doubleRollCount++;
                //MOVE PLAYER
            }
            else if (playerList[currentPlayer].NumTurnsInJail >= maxTurnsInJail)
            {
                //ALLOWED TO LEAVE
                playerList[currentPlayer].SetOutOfJail();
                OnUpdateMessage?.Invoke(
                    $"{playerList[currentPlayer].name} has been in jail for <b>{maxTurnsInJail} turns</b> and <b><color=green>is out of jail!</color></b>");
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
                if (doubleRollCount >= 3)
                {
                    //MOVE TO JAIL
                    int indexOnBoard = MonopolyBoard.instance.route.IndexOf(playerList[currentPlayer].MyMonopolyNode);
                    playerList[currentPlayer].GoToJail(indexOnBoard);

                    OnUpdateMessage?.Invoke(
                        $"{playerList[currentPlayer].name} rolled <b>3 doubles</b> in a row and <b><color=red>is sent to jail!</color></b>");

                    rolledADouble = false;
                    return;
                }
            }
        }

        //LEAVE JAIL

        //MOVE IF ALLOWED
        if (allowedToMove)
        {
            OnUpdateMessage?.Invoke(
                $"{playerList[currentPlayer].name} rolled a <b>{rolledDice[0] + rolledDice[1]}</b> and is moving...");
            StartCoroutine(DelayBeforeMove(rolledDice[0] + rolledDice[1]));
        }
        else
        {
            //SWITCH PLAYER
            OnUpdateMessage?.Invoke(
                $"{playerList[currentPlayer].name} rolled a <b>{rolledDice[0]} & {rolledDice[1]}</b> and <b><color=red>is still in jail!</color></b>");
            StartCoroutine(DeleyBeforeSwitchPlayer());
        }

    }

    IEnumerator DelayBeforeMove(int rolledDice)
    {
        CameraSwitcher.instance.switchToPlayer(playerList[currentPlayer].MyToken.transform);
        yield return new WaitForSeconds(secondsBeetweenTurns);
        gameBoard.MovePlayerToken(rolledDice, playerList[currentPlayer]);
    }

    IEnumerator DeleyBeforeSwitchPlayer()
    {
        yield return new WaitForSeconds(secondsBeetweenTurns);
        SwitchPlayers();
    }

    public void SwitchPlayers()
    {
        CameraSwitcher.instance.switchToTopDown();
        currentPlayer++;
        hasRolledDice = false;

        doubleRollCount = 0;
        if (currentPlayer >= playerList.Count)
        {
            currentPlayer = 0;
        }

        DeactivateArrows();
        playerList[currentPlayer].ActivateSelector(true);

        if (playerList[currentPlayer].playerType == Player.PlayerType.AI)
        {
            //RollDice(); 
            RollPhysicalDice();
            OnShowHumanPanel.Invoke(false, false, false, false, false);
        }
        else //if human - show ui
        {
            bool jail1 = playerList[currentPlayer].HasChanceJailFreeCard;
            bool jail2 = playerList[currentPlayer].HasCommunityJailFreeCard;
            OnShowHumanPanel.Invoke(true, true, false, jail1, jail2);
        }
    }

    public List<int> LastRolledDice => rolledDice;

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

    public void RemovePlayer(Player player)
    {
        playerList.Remove(player);
        //check for game over
        CheckForGameOver();
    }

    void CheckForGameOver()
    {
        if (playerList.Count == 1)
        {
            Debug.Log(playerList[0].name + "IS THE WINNER!");
            OnUpdateMessage?.Invoke(playerList[0].name + "IS THE WINNER!");
            //STOP THE GAME LOOP ANYHOW

            //SHOW UI
            gameOverPanel.SetActive(true);
            winnerNameText.text = playerList[0].name;
        }
    }

    void DeactivateArrows()
    {
        foreach (Player player in playerList)
        {
            player.ActivateSelector(false);
        }
    }

    public void Continue()
    {
        if (playerList.Count > 1)
        {
            Invoke("ContinueGame", SecondsBeetweenTurns);
        }
    }
    void ContinueGame()
    {
        if (RolledADouble)
        {
            //ROLL AGAIN
            //RollDice();
            RollPhysicalDice();
        }
        else
        {
            if (playerList.Count > 1)
            {
                //SWITCH PLAYER
                SwitchPlayers();
            }

        }
    }


    //HUMAN BANKRUPT
    public void HumanBankrupcy()
    {
        playerList[currentPlayer].Bankrupt();
    }


    //JAIL FREE CARDS BUTTONS
    public void UseJail1Card() // CHANCE JAIL CARD
    {
        playerList[currentPlayer].UseChanceJailFreeCard();
    }

    public void UseJail2Card()
    {
        playerList[currentPlayer].UseCommunityJailFreeCard();
    }
}
