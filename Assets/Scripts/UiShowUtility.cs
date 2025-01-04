using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;

public class UiShowUtility : MonoBehaviour
{
    MonopolyNode nodeReference;
    Player playerReference;

    [Header("Buy Utility UI")]
    [SerializeField] GameObject utilityUiPanel;
    [SerializeField] TMP_Text utilityNameText;
    [SerializeField] Image colorField;
    [Space]
    [SerializeField] TMP_Text mortgagePriceText;
    [Space]
    [SerializeField] Button buyUtilityButton;
    [Space]
    [SerializeField] TMP_Text utilityPriceText;
    [SerializeField] TMP_Text playerMoneyText;

    void OnEnable()
    {
        MonopolyNode.OnShowUtilityBuyPanel += ShowBuyUtilityPanel;
    }
    void OnDisable()
    {
        MonopolyNode.OnShowUtilityBuyPanel -= ShowBuyUtilityPanel;
    }

    void Start()
    {
        utilityUiPanel.SetActive(false);
    }

    void ShowBuyUtilityPanel(MonopolyNode node, Player currentPlayer)
    {
        nodeReference = node;
        playerReference = currentPlayer;
        //TOP PANEL CONTENT
        utilityNameText.text = node.name;
        //colorField.color = node.propertyColorField.color;

        //cost of buildings
        mortgagePriceText.text = node.MortgageValue + "RON";
        //BOTTOM BAR
        utilityPriceText.text = "Price: " + node.price + "RON";
        playerMoneyText.text = "You have: " + currentPlayer.ReadMoney + "RON";
        //BUY PROPERTY BUTTON
        if (currentPlayer.CanAffordNode(node.price))
        {
            buyUtilityButton.interactable = true;
        }
        else
        {
            buyUtilityButton.interactable = false;
        }

        //SHOW THE PANEL
        utilityUiPanel.SetActive(true);
    }

    public void BuyUtilityButton() //this is called from the buy button
    {
        //tell the player to buy the property
        playerReference.BuyProperty(nodeReference);
        //maybe close the property card


        //make the button not interactable anumore
        buyUtilityButton.interactable = false;
    }

    public void CloseUtilityButton() //this is called from the buy button
    {
        //close the panel
        utilityUiPanel.SetActive(false);
        //clear node reference
        nodeReference = null;
        playerReference = null;
    }
}
