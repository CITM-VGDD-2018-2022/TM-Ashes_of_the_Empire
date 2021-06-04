using System;
using DiamondEngine;
using System.Collections.Generic;

public class CinematicManager : DiamondComponent
{
    public GameObject gameCamera;
    public GameObject cameraPos1;

    public GameObject helmet;
    public GameObject razor;


    private Vector3 initPos;
    private Quaternion initRot;
    public GameObject postCinematicDialogue;
    public bool init = false;

    private Sequence helmetSequence;
    private Sequence razorSequence;
    private Sequence cameraSequence;
    private List<Sequence> listSequences = new List<Sequence>();
    public void Awake()
    {
        if(helmet != null)
        {
            helmetSequence =  helmet.GetComponent<Sequence>();
            if (helmetSequence != null)
            {
                listSequences.Add(helmetSequence);

            }
        }   
        
        if(razor != null)
        {
            razorSequence =  razor.GetComponent<Sequence>();
            if (razorSequence != null)
            {
                listSequences.Add(razorSequence);
                helmetSequence.startNextSequence += () => { razorSequence.isRunning = true; };
                
            }
        }        
        if(gameCamera != null)
        {
            cameraSequence =  gameCamera.GetComponent<Sequence>();
            if (cameraSequence != null)
            {
                listSequences.Add(cameraSequence);
                razorSequence.startNextSequence += () => { cameraSequence.isRunning = true; };
                razorSequence.onEndSequence += () => { cameraSequence.endSequence = true; };
            }
        }




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

        if (!init)
        {
            return;
        }

        foreach (Sequence sequence in listSequences)
        {
            sequence.RunSequence();
        }

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