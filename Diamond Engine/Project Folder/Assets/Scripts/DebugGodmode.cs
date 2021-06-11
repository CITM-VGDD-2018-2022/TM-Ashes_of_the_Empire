using System;
using DiamondEngine;

public class DebugGodmode : DiamondComponent
{

    public void OnExecuteCheckbox(bool active)
    {
        if(active)
        {
            DebugOptionsHolder.godModeActive = true;
        }
        else
        {
            DebugOptionsHolder.godModeActive = false;
        }
    }

    public void Awake()
    { 
        Navigation navigation = gameObject.GetComponent<Navigation>();
        if (navigation != null)
        {
            navigation.SetUIElementAsActive(DebugOptionsHolder.godModeActive);
            navigation.Select();
        }
    }


    public void Update()
    {
    }
}