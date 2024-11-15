using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        node.playerLandedOnNode(this);
    }
}