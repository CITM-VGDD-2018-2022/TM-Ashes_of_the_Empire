using System;
using DiamondEngine;

public class SkyTrooperShot : DiamondComponent
{
	public Skytrooper skytrooper = null;

	public GameObject visualFeedback = null;
	private MeshRenderer visualFeedbackRenderer = null;

	MeshRenderer meshRenderer = null;
	private BoxCollider collider = null;

	public int damage = 14;
	public float speed= 0.0f;
	public float gravity = 0.0f;

	public float damageRange = 3.5f;

	private bool damagedPlayer = false;

	Vector3 initialPos = null;
	Vector3 velocity = new Vector3(0,0,0);
	Vector3 targetPosition = null;

	float time = 0.0f;
	float disableTimer = 0.0f;
	public float disableTime = 4.0f;
	//private float rotationSpeed = 0.0f;

	public GameObject explosionObj = null;
	private ParticleSystem explosionParticles = null;
	public GameObject waveObj = null;
	private ParticleSystem waveParticles = null;

	private bool started = false;

	public void Start()
    {
		disableTimer = disableTime;

		if (gravity > 0)
			gravity = -gravity;

		meshRenderer = gameObject.GetComponent<MeshRenderer>();
		collider = gameObject.GetComponent<BoxCollider>();

		visualFeedback = InternalCalls.CreatePrefab("Library/Prefabs/203996773.prefab", new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity, null);
		if (visualFeedback != null)
		{
			visualFeedbackRenderer = visualFeedback.GetComponent<MeshRenderer>();
		}

		if (explosionObj != null)
			explosionParticles = explosionObj.GetComponent<ParticleSystem>();

		if (waveObj != null)
			waveParticles = waveObj.GetComponent<ParticleSystem>();

		Deactivate();

		started = true;
	}

	public void Update()
	{
		if (!started)
			Start();

		if (time < disableTime && initialPos != null)
        {
			time += Time.deltaTime;
			Vector3 pos = new Vector3(initialPos.x,initialPos.y,initialPos.z);
			pos.x += velocity.x * time;
			pos.z += velocity.z * time;
			pos.y += (velocity.y * time) + (0.5f * gravity * time * time);
			gameObject.transform.localPosition = pos;
        }

		if(disableTimer > 0.0f)
        {
			disableTimer -= Time.deltaTime;

			if(disableTimer <= 0.0f)
            {
				Deactivate();
			}
        }

		gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, new Quaternion(210.0f, 10.0f, 0.0f), Time.deltaTime);
	}

	public void SetTarget(Vector3 target, bool low_angle)
    {
		if (target == null || skytrooper == null)
			return;

        #region Position and Speed calculations
        initialPos = gameObject.transform.localPosition;
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
        #endregion

        if (visualFeedback != null)
        {
			visualFeedback.transform.localPosition = targetPosition;
        }
		
		disableTimer = disableTime;
	}

	public void OnCollisionEnter(GameObject collidedGameObject)
	{
		if(collidedGameObject.CompareTag("Player") && damagedPlayer == false)
        {
			PlayerHealth playerHealth = collidedGameObject.GetComponent<PlayerHealth>();
			
			if(playerHealth != null)
				playerHealth.TakeDamage(damage);

			damagedPlayer = true;
		}
		else if (damagedPlayer == false)
        {
			if (targetPosition != null && Mathf.Distance(targetPosition, gameObject.transform.globalPosition) < 1.0f)
            {
				//Debug.Log("SKYTROOPER BULLET ON TARGET POSITION");
				if (Mathf.Distance(Core.instance.gameObject.transform.globalPosition, targetPosition) < damageRange)
				{
					//Debug.Log("SKYTROOPER BULLET IN DAMAGE RANGE");
					
					if (Core.instance != null)
                    {
						Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
						//Debug.Log("Damage: " + damage.ToString());
						damagedPlayer = true;
					}
				}
            }
        }

		Explode();
		Deactivate();
    }

	public void Activate()
    {
		if(meshRenderer != null) 
			meshRenderer.active = true;
        
		if(collider != null) 
			collider.active = true;
        
		if (visualFeedbackRenderer != null)
			visualFeedbackRenderer.active = true;

		time = 0.0f;
		disableTimer = disableTime;
	}

	public void Deactivate()
    {
		if (meshRenderer != null)
			meshRenderer.active = false;
		
		if (collider != null)
			collider.active = false;
		
		if(visualFeedbackRenderer != null)
			visualFeedbackRenderer.active = false;

		initialPos = null;
	}
	
	public void Explode()
	{
		if (explosionParticles != null)
			explosionParticles.Play();

		if (waveParticles != null)
			waveParticles.Play();

		Audio.PlayAudio(gameObject, "Play_Skytrooper_Grenade_Explosion");
	}


	public bool IsActive()
    {
		if (meshRenderer != null)
			return meshRenderer.active;
		
		return false;
	}

	public void OnDestroy()
	{
		if(visualFeedback != null)
        {
			InternalCalls.Destroy(visualFeedback);
        }
	}
}