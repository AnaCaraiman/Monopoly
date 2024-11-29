using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Chance Card", menuName = "Monopoly/Cards/Chance")]
public class SCR_ChanceCard : ScriptableObject
{
    public string textOnCard;//description of the card
    public int rewardMoney;
    public int penalityMoney;
    public int moveToBoardIndex = -1;
    public bool payToPlayer;
    [Header("Move To Location")]
    public bool nextRailroad;
    public bool nextUtility;
    public int moveStepsBackwards;

    [Header("Jail Content")]
    public bool goToJail;
    public bool jailFreeCard;

    [Header("Street Repairs")]
    public bool streetRepairs;
    public int streetRepairsHousePrice = 25;
    public int streetRepairsHotelPrice = 100;

}
