using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum MonopolyNodeType
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
    public Image propertyColorField;

    [Header("Property Name")] [SerializeField]
    internal new string name;

    [SerializeField] TMP_Text nameText;
    [Header("Property Price")] public int price;
    public int houseCost;
    [SerializeField] TMP_Text priceText;

    [Header("Property Rent")] [SerializeField]
    bool calculateRentAuto;

    [SerializeField] int currentRent;
    [SerializeField] internal int baseRent;
    [SerializeField] internal List<int> rentWithHouses = new List<int>();
    int numberOfHouses;
    public int NumberOfHouses => numberOfHouses;
    [SerializeField] private GameObject[] houses;
    [SerializeField] private GameObject hotel;

    [Header("Property Mortgage")] [SerializeField]
    GameObject mortgageImage;

    [SerializeField] GameObject propertyImage;
    [SerializeField] bool isMortgaged;
    [SerializeField] int mortgageValue;

    [Header("Property Owner")] [SerializeField]
    GameObject ownerBar;

    [SerializeField] TMP_Text ownerText;
    Player owner;

    //MESSAGE SYSTEM
    public delegate void UpdateMessage(string message);

    public static UpdateMessage OnUpdateMessage;

    //DRAG A COMMUNITY CARD
    public delegate void DrawCommunityCard(Player player);

    public static DrawCommunityCard OnDrawCommunityCard;

    //DRAG A CHANCE CARD
    public delegate void DrawChanceCard(Player player);

    public static DrawCommunityCard OnDrawChanceCard;

    public Player Owner => owner;

    public void SetOwner(Player newOwner)
    {
        owner = newOwner;
    }

    void OnValidate()
    {
        if (nameText != null)
        {
            nameText.text = name;
        }

        //CALCULATION
        if (calculateRentAuto)
        {
            if (monopolyNodeType == MonopolyNodeType.Property)
            {
                if (baseRent > 0)
                {
                    price = 3 * (baseRent * 10);
                    //MORTGAGE PRICE
                    mortgageValue = price / 2;
                    rentWithHouses.Clear();

                    rentWithHouses.Add(baseRent * 5);
                    rentWithHouses.Add(baseRent * 5 * 3);
                    rentWithHouses.Add(baseRent * 5 * 9);
                    rentWithHouses.Add(baseRent * 5 * 16);
                    rentWithHouses.Add(baseRent * 5 * 25);
                }
                else if (baseRent <= 0)
                {
                    price = 0;
                    baseRent = 0;
                    rentWithHouses.Clear();
                    mortgageValue = 0;
                }
            }

            if (monopolyNodeType == MonopolyNodeType.Utility)
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

    public void UpdateColorField(Color color)
    {
        if (propertyColorField != null)
        {
            propertyColorField.color = color;
        }
    }

    //MORTGAGE CONTENT
    public int MortgageProperty()
    {
        isMortgaged = true;
        if (mortgageImage != null)
        {
            mortgageImage.SetActive(true);
        }

        if (propertyImage != null)
        {
            propertyImage.SetActive(false);
        }

        return mortgageValue;
    }

    public void UnMortgageProperty()
    {
        isMortgaged = false;
        if (mortgageImage != null)
        {
            mortgageImage.SetActive(false);
        }

        if (propertyImage != null)
        {
            propertyImage.SetActive(true);
        }
    }

    public bool IsMortgaged => isMortgaged;

    public int MortgageValue => mortgageValue;

    //UPDATE OWNER
    public void OnOwnerUpdated()
    {
        if (ownerBar != null)
        {
            if (owner != null && owner.name != "")
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

    public void PlayerLandedOnNode(Player currentPlayer)
    {
        bool playerIsHuman = currentPlayer.playerType == Player.PlayerType.Human;
        bool continueTurn = true;
        //check for node type and act

        switch (monopolyNodeType)
        {
            case MonopolyNodeType.Property:
                if (!playerIsHuman) //AI
                {
                    //IF IT IS OWNED AND WE ARE NOT OWNER AND IS NOT MORTGAGED
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //PAY RENT TO SOMEBODY

                        //CALCUATE REN
                        int rentToPay = CalculatePropertyRent();
                        //PAY RENT TO OWNER
                        currentPlayer.PayRent(rentToPay, owner);
                        //TODO take the corect color of the players
                        OnUpdateMessage.Invoke(
                            $"<b>{currentPlayer.name}</b> pays rent of <b><color=green>${rentToPay}</color></b> to <b>{owner.name}</b>! 💸");
                    }
                    else if (owner == null && currentPlayer.CanAffordNode(price))
                    {
                        //BUY THE NODE
                        currentPlayer.BuyProperty(this);
                        OnOwnerUpdated();
                        OnUpdateMessage.Invoke(
                            $"<b>{currentPlayer.name}</color></b> bought <b>{name}</b> for <b><color=green>${price}</color></b>! 🏠");
                    }
                    else
                    {
                        //IS UNOWNED AND CANNOT AFFORD
                    }
                }
                else //HUMAN
                {
                    //IF IT IS OWNED AND WE ARE NOT OWNER AND IS NOT MORTGAGED
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //PAY RENT TO SOMEBODY

                        //CALCUATE RENT

                        //PAY RENT TO OWNER

                        //SHOW A MESSAGE
                    }
                    else if (owner == null)
                    {
                        //SHOW BUY INTERFACE FOR PROPERTY
                    }
                    else
                    {
                        //IS UNOWNED AND CANNOT AFFORD
                    }
                }

                break;
            case MonopolyNodeType.Utility:
                if (!playerIsHuman) //AI
                {
                    //IF IT IS OWNED AND WE ARE NOT OWNER AND IS NOT MORTGAGED
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //PAY RENT TO SOMEBODY

                        //CALCUATE RENT
                        int rentToPay = CalculateUtilityRent();
                        currentRent = rentToPay;
                        //PAY RENT TO OWNER
                        currentPlayer.PayRent(rentToPay, owner);

                        OnUpdateMessage.Invoke(
                            $"<b>{currentPlayer.name}</b> pays Utility rent of <b><color=green>${rentToPay}</color></b> to <b>{owner.name}</b>! 💸");
                    }
                    else if (owner == null && currentPlayer.CanAffordNode(price))
                    {
                        //BUY THE NODE
                        currentPlayer.BuyProperty(this);
                        OnOwnerUpdated();
                        OnUpdateMessage.Invoke(
                            $"<b>{currentPlayer.name}</b> bought <b>{name}</b> for <b><color=green>${price}</color></b>! \ud83d\udee0\ufe0f");
                    }
                    else
                    {
                        //IS UNOWNED AND CANNOT AFFORD
                    }
                }
                else //HUMAN
                {
                    //IF IT IS OWNED AND WE ARE NOT OWNER AND IS NOT MORTGAGED
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //PAY RENT TO SOMEBODY

                        //CALCUATE RENT

                        //PAY RENT TO OWNER

                        //SHOW A MESSAGE
                    }
                    else if (owner == null)
                    {
                        //SHOW BUY INTERFACE FOR PROPERTY
                    }
                    else
                    {
                        //IS UNOWNED AND CANNOT AFFORD
                    }
                }

                break;
            case MonopolyNodeType.Railroad:
                if (!playerIsHuman) //AI
                {
                    //IF IT IS OWNED AND WE ARE NOT OWNER AND IS NOT MORTGAGED
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //PAY RENT TO SOMEBODY
                        int rentToPay = CalculateRailroadRent();
                        currentRent = rentToPay;
                        //PAY RENT TO OWNER
                        currentPlayer.PayRent(rentToPay, owner);

                        OnUpdateMessage.Invoke(
                            $"<b>{currentPlayer.name}</b> pays Railroad rent of <b><color=green>${rentToPay}</color></b> to <b>{owner.name}</b>! 💸");
                    }
                    else if (owner == null && currentPlayer.CanAffordNode(price))
                    {
                        currentPlayer.BuyProperty(this);
                        OnOwnerUpdated();
                        OnUpdateMessage.Invoke(
                            $"<b>{currentPlayer.name}</b> bought <b>{name}</b> for <b><color=green>${price}</color></b>! \ud83d\ude82");
                    }
                    else
                    {
                        //IS UNOWNED AND CANNOT AFFORD
                    }
                }
                else //HUMAN
                {
                    //IF IT IS OWNED AND WE ARE NOT OWNER AND IS NOT MORTGAGED
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //PAY RENT TO SOMEBODY

                        //CALCUATE RENT

                        //PAY RENT TO OWNER

                        //SHOW A MESSAGE
                    }
                    else if (owner == null)
                    {
                        //SHOW BUY INTERFACE FOR PROPERTY
                    }
                    else
                    {
                        //IS UNOWNED AND CANNOT AFFORD
                    }
                }

                break;
            case MonopolyNodeType.Tax:
                GameManager.instance.AddTaxToPool(price);
                currentPlayer.PayMoney(price);
                //SHOW A MESSAGE
                OnUpdateMessage.Invoke(
                    $"<b>{currentPlayer.name}</b> pays <b><color=red>${price}</color></b> in taxes! 💸");

                break;
            case MonopolyNodeType.FreeParking:
                int tax = GameManager.instance.GetTaxPool();
                currentPlayer.CollectMoney(tax);
                //SHOW A MESSAGE
                OnUpdateMessage.Invoke(
                    $"<b>{currentPlayer.name}</b> collects <b><color=green>${tax}</color></b> from Free Parking! 💸");

                break;
            case MonopolyNodeType.GoToJail:
                int indexOnBoard = MonopolyBoard.instance.route.IndexOf(currentPlayer.MyMonopolyNode);
                currentPlayer.GoToJail(indexOnBoard);
                OnUpdateMessage.Invoke($"<b>{currentPlayer.name}</b> is sent to <color=red>jail</color>! 🚓");
                continueTurn = false;
                break;
            case MonopolyNodeType.Chance:
                OnDrawChanceCard.Invoke(currentPlayer);
                continueTurn = false;
                break;
            case MonopolyNodeType.CommunityChest:
                OnDrawCommunityCard.Invoke(currentPlayer);
                continueTurn = false;
                break;
        }

        if (!continueTurn)
        {
            return;
        }

        //continue
        if (!playerIsHuman)
        {
            Invoke("ContinueGame", GameManager.instance.SecondsBeetweenTurns);
        }
        else
        {
        }
    }

    void ContinueGame()
    {
        if (GameManager.instance.RolledADouble)
        {
            //ROLL AGAIN
            GameManager.instance.RollDice();
        }
        else
        {
            //SWITCH PLAYER
            GameManager.instance.SwitchPlayers();
        }
    }

    int CalculatePropertyRent()
    {
        switch (numberOfHouses)
        {
            case 0:
                //CHECK IF OWNER HAS FULL SET OF THIS NODES
                var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(this);
                if (allSame)
                {
                    currentRent = baseRent * 2;
                }
                else
                {
                    currentRent = baseRent;
                }

                break;
            case 1:
                currentRent = rentWithHouses[0];
                break;
            case 2:
                currentRent = rentWithHouses[1];
                break;
            case 3:
                currentRent = rentWithHouses[2];
                break;
            case 4:
                currentRent = rentWithHouses[3];
                break;
            case 5: //HOTEL
                currentRent = rentWithHouses[4];
                break;
        }

        return currentRent;
    }

    int CalculateUtilityRent()
    {
        int[] lastRolledDice = GameManager.instance.LastRolledDice;

        int result = 0;
        var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(this);
        if (allSame)
        {
            result = (lastRolledDice[0] + lastRolledDice[1]) * 10;
        }
        else
        {
            result = (lastRolledDice[0] + lastRolledDice[1]) * 4;
        }

        return result;
    }

    int CalculateRailroadRent()
    {
        int result = 0;
        var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(this);

        int amount = 0;
        foreach (var item in list)
        {
            amount += (item.owner == this.owner) ? 1 : 0;
        }

        result = baseRent * (int)Mathf.Pow(2, amount - 1);

        return result;
    }

    void VisualizeHouses()
    {
        switch (numberOfHouses)
        {
            case 0:
                houses[0].SetActive(false);
                houses[1].SetActive(false);
                houses[2].SetActive(false);
                houses[3].SetActive(false);
                hotel.SetActive(false);
                break;
            case 1:
                houses[0].SetActive(true);
                houses[1].SetActive(false);
                houses[2].SetActive(false);
                houses[3].SetActive(false);
                hotel.SetActive(false);
                break;
            case 2:
                houses[0].SetActive(true);
                houses[1].SetActive(true);
                houses[2].SetActive(false);
                houses[3].SetActive(false);
                hotel.SetActive(false);
                break;
            case 3:
                houses[0].SetActive(true);
                houses[1].SetActive(true);
                houses[2].SetActive(true);
                houses[3].SetActive(false);
                hotel.SetActive(false);
                break;
            case 4:
                houses[0].SetActive(true);
                houses[1].SetActive(true);
                houses[2].SetActive(true);
                houses[3].SetActive(true);
                hotel.SetActive(false);
                break;
            case 5:
                houses[0].SetActive(false);
                houses[1].SetActive(false);
                houses[2].SetActive(false);
                houses[3].SetActive(false);
                hotel.SetActive(true);
                break; 
        }
    }

    public void BuildHouseOrHotel()
    {
        if (monopolyNodeType == MonopolyNodeType.Property && numberOfHouses < 5)
        {
            numberOfHouses++;
            VisualizeHouses();
        }
    }

    public void SellHouseOrHotel()
    {
        if (monopolyNodeType == MonopolyNodeType.Property && numberOfHouses > 0)
        {
            numberOfHouses--;
            VisualizeHouses();
        }
    }

    public void ResetNode()
    {
        //IF IS MORTEGAGED
        if (isMortgaged)
        {
            propertyImage.SetActive(true);
            mortgageImage.SetActive(false);
            isMortgaged = false;
            //UnMortgageProperty();
        }

        //RESET HOUSES AND HOTEL
        if (monopolyNodeType == MonopolyNodeType.Property)
        {
            numberOfHouses = 0;
            VisualizeHouses();
        }

        //RESET THE OWNER
        //REMOVE PROPERTY FROM OWNER
        owner.name = "";

        //UPDATE UI
        OnOwnerUpdated();
    }
}