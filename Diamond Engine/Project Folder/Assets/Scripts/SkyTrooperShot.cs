using System;
using DiamondEngine;

public class SkyTrooperShot : DiamondComponent
{
	SkytrooperHitCollider hitCollider = null;

	public int damage = 14;
	public float speed= 0.0f;
	public float gravity = 0.0f;

	public float damageRange = 3.5f;

	Vector3 initialPos = null;
	Vector3 velocity = new Vector3(0,0,0);
	Vector3 targetPosition = null;

	float time = 0.0f;
	float deleteTimer = 0.0f;
	public float lifeTime = 6.0f;

	public void Awake()
    {
		deleteTimer = lifeTime;

		if (gravity > 0)
			gravity = -gravity;
    }
	public void Update()
	{
        if (time < lifeTime)
        {
			time += Time.deltaTime;
			Vector3 pos = new Vector3(initialPos.x,initialPos.y,initialPos.z);
			pos.x += velocity.x * time;
			pos.z += velocity.z * time;
			pos.y += (velocity.y * time) + (0.5f * gravity * time * time);
			gameObject.transform.localPosition = new Vector3(pos.x,pos.y,pos.z);
        }

		if(deleteTimer > 0.0f)
        {
			deleteTimer -= Time.deltaTime;

			if(deleteTimer <= 0.0f)
            {
				InternalCalls.Destroy(gameObject);
			}
        }
	}

	public void SetTarget(Vector3 target, bool low_angle)
    {
		initialPos = gameObject.transform.globalPosition;
		targetPosition = new Vector3(target.x, target.y, target.z);

		float distanceX;
		float distanceZ;
		float distanceHorizontal;

		float distanceY = targetPosition.y - initialPos.y;

		if (initialPos.x == targetPosition.x)
        {
			distanceX = 0;
			distanceZ = targetPosition.z - initialPos.z;
			distanceHorizontal = distanceZ;
        }
		else if (initialPos.z == targetPosition.z)
        {
			distanceX = targetPosition.x - initialPos.x;
			distanceZ = 0;
			distanceHorizontal = distanceX;
		}
        else
        {
			distanceX = targetPosition.x - initialPos.x;
			distanceZ = targetPosition.z - initialPos.z;

			distanceHorizontal = (float)Math.Sqrt(distanceX*distanceX+distanceZ*distanceZ);
		}

		float calcAngleSubFunction = (speed * speed) / (-gravity * distanceHorizontal);
		float angleSpeed;
		if (low_angle)
			angleSpeed = (float)Math.Atan(calcAngleSubFunction - (float)Math.Sqrt(calcAngleSubFunction * calcAngleSubFunction - 1 - 2 * calcAngleSubFunction * distanceY / distanceHorizontal));
		else 
			angleSpeed = (float)Math.Atan(calcAngleSubFunction + (float)Math.Sqrt(calcAngleSubFunction * calcAngleSubFunction - 1 - 2 * calcAngleSubFunction * distanceY / distanceHorizontal));


		float velocityHorizontal = speed * (float)Math.Cos(angleSpeed);
		
		velocity.y = speed *(float)Math.Sin(angleSpeed);

		if (distanceX == 0)
        {
			velocity.x = 0;
			velocity.z = velocityHorizontal;
        }
		else if (distanceZ == 0)
        {
			velocity.x = velocityHorizontal;
			velocity.z = 0;
		}
		else
        {
			float angle = (float)Math.Atan(distanceZ / distanceX); 
			velocity.x = velocityHorizontal * (float)Math.Cos(angle);
			velocity.z = velocityHorizontal * (float)Math.Sin(angle);
		}
		
		if (distanceX < 0 && velocity.x > 0)
			velocity.x = -velocity.x;


		if ((distanceZ < 0 && velocity.z > 0) || (distanceZ > 0 && velocity.z < 0))
			velocity.z = -velocity.z;

		GameObject hitColliderObject = InternalCalls.CreatePrefab("Library/Prefabs/203996773.prefab", targetPosition, new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f));

		if (hitColliderObject != null)
			hitCollider = hitColliderObject.GetComponent<SkytrooperHitCollider>();
	}

	public void OnCollisionEnter(GameObject collidedGameObject)
	{
		if(collidedGameObject.CompareTag("Player"))
        {
			PlayerHealth playerHealth = collidedGameObject.GetComponent<PlayerHealth>();
			
			if(playerHealth != null)
				playerHealth.TakeDamage(damage);
        }
		else
        {
			if (Mathf.Distance(targetPosition, gameObject.transform.globalPosition) < 1.0f)
            {
				if (Mathf.Distance(Core.instance.gameObject.transform.globalPosition, targetPosition) < damageRange)
				{
					PlayerHealth playerHealth = collidedGameObject.GetComponent<PlayerHealth>();

					if (playerHealth != null)
						playerHealth.TakeDamage(damage);
				}
            }
        }

		if(hitCollider != null)
			InternalCalls.Destroy(hitCollider.gameObject);

		hitCollider = null;
		InternalCalls.Destroy(gameObject);
	}
}