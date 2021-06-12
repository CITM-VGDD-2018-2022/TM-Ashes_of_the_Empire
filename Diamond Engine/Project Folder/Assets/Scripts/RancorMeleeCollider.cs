using System;
using DiamondEngine;

public class RancorMeleeCollider : DiamondComponent
{
    public float colliderSpeed = 1.0f;
    public float lifeTime = 2.0f;

    public int damage = 10;


    public void Update()
    {
        if (lifeTime > 0.0f)
        {
            lifeTime -= Time.deltaTime;

            if (lifeTime <= 0.0f)
                InternalCalls.Destroy(gameObject);
        }

        gameObject.transform.localPosition += gameObject.transform.GetForward() * colliderSpeed * Time.deltaTime;
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Player"))
        {
            PlayerHealth health = triggeredGameObject.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);
        }
    }

}