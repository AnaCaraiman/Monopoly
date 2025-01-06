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

    //FOR 1 SPECIFIC CARD SET
    public void SetProperty(List<MonopolyNode> nodes, Player owner)
    {
        playerReference = owner;
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
        sellHousePriceText.text = "+" + nodesInSet[0].houseCost;
    }

    public void BuyHouseButton()
    {
        if (!CheckIfBuyAllowed())
        {
            return;
        }
        if (playerReference.CanAffordHouse(nodesInSet[0].houseCost))
        {
            playerReference.BuildHousesOrHotelEvenly(nodesInSet);
            //UPDATE MONEY TEXT
            UpdateHouseVisuals();
        }
        else
        {
            //CANT AFFOD HOUSE
        }
        sellHouseButton.interactable = CheckIfSellAllowed();
    }

    public void SellHouseButton()
    {
        playerReference.SellHouseEvenly(nodesInSet);
        //UPDATE MONEY TEXT
        UpdateHouseVisuals();

        sellHouseButton.interactable = CheckIfSellAllowed();
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
        return false;
    }

    void UpdateHouseVisuals()
    {
        foreach (var card in cardsInSet)
        {
            card.GetComponent<ManageCardUi>().ShowBuildings();
        }
    }
}
