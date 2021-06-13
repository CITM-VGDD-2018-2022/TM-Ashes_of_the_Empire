using System;
using DiamondEngine;
using System.Collections.Generic;

public class SlowGrenade : DiamondComponent
{
    bool start = true;
    bool detonate = false;
    public GameObject grenadeActiveObj = null;
    private ParticleSystem grenadeActivePar = null;
    public GameObject granadeAreaObj = null;
    private ParticleSystem granadeArea = null;
    private Vector3 forceToAdd = Vector3.zero;
    private Vector3 myScale = Vector3.zero;

    public float lifeTime = 4.0f;
    private float lifeTimer = 0.0f;

    public float explosionTime = 4.0f;
    private float explosionTimer = 0.0f;

    public float areaRadius = 1.85f;
    private float currentRadius = 1f;
    public float damage = 3.5f;
    public float procTime = 0.25f;
    private float procTimer = 0f;
    private bool procActivation = false;
    private bool addedForce = false;
    private bool readyToAddForce = false;
    List<Entity> enemies = new List<Entity>();
    private bool destroy = false;

    public void Update()
    {
        if (start == true)
        {
            if (myScale == Vector3.zero)
                myScale = this.gameObject.transform.localScale;

            detonate = false;
            start = false;
        }

        if (addedForce == false && readyToAddForce == true)
        {
            if (detonate == false)
                gameObject.AddForce(forceToAdd);
            addedForce = true;
        }

        if (!detonate)
        {
            lifeTimer += Time.deltaTime;
        }
        else
        {
            float grenadeDamage = damage;
            if (Core.instance != null)
            {
                grenadeDamage *= Core.instance.GetGrenadeDamageMod();
                if (Core.instance.HasStatus(STATUS_TYPE.BOSSK_AMMO) && enemies.Count > 1)
                    grenadeDamage *= 2;
            }

            explosionTimer += Time.deltaTime;
            float mult = 1;
            if (Core.instance.HasStatus(STATUS_TYPE.BOBBA_STUN_AMMO))
                mult += Core.instance.GetStatusData(STATUS_TYPE.BOBBA_STUN_AMMO).severity / 100;
            procTimer += Time.deltaTime * mult;

            if (procTimer > procTime)
            {
                procActivation = true;
                Audio.StopAudio(gameObject);
                Audio.PlayAudio(gameObject, "Play_Mando_Grenade_Pulse_Wave");
                procTimer = 0f;
                SerchEnemiesByDistance();
            }


            for (int i = 0; i < enemies.Count; i++)
            {

                if (enemies[i] == null || enemies[i].gameObject.transform.globalPosition.Distance(this.gameObject.transform.globalPosition) > currentRadius)
                {
                    enemies.Remove(enemies[i]);
                    i--;
                    continue;
                }

                float slow = 0.3f;
                if (Core.instance != null)
                    if (Core.instance.HasStatus(STATUS_TYPE.SEC_SLOW))
                        slow *= (1f + Core.instance.GetStatusData(STATUS_TYPE.SEC_SLOW).severity / 100);

                Entity myEntComp = enemies[i];
                Enemy eneScript = null;

                if(myEntComp.gameObject != null)
                {
                    switch (myEntComp.GetEntityType())
                    {
                        case ENTITY_TYPE.STROMTROOPER:
                            {
                                eneScript = myEntComp.gameObject.GetComponent<StormTrooper>();
                                if (eneScript == null)
                                {
                                    eneScript = myEntComp.gameObject.GetComponent<DummyStormtrooper>();
                                }
                            }
                            break;
                        case ENTITY_TYPE.BANTHA:
                            {
                                eneScript = myEntComp.gameObject.GetComponent<Bantha>();
                            }
                            break;
                        case ENTITY_TYPE.SKYTROOPER:
                            {
                                eneScript = myEntComp.gameObject.GetComponent<Skytrooper>();
                            }
                            break;
                        case ENTITY_TYPE.TURRET:
                            {
                                eneScript = myEntComp.gameObject.GetComponent<LaserTurret>();
                            }
                            break;
                        case ENTITY_TYPE.DEATHTROOPER:
                            {
                                eneScript = myEntComp.gameObject.GetComponent<Deathtrooper>();
                            }
                            break;
                        case ENTITY_TYPE.HEAVYTROOPER:
                            {
                                eneScript = myEntComp.gameObject.GetComponent<HeavyTrooper>();
                            }
                            break;
                        case ENTITY_TYPE.RANCOR:
                            {
                                Rancor bossScript = myEntComp.gameObject.GetComponent<Rancor>();

                                if (bossScript != null)
                                {
                                    if (bossScript.healthPoints <= 0)
                                    {
                                        enemies.Remove(enemies[i]);
                                        i--;
                                    }
                                    else if (procActivation == true)
                                    {
                                        Core.instance.hud.GetComponent<HUD>().AddToCombo(5, 1.3f);
                                        bossScript.TakeDamage(grenadeDamage * bossScript.damageRecieveMult);
                                        bossScript.AddStatus(STATUS_TYPE.SLOWED, STATUS_APPLY_TYPE.BIGGER_TIME, slow * 0.5f, 0.175f);
                                    }
                                }
                            }
                            break;
                        case ENTITY_TYPE.WAMPA:
                            {
                                Wampa bossScript = myEntComp.gameObject.GetComponent<Wampa>();

                                if (bossScript != null)
                                {
                                    if (bossScript.healthPoints <= 0)
                                    {
                                        enemies.Remove(enemies[i]);
                                        i--;
                                    }
                                    else if (procActivation == true)
                                    {
                                        Core.instance.hud.GetComponent<HUD>().AddToCombo(5, 1.3f);
                                        bossScript.TakeDamage(grenadeDamage * bossScript.damageRecieveMult);
                                        bossScript.AddStatus(STATUS_TYPE.SLOWED, STATUS_APPLY_TYPE.BIGGER_TIME, slow * 0.5f, 0.175f);
                                    }
                                }
                            }
                            break;
                        case ENTITY_TYPE.SKEL:
                            {
                                Skel bossScript = myEntComp.gameObject.GetComponent<Skel>();

                                if (bossScript != null)
                                {
                                    if (bossScript.healthPoints <= 0)
                                    {
                                        enemies.Remove(enemies[i]);
                                        i--;
                                    }
                                    else if (procActivation == true)
                                    {
                                        Core.instance.hud.GetComponent<HUD>().AddToCombo(5, 1.3f);
                                        bossScript.TakeDamage(grenadeDamage * bossScript.damageRecieveMult);
                                        bossScript.AddStatus(STATUS_TYPE.SLOWED, STATUS_APPLY_TYPE.BIGGER_TIME, slow * 0.5f, 0.175f);
                                    }
                                }
                            }
                            break;
                        case ENTITY_TYPE.MOFF:
                            {
                                MofGuideonRework bossScript = myEntComp.gameObject.GetComponent<MofGuideonRework>();

                                if (bossScript != null)
                                {
                                    if (bossScript.currentHealthPoints <= 0)
                                    {
                                        enemies.Remove(enemies[i]);
                                        i--;
                                    }
                                    else if (procActivation == true)
                                    {
                                        Core.instance.hud.GetComponent<HUD>().AddToCombo(5, 1.3f);
                                        bossScript.TakeDamage(grenadeDamage * bossScript.damageRecieveMult);
                                        bossScript.AddStatus(STATUS_TYPE.SLOWED, STATUS_APPLY_TYPE.BIGGER_TIME, slow * 0.5f, 0.175f);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                if (eneScript != null && !myEntComp.IsBoss())
                {
                    if (eneScript.healthPoints <= 0)
                    {
                        enemies.Remove(enemies[i]);
                        i--;
                    }
                    else if (procActivation == true)
                    {
                        Core.instance.hud.GetComponent<HUD>().AddToCombo(5, 1.3f);
                        eneScript.TakeDamage(grenadeDamage * eneScript.damageRecieveMult);
                        eneScript.PlayGrenadeHitParticles();
                        eneScript.AddStatus(STATUS_TYPE.SLOWED, STATUS_APPLY_TYPE.BIGGER_TIME, slow, 0.175f);
                    }

                }

            }

            procActivation = false;
        }

        if (lifeTimer >= lifeTime && !detonate)
        {
            Detonate();

        }
        if (Core.instance != null && Core.instance.HasStatus(STATUS_TYPE.SEC_DURATION))
        {
            if (explosionTimer >= explosionTime * (1 + Core.instance.GetStatusData(STATUS_TYPE.SEC_DURATION).severity / 100))
                Explode();
        }
        else
        {
            if (explosionTimer >= explosionTime)
                Explode();
        }

        readyToAddForce = true;

    }

    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (detonate == true)
            return;

        if (detonate == false && gameObject.IsEnabled() && !collidedGameObject.CompareTag("Player") && !collidedGameObject.CompareTag("StormTrooperBullet"))
        {
            Detonate();
        }
    }

    private void Detonate()
    {
        if (detonate == true)
            return;

        this.gameObject.SetVelocity(new Vector3(0, 0, 0));

        BoxCollider myBoxColl = this.gameObject.GetComponent<BoxCollider>();

        if (myBoxColl != null)
            myBoxColl.active = true;

        if (grenadeActiveObj != null)
        {
            grenadeActivePar = grenadeActiveObj.GetComponent<ParticleSystem>();
            if (grenadeActivePar != null)
                grenadeActivePar.Play();
        }
        if (granadeAreaObj != null)
        {
            granadeArea = granadeAreaObj.GetComponent<ParticleSystem>();
            if (granadeArea != null)
                granadeArea.Play();
        }
        detonate = true;
        Audio.PlayAudio(gameObject, "Play_Mando_Grenade_Set_Up");
        lifeTimer = 0;

        SerchEnemiesByDistance();
    }

    private void Explode()
    {
        //InternalCalls.CreatePrefab("Library/Prefabs/2084632366.prefab", gameObject.transform.globalPosition, new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f));

        if (destroy == false)
            GrenadeFinish();
        else
            InternalCalls.Destroy(this.gameObject);
    }

    private bool AddEnemyTolist(GameObject enemy)
    {
        bool ret = false;

        if (IsEnemyOnList(enemy) == false)
        {
            Entity script = enemy.GetComponent<StormTrooper>();
            if (script == null)
            {
                script = enemy.GetComponent<DummyStormtrooper>();
            }
            if (script == null)
            {
                script = enemy.GetComponent<Bantha>();
            }
            if (script == null)
            {
                script = enemy.GetComponent<Skytrooper>();
            }
            if (script == null)
            {
                script = enemy.GetComponent<LaserTurret>();
            }
            if (script == null)
            {
                script = enemy.GetComponent<HeavyTrooper>();
            }
            if (script == null)
            {
                script = enemy.GetComponent<Deathtrooper>();
            }
            if (script == null)
            {
                script = enemy.GetComponent<Rancor>();
            }
            if (script == null)
            {
                script = enemy.GetComponent<Skel>();
            }
            if (script == null)
            {
                script = enemy.GetComponent<Wampa>();
            }
            if (script == null)
            {
                script = enemy.GetComponent<MofGuideonRework>();
            }
            if (script == null)
            {
                script = enemy.GetComponent<MoffGideon>();
            }

            if (script != null)
            {
                if (script.IsDying() == false)
                    enemies.Add(script);

                ret = true;
            }

        }

        return ret;
    }

    private bool IsEnemyOnList(GameObject enemy)
    {
        bool ret = false;

        foreach (Entity item in enemies)
        {
            if (item.gameObject.GetUid() == enemy.GetUid())
            {
                ret = true;
                break;
            }
        }

        return ret;
    }

    private bool RemoveEnemyFromList(GameObject enemy)
    {
        bool ret = false;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].gameObject.GetUid() == enemy.GetUid() && enemies[i].gameObject.transform.globalPosition.Distance(this.gameObject.transform.globalPosition) > currentRadius)
            {
                ret = true;
                enemies.RemoveAt(i);
                break;
            }
        }

        return ret;
    }

    public void GrenadeInit(Vector3 position, Quaternion rotation, Vector3 force, bool destroy = false)
    {
        //This doesn't work if the grenade is child of anything
        this.gameObject.transform.localPosition = position;
        this.gameObject.transform.localRotation = rotation;

        float modifier = 1;
        if (Core.instance != null)
            if (Core.instance.HasStatus(STATUS_TYPE.SEC_RANGE))
                modifier = 1 + Core.instance.GetStatusData(STATUS_TYPE.SEC_RANGE).severity / 100;

        forceToAdd = force * modifier;

        currentRadius = areaRadius;
        if (Core.instance != null)
            if (Core.instance.HasStatus(STATUS_TYPE.BOBBA_STUN_AMMO))
                currentRadius *= (1 + Core.instance.GetStatusData(STATUS_TYPE.BOBBA_STUN_AMMO).severity / 100);


        forceToAdd.x = Math.Min(forceToAdd.x, 2000f);
        forceToAdd.y = Math.Min(forceToAdd.y, 100f);
        forceToAdd.z = Math.Min(forceToAdd.z, 2000f);

        this.destroy = destroy;

        BoxCollider myBoxColl = this.gameObject.GetComponent<BoxCollider>();

        if (myBoxColl != null)
            myBoxColl.active = false;

        enemies.Clear();

        myScale = this.gameObject.transform.localScale;

        this.gameObject.SetVelocity(new Vector3(0, 0, 0));

        if (grenadeActiveObj != null)
        {
            grenadeActivePar = grenadeActiveObj.GetComponent<ParticleSystem>();
            if (grenadeActivePar != null)
                grenadeActivePar.Stop();
        }
        if (granadeAreaObj != null)
        {
            granadeArea = granadeAreaObj.GetComponent<ParticleSystem>();
            if (granadeArea != null)
                granadeArea.Stop();
        }
    }

    public void GrenadeFinish()
    {
        //this.gameObject.transform.localPosition = Vector3.positiveInfinity;
        this.gameObject.SetVelocity(new Vector3(0, 0, 0));

        enemies.Clear();

        currentRadius = areaRadius;
        readyToAddForce = false;
        start = true;
        detonate = false;
        lifeTimer = 0.0f;
        procTimer = 0f;

        explosionTimer = 0.0f;
        procActivation = false;
        addedForce = false;

        Audio.StopAudio(gameObject);

        if (grenadeActiveObj != null)
        {
            grenadeActivePar = grenadeActiveObj.GetComponent<ParticleSystem>();
            if (grenadeActivePar != null)
                grenadeActivePar.Stop();
        }
        if (granadeAreaObj != null)
        {
            granadeArea = granadeAreaObj.GetComponent<ParticleSystem>();
            if (granadeArea != null)
                granadeArea.Stop();
        }

        forceToAdd = Vector3.zero;
        this.gameObject.transform.localScale = myScale;

        BoxCollider myBoxColl = this.gameObject.GetComponent<BoxCollider>();

        if (myBoxColl != null)
            myBoxColl.active = false;

        this.gameObject.Enable(false);
    }

    public void EnemyDied(GameObject deadEnemy)
    {
        RemoveEnemyFromList(deadEnemy);
    }

    private void SerchEnemiesByDistance()
    {
        if (EnemyManager.currentEnemies != null)
        {
            for (int i = 0; i < EnemyManager.currentEnemies.Count; i++)
            {
                float distance = EnemyManager.currentEnemies[i].transform.globalPosition.Distance(this.gameObject.transform.globalPosition);

                if (distance <= currentRadius)
                    AddEnemyTolist(EnemyManager.currentEnemies[i]);
            }

        }
    }
}