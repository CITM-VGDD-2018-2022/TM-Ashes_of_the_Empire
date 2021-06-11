using System;
using DiamondEngine;

public class DebugToggleFPS : DiamondComponent
{
    public void OnExecuteCheckbox(bool active)
    {
        if (active)
        {
            DebugOptionsHolder.showFPS = true;
        }
        else
        {
            DebugOptionsHolder.showFPS = false;
        }
    }
}