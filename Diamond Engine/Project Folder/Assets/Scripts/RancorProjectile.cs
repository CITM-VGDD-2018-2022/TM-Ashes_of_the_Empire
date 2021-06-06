using System;
using DiamondEngine;

public class RancorProjectile : DiamondComponent
{
	public float speed = 30.0f;
	public float lifeTime = 20.0f;
	public int damage = 10;

	public Vector3 targetPos = new Vector3(0, 0, 0);    //Set from Rancor.cs
	private Vector3 targetDirection = Vector3.zero;


	public void Update()
	{
		if (targetDirection == Vector3.zero)
			targetDirection = targetPos - gameObject.transform.globalPosition;
		
		gameObject.transform.localPosition += gameObject.transform.GetForward().normalized * speed * Time.deltaTime;

		lifeTime -= Time.deltaTime;

		if (lifeTime < 0.0f)
		{
			Audio.PlayAudio(gameObject, "Play_Rock_Impact");
			InternalCalls.Destroy(gameObject);
		}
	}

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Player"))
        {
			Audio.PlayAudio(gameObject, "Play_Rock_Impact");
			PlayerHealth health = triggeredGameObject.GetComponent<PlayerHealth>();
            if (health != null)
				health.TakeDamage(damage);
		}
    }
	public void OnCollisionEnter(GameObject collidedGameObject)
	{
		Audio.PlayAudio(gameObject, "Play_Rock_Impact");
		InternalCalls.Destroy(gameObject);
    }
}