using System;
using DiamondEngine;

public class RancorProjectile : DiamondComponent
{
	public float speed = 30.0f;
	public float lifeTime = 20.0f;
	public int damage = 10;

	public GameObject destroyParticlesObj = null;
	private ParticleSystem destroyParticles = null;

	public Vector3 targetPos = new Vector3(0, 0, 0);    //Set from Rancor.cs

	private bool toDestroy = false;
	private float timer = 1.3f;
	
	public void Awake()
    {
		if(destroyParticlesObj != null)
        {
			destroyParticles = destroyParticlesObj.GetComponent<ParticleSystem>();
        }
    }
	
	public void Update()
	{
		if (!toDestroy)
		{
			gameObject.transform.localPosition += gameObject.transform.GetForward().normalized * speed * Time.deltaTime;
		}

		if (timer > 0f && !toDestroy)
		{
			timer -= Time.deltaTime;
			if (timer <= 0f)
			{
				toDestroy = true;
				gameObject.GetChild("RancorProjectile").GetComponent<MeshRenderer>().active = false;
				timer = 1f;
			}
		}


		if (toDestroy)
		{
			if (timer > 0f && toDestroy)
			{
				timer -= Time.deltaTime;
				if (timer <= 0f)
				{
					InternalCalls.Destroy(gameObject);
				}
			}
		}
	}

	public void OnCollisionEnter(GameObject collidedGameObject)
	{
		if (collidedGameObject.CompareTag("Player") && !toDestroy)
		{
			PlayerHealth health = collidedGameObject.GetComponent<PlayerHealth>();
			if (health != null)
				health.TakeDamage(damage);
			toDestroy = true;
			gameObject.GetChild("RancorProjectile").GetComponent<MeshRenderer>().active = false;
			timer = 1.3f;
		}

		Audio.PlayAudio(gameObject, "Play_Rock_Impact");
		toDestroy = true;
		gameObject.GetChild("RancorProjectile").GetComponent<MeshRenderer>().active = false;
		timer = 1.3f;
		if (destroyParticles != null)
		{
			destroyParticles.Play();
			Debug.Log("Particles played");
		}
	}
}