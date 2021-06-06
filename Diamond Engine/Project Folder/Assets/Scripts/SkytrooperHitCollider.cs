using System;
using DiamondEngine;

public class SkytrooperHitCollider : DiamondComponent
{
	private float destroyTimer = 0.0f;
	public float destroyTime = 6.0f;

	public void Awake()
    {
		//Debug.Log("Timer Awake");
		destroyTimer = destroyTime;
    }

	public void Update()
	{
		if(destroyTimer > 0.0f)
        {
			destroyTimer -= Time.deltaTime;

			if (destroyTimer <= 0.0f)
            {
				gameObject.Enable(false);
            }
        }
	}
}