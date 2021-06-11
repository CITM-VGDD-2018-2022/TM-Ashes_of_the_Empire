using System;
using DiamondEngine;

public class DebugToggleNoClip : DiamondComponent
{
    /*public void Awake()
    {
        
    }*/

    public void OnExecuteCheckbox(bool active)
    {
        if (active)
        {
            DebugOptionsHolder.noClip = true;
            Core.instance.gameObject.GetComponent<PlayerHealth>().ToggleNoClip(true);
        }
        else
        {
            DebugOptionsHolder.noClip = false;
            Core.instance.gameObject.GetComponent<PlayerHealth>().ToggleNoClip(false);
        }
    }
}