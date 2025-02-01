using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Cinemachine;

public class CameraSwitchEditModeTest
{
    private MockCameraSwitcher cameraSwitcher;
    private CinemachineVirtualCamera topDownCamera;
    private CinemachineVirtualCamera diceCamera;
    private CinemachineVirtualCamera playerFollowCamera;

    [SetUp]
    public void Setup()
    {
        GameObject cameraSwitcherObject = new GameObject("CameraSwitcher");
        cameraSwitcher = cameraSwitcherObject.AddComponent<MockCameraSwitcher>();

        GameObject topDownCameraObject = new GameObject("TopDownCamera");
        topDownCamera = topDownCameraObject.AddComponent<CinemachineVirtualCamera>();
        cameraSwitcher.topDownCamera = topDownCamera;

        GameObject diceCameraObject = new GameObject("DiceCamera");
        diceCamera = diceCameraObject.AddComponent<CinemachineVirtualCamera>();
        cameraSwitcher.diceCamera = diceCamera;

        GameObject playerFollowCameraObject = new GameObject("PlayerFollowCamera");
        playerFollowCamera = playerFollowCameraObject.AddComponent<CinemachineVirtualCamera>();
        cameraSwitcher.playerFollowCamera = playerFollowCamera;
    }

    [Test]
    public void SwitchToTopDown_SetsCorrectCameraPriority()
    {
        // Act
        cameraSwitcher.switchToTopDown();

        // Assert
        Assert.AreEqual(2, topDownCamera.Priority);
        Assert.AreEqual(0, diceCamera.Priority);
        Assert.AreEqual(0, playerFollowCamera.Priority);
    }

    [Test]
    public void SwitchToDice_SetsCorrectCameraPriority()
    {
        // Act
        cameraSwitcher.switchToDice();

        // Assert
        Assert.AreEqual(0, topDownCamera.Priority);
        Assert.AreEqual(2, diceCamera.Priority);
        Assert.AreEqual(0, playerFollowCamera.Priority);
    }

    [Test]
    public void SwitchToPlayer_SetsCorrectCameraPriorityAndFollowTarget()
    {
        // Arrange
        GameObject followTarget = new GameObject("FollowTarget");
        Transform followTransform = followTarget.transform;

        // Act
        cameraSwitcher.switchToPlayer(followTransform);

        // Assert
        Assert.AreEqual(0, topDownCamera.Priority);
        Assert.AreEqual(0, diceCamera.Priority);
        Assert.AreEqual(2, playerFollowCamera.Priority);
        Assert.AreEqual(followTransform, playerFollowCamera.Follow);
        Assert.AreEqual(followTransform, playerFollowCamera.LookAt);
    }
}