using System;
using DiamondEngine;

public class CinematicManager : DiamondComponent
{
    private GameObject gameCamera;
    public GameObject cameraPos1;

    public void Awake()
    {
        gameCamera = InternalCalls.FindObjectWithName("Game Camera");

        if (cameraPos1 != null)
        {
            gameCamera.transform = cameraPos1.transform;
        }
    }
}