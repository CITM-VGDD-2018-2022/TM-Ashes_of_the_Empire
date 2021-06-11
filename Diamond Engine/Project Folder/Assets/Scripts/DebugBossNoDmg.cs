using System;
using DiamondEngine;

public class DebugBossNoDmg : DiamondComponent
{
    public void OnExecuteCheckbox(bool active)
    {
        if (active)
        {
            DebugOptionsHolder.bossDmg = true;
        }
        else
        {
            DebugOptionsHolder.bossDmg = false;
        }
    }

}