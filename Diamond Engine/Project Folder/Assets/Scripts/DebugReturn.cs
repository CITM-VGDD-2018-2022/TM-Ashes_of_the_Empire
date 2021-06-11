using System;
using DiamondEngine;

public class DebugReturn : DiamondComponent
{
    public GameObject window = null;
    private bool start = true;

    public void OnExecuteButton()
    {
        Time.ResumeGame();

        if (window != null)
            InternalCalls.Destroy(window);
    }

    public void Update()
    {
        if (start)
        {
            Time.PauseGame();
            start = false;
        }
    }

}