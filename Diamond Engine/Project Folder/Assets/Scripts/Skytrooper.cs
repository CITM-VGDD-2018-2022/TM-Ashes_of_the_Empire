using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class Skytrooper : Enemy
{
    enum STATE : int
    {
        NONE = -1,
        IDLE,
        WANDER,
        DASH,
        PUSHED,
        SHOOT,
        HIT,
        DIE
    }

    enum INPUT : int
    {
        IN_IDLE,
        IN_IDLE_END,
        IN_DASH,
        IN_DASH_END,
        IN_WANDER,
        IN_PUSHED,
        IN_SHOOT,
        IN_HIT,
        IN_DIE,
        IN_PLAYER_IN_RANGE,

    }

    //State
    private STATE currentState = STATE.NONE;

    private List<INPUT> inputsList = new List<INPUT>();

    public GameObject shootPoint = null;
    public GameObject blaster = null;

    public GameObject meshObj = null;
    public GameObject blasterMeshObj = null;

    private float initialHeight = 0.0f;

    private bool standStill = false;

    //Action times
    public float idleTime = 5.0f;
    public float wanderTime = 0.0f;
    public float dashTime = 1.0f;
    private float dieTime = 0.75f;
    public float timeBewteenShots = 0.5f;
    public float timeBewteenShootingStates = 1.5f;
    private float shootAnimationTime = 0.0f;

    //Speeds
    public float wanderSpeed = 3.5f;
    private float dashSpeed = 7.5f;

    //Ranges
    public float wanderRange = 7.5f;
    public float dashRange = 12.5f;

    //Timers
    private float idleTimer = 0.0f;
    private float wanderTimer = 0.0f;
    private float dashTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float shootTimer = 0.0f;
    private float pushTimer = 0.0f;
    private float currAnimationPlaySpd = 1f;

    //Action variables
    private int shotsShooted = 0;
    public int maxShots = 2;
    public float explosionDistance = 2.0f;

    //Push
    public float forcePushMod = 1.0f;
    public float PushStun = 2.0f;

    //Shoot
    private GameObject primaryBulletObj = null;
    private SkyTrooperShot primaryBullet = null;
    private GameObject secondaryBulletObj = null;
    private SkyTrooperShot secondaryBullet = null;
    private float shootAnimTimer = 0.0f;

    //Hit particles
    public GameObject explosionParticlesObj = null;
    public GameObject hitParticlesObj = null;
    public GameObject sniperHitParticleObj = null;
    public GameObject grenadeHitParticleObj = null;
    private ParticleSystem explosionParticles = null;
    private ParticleSystem hitParticles = null;
    private ParticleSystem sniperHitParticle = null;
    private ParticleSystem grenadeHitParticle = null;

    private float raycastOffset = 0.75f;

    //Raycast Debug
    //Vector3 raycastPoint = null;
    //Vector3 notHitPoint = null;

    public void Awake()
    {
        InitEntity(ENTITY_TYPE.SKYTROOPER);
        EnemyManager.AddEnemy(gameObject);

        agent = gameObject.GetComponent<NavMeshAgent>();
        targetPosition = null;

        currentState = STATE.IDLE;
        Animator.Play(gameObject, "SK_Idle", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "SK_Idle", speedMult);

        UpdateAnimationSpd(speedMult);

        idleTimer = idleTime;
        //dashTime = Animator.GetAnimationDuration(gameObject, "SK_Dash");
        dashSpeed = dashRange / dashTime;

        initialHeight = gameObject.transform.globalPosition.y;

        if (explosionParticlesObj != null)
            explosionParticles = explosionParticlesObj.GetComponent<ParticleSystem>();

        if (hitParticlesObj != null)
            hitParticles = hitParticlesObj.GetComponent<ParticleSystem>();

        if (sniperHitParticleObj != null)
            sniperHitParticle = sniperHitParticleObj.GetComponent<ParticleSystem>();

        if (grenadeHitParticleObj != null)
            grenadeHitParticle = grenadeHitParticleObj.GetComponent<ParticleSystem>();

        shootAnimationTime = Animator.GetAnimationDuration(gameObject, "SK_Shoot");

        //Bullets creation
        primaryBulletObj = InternalCalls.CreatePrefab("Library/Prefabs/1662408971.prefab", new Vector3(0.0f, 0.0f, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), null);
        if (primaryBulletObj != null)
        {
            primaryBullet = primaryBulletObj.GetComponent<SkyTrooperShot>();

            if (primaryBullet != null)
            {
                primaryBullet.skytrooper = this;
            }
        }

        secondaryBulletObj = InternalCalls.CreatePrefab("Library/Prefabs/1662408971.prefab", new Vector3(0.0f, 0.0f, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), null);
        if (secondaryBulletObj != null)
        {
            secondaryBullet = secondaryBulletObj.GetComponent<SkyTrooperShot>();

            if (secondaryBullet != null)
            {
                secondaryBullet.skytrooper = this;
            }
        }
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
                inputsList.Add(INPUT.IN_WANDER);
            }
        }

        if (currentState == STATE.WANDER && wanderTimer > 0.0f)
        {
            wanderTimer -= myDeltaTime;
            if (wanderTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_IDLE);
            }
        }

        if (currentState == STATE.DASH && dashTimer > 0.0f)
        {
            dashTimer -= myDeltaTime;
            if (dashTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_DASH_END);
            }
        }

        //Raycast Debug
        /*
        if (notHitPoint != null)
        {
            Vector3 offsetedOrigin = gameObject.transform.globalPosition + (notHitPoint - gameObject.transform.globalPosition).normalized * raycastOffset;
            InternalCalls.DrawRay(offsetedOrigin + Vector3.up, notHitPoint, new Vector3(0.0f, 1.0f, 0.0f));
        }

        if (raycastPoint != null)
        {
            Vector3 offsetedOrigin = gameObject.transform.globalPosition + (raycastPoint - gameObject.transform.globalPosition).normalized * raycastOffset;
            InternalCalls.DrawRay(offsetedOrigin + Vector3.up, raycastPoint, new Vector3(1.0f, 0.0f, 0.0f));
        }
        */
    }

    //All events from outside the stormtrooper
    private void ProcessExternalInput()
    {
        if (currentState != STATE.DIE && currentState != STATE.DASH)
        {
            if (Core.instance.gameObject == null)
                return;

            if (InRange(Core.instance.gameObject.transform.globalPosition, detectionRange))
            {
                inputsList.Add(INPUT.IN_PLAYER_IN_RANGE);
                LookAt(Core.instance.gameObject.transform.globalPosition);
            }
        }
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
                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            IdleEnd();
                            StartWander();
                            break;

                        case INPUT.IN_PLAYER_IN_RANGE:
                            currentState = STATE.SHOOT;
                            IdleEnd();
                            PlayerDetected();
                            StartShoot();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            IdleEnd();
                            StartDie();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            IdleEnd();
                            StartPush();
                            break;
                    }
                    break;

                case STATE.WANDER:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            WanderEnd();
                            StartIdle();
                            break;

                        case INPUT.IN_PLAYER_IN_RANGE:
                            currentState = STATE.SHOOT;
                            WanderEnd();
                            PlayerDetected();
                            StartShoot();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            WanderEnd();
                            StartDie();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            StartPush();
                            break;
                    }
                    break;

                case STATE.DASH:
                    switch (input)
                    {
                        case INPUT.IN_DASH_END:
                            currentState = STATE.IDLE;
                            DashEnd();
                            StartIdle();
                            break;

                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            DashEnd();
                            StartWander();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            DashEnd();
                            StartDie();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            DashEnd();
                            StartPush();
                            break;
                    }
                    break;

                case STATE.SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_DASH:
                            currentState = STATE.DASH;
                            StartDash();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;
                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            StartPush();
                            break;
                    }
                    break;
                case STATE.PUSHED:
                    switch (input)
                    {
                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            DashEnd();
                            StartIdle();
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
                UpdateIdle();
                break;
            case STATE.DASH:
                UpdateDash();
                break;
            case STATE.WANDER:
                UpdateWander();
                break;
            case STATE.SHOOT:
                UpdateShoot();
                break;
            case STATE.DIE:
                UpdateDie();
                break;
            case STATE.PUSHED:
                UpdatePush();
                break;
            default:
                break;
        }
    }

    #region IDLE
    private void StartIdle()
    {
        idleTimer = idleTime;
        Animator.Play(gameObject, "SK_Idle", speedMult);
        Animator.Play(blaster, "SK_Idle", speedMult);
        UpdateAnimationSpd(speedMult);



        Audio.PlayAudio(gameObject, "Play_Skytrooper_Jetpack_Loop");
    }

    private void UpdateIdle()
    {
        UpdateAnimationSpd(speedMult);

        if (gameObject.transform.globalPosition.y > initialHeight)
            gameObject.transform.localPosition.y--;
    }

    private void IdleEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region WANDER
    private void StartWander()
    {
        wanderTimer = wanderTime;

        Animator.Play(gameObject, "SK_Wander", speedMult);
        Animator.Play(blaster, "SK_Wander", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Skytrooper_Jetpack_Loop");

        targetPosition = CalculateNewPosition(wanderRange);

    }
    private void UpdateWander()
    {
        LookAt(targetPosition);

        //if (skill_slowDownActive)
        //    MoveToPosition(targetPosition, wanderSpeed * (1 - Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownAmount));
        //else 
        MoveToPosition(targetPosition, wanderSpeed * speedMult);

        UpdateAnimationSpd(speedMult);
    }
    private void WanderEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region DASH
    private void StartDash()
    {
        dashTimer = dashTime;

        targetPosition = CalculateRandomInRangePosition();

        if (!standStill)
        {
            Animator.Play(gameObject, "SK_Dash", speedMult);
            Animator.Play(blaster, "SK_Dash", speedMult);
            Audio.PlayAudio(gameObject, "Play_Skytrooper_Dash");
        }

        UpdateAnimationSpd(speedMult);
    }
    private void UpdateDash()
    {
        if (!standStill)
        {
            LookAt(targetPosition);
            MoveToPosition(targetPosition, dashSpeed * speedMult);
        }

        UpdateAnimationSpd(speedMult);
    }
    private void DashEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region SHOOT
    private void StartShoot()
    {
        shootTimer = timeBewteenShootingStates;
        shotsShooted = 0;
        Animator.Play(gameObject, "SK_Idle", speedMult);
        Animator.Play(blaster, "SK_Idle", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Skytrooper_Jetpack_Loop");
    }

    private void UpdateShoot()
    {
        shootTimer -= myDeltaTime;

        HandleShootAnimation();

        if (shootTimer <= 0.0f)
        {
            if (shotsShooted == maxShots)
            {
                inputsList.Add(INPUT.IN_DASH);
            }
            else
            {
                Shoot();
            }
        }
        UpdateAnimationSpd(speedMult);
    }

    private void HandleShootAnimation()
    {
        if (shootAnimTimer > 0.0f)
        {
            shootAnimTimer -= myDeltaTime;

            if (shootAnimTimer <= 0f)
            {
                Animator.Play(gameObject, "SK_Idle", speedMult);
                Animator.Play(blaster, "SK_Idle", speedMult);
                UpdateAnimationSpd(speedMult);
                shootAnimTimer = 0.0f;
            }
        }
    }

    private void Shoot()
    {
        //Check which bullet is available
        SkyTrooperShot bullet = null;
        if (primaryBullet != null && !primaryBullet.IsActive())
        {
            bullet = primaryBullet;
        }
        else if (secondaryBullet != null && !secondaryBullet.IsActive())
        {
            bullet = secondaryBullet;
        }

        if (bullet == null)
        {
            return;
        }

        bullet.Activate();

        bullet.gameObject.transform.localPosition = shootPoint.transform.globalPosition;
        bullet.gameObject.transform.localRotation = shootPoint.transform.globalRotation;

        //Calculate end position
        Vector2 player2DPosition = new Vector2(Core.instance.gameObject.transform.globalPosition.x, Core.instance.gameObject.transform.globalPosition.z);
        Vector2 randomPosition = Mathf.RandomPointAround(player2DPosition, 1);

        Vector3 projectileEndPosition = new Vector3(randomPosition.x, Core.instance.gameObject.transform.globalPosition.y, randomPosition.y);

        if (Core.instance.GetSate() == Core.STATE.DASH)
        {
            projectileEndPosition += Core.instance.gameObject.transform.GetForward().normalized * Core.instance.dashDistance;
        }

        bullet.SetTarget(projectileEndPosition, false);

        Animator.Play(gameObject, "SK_Shoot", speedMult);
        Animator.Play(blaster, "SK_Shoot", speedMult);
        shootAnimTimer = shootAnimationTime;

        Audio.PlayAudio(gameObject, "PLay_Skytrooper_Grenade_Launch");

        shotsShooted++;
        if (shotsShooted < maxShots)
            shootTimer = timeBewteenShots;
        else
        {
            shootTimer = timeBewteenShootingStates;
        }
    }
    private void PlayerDetected()
    {
        Audio.PlayAudio(gameObject, "Play_Enemy_Detection");
    }
    #endregion

    #region DIE
    private void StartDie()
    {
        dieTimer = dieTime;

        Explode();
        Audio.PlayAudio(gameObject, "Play_Skytrooper_Death");
        if (Core.instance != null)
            Audio.PlayAudio(Core.instance.gameObject, "Play_Mando_Kill_Voice");

        EnemyManager.RemoveEnemy(gameObject);

        //Combo
        if (PlayerResources.CheckBoon(BOONS.BOON_MASTER_YODA_FORCE))
        {
            HUD hud = Core.instance.hud.GetComponent<HUD>();

            if (hud != null)
                hud.AddToCombo(300, 1.0f);
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
        Counter.SumToCounterType(Counter.CounterTypes.ENEMY_SKYTROOPER);

        DropCoins();

        Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage(-PlayerHealth.healWhenKillingAnEnemy);

        //Explosion
        InternalCalls.Destroy(gameObject);
    }

    private void Explode()
    {
        if (meshObj != null)
        {
            MeshRenderer mesh = meshObj.GetComponent<MeshRenderer>();
            if (mesh != null)
            {
                mesh.active = false;
            }
        }

        if (blasterMeshObj != null)
        {
            MeshRenderer blasterMesh = blasterMeshObj.GetComponent<MeshRenderer>();
            if (blasterMesh != null)
            {
                blasterMesh.active = false;
            }
        }

        if (explosionParticles != null)
            explosionParticles.Play();

        if (Mathf.Distance(Core.instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition) <= explosionDistance)
        {
            PlayerHealth playerHealth = Core.instance.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage);
            }
        }
    }

    #endregion

    #region PUSH

    private void StartPush()
    {
        Vector3 force = pushDir.normalized;
        if (BabyYoda.instance != null)
        {
            force.y = 0f;
            force.x *= BabyYoda.instance.pushHorizontalForce;
            force.z *= BabyYoda.instance.pushHorizontalForce;
            gameObject.AddForce(force * forcePushMod);

            pushTimer = 0.0f;
        }

    }
    private void UpdatePush()
    {
        pushTimer += myDeltaTime;
        if (pushTimer >= PushStun)
            inputsList.Add(INPUT.IN_IDLE);

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
                if (Core.instance.HasStatus(STATUS_TYPE.PRIM_MOV_SPEED))
                    Core.instance.AddStatus(STATUS_TYPE.ACCELERATED, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, Core.instance.GetStatusData(STATUS_TYPE.PRIM_MOV_SPEED).severity / 100, 5, false);
                if (Core.instance.HasStatus(STATUS_TYPE.MANDO_QUICK_DRAW))
                    AddStatus(STATUS_TYPE.BLASTER_VULN, STATUS_APPLY_TYPE.ADDITIVE, Core.instance.GetStatusData(STATUS_TYPE.MANDO_QUICK_DRAW).severity / 100, 5);
            }
            BH_Bullet bullet = collidedGameObject.GetComponent<BH_Bullet>();

            if (bullet != null)
            {
                TakeDamage(bullet.GetDamage() * damageRecieveMult * BlasterVulnerability);

                Audio.PlayAudio(gameObject, "Play_Stormtrooper_Hit");

                if (Core.instance.hud != null && currentState != STATE.DIE)
                {
                    HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                    if (hudComponent != null)
                        hudComponent.AddToCombo(25, 0.95f);
                }
            }
        }
        else if (collidedGameObject.CompareTag("ChargeBullet"))
        {
            ChargedBullet bullet = collidedGameObject.GetComponent<ChargedBullet>();
            Audio.PlayAudio(gameObject, "Play_Sniper_Hit");
            if (bullet != null && currentState != STATE.DIE)
            {

                if (sniperHitParticle != null)
                    sniperHitParticle.Play();

                Audio.PlayAudio(gameObject, "Play_Stormtrooper_Hit");

                if (Core.instance.hud != null && currentState != STATE.DIE)
                {
                    Core.instance.hud.GetComponent<HUD>().AddToCombo(55, 0.25f);
                }
                {
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
                    }
                    this.AddStatus(STATUS_TYPE.ENEMY_VULNERABLE, applyType, vulerableSev, vulerableTime);

                    TakeDamage(bullet.GetDamage() * damageMult);

                    if (Core.instance != null && healthPoints <= 0f)
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
                        if (Core.instance.HasStatus(STATUS_TYPE.CROSS_HAIR_LUCKY_SHOT))
                        {
                            float mod = Core.instance.GetStatusData(STATUS_TYPE.CROSS_HAIR_LUCKY_SHOT).severity;
                            Random rand = new Random();
                            float result = rand.Next(1, 101);
                            if (result <= mod)
                                Core.instance.RefillSniper();

                            Core.instance.luckyMod = 1 + mod / 100;
                        }

                        if (Core.instance.HasStatus(STATUS_TYPE.AHSOKA_DET))
                        {
                            Core.instance.RefillSniper();
                        }
                    }
                }
            }
        }
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("PushSkill") && currentState != STATE.PUSHED && currentState != STATE.DIE)
        {
            pushDir = triggeredGameObject.transform.GetForward();
            inputsList.Add(INPUT.IN_PUSHED);

            if (Core.instance != null && currentState != STATE.DIE)
            {
                HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                if (hudComponent != null)
                    hudComponent.AddToCombo(10, 0.35f);
            }

        }
    }

    public void OnTriggerExit(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("PushSkill") && currentState != STATE.PUSHED && currentState != STATE.DIE)
        {
            if (Core.instance != null)
            {
                pushDir = triggeredGameObject.transform.GetForward();
                inputsList.Add(INPUT.IN_PUSHED);
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        Audio.PlayAudio(gameObject, "Play_Skytrooper_Hit");
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
        hitParticles.Play();
        if (currentState != STATE.DIE)
        {
            if (healthPoints <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DIE);
                if (Core.instance != null)
                {
                    if (Core.instance.HasStatus(STATUS_TYPE.WINDU_FORCE))
                        BabyYoda.instance.SetCurrentForce(BabyYoda.instance.GetCurrentForce() + (int)(Core.instance.GetStatusData(STATUS_TYPE.WINDU_FORCE).severity));
                }
            }
        }
    }

    private Vector3 CalculateRandomInRangePosition()
    {
        if (Core.instance == null)
            return null;

        Vector3 newPosition = null;

        //Raycast Debug
        //notHitPoint = null;
        //raycastPoint = null;

        if (Mathf.Distance(Core.instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition) < detectionRange * 0.5f)
        {
            newPosition = GoBackwards();

            if (newPosition == null)
                newPosition = GoForward();
        }
        else if (Mathf.Distance(Core.instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition) < detectionRange)
        {
            newPosition = GoForward();

            if (newPosition == null)
                newPosition = GoBackwards();
        }

        if (newPosition != null)
        {
            standStill = false;
            return newPosition;
        }
        else
        {
            standStill = true;
            //Debug.Log("Stand still");
            return gameObject.transform.globalPosition;
        }
    }

    private void UpdateAnimationSpd(float newSpd)
    {
        if (currAnimationPlaySpd != newSpd)
        {
            Animator.SetSpeed(gameObject, newSpd);
            Animator.SetSpeed(blaster, newSpd);
            currAnimationPlaySpd = newSpd;
        }
    }

    private Vector3 GoForward()
    {
        Random randomizer = new Random();
        int randomDirection = randomizer.Next(2);

        //Debug.Log("Un pasito palante, María");

        Vector3 desiredDirection = null;
        GameObject leftObject;
        GameObject rightObject;

        Random randomAngle = new Random();
        float leftAngle = -50 + randomAngle.Next(-5, 5);
        float rightAngle = 50 + randomAngle.Next(-5, 5);

        Vector3 centerDirection = Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition;

        if (randomDirection == 0) //First check left
        {
            leftObject = FindObjectInAngle(leftAngle, centerDirection, ref desiredDirection);

            if (leftObject != null)
            {
                rightObject = FindObjectInAngle(rightAngle, centerDirection, ref desiredDirection);

                if (rightObject == null)
                {
                    return gameObject.transform.globalPosition + desiredDirection.normalized * dashRange;
                }
            }
            else
            {
                return gameObject.transform.globalPosition + desiredDirection.normalized * dashRange;
            }
        }
        else //First check right
        {
            rightObject = FindObjectInAngle(rightAngle, centerDirection, ref desiredDirection);

            if (rightObject != null)
            {
                leftObject = FindObjectInAngle(leftAngle, centerDirection, ref desiredDirection);

                if (leftObject == null)
                {
                    return gameObject.transform.globalPosition + desiredDirection.normalized * dashRange;
                }
            }
            else
            {
                return gameObject.transform.globalPosition + desiredDirection.normalized * dashRange;
            }

        }

        return null;
    }

    private Vector3 GoBackwards()
    {
        Random randomizer = new Random();
        int randomDirection = randomizer.Next(2);

        //Debug.Log("Un pasito patrás");

        Vector3 desiredDirection = null;
        GameObject leftObject;
        GameObject rightObject;

        Random randomAngle = new Random();
        float leftAngle = 220 + randomAngle.Next(-5, 5);
        float rightAngle = 140 + randomAngle.Next(-5, 5);

        Vector3 centerDirection = Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition;

        if (randomDirection == 0) //First check left
        {
            leftObject = FindObjectInAngle(leftAngle, centerDirection, ref desiredDirection);

            if (leftObject != null)
            {
                rightObject = FindObjectInAngle(rightAngle, centerDirection, ref desiredDirection);

                if (rightObject == null)
                {
                    return gameObject.transform.globalPosition + desiredDirection.normalized * dashRange;
                }
            }
            else
            {
                return gameObject.transform.globalPosition + desiredDirection.normalized * dashRange;
            }
        }
        else //First check right
        {
            rightObject = FindObjectInAngle(rightAngle, centerDirection, ref desiredDirection);

            if (rightObject != null)
            {
                leftObject = FindObjectInAngle(leftAngle, centerDirection, ref desiredDirection);

                if (leftObject == null)
                {
                    return gameObject.transform.globalPosition + desiredDirection.normalized * dashRange;
                }
            }
            else
            {
                return gameObject.transform.globalPosition + desiredDirection.normalized * dashRange;
            }

        }

        return null;
    }

    GameObject FindObjectInAngle(float angle, Vector3 centerDirection, ref Vector3 desiredDirection)
    {
        desiredDirection = new Vector3((float)(Math.Cos(angle * Mathf.Deg2RRad) * centerDirection.normalized.x - Math.Sin(angle * Mathf.Deg2RRad) * centerDirection.normalized.z),
                                       0.0f,
                                       (float)(Math.Sin(angle * Mathf.Deg2RRad) * centerDirection.normalized.x + Math.Cos(angle * Mathf.Deg2RRad) * centerDirection.normalized.z));

        desiredDirection = desiredDirection.normalized;

        float hitDistance = 0.0f;
        Vector3 up = new Vector3(0.0f, 1.0f, 0.0f);
        GameObject hitObject = InternalCalls.RayCast(gameObject.transform.globalPosition + up + (desiredDirection.normalized * raycastOffset), desiredDirection, dashRange, ref hitDistance);

        if (hitObject == null)
        {
            //InternalCalls.DrawRay(gameObject.transform.globalPosition + up + (desiredDirection.normalized * raycastOffset),
            //                      gameObject.transform.globalPosition + up + (desiredDirection.normalized * raycastOffset) + (desiredDirection * dashRange),
            //                      new Vector3(1.0f, 1.0f, 1.0f));

            //notHitPoint = gameObject.transform.globalPosition + up + desiredDirection.normalized * raycastOffset + desiredDirection * dashRange;
            //Debug.Log("Null object");
        }
        else
        {
            //InternalCalls.DrawRay(gameObject.transform.globalPosition + up + desiredDirection.normalized * raycastOffset,
            //                      gameObject.transform.globalPosition + up + desiredDirection.normalized * raycastOffset + desiredDirection * hitDistance,
            //                      new Vector3(1.0f, 1.0f, 0.0f));

            //raycastPoint = gameObject.transform.globalPosition + up + desiredDirection.normalized * raycastOffset + desiredDirection * hitDistance;
            //Debug.Log("Raycasted object");
        }

        return hitObject;
    }

    public override void PlayGrenadeHitParticles()
    {
        if (grenadeHitParticle != null)
            grenadeHitParticle.Play();
    }

    public void OnDestroy()
    {
        if (primaryBulletObj != null)
        {
            InternalCalls.Destroy(primaryBulletObj);
        }

        if (secondaryBulletObj != null)
        {
            InternalCalls.Destroy(secondaryBulletObj);
        }
    }
}