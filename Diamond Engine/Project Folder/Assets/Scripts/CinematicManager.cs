using System;
using DiamondEngine;
using System.Collections.Generic;

public class CinematicManager : DiamondComponent
{
    public GameObject gameCamera;
    public GameObject cameraPos1;

    public GameObject sequence1;
    public GameObject sequence2;
    public GameObject sequence3;
    public GameObject sequence4;
    public GameObject sequence5;
    public GameObject sequence6;


    private Vector3 initPos;
    private Quaternion initRot;
    public GameObject postCinematicDialogue;
    public bool init = false;

    private List<Sequence> listSequences = new List<Sequence>();
    public void Awake()
    {


        if (!init)
        {
            gameCamera.GetComponent<CameraController>().startFollow = true;
            postCinematicDialogue.Enable(true);
            postCinematicDialogue.GetChild("Button").GetComponent<Navigation>().Select();
            CameraManager.SetCameraOrthographic(gameCamera);
            return;
        }


        AddSequence(sequence1);
        AddSequence(sequence2);
        AddSequence(sequence3);
        AddSequence(sequence4);
        AddSequence(sequence5);
        AddSequence(sequence6);

        //Start first sequence
        if (listSequences.Count > 0)
        {
            listSequences[0].StartRunning();
        }





        if (gameCamera == null || cameraPos1 == null)
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

    private void AddSequence(GameObject sequenceObject)
    {
        if (sequenceObject == null) 
            return;

        Sequence sequence = sequenceObject.GetComponent<Sequence>();

        if(sequence != null)
        {
            listSequences.Add(sequence);
        }
    }
}