using System;
using DiamondEngine;

public class smallGrenade : DiamondComponent
{
    public GameObject thisReference = null; //This is needed until i make all this be part of a component base class


    public float detonationTime = 2.0f;

    private float Timer = 0.0f;

    private bool move = true;

    private bool detonate = false;

    private bool start = true;

    public void OnCollisionEnter(GameObject collidedGameObject)
    {        
        if (collidedGameObject.CompareTag("Enemy"))
        {
            detonate = true;
        }
    }

    public void Update()
    {
        if (start)
        {
            start = false;
            Core.instance.smallGrenades.Add(this);
        }

        if (thisReference.transform.globalPosition.y < Core.instance.gameObject.transform.globalPosition.y + 0.5)
        {
            move = false;
        }

        if (!move)
        {
            Timer += Time.deltaTime;
        }

        if (Timer > detonationTime || detonate)
        {
            Core.instance.smallGrenades.Remove(this);
            InternalCalls.Destroy(thisReference);
          
        }

    }

}