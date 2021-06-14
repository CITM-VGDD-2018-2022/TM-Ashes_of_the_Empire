using System;
using DiamondEngine;

public class Sequence : DiamondComponent
{
    public Action onStartSequence;
    public Action onEndSequence;
    public Sequence numSequence;
    public float sequenceTime;
    //public GameObject point0;
    //public GameObject point1;
    //public GameObject teleportPoint1;
    private bool isRunning = false;
    private float sequenceTimer;
    //private float timerToNextSequence;
    //public float timeToNextSequence;
    public bool endSequence = false;
    public bool endFinalSequence = false;

    //Game Objects of the sequence
    public GameObject object1;
    public GameObject object2;
    public GameObject object3;

    //Positions of the objects
    public GameObject object1_pos1;
    public bool transition1;
    public bool lerp1;
    private float lerpTimer1;
    public GameObject object1_pos2;
    public float speedRotationObject1;
    public float speedObject1;

    public GameObject object2_pos1;
    public bool transition2;
    public bool lerp2;
    private float lerpTimer2;

    public GameObject object2_pos2;
    public float speedRotationObject2;
    public float speedObject2;

    public GameObject object3_pos1;
    public bool transition3;
    public bool lerp3;
    public GameObject object3_pos2;
    public float speedRotationObject3;
    public float speedObject3;

    public bool rotateObject1 = false;
    public bool rotateObject2 = false;
    public bool rotateObject3 = false;

    public GameObject nextSequenceToStart;
    private bool startedNextSequence = false;
    //Timers to next sequence
    private float timerToNextSequence;
    public float timeToNextSequence;

    public bool startObject1 = false;
    public bool startObject2 = false;
    private bool startObject3 = false;

    public bool stopAllSequences = false;
    public bool keepObject1 = true;
    private float interpolateAmount;
    public float interpolateAmountSpeed;
    public bool useQuadraticInterpolation = false;
    public GameObject pointA;
    public GameObject pointB;
    public GameObject pointC;
    public bool useEaseIn = false;

    public bool activateAnimation;
    public GameObject greef;
    public void Awake()
    {
        startObject1 = true;
        lerpTimer1 = 0f;
        lerpTimer2 = 0f;
    }
    public void RunSequence()
    {
        if(!isRunning)
        {
            return;
        }



        float newDeltaTime = Time.deltaTime;

        if (!startedNextSequence)
        {
            if (timerToNextSequence < timeToNextSequence)
            {
                timerToNextSequence += newDeltaTime;
            }
            else
            {
                //Start next sequence
                startedNextSequence = true;
                if (nextSequenceToStart != null)
                {
                    if (stopAllSequences)
                    {
                        CinematicManager.instance.StopAllSequences();
                    }
                    nextSequenceToStart.GetComponent<Sequence>().StartRunning();
                    nextSequenceToStart.GetComponent<Sequence>().onStartSequence?.Invoke();
                }
            }
        }


        if (useQuadraticInterpolation)
        {
            interpolateAmount += newDeltaTime * interpolateAmountSpeed;

            if (interpolateAmount < 1)
            {
                if (useEaseIn)
                {
                    object1.transform.localPosition = QuadraticLerp(pointA.transform.globalPosition, pointB.transform.globalPosition, pointC.transform.globalPosition, Mathf.EaseInOutCubic(interpolateAmount));

                    object1.transform.localRotation = QuadraticSlerp(pointA.transform.localRotation, pointB.transform.localRotation, pointC.transform.localRotation, Mathf.EaseInOutCubic(interpolateAmount));
                }
                else
                {
                    object1.transform.localPosition = QuadraticLerp(pointA.transform.globalPosition, pointB.transform.globalPosition, pointC.transform.globalPosition, interpolateAmount);

                    object1.transform.localRotation = QuadraticSlerp(pointA.transform.localRotation, pointB.transform.localRotation, pointC.transform.localRotation, interpolateAmount);
                }

                if (interpolateAmount > 0.7f)
                {
                    if (endSequence)
                    {
                        CinematicManager.instance.EndFirstSequences();
                    }
                }

            }

            else if (endFinalSequence)
            {
                CinematicManager.instance.EndCinematic();
            }
            else if(activateAnimation && greef != null)
            {
                Animator.Play(greef, "Greef_Greet");
                activateAnimation = false;
                Audio.PlayAudio(CinematicManager.instance.gameObject, "Play_Greef_Karga_Greeting");
            }



            return;
        }

        if(object1 != null && startObject1)
        {
            if(object1_pos1 != null)
            {
                if (transition1)
                {
                    if (lerp1) 
                    {
                        if (lerpTimer1 < 1)
                        {
                            lerpTimer1 += newDeltaTime * 0.5f;
                            object1.transform.localPosition = Vector3.Lerp(object1_pos1.transform.localPosition, object1_pos2.transform.localPosition, lerpTimer1);
                            if (rotateObject1)
                            {
                                object1.transform.localRotation = Quaternion.Slerp(object1.transform.localRotation, object1_pos2.transform.localRotation, speedRotationObject1 * newDeltaTime);
                            }
                        }
                    }
                    else
                    {
                        if (object1.transform.localPosition.DistanceNoSqrt(object1_pos1.transform.localPosition) > 0.1)
                        {
                            object1.transform.localPosition += (object1_pos1.transform.localPosition - object1.transform.localPosition).normalized * newDeltaTime * speedObject1;
                        }
                        else
                        {
                            startObject2 = true;
                            if (!keepObject1)
                            {
                                startObject1 = false;
                            }
                        }

                        if (rotateObject1)
                        {
                            object1.transform.localRotation = Quaternion.Slerp(object1.transform.localRotation, object1_pos1.transform.localRotation, speedRotationObject1 * newDeltaTime);
                        }
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
                    if (!keepObject1)
                    {
                        startObject1 = false;
                    }
                }
                

            }
        }

        if(object2 != null && startObject2)
        {
            if (object2_pos1 != null)
            {
                if (transition2)
                {
                    if (lerp2)
                    {
                        if (lerpTimer2 < 1)
                        {
                            lerpTimer2 += newDeltaTime * 0.5f;
                            object2.transform.localPosition = Vector3.Lerp(object2_pos1.transform.localPosition, object2_pos2.transform.localPosition, Mathf.EaseOutCubic(lerpTimer2));
                        }
                    }
                    else
                    {
                        if (object2.transform.localPosition.DistanceNoSqrt(object2_pos1.transform.localPosition) > 0.1)
                        {
                            object2.transform.localPosition += (object2_pos1.transform.localPosition - object2.transform.localPosition).normalized * newDeltaTime * speedObject2;
                        }
                        else
                        {
                            startObject3 = true;
                        }

                        if (rotateObject2)
                        {
                            object2.transform.localRotation = Quaternion.Slerp(object2.transform.localRotation, object2_pos1.transform.localRotation, speedRotationObject2 * newDeltaTime);
                        }
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

    }

    public void StartRunning()
    {
        isRunning = true;
    }
    public void StopRunning()
    {
        isRunning = false;
    }

    private Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);

        return Vector3.Lerp(ab, bc, t);
    }

    private Quaternion QuadraticSlerp(Quaternion a, Quaternion b, Quaternion c, float t)
    {
        Quaternion ab = Quaternion.Slerp(a, b, t);
        Quaternion bc = Quaternion.Slerp(b, c, t);

        return Quaternion.Slerp(ab, bc, t);
    }

}


