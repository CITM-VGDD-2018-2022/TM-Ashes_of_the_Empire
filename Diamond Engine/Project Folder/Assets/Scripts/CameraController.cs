using System;
using DiamondEngine;

public class CameraController : DiamondComponent
{
    public GameObject target = null;

    public float x = 0.0f;
    public float y = 0.0f;
    public float z = 0.0f;
    public float smoothSpeed = 0.1f;
    private float zoomDesired = 0.1f;

    //ZOOM Variables
    private bool zooming = false;
    public float timer_easing_sec = 0.2f;
    public float timer = 0.0f;
    private float pointA_zoom;
    public bool waitPlayerZoom = false;

    public float wait_Time = 1.0f;
    public float cornerTopLeftX = -100.0f;
    public float cornerTopLeftY = -100.0f;
    public float cornerBotRightX = 100.0f;
    public float cornerBotRightY = 100.0f;

    //Die Zoom variables
    public float deadZoom = 20f;
    public float deadEasingSec = 0.5f;

    private float wait_timer = 0.0f;

    public bool startFollow = false;
    public void Awake()
    {
        //PlayerHealth.onPlayerDeath += DeadZoom;
        //onZoom += Zoom;
        PlayerHealth.onPlayerDeath += DeadZoom;
        pointA_zoom = CameraManager.GetOrthSize(this.gameObject);
        wait_timer = 0.0f;
    }

    public void Update()
    {
        if (!startFollow)
        {
            return;
        }

        if (waitPlayerZoom && target != null)
        {
            if (wait_timer < wait_Time) wait_timer += Time.deltaTime;
            else
            {
                wait_timer = 0.0f;
                waitPlayerZoom = false;
                target = null;
            }
        }

        if (target == null && Core.instance != null)        
            target = Core.instance.gameObject;

        Vector3 offset = new Vector3(x, y, z);
        Vector3 desiredPosition = new Vector3(0.0f, 0.0f, 0.0f);

        if (target != null)
            desiredPosition = target.transform.localPosition + offset;

        Vector3 smoothPosition = Vector3.Lerp(gameObject.transform.localPosition, desiredPosition, 1.0f - (float)Math.Pow(smoothSpeed, Time.deltaTime));
        Vector3 finalPos = new Vector3(Mathf.Clamp(smoothPosition.x, cornerTopLeftX, cornerBotRightX), smoothPosition.y, Mathf.Clamp(smoothPosition.z, cornerTopLeftY, cornerBotRightY));
        gameObject.transform.localPosition = finalPos;

        //ZOOM ALGORITHM 

        if (zooming && timer_easing_sec>=0.0f)
        {
            timer += Time.deltaTime;
            float t = 0;

            t = timer / timer_easing_sec;

            if (timer >= timer_easing_sec)
            {
                timer = 0;
                zooming = false;
                return;
            }
            CameraManager.SetOrthSize(this.gameObject, Ease.PointLerp(pointA_zoom, zoomDesired, Ease.OutCubic(t)));
        }
    }

    public void Zoom(float size, float timeInSec)
    {
        zooming = true;
        pointA_zoom = CameraManager.GetOrthSize(this.gameObject);
        timer = 0;
        zoomDesired = size;
        timer_easing_sec = timeInSec;
    }

    public void DeadZoom()
    {
        if(zooming == false)
        {
            timer = 0;
        }
        if (deadZoom == 0 || deadEasingSec == 0)
            return; 

        zooming = true;

        zoomDesired = deadZoom;
        timer_easing_sec = deadEasingSec;
    }
}
