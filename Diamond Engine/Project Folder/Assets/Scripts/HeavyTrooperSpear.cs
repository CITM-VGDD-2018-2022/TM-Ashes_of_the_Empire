using System;
using DiamondEngine;

public class HeavyTrooperSpear : DiamondComponent
{
    public int damage = 0;

    BoxCollider collider = null;

    public bool canDamage { get; private set; } = false;

    private bool alreadyDamaged = false;

    public void Awake()
    {
        collider = gameObject.GetComponent<BoxCollider>();

        if (collider != null)
        {
            canDamage = false;
        }
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Player") && alreadyDamaged == false)
        {
            PlayerHealth playerHealth = Core.instance.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null && collider != null && canDamage == true)
            {
                playerHealth.TakeDamage(damage);
                alreadyDamaged = true;
                if (gameObject.CompareTag("Saber"))
                {
                    Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Lightsaber_Attack");
                }
            }
        }
    }

    public void OnTriggerExit(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Player") && alreadyDamaged == false)
        {
            PlayerHealth playerHealth = Core.instance.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null && collider != null && canDamage == true)
            {
                playerHealth.TakeDamage(damage);
                alreadyDamaged = true;
                if (gameObject.CompareTag("Saber"))
                {
                    Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Lightsaber_Attack");
                }
            }
        }
    }


    public void SetCanDamage(bool state)
    {
        canDamage = state;

        if(state == false)
        {
            alreadyDamaged = false;
        }
    }


}