using System;
using DiamondEngine;


public class Enemy : Entity
{
	public float detectionRange = 12.5f;
	public float damage = 5.0f;
	private float baseDamage = 1f;
    public float healthPoints = 60.0f;
	public float damageRecieveMult { get; protected set; } = 1f;
	public float damageMult { get; protected set; } = 1f;


	protected Vector3 targetPosition = null;
	protected float stoppingDistance = 1.0f;
	private string coinDropPath = "Library/Prefabs/989682380.prefab";

	public float slerpSpeed = 5.0f;

	//protected STATES currentState = STATES.WANDER;

	protected NavMeshAgent agent;

	protected Vector3 pushDir = Vector3.zero;

	protected override void InitEntity(ENTITY_TYPE myType)
	{
		eType = myType;
		speedMult = 1f;
		myDeltaTime = Time.deltaTime;
		baseDamage = damage;
		damageRecieveMult = 1f;
		damageMult = 1f;

		AddLoC();
	}

	protected virtual void AddLoC()
    {
		if (RoomSwitch.currentLevelIndicator == RoomSwitch.LEVELS.TWO)
		{
			AddStatus(STATUS_TYPE.ACCELERATED, STATUS_APPLY_TYPE.SUBSTITUTE, 0.075f, 1f, true);
			AddStatus(STATUS_TYPE.ENEMY_DAMAGE_UP, STATUS_APPLY_TYPE.SUBSTITUTE, 0.1f, 1f, true);
			AddStatus(STATUS_TYPE.ENEMY_HP_UP, STATUS_APPLY_TYPE.SUBSTITUTE, 0.2f, 1f, true);
		}
		else if (RoomSwitch.currentLevelIndicator == RoomSwitch.LEVELS.THREE)
		{
			Debug.Log("Enemy lvl 3 added statuses");
			AddStatus(STATUS_TYPE.ACCELERATED, STATUS_APPLY_TYPE.SUBSTITUTE, 0.15f, 1f, true);
			AddStatus(STATUS_TYPE.ENEMY_DAMAGE_UP, STATUS_APPLY_TYPE.SUBSTITUTE, 0.2f, 1f, true);
			AddStatus(STATUS_TYPE.ENEMY_HP_UP, STATUS_APPLY_TYPE.SUBSTITUTE, 0.5f, 1f, true);
		}
	}

	public virtual void TakeDamage(float damage)
	{}

	public virtual Vector3 CalculateNewPosition(float maxPos)
	{
        Vector3 newPosition = new Vector3(0, 0, 0);
        Random random = new Random();

        newPosition.x = random.Next(-(int)maxPos, (int)maxPos);
        newPosition.y = gameObject.transform.localPosition.y;
        newPosition.z = random.Next(-(int)maxPos, (int)maxPos);

        return newPosition;

        //return InternalCalls.GetWalkablePointAround(gameObject.transform.globalPosition, maxPos);
    }

