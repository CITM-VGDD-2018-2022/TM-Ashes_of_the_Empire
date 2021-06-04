using System;
using DiamondEngine;

public class MoffSwing : DiamondComponent
{
    public float speed = 60.0f;
    public float maxLifeTime = 5.0f;
    public int damage = 5;

    public GameObject swingParticleObj = null;
    private ParticleSystem swingParticle = null;

    private float speedMult = 1f;
    private float timeMult = 1f;

    private float currentLifeTime = 0.0f;
    private bool start = false;

    public bool hasCollided = false;

    private Vector3 swingDirection = Vector3.zero;

    public void Awake()
    {
        if (swingParticleObj != null)
            swingParticle = swingParticleObj.GetComponent<ParticleSystem>();
    }

    public void Update()
    {
        currentLifeTime += Time.deltaTime;

        Vector3 mySwingDir = swingDirection == Vector3.zero ? gameObject.transform.GetForward() : swingDirection;

        gameObject.transform.localPosition += mySwingDir * (speed * speedMult * Time.deltaTime);

        if (currentLifeTime >= maxLifeTime * timeMult)
        {
            InternalCalls.Destroy(this.gameObject);
        }

        if (start == false)
        {
            if (swingParticle != null)
                swingParticle.Play();

            start = true;
        }

    }

    public void SetDirection(Vector3 myDir)
    {
        swingDirection = myDir;
    }

    public void SetMultipliers(float speedMult, float timeMult)
    {
        this.speedMult = speedMult;
        this.timeMult = timeMult;
    }

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }

}