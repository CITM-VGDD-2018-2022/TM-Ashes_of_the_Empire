using System;
using DiamondEngine;

public class TutoButtonLoop : DiamondComponent
{
    public GameObject releasedButton = null;
    public GameObject pressedButton = null;
    public GameObject radiantButton = null;

    private float maxTime = 0.35f;
    private float currentTime = 0.0f;
    private int loopState = 0;

    public void Awake()
    {
        if (pressedButton == null || releasedButton == null || radiantButton == null)
            return;
        pressedButton.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        releasedButton.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        radiantButton.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

    }
    public void Update()
    {
        //buttonMaterial=this.gameObject.GetComponent<Material>();
        if (releasedButton == null || pressedButton == null || radiantButton == null)
        {
            Debug.Log("Missing tutorial button reference");
        }

        else
        {
            currentTime += Time.deltaTime;
            if (currentTime >= maxTime)
            {
                switch (loopState)
                {
                    case 0:
                        pressedButton.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        //pressedButton.transform.localRotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
                        //pressedButton.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

                        releasedButton.transform.localPosition = new Vector3(10000.0f, 0.0f, 0.0f);
                        //releasedButton.transform.localPosition = new Vector3(10000.0f, 0.0f, 0.0f);
                        //releasedButton.transform.localPosition = new Vector3(10000.0f, 0.0f, 0.0f);
                        loopState = 1;
                        break;
                    case 1:
                        radiantButton.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        pressedButton.transform.localPosition = new Vector3(10000.0f, 0.0f, 0.0f);
                        loopState = 2;
                        break;
                    case 2:
                        releasedButton.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                        radiantButton.transform.localPosition = new Vector3(10000.0f, 0.0f, 0.0f);
                        loopState = 0;
                        break;
                }
                currentTime = 0.0f;
            }
        }
    }

}