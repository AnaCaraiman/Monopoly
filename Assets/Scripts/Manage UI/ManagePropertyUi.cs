using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ManagePropertyUi : MonoBehaviour
{
    [SerializeField] Transform cardHolder; //HORIZONTAL LAYOUT
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Button buyHouseButton, sellHouseButton;
    [SerializeField] TMP_Text buyHousePriceText, sellHousePriceText;
    Player playerReference;
    List<MonopolyNode> nodesInSet = new List<MonopolyNode>();
    List<GameObject> cardsInSet = new List<GameObject>();
    [SerializeField] GameObject buttonBox;

    //FOR 1 SPECIFIC CARD SET
    public void SetProperty(List<MonopolyNode> nodes, Player owner)
    {
        playerReference = owner;
        if (nodes.Count == 0)
        {
            return;
        }
        nodesInSet.AddRange(nodes);
        for (int i = 0; i < nodesInSet.Count; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardHolder, false);
            ManageCardUi manageCardUi = newCard.GetComponent<ManageCardUi>();
            cardsInSet.Add(newCard);
            manageCardUi.SetCard(nodesInSet[i], owner, this);
        }

        var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(nodesInSet[0]);
        buyHouseButton.interactable = allSame && CheckIfBuyAllowed();
        sellHouseButton.interactable = CheckIfSellAllowed();

        buyHousePriceText.text = "-" + nodesInSet[0].houseCost;
        sellHousePriceText.text = "+" + nodesInSet[0].houseCost / 2;
        if (nodes[0].monopolyNodeType != MonopolyNodeType.Property)
        {
            buttonBox.SetActive(false);
        }
    }

    public void BuyHouseButton()
    {
        if (nodesInSet.Count == 0)
        {
            string message = "No properties to build a house";
            ManageUi.instance.UpdateSystemMessage(message);
            return;
        }
        if (!CheckIfBuyAllowed())
        {
            //ERROR MESSAGE
            string message = "One or more properties are mortgaged, you can't build a house";
            ManageUi.instance.UpdateSystemMessage(message);
            return;
        }
        if (playerReference.CanAffordHouse(nodesInSet[0].houseCost))
        {
            playerReference.BuildHousesOrHotelEvenly(nodesInSet);
            //UPDATE MONEY TEXT
            UpdateHouseVisuals();
            string message = "You build a house";
            ManageUi.instance.UpdateSystemMessage(message);
        }
        else
        {
            //CANT AFFOD HOUSE
            string message = "You don't have enough money";
            ManageUi.instance.UpdateSystemMessage(message);
        }
        sellHouseButton.interactable = CheckIfSellAllowed();
        ManageUi.instance.UpdateMoneyText();
    }

    public void SellHouseButton()
    {
        if (nodesInSet.Count == 0)
        {
            string message = "No properties to sell a house";
            ManageUi.instance.UpdateSystemMessage(message);
            return;
        }
        if (!CheckIfSellAllowed())
        {
            //ERROR MESSAGE
            string message = "No houses to sell";
            ManageUi.instance.UpdateSystemMessage(message);
            return;
        }
        playerReference.SellHouseEvenly(nodesInSet);
        //UPDATE MONEY TEXT
        UpdateHouseVisuals();

        sellHouseButton.interactable = CheckIfSellAllowed();
        ManageUi.instance.UpdateMoneyText();
    }

    bool CheckIfSellAllowed()
    {
        if (nodesInSet.Any(n => n.NumberOfHouses > 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool CheckIfBuyAllowed()
    {
        if (nodesInSet.Any(n => n.IsMortgaged == true))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool CheckIfMortgageAllowed()
    {
        if (nodesInSet.Any(n => n.NumberOfHouses > 0))
        {
            return false;
        }
        return true;
    }

    void UpdateHouseVisuals()
    {
        foreach (var card in cardsInSet)
        {
            card.GetComponent<ManageCardUi>().ShowBuildings();
        }
    }
}
