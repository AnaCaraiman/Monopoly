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
    
    //RETURN SOME INFOS
    public bool IsInJail => isInJail;
    public GameObject MyToken => myToken;
    public MonopolyNode CurrentNode => currentNode;
    
    private int aiMoneySavity = 200;
}