using System;
using DiamondEngine;

public class UI_Fade : DiamondComponent
{
    public float delay = 0.5f;
    public float duration = 1.0f;

    public bool playOnAwake = true;

    private float timer = 0.0f;

    private Image2D img = null;
    private bool firstFramePassed = false;  //first frame when charging a scene has a large dt, so we ignore it
    private bool started = false;
    private bool ended = false;

    public void Awake()
    {
        img = gameObject.GetComponent<Image2D>();

        if (img == null)
        {
            ended = true;
            Debug.Log("No Image was found, Fade cannot be performed");
        }
        else
        {
            img.SetFadeValue(0.0f);
        }

        if (playOnAwake == false)
        {
            ended = true;
        }
    }

    public void Update()
    {
        if (firstFramePassed == true && ended == false && img != null)
        {

            timer += Time.deltaTime;

            if (started == false && timer >= delay)
            {
                started = true;
                timer = 0.0f;
            }
            else if (started == true && timer < duration)
            {
                float value = Mathf.Lerp(0.0f, 1.0f, timer / duration);
                img.SetFadeValue(value);
            }

            else if (started == true && timer >= duration)
                ended = true;
        }

        else
            firstFramePassed = true;
    }

    public void Activate()
    {
        if (img != null)
        {
            ended = false;
            started = false;
            timer = 0.0f;

            img.SetFadeValue(0.0f);
        }
    }
}