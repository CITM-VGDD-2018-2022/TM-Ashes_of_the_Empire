using System;
using DiamondEngine;

public class Geyser : DiamondComponent
{
    float timer = 0.0f;
    public float timeBetweenEruptions = 10.0f;

    public int damage = 5;
    private bool playerInRange = false;
    private bool canHurtPlayer = false;

    public GameObject particleSystemObject = null;
    private ParticleSystem eruptionParticles = null;
    private bool particlesWerePlaying = false;

    public void Awake()
    {
        timer = timeBetweenEruptions;

        if (particleSystemObject != null)
        {
            eruptionParticles = particleSystemObject.GetComponent<ParticleSystem>();
        }
    }

    public void Update()
    {
        if (eruptionParticles != null)
        {
            if (particlesWerePlaying)
            {
                if (eruptionParticles.playing)
                {
                    if (canHurtPlayer && playerInRange && Core.instance != null)
                    {
                        HurtPlayer();
                    }
                }
                else
                {
                    timer = timeBetweenEruptions;
                    particlesWerePlaying = false;
                }
            }
        }

        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;

            if (timer <= 0.0f)
            {
                Erupt();
            }
        }
    }

    private void Erupt()
    {
        if (eruptionParticles != null)
        {
            eruptionParticles.Play();
            particlesWerePlaying = true;
            canHurtPlayer = true;
        }

        timer = timeBetweenEruptions;
    }

    private void HurtPlayer()
    {
        PlayerHealth playerHealth = Core.instance.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        canHurtPlayer = false;
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    public void OnTriggerExit(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}