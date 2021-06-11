using System;
using DiamondEngine;

public class Shake2D : DiamondComponent
{
    public GameObject elementShake = null; //if this is left as null, shake will take its game object as the shake element
    public float shakeXMax = 0.0f;
    public float shakeYMax = 0.0f;
    public float maxShakeTime = 0.0f;
    public float noiseScale = 10.0f;
    public float noiseLacunarity = 2.0f;
    public int noiseOctaves = 4;


    float currShakeTime = 0.0f;
    PerlinNoise2D shakeNoiseX = new PerlinNoise2D();
    PerlinNoise2D shakeNoiseY = new PerlinNoise2D();
    Vector2 initialPos = new Vector2(0.0f, 0.0f);

    float shake = 1.0f;//value between 0 and 1 that will control the amount of shake performed in the future

    bool start = true;
    public void Update()
    {
        if (start)
        {
            noiseOctaves = (int)Mathf.Clamp(noiseOctaves, 1.0f, 12.0f);
            if (noiseScale <= 0.0f)
                noiseScale = 1.0f;

            //TODO need to have Transform2D as a c# component to continue
            Transform2D trans2D = null;
            if (elementShake == null)
            {
                trans2D = gameObject.GetComponent<Transform2D>();
                if (trans2D != null)
                    elementShake = gameObject;
            }
            else
            {
                trans2D = elementShake.GetComponent<Transform2D>();
                if (trans2D == null)
                    elementShake = null;
            }

            if (elementShake != null)
            {
                initialPos = new Vector2(trans2D.lPos.x, trans2D.lPos.y);
            }

            start = false;
        }

        if (elementShake == null)
            return;

        if (currShakeTime < maxShakeTime)
        {
            PerformShake();
        }


        //Test CODE
        if (Input.GetKey(DEKeyCode.J) == KeyState.KEY_DOWN) //test key
        {
            StartShake(1.0f);
        }
        if (Input.GetKey(DEKeyCode.K) == KeyState.KEY_DOWN) //test key
        {
            StartShake(1.0f, 2);
        }

    }

    public void StartShake(float duration, int seed = int.MinValue)
    {
        maxShakeTime = Math.Abs(duration);
        currShakeTime = 0.0f;

        shakeNoiseX.GenerateNoise(seed, noiseOctaves, noiseScale, noiseLacunarity);
        shakeNoiseY.GenerateNoise(seed, noiseOctaves, noiseScale, noiseLacunarity);

    }
    public void StopShake()
    {
        maxShakeTime = -1.0f;
        currShakeTime = 0.0f;

        if (elementShake != null)
        {
            Transform2D trans2D = elementShake.GetComponent<Transform2D>();
            if (trans2D != null)
                trans2D.lPos = new Vector3(initialPos.x, initialPos.y, 0.0f);
        }
    }

    void PerformShake()
    {
        //TODO need to have Transform2D as a c# component to continue

        Transform2D trans2D = elementShake.GetComponent<Transform2D>();
        Vector2 offset = new Vector2(0.0f, 0.0f);
        offset.x = shakeXMax * shake * ((shakeNoiseX.GetNoiseAt(new Vector2(0.0f, currShakeTime)) * 2.0f) - 1.0f);
        offset.y = shakeYMax * shake * ((shakeNoiseY.GetNoiseAt(new Vector2(currShakeTime, 0.0f)) * 2.0f) - 1.0f);

        trans2D.lPos = new Vector3(initialPos.x + offset.x, initialPos.y + offset.y, 0.0f);

        currShakeTime += Time.deltaTime;

        if (currShakeTime > maxShakeTime)
            StopShake();
    }

}