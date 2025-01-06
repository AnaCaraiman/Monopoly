using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class ManageCardUi : MonoBehaviour
{
    [SerializeField] Image colorField;
    [SerializeField] GameObject[] buildings;
    [SerializeField] GameObject mortgageImage;
    [SerializeField] TMP_Text mortgageValueText;
    [SerializeField] Button mortgageButton, unMortgageButton;

    Player playerReference;
    MonopolyNode nodeReference;
    ManagePropertyUi propertyReference;

    public void SetCard(MonopolyNode node, Player owner, ManagePropertyUi propertSet)
    {
        nodeReference = node;
        playerReference = owner;
        propertyReference = propertSet;

        //SET COLOR
        if (node.propertyColorField != null)
        {
            colorField.color = node.propertyColorField.color;
        }
        else
        {
            colorField.color = Color.black;
        }

        ShowBuildings();

        //SHOW MORTGAGE IMAGE
        mortgageImage.SetActive(node.IsMortgaged);
        //TEXT UPDATE
        mortgageValueText.text = "Mortgage Value <br><b>$ " + node.MortgageValue + " RON";

        //BUTTONS
        mortgageButton.interactable = !node.IsMortgaged;
        unMortgageButton.interactable = node.IsMortgaged;
    }

    public void MortgageButton()
    {
        if(!propertyReference.CheckIfMortgageAllowed())
        {
            //ERROR MESSAGE
            return;
        }
        {
            //ERROR MESSAGE
            return;
        }
        if (nodeReference.IsMortgaged)
        {
            //ERROR MESSAGE
            return;
        }
        playerReference.CollectMoney(nodeReference.MortgageProperty());
        mortgageImage.SetActive(true);
        mortgageButton.interactable = false;
        unMortgageButton.interactable = true;
    }

    public void UnMortgageButton()
    {
        if (!nodeReference.IsMortgaged)
        {
            //ERROR MESSAGE
            return;
        }
        if(playerReference.ReadMoney < nodeReference.MortgageValue)
        {
            //ERROR MESSAGE
            return;
        }
        playerReference.PayMoney(nodeReference.MortgageValue);
        nodeReference.UnMortgageProperty();
        mortgageImage.SetActive(false);
        mortgageButton.interactable = true;
        unMortgageButton.interactable = false;
    }

    public void ShowBuildings()
    {
        //HIDE ALL BUILDINGS
        foreach (var icon in buildings)
        {
            icon.SetActive(false);
        }
        //SHOW BUILDINGS
        if (nodeReference.NumberOfHouses < 5)
        {
            for (int i = 0; i < nodeReference.NumberOfHouses; i++)
            {
                buildings[i].SetActive(true);
            }
        }
        else
        {
            buildings[4].SetActive(true);
        }
    }
}
