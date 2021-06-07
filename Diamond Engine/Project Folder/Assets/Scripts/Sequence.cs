using System;
using DiamondEngine;

public class Sequence : DiamondComponent
{
    public Action startNextSequence;
    public Action onEndSequence;
    public Sequence numSequence;
    public float sequenceTime;
    public float speed;
    //public GameObject point0;
    //public GameObject point1;
    //public GameObject teleportPoint1;
    private bool isRunning = false;
    private float sequenceTimer;
    //private float timerToNextSequence;
    //public float timeToNextSequence;
    public bool endSequence = false;

    //Game Objects of the sequence
    public GameObject object1;
    public GameObject object2;
    public GameObject object3;

    //Positions of the objects
    public GameObject object1_pos1;
    public bool transition1;
    public GameObject object1_pos2;
    public float speedRotationObject1;

    public GameObject object2_pos1;
    public bool transition2;
    public GameObject object2_pos2;
    public float speedRotationObject2;
    
    public GameObject object3_pos1;
    public bool transition3;
    public GameObject object3_pos2;
    public float speedRotationObject3;

    public bool rotateObject1 = false;
    public bool rotateObject2 = false;
    public bool rotateObject3 = false;

    public GameObject nextSequenceToStart;
    private bool startedNextSequence = false;
    //Timers to next sequence
    private float timerToNextSequence;
    public float timeToNextSequence;

    private bool startObject2 = false;
    private bool startObject3 = false;
    public void Awake()
    {
        //endSequence = false;
    }
    public void RunSequence()
    {
        if(!isRunning)
        {
            return;
        }



        if (!startedNextSequence)
        {
            if (timerToNextSequence < timeToNextSequence)
            {
                timerToNextSequence += Time.deltaTime;
            }
            else
            {
                //Start next sequence
                startedNextSequence = true;
                if (nextSequenceToStart != null)
                {
                    nextSequenceToStart.GetComponent<Sequence>().StartRunning();
                }
            }
        }


        if(object1 != null)
        {
            if(object1_pos1 != null)
            {
                if (transition1)
                {
                    if (object1.transform.localPosition.DistanceNoSqrt(object1_pos1.transform.localPosition) > 0.1)
                    {
                        object1.transform.localPosition += (object1_pos1.transform.localPosition - object1.transform.localPosition).normalized * Time.deltaTime * speed;
                    }
                    else
                    {
                        startObject2 = true;
                    }

                    if (rotateObject1)
                    {
                        object1.transform.localRotation = Quaternion.Slerp(object1.transform.localRotation, object1_pos1.transform.localRotation, speedRotationObject1 * Time.deltaTime);
                    }
                }
                else
                {
                    if (rotateObject1)
                    {
                        object1.transform.localRotation = object1_pos1.transform.localRotation;
                    }
                    object1.transform.localPosition = object1_pos1.transform.globalPosition;
                    startObject2 = true;
                }
                

            }
        }

        if(object2 != null && startObject2)
        {
            if (object2_pos1 != null)
            {
                if (transition2)
                {
                    if (object2.transform.localPosition.DistanceNoSqrt(object2_pos1.transform.localPosition) > 0.1)
                    {
                        object2.transform.localPosition += (object2_pos1.transform.localPosition - object2.transform.localPosition).normalized * Time.deltaTime * speed;
                    }
                    else
                    {
                        startObject3 = true;
                    }

                    if (rotateObject2)
                    {
                        object2.transform.localRotation = Quaternion.Slerp(object2.transform.localRotation, object2_pos1.transform.localRotation, speedRotationObject2 * Time.deltaTime);
                    }
                }
                else
                {
                    if (rotateObject2)
                    {
                        object2.transform.localRotation = object2_pos1.transform.localRotation;
                    }

                    object2.transform.localPosition = object2_pos1.transform.globalPosition;
                    startObject3 = true;
                }

            }
        }


        if(object3 != null)
        {

        }

        //if(timerToNextSequence < timeToNextSequence && isRunning)
        //{
        //    timerToNextSequence += Time.deltaTime;
        //}
        //else if(isRunning)
        //{
        //    startNextSequence?.Invoke();
        //    Debug.Log("Finish next sequence");
            
        //    nextSequence = true;
            
        //}

        //if (endSequence)
        //{
        //    if (teleportPoint1 != null)
        //    {
           
        //        gameObject.transform.localPosition = teleportPoint1.transform.globalPosition;
        //        gameObject.transform.localRotation = teleportPoint1.transform.localRotation;

        //    }
        //    return;

        //}

        //if (isRunning)
        //{


            //if (nextSequence)
            //{
            //    if (point1 != null)
            //    {
            //        gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, point1.transform.localRotation, rotationSpeed * Time.deltaTime);
            //    }
            //}

            //if (sequenceTimer < sequenceTime)
            //{
            //    sequenceTimer += Time.deltaTime;
            //    if (point0 != null)
            //    {
            //        if (gameObject.transform.localPosition.DistanceNoSqrt(point0.transform.localPosition) > 0.1f)
            //        {
            //            gameObject.transform.localPosition += (point0.transform.localPosition - gameObject.transform.localPosition).normalized * Time.deltaTime * speed;
            //        }
            //        else
            //        {
            //            onEndSequence?.Invoke();
            //        }

            //        if (rotation && !nextSequence)
            //        {
            //            gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, point0.transform.localRotation, 0.4f * Time.deltaTime);
            //        }
            //    }

            //}
            //else
            //{
            //    isRunning = false;
            //    sequenceTimer = 0;
            //    //endSequence = true;
            //}



        //}
    }

    public void StartRunning()
    {
        isRunning = true;
    }
    public void StopRunning()
    {
        isRunning = false;
    }
}


