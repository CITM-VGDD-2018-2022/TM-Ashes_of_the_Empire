using System;
using DiamondEngine;

public class NeedControllerUI : DiamondComponent
{
	public GameObject controllerAlert = null;

	public void Update()
	{
		if (Input.GetControllerType() == ControllerType.SDL_CONTROLLER_TYPE_UNKNOWN)
        {
			if (controllerAlert != null)
				controllerAlert.Enable(true);
		}
		else
        {
			if (controllerAlert != null)
				controllerAlert.Enable(false);
		}
	}

}