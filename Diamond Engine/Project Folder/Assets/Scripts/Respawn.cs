using System;
using DiamondEngine;

public class Respawn : DiamondComponent
{
    bool haveToRespawn = false; //I don't know why but it doesn't edit mando's pos on collision enter we have to have a dirty bool and do it from the update
    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("Player"))
        {
            haveToRespawn = true;
        }

    }

    public void Update()
	{
        if(Core.instance != null && Core.instance.gameObject.transform.globalPosition.y < -20.0f)
        {
            haveToRespawn = true;
        }

        if(haveToRespawn)
        {
            Core.instance.gameObject.GetComponent<Core>().RespawnOnFall();
            haveToRespawn = false;
        }
    }

}