using System;
using DiamondEngine;

public class DPSDamage : DiamondComponent
{
    public int baseDamage = 4;
    public int damage = 0;

    public float damageTime = 1.0f;
    public float damageTimer = 0.0f;

    PlayerHealth playerHealth = null;

    public void Awake()
    {
        damage = baseDamage;
        damageTimer = 0.0f;
    }

    public void Update()
    {
        if (damageTimer > 0.0f)
        {
            damageTimer -= Time.deltaTime;

            if(damageTimer < 0.0f)
            {
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage, true);
                    IncrementDamage();
                    damageTimer = damageTime;
                    //Debug.Log("Water Damage");
                }
            }
        }
    }


    public void OnTriggerEnter(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            if(playerHealth == null)
            {
                playerHealth = other.GetComponent<PlayerHealth>();
            }

            damageTimer = damageTime;
        }
    }

    public void OnTriggerExit(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            damageTimer = 0.0f;
            damage = baseDamage;
        }
    }

    private void IncrementDamage()
    {
        if (damage < 10)
        {
            damage += 2;
            if (damage > 10)
            {
                damage = 10;
            }
        }
    }
}