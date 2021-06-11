using System;
using DiamondEngine;

public class GroguFollowPoints : DiamondComponent
{
    public int priority = 0;
	private bool blocked = false;

    public bool IsBlocked()
    {
        return blocked;
    }

    public void Update()
    {
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Untagged"))
        {
            blocked = !blocked;
        }
    }
}