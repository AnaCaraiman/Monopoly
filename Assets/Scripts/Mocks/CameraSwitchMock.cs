using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MockCameraSwitcher : MonoBehaviour
{
    public static MockCameraSwitcher instance;
    public CinemachineVirtualCamera topDownCamera;
    public CinemachineVirtualCamera diceCamera;
    public CinemachineVirtualCamera playerFollowCamera;

    void Awake()
    {
        instance = this;
    }

    public void switchToTopDown()
    {
        topDownCamera.Priority = 2;
        diceCamera.Priority = 0;
        playerFollowCamera.Priority = 0;
    }

    public void switchToDice()
    {
        topDownCamera.Priority = 0;
        diceCamera.Priority = 2;
        playerFollowCamera.Priority = 0;
    }

    public void switchToPlayer(Transform followTarget)
    {
        topDownCamera.Priority = 0;
        diceCamera.Priority = 0;
        playerFollowCamera.Priority = 2;
        playerFollowCamera.Follow = followTarget;
        playerFollowCamera.LookAt = followTarget;
    }
}