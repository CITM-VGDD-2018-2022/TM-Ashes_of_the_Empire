using System;
using DiamondEngine;

public class RancorProjectile : DiamondComponent
{
	public float speed = 30.0f;
	public float lifeTime = 20.0f;
	public int damage = 10;

	public Vector3 targetPos = new Vector3(0, 0, 0);    //Set from Rancor.cs
	private Vector3 targetDirection = Vector3.zero;

	private bool to_destroy = false;
	private float timer = 1.3f;
	public void Update()
	{
		if (!to_destroy)
		{
			gameObject.transform.localPosition += gameObject.transform.GetForward().normalized * speed * Time.deltaTime;
		}

		if (timer > 0f && !to_destroy)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				to_destroy = true;
				gameObject.GetChild("RancorProjectile").GetComponent<MeshRenderer>().active = false;
				timer = 1f;
			}
		}


		if (to_destroy)
		{
			if (timer > 0f && to_destroy)
			{
				timer -= Time.deltaTime;
				if (timer <= 0f)
				{
					InternalCalls.Destroy(gameObject);
				}
			}
		}
	}

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Player") && !to_destroy)
        {

			PlayerHealth health = triggeredGameObject.GetComponent<PlayerHealth>();
            if (health != null)
				health.TakeDamage(damage);
			to_destroy = true;
			gameObject.GetChild("RancorProjectile").GetComponent<MeshRenderer>().active = false;
			timer = 1.3f;
		}
    }
	public void OnCollisionEnter(GameObject collidedGameObject)
	{
		Audio.PlayAudio(gameObject, "Play_Rock_Impact");
		to_destroy = true;
		gameObject.GetChild("RancorProjectile").GetComponent<MeshRenderer>().active = false;
		timer = 1.3f;
	}
}