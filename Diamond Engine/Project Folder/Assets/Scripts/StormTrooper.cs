using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class StormTrooper : Enemy
{
    enum STATE : int
    {
        NONE = -1,
        IDLE,
        WANDER,
        RUN,
        PUSHED,
        FIND_AIM,
        SHOOT,
        HIT,
        DIE
    }

    enum INPUT : int
    {
        IN_IDLE,
        IN_IDLE_END,
        IN_RUN,
        IN_RUN_END,
        IN_WANDER,
        IN_PUSHED,
        IN_FIND_AIM,
        IN_SHOOT,
        IN_HIT,
        IN_DIE,
        IN_PLAYER_IN_RANGE,
        IN_PLAYER_SHOOTABLE
    }

    //State
    private STATE currentState = STATE.NONE;

    private List<INPUT> inputsList = new List<INPUT>();

    public GameObject shootPoint = null;
    public GameObject blaster = null;

    //Action times
    public float idleTime = 5.0f;
    private float dieTime = 3.0f;
    public float timeBetweenShots = 0.5f;
    public float timeBetweenSequences = 0.5f;
    public float timeBetweenStates = 1.5f;
    //private float reAimTime = 0.5f;

    //Speeds
    public float wanderSpeed = 3.5f;
    public float runningSpeed = 7.5f;
    public float bulletSpeed = 10.0f;
    private bool skill_slowDownActive = false;
    private float currAnimationPlaySpd = 1f;

    //Ranges
    public float wanderRange = 7.5f;
    public float runningRange = 12.5f;
    public float avoidRange = 15f;

    //Timers
    private float idleTimer = 0.0f;
    private float shotTimer = 0.0f;
    private float sequenceTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float statesTimer = 0.0f;
    private float pushTimer = 0.0f;
    //private float reAimTimer = 0.0f;
    private float skill_slowDownTimer = 0.0f;

    //Action variables
    int shotTimes = 0;
    public int maxShots = 2;
    private int shotSequences = 0;
    public int maxSequences = 2;
    private bool shooting = false;
    private bool needFindAim = false;
    public float pushTime = 0f;

    //force
    public float forcePushMod = 1;

    //Death point
    public GameObject deathPoint = null;

    private StormTrooperParticles myParticles = null;
    public void Awake()
    {
        InitEntity(ENTITY_TYPE.STROMTROOPER);
        EnemyManager.AddEnemy(gameObject);

        agent = gameObject.GetComponent<NavMeshAgent>();
        targetPosition = null;

        currentState = STATE.IDLE;

        Animator.Play(gameObject, "ST_Idle", 1.0f);

        if (blaster != null)
            Animator.Play(blaster, "ST_Idle", 1.0f);

        UpdateAnimationSpd(1.0f);

        shotTimes = 0;
        shotSequences = 0;

        idleTimer = idleTime;
        dieTime = Animator.GetAnimationDuration(gameObject, "ST_Die");
        timeBetweenShots = Animator.GetAnimationDuration(gameObject, "ST_Shoot");

        myParticles = gameObject.GetComponent<StormTrooperParticles>();
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

        if (currentState == STATE.RUN || currentState == STATE.WANDER)
        {
            if (agent != null && Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance)
            {
                inputsList.Add(INPUT.IN_IDLE);
            }
        }

        if (currentState == STATE.FIND_AIM)
        {
            if (agent != null && Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance)
            {
                inputsList.Add(INPUT.IN_SHOOT);
            }
        }
    }

    //All events from outside the stormtrooper
    private void ProcessExternalInput()
    {
        if (currentState != STATE.DIE && currentState != STATE.RUN)
        {
            if (Core.instance != null && InRange(Core.instance.gameObject.transform.globalPosition, detectionRange))
            {
                inputsList.Add(INPUT.IN_PLAYER_IN_RANGE);
            }
        }

        if (currentState == STATE.RUN && needFindAim)
        {
            inputsList.Add(INPUT.IN_FIND_AIM);
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
                            StartWander();
                            break;

                        case INPUT.IN_PLAYER_IN_RANGE:
                            currentState = STATE.SHOOT;
                            PlayerDetected();
                            StartShoot();
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
                            WanderEnd();
                            StartPush();
                            break;
                    }
                    break;

                case STATE.RUN:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            RunEnd();
                            StartIdle();
                            break;

                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            RunEnd();
                            StartWander();
                            break;

                        case INPUT.IN_FIND_AIM:
                            currentState = STATE.FIND_AIM;
                            RunEnd();
                            StartFindAim();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            RunEnd();
                            StartDie();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            RunEnd();
                            StartPush();
                            break;
                    }
                    break;

                case STATE.FIND_AIM:
                    switch (input)
                    {
                        case INPUT.IN_SHOOT:
                            currentState = STATE.SHOOT;
                            FindAimEnd();
                            StartShoot();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            FindAimEnd();
                            StartPush();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            FindAimEnd();
                            StartDie();
                            break;
                    }
                    break;

                case STATE.SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_RUN:
                            currentState = STATE.RUN;
                            StartRun();
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
            case STATE.RUN:
                UpdateRun();
                break;
            case STATE.WANDER:
                UpdateWander();
                break;
            case STATE.FIND_AIM:
                UpdateFindAim();
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
        Animator.Play(gameObject, "ST_Idle", speedMult);

        if (blaster != null)
            Animator.Play(blaster, "ST_Idle", speedMult);

        UpdateAnimationSpd(speedMult);

    }

    private void UpdateIdle()
    {
        UpdateAnimationSpd(speedMult);
    }

    #endregion

    #region WANDER
    private void StartWander()
    {
        if (agent != null)
            agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);

        Animator.Play(gameObject, "ST_Run", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "ST_Idle", speedMult);

        UpdateAnimationSpd(speedMult);

        Audio.PlayAudio(gameObject, "Play_Footsteps_Stormtrooper");
    }
    private void UpdateWander()
    {
        if (agent != null)
        {
            LookAt(agent.GetDestination());
            agent.MoveToCalculatedPos(wanderSpeed * speedMult);
        }

        UpdateAnimationSpd(speedMult);

    }
    private void WanderEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region RUN
    private void StartRun()
    {
        if (agent != null)
        {
            agent.CalculateRandomPath(gameObject.transform.globalPosition, runningRange);
        }

        Animator.Play(gameObject, "ST_Run", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "ST_Run", speedMult);

        UpdateAnimationSpd(speedMult);

        Audio.PlayAudio(gameObject, "Play_Footsteps_Stormtrooper");
    }

    private void UpdateRun()
    {
        if (agent != null)
        {
            LookAt(agent.GetDestination());
            agent.MoveToCalculatedPos(runningSpeed * speedMult);
        }

        UpdateAnimationSpd(speedMult);
    }
    private void RunEnd()
    {
        Audio.StopAudio(gameObject);

        PlayerIsShootable();
    }
    #endregion

    #region FIND_AIM

    private void StartFindAim()
    {
        Animator.Play(gameObject, "ST_Run", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "ST_Run", speedMult);

        UpdateAnimationSpd(speedMult);

        Audio.PlayAudio(gameObject, "Play_Footsteps_Stormtrooper");

        if (agent != null && Core.instance != null && agent.GetPathSize() >= 2)
        {
            agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
            Vector3 directionFindAim = new Vector3(agent.GetPointAt(1) - gameObject.transform.globalPosition);
            //agent.ClearPath();
            targetPosition = directionFindAim.normalized * runningRange + gameObject.transform.globalPosition;
            targetPosition.y = gameObject.transform.globalPosition.y;
            agent.CalculatePath(gameObject.transform.globalPosition, targetPosition);
        }
    }

    private void UpdateFindAim()
    {
        if (agent != null)
        {
            LookAt(agent.GetDestination());
            agent.MoveToCalculatedPos(runningSpeed * speedMult);
        }

        UpdateAnimationSpd(speedMult);
    }
    private void FindAimEnd()
    {
        Audio.StopAudio(gameObject);

        PlayerIsShootable();
    }
    #endregion

    #region SHOOT
    private void StartShoot()
    {
        statesTimer = timeBetweenStates;
        Animator.Play(gameObject, "ST_Idle", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "ST_Idle", speedMult);
        UpdateAnimationSpd(speedMult);
    }

    private void UpdateShoot()
    {
        if (statesTimer > 0.0f)
        {
            statesTimer -= myDeltaTime;

            if (statesTimer <= 0.0f)
            {
                //First Timer
                if (shotSequences == 0)
                {
                    //First Shot
                    Shoot();
                    shotTimer = timeBetweenShots;
                }
                //Second Timer
                else
                {
                    //Reboot times
                    shooting = false;
                    shotTimes = 0;
                    shotSequences = 0;
                    inputsList.Add(INPUT.IN_RUN);
                }
            }
        }

        if (shotTimer > 0.0f)
        {
            shotTimer -= myDeltaTime;

            if (shotTimer <= 0.0f)
            {
                Shoot();

                if (shotTimes >= maxShots)
                {
                    shotSequences++;

                    Animator.Play(gameObject, "ST_Idle", speedMult);
                    if (blaster != null)
                        Animator.Play(blaster, "ST_Idle", speedMult);
                    UpdateAnimationSpd(speedMult);

                    //End of second shot of the first sequence
                    if (shotSequences < maxSequences)
                    {
                        //Start of pause between sequences
                        sequenceTimer = timeBetweenSequences;
                        shotTimes = 0;
                        shooting = false;
                    }
                    //End of second shot of the second sequence
                    else
                    {
                        statesTimer = timeBetweenStates;
                        shooting = false;
                    }
                }
            }
        }

        if (sequenceTimer > 0.0f)
        {
            sequenceTimer -= myDeltaTime;

            if (sequenceTimer <= 0.0f)
            {
                Shoot();
                shotTimer = timeBetweenShots;
                //shooting = true;
            }
        }

        if (!shooting && Core.instance != null)
        {
            LookAt(Core.instance.gameObject.transform.globalPosition);
        }

        UpdateAnimationSpd(speedMult);
    }

    private void Shoot()
    {
        shooting = true;

        GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/1635392825.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, shootPoint.transform.globalScale);

        if (bullet != null)
        {
            BH_Bullet bulletScript = bullet.GetComponent<BH_Bullet>();

            if (bulletScript != null)
            {
                bulletScript.damage = damage;
                bulletScript.SetGameObjectToAvoid(this.gameObject);
            }
        }

        Animator.Play(gameObject, "ST_Shoot", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "ST_Shoot", speedMult);

        Audio.PlayAudio(gameObject, "PLay_Blaster_Stormtrooper");
        UpdateAnimationSpd(speedMult);
        shotTimes++;
    }
    private void PlayerDetected()
    {
        Audio.PlayAudio(gameObject, "Play_Enemy_Detection");
        if (myParticles != null && myParticles.alert != null)
        {
            myParticles.alert.Play();
        }
    }
    #endregion

    #region DIE
    private void StartDie()
    {
        EnemyManager.RemoveEnemy(gameObject);
        dieTimer = dieTime;

        Animator.Play(gameObject, "ST_Die", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "ST_Die", speedMult);
        UpdateAnimationSpd(speedMult);

        Audio.PlayAudio(gameObject, "Play_Stormtrooper_Death");

        if (Core.instance != null)
        {
            Audio.PlayAudio(Core.instance.gameObject, "Play_Mando_Kill_Voice");

            //Combo
            if (PlayerResources.CheckBoon(BOONS.BOON_MASTER_YODA_FORCE) && Core.instance.hud != null)
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(300, 1.0f);
            }
        }

    }
    private void UpdateDie()
    {
        if (dieTimer > 0.0f)
        {
            dieTimer -= Time.deltaTime;

            if (dieTimer <= 0.0f)
            {
                Die();
            }
        }

        UpdateAnimationSpd(speedMult);
    }

    private void Die()
    {
        Counter.SumToCounterType(Counter.CounterTypes.ENEMY_STORMTROOPER);

        float dist = (deathPoint.transform.globalPosition - gameObject.transform.globalPosition).magnitude;
        Vector3 forward = gameObject.transform.GetForward();
        forward = forward.normalized * (-dist);

        if (Core.instance != null)
        {
            PlayerHealth playerHealth = Core.instance.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(-PlayerHealth.healWhenKillingAnEnemy);
            }
        }

        InternalCalls.CreatePrefab("Library/Prefabs/230945350.prefab", new Vector3(gameObject.transform.globalPosition.x + forward.x, gameObject.transform.globalPosition.y, gameObject.transform.globalPosition.z + forward.z), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f));
        DropCoins();

        InternalCalls.Destroy(gameObject);
    }

    #endregion

    #region PUSH
    private void StartPush()
    {
        Vector3 force = pushDir.normalized;
        if (BabyYoda.instance != null)
        {
            force.y  = BabyYoda.instance.pushVerticalForce * forcePushMod;
            force.x *= BabyYoda.instance.pushHorizontalForce;
            force.z *= BabyYoda.instance.pushHorizontalForce;
            gameObject.AddForce(force * forcePushMod);

            pushTimer = 0.0f;
        }
    }
    private void UpdatePush()
    {
        pushTimer += Time.deltaTime;
        if (pushTimer >= pushTime)
            inputsList.Add(INPUT.IN_IDLE);
    }
    #endregion

    private bool PlayerIsShootable()
    {
        float distance = detectionRange;
        float hitDistance = detectionRange;

        if (Core.instance == null)
        {
            return false;
        }

        GameObject raycastHit = InternalCalls.RayCast(shootPoint.transform.globalPosition, (Core.instance.gameObject.transform.globalPosition - shootPoint.transform.globalPosition).normalized, distance, ref hitDistance);

        if (raycastHit != null)
        {
            if (raycastHit.CompareTag("Player"))
            {
                needFindAim = false;
                return true;
            }

        }

        needFindAim = true;
        return false;
    }

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
                TakeDamage(bullet.GetDamage() * damageRecieveMult * BlasterVulnerability);

            Audio.PlayAudio(gameObject, "Play_Stormtrooper_Hit");

            if (Core.instance != null && Core.instance.hud != null && currentState != STATE.DIE)
            {
                HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                if (hudComponent != null)
                    hudComponent.AddToCombo(25, 0.95f);
            }

            if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.PRIMARY_SLOW_SPEED))
            {
                skill_slowDownActive = true;
                skill_slowDownTimer = 0.0f;
            }
        }
        else if (collidedGameObject.CompareTag("ChargeBullet"))
        {
            ChargedBullet bullet = collidedGameObject.GetComponent<ChargedBullet>();
            Audio.PlayAudio(gameObject, "Play_Sniper_Hit");
            if (bullet != null && currentState != STATE.DIE)
            {
                if (myParticles != null && myParticles.sniperHit != null)
                    myParticles.sniperHit.Play();

                Audio.PlayAudio(gameObject, "Play_Stormtrooper_Hit");

                if (Core.instance != null && Core.instance.hud != null && currentState != STATE.DIE)
                {
                    HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                    if (hudComponent != null)
                        hudComponent.AddToCombo(55, 0.25f);
                }


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
        else if (collidedGameObject.CompareTag("ExplosiveBarrel") && collidedGameObject.GetComponent<SphereCollider>() != null && collidedGameObject.GetComponent<SphereCollider>().active)
        {
            BH_DestructBox explosion = collidedGameObject.GetComponent<BH_DestructBox>();

            if (explosion != null)
            {
                healthPoints -= explosion.explosion_damage * 2;
                if (currentState != STATE.DIE && healthPoints <= 0.0f)
                    inputsList.Add(INPUT.IN_DIE);
            }

            if (Core.instance != null && currentState != STATE.DIE)
            {
                if (Core.instance.hud != null)
                {
                    HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                    if (hudComponent != null)
                        hudComponent.AddToCombo(33, 0.65f);
                }
            }
        }
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("PushSkill") && currentState != STATE.PUSHED && currentState != STATE.DIE)
        {
            if (Core.instance != null && currentState != STATE.DIE)
            {
                pushDir = triggeredGameObject.transform.GetForward();
                inputsList.Add(INPUT.IN_PUSHED);

                if (Core.instance != null)
                {
                    HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                    if (hudComponent != null)
                        hudComponent.AddToCombo(10, 0.35f);
                }
            }
        }
    }

    public void OnTriggerExit(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("PushSkill") && currentState != STATE.PUSHED && currentState != STATE.DIE)
        {
            if (Core.instance != null && currentState != STATE.DIE)
            {
                pushDir = triggeredGameObject.transform.GetForward();
                inputsList.Add(INPUT.IN_PUSHED);

                if (Core.instance != null)
                {
                    HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                    if (hudComponent != null)
                        hudComponent.AddToCombo(10, 0.35f);
                }
            }
        }
    }


    public override void TakeDamage(float damage)
    {
        if (currentState != STATE.DIE)
        {
            if (myParticles != null && myParticles.hit != null)
                myParticles.hit.Play();

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

            if (healthPoints <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DIE);
                if (Core.instance != null && BabyYoda.instance != null)
                {
                    if (Core.instance.HasStatus(STATUS_TYPE.WINDU_FORCE))
                    {
                        BabyYoda.instance.SetCurrentForce(BabyYoda.instance.GetCurrentForce() + (int)(Core.instance.GetStatusData(STATUS_TYPE.WINDU_FORCE).severity));
                    }
                }
            }
        }
    }
    private void UpdateAnimationSpd(float newSpd)
    {
        if (currAnimationPlaySpd != newSpd)
        {
            Animator.SetSpeed(gameObject, newSpd);
            if (blaster != null)
                Animator.SetSpeed(blaster, newSpd);
            currAnimationPlaySpd = newSpd;
        }
    }

    public override void PlayGrenadeHitParticles()
    {
        if (myParticles != null && myParticles.grenadeHit != null)
            myParticles.grenadeHit.Play();
    }

}