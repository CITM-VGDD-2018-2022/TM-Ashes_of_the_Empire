using System;
using DiamondEngine;

public class DebugShowTris : DiamondComponent
{
    public void OnExecuteCheckbox(bool active)
    {
        if (active)
        {
            DebugOptionsHolder.showTris = true;
        }
        else
        {
            DebugOptionsHolder.showTris = false;
        }
    }
}