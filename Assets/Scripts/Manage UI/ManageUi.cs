using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageUi : MonoBehaviour
{
    [SerializeField] GameObject managePanel; // to show and hide
    [SerializeField] Transform propertyGrid; // to parent property sets to it
    [SerializeField] GameObject propertySetPrefab;
    Player playerReference;
    List<GameObject> propertyPrefabs = new List<GameObject>();

    void Start()
    {
        managePanel.SetActive(false);
    }

     public void OpenManager() // call from manage button
    {
        playerReference = GameManager.instance.GetCurrentPlayer;
        //get all nodes as node sets
        List<MonopolyNode> processedSet = null;

        foreach ( var node in playerReference.GetMonopolyNodes)
        {
            var (list, allsame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(node);
            List<MonopolyNode> nodeSet = new List<MonopolyNode>();
            nodeSet.AddRange(list);

            if(nodeSet != null && list != processedSet)
            {
                
                processedSet = list;

                nodeSet.RemoveAll(n => n.Owner != playerReference);

                //create prefab with all nodes owned by player
                GameObject newPropertySet = Instantiate(propertySetPrefab, propertyGrid, false);
                newPropertySet.GetComponent<ManagePropertyUi>().SetProperty(nodeSet, playerReference);

                propertyPrefabs.Add(newPropertySet);
            }
        }
        managePanel.SetActive(true);
    }

    public void CloseManager() 
    {
        managePanel.SetActive(false);
        for (int i = propertyPrefabs.Count - 1; i >= 0; i--)
        {
            Destroy(propertyPrefabs[i]);
        }
        propertyPrefabs.Clear();
    }
}
