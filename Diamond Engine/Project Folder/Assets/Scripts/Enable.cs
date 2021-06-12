using System;
using DiamondEngine;

public class Enable : DiamondComponent
{
	public GameObject enable = null;
	public GameObject disable = null;
	public GameObject disable2 = null;
	public GameObject disable3 = null;
	public GameObject select = null;
	public int startStatus = 0;

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
		if (disable3 != null)
		{
			disable3.EnableNav(false);
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
        if (startStatus > 0)
        {
            if (startStatus == 1)
            {
				Core.instance.startAvailable = false;
            }
			else if (startStatus == 2)
            {
				Core.instance.startAvailable = true;
			}
		}
    }
}