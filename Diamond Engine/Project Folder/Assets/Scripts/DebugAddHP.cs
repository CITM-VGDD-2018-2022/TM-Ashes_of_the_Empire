using System;
using DiamondEngine;

public class DebugAddHP : DiamondComponent
{
    public void OnExecuteButton()
    {
        if (Core.instance != null && Core.instance.gameObject != null)
        {
            PlayerHealth hp = Core.instance.gameObject.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                if (!DebugOptionsHolder.godModeActive)
                {
                    hp.TakeDamage(-20);
                }
            }
        }

    }

    //TODO TEST DELETE AWAKE
    //bool started = true;
    //public void Update()
    //{
    //    if (started)
    //    {
    //        PlayerHealth.ResetMaxAndCurrentHPToDefault();
    //        if (Core.instance != null && Core.instance.gameObject != null)
    //        {
    //            PlayerHealth hp = Core.instance.gameObject.GetComponent<PlayerHealth>();
    //            if (hp != null)
    //            {
    //                hp.SetCurrentHP(1);
    //            }
    //        }
    //        started = false;
    //    }
    //}

}