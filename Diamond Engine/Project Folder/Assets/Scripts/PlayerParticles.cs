using System;
using DiamondEngine;

public class PlayerParticles : DiamondComponent
{
	public GameObject dustObj = null;
	public GameObject iceObj = null;
	public GameObject waterObj = null;
	public GameObject muzzleObj = null;
	public GameObject jetpackObj = null;
	public GameObject impactObj = null;
    public GameObject grenade_not = null;
    public GameObject sniper_not = null;
    public GameObject healObj = null;
    public GameObject sniperMuzzelObj = null;
    public GameObject sniperChargeObj = null;

	public ParticleSystem dust = null;
	public ParticleSystem ice = null;
	public ParticleSystem water = null;
	public ParticleSystem muzzle = null;
	public ParticleSystem jetpack = null;
	public ParticleSystem impact = null;
    public ParticleSystem grenade = null;
    public ParticleSystem sniper = null;
    public ParticleSystem heal = null;
    public ParticleSystem sniperMuzzel = null;
    public ParticleSystem sniperCharge = null;

    public void Awake()
    {
        if (dustObj != null)
        {
            dust = dustObj.GetComponent<ParticleSystem>();
        }
        if(iceObj != null)
        {
            ice = iceObj.GetComponent<ParticleSystem>();
        }
        if (waterObj != null)
        {
            water = waterObj.GetComponent<ParticleSystem>();
        }
        if (muzzleObj != null)
        {
            muzzle = muzzleObj.GetComponent<ParticleSystem>();
        }
        if (jetpackObj != null)
        {
            jetpack = jetpackObj.GetComponent<ParticleSystem>();
        }

        if (impactObj != null)
        {
            impact = impactObj.GetComponent<ParticleSystem>();
        }
        
        if (grenade_not != null)
        {
            grenade = grenade_not.GetComponent<ParticleSystem>();
        }
        if (sniper_not != null)
        {
            sniper = sniper_not.GetComponent<ParticleSystem>();
        }
        if(healObj != null)
        {
            heal = healObj.GetComponent<ParticleSystem>();
        }
        if (sniperMuzzelObj != null)
        {
            sniperMuzzel = sniperMuzzelObj.GetComponent<ParticleSystem>();
        }
        if (sniperChargeObj !=  null)
        {
            sniperCharge = sniperChargeObj.GetComponent<ParticleSystem>();
        }
    }
    public void Update()
	{

	}

}