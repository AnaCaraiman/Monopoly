using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class TradingSystem : MonoBehaviour
{
    public static TradingSystem instance;

    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject tradePanel;
    [SerializeField] GameObject resultPanel;
    [SerializeField] TMP_Text resultMessageText;
    [Header("LEFT SIDE")] [SerializeField] TMP_Text leftOffererNameText;
    [SerializeField] Transform leftCardGrid;
    [SerializeField] ToggleGroup leftToggleGroup;
    [SerializeField] TMP_Text leftYourMoneyText;
    [SerializeField] TMP_Text leftOfferMoney;
    [SerializeField] Slider leftMoneySlider;
    List<GameObject> leftCardPrefabList = new List<GameObject>();

    Player leftPlayerReference;

    [Header("MIDDLE")] [SerializeField] Transform buttonGrid;
    [SerializeField] GameObject playerButtonPrefab;

    List<GameObject> playerButtonList = new List<GameObject>();

    [Header("RIGHT SIDE")] [SerializeField]
    TMP_Text rightOffererNameText;

    [SerializeField] Transform rightCardGrid;
    [SerializeField] ToggleGroup rightToggleGroup;
    [SerializeField] TMP_Text rightYourMoneyText;
    [SerializeField] TMP_Text rightOfferMoney;
    [SerializeField] Slider rightMoneySlider;
    List<GameObject> rightCardPrefabList = new List<GameObject>();
    
    Player rightPlayerReference;

    [Header("Trade Offer Panel")]
    [SerializeField] GameObject tradeOfferPanel;
    [SerializeField] TMP_Text leftMessageText, rightMessageText, leftMoneyText, rightMoneyText;
    [SerializeField] GameObject leftCard, rightCard;
    [SerializeField] Image leftColorField, rightColorField;
    [SerializeField] Image leftPropImage, rightPropImage;
    [SerializeField] Sprite houseSprite, railroadSprite, utilitySprite;

    //STORE THE OFFER FOR HUMAN
     Player currentPlayer, nodeOwner;
    MonopolyNode requestedNode, offeredNode;
    int offeredMoney, requestedMoney;

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
        resultPanel.SetActive(false);
        tradeOfferPanel.SetActive(false);
    }

    public void FindMissingProperty(Player currentPlayer)
    {
        List<MonopolyNode> processedSet = null;
        MonopolyNode requestedNode = null;
        foreach (var node in currentPlayer.GetMonopolyNodes)
        {
            var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(node);
            List<MonopolyNode> nodeSet = new List<MonopolyNode>();
            nodeSet.AddRange(list);
            //if all have been purchased
            bool notAllPurchased = list.Any(n => n.Owner == null);

            if (allSame || processedSet == list || notAllPurchased)
            {
                processedSet = list;
                continue;
            }

            //AI owns this set already
            if (allSame || processedSet == list)
            {
                processedSet = list;
                continue;
            }

            if (list.Count == 2)
            {
                requestedNode = list.Find(n => n.Owner != currentPlayer && n.Owner != null);
                if (requestedNode != null)
                {
                    MakeTradeDecision(currentPlayer, requestedNode.Owner, requestedNode);
                    return;
                }
            }

            if (list.Count >= 3)
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
        //CONTINUE IF NOTHING HAS BEEN FOUND
        if(requestedNode==null)
        {
            currentPlayer.ChangeState(Player.AiStates.IDLE);
        }
    }

    void MakeTradeDecision(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode)
    {
        if (currentPlayer.ReadMoney >= CalculateValueOfNode(requestedNode))
        {
            MakeTradeOffer(currentPlayer, nodeOwner, requestedNode, null, CalculateValueOfNode(requestedNode), 0);
            return;
        }

        bool foundDecision = false;

        foreach (var node in currentPlayer.GetMonopolyNodes)
        {
            var checkedSet = MonopolyBoard.instance.PlayerHasAllNodesOfSet(node).list;
            if (checkedSet.Contains(requestedNode))
            {
                continue;
            }

            if (checkedSet.Count(n => n.Owner == currentPlayer) == 1)
            {
                if (CalculateValueOfNode(node) + currentPlayer.ReadMoney >= requestedNode.price)
                {
                    int difference = CalculateValueOfNode(requestedNode) - CalculateValueOfNode(node);
                    if (difference > 0)
                    {
                        MakeTradeOffer(currentPlayer, nodeOwner, requestedNode, node, difference, 0);
                    }
                    else
                    {
                        MakeTradeOffer(currentPlayer, nodeOwner, requestedNode, node, 0, Mathf.Abs(difference));
                    }
                    foundDecision = true;
                    break;
                }
            }
        }
        if(!foundDecision)
        {
            currentPlayer.ChangeState(Player.AiStates.IDLE);
        }
    }

    void MakeTradeOffer(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode, MonopolyNode offeredNode,
        int offeredMoney, int requestedMoney)
    {
        if (nodeOwner.playerType == Player.PlayerType.AI)
        {
            ConsiderTradeOffer(currentPlayer, nodeOwner, requestedNode, offeredNode, offeredMoney, requestedMoney);
        }
        else if (nodeOwner.playerType == Player.PlayerType.Human)
        {
            ShowTradeOfferPanel(currentPlayer, nodeOwner, requestedNode, offeredNode, offeredMoney, requestedMoney);
        }
    }

    void ConsiderTradeOffer(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode,
        MonopolyNode offeredNode, int offeredMoney, int requestedMoney)
    {
        int valueOfTheTrade = (CalculateValueOfNode(requestedNode) + requestedMoney) -
                           (CalculateValueOfNode(offeredNode) + offeredMoney);
        // sell a node for money only 
        if (requestedNode == null && offeredNode != null && requestedMoney <= nodeOwner.ReadMoney / 3 && !MonopolyBoard.instance.PlayerHasAllNodesOfSet(requestedNode).allSame)
        {
            Trade(currentPlayer, nodeOwner, requestedNode, offeredNode, offeredMoney, requestedMoney);
            if(currentPlayer.playerType == Player.PlayerType.Human)
            {
                TradeResult(true);
            }
            
            return;
        }

        //normal trade
        if (valueOfTheTrade <= 0 && !MonopolyBoard.instance.PlayerHasAllNodesOfSet(requestedNode).allSame)
        {
            Trade(currentPlayer, nodeOwner, requestedNode, offeredNode, offeredMoney, requestedMoney);
            if (currentPlayer.playerType == Player.PlayerType.Human)
            {
                TradeResult(true);
            }
        }
        else
        {
            if (currentPlayer.playerType == Player.PlayerType.Human)
            {
                TradeResult(false);
            }
            Debug.Log("AI rejected trade");
        }
    }

    int CalculateValueOfNode(MonopolyNode requestedNode)
    {
        int value = 0;
        if (requestedNode != null)
        {
            if (requestedNode.monopolyNodeType == MonopolyNodeType.Property)
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

    void Trade(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode, MonopolyNode offeredNode,
        int offeredMoney, int requestedMoney)
    {
        if (requestedNode != null)
        {
            currentPlayer.PayMoney(offeredMoney);

            requestedNode.changeOwner(currentPlayer);

            nodeOwner.CollectMoney(offeredMoney);
            nodeOwner.PayMoney(requestedMoney);
            if (offeredNode != null)
            {
                offeredNode.changeOwner(nodeOwner);
            }

            string offeredNodeName = offeredNode != null ? " & " + offeredNode.name : "";
            OnUpdateMessage?.Invoke(currentPlayer.name + " traded " + requestedNode.name + " for " + offeredMoney +
                                   offeredNodeName + " to " + nodeOwner.name);
        }
        else if (offeredNode != null && requestedNode == null)
        {
            currentPlayer.CollectMoney(requestedMoney);
            nodeOwner.PayMoney(requestedMoney);
            offeredNode.changeOwner(nodeOwner);
            OnUpdateMessage?.Invoke(currentPlayer.name + " sold " + offeredNode.name + " to " + nodeOwner.name + " for" +
                                   requestedMoney);
        }
        
        //HIDE UI FOR HUMAN AI
        CloseTradePanel();
        if(currentPlayer.playerType==Player.PlayerType.AI)
        {
            currentPlayer.ChangeState(Player.AiStates.IDLE);
        }
    }

    void CreateLeftPanel()
    {
        leftOffererNameText.text = leftPlayerReference.name;

        List<MonopolyNode> referenceNodes = leftPlayerReference.GetMonopolyNodes;

        for (int i = 0; i < referenceNodes.Count; i++)
        {
            GameObject tradeCard = Instantiate(cardPrefab, leftCardGrid, false);
            tradeCard.GetComponent<TradePropertyCard>().SetTradeCard(referenceNodes[i], leftToggleGroup);

            leftCardPrefabList.Add(tradeCard);
        }

        leftYourMoneyText.text = "Your Money: " + leftPlayerReference.ReadMoney + "RON";

        leftMoneySlider.maxValue = leftPlayerReference.ReadMoney;

        leftMoneySlider.value = 0;
        UpdateLeftSlider(leftMoneySlider.value);


        tradePanel.SetActive(true);
    }

    public void UpdateLeftSlider(float value)
    {
        leftOfferMoney.text = "Offer money: " + leftMoneySlider.value +"RON";
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

        List<MonopolyNode> referenceNodes = rightPlayerReference.GetMonopolyNodes;
        for (int i = 0; i < referenceNodes.Count; i++)
        {
            GameObject tradeCard = Instantiate(cardPrefab, rightCardGrid, false);

            tradeCard.GetComponent<TradePropertyCard>().SetTradeCard(referenceNodes[i], rightToggleGroup);
            rightCardPrefabList.Add(tradeCard);
        }

        rightYourMoneyText.text = "Your Money: " + rightPlayerReference.ReadMoney + "RON";

        rightMoneySlider.maxValue = rightPlayerReference.ReadMoney;

        rightMoneySlider.value = 0;
        UpdateRightSlider(rightMoneySlider.value);


        tradePanel.SetActive(true);
    }

    public void UpdateRightSlider(float value)
    {
        rightOfferMoney.text = "Requested money: " + rightMoneySlider.value + "RON";
    }

    void CreateMiddleButton()
    {
        for (int i = playerButtonList.Count - 1; i >= 0; i--)
        {
            Destroy(playerButtonList[i]);
        }

        playerButtonList.Clear();

        List<Player> allPlayers = new List<Player>();
        allPlayers.AddRange(GameManager.instance.GetPlayers);
        allPlayers.Remove(leftPlayerReference);

        foreach (var player in allPlayers)
        {
            GameObject newPlayerButton = Instantiate(playerButtonPrefab, buttonGrid, false);
            newPlayerButton.GetComponent<TradePlayerButton>().SetPlayer(player);

            playerButtonList.Add(newPlayerButton);
        }
    }

    void ClearAll()
    {
        rightOffererNameText.text = "Select a Player";
        rightYourMoneyText.text = "Your Money: 0 RON";
        rightMoneySlider.maxValue = 0;
        rightMoneySlider.value = 0;
        UpdateRightSlider(rightMoneySlider.value);
        for (int i = playerButtonList.Count - 1; i >= 0; i--)
        {
            Destroy(playerButtonList[i]);
        }

        for (int i = leftCardPrefabList.Count - 1; i >= 0; i--)
        {
            Destroy(leftCardPrefabList[i]);
        }

        for (int i = rightCardPrefabList.Count - 1; i >= 0; i--)
        {
            Destroy(rightCardPrefabList[i]);
        }
    }

    void ClearRightPanel()
    {
        for (int i = rightCardPrefabList.Count - 1; i >= 0; i--)
        {
            Destroy(rightCardPrefabList[i]);
        }

        rightCardPrefabList.Clear();

        rightMoneySlider.maxValue = 0;

        rightMoneySlider.value = 0;
        UpdateRightSlider(rightMoneySlider.value);
    }

    //------------------------------ MAKE OFFER BUTTONS ------------------------------ HUMAN
    public void MakeOfferButton()
    {
        MonopolyNode requestedNode = null;
        MonopolyNode offeredNode = null;

        if (rightPlayerReference == null)
        {
            //ERROR MESSAGE HERE
            return;
        }

        //left
        Toggle offeredToggle = leftToggleGroup.ActiveToggles().FirstOrDefault();
        if (offeredToggle != null)
        {
            offeredNode = offeredToggle.GetComponentInParent<TradePropertyCard>().Node();
        }

        //right
        Toggle requestedToggle = rightToggleGroup.ActiveToggles().FirstOrDefault();
        if (requestedToggle != null)
        {
            requestedNode = requestedToggle.GetComponentInParent<TradePropertyCard>().Node();
        }

        MakeTradeOffer(leftPlayerReference, rightPlayerReference, requestedNode, offeredNode,
            (int)leftMoneySlider.value, (int)rightMoneySlider.value);
    }


    //------------------------------ TRADE RESULT ------------------------------ HUMAN 

    void TradeResult(bool accepted)
    {
        if(accepted)
        {
            resultMessageText.text = rightPlayerReference.name + "<b><color=green> accepted </color></b>" + "the trade.";
        }
        else
        {
            resultMessageText.text = rightPlayerReference.name + "<b><color=red> rejected </color></b>" + "the trade.";
        }

        resultPanel.SetActive(true);
    }

    //------------------------------ TRADE OFFER PANEL ------------------------------ HUMAN
    
    void ShowTradeOfferPanel(Player _currentPlayer, Player _nodeOwner, MonopolyNode _requestedNode, MonopolyNode _offeredNode,
        int _offeredMoney, int _requestedMoney)
    {
        //FILL THE ACTUAL OFFER CONTENT
        currentPlayer = _currentPlayer;
        nodeOwner = _nodeOwner;
        requestedNode = _requestedNode;
        offeredNode = _offeredNode;
        offeredMoney = _offeredMoney;
        requestedMoney = _requestedMoney;
       //SHOW PANEL CONTENT
        tradeOfferPanel.SetActive(true);
        leftMessageText.text = currentPlayer.name + " offers:";
        rightMessageText.text = "For " + nodeOwner.name + " 's:";
        leftMoneyText.text =  offeredMoney + "+RON";
        rightMoneyText.text = requestedMoney + "+RON";
        leftCard.SetActive(offeredNode != null ? true : false);
        rightCard.SetActive(requestedMoney != null ? true : false);

        if(leftCard.activeInHierarchy)
        {
            leftColorField.color = (offeredNode.propertyColorField != null)?offeredNode.propertyColorField.color:Color.black;
            switch (offeredNode.monopolyNodeType)
            {
                case MonopolyNodeType.Property:
                    leftPropImage.sprite = houseSprite;
                    leftPropImage.color = Color.blue;
                    break;
                case MonopolyNodeType.Railroad:
                    leftPropImage.sprite = railroadSprite;
                    leftPropImage.color = Color.white;
                    break;
                case MonopolyNodeType.Utility:
                    leftPropImage.sprite = utilitySprite;
                    leftPropImage.color = Color.black;
                    break;
            }
        }

        if (rightCard.activeInHierarchy)
        {
            rightColorField.color = (requestedNode.propertyColorField != null) ? requestedNode.propertyColorField.color : Color.black;
            switch (requestedNode.monopolyNodeType)
            {
                case MonopolyNodeType.Property:
                    rightPropImage.sprite = houseSprite;
                    rightPropImage.color = Color.blue;
                    break;
                case MonopolyNodeType.Railroad:
                    rightPropImage.sprite = railroadSprite;
                    rightPropImage.color = Color.white;
                    break;
                case MonopolyNodeType.Utility:
                    rightPropImage.sprite = utilitySprite;
                    rightPropImage.color = Color.black;
                    break;
            }
        }
    }

    public void AcceptOffer()
    {   
        Trade(currentPlayer, nodeOwner, requestedNode, offeredNode, offeredMoney, requestedMoney);
       
        ResetOffer();
    }

    public void RejectOffer()
    {
        currentPlayer.ChangeState(Player.AiStates.IDLE);
        ResetOffer();
    }
    void ResetOffer()
    {
        currentPlayer = null;
        nodeOwner = null;
        requestedNode = null;
        offeredNode = null;
        offeredMoney = 0;
        requestedMoney = 0;
    }
}