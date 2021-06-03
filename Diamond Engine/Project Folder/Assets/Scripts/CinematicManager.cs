using System;
using DiamondEngine;

public class CinematicManager : DiamondComponent
{
    public GameObject gameCamera;
    public GameObject cameraPos1;
    public GameObject cameraPos2;
    public GameObject helmet;
    public GameObject helmetFinal; 
    public GameObject razorPoint1;
    public GameObject razorPoint2;
    private Vector3 initPos;
    private Quaternion initRot;
    public GameObject postCinematicDialogue;
    public bool init = false;
    private bool razorInit = false;

    private float helmetTimer = 0f;
    public void Awake()
    {
        razorInit = false;
        helmetTimer = 0f;
        if (!init)
        {
            gameCamera.GetComponent<CameraController>().startFollow = true;
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

        if(helmetTimer < 1f)
        {
            helmetTimer += Time.deltaTime * 0.005f;
        }

        //helmet.transform.localPosition = Vector3.Lerp(helmet.transform.localPosition, helmetFinal.transform.localPosition,helmetTimer);
        if (helmet.transform.localPosition.Distance(helmetFinal.transform.localPosition) > 0.2)
        {
            helmet.transform.localPosition += (helmetFinal.transform.localPosition - helmet.transform.localPosition).normalized * Time.deltaTime * 0.40f;

        }
        else
        {
            razorInit = true;
            //Start Razor movement
        }
        helmet.transform.localRotation = Quaternion.Slerp(helmet.transform.localRotation, helmetFinal.transform.localRotation, 0.3f * Time.deltaTime);


        //Razor Movement
        if (razorInit)
        {
            razorPoint1.transform.localPosition += (razorPoint2.transform.localPosition - razorPoint1.transform.localPosition).normalized * Time.deltaTime * 8.0f;
        }

        //ResetInitalTransform();

    }
    private void SetAsPerspectiveCamera()
    {
        CameraManager.SetCameraPerspective(gameCamera);
        CameraManager.SetVerticalFOV(gameCamera, 25.0f);
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