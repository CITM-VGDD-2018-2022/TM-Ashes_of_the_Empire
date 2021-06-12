using System;
using DiamondEngine;

public class MoffGideonCinematic : DiamondComponent
{   // The classic script you have to do two days before ending the project
    public GameObject gameCamera = null;
    public GameObject initialCamera1 = null;
    public GameObject endCamera1 = null;
    public GameObject initialCamera3 = null;
    public GameObject endCamera3 = null;
    private Transform defaultCameraTransform = null;

    int sequenceCounter = 0;
    int amountOfSequencesPlusOne = 3;
    Vector3 cameraAuxPosition = new Vector3(0.0f, 0.0f, 0.0f);

    float speed1 = 1.0f;
    float timer2 = 0.0f;
    float time2 = 0.75f;
    float speed3 = 3.0f;
    float destinationDistance = 0.2f;

    public void Awake()
    {
        if (gameCamera == null || initialCamera1 == null || endCamera1 == null || initialCamera3 == null || endCamera3 == null)
        {
            sequenceCounter = amountOfSequencesPlusOne;
            return;
        }

        defaultCameraTransform = gameCamera.transform;
        cameraAuxPosition = gameCamera.transform.localPosition = initialCamera1.transform.localPosition;
        gameCamera.transform.localRotation = initialCamera1.transform.localRotation;
        gameCamera.GetComponent<CameraController>().startFollow = false;
        CameraManager.SetCameraPerspective(gameCamera);
        CameraManager.SetVerticalFOV(gameCamera, 60.0f);

    }

    public void Update()
    {
        if (sequenceCounter < amountOfSequencesPlusOne)
        {
            float newDeltaTime = Time.deltaTime;
            if (newDeltaTime > 0.016f)
            {
                newDeltaTime = 0.016f;
            }

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
        BlackFade.StartFadeIn(() =>
        {
            gameCamera.transform.localPosition = defaultCameraTransform.localPosition;
            gameCamera.transform.localRotation = defaultCameraTransform.localRotation;
            CameraManager.SetCameraOrthographic(gameCamera);
            BlackFade.StartFadeOut(() =>
            {
                gameCamera.GetComponent<CameraController>().startFollow = true;
            });
        });
    }

}