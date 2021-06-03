using System;
using DiamondEngine;

public class TutoManager : DiamondComponent
{
	public void Awake()
    {
        if (Core.instance != null)
            BlackFade.onFadeInCompleted = Core.instance.gameObject.GetComponent<PlayerHealth>().TutorialDie;
    }
}