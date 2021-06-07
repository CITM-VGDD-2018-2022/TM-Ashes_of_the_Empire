using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class HeavyTrooper : Enemy
{
    enum STATE : int
    {
        NONE = -1,
        IDLE,
        WANDER,
        RUN,
        LOADING_DASH,
        DASH,
        SWEEP,
        TIRED,
        PUSHED,
        HIT,
        DIE
    }

    enum INPUT : int
    {
        IN_IDLE,
        IN_RUN,
        IN_LOADING_DASH,
        IN_DASH,
        IN_DASH_END,
        IN_WANDER,
        IN_PUSHED,
        IN_SWEEP,
        IN_SWEEP_END,
        IN_HIT,
        IN_DIE,
        IN_DIE_END,
        IN_DASH_RANGE,
        IN_PLAYER_IN_RANGE
    }

    //State
    private STATE currentState = STATE.NONE;

    private List<INPUT> inputsList = new List<INPUT>();

    public GameObject spear = null;
    private BoxCollider spearCollider = null;

    private bool straightPath = false;

    //Action times
    public float idleTime = 5.0f;
    private float dieTime = 3.0f;
    private float sweepTime = 0.0f;
    public float tiredTime = 2.0f;
    public float loadingTime = 2.0f;
    public float directionDecisionTime = 0.8f;
    public float directionSweepDecisionTime = 0.8f;

    //Speeds
    public float wanderSpeed = 3.5f;
    public float runningSpeed = 7.5f;
    public float dashSpeed = 60.0f;
    public float dashSpeedReduction = 0.3f;
    private bool skill_slowDownActive = false;

    //Ranges
    public float wanderRange = 7.5f;
    public float sweepRange = 3.0f;
    public float attackRange = 5.0f;
    public float dashLength = 5.0f;

    //Timers
    private float idleTimer = 0.0f;
    private float dashTimer = 0.0f;
    private float sweepTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float tiredTimer = 0.0f;
    private float loadingTimer = 0.0f;
    private float directionDecisionTimer = 0.0f;
    private float directionSweepDecisionTimer = 0.0f;
    private float skill_slowDownTimer = 0.0f;
    private float pushTimer = 0.0f;
    private float currAnimationPlaySpd = 1f;

    int doneDashes = 0;
    private bool canSweep = true;

    //force
    public float forcePushMod = 1;

    //Particles
    private HeavyTrooperParticles particles = null;

    public void Awake()
    {
        InitEntity(ENTITY_TYPE.HEAVYTROOPER);
        EnemyManager.AddEnemy(gameObject);

        StartIdle();
        agent = gameObject.GetComponent<NavMeshAgent>();
        targetPosition = null;

        currentState = STATE.IDLE;

        if (spear != null)
        {
            HeavyTrooperSpear heavyTrooperSpear = spear.GetComponent<HeavyTrooperSpear>();

            heavyTrooperSpear.damage = (int)damage;

            spearCollider = spear.GetComponent<BoxCollider>();
        }

        loadingTime = Animator.GetAnimationDuration(gameObject, "HVY_Dash_P1");
        sweepTime = Animator.GetAnimationDuration(gameObject, "HVY_Sweep");
        dieTime = Animator.GetAnimationDuration(gameObject, "HVY_Die");

        particles = gameObject.GetComponent<HeavyTrooperParticles>();
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

            if (idleTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_WANDER);
            }
        }

        if (dashTimer > 0.0f)
        {
            dashTimer -= myDeltaTime;

            if (targetPosition == null)
                targetPosition = Core.instance.gameObject.transform.globalPosition;

            if (dashTimer < 0.0f)
            {
                if (Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) < sweepRange || doneDashes >= 2)
                {
                    inputsList.Add(INPUT.IN_SWEEP);
                }
                else
                {
                    inputsList.Add(INPUT.IN_RUN);
                }
            }
        }

        if (currentState == STATE.SWEEP && sweepTimer > 0.0f)
        {
            sweepTimer -= myDeltaTime;

            if (sweepTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_SWEEP_END);
            }
        }

        if (loadingTimer > 0.0f)
        {
            loadingTimer -= myDeltaTime;

            if (loadingTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_DASH);
            }
        }

        if (tiredTimer > 0.0f)
        {
            tiredTimer -= myDeltaTime;

            if (tiredTimer < 0.0f)
            {
                if (InRange(Core.instance.gameObject.transform.globalPosition, sweepRange)
                    && Vector3.Dot(gameObject.transform.GetForward(), Core.instance.gameObject.transform.globalPosition) > 0.9f)
                    inputsList.Add(INPUT.IN_SWEEP);
                else
                    inputsList.Add(INPUT.IN_RUN);
            }
        }
    }

    //All events from outside the heavytrooper
    private void ProcessExternalInput()
    {
        if (currentState != STATE.DIE && currentState != STATE.DASH)
        {
            if (InRange(Core.instance.gameObject.transform.globalPosition, sweepRange) && tiredTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_SWEEP);
            }
            else if (InRange(Core.instance.gameObject.transform.globalPosition, attackRange) && straightPath)
            {
                inputsList.Add(INPUT.IN_DASH_RANGE);
            }

            if (InRange(Core.instance.gameObject.transform.globalPosition, detectionRange)
                && agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition))
            {
                inputsList.Add(INPUT.IN_PLAYER_IN_RANGE);
            }
        }

        if (currentState == STATE.RUN)
        {
            if (InRange(Core.instance.gameObject.transform.globalPosition, sweepRange))
            {
                inputsList.Add(INPUT.IN_SWEEP);
            }

            if (!InRange(Core.instance.gameObject.transform.globalPosition, detectionRange))
            {
                inputsList.Add(INPUT.IN_WANDER);
            }
        }
    }

    private void ProcessState()
    {
        while (inputsList.Count > 0)
        {
            INPUT input = inputsList[0];

            switch (currentState)
            {
                case STATE.NONE:
                    Debug.Log("HEAVYTROOPER ERROR STATE");
                    break;

                case STATE.IDLE:
                    switch (input)
                    {
                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartWander();
                            break;

                        case INPUT.IN_PLAYER_IN_RANGE:
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

                case STATE.WANDER:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            WanderEnd();
                            StartIdle();
                            break;

                        case INPUT.IN_PLAYER_IN_RANGE:
                            currentState = STATE.RUN;
                            WanderEnd();
                            StartRun();
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

                        case INPUT.IN_DASH_RANGE:
                            currentState = STATE.LOADING_DASH;
                            RunEnd();
                            StartLoading();
                            break;

                        case INPUT.IN_SWEEP:
                            currentState = STATE.SWEEP;
                            RunEnd();
                            StartSweep();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            RunEnd();
                            StartDie();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            StartPush();
                            break;
                    }
                    break;

                case STATE.LOADING_DASH:
                    switch (input)
                    {
                        case INPUT.IN_DASH:
                            currentState = STATE.DASH;
                            StartDash();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            StartPush();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;
                    }
                    break;

                case STATE.DASH:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            StartIdle();
                            break;

                        case INPUT.IN_RUN:
                            currentState = STATE.RUN;
                            StartRun();
                            break;

                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartWander();
                            break;

                        case INPUT.IN_LOADING_DASH:
                            currentState = STATE.LOADING_DASH;
                            StartLoading();
                            break;

                        case INPUT.IN_DASH:
                            StartDash();
                            break;

                        case INPUT.IN_SWEEP:
                            currentState = STATE.SWEEP;
                            StartSweep();
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

                case STATE.SWEEP:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            StartIdle();
                            break;

                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartWander();
                            break;

                        case INPUT.IN_SWEEP_END:
                            currentState = STATE.TIRED;
                            StartTired();
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

                case STATE.TIRED:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            StartIdle();
                            break;

                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartWander();
                            break;

                        case INPUT.IN_SWEEP:
                            currentState = STATE.SWEEP;
                            StartSweep();
                            break;

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
                    Debug.Log("NEED TO ADD STATE TO HEAVYTROOPER SWITCH");
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
            case STATE.LOADING_DASH:
                UpdateLoading();
                break;
            case STATE.DASH:
                UpdateDash();
                break;
            case STATE.SWEEP:
                UpdateSweep();
                break;
            case STATE.TIRED:
                UpdateTired();
                break;
            case STATE.DIE:
                UpdateDie();
                break;
            case STATE.PUSHED:
                UpdatePush();
                break;
            default:
                Debug.Log("NEED TO ADD STATE TO HEAVYTROOPER");
                break;
        }
    }

    #region IDLE
    private void StartIdle()
    {
        //Debug.Log("HEAVYTROOPER IDLE");
        idleTimer = idleTime;
        Animator.Play(gameObject, "HVY_Idle", speedMult);

        if (spear != null)
            Animator.Play(spear, "HVY_Idle", speedMult);

        UpdateAnimationSpd(speedMult);
    }

    private void UpdateIdle()
    {
        UpdateAnimationSpd(speedMult);
    }

    #endregion

    #region RUN
    private void StartRun()
    {
        //Debug.Log("HEAVYTROOPER RUN");
        Animator.Play(gameObject, "HVY_Run", speedMult);
        if (spear != null)
            Animator.Play(spear, "HVY_Run", speedMult);

        Audio.PlayAudio(gameObject, "Play_Heavytrooper_Run");

        UpdateAnimationSpd(speedMult);
    }
    private void UpdateRun()
    {
        if (!agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition))
        {
            inputsList.Add(INPUT.IN_WANDER);
            return;
        }

        LookAt(agent.GetDestination());

        agent.MoveToCalculatedPos(runningSpeed * speedMult);

        StraightPath();

        UpdateAnimationSpd(speedMult);
    }
    private void RunEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region WANDER
    private void StartWander()
    {
        //Debug.Log("HEAVYTROOPER WANDER");
        agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);

        Animator.Play(gameObject, "HVY_Wander", speedMult);
        if (spear != null)
            Animator.Play(spear, "HVY_Wander", speedMult);

        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Heavytrooper_Wander");
    }
    private void UpdateWander()
    {
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(wanderSpeed * speedMult);

        if (Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance)
            inputsList.Add(INPUT.IN_IDLE);

        UpdateAnimationSpd(speedMult);
    }
    private void WanderEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region LOADING_ATTACK
    private void StartLoading()
    {
        //Debug.Log("HEAVYTROOPER LOADING");
        loadingTimer = loadingTime * (doneDashes > 0 ? 0.5f : 1.0f);
        // Debug.Log("Loading timer: " + loadingTimer.ToString());
        directionDecisionTimer = directionDecisionTime;

        Animator.Play(gameObject, "HVY_Dash_P1", speedMult);
        if (spear != null)
            Animator.Play(spear, "HVY_Dash_P1", speedMult);

        UpdateAnimationSpd(speedMult);
    }
    private void UpdateLoading()
    {
        if (directionDecisionTimer > 0.0f)
        {
            //Debug.Log("HEAVYTROOPER LOADING ENTRY 1");
            directionDecisionTimer -= myDeltaTime;
            LookAt(Core.instance.gameObject.transform.globalPosition);

            if (directionDecisionTimer < 0.1f)
            {
                //Debug.Log("HEAVYTROOPER LOADING ENTRY 2");
                Vector3 direction = Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition;
                targetPosition = direction.normalized * dashLength + gameObject.transform.globalPosition;
                agent.CalculatePath(gameObject.transform.globalPosition, targetPosition);
            }
        }

        UpdateAnimationSpd(speedMult);
    }
    #endregion

    #region DASH
    private void StartDash()
    {
        //Debug.Log("HEAVYTROOPER DASH");
        if (!straightPath)
        {
            dashTimer = (dashLength / (dashSpeed * dashSpeedReduction * speedMult));
        }
        else
        {
            dashTimer = (dashLength / (dashSpeed * speedMult));
        }

        StraightPath();

        Animator.Play(gameObject, "HVY_Dash_P2", speedMult);
        if (spear != null)
            Animator.Play(spear, "HVY_Dash_P2", speedMult);

        Audio.PlayAudio(gameObject, "Play_Heavytrooper_Dash");
        UpdateAnimationSpd(speedMult);
        particles.Play(HeavyTrooperParticles.HEAVYROOPER_PARTICLES.DASH);

        doneDashes++;
    }
    private void UpdateDash()
    {
        LookAt(agent.GetDestination());

        if (!straightPath)
            agent.MoveToCalculatedPos(dashSpeed * dashSpeedReduction * speedMult);
        else
            agent.MoveToCalculatedPos(dashSpeed * speedMult);

        UpdateAnimationSpd(speedMult);
    }
    #endregion

    #region SWEEP
    private void StartSweep()
    {
        //Debug.Log("HEAVYTROOPER SWEEP");
        sweepTimer = sweepTime;
        particles.Stop(HeavyTrooperParticles.HEAVYROOPER_PARTICLES.DASH);
        Animator.Play(gameObject, "HVY_Sweep", speedMult);

        if (spear != null)
            Animator.Play(spear, "HVY_Sweep", speedMult);

        Audio.PlayAudio(gameObject, "Play_Heavytrooper_Attack");

        doneDashes = 0;
        canSweep = true;
        directionSweepDecisionTimer = directionSweepDecisionTime;
    }
    private void UpdateSweep()
    {
        if (directionSweepDecisionTimer > 0.0f)
        {
            directionSweepDecisionTimer -= myDeltaTime;
            LookAt(Core.instance.gameObject.transform.globalPosition);
        }

        if (spearCollider != null)
        {
            if (sweepTimer < sweepTime * 0.33f && spearCollider.active) // Stop doing damage at the last third of the animation
            {
                //Debug.Log("Stop doing damage");
                spearCollider.active = false;

                if (particles != null)
                    particles.Stop(HeavyTrooperParticles.HEAVYROOPER_PARTICLES.SPEAR);
            }
            else if (sweepTimer <= sweepTime * 0.75f && !spearCollider.active && canSweep) //Start doing damage at the first fourth of the animation
            {
                //Debug.Log("Start doing damage");
                spearCollider.active = true;

                if (particles != null)
                    particles.Play(HeavyTrooperParticles.HEAVYROOPER_PARTICLES.SPEAR);

                canSweep = false;
            }
        }

        UpdateAnimationSpd(speedMult);
    }

    #endregion

    #region TIRED
    private void StartTired()
    {
        //Debug.Log("HEAVYTROOPER TIRED");
        tiredTimer = tiredTime;
        doneDashes = 0;
        canSweep = true;

        Animator.Play(gameObject, "HVY_Idle", speedMult * 0.5f);

        if (spear != null)
            Animator.Play(spear, "HVY_Idle", speedMult * 0.5f);
    }

    private void UpdateTired()
    {
        if (tiredTimer < tiredTime * 0.5f)
        {
            LookAt(Core.instance.gameObject.transform.globalPosition);
            if (Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) < sweepRange * 2.0f)
            {
                //Debug.Log("Going Back");
                gameObject.transform.localPosition -= new Vector3(0.0f, 0.0f, 0.1f);
            }
        }

        UpdateAnimationSpd(speedMult);
    }

    #endregion

    #region DIE
    private void StartDie()
    {
        EnemyManager.RemoveEnemy(gameObject);
        dieTimer = dieTime;

        Animator.Play(gameObject, "HVY_Die", speedMult);
        if (spear != null)
            Animator.Play(spear, "HVY_Die", speedMult);
        UpdateAnimationSpd(speedMult);

        Audio.PlayAudio(gameObject, "Play_Heavytrooper_Death");
        if (Core.instance != null)
            Audio.PlayAudio(Core.instance.gameObject, "Play_Mando_Kill_Voice");

        //Combo
        if (PlayerResources.CheckBoon(BOONS.BOON_MASTER_YODA_FORCE))
        {
            HUD hud = Core.instance.hud.GetComponent<HUD>();
            
            if(hud != null)
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

        UpdateAnimationSpd(speedMult);
    }
    private void Die()
    {
        Counter.SumToCounterType(Counter.CounterTypes.ENEMY_HEAVYTROOPER);

        DropCoins();

        PlayerHealth playerHealth = Core.instance.gameObject.GetComponent<PlayerHealth>();
        
        if(playerHealth != null)
            playerHealth.TakeDamage(-PlayerHealth.healWhenKillingAnEnemy);

        InternalCalls.Destroy(gameObject);
    }
    #endregion

    #region PUSH
    private void StartPush()
    {
        Vector3 force = pushDir.normalized;
        if (BabyYoda.instance != null)
        {
            force.y = BabyYoda.instance.pushVerticalForce;
            force.x *= BabyYoda.instance.pushHorizontalForce;
            force.z *= BabyYoda.instance.pushHorizontalForce;
            gameObject.AddForce(force * forcePushMod);

            pushTimer = 0.0f;
        }

    }
    private void UpdatePush()
    {
        pushTimer += Time.deltaTime;
        if (BabyYoda.instance != null)
        {
            if (pushTimer >= BabyYoda.instance.PushStun)
                inputsList.Add(INPUT.IN_IDLE);
        }
        else
        {
            inputsList.Add(INPUT.IN_IDLE);
        }

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
                if (Core.instance.HasStatus(STATUS_TYPE.MANDO_QUICK_DRAW))
                    AddStatus(STATUS_TYPE.BLASTER_VULN, STATUS_APPLY_TYPE.ADDITIVE, Core.instance.GetStatusData(STATUS_TYPE.MANDO_QUICK_DRAW).severity / 100, 5);
                if (Core.instance.HasStatus(STATUS_TYPE.PRIM_MOV_SPEED))
                    Core.instance.AddStatus(STATUS_TYPE.ACCELERATED, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, Core.instance.GetStatusData(STATUS_TYPE.PRIM_MOV_SPEED).severity / 100, 5, false);
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

                if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.PRIMARY_SLOW_SPEED))
                {
                    skill_slowDownActive = true;
                    skill_slowDownTimer = 0.0f;
                }
            }
        }
        else if (collidedGameObject.CompareTag("ChargeBullet"))
        {
            ChargedBullet bullet = collidedGameObject.GetComponent<ChargedBullet>();
            Audio.PlayAudio(gameObject, "Play_Sniper_Hit");
            if (bullet != null && currentState != STATE.DIE)
            {

                particles.Play(HeavyTrooperParticles.HEAVYROOPER_PARTICLES.SNIPER_HIT);

                Audio.PlayAudio(gameObject, "Play_Stormtrooper_Hit");

                if (Core.instance.hud != null)
                {
                    Core.instance.hud.GetComponent<HUD>().AddToCombo(55, 0.25f);
                }
                //healthPoints -= bullet.GetDamage();
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
        else if (collidedGameObject.CompareTag("ExplosiveBarrel") && collidedGameObject.GetComponent<SphereCollider>().active)
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
        else if (collidedGameObject.CompareTag("WorldLimit"))
        {
            if (currentState != STATE.DIE)
            {
                inputsList.Add(INPUT.IN_DIE);
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


    public override void TakeDamage(float damage)
    {

        Audio.PlayAudio(gameObject, "Play_Heavytrooper_Hit");
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
        particles.Play(HeavyTrooperParticles.HEAVYROOPER_PARTICLES.HIT);
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

    public void StraightPath()
    {
        if (Vector2.Dot(agent.GetLastVector().ToVector2(), (agent.GetDestination() - gameObject.transform.localPosition).ToVector2()) > 0.9f)
        {
            straightPath = true;
        }
        else
        {
            straightPath = false;
        }
        //Debug.Log("StraightPath: " + straightPath);
    }

    private void UpdateAnimationSpd(float newSpd)
    {
        if (currAnimationPlaySpd != newSpd)
        {
            Animator.SetSpeed(gameObject, newSpd);
            if (spear != null)
                Animator.SetSpeed(spear, newSpd);

            currAnimationPlaySpd = newSpd;
        }
    }

    public override void PlayGrenadeHitParticles()
    {

        if (particles != null)
            particles.Play(HeavyTrooperParticles.HEAVYROOPER_PARTICLES.GRENADE_HIT);

    }

}