using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Community Card", menuName ="Monopoly/Cards/Community")]
public class SCR_CommunityCard : ScriptableObject
{
    public string textOnCard;//description of the card
    public int rewardMoney;
    public int penalityMoney;
    public int moveToBoardIndex = -1;
    public bool collectFromPlayer;
    [Header("Jail Content")]
    public bool goToJail;
    public bool jailFreeCard;

    [Header("Street Repairs")]
    public bool streetRepairs;
    public int streetRepairsHousePrice = 40;
    public int streetRepairsHotelPrice = 115;

}
