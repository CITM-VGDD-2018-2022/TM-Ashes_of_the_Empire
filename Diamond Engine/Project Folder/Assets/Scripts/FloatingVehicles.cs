using System;
using DiamondEngine;

public class FloatingVehicles : DiamondComponent
{
    //Vehicle Movement
	private float maxHeight = 1.0f;
	private float verticalSpeed = 0.5f;
	private bool goingUp = true;
	private float currPercentageOfAnimation = 0.0f;    
    private Vector3 initialPos;    

    //Vehicle Rotation
    private float currRotation = 0.0f;
    private float maxRotation = 0.15f;

    //Randomness
    private float waitBeforeStart = 0.0f;
    private float waitBeforeStart_timer = 0.0f;

    public void Awake()
    {
        initialPos = gameObject.transform.localPosition;
        SetVehicleProperties();
        SetRandomInitialValues();
    }

    public void Update()
    {
        if (waitBeforeStart_timer < waitBeforeStart)
        {
            waitBeforeStart_timer += Time.deltaTime;
            return;
        }            

        if (goingUp) currPercentageOfAnimation += Time.deltaTime * verticalSpeed;        
        else currPercentageOfAnimation -= Time.deltaTime * verticalSpeed;

        if (currPercentageOfAnimation > 1.0f) goingUp = false;
        else if (currPercentageOfAnimation < 0.0f) goingUp = true;

        VehicleRotation();
        VehicleMovement();        
    }

    public float ParametricBlend(float t) => ((t * t) / (2.0f * ((t * t) - t) + 1.0f));

    private float GetRandomValue(float min, float max)
    {
        System.Random random = new System.Random();
        double val = (random.NextDouble() * (max - min) + min);
        return (float)val;
    }

    private void VehicleRotation()
    {
        if(currPercentageOfAnimation <= 0.5f)
        {
            currRotation = currPercentageOfAnimation * 2.0f * maxRotation;
        }
        else
        {
            float localPercentage = (currPercentageOfAnimation - 0.5f) * 2.0f;
            currRotation = -1.0f * (1.0f - localPercentage) * maxRotation;
        }

        gameObject.transform.localRotation *= Quaternion.RotateAroundAxis(Vector3.right, currRotation * Mathf.Deg2RRad);
    }

    private void VehicleMovement()
    {
        float yPos = ParametricBlend(currPercentageOfAnimation);
        Vector3 newPos = new Vector3(initialPos.x, initialPos.y, initialPos.z);
        newPos.y += yPos * maxHeight;

        gameObject.transform.localPosition = newPos;
    }

    private void SetVehicleProperties()
    {
        switch (gameObject.Name)
        {
            case "LandSpeeder": 
                maxHeight = 2.0f;
                verticalSpeed = 0.3f;
                maxRotation = 0.09f;
                break;
            case "SpeederBike": 
                maxHeight = 0.8f;
                verticalSpeed = 0.5f;
                maxRotation = 0.15f;
                break;
            case "SnowSpeeder": 
                maxHeight = 1.5f;
                verticalSpeed = 0.15f;
                maxRotation = 0.05f;
                break;
            case "XWing":
                maxHeight = 3.0f;
                verticalSpeed = 0.25f;
                maxRotation = 0.07f;
                break;
            case "XWingRepairing": //Lvl 2 - Room 3
                maxHeight = 1.2f;
                verticalSpeed = 0.2f;
                maxRotation = 0.05f;
                break;
            case "Tie_Fighter":
                maxHeight = 0.4f;
                verticalSpeed = 0.5f;
                maxRotation = 0.0f;
                break;
            case "Tie_In":
                maxHeight = 0.8f;
                verticalSpeed = 0.5f;
                maxRotation = 0.0f;
                break;
            default:
                break;            
        }        
    }

    private void SetRandomInitialValues()
    {        
        waitBeforeStart = GetRandomValue(0.0f, maxHeight / verticalSpeed);
    }
}