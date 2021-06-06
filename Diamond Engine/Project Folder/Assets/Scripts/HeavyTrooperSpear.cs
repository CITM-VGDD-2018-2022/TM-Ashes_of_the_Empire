using System;
using DiamondEngine;

public class HeavyTrooperSpear : DiamondComponent
{
    public int damage = 0;

    BoxCollider collider = null;

    public void Awake()
    {
        collider = gameObject.GetComponent<BoxCollider>();

        if (collider != null)
            collider.active = false;
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if(triggeredGameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = Core.instance.gameObject.GetComponent<PlayerHealth>();

            if(playerHealth != null  && collider != null && collider.active)
            {
                //Debug.Log("Player Hit with damage: " + damage);
                playerHealth.TakeDamage(damage);
                if (gameObject.CompareTag("Saber"))
                {
                    Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Lightsaber_Attack");
                }
            }
        }
    }

}