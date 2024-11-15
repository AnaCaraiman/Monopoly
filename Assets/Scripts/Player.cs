using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

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
    public MonopolyNode CurrentNode => currentNode;

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
}