using System;
using DiamondEngine;

public class Sequence : DiamondComponent
{
    public Action startNextSequence;
    public Action onEndSequence;
    public Sequence numSequence;
    public float sequenceTime;
    public float speed;
    public GameObject point0;
    public GameObject point1;
    public GameObject teleportPoint1;
    public bool rotation;
    public bool isRunning;
    private float sequenceTimer;
    private float timerToNextSequence;
    public float timeToNextSequence;
    private bool nextSequence = false;
    public float rotationSpeed;
    public bool endSequence = false;

    public void Awake()
    {
        endSequence = false;
    }
    public void RunSequence()
    {

        if(timerToNextSequence < timeToNextSequence && isRunning)
        {
            timerToNextSequence += Time.deltaTime;
        }
        else if(isRunning)
        {
            startNextSequence?.Invoke();
            Debug.Log("Finish next sequence");
            
            nextSequence = true;
            
        }

        if (endSequence)
        {
            if (teleportPoint1 != null)
            {
           
                gameObject.transform.localPosition = teleportPoint1.transform.globalPosition;
                gameObject.transform.localRotation = teleportPoint1.transform.localRotation;

            }
            return;

        }

        if (isRunning)
        {


            if (nextSequence)
            {
                if (point1 != null)
                {
                    gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, point1.transform.localRotation, rotationSpeed * Time.deltaTime);
                }
            }

            if (sequenceTimer < sequenceTime)
            {
                sequenceTimer += Time.deltaTime;
                if (point0 != null)
                {
                    if (gameObject.transform.localPosition.DistanceNoSqrt(point0.transform.localPosition) > 0.1f)
                    {
                        gameObject.transform.localPosition += (point0.transform.localPosition - gameObject.transform.localPosition).normalized * Time.deltaTime * speed;
                    }
                    else
                    {
                        onEndSequence?.Invoke();
                    }

                    if (rotation && !nextSequence)
                    {
                        gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, point0.transform.localRotation, 0.4f * Time.deltaTime);
                    }
                }

            }
            else
            {
                isRunning = false;
                sequenceTimer = 0;
                //endSequence = true;
            }



        }
    }

}