using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class UiShowProperty : MonoBehaviour
{
    MonopolyNode nodeReference;

    [Header("Buy Property UI")]
    [SerializeField] GameObject propertyUiPanel;
    [SerializeField] TMP_Text propertyNameText;
    [SerializeField] Image colorField;
    [Space]
    [SerializeField] TMP_Text rentPriceText; //without a house
    [SerializeField] TMP_Text oneHouseRentText;
    [SerializeField] TMP_Text twoHouseRentText;
    [SerializeField] TMP_Text threeHouseRentText;
    [SerializeField] TMP_Text fourHouseRentText;
    [SerializeField] TMP_Text hotelRentText;
    [Space]
    [SerializeField] TMP_Text housePriceText;
    [SerializeField] TMP_Text hotelPriceText;
    [Space]
    [SerializeField] Button buyPropertyButton;
    [Space]
    [SerializeField] TMP_Text properrtyPriceText;
    [SerializeField] TMP_Text playerMoneyText;

    void ShowBuyPropertyUi(MonopolyNode node, Player currentPlayer)
    {
        nodeReference = node;
        //TOP PANEL CONTENT
        propertyNameText.text = node.name;
        colorField.color = node.propertyColorField.color;
        //CENTER OF THE CARD
        rentPriceText.text = node.baseRent + "RON";
        oneHouseRentText.text = node.rentWithHouses[0] + "RON";
        twoHouseRentText.text = node.rentWithHouses[1] + "RON";
        threeHouseRentText.text = node.rentWithHouses[2] + "RON";
        fourHouseRentText.text = node.rentWithHouses[3] + "RON";
        hotelRentText.text = node.rentWithHouses[4] + "RON";
        //cost of buildings
        housePriceText.text = node.houseCost + "RON";
        hotelPriceText.text = node.houseCost + "RON";
        //BOTTOM BAR
        properrtyPriceText.text = "Price: " + node.price + "RON";
        playerMoneyText.text = "You have: " + currentPlayer.ReadMoney + "RON";
        //BUY PROPERTY BUTTON
        if(currentPlayer.CanAffordNode(node.price))
        {
            buyPropertyButton.interactable = true;
        }
        else
        {
            buyPropertyButton.interactable = false;
        }

        //SHOW THE PANEL
        propertyUiPanel.SetActive(true);
    }
}
