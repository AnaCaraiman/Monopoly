using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEditor;

[System.Serializable]
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

    // PLAYER INFO
    private PlayerInfo myInfo;

    //AI
    private int aiMoneySavity = 200;

    //RETURN SOME INFOS
    public bool IsInJail => isInJail;
    public GameObject MyToken => myToken;
    public MonopolyNode MyMonopolyNode => currentNode;
    public int ReadMoney => money;
    
    //MESSAGE SYSTEM
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;

    public void InitializePlayer(MonopolyNode startingNode, int startMoney, PlayerInfo playerInfo, GameObject token)
    {
        currentNode = startingNode;
        money = startMoney;
        myInfo = playerInfo;
        myInfo.SetPlayerNameAndCash(name, money);
        myToken = token;    
    }

    public void SetMyCurrentNode(MonopolyNode node)
    {
        currentNode = node;
        node.PlayerLandedOnNode(this);
    }

    public void CollectMoney(int amount)
    {
        money+=amount;
        myInfo.SetPlayerCash(money);
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
        if(money < rentAmount)
        {
            //HANDLE INSUFFICIENT FUNDS > AI
        }
        money -= rentAmount;
        owner.CollectMoney(rentAmount);
        //UPDATE UI
        myInfo.SetPlayerCash(money);
    }

    internal void PayMoney(int amount)
    {
        if (money < amount)
        {
            //HANDLE INSUFFICIENT FUNDS > AI
        }
        money -= amount;

        //UPDATE UI
        myInfo.SetPlayerCash(money);
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
        int houses = 0;  //GOES TO INDEX 0
        int hotels = 0;  //GOES TO INDEX 1

        foreach (var node in myMonopolyNodes)
        {
            if(node.NumberOfHouses != 5)
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

}