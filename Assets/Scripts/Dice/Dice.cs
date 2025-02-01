using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    Rigidbody rb;
    bool hasLanded;
    bool thrown;

    Vector3 initPosition;
    int diceValue;

    [SerializeField] DiceSide[] diceSide;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initPosition = transform.position;
        rb.useGravity = false;
        rb.isKinematic = true;
    }


    // Update is called once per frame
    public void Update()
    {
        if(rb.IsSleeping() && !hasLanded && thrown)
        {
            hasLanded = true;
            rb.useGravity = false;
            rb.isKinematic = true;
            SideValueCheck();
        }
        else if(rb.IsSleeping() && hasLanded && diceValue == 0)
        {
            ReRollDice();
           
        }
    }

    public void RollDice()
    {
        Reset();
        if(!thrown && !hasLanded)
        {
            Reset();
            thrown = true;
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.AddTorque(Random.Range(0,500), Random.Range(0,500), Random.Range(0,500));
        }
        //else if(thrown && hasLanded)
        //{
            //reset dice
            //Reset();
        //}
    }
     void Reset()
    {
         transform.position = initPosition;
         thrown = false;
         hasLanded = false;
         rb.useGravity = false;
         rb.isKinematic = true;
     }


    void ReRollDice()
    {
        // if (hasLanded) 
        // {
            Reset(); 
            thrown = true; 
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.AddTorque(Random.Range(0, 500), Random.Range(0, 500), Random.Range(0, 500));
        // }
    }


    void SideValueCheck()
    {
        diceValue = 0;
        foreach(var side in diceSide)
        {
            if (side.OnGround)
            {
                diceValue = side.SideValue();
                Debug.Log("ROLLED NUMBER = " + diceValue);
                break;
            }
        }
        GameManager.instance.ReportDiceRolled(diceValue);
    }
}
