using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public enum  MonopolyNodeType 
{
    Property,
    Utility,
    Railroad,
    Tax,
    Chance,
    CommunityChest,
    Go,
    Jail,
    FreeParking,
    GoToJail
}

public class MonopolyNode : MonoBehaviour
{
    public MonopolyNodeType monopolyNodeType;
    [Header("Property Name")]
    [SerializeField] internal new string name;
    [SerializeField] TMP_Text nameText;
    [Header("Property Price")]
    public int price;
    [SerializeField] TMP_Text priceText;
    [Header("Property Rent")]
    [SerializeField] bool calculateRentAuto;
    [SerializeField] int currentRent;
    [SerializeField] internal int baseRent;
    [SerializeField] internal int[] rentWithHouses;
    [Header("Property Mortgage")]
    [SerializeField] GameObject mortgageImage;
    [SerializeField] GameObject propertyImage;
    [SerializeField] bool isMortgaged;
    [SerializeField] int mortgageValue;
    [Header("Property Owner")]
    public Player owner;
    [SerializeField] GameObject ownerBar;
    [SerializeField] TMP_Text ownerText;
    void OnValidate()
    {
        if (nameText != null)
        {
            nameText.text = name;
        }

        //CALCULATION
        if (calculateRentAuto)
        {
            if(monopolyNodeType == MonopolyNodeType.Property)
            {
                if (baseRent > 0)
                {
                    price = 3 * (baseRent * 10);
                    //MORTGAGE PRICE
                    mortgageValue = price / 2;
                    rentWithHouses = new int[]
                    {
                        baseRent*5,
                        baseRent*5*3,
                        baseRent*5*9,
                        baseRent*5*16,
                        baseRent*5*25,
                    };
                }
            }
            if(monopolyNodeType == MonopolyNodeType.Utility)
            {
               mortgageValue = price / 2;
            }
            if (monopolyNodeType == MonopolyNodeType.Railroad)
            {
                mortgageValue = price / 2;
            }
        }

        if (priceText != null)
        {
            priceText.text = "$ " + price;
        }
        //UPDATE OWNER
        OnOwnerUpdated();
        UnMortgageProperty();
        // isMortgaged = false;
    }

    //MORTGAGE CONTENT
    public int MortgageProperty()
    {
        isMortgaged = true;
        if(mortgageImage!=null){
        mortgageImage.SetActive(true);
        }
        if(propertyImage!=null)
        {
            propertyImage.SetActive(false);
        }
        return mortgageValue;
    }

    public void UnMortgageProperty()
    {
        isMortgaged = false;
        if(mortgageImage!=null){
        mortgageImage.SetActive(false);
        }
        if(propertyImage!=null)
        {
            propertyImage.SetActive(true);
        }
    }

    public bool IsMortgaged => isMortgaged;
    public int MortgageValue => mortgageValue;
    //UPDATE OWNER
    public void OnOwnerUpdated()
    {
        if(ownerBar!=null)
        {
            if(owner.name != "")
            {
                ownerBar.SetActive(true);
                ownerText.text = owner.name;
            }
            else
            {
                ownerBar.SetActive(false);
                ownerText.text = "";
            }

        }
    }
}
