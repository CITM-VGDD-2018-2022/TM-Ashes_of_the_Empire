using System;
using DiamondEngine;

public class DebugNextRoom : DiamondComponent
{
    public void OnExecuteButton()
    {
        DebugOptionsHolder.goToNextRoom = true;
    }
}