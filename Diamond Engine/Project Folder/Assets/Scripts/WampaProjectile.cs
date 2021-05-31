using System;
using DiamondEngine;

public class WampaProjectile : DiamondComponent
{
	private float speed = 30.0f;
	private float lifeTime = 2f;
	public int damage = 10;

	public Vector3 targetDirection = Vector3.zero;

	private bool to_destroy = false;
	private float timer = 1.10f;

	public void Update()
	{
		if (!to_destroy)
		{
			//if (targetDirection == Vector3.zero)
			//	targetDirection = targetPos - gameObject.transform.globalPosition;

			gameObject.transform.localPosition += targetDirection.normalized * speed * Time.deltaTime;

			LookAt(targetDirection * gameObject.transform.GetForward() * 100);

		}
        if (lifeTime > 0)
        {
			lifeTime -= Time.deltaTime;
			if (lifeTime <= 0.0f)
				to_destroy = true;

        }


		else if (to_destroy)
		{
			if (timer > 0)
			{
				timer -= Time.deltaTime;
				if (timer <= 0.0f)
					InternalCalls.Destroy(gameObject);
			}
		}
	}

	public void OnTriggerEnter(GameObject triggeredGameObject)
	{
		if (triggeredGameObject.CompareTag("Player"))
		{
			PlayerHealth health = triggeredGameObject.GetComponent<PlayerHealth>();
			if (health != null)
				health.TakeDamage(damage);
			to_destroy = true;
		}
	}
	public void OnCollisionEnter(GameObject collidedGameObject)
	{
		Audio.PlayAudio(gameObject, "Play_Wampa_Projectile_Impact");
		//gameObject.transform.localPosition += new Vector3(0f, -10f, 0f);
		to_destroy = true;
	}

	public void LookAt(Vector3 direction)
	{
		float angle = (float)Math.Atan2(direction.x, direction.z);

		if (Math.Abs(angle * Mathf.Rad2Deg) < 1.0f)
			return;

		Quaternion dir = Quaternion.RotateAroundAxis(Vector3.up, angle);

		float rotationSpeed = Time.deltaTime * 5f;

		Quaternion desiredRotation = Quaternion.Slerp(gameObject.transform.localRotation, dir, rotationSpeed);

		gameObject.transform.localRotation = desiredRotation;
	}
}