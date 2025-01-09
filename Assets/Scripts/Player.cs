using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Player
{
    public enum PlayerType
    {
        Human,
        AI
    }

    public PlayerType playerType;
    public string name;
    private int money;
    MonopolyNode currentNode;
    private bool isInJail;
    int numTurnsInJail;
    [SerializeField] private GameObject myToken;
    [SerializeField] private List<MonopolyNode> myMonopolyNodes = new List<MonopolyNode>();
    public List<MonopolyNode> GetMonopolyNodes => myMonopolyNodes;

    // PLAYER INFO
    private PlayerInfo myInfo;

    //AI
    private int aiMoneySavity = 200;

    //AI STATES
    public enum AiStates
    {
       IDLE,
       TRADING
    }   

    public AiStates aiState;

    //RETURN SOME INFOS
    public bool IsInJail => isInJail;
    public GameObject MyToken => myToken;
    public MonopolyNode MyMonopolyNode => currentNode;
    public int ReadMoney => money;

    //MESSAGE SYSTEM
    public delegate void UpdateMessage(string message);

    public static UpdateMessage OnUpdateMessage;

    //HUMAN INOUT PANEL
    public delegate void ShowHumanPanel(bool activatePanel, bool activateRollDice, bool activateEndTurn);

    public static ShowHumanPanel OnShowHumanPanel;

    public void InitializePlayer(MonopolyNode startingNode, int startMoney, PlayerInfo playerInfo, GameObject token)
    {
        currentNode = startingNode;
        money = startMoney;
        myInfo = playerInfo;
        myInfo.SetPlayerNameAndCash(name, money);
        myToken = token;
        myInfo.ActivateArrow(false);
    }

    public void SetMyCurrentNode(MonopolyNode newNode)
    {
        currentNode = newNode;
        newNode.PlayerLandedOnNode(this);
        //IF ITS AI PLAYER
        if (playerType == PlayerType.AI)
        {
            //check if can build houses
            CheckIfPlayerHasASet();
            //check for unmortgaged properties
            UnMortgageProperties();
            //UnMortgageProperty();
            //TradingSystem.instance.FindMissingProperty(this);
        }
    }

    public void CollectMoney(int amount)
    {
        money += amount;
        myInfo.SetPlayerCash(money);
        if (playerType == PlayerType.Human && GameManager.instance.GetCurrentPlayer == this)
        {
            bool canEndTurn = !GameManager.instance.RolledADouble && ReadMoney >= 0 &&
                              GameManager.instance.HasRolledDice;
            bool canRollDice = GameManager.instance.RolledADouble && ReadMoney >= 0 &&
                               GameManager.instance.HasRolledDice;
            //SHOW UI
            OnShowHumanPanel.Invoke(true, canRollDice, canEndTurn);
        }
    }

    internal bool CanAffordNode(int price)
    {
        return price <= money;
    }

    public void BuyProperty(MonopolyNode node)
    {
        money -= node.price;
        node.SetOwner(this);
        //UPDATE UI
        myInfo.SetPlayerCash(money);
        //SET OWNERSHIP
        myMonopolyNodes.Add(node);
        //SORT ALL NODES BY PRICE
        SortPropertiesByPrice();
    }

    void SortPropertiesByPrice()
    {
        //NULL REFERENCE EXCEPTION SOLVED
        myMonopolyNodes = myMonopolyNodes
            .Where(_node => _node != null)
            .OrderBy(_node => _node.price)
            .ToList();
    }

    internal void PayRent(int rentAmount, Player owner)
    {
        //DON'T HAVE ENOUGH MONEY
        if (money < rentAmount)
        {
            if (playerType == PlayerType.AI)
            {
                //HANDLE INSUFFICIENT FUNDS > AI
                HandleInsufficientFunds(rentAmount);
            }
            else
            {
                //disable human turn and roll dice
                OnShowHumanPanel.Invoke(true, false, false);
            }
        }

        money -= rentAmount;
        owner.CollectMoney(rentAmount);
        //UPDATE UI
        myInfo.SetPlayerCash(money);
    }

    internal void PayMoney(int amount)
    {
        //dont have n\enough money
        if (money < amount)
        {
            if (playerType == PlayerType.AI)
            {
                //HANDLE INSUFFICIENT FUNDS > AI
                HandleInsufficientFunds(amount);
            }
            //else
            //{
            //    //disable human turn and roll dice
            //    OnShowHumanPanel.Invoke(true, false, false);
            //}
        }

        money -= amount;

        //UPDATE UI
        myInfo.SetPlayerCash(money);

        if (playerType == PlayerType.Human && GameManager.instance.GetCurrentPlayer == this)
        {
            bool canEndTurn = !GameManager.instance.RolledADouble && ReadMoney >= 0 &&
                              GameManager.instance.HasRolledDice;
            bool canRollDice = (GameManager.instance.RolledADouble && ReadMoney >= 0) ||
                               (!GameManager.instance.HasRolledDice && ReadMoney >= 0);
            //SHOW UI
            OnShowHumanPanel.Invoke(true, canRollDice, canEndTurn);
        }
    }

    //--------------------------------JAIL--------------------------------------
    public void GoToJail(int indexOnBoard)
    {
        isInJail = true;
        Debug.Log($"{name} is sent to jail!");

        //REPOSITION PLAYER 
        //myToken.transform.position = MonopolyBoard.instance.route[10].transform.position;
        //currentNode = MonopolyBoard.instance.route[10];
        MonopolyBoard.instance.MovePlayerToken(CalculateDistanceFromJail(indexOnBoard), this);
        GameManager.instance.ResetRolledADouble();
    }

    public void SetOutOfJail()
    {
        isInJail = false;
        //RESET TURNS IN JAIL
        numTurnsInJail = 0;
    }

    int CalculateDistanceFromJail(int indexOnBoard)
    {
        int result = 0;
        int indexOfJail = 10;
        if (indexOnBoard > indexOfJail)
        {
            result = (indexOnBoard - indexOfJail) * -1;
        }
        else
        {
            result = indexOfJail - indexOnBoard;
        }

        return result;
    }

    public int NumTurnsInJail => numTurnsInJail;

    public void IncreaseNumTurnsInJail()
    {
        numTurnsInJail++;
    }

    //STREET REPAIRS
    public int[] CountHousesAndHotels()
    {
        int houses = 0; //GOES TO INDEX 0
        int hotels = 0; //GOES TO INDEX 1

        foreach (var node in myMonopolyNodes)
        {
            if (node.NumberOfHouses != 5)
            {
                houses += node.NumberOfHouses;
            }
            else
            {
                hotels += 1;
            }
        }

        int[] allBuildings = new int[] { houses, hotels };
        return allBuildings;
    }

    //handle insufficient funds
    public void HandleInsufficientFunds(int amountToPay)
    {
        int housesToSell = 0;
        int allHouses = 0;
        int propertiesToMortgage = 0;
        int allPropertiesToMortgage = 0;

        //count all houses
        foreach (var node in myMonopolyNodes)
        {
            allHouses += node.NumberOfHouses;
        }

        //loop through all properties and try to sell as much as needed
        while (money < amountToPay && allHouses > 0)
        {
            foreach (var node in myMonopolyNodes)
            {
                housesToSell = node.NumberOfHouses;
                if (housesToSell > 0)
                {
                    CollectMoney(node.SellHouseOrHotel());
                    allHouses--;
                    //do we need more money?
                    if (money >= amountToPay)
                    {
                        return;
                    }
                }
            }
        }

        //MORTGAGE
        foreach (var node in myMonopolyNodes)
        {
            allPropertiesToMortgage += (!node.IsMortgaged) ? 1 : 0;
        }

        //loop through all properties and try to sell as much as needed
        while (money < amountToPay && allPropertiesToMortgage > 0)
        {
            foreach (var node in myMonopolyNodes)
            {
                propertiesToMortgage = (!node.IsMortgaged) ? 1 : 0;
                if (propertiesToMortgage > 0)
                {
                    CollectMoney(node.MortgageProperty());
                    allPropertiesToMortgage--;
                    //do we need more money?
                    if (money >= amountToPay)
                    {
                        return;
                    }
                }
            }
        }

        //we go bankrupt if we reach this point
        Bankrupt();
    }

    internal void Bankrupt()
    {
        //REMOVE PLAYER FROM THE GAME
        //GameManager.instance.RemovePlayer(this);

        //SEND A MESSAGE TO THE SYSTEM
        OnUpdateMessage?.Invoke($"{name} is bankrupt!");

        //clear all what the player has owned
        for (int i = myMonopolyNodes.Count - 1; i >= 0; i--)
        {
            if (myMonopolyNodes[i] != null)
            {
                myMonopolyNodes[i].ResetNode();
            }
            
        }

        //remove the player from the game
        GameManager.instance.RemovePlayer(this);
    }

    //--------------------------------CHECK IF PLAYER HAS A PROPERTY SET--------------------------------------
    void CheckIfPlayerHasASet()
    {
        foreach (var node in myMonopolyNodes)
        {
            var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(node);

            if (!allSame)
            {
                continue;
            }

            List<MonopolyNode> nodeSets = list;
            if (nodeSets != null)
            {
                bool hasMordgadedNode = nodeSets.Any(_node => _node.IsMortgaged) ? true : false;
                if (!hasMordgadedNode)
                {
                    if (nodeSets[0].monopolyNodeType == MonopolyNodeType.Property)
                    {
                        //WE COULD BUILD A HOUSE ON THIS SET
                        BuildHousesOrHotelEvenly(nodeSets);
                    }
                }
            }
        }
    }

    void UnMortgageProperties()
    {
        //for AI
        foreach (var node in myMonopolyNodes)
        {
            if (node && node.IsMortgaged)
            {
                int cost = node.MortgageValue + (int)(node.MortgageValue * 0.1f); //10% interest
                //can we afford to unmortgage?
                if (money >= aiMoneySavity + cost)
                {
                    PayMoney(cost);
                    node.UnMortgageProperty();
                }
            }
        }
    }

    //--------------------------------BUILD HOUSES EVENLY ON NODE SETS--------------------------------------
    internal void BuildHousesOrHotelEvenly(List<MonopolyNode> nodesToBuildOn)
    {
        int minHouses = int.MaxValue;
        int maxHouses = int.MinValue;
        //GET MIN AND MAX NUMBER OF HOUSES CURRENTLY ON THE PROPERTIES
        foreach (var node in nodesToBuildOn)
        {
            int numberOfHouses = node.NumberOfHouses;
            if (numberOfHouses < minHouses)
            {
                minHouses = numberOfHouses;
            }

            if (numberOfHouses > maxHouses)
            {
                maxHouses = numberOfHouses;
            }
        }

        //BUY HOUSES ON THE PROPERTIES FOR MAX ALLOWED ON THE PROPERTIES
        foreach (var node in nodesToBuildOn)
        {
            if (node.NumberOfHouses == minHouses && node.NumberOfHouses < 5 && CanAffordHouse(node.houseCost))
            {
                Debug.Log($"{name} is building a house on {node.name} & {node.price}");
                OnUpdateMessage?.Invoke($"{name} is building a house on {node.name}");
                node.BuildHouseOrHotel();
                PayMoney(node.houseCost);
            }
        }
    }

    internal void SellHouseEvenly(List<MonopolyNode> nodesToSellFrom)
    {
        int minHouses = int.MaxValue;
        bool houseSold = false;
        foreach (var node in nodesToSellFrom)
        {
            minHouses = Mathf.Min(minHouses, node.NumberOfHouses);
        }

        //SELL HOUSE
        for (int i = nodesToSellFrom.Count - 1; i >= 0; i--)
        {
            if (nodesToSellFrom[i].NumberOfHouses > minHouses)
            {
                CollectMoney(nodesToSellFrom[i].SellHouseOrHotel());
                houseSold = true;
                break;
            }
        }

        if (!houseSold)
        {
            CollectMoney(nodesToSellFrom[nodesToSellFrom.Count - 1].SellHouseOrHotel());
        }
    }

    //--------------------------------HOUSES AND HOTELS - CAN AFFORD AND COUNT--------------------------------------
    public bool CanAffordHouse(int price)
    {
        if (playerType == PlayerType.AI)
        {
            return (money - aiMoneySavity) >= price;
        }

        //HUMAN ONLY
        return money >= price;
    }

    public void ActivateSelector(bool active)
    {
        myInfo.ActivateArrow(active);
    }

    public void addProperty(MonopolyNode node)
    {
        myMonopolyNodes.Add(node);
        SortPropertiesByPrice();
    }

    public void RemoveProperty(MonopolyNode node)
    {
        myMonopolyNodes.Remove(node);
        SortPropertiesByPrice();
    }

    public void ChangeState(AiStates state)
    {   if(playerType==PlayerType.Human)
        {
            return;
        }
        aiState = state;
        switch(aiState)
        {
            case AiStates.IDLE:
                {
                    //CONTINUE THE GAME
                    GameManager.instance.Continue();
                    
                }
                break;
            case AiStates.TRADING:
                {
                    //HOLD THE GAME UNTIL CONTINUED
                    TradingSystem.instance.FindMissingProperty(this);
                }
                break;
        }

    }
   
}