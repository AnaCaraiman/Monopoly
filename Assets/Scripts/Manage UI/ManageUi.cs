using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class ManageUi : MonoBehaviour
{
    public static ManageUi instance;

    [SerializeField] GameObject managePanel; // to show and hide
    [SerializeField] Transform propertyGrid; // to parent property sets to it
    [SerializeField] GameObject propertySetPrefab;
    Player playerReference;
    List<GameObject> propertyPrefabs = new List<GameObject>();
    [SerializeField] TMP_Text yourMoneyText;
    [SerializeField] TMP_Text systemMessageText;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        managePanel.SetActive(false);
    }

     public void OpenManager() // call from manage button
    {
        playerReference = GameManager.instance.GetCurrentPlayer;

        CreateProperties();

        managePanel.SetActive(true);
        UpdateMoneyText();
    }

    public void CloseManager() 
    {
        managePanel.SetActive(false);
        ClearProperties();
    }

    void ClearProperties()
    {
        for (int i = propertyPrefabs.Count - 1; i >= 0; i--)
        {
            Destroy(propertyPrefabs[i]);
        }
        propertyPrefabs.Clear();
    }

    void CreateProperties()
    {
        //get all nodes as node sets
        List<MonopolyNode> processedSet = null;

        foreach (var node in playerReference.GetMonopolyNodes)
        {
            var (list, allsame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(node);
            List<MonopolyNode> nodeSet = new List<MonopolyNode>();
            nodeSet.AddRange(list);

            if (nodeSet != null && list != processedSet)
            {

                processedSet = list;

                nodeSet.RemoveAll(n => n.Owner != playerReference);

                //create prefab with all nodes owned by player
                GameObject newPropertySet = Instantiate(propertySetPrefab, propertyGrid, false);
                newPropertySet.GetComponent<ManagePropertyUi>().SetProperty(nodeSet, playerReference);

                propertyPrefabs.Add(newPropertySet);
            }
        }
    }

    public void UpdateMoneyText()
    {
        string showMoney = (playerReference.ReadMoney >= 0) ? "<color=green>RON" + playerReference.ReadMoney : "<color=red>RON" + playerReference.ReadMoney;
        yourMoneyText.text = "<color=black>Your Money: </color>" + showMoney;
    }

    public void UpdateSystemMessage(string message)
    {
        systemMessageText.text = message;
    }

    public void AutoHandleFunds()//call from button
    {
        if(playerReference.ReadMoney > 0)
        {
            UpdateSystemMessage("You don't need to do that, you have enough money!");
            return;
        }
        playerReference.HandleInsufficientFunds(Mathf.Abs(playerReference.ReadMoney));
        //update the ui
        ClearProperties();
        CreateProperties();
        //update system message
        UpdateMoneyText();
    }
}