	public void MoveToPosition(Vector3 positionToReach, float speed)
	{
		Vector3 direction = positionToReach - gameObject.transform.localPosition;

		gameObject.transform.localPosition += direction.normalized * speed * myDeltaTime;
	}
	public void InterpolatePosition(Vector3 positionToReach, float speed)
	{
		gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, positionToReach, speed * myDeltaTime);
	}

	public void LookAt(Vector3 pointToLook)
	{
		Vector3 direction = pointToLook - gameObject.transform.globalPosition;
		direction = direction.normalized;
		float angle = (float)Math.Atan2(direction.x, direction.z);

		//Debug.Log("Desired angle: " + (angle * Mathf.Rad2Deg).ToString());

		if (Math.Abs(angle * Mathf.Rad2Deg) < 1.0f)
			return;

		Quaternion dir = Quaternion.RotateAroundAxis(Vector3.up, angle);

		float rotationSpeed = myDeltaTime * slerpSpeed;

		Quaternion desiredRotation = Quaternion.Slerp(gameObject.transform.localRotation, dir, rotationSpeed);

		gameObject.transform.localRotation = desiredRotation;
	}

	public bool InRange(Vector3 point, float givenRange)
    {
		return Mathf.Distance(gameObject.transform.globalPosition, point) < givenRange;
	}

	public void DropCoins()
    {
		//Created dropped coins
		var rand = new Random();
		int droppedCoins = rand.Next(1, 3);
		for (int i = 0; i < droppedCoins; i++)
		{
			Vector3 pos = gameObject.transform.globalPosition;
			pos.x += rand.Next(-200, 201) / 100;
			pos.z += rand.Next(-200, 201) / 100;
			InternalCalls.CreatePrefab(coinDropPath, pos, Quaternion.identity, new Vector3(2.0f, 2.0f, 2.0f));
		}
	}

	protected override void OnInitStatus(ref StatusData statusToInit)
	{
		switch (statusToInit.statusType)
		{
			case STATUS_TYPE.SLOWED:
				{
					if (this.speedMult <= 0.1f)
						statusToInit.severity = 0f;

					this.speedMult -= statusToInit.severity;

					if (speedMult < 0.1f)
					{
						statusToInit.severity = statusToInit.severity - (Math.Abs(this.speedMult) + 0.1f);

						speedMult = 0.1f;
					}

					this.myDeltaTime = Time.deltaTime * speedMult;

				}
				break;
			case STATUS_TYPE.ENEMY_SLOWED:
				{
					if (this.speedMult <= 0.1f)
						statusToInit.severity = 0f;

					this.speedMult -= statusToInit.severity;

					if (speedMult < 0.1f)
					{
						statusToInit.severity = statusToInit.severity - (Math.Abs(this.speedMult) + 0.1f);

						speedMult = 0.1f;
					}

					this.myDeltaTime = Time.deltaTime * speedMult;

				}
				break;
			case STATUS_TYPE.ACCELERATED:
				{
					this.speedMult += statusToInit.severity;

					this.myDeltaTime = Time.deltaTime * speedMult;
				}
				break;
			case STATUS_TYPE.ENEMY_DAMAGE_DOWN:
				{
					float damageSubstracted = baseDamage * statusToInit.severity;

					this.damage -= damageSubstracted;

					statusToInit.statChange = damageSubstracted;

				}
				break;
			case STATUS_TYPE.ENEMY_VULNERABLE:
				{
					this.damageRecieveMult += statusToInit.severity;

				}
				break;
			case STATUS_TYPE.ENEMY_DAMAGE_UP:
				{
					float damageSubstracted = baseDamage * statusToInit.severity;

					this.damage += damageSubstracted;

					damageMult += statusToInit.severity;

					statusToInit.statChange = damageSubstracted;

				}
				break;
			case STATUS_TYPE.ENEMY_HP_UP:
				{
					float lifeGiven = healthPoints * statusToInit.severity;

					this.healthPoints += lifeGiven;

					statusToInit.statChange = lifeGiven;

				}
				break;
			default:
				break;
		}
	}

	protected override void OnUpdateStatus(StatusData statusToUpdate)
	{
		switch (statusToUpdate.statusType)
		{
			case STATUS_TYPE.ENEMY_BLEED:
				{
					float damageToTake = statusToUpdate.severity * Time.deltaTime;

					TakeDamage(damageToTake);
				}
				break;

			default:
				break;
		}
	}

	protected override void OnDeleteStatus(StatusData statusToDelete)
	{
		switch (statusToDelete.statusType)
		{
			case STATUS_TYPE.SLOWED:
				{
					this.speedMult += statusToDelete.severity;

					this.myDeltaTime = Time.deltaTime * speedMult;
				}
				break;
			case STATUS_TYPE.ENEMY_SLOWED:
				{
					this.speedMult += statusToDelete.severity;

					this.myDeltaTime = Time.deltaTime * speedMult;
				}
				break;
			case STATUS_TYPE.ACCELERATED:
				{
					this.speedMult -= statusToDelete.severity;

					this.myDeltaTime = Time.deltaTime * speedMult;
				}
				break;
			case STATUS_TYPE.ENEMY_DAMAGE_DOWN:
				{
					this.damage += statusToDelete.statChange;
				}
				break;
			case STATUS_TYPE.ENEMY_VULNERABLE:
				{
					this.damageRecieveMult -= statusToDelete.severity;
				}
				break;
			case STATUS_TYPE.ENEMY_DAMAGE_UP:
				{
					this.damage -= statusToDelete.statChange;

					damageMult -= statusToDelete.severity;
				}
				break;
			case STATUS_TYPE.ENEMY_HP_UP:
				{
					this.healthPoints -= statusToDelete.statChange;

					this.healthPoints = Math.Max(this.healthPoints, 1f);
				}
				break;
			default:
				break;
		}
	}

	public override bool IsDying()
	{
		return healthPoints <= 0f;
	}

	public virtual void PlayGrenadeHitParticles()
    {
		return;
    }

}
