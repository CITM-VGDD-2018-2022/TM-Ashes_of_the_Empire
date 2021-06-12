using System;
using DiamondEngine;

public class BH_DestructBox : DiamondComponent
{
	public GameObject explosion = null;
	public GameObject wave = null;
	private ParticleSystem partExp = null;
	private ParticleSystem partWave = null;

	private SphereCollider explosionCollider = null;

	public GameObject mesh = null;

	private bool triggered = false;
	public float explosionTime = 2.0f;
	private float timer = 0;
	public int explosion_damage = 0;

	int framesActive = 0;

	public void Awake()
    {
		EnemyManager.AddProp(gameObject);

		explosionCollider = gameObject.GetComponent<SphereCollider>();

		if(explosionCollider != null)
        {
			explosionCollider.active = false;
        }
	}


	public void Update()
	{
		if (triggered)
        {
			timer += Time.deltaTime;

			if(framesActive > 1)
            {
				if (explosionCollider != null && explosionCollider.active)
				{
					explosionCollider.active = false;
				}
			}
			else {
				framesActive++;
            }
        }

		if (timer >= explosionTime)
        {
			InternalCalls.Destroy(gameObject);
		}

	}

	public void OnTriggerEnter(GameObject triggeredGameObject)
	{
		if((triggeredGameObject.CompareTag("Bullet") || triggeredGameObject.CompareTag("ChargeBullet")) && triggered == false)
        {
			if (explosion != null && wave != null)
			{
				partExp = explosion.GetComponent<ParticleSystem>();
				partWave = wave.GetComponent<ParticleSystem>();

				Audio.PlayAudio(gameObject, "Play_Barrel_Explosion");
				EnemyManager.RemoveProp(gameObject);
			}

			if (partExp != null && !triggered)
				partExp.Play();

			if (partWave != null && !triggered)
				partWave.Play();

			if (mesh != null)
				InternalCalls.Destroy(mesh);
			triggered = true;

			if(explosionCollider != null)
            {
				explosionCollider.active = true;
            }
		}
	}

	public void OnDestroy()
    {
		EnemyManager.RemoveProp(gameObject);
	}
}