using System;
using DiamondEngine;

public class DebugSkipLevel : DiamondComponent
{
    public void OnExecuteButton()
    {
        //RoomSwitch.SwitchToLevel2();
        DebugOptionsHolder.goToNextLevel = true;
    }
}