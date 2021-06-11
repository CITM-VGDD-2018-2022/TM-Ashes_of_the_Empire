using System;
using DiamondEngine;

public class AtackBosslv2 : DiamondComponent
{
    public float damage = 0.0f;
    public GameObject camera = null;
    public GameObject boss = null;

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Player") && active)
        {
            PlayerHealth health = triggeredGameObject.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage((int)damage);
                active = false;

                if (boss != null) {
                    if (boss.GetComponent<Wampa>() != null && boss.GetComponent<Wampa>().angry)
                    {
                        health.TakeDamage((int)damage);
                        Audio.PlayAudio(Core.instance.gameObject, "Play_Mando_Hit");
                    }
                    else if (boss.GetComponent<Skel>() != null && boss.GetComponent<Skel>().angry)
                    {
                        health.TakeDamage((int)damage);
                        Audio.PlayAudio(Core.instance.gameObject, "Play_Mando_Hit");
                    }
                }

                if (camera != null)
                {
                    Shake3D shake = camera.GetComponent<Shake3D>();
                    if (shake != null)
                    {
                        shake.StartShaking(0.8f, 0.1f);
                        Input.PlayHaptic(0.5f, 400);
                    }
                }
            }
            if (gameObject.CompareTag("Rush"))
            {
                Wampa wampa = boss.GetComponent<Wampa>();
                if (wampa != null)
                {
                    wampa.rushCrash = true;
                }
            }
        }
    }
}