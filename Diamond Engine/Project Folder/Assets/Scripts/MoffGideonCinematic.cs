using System;
using DiamondEngine;

public class MoffGideonCinematic : DiamondComponent
{   // The classic script you have to do two days before ending the project
    public GameObject gameCamera = null;
    public GameObject initialCamera1 = null;
    public GameObject endCamera1 = null;
    public GameObject initialCamera3 = null;
    public GameObject endCamera3 = null;
    public GameObject moffGideon = null;
    private Vector3 defaultCameraPos = null;
    private Quaternion defaultCameraRot = null;
    private bool start = false;

    int sequenceCounter = 0;
    int amountOfSequencesPlusOne = 3;
    Vector3 cameraAuxPosition = new Vector3(0.0f, 0.0f, 0.0f);

    float speed1 = 1.0f;
    float timer2 = 0.0f;
    float time2 = 0.55f;
    float speed3 = 3.75f;
    float destinationDistance = 0.2f;

    public void Awake()
    {
        if (gameCamera == null || initialCamera1 == null || endCamera1 == null || initialCamera3 == null || endCamera3 == null)
        {
            sequenceCounter = amountOfSequencesPlusOne;
            return;
        }

        defaultCameraPos = gameCamera.transform.localPosition;
        defaultCameraRot = gameCamera.transform.localRotation;
        cameraAuxPosition = gameCamera.transform.localPosition = initialCamera1.transform.localPosition;
        gameCamera.transform.localRotation = initialCamera1.transform.localRotation;
        gameCamera.GetComponent<CameraController>().startFollow = false;
        CameraManager.SetCameraPerspective(gameCamera);
        CameraManager.SetVerticalFOV(gameCamera, 60.0f);

    }

    public void Update()
    {
        if (start == false)
        {
            if (Core.instance != null)
            {
                Core.instance.LockInputs(true);

                if (Core.instance.hud != null)
                {
                    Core.instance.hud.Enable(false);
                }

            }

            start = true;
        }

        if (sequenceCounter < amountOfSequencesPlusOne)
        {
            if (Core.instance != null)
            {
                if (Core.instance.hud != null)
                {
                    Core.instance.hud.Enable(false);
                }
            }

            float newDeltaTime = Time.deltaTime;
            switch (sequenceCounter)
            {
                case 0:
                    cameraAuxPosition += (endCamera1.transform.localPosition - gameCamera.transform.localPosition).normalized * newDeltaTime * speed1;

                    if (Mathf.Distance(gameCamera.transform.localPosition, endCamera1.transform.localPosition) <= destinationDistance)
                    {
                        sequenceCounter++;
                    }
                    break;

                case 1:
                    timer2 += newDeltaTime;
                    if (timer2 > time2)
                    {
                        sequenceCounter++;
                        gameCamera.transform.localPosition = initialCamera3.transform.localPosition;
                        gameCamera.transform.localRotation = initialCamera3.transform.localRotation;
                    }
                    break;

                case 2:
                    cameraAuxPosition += (endCamera3.transform.localPosition - gameCamera.transform.localPosition).normalized * newDeltaTime * speed3;
                    gameCamera.transform.localRotation = Quaternion.Slerp(gameCamera.transform.localRotation, endCamera3.transform.localRotation, 0.25f * newDeltaTime);

                    if (Mathf.Distance(gameCamera.transform.localPosition, endCamera3.transform.localPosition) <= destinationDistance)
                    {
                        sequenceCounter++;
                    }
                    break;
            }

            gameCamera.transform.localPosition = cameraAuxPosition;

            if (sequenceCounter >= amountOfSequencesPlusOne)
            {
                EndCinematic();
            }

            if (Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_DOWN || Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_REPEAT)
            {
                EndCinematic();
            }
        }
    }

    private void EndCinematic()
    {
        sequenceCounter = amountOfSequencesPlusOne;
        BlackFade.SetFadeSpdMult(1.5f);
        BlackFade.StartFadeIn(() =>
        {
            gameCamera.transform.localPosition = defaultCameraPos;
            gameCamera.transform.localRotation = defaultCameraRot;
            CameraManager.SetCameraOrthographic(gameCamera);

            if (moffGideon != null)
            {
                MofGuideonRework moffScript = moffGideon.GetComponent<MofGuideonRework>();

                if (moffScript != null)
                {
                    moffScript.HideCape();
                }
            }

            if (Core.instance != null)
            {
                if (Core.instance.hud != null)
                {
                    Core.instance.hud.Enable(true);
                }
            }

            BlackFade.StartFadeOut(() =>
            {
                gameCamera.GetComponent<CameraController>().startFollow = true;

                if (Core.instance != null)
                {
                    Core.instance.LockInputs(false);
                    if (Core.instance.hud != null)
                    {
                        Core.instance.hud.Enable(true);
                    }
                }

            });

        });
    }

}