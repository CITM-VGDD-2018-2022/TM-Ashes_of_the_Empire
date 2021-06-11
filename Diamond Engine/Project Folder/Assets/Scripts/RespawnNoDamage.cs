using System;
using DiamondEngine;

public class RespawnNoDamage : DiamondComponent
{
	private Vector3 initialPlayerPosition = null;
    private bool respawn = false;

	public void Awake()
    {
        if(Core.instance != null)
        {
            initialPlayerPosition = new Vector3(Core.instance.gameObject.transform.localPosition);
        }
    }

	public void Update()
	{
        if(initialPlayerPosition == null)
        {
            if (Core.instance != null)
            {
                initialPlayerPosition = new Vector3(Core.instance.gameObject.transform.localPosition);
            }
        }

        if(respawn)
        {
            Core.instance.gameObject.transform.localPosition = initialPlayerPosition;
            respawn = false;
        }
	}

    public void OnTriggerEnter(GameObject other)
    {
        if (other.CompareTag("Player") && initialPlayerPosition != null)
        {
            respawn = true;
        }
    }
}