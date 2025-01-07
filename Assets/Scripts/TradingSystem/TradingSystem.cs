using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;
using UnityEngine.UI;
public class TradingSystem : MonoBehaviour
{
    public static TradingSystem instance;

    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject tradePanel;

    [Header("LEFT SIDE")]
    [SerializeField] TMP_Text leftOffererNameText;
    [SerializeField] Transform leftCardGrid;
    [SerializeField] ToggleGroup leftToggleGroup;
    [SerializeField] TMP_Text leftYourMoneyText;
    [SerializeField] TMP_Text leftOfferMoney;
    [SerializeField] Slider leftMoneySlider;
    List<GameObject> leftCardPrefabList = new List<GameObject>();
    int leftChosenMoneyAmount;
    MonopolyNode leftSelectedNode;
    Player leftPlayerReference;

    [Header("MIDDLE")]
    [SerializeField] Transform buttonGrid;
    [SerializeField] GameObject playerButtonPrefab;

    List<GameObject> playerButtonList = new List<GameObject>();

    [Header("RIGHT SIDE")]
    [SerializeField] TMP_Text rightOffererNameText;
    [SerializeField] Transform rightCardGrid;
    [SerializeField] ToggleGroup rightToggleGroup;
    [SerializeField] TMP_Text rightYourMoneyText;
    [SerializeField] TMP_Text rightOfferMoney;
    [SerializeField] Slider rightMoneySlider;
    List<GameObject> rightCardPrefabList = new List<GameObject>();
    int rightChosenMoneyAmount;
    MonopolyNode rightSelectedNode;
    Player rightPlayerReference;

    //MESSAGE SYSTEM
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;
    void Awake() 
    {
        instance = this;
    }
    void Start()
    {
        tradePanel.SetActive(false);
    }

    public void FindMissingProperty(Player currentPlayer)
    {
        List<MonopolyNode> processedSet = null;
        MonopolyNode requestedNode = null;
        foreach (var node in currentPlayer.GetMonopolyNodes)
        {
            var (list,allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(node);
            List <MonopolyNode> nodeSet = new List<MonopolyNode>();
            nodeSet.AddRange(list);
            //if all have been purchased
            bool notAllPurchased = list.Any(n => n.Owner == null);

            if(allSame || processedSet == list || notAllPurchased)
            {
                processedSet = list;
                continue;
            }
            //AI owns this set already
            if(allSame || processedSet == list)
            {
                processedSet = list;
                continue;
            }
            if(list.Count == 2) 
            {
                requestedNode = list.Find(n => n.Owner != currentPlayer && n.Owner != null);
                if(requestedNode != null)
                {
                   MakeTradeDecision(currentPlayer, requestedNode.Owner, requestedNode);
                    return;
                }
            }
            if(list.Count >= 3)
            {
                int hasMostOfSet = list.Count(n => n.Owner == currentPlayer);
                if (hasMostOfSet >= 2)
                {
                    requestedNode = list.Find(n => n.Owner != currentPlayer && n.Owner != null);
                    MakeTradeDecision(currentPlayer, requestedNode.Owner, requestedNode);
                    break;
                }
            }
        }
    }

    void MakeTradeDecision(Player currentPlayer, Player nodeOwner,MonopolyNode requestedNode)
    {
        if(currentPlayer.ReadMoney >= CalculateValueOfNode(requestedNode))
        {
            MakeTradeOffer(currentPlayer,nodeOwner,requestedNode, null, CalculateValueOfNode(requestedNode), 0);
            return;
        }

        foreach (var node in currentPlayer.GetMonopolyNodes)
        {
            var checkedSet = MonopolyBoard.instance.PlayerHasAllNodesOfSet(node).list;
            if(checkedSet.Contains(requestedNode))
            {
                continue;
            }
            if(checkedSet.Count(n => n.Owner == currentPlayer) == 1)
            {
                if(CalculateValueOfNode(node) + currentPlayer.ReadMoney >= requestedNode.price)
                {
                    int difference = CalculateValueOfNode(requestedNode) - CalculateValueOfNode(node);
                    if(difference > 0)
                    {
                        MakeTradeOffer(currentPlayer, nodeOwner, requestedNode, node,difference,0);

                    }
                    else
                    {
                        MakeTradeOffer(currentPlayer, nodeOwner, requestedNode, node,0,Mathf.Abs(difference));
                    }
                    
                    break;
                }
            }
        }

    }

    void MakeTradeOffer(Player currentPlayer, Player nodeOwner,MonopolyNode requestedNode, MonopolyNode offeredNode, int offeredMoney, int requestedMoney )
    {
        if(nodeOwner.playerType == Player.PlayerType.AI)
        {
            ConsiderTradeOffer(currentPlayer, nodeOwner, requestedNode, offeredNode, offeredMoney, requestedMoney);
        }
        else if (nodeOwner.playerType == Player.PlayerType.Human)
        {

        }
    }

    void ConsiderTradeOffer(Player currentPlayer, Player nodeOwner,MonopolyNode requestedNode, MonopolyNode offeredNode, int offeredMoney, int requestedMoney )
    {
        int valueOfTrade = (CalculateValueOfNode(requestedNode) + requestedMoney) - (CalculateValueOfNode(offeredNode) + offeredMoney);
        // sell a node for money only 
        if(requestedNode == null && offeredNode != null && requestedMoney < nodeOwner.ReadMoney/3)
        {
            Trade(currentPlayer, nodeOwner, requestedNode, offeredNode, offeredMoney, requestedMoney);
            return;
        }
        //normal trade
        if(valueOfTrade <= 0)
        {
            Trade(currentPlayer, nodeOwner, requestedNode, offeredNode, offeredMoney, requestedMoney);
        }
        else 
        {
            Debug.Log("AI rejected trade");
        }
    }

    int CalculateValueOfNode(MonopolyNode requestedNode)
    {
        int value = 0;
        if(requestedNode != null )
        {
            if(requestedNode.monopolyNodeType == MonopolyNodeType.Property)
            {
                value = requestedNode.price + requestedNode.NumberOfHouses * requestedNode.houseCost;

            }
            else
            {
                value = requestedNode.price;
            }
            return value;
        }
        return value;
    }
    
    void Trade(Player currentPlayer, Player nodeOwner,MonopolyNode requestedNode, MonopolyNode offeredNode, int offeredMoney, int requestedMoney)
    {
        if(requestedNode != null)
        {
            currentPlayer.PayMoney(offeredMoney);

            requestedNode.changeOwner(currentPlayer);

            nodeOwner.CollectMoney(offeredMoney);
            nodeOwner.PayMoney(requestedMoney);
            if(offeredNode != null)
            {
                offeredNode.changeOwner(nodeOwner);
                
            }
            string offeredNodeName = offeredNode != null ? " & " + offeredNode.name : "";
            OnUpdateMessage.Invoke(currentPlayer.name + " traded " + requestedNode.name +  " for " + offeredMoney + offeredNodeName + " to " + nodeOwner.name);
        }
        else if(offeredNode != null && requestedNode == null)
            {
                currentPlayer.CollectMoney(requestedMoney);
                nodeOwner.PayMoney(requestedMoney);
                offeredNode.changeOwner(nodeOwner);
                OnUpdateMessage.Invoke(currentPlayer.name + " sold " + offeredNode.name +  " to " + nodeOwner.name + " for" + requestedMoney);
            }
        
    }

    void CreateLeftPanel()
    {
        leftOffererNameText.text = leftPlayerReference.name;

        for(int i = 0; i< leftPlayerReference.GetMonopolyNodes.Count; i++)
        {
            GameObject tradeCard = Instantiate(cardPrefab, leftCardGrid, false);
            leftCardPrefabList.Add(tradeCard);
        }
        leftYourMoneyText.text = "Your Money: " + leftPlayerReference.ReadMoney;

        leftMoneySlider.maxValue = leftPlayerReference.ReadMoney;

        leftMoneySlider.value = 0;
        UpdateLeftSlider(leftMoneySlider.value);


        tradePanel.SetActive(true);
    }

    public void UpdateLeftSlider(float value)
    {
        leftOfferMoney.text = "Offer money: $ " + leftMoneySlider.value;

    }

    public void CloseTradePanel()
    {
        tradePanel.SetActive(false);

        ClearAll();
    }

    public void OpenTradePanel()
    {
        leftPlayerReference = GameManager.instance.GetCurrentPlayer;
        rightOffererNameText.text = "Select a Player";

        CreateLeftPanel();

        CreateMiddleButton();
    }


    public void ShowRightPlayer(Player player)
    {
        rightPlayerReference = player;
        ClearRightPanel();

        rightOffererNameText.text = rightPlayerReference.name;

        for(int i = 0; i< rightPlayerReference.GetMonopolyNodes.Count; i++)
        {
            GameObject tradeCard = Instantiate(cardPrefab, rightCardGrid, false);
            rightCardPrefabList.Add(tradeCard);
        }
        rightYourMoneyText.text = "Your Money: " + rightPlayerReference.ReadMoney;

        rightMoneySlider.maxValue = rightPlayerReference.ReadMoney;

        rightMoneySlider.value = 0;
        UpdateRightSlider(rightMoneySlider.value);


        tradePanel.SetActive(true);
    }

    void CreateMiddleButton()
    {
        for(int i = playerButtonList.Count - 1; i >=0 ; i-- )
        {
            Destroy(playerButtonList[i]);
        }

        playerButtonList.Clear();

        List<Player> allPlayers = new List<Player>();
        allPlayers.AddRange(GameManager.instance.GetPlayers);
        allPlayers.Remove(leftPlayerReference);

        foreach(var player in allPlayers)
        {
            GameObject newPlayerButton = Instantiate(playerButtonPrefab, buttonGrid, false);
            newPlayerButton.GetComponent<TradePlayerButton>().SetPlayer(player);

            playerButtonList.Add(newPlayerButton);
        }

    }

    void ClearAll()
    {
        rightOffererNameText.text = "Select a Player";
        rightYourMoneyText.text = "Your Money: $ 0";
        rightMoneySlider.maxValue = 0;
        rightMoneySlider.value = 0;
        UpdateRightSlider(rightMoneySlider.value);
        for(int i = playerButtonList.Count - 1; i >=0 ; i-- )
        {
            Destroy(playerButtonList[i]);
        }

        for(int i = leftCardPrefabList.Count - 1; i >=0 ; i-- )
        {
            Destroy(leftCardPrefabList[i]);
        }

        for(int i = rightCardPrefabList.Count - 1; i >=0 ; i-- )
        {
            Destroy(rightCardPrefabList[i]);
        }
    }

    void ClearRightPanel()
    {  
        for(int i = rightCardPrefabList.Count - 1; i >=0 ; i-- )
        {
            Destroy(rightCardPrefabList[i]);
        }

        rightCardPrefabList.Clear(); 

        rightMoneySlider.maxValue = 0;

        rightMoneySlider.value = 0;
        UpdateRightSlider(rightMoneySlider.value);   
    }

    public void UpdateRightSlider(float value)
    {
        rightOfferMoney.text = "Requested money: $ " + rightMoneySlider.value;

    }
}
