using System;
using DiamondEngine;

public class WampaProjectile : DiamondComponent
{
	private float speed = 30.0f;
	public int damage = 10;

	public Vector3 targetDirection = Vector3.zero;

	private bool to_destroy = false;
	private float timer = 3f;


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
				gameObject.GetChild("Spike").GetComponent<MeshRenderer>().active = false;
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
			gameObject.GetChild("Spike").GetComponent<MeshRenderer>().active = false;
			timer = 1f;
		}
	}
	public void OnCollisionEnter(GameObject collidedGameObject)
	{
		Audio.PlayAudio(gameObject, "Play_Wampa_Projectile_Impact");
		//gameObject.transform.localPosition += new Vector3(0f, -10f, 0f);
		to_destroy = true;
		gameObject.GetChild("Spike").GetComponent<MeshRenderer>().active = false;
		timer = 1f;
	}

	public void LookAt(Vector3 direction)
	{
		float angle = (float)Math.Atan2(direction.x, direction.z);

		if (Math.Abs(angle * Mathf.Rad2Deg) < 1.0f)
			return;

		Quaternion dir = Quaternion.RotateAroundAxis(Vector3.up, angle);

		float rotationSpeed = Time.deltaTime * 50f;

		Quaternion desiredRotation = Quaternion.Slerp(gameObject.transform.localRotation, dir, rotationSpeed);

		gameObject.transform.localRotation = desiredRotation;
	}
}