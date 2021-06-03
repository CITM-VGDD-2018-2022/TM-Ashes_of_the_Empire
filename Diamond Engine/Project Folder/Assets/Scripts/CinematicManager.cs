using System;
using DiamondEngine;

public class CinematicManager : DiamondComponent
{
    public GameObject gameCamera;
    public GameObject cameraPos1;
    public GameObject cameraPos2;
    private Vector3 initPos;
    private Quaternion initRot;
    public GameObject postCinematicDialogue;
    public bool init = false;

    public void Awake()
    {

        if (!init)
        {
            gameCamera.GetComponent<CameraController>().startFollow = false;
            postCinematicDialogue.Enable(true);
            postCinematicDialogue.GetChild("Button").GetComponent<Navigation>().Select();
            CameraManager.SetCameraOrthographic(gameCamera);
            return;
        }
        if(gameCamera == null || cameraPos1 == null)
        {
            return;
        }
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
        if(Input.GetKey(DEKeyCode.A) == KeyState.KEY_DOWN)
        {
            ResetInitalTransform();
        }
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
        gameCamera.GetComponent<CameraController>().startFollow = true;
        postCinematicDialogue.Enable(true);
        postCinematicDialogue.GetChild("Button").GetComponent<Navigation>().Select();
    }
}