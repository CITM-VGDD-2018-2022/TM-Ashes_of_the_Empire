using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class LaserTurret : Enemy
{
    enum STATE : int
    {
        NONE = -1,
        IDLE,
        LOAD,
        SHOOT,
        DIE
    }

    enum INPUT : int
    {
        IN_IDLE,
        IN_IDLE_END,
        IN_LOAD,
        IN_SHOOT,
        IN_DIE
    }

    //State
    private STATE currentState = STATE.NONE;

    private List<INPUT> inputsList = new List<INPUT>();

    //Action times
    public float idleTime = 0.0f;
    public float loadTime = 0.0f;
    public float shotTime = 0.0f;
    public float dieTime = 0.0f;

    public float damageMaxTimer = 0.0f;
    public float feedbackTime = 0.0f;
    public float laserWidth = 5.0f;

    //Speeds
    private float angle = 0.0f;
    private float rotationSpeed = 0.0f;

    //Ranges
    public float laserRange = 0.0f;
    public float shotTotalAngle = 0.0f;

    //Timers
    private float idleTimer = 0.0f;
    private float loadTimer = 0.0f;
    private float shotTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float damageCurrentTimer = 0.0f;

    private Vector3[] laserDirections;
    public int lasersNumber = 4;
    public float laserOffset;

    //Explosion effect
    public GameObject hit = null;
    public GameObject sniperHitObj = null;
    public GameObject grenadeHitObj = null;

    private ParticleSystem hitParticle = null;
    private ParticleSystem sniperHitParticle = null;
    private ParticleSystem grenadeHit = null;

    public void Awake()
    {
        InitEntity(ENTITY_TYPE.TURRET);
        EnemyManager.AddEnemy(gameObject);

        laserDirections = new Vector3[lasersNumber];

        targetPosition = null;

        currentState = STATE.IDLE;

        idleTimer = idleTime;

        ParticleSystem spawnparticles = null;

        StormTrooperParticles myParticles = gameObject.GetComponent<StormTrooperParticles>();
        if (myParticles != null)
        {
            spawnparticles = myParticles.spawn;
        }

        if (spawnparticles != null)
        {
            spawnparticles.Play();
        }

        if (hit != null)
        {
            hitParticle = hit.GetComponent<ParticleSystem>();
        }
        if (sniperHitObj != null)
        {
            sniperHitParticle = sniperHitObj.GetComponent<ParticleSystem>();
        }
        if (grenadeHitObj != null)
        {
            grenadeHit = grenadeHitObj.GetComponent<ParticleSystem>();
        }

        rotationSpeed = shotTotalAngle * Mathf.Deg2RRad / shotTime;
    }

    public void Update()
    {
        myDeltaTime = Time.deltaTime * speedMult;
        UpdateStatuses();

        #region STATE MACHINE

        ProcessInternalInput();
        ProcessExternalInput();
        ProcessState();

        UpdateState();

        #endregion
    }


    //Timers go here
    private void ProcessInternalInput()
    {
        if (currentState != STATE.DIE && gameObject.transform.globalPosition.y <= -120.0f)
        {
            inputsList.Add(INPUT.IN_DIE);
        }

        if (idleTimer > 0.0f)
        {
            idleTimer -= myDeltaTime;

            if (idleTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_LOAD);
            }
        }

        if (loadTimer > 0.0f)
        {
            loadTimer -= myDeltaTime;

            if (loadTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_SHOOT);
            }
        }

        if (damageCurrentTimer < damageMaxTimer)
            damageCurrentTimer += myDeltaTime;

        if (shotTimer > 0.0f)
        {
            shotTimer -= myDeltaTime;

            if (shotTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_IDLE);
            }
        }
    }

    private void ProcessExternalInput()
    {

    }

    //Manages state changes throught inputs
    private void ProcessState()
    {
        while (inputsList.Count > 0)
        {
            INPUT input = inputsList[0];

            switch (currentState)
            {
                case STATE.NONE:
                    break;

                case STATE.IDLE:
                    switch (input)
                    {
                        case INPUT.IN_LOAD:
                            currentState = STATE.LOAD;
                            StartLoad();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;
                    }
                    break;

                case STATE.LOAD:
                    switch (input)
                    {
                        case INPUT.IN_SHOOT:
                            currentState = STATE.SHOOT;
                            LoadEnd();
                            StartShoot();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            LoadEnd();
                            StartDie();
                            break;
                    }
                    break;

                case STATE.SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            ShootEnd();
                            StartIdle();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            ShootEnd();
                            StartDie();
                            break;
                    }
                    break;

                default:
                    break;
            }
            inputsList.RemoveAt(0);
        }
    }


    private void UpdateState()
    {
        switch (currentState)
        {
            case STATE.NONE:
                break;
            case STATE.IDLE:
                break;
            case STATE.LOAD:
                UpdateLoad();
                break;
            case STATE.SHOOT:
                UpdateShoot();
                break;
            case STATE.DIE:
                UpdateDie();
                break;
            default:
                break;
        }
    }

    #region IDLE
    private void StartIdle()
    {
        idleTimer = idleTime;
    }
    #endregion

    #region LOAD
    private void StartLoad()
    {
        loadTimer = loadTime;
        Audio.PlayAudio(gameObject, "Play_Turret_Shot_Charge");
    }
    private void UpdateLoad()
    {

    }
    private void LoadEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region SHOOT
    private void StartShoot()
    {
        shotTimer = shotTime;
        Audio.PlayAudio(gameObject, "Play_Turret_Shot");
        Audio.PlayAudio(gameObject, "Play_Turret_Charge");
    }

    private void UpdateShoot()
    {
        angle += rotationSpeed * myDeltaTime;
        gameObject.transform.localRotation = Quaternion.RotateAroundAxis(Vector3.up, angle);

        //LASER ROTATION
        CalculateLaserRotation();
    }
    private void ShootEnd()
    {
        Audio.StopAudio(gameObject);
    }

    private void CalculateLaserRotation()
    {
        float angleIncrement = 360 / lasersNumber;
        for (int i = 0; i < lasersNumber; i++)
        {
            ////Quaternion rotation = 
            Quaternion q = Quaternion.RotateAroundAxis(new Vector3(0, 1, 0), (angleIncrement * i) * 0.0174532925f);
            Vector3 v = gameObject.transform.GetForward() /** laserRange*/;

            // Do the math
            laserDirections[i] = Vector3.RotateAroundQuaternion(q, v);
        }


        for (int i = 0; i < laserDirections.Length; i++)
        {
            float hitDistance = 0;
            GameObject hit = InternalCalls.RayCast(gameObject.transform.globalPosition + Vector3.up + (laserDirections[i] * laserOffset), laserDirections[i], laserRange, ref hitDistance);
            if (hit != null)
            {
                if (hit.CompareTag("Player") && damageCurrentTimer >= damageMaxTimer)
                {
                    PlayerHealth health = hit.GetComponent<PlayerHealth>();
                    health.TakeDamage((int)damage);
                    damageCurrentTimer = 0.0f;
                }
            }

            InternalCalls.DrawRay(gameObject.transform.globalPosition + Vector3.up, gameObject.transform.globalPosition + Vector3.up + (laserDirections[i] * (hitDistance != 0 ? (hitDistance + laserOffset) : laserRange)),
                                    new Vector3(1.0f, 0.0f, 0.0f), laserWidth);
        }
    }
    #endregion

    #region DIE
    private void StartDie()
    {
        EnemyManager.RemoveEnemy(gameObject);

        dieTimer = dieTime;

        if (Core.instance != null)
            Audio.PlayAudio(Core.instance.gameObject, "Play_Mando_Kill_Voice");

        //Combo
        if (PlayerResources.CheckBoon(BOONS.BOON_MASTER_YODA_FORCE))
        {
            Core.instance.hud.GetComponent<HUD>().AddToCombo(300, 1.0f);
        }
    }
    private void UpdateDie()
    {
        if (dieTimer > 0.0f)
        {
            dieTimer -= myDeltaTime;

            if (dieTimer <= 0.0f)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Counter.SumToCounterType(Counter.CounterTypes.ENEMY_LASER_TURRET);

        DropCoins();

        Core.instance.gameObject.GetComponent<PlayerHealth>().SetCurrentHP(PlayerHealth.currHealth + PlayerHealth.healWhenKillingAnEnemy);
        GameObject obj = InternalCalls.CreatePrefab("Library/Prefabs/828188331.prefab", gameObject.transform.globalPosition, Quaternion.identity, new Vector3(1, 1, 1));
        Audio.PlayAudio(obj, "Play_Turret_Destruction");
        InternalCalls.Destroy(gameObject);
    }

    #endregion

    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("Bullet"))
        {
            if (Core.instance != null)
            {
                if (Core.instance.HasStatus(STATUS_TYPE.PRIM_SLOW))
                    AddStatus(STATUS_TYPE.SLOWED, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, Core.instance.GetStatusData(STATUS_TYPE.PRIM_SLOW).severity / 100, 2, false);
                if (Core.instance.HasStatus(STATUS_TYPE.QUICK_COMBO))
                    AddStatus(STATUS_TYPE.BLASTER_VULN, STATUS_APPLY_TYPE.ADDITIVE, Core.instance.GetStatusData(STATUS_TYPE.QUICK_COMBO).severity / 100, 5);
                if (Core.instance.HasStatus(STATUS_TYPE.PRIM_MOV_SPEED))
                    Core.instance.AddStatus(STATUS_TYPE.ACCELERATED, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, Core.instance.GetStatusData(STATUS_TYPE.PRIM_MOV_SPEED).severity / 100, 5, false);
            }
            BH_Bullet bullet = collidedGameObject.GetComponent<BH_Bullet>();
            if (bullet != null)
            {
                Audio.PlayAudio(gameObject, "Play_Turret_Hit");
                TakeDamage(bullet.GetDamage() * damageRecieveMult * BlasterVulnerability);
                // healthPoints -= collidedGameObject.GetComponent<BH_Bullet>().damage;
            }

            if (Core.instance.hud != null && currentState != STATE.DIE)
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(25, 0.95f);
            }

            if (currentState != STATE.DIE && healthPoints <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DIE);
            }

        }
        else if (collidedGameObject.CompareTag("ChargeBullet"))
        {
            ChargedBullet bullet = collidedGameObject.GetComponent<ChargedBullet>();
            Audio.PlayAudio(gameObject, "Play_Turret_Sniper_Hit");
            if (bullet != null && currentState != STATE.DIE)
            {
                if (sniperHitParticle != null)
                    sniperHitParticle.Play();

                if (Core.instance.hud != null)
                {
                    Core.instance.hud.GetComponent<HUD>().AddToCombo(55, 0.25f);
                }

                if (healthPoints <= 0.0f && Core.instance != null && Core.instance.HasStatus(STATUS_TYPE.AHSOKA_DET))
                    Core.instance.RefillSniper();
                //healthPoints -= bullet.damage;
                float vulerableSev = 0.2f;
                float vulerableTime = 4.5f;
                STATUS_APPLY_TYPE applyType = STATUS_APPLY_TYPE.BIGGER_PERCENTAGE;
                float damageMult = 1f;

                if (Core.instance != null)
                {
                    if (Core.instance.HasStatus(STATUS_TYPE.SNIPER_STACK_DMG_UP))
                    {
                        vulerableSev += Core.instance.GetStatusData(STATUS_TYPE.SNIPER_STACK_DMG_UP).severity;
                    }
                    if (Core.instance.HasStatus(STATUS_TYPE.SNIPER_STACK_ENABLE))
                    {
                        vulerableTime += Core.instance.GetStatusData(STATUS_TYPE.SNIPER_STACK_ENABLE).severity;
                        applyType = STATUS_APPLY_TYPE.ADD_SEV;
                    }
                    if (Core.instance.HasStatus(STATUS_TYPE.SNIPER_STACK_WORK_SNIPER))
                    {
                        vulerableSev += Core.instance.GetStatusData(STATUS_TYPE.SNIPER_STACK_WORK_SNIPER).severity;
                        damageMult = damageRecieveMult;
                    }
                    if (Core.instance.HasStatus(STATUS_TYPE.SNIPER_STACK_BLEED))
                    {
                        StatusData bleedData = Core.instance.GetStatusData(STATUS_TYPE.SNIPER_STACK_BLEED);
                        float chargedBulletMaxDamage = Core.instance.GetSniperMaxDamage();

                        damageMult *= bleedData.remainingTime;
                        this.AddStatus(STATUS_TYPE.ENEMY_BLEED, STATUS_APPLY_TYPE.ADD_SEV, (chargedBulletMaxDamage * bleedData.severity) / vulerableTime, vulerableTime);
                    }
                    if (Core.instance.HasStatus(STATUS_TYPE.CROSS_HAIR_LUCKY_SHOT))
                    {
                        float mod = Core.instance.GetStatusData(STATUS_TYPE.CROSS_HAIR_LUCKY_SHOT).severity;
                        Random rand = new Random();
                        float result = rand.Next(1, 101);
                        if (result <= mod)
                            Core.instance.RefillSniper();

                        Core.instance.luckyMod = 1 + mod / 100;
                    }
                }
                this.AddStatus(STATUS_TYPE.ENEMY_VULNERABLE, applyType, vulerableSev, vulerableTime);

                Audio.PlayAudio(gameObject, "Play_Turret_Hit");
                TakeDamage(bullet.GetDamage() * damageMult);

                if (Core.instance != null)
                {
                    if (Core.instance.HasStatus(STATUS_TYPE.SP_HEAL))
                    {
                        if (Core.instance.gameObject != null && Core.instance.gameObject.GetComponent<PlayerHealth>() != null)
                        {
                            float healing = Core.instance.GetStatusData(STATUS_TYPE.SP_HEAL).severity;
                            Core.instance.gameObject.GetComponent<PlayerHealth>().SetCurrentHP(PlayerHealth.currHealth + (int)(healing));
                        }
                    }
                    if (Core.instance.HasStatus(STATUS_TYPE.SP_FORCE_REGEN))
                    {
                        if (Core.instance.gameObject != null && BabyYoda.instance != null)
                        {
                            float force = Core.instance.GetStatusData(STATUS_TYPE.SP_FORCE_REGEN).severity;
                            BabyYoda.instance.SetCurrentForce((int)(BabyYoda.instance.GetCurrentForce() + force));
                        }
                    }


                    if (Core.instance.HasStatus(STATUS_TYPE.AHSOKA_DET))
                    {
                        Core.instance.RefillSniper();
                    }
                }
            }

        }
        else if (collidedGameObject.CompareTag("WorldLimit"))
        {
            if (currentState != STATE.DIE)
            {
                inputsList.Add(INPUT.IN_DIE);
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        float mod = 1f;
        if (Core.instance != null && Core.instance.HasStatus(STATUS_TYPE.GEOTERMAL_MARKER))
        {
            if (HasNegativeStatus())
            {
                mod = 1f + GetStatusData(STATUS_TYPE.GEOTERMAL_MARKER).severity / 100;
            }
        }
        healthPoints -= damage * mod;
        if (Core.instance != null)
        {
            if (Core.instance.HasStatus(STATUS_TYPE.WRECK_HEAVY_SHOT) && HasStatus(STATUS_TYPE.SLOWED))
                AddStatus(STATUS_TYPE.ENEMY_SLOWED, STATUS_APPLY_TYPE.SUBSTITUTE, Core.instance.GetStatusData(STATUS_TYPE.WRECK_HEAVY_SHOT).severity / 100, 3f);

            if (Core.instance.HasStatus(STATUS_TYPE.LIFESTEAL))
            {
                Random rand = new Random();
                float result = rand.Next(1, 101);
                if (result <= 11)
                    if (Core.instance.gameObject != null && Core.instance.gameObject.GetComponent<PlayerHealth>() != null)
                    {
                        float healing = Core.instance.GetStatusData(STATUS_TYPE.LIFESTEAL).severity * damage / 100;
                        if (healing < 1) healing = 1;
                        Core.instance.gameObject.GetComponent<PlayerHealth>().SetCurrentHP(PlayerHealth.currHealth + (int)(healing));
                    }
            }
            if (Core.instance.HasStatus(STATUS_TYPE.SOLO_HEAL))
            {
                Core.instance.gameObject.GetComponent<PlayerHealth>().SetCurrentHP(PlayerHealth.currHealth + (int)Core.instance.skill_SoloHeal);
                Core.instance.skill_SoloHeal = 0;
            }
        }

        if (hitParticle != null)
        {
            hitParticle.Play();
        }

        if (currentState != STATE.DIE)
        {
            if (healthPoints <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DIE);
                if (Core.instance != null)
                {
                    if (Core.instance.HasStatus(STATUS_TYPE.WINDU_FORCE) && BabyYoda.instance != null)
                        BabyYoda.instance.SetCurrentForce(BabyYoda.instance.GetCurrentForce() + (int)(Core.instance.GetStatusData(STATUS_TYPE.WINDU_FORCE).severity));
                }
            }
        }

    }

    public override void PlayGrenadeHitParticles()
    {
        if (grenadeHit != null)
        {
            grenadeHit.Play();
        }
    }
}