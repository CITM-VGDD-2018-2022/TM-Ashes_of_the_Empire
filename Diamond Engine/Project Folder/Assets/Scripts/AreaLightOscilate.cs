using System;
using DiamondEngine;

public class AreaLightOscilate : DiamondComponent
{
	public float duration = 1.0f;

	public float initialIntensity = 0.0f;
	public float finalIntensity = 0.0f;

	public float initialFadeDistance = 0.0f;
	public float finalFadeDistance = 0.0f;

	public float initialMaxDistance = 0.0f;
	public float finalMaxDistance = 0.0f;

	enum INTERPOLATION_TYPE
	{
		LINEAR = 0,
		EASE_IN = 1,
		EASE_OUT_QUAD = 2,
		EASE_IN_CUBIC = 3,
		EASE_OUT_CUBIC = 4,
		EASE_IN_QUART = 5,
		EASE_IN_QUINT = 6,
		EASE_OUT_QUINT = 7
	}

	public int interpolationType = 0;

	private float timer = 0.0f;

	private AreaLight light = null;
	private bool firstFramePassed = false;  //first frame when charging a scene has a large dt, so we ignore it
	private bool invertLerp = false;

	public void Awake()
	{
		light = gameObject.GetComponent<AreaLight>();

		if (light != null)
		{
			light.SetIntensity(initialIntensity);
			light.SetFadeDistance(initialFadeDistance);
			light.SetMaxDistance(initialMaxDistance);
		}
	}

	public void Update()
	{
		if (firstFramePassed == true)
		{

			timer += Time.deltaTime;

			UpdateValues();


			if (timer >= duration)
			{
				invertLerp = !invertLerp;
				timer = 0.0f;
			}
		}

		else
			firstFramePassed = true;
	}


	private void UpdateValues()
	{
		float interpolationValue = timer / duration;

		switch ((INTERPOLATION_TYPE)interpolationType)
		{
			case INTERPOLATION_TYPE.EASE_IN:
				interpolationValue = Mathf.EaseInSine(interpolationValue);
				break;

			case INTERPOLATION_TYPE.EASE_OUT_QUAD:
				interpolationValue = Mathf.EaseOutQuad(interpolationValue);
				break;

			case INTERPOLATION_TYPE.EASE_IN_CUBIC:
				interpolationValue = Mathf.EaseInCubic(interpolationValue);
				break;

			case INTERPOLATION_TYPE.EASE_OUT_CUBIC:
				interpolationValue = Mathf.EaseOutCubic(interpolationValue);
				break;

			case INTERPOLATION_TYPE.EASE_IN_QUART:
				interpolationValue = Mathf.EaseInQuart(interpolationValue);
				break;

			case INTERPOLATION_TYPE.EASE_IN_QUINT:
				interpolationValue = Mathf.EaseInQuint(interpolationValue);
				break;

			case INTERPOLATION_TYPE.EASE_OUT_QUINT:
				interpolationValue = Mathf.EaseOutQuint(interpolationValue);
				break;

			default:
				break;
		}


		float intensity = 0.0f;
		float fadeDistance = 0.0f;
		float maxDistance = 0.0f;

		if (invertLerp == false)
		{
			intensity = Mathf.Lerp(initialIntensity, finalIntensity, interpolationValue);
			fadeDistance = Mathf.Lerp(initialFadeDistance, finalFadeDistance, interpolationValue);
			maxDistance = Mathf.Lerp(initialMaxDistance, finalMaxDistance, interpolationValue);
		}

		else
        {
			intensity = Mathf.Lerp(finalIntensity, initialIntensity, interpolationValue);
			fadeDistance = Mathf.Lerp(finalFadeDistance, initialFadeDistance, interpolationValue);
			maxDistance = Mathf.Lerp(finalMaxDistance, initialMaxDistance, interpolationValue);
		}


		light.SetIntensity(intensity);
		light.SetFadeDistance(fadeDistance);
		light.SetMaxDistance(maxDistance);
	}

}