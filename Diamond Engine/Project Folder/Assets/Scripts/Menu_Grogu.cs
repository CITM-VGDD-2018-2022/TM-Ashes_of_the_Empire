using System;
using DiamondEngine;

public class Menu_Grogu : DiamondComponent
{
	public float verticalSpeed = 0.8f;

	public float verticalAmplitude = 0.2f;
	private bool flipVertical = false;
	private float time = 0.0f;
	public void Awake()
	{
		//MarcBurru

	}
	public void Update()
	{
		time += Time.deltaTime;
		gameObject.transform.localPosition += new Vector3(0.0f, verticalAmplitude * (float)Math.Sin(time), 0.0f);

	}

}