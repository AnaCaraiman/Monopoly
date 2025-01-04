using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class UIShowRailroad : MonoBehaviour
{
    MonopolyNode nodeReference;
    Player playerReference;

    [Header("Buy Railroad UI")]
    [SerializeField] GameObject railroadUiPanel;
    [SerializeField] TMP_Text railroadNameText;
    [SerializeField] Image colorField;
    [Space]
    [SerializeField] TMP_Text oneRailroadRentText;
    [SerializeField] TMP_Text twoRailroadRentText;
    [SerializeField] TMP_Text threeRailroadRentText;
    [SerializeField] TMP_Text fourRailroadRentText;
    [Space]
    [SerializeField] TMP_Text mortgagePriceText;
    [Space]
    [SerializeField] Button buyRailroadButton;
    [Space]
    [SerializeField] TMP_Text properrtyPriceText;
    [SerializeField] TMP_Text playerMoneyText;

    void OnEnable()
    {
        MonopolyNode.OnShowRailroadBuyPanel += ShowBuyRailroadPanelUi;
    }
    void OnDisable()
    {
        MonopolyNode.OnShowRailroadBuyPanel -= ShowBuyRailroadPanelUi;
    }

    void Start()
    {
        railroadUiPanel.SetActive(false);
    }

    void ShowBuyRailroadPanelUi(MonopolyNode node, Player currentPlayer)
    {
        nodeReference = node;
        playerReference = currentPlayer;
        //TOP PANEL CONTENT
        railroadNameText.text = node.name;
        //colorField.color = node.propertyColorField.color;
        //CENTER OF THE CARD
        //result = baseRent * (int)Mathf.Pow(2, amount - 1);
        oneRailroadRentText.text = node.baseRent * (int)Mathf.Pow(2, 1 - 1) + "RON";
        twoRailroadRentText.text = node.baseRent * (int)Mathf.Pow(2, 2 - 1) + "RON";
        threeRailroadRentText.text = node.baseRent * (int)Mathf.Pow(2, 3 - 1) + "RON";
        fourRailroadRentText.text = node.baseRent * (int)Mathf.Pow(2, 4 - 1) + "RON";
        //cost of buildings
        mortgagePriceText.text = node.MortgageValue + "RON";
        //BOTTOM BAR
        properrtyPriceText.text = "Price: " + node.price + "RON";
        playerMoneyText.text = "You have: " + currentPlayer.ReadMoney + "RON";
        //BUY PROPERTY BUTTON
        if (currentPlayer.CanAffordNode(node.price))
        {
            buyRailroadButton.interactable = true;
        }
        else
        {
            buyRailroadButton.interactable = false;
        }

        //SHOW THE PANEL
        railroadUiPanel.SetActive(true);
    }

    public void BuyRailroadButton() //this is called from the buy button
    {
        //tell the player to buy the property
        playerReference.BuyProperty(nodeReference);
        //maybe close the property card


        //make the button not interactable anumore
        buyRailroadButton.interactable = false;
    }

    public void CloseRailroadButton() //this is called from the buy button
    {
        //close the panel
        railroadUiPanel.SetActive(false);
        //clear node reference
        nodeReference = null;
        playerReference = null;
    }
}
