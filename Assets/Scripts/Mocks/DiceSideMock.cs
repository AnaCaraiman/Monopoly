using System;
using UnityEngine;

public class MockDiceSide : MonoBehaviour
{
    public bool onGround;
    public bool OnGround => onGround;

    public void OnTriggerStay(Collider col)
    {
        if (col.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    public void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Ground"))
        {
            onGround = false;
        }
    }

    public int SideValue()
    {
        int value = Int32.Parse(name);
        return value;
    }
}