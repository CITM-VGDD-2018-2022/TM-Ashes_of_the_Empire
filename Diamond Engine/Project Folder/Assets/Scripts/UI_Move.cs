using System;
using DiamondEngine;

public class UI_Move : DiamondComponent
{
    public float delay = 0.5f;
    public float duration = 1.0f;

    public float initialPosX = 0.0f;
    public float initialPosY = 0.0f;
    public float finalPosX = 0.0f;
    public float finalPosY = 0.0f;

    public float initialScaleX = 0.0f;
    public float initialScaleY = 0.0f;
    public float finalScaleX = 0.0f;
    public float finalScaleY = 0.0f;

    public bool playOnAwake = true;

    enum INTERPOLATION_TYPE
    {
        LINEAR = 0,
        EASE_IN = 1,
        EASE_OUT_QUAD = 2,
        EASE_IN_CUBIC = 3,
        EASE_OUT_CUBIC = 4,
        EASE_IN_QUART = 5,
        EASE_IN_QUINT = 6,
        EASE_OUT_QUINT = 7
    }

    public int interpolationType = 0;

    private float timer = 0.0f;

    private Transform2D trans = null;
    private bool firstFramePassed = false;  //first frame when charging a scene has a large dt, so we ignore it
    private bool started = false;
    private bool ended = false;

    public void Awake()
    {
        trans = gameObject.GetComponent<Transform2D>();

        if (trans == null)
        {
            Debug.Log("Need to add transform2D to gameObject: " + gameObject.Name);
            ended = true;
        }
        else
        {
            trans.SetLocalTransform(new Vector3(initialPosX, initialPosY, 1.0f), trans.lRot, new Vector3(initialScaleX, initialScaleY, 1.0f));
        }

        if (playOnAwake == false)
            ended = true;
    }

    public void Update()
    {
        if (firstFramePassed == true && ended == false && trans!=null)
        {

            timer += Time.deltaTime;

            if (started == false && timer >= delay)
            {
                started = true;
                timer = 0.0f;
            }
            else if (started == true && timer <= duration)
            {
                UpdateValues();
            }

            else if (started == true && timer > duration)
                ended = true;
        }

        else
            firstFramePassed = true;
    }

    public void Activate()
    {
        if (trans != null)
        {
            ended = false;
            started = false;
            timer = 0.0f;

            trans.SetLocalTransform(new Vector3(initialPosX, initialPosY, 1.0f), trans.lRot, new Vector3(initialScaleX, initialScaleY, 1.0f));
        }
    }


    private void UpdateValues()
    {
        float interpolationValue = timer / duration;

        switch ((INTERPOLATION_TYPE)interpolationType)
        {
            case INTERPOLATION_TYPE.EASE_IN:
                interpolationValue = Mathf.EaseInSine(interpolationValue);
                break;

            case INTERPOLATION_TYPE.EASE_OUT_QUAD:
                interpolationValue = Mathf.EaseOutQuad(interpolationValue);
                break;

            case INTERPOLATION_TYPE.EASE_IN_CUBIC:
                interpolationValue = Mathf.EaseInCubic(interpolationValue);
                break;

            case INTERPOLATION_TYPE.EASE_OUT_CUBIC:
                interpolationValue = Mathf.EaseOutCubic(interpolationValue);
                break;

            case INTERPOLATION_TYPE.EASE_IN_QUART:
                interpolationValue = Mathf.EaseInQuart(interpolationValue);
                break;

            case INTERPOLATION_TYPE.EASE_IN_QUINT:
                interpolationValue = Mathf.EaseInQuint(interpolationValue);
                break;

            case INTERPOLATION_TYPE.EASE_OUT_QUINT:
                interpolationValue = Mathf.EaseOutQuint(interpolationValue);
                break;

            default:
                break;
        }

        float xPosition = Mathf.Lerp(initialPosX, finalPosX, interpolationValue);
        float yPosition = Mathf.Lerp(initialPosY, finalPosY, interpolationValue);

        float xScale = Mathf.Lerp(initialScaleX, finalScaleX, interpolationValue);
        float yScale = Mathf.Lerp(initialScaleY, finalScaleY, interpolationValue);

        trans.SetLocalTransform(new Vector3(xPosition, yPosition, 1.0f), trans.lRot, new Vector3(xScale, yScale, 1.0f));
    }

}