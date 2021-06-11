using System;
using DiamondEngine;

public class HeavyTrooperParticles : DiamondComponent
{
    public GameObject sweepParticlesObj = null;
    public GameObject dashParticlesObj = null;
    public GameObject hitParticlesObj = null;
    public GameObject sniperHitParticleObj = null;
    public GameObject grenadeHitParticleObj = null;
    private ParticleSystem sweepParticles = null;
    private ParticleSystem dashParticles = null;
    private ParticleSystem hitParticles = null;
    private ParticleSystem sniperHitParticle = null;
    private ParticleSystem grenadeHitParticle = null;

    public enum HEAVYROOPER_PARTICLES : int
    {
        SPEAR,
        DASH,
        HIT,
        SNIPER_HIT,
        GRENADE_HIT
    }

    public void Awake()
    {
        if (sweepParticlesObj != null)
        {
            sweepParticles = sweepParticlesObj.GetComponent<ParticleSystem>();
        }
        if (dashParticlesObj != null)
        {
            dashParticles = dashParticlesObj.GetComponent<ParticleSystem>();
        }
        if (hitParticlesObj != null)
        {
            hitParticles = hitParticlesObj.GetComponent<ParticleSystem>();
        }       
        if (sniperHitParticleObj != null)
        {
            sniperHitParticle = sniperHitParticleObj.GetComponent<ParticleSystem>();
        }
        if (grenadeHitParticleObj != null)
        {
            grenadeHitParticle = grenadeHitParticleObj.GetComponent<ParticleSystem>();
        }
    }

    public void Play(HEAVYROOPER_PARTICLES particleType)
    {
        switch (particleType)
        {
            case HEAVYROOPER_PARTICLES.SPEAR:
                if (sweepParticles != null)
                    sweepParticles.Play();
                break;
            case HEAVYROOPER_PARTICLES.DASH:
                if (dashParticles != null)
                    dashParticles.Play();
                break;
            case HEAVYROOPER_PARTICLES.HIT:
                if (hitParticles != null)
                    hitParticles.Play();
                break;
            case HEAVYROOPER_PARTICLES.SNIPER_HIT:
                if (sniperHitParticle != null)
                    sniperHitParticle.Play();
                break;
            case HEAVYROOPER_PARTICLES.GRENADE_HIT:
                if (grenadeHitParticle != null)
                    grenadeHitParticle.Play();
                break;
        }
    }

    public void Stop(HEAVYROOPER_PARTICLES particleType)
    {
        switch (particleType)
        {
            case HEAVYROOPER_PARTICLES.SPEAR:
                if (sweepParticles != null)
                    sweepParticles.Stop();
                break;
            case HEAVYROOPER_PARTICLES.DASH:
                if (dashParticles != null)
                    dashParticles.Stop();
                break;
            case HEAVYROOPER_PARTICLES.HIT:
                if (hitParticles != null)
                    hitParticles.Stop();
                break;
            case HEAVYROOPER_PARTICLES.SNIPER_HIT:
                if (sniperHitParticle != null)
                    sniperHitParticle.Stop();
                break;
            case HEAVYROOPER_PARTICLES.GRENADE_HIT:
                if (grenadeHitParticle != null)
                    grenadeHitParticle.Stop();
                break;
        }
    }
}
