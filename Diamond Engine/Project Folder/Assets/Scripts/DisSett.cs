using System;
using DiamondEngine;

public class DisSett : DiamondComponent
{
    public GameObject displayScreen = null;
    public GameObject bigBrother = null;
    //private Pause aux = null;

    public void Awake()
    {
        Checkbox checkbox = gameObject.GetComponent<Checkbox>();
        if (checkbox == null)
            return;
        if (Config.IsVSYNCEnabled())
            checkbox.ChangeActive(true);
        else
            checkbox.ChangeActive(false);
    }
    public void OnExecuteCheckbox(bool active)
    {
        if (active)
            Config.VSYNCEnable(true);
        else
            Config.VSYNCEnable(false);
    }

    public void OnExecuteButton()
    {
        if (gameObject.Name == "Return")
        {
            bigBrother.EnableNav(true);
            displayScreen.EnableNav(false);
        }
        else if (gameObject.Name == "ResolutionUp")
            Config.SetResolution(Config.GetResolution() + 1);

        else if (gameObject.Name == "ResolutionDown")
            Config.SetResolution(Config.GetResolution() - 1);

        else if (gameObject.Name == "WindowModeUp" || gameObject.Name == "WindowModeDown")
        {
            int windowMode = Config.GetWindowMode();
            if (windowMode == 1)
                windowMode = 2;
            else
                windowMode = 0;
            Config.SetWindowMode(windowMode);
            ConfigFunctionality.UpdateDisplayText();
        }
    }

    public void Update()
    {

    }

}