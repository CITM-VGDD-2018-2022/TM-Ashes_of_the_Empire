using System;
using DiamondEngine;

public class CinematicManager : DiamondComponent
{
    public GameObject gameCamera;
    public GameObject cameraPos1;
    public GameObject cameraPos2;
    private Vector3 initPos;
    private Quaternion initRot;

    public void Awake()
    {
        initPos = gameCamera.transform.localPosition;
        initRot = gameCamera.transform.localRotation;
        Debug.Log(gameCamera.transform.localPosition.ToString());
        Debug.Log(cameraPos1.transform.localPosition.ToString());
        SetAsPerspectiveCamera();

        gameCamera.transform.localPosition = cameraPos1.transform.localPosition;
        gameCamera.transform.localRotation = cameraPos1.transform.localRotation;
    }

    public void Update()
    {

            //ResetInitalTransform();
        
    }
    private void SetAsPerspectiveCamera()
    {
        CameraManager.SetCameraPerspective(gameCamera);
        CameraManager.SetVerticalFOV(gameCamera, 60.0f);
    }

    private void ResetInitalTransform()
    {
        gameCamera.transform.localPosition = initPos;
        gameCamera.transform.localRotation = initRot;
        CameraManager.SetCameraOrthographic(gameCamera);
    }
}