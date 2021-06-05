using System;
using DiamondEngine;

public class Geyser : DiamondComponent
{
    public int damage = 5;
    private bool playerInRange = false;
    private bool canHurtPlayer = false;
    private bool opened = false;
    private bool closed = false;

    //timers
    private float eruptionTimer = 0.0f;
    private float betweenEruptionsTimer = 0.0f;
    private float regressionTimer = 0.0f;

    //times
    public float eruptionTime = 2.47f;
    public float timeBetweenEruptions = 10.0f;
    public float regressionTime = 2.0f;

    //Particles
    public GameObject idleParticlesObject = null;
    private ParticleSystem idleParticles = null;

    public GameObject eruptionParticlesObject = null;
    private ParticleSystem eruptionParticles = null;

    private GameObject doorTrap = null;

    public void Awake()
    {
        betweenEruptionsTimer = timeBetweenEruptions;

        if(idleParticlesObject != null)
        {
            idleParticles = idleParticlesObject.GetComponent<ParticleSystem>();

            if(idleParticles != null)
            {
                idleParticles.Play();
            }
        }

        if (eruptionParticlesObject != null) 
        {
            eruptionParticles = eruptionParticlesObject.GetComponent<ParticleSystem>();
        }

        doorTrap = gameObject.GetChild("DoorTrap");
        if (doorTrap == null)
            Debug.Log("DoorTrap is null");
    }

    public void Update()
    {
        {
            //if (eruptionParticles != null)
            //{
            //    if (particlesWerePlaying)
            //    {
            //        if (eruptionParticles.playing)
            //        {
            //            if (canHurtPlayer && playerInRange && Core.instance != null)
            //            {
            //                HurtPlayer();
            //            }
            //        }
            //        else
            //        {
            //            timer = timeBetweenEruptions;
            //            particlesWerePlaying = false;
            //        }
            //    }
            //}

            //if (timer > 0.0f)
            //{
            //    timer -= Time.deltaTime;

            //    if (timer <= 0.0f)
            //    {
            //        Erupt();
            //    }
            //}
        }

        if(betweenEruptionsTimer > 0.0f)
        {
            betweenEruptionsTimer -= Time.deltaTime;

            if(betweenEruptionsTimer <= 0.0f)
            {
                //leave some time between idle and eruption so eruption is anticipated and more spectacular
                regressionTimer = regressionTime;
            }
        }

        if(regressionTimer > 0.0f)
        {
            regressionTimer -= Time.deltaTime;

            if (regressionTimer <= 0.3f && !opened)
            {
                if (doorTrap != null)
                {
                    Animator.Play(doorTrap, "DoorTrap_OpenDoor");
                }
                opened = true;
                closed = false;
            }

            if (regressionTimer <= 0.0f)
            {
                Erupt();
            }
        }

        if (eruptionTimer > 0.0f)
        {
            eruptionTimer -= Time.deltaTime;

            if(eruptionTimer > eruptionTime * 0.5f && playerInRange && canHurtPlayer)
            {
                HurtPlayer();
            }

            if (eruptionTimer <= 0.5f && !closed)
            {
                if (doorTrap != null)
                {
                    Animator.Play(doorTrap, "DoorTrap_CloseDoor");
                }
                closed = true;
            }

            if (eruptionTimer <= 0.0f)
            {
                if(idleParticles != null)
                {
                    idleParticles.Play();
                    betweenEruptionsTimer = timeBetweenEruptions;
                    opened = false;
                }
            }
        }
    }

    private void Erupt()
    {
        if (eruptionParticles != null)
        {
            eruptionParticles.Play();
        }

        Audio.PlayAudio(gameObject, "Play_Geyser");
        eruptionTimer = eruptionTime;
        canHurtPlayer = true;
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
        //Debug.Log("Player In Range");
        if (triggeredGameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    public void OnTriggerExit(GameObject triggeredGameObject)
    {
        //Debug.Log("Player Out of Range");
        if (triggeredGameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}