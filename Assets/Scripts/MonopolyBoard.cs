using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MonopolyBoard : MonoBehaviour
{
    public List<MonopolyNode> route = new List<MonopolyNode>();

    [System.Serializable]
    public class NodeSet
    {
        public Color setColor = Color.white;
        public List<MonopolyNode> nodesInSetList = new List<MonopolyNode>();
    }

    [SerializeField] List<NodeSet> nodeSetList = new List<NodeSet>();

    void OnValidate()
    {
        route.Clear();  
        foreach(Transform node in transform.GetComponentInChildren<Transform>())
        {
            route.Add(node.GetComponent<MonopolyNode>());
        }

        //UPDATE ALL NODE COLORS
        for (int i = 0; i < nodeSetList.Count; i++)
        {
            for (int j = 0; j < nodeSetList[i].nodesInSetList.Count; j++)
            {
                nodeSetList[i].nodesInSetList[j].UpdateColorField(nodeSetList[i].setColor);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(route.Count > 1) 
        {
            for (int i = 0; i < route.Count; i++)
            {
                Vector3 current = route[i].transform.position;
                Vector3 next = (i + 1 < route.Count) ? route[i+1].transform.position : current;

                Gizmos.color = Color.green;
                Gizmos.DrawLine(current, next);
            }    
        }
    }

    public void MovePlayerToken(int steps,Player player)
    {
        StartCoroutine(MovePlayerInStep(steps, player));
    }

    IEnumerator MovePlayerInStep(int steps, Player player)
    {
        int stepsLeft = steps;
        GameObject tokenToMove = player.MyToken;
        int indexOnBoard = route.IndexOf(player.CurrentNode);
        bool moveOverGo = false;
        while(stepsLeft>0) 
        {
            indexOnBoard++;

            if(indexOnBoard>route.Count-1) {
                indexOnBoard = 0;
                moveOverGo = true;
            }

            Vector3 startPos = tokenToMove.transform.position;
            Vector3 endPos = route[indexOnBoard].transform.position;

            while(MoveToNextNode(tokenToMove, endPos, 10f))
            {
                yield return null;
            }

            stepsLeft--;
        }
        if(moveOverGo){
            player.CollectMoney(GameManager.instance.GetGoMoney);
        }

        player.SetMyCurrentNode(route[indexOnBoard]);

    }

    bool MoveToNextNode(GameObject tokenToMove, Vector3 endPos, float speed)
    {
        return endPos != (tokenToMove.transform.position = Vector3.MoveTowards(tokenToMove.transform.position, endPos, speed * Time.deltaTime));
    }


}
