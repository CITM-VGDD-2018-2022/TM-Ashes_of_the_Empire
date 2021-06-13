using System;
using DiamondEngine;

public class EnableWithPause : DiamondComponent
{
	public GameObject enable = null;
	public GameObject disable = null;
	public GameObject disable2 = null;
	public GameObject select = null;
	public bool pause = false;
	public bool unpause = false;
	private bool start = true;


	public void Update()
    {
		if (!start)
			return;
		start = false;
		if (pause)
			Time.PauseGame();
    }
	public void OnExecuteButton()
	{
		if (disable != null)
		{
			disable.EnableNav(false);
		}

		if (disable2 != null)
		{
			disable2.EnableNav(false);
		}
		
		if (enable != null)
		{
			enable.EnableNav(true);
		}

		if (select != null)
		{
			Navigation navComponent = select.GetComponent<Navigation>();

			if (navComponent != null)
			{
				navComponent.Select();
			}
		}

		if (unpause)
        {
			Time.ResumeGame();
        }

		
	}
}