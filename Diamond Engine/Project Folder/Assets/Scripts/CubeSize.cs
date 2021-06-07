using System;
using DiamondEngine;

public class CubeSize : DiamondComponent
{
    //Cube Movement
    public float maxHeight = 0.4f;
    public float moveSpeed = 0.5f;
    private bool goingUp = true;

    private Vector3 initialPos;

    //Cube Size
    public float maxSize = 0.2f;
    public float sizeSpeed = 0.5f;
    private Vector3 initialSize;

    //Randomness
    private float waitBeforeStart = 0.0f;
    private float waitBeforeStart_timer = 0.0f;

    private float currPercentageOfAnimation = 0.0f;

    public void Awake()
    {
        initialPos = gameObject.transform.localPosition;
        initialSize = gameObject.transform.localScale;
        SetRandomInitialValues();

    }

    public void Update()
    {
        if (waitBeforeStart_timer < waitBeforeStart)
        {
            waitBeforeStart_timer += Time.deltaTime;
            return;
        }

        if (goingUp) currPercentageOfAnimation += Time.deltaTime * moveSpeed;
        else currPercentageOfAnimation -= Time.deltaTime * moveSpeed;

        if (currPercentageOfAnimation > 1.0f) goingUp = false;
        else if (currPercentageOfAnimation < 0.0f) goingUp = true;

        VehicleMovement();
        VehicleSize();
    }

    private void VehicleMovement()
    {
        float yPos = ParametricBlend(currPercentageOfAnimation);
        Vector3 newPos = new Vector3(initialPos.x, initialPos.y, initialPos.z);
        newPos.y += yPos * maxHeight;

        gameObject.transform.localPosition = newPos;
    }

    private void VehicleSize()
    {
        float multiplier = ParametricBlend(currPercentageOfAnimation);
        Vector3 newScale = new Vector3(initialSize.x, initialSize.y, initialSize.z);
        newScale += multiplier * maxSize;

        gameObject.transform.localScale = newScale;
    }

    public float ParametricBlend(float t) => ((t * t) / (2.0f * ((t * t) - t) + 1.0f));

    private float GetRandomValue(float min, float max)
    {
        System.Random random = new System.Random();
        double val = (random.NextDouble() * (max - min) + min);
        return (float)val;
    }

    private void SetRandomInitialValues()
    {
        waitBeforeStart = GetRandomValue(0.0f, maxHeight / moveSpeed);
    }
}