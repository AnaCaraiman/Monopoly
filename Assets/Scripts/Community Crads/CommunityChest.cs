using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Unity.VisualScripting;
using System.Net.Sockets;
using UnityEngine.UI;
public class CommunityCard : MonoBehaviour
{
    [SerializeField] List<SCR_CommunityCard> cards = new List<SCR_CommunityCard>();
    [SerializeField] TMP_Text cardText;
    [SerializeField] GameObject cardHolderBackground;
    [SerializeField] float showTime = 3; //hide card automatic after 3 sec
    [SerializeField] Button closeCardButton;

    List<SCR_CommunityCard> cardPool = new List<SCR_CommunityCard>();
    List<SCR_CommunityCard> usedCardPool = new List<SCR_CommunityCard>();
    //CURRENT CARD AND CURRENT PLAYER
    SCR_CommunityCard pickedCard;
    Player currentPlayer;

    private void OnEnable()
    {
        MonopolyNode.OnDrawCommunityCard += DrawCard;
    }

    private void OnDisable()
    {
        MonopolyNode.OnDrawCommunityCard -= DrawCard;
    }

    private void Start()
    {
        cardHolderBackground.SetActive(false);
        //ADD ALL CARDS TO THE POOL
        cardPool.AddRange(cards);
        //SHUFFLE THE CARDS
        ShuffleCards();
    }

    void ShuffleCards()
    { 
        for (int i = 0; i < cardPool.Count; i++)
        {
            int index = Random.Range(0, cardPool.Count);
            SCR_CommunityCard tempCard = cardPool[index];
            cardPool[index] = cardPool[i];
            cardPool[i] = tempCard;
        }
    }

    void DrawCard(Player cardTaker)
    {
        //DRAW AN ACTUAL CARD
        pickedCard = cardPool[0];
        cardPool.RemoveAt(0);
        usedCardPool.Add(pickedCard);
        if(cardPool.Count == 0)
        {
            //PUT BACK ALL CARDS
            cardPool.AddRange(usedCardPool);
            usedCardPool.Clear();
            //SHUFFLE THE CARDS
            ShuffleCards();
        }
        //WHO IS CURRENT PLAYER
        currentPlayer = cardTaker;
        //SHOW CARD
        cardHolderBackground.SetActive(true);

        //FILL IN THE TEXT ON THE DESCRIPTION
        cardText.text = pickedCard.textOnCard;

        //DEACTIVATE THE BUTTON IF WE ARE AN AI PLAYER
        if(currentPlayer.playerType == Player.PlayerType.AI)
        {
            closeCardButton.interactable = false;
            Invoke("ApplyCardEffect", showTime);   
        }
        else
        {
            closeCardButton.interactable = true;
        }

    }

    public void ApplyCardEffect() //close button of the card
    {
        bool isMoving = false;
        if (pickedCard.rewardMoney != 0 && !pickedCard.collectFromPlayer)
        {
            currentPlayer.CollectMoney(pickedCard.rewardMoney);
        }
        else if (pickedCard.penalityMoney != 0)
        {
            currentPlayer.PayMoney(pickedCard.penalityMoney); //HANDLE INSUFF FUNDS
        }
        else if (pickedCard.moveToBoardIndex != -1)
        {
            isMoving = true;
            //STPES TO GOAL
            int currentIndex = MonopolyBoard.instance.route.IndexOf(currentPlayer.MyMonopolyNode);
            int lentgthOfBoard = MonopolyBoard.instance.route.Count;
            int stepsToMove = 0;
            if (currentIndex < pickedCard.moveToBoardIndex)
            {
                stepsToMove = pickedCard.moveToBoardIndex - currentIndex;
            }
            else if (currentIndex > pickedCard.moveToBoardIndex)
            {
                stepsToMove = lentgthOfBoard - currentIndex + pickedCard.moveToBoardIndex;
            }

            //START TO MOVE
            MonopolyBoard.instance.MovePlayerToken(stepsToMove, currentPlayer);
        }
        else if (pickedCard.collectFromPlayer)
        {
            int totalCollected = 0;
            List<Player> allPlayers = GameManager.instance.GetPlayers;

            foreach (var player in allPlayers)
            {
                if (player != currentPlayer)
                {
                    //PREVENT BANKRUPCY
                    int amount = Mathf.Min(player.ReadMoney, pickedCard.rewardMoney);
                    player.PayMoney(amount);
                    totalCollected += amount;
                }
            }
            currentPlayer.CollectMoney(totalCollected);
        }
        else if (pickedCard.streetRepairs)
        {
            int[] allBuilding = currentPlayer.CountHousesAndHotels();
            int totalCosts = pickedCard.streetRepairsHousePrice * allBuilding[0] + pickedCard.streetRepairsHotelPrice * allBuilding[1];
            currentPlayer.PayMoney(totalCosts);
        }
        else if (pickedCard.goToJail)
        {
            isMoving = true;
            currentPlayer.GoToJail(MonopolyBoard.instance.route.IndexOf(currentPlayer.MyMonopolyNode));
        }
        else if (pickedCard.jailFreeCard) //JAIL FREE CARD
        {

        }
        cardHolderBackground.SetActive(false);
        ContinueGame(isMoving);
    }

    void ContinueGame(bool isMoving)
    {
        if (currentPlayer.playerType == Player.PlayerType.AI)
        {
            if (!isMoving && GameManager.instance.RolledADouble)
            {
                GameManager.instance.RollDice();
            }
            else if(!isMoving && !GameManager.instance.RolledADouble)
            {
                GameManager.instance.SwitchPlayers();
            }
        }
        else //HUMAN INPUT
        { 
            
        }
    }
}
