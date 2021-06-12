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

    float speed1 = 1.0f;
    float timer2 = 0.0f;
    float time2 = 1.0f;
    float speed3 = 1.0f;
    float destinationDistance = 0.2f;

    public void Awake()
    {
        if (gameCamera == null || initialCamera1 == null || endCamera1 == null || initialCamera3 == null || endCamera3 == null)
        {
            sequenceCounter = amountOfSequencesPlusOne;
            return;
        }

        defaultCameraTransform = gameCamera.transform;
        gameCamera.transform.localPosition = initialCamera1.transform.localPosition;

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
                    gameCamera.transform.localPosition += (endCamera1.transform.localPosition - gameCamera.transform.localPosition) * newDeltaTime * speed1;
                    if (Mathf.Distance(gameCamera.transform.localPosition, endCamera1.transform.localPosition) <= destinationDistance)
                    {
                        sequenceCounter++;
                    }
                    break;

                case 1:
                    timer2 += Time.deltaTime;
                    if (timer2 > time2)
                    {
                        sequenceCounter++;
                        gameCamera.transform = initialCamera3.transform;
                    }
                    break;

                case 2:
                    gameCamera.transform.localPosition += (endCamera3.transform.localPosition - gameCamera.transform.localPosition).normalized * newDeltaTime * speed3;
                    gameCamera.transform.localRotation = Quaternion.Slerp(gameCamera.transform.localRotation, endCamera3.transform.localRotation, 0.25f * newDeltaTime);

                    if (Mathf.Distance(gameCamera.transform.localPosition, endCamera3.transform.localPosition) <= destinationDistance)
                    {
                        sequenceCounter++;
                    }
                    break;
            }

            if (sequenceCounter >= amountOfSequencesPlusOne)
            {
                EndCinematic();
            }
        }
    }

    private void EndCinematic()
    {
        BlackFade.StartFadeIn(() =>
        {
            gameCamera.transform = defaultCameraTransform;
            CameraManager.SetCameraOrthographic(gameCamera);
            BlackFade.StartFadeOut(() =>
            {
                gameCamera.GetComponent<CameraController>().startFollow = true;
            });
        });
    }

}