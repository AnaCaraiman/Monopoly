using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DicePlayModeTest
{
    private GameObject diceObject;
    private Dice dice;
    private Rigidbody rb;

    [SetUp]
    public void Setup()
    {
        // Creăm zarul în scenă
        diceObject = new GameObject("Dice");
        rb = diceObject.AddComponent<Rigidbody>();
        dice = diceObject.AddComponent<Dice>();

        // Adăugăm collider pentru coliziuni
        diceObject.AddComponent<BoxCollider>();

        // Simulăm Start()
        dice.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
    }

    [TearDown]
    public void Teardown()
    {
        GameObject.Destroy(diceObject);
    }

    [UnityTest]
    public IEnumerator Dice_RollsAndStops()
    {
        dice.RollDice();

        yield return new WaitForSeconds(2f); 

        Assert.IsTrue(!rb.IsSleeping());
        Assert.IsFalse(!rb.useGravity);
    }

    [UnityTest]
    public IEnumerator Dice_ReRollsIfZero()
    {
        dice.RollDice();
        yield return new WaitForSeconds(2f);
        
        FieldInfo diceValueField = typeof(Dice).GetField("diceValue", BindingFlags.NonPublic | BindingFlags.Instance);
        diceValueField.SetValue(dice, 0);
        
        dice.Update();

        yield return new WaitForSeconds(2f);

        Assert.IsTrue(rb.useGravity);
    }
}