using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class DiceSideEditModeTest
{
    private MockDiceSide diceSide;
    private GameObject ground;

    [SetUp]
    public void Setup()
    {
        GameObject diceSideObject = new GameObject("1");
        diceSide = diceSideObject.AddComponent<MockDiceSide>();

        ground = new GameObject("Ground");
        ground.tag = "Ground";
        ground.AddComponent<BoxCollider>();
    }

    [Test]
    public void OnTriggerStay_SetsOnGroundTrue()
    {
        // Arrange
        Collider groundCollider = ground.GetComponent<Collider>();

        // Act
        diceSide.OnTriggerStay(groundCollider);

        // Assert
        Assert.IsTrue(diceSide.OnGround);
    }

    [Test]
    public void OnTriggerExit_SetsOnGroundFalse()
    {
        // Arrange
        Collider groundCollider = ground.GetComponent<Collider>();

        // Act
        diceSide.OnTriggerStay(groundCollider);
        diceSide.OnTriggerExit(groundCollider);

        // Assert
        Assert.IsFalse(diceSide.OnGround);
    }

    [Test]
    public void SideValue_ReturnsCorrectValue()
    {
        // Act
        int value = diceSide.SideValue();

        // Assert
        Assert.AreEqual(1, value);
    }
}