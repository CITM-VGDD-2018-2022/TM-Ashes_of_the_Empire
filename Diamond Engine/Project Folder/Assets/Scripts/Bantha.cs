using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class Bantha : Enemy
{
    enum STATE : int
    {
        NONE = -1,
        IDLE,
        RUN,
        WANDER,
        LOADING_ATTACK,
        CHARGE,
        TIRED,
        PUSHED,
        HIT,
        DIE
    }

    enum INPUT : int
    {
        IN_IDLE,
        IN_RUN,
        IN_WANDER,
        IN_PUSHED,
        IN_LOADING,
        IN_CHARGE,
        IN_CHARGE_END,
        IN_HIT,
        IN_DIE,
        IN_DIE_END,
        IN_CHARGE_RANGE,
        IN_PLAYER_IN_RANGE,
        IN_RUN_END
    }

    //State
    private STATE currentState = STATE.NONE;

    private List<INPUT> inputsList = new List<INPUT>();

    public GameObject chargePoint = null;
    public GameObject hitParticles = null;
    public GameObject snipHitParticles = null;
    private GameObject visualFeedback = null;
    private GameObject grenadeHitParticleObj = null;

    private bool straightPath = false;

    //Action times
    public float idleTime = 5.0f;
    private float dieTime = 3.0f;
    public float tiredTime = 2.0f;
    public float loadingTime = 2.0f;
    public float timeBewteenStates = 1.5f;
    public float directionDecisionTime = 0.8f;

    //Speeds
    public float wanderSpeed = 3.5f;
    public float runningSpeed = 7.5f;
    public float chargeSpeed = 60.0f;
    public float chargeSpeedReduction = 0.3f;
    private bool skill_slowDownActive = false;
    private float currAnimationPlaySpd = 1f;

    //Ranges
    public float wanderRange = 7.5f;
    public float chargeRange = 5.0f;
    public float chargeLength = 20.0f;

    //Timers
    private float idleTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float tiredTimer = 0.0f;
    private float loadingTimer = 0.0f;
    private float chargeTimer = 0.0f;
    private float directionDecisionTimer = 0.0f;
    private float skill_slowDownTimer = 0.0f;
    private float pushTimer = 0.0f;

    //force
    public float forcePushMod = 1;
    public float pushTime = 0f;

    //Particles
    public GameObject stunParticle = null;
    private ParticleSystem stun = null;

    public void Awake()
    {
        InitEntity(ENTITY_TYPE.BANTHA);
        EnemyManager.AddEnemy(gameObject);

        StartIdle();
        agent = gameObject.GetComponent<NavMeshAgent>();
        targetPosition = null;

        currentState = STATE.IDLE;

        loadingTime = Animator.GetAnimationDuration(gameObject, "BT_Charge");
        dieTime = Animator.GetAnimationDuration(gameObject, "BT_Die");

        if (stunParticle != null)
            stun = stunParticle.GetComponent<ParticleSystem>();
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

        if (currentState == STATE.WANDER)
        {
            if (Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance)
            {
                inputsList.Add(INPUT.IN_IDLE);
            }
        }

        if (loadingTimer > 0.0f)
        {
            loadingTimer -= myDeltaTime;

            if (loadingTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_CHARGE);
            }
        }

        if (currentState == STATE.CHARGE && chargeTimer > 0.0f)
        {
            chargeTimer -= myDeltaTime;
            if (Mathf.Distance(gameObject.transform.globalPosition, targetPosition) <= agent.stoppingDistance || chargeTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_CHARGE_END);
            }
        }

        if (tiredTimer > 0.0f)
        {
            tiredTimer -= myDeltaTime;

            if (tiredTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_RUN);
            }
        }

        /*if (skill_slowDownActive)
        {
            skill_slowDownTimer += myDeltaTime;

            if (skill_slowDownTimer >= Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownDuration) //Get duration from Primary Weapon Skill 4
            {
                skill_slowDownTimer = 0.0f;
                skill_slowDownActive = false;
            }
        }*/
    }

    //All events from outside the stormtrooper
    private void ProcessExternalInput()
    {
        if (currentState != STATE.DIE && currentState != STATE.CHARGE)
        {
            if (Core.instance != null)
            {
                if (InRange(Core.instance.gameObject.transform.globalPosition, detectionRange) && agent.IsPathPossible(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition))
                {
                    inputsList.Add(INPUT.IN_PLAYER_IN_RANGE);
                }
                if (InRange(Core.instance.gameObject.transform.globalPosition, chargeRange) && straightPath)
                {
                    inputsList.Add(INPUT.IN_CHARGE_RANGE);
                }
            }
        }
        if (currentState == STATE.RUN)
        {
            if (agent == null)
            {
                inputsList.Add(INPUT.IN_IDLE);
                //Debug.Log("My agent is null :)");
            }
            if (Core.instance != null)
            {
                if (!InRange(Core.instance.gameObject.transform.globalPosition, detectionRange))
                {
                    inputsList.Add(INPUT.IN_WANDER);
                }
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
                    Debug.Log("BANTHA ERROR STATE");
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
                        case INPUT.IN_CHARGE_RANGE:
                            currentState = STATE.LOADING_ATTACK;
                            StartLoading();
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

                        case INPUT.IN_CHARGE_RANGE:
                            currentState = STATE.LOADING_ATTACK;
                            RunEnd();
                            StartLoading();
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

                case STATE.LOADING_ATTACK:
                    switch (input)
                    {
                        case INPUT.IN_CHARGE:
                            currentState = STATE.CHARGE;
                            StartCharge();
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

                case STATE.CHARGE:
                    switch (input)
                    {
                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartWander();
                            break;

                        case INPUT.IN_RUN:
                            currentState = STATE.RUN;
                            StartRun();
                            break;

                        case INPUT.IN_CHARGE_END:
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
                    Debug.Log("NEED TO ADD STATE TO BANTHA SWITCH");
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
            case STATE.LOADING_ATTACK:
                UpdateLoading();
                break;
            case STATE.CHARGE:
                UpdateCharge();
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
                Debug.Log("NEED TO ADD STATE TO BANTHA");
                break;
        }
    }

    #region IDLE
    private void StartIdle()
    {
        idleTimer = idleTime;
        Animator.Play(gameObject, "BT_Idle", speedMult);
        UpdateAnimationSpd(speedMult);
    }

    private void UpdateIdle()
    {
        UpdateAnimationSpd(speedMult);
    }

    #endregion

    #region TIRED
    private void StartTired()
    {
        Audio.StopAudio(gameObject);

        tiredTimer = tiredTime;
        Animator.Play(gameObject, "BT_Idle", speedMult);
        Audio.PlayAudio(gameObject, "Play_Bantha_Breath");
        stun.Play();
    }
    private void UpdateTired()
    {
        UpdateAnimationSpd(speedMult);

        if (stun.playing == false)
            stun.Play();
    }

    #endregion

    #region RUN
    private void StartRun()
    {
        Animator.Play(gameObject, "BT_Walk", speedMult);
        UpdateAnimationSpd(speedMult);
    }
    private void UpdateRun()
    {
        if (Core.instance != null)
        {
            if (agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) == false)
            {
                inputsList.Add(INPUT.IN_IDLE);
                return;
            }
        }

        LookAt(agent.GetDestination());
        //if (skill_slowDownActive)
        //    agent.MoveToCalculatedPos(runningSpeed * (1 - Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownAmount));

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
        agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);
        Animator.Play(gameObject, "BT_Walk", 1.4f * speedMult);
        UpdateAnimationSpd(1.4f * speedMult);

        Audio.PlayAudio(gameObject, "Play_Footsteps_Bantha");
    }
    private void UpdateWander()
    {
        LookAt(agent.GetDestination());
        //if (skill_slowDownActive)
        //    agent.MoveToCalculatedPos(wanderSpeed * (1 - Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownAmount));

        agent.MoveToCalculatedPos(wanderSpeed * speedMult);

        UpdateAnimationSpd(1.4f * speedMult);

    }
    private void WanderEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region LOADING_ATTACK
    private void StartLoading()
    {
        loadingTimer = loadingTime;
        directionDecisionTimer = directionDecisionTime;

        visualFeedback = InternalCalls.CreatePrefab("Library/Prefabs/1137197426.prefab", chargePoint.transform.globalPosition, chargePoint.transform.globalRotation, new Vector3(1.0f, 1.0f, 0.01f));
        Animator.Play(gameObject, "BT_Charge", speedMult);
        UpdateAnimationSpd(speedMult);
    }
    private void UpdateLoading()
    {
        if (directionDecisionTimer > 0.0f)
        {
            directionDecisionTimer -= myDeltaTime;

            if (directionDecisionTimer > 0.1f)
            {
                if (Core.instance != null)
                {
                    Vector3 direction = Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition;
                    targetPosition = direction.normalized * chargeLength + gameObject.transform.globalPosition;
                    agent.CalculatePath(gameObject.transform.globalPosition, targetPosition);
                    LookAt(targetPosition);
                }
            }
        }
        if (visualFeedback.transform.globalScale.z < 1.0)
        {
            visualFeedback.transform.localScale = new Vector3(1.0f, 1.0f, Mathf.Lerp(visualFeedback.transform.localScale.z, 1.0f, myDeltaTime * (loadingTime / loadingTimer)));
            visualFeedback.transform.localRotation = gameObject.transform.globalRotation;
        }

        UpdateAnimationSpd(speedMult);
    }
    #endregion

    #region CHARGE
    private void StartCharge()
    {
        //if (skill_slowDownActive)
        //    chargeTimer = chargeLength / (chargeSpeed * (1 - Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownAmount));
        if (!straightPath)
            chargeTimer = (chargeLength / (chargeSpeed * chargeSpeedReduction)) * speedMult;
        else chargeTimer = (chargeLength / chargeSpeed) * speedMult;

        Animator.Play(gameObject, "BT_Run", speedMult);
        UpdateAnimationSpd(speedMult);
        InternalCalls.Destroy(visualFeedback);
        visualFeedback = null;

        Audio.PlayAudio(gameObject, "Play_Bantha_Attack");
        Audio.PlayAudio(gameObject, "Play_Bantha_Ramming");
        Audio.PlayAudio(gameObject, "Play_Footsteps_Bantha");

        StraightPath();

        LookAt(agent.GetDestination());
    }
    private void UpdateCharge()
    {
        //if (skill_slowDownActive)
        //    agent.MoveToCalculatedPos(chargeSpeed * (1 - Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownAmount));

        if (!straightPath)
            agent.MoveToCalculatedPos(chargeSpeed * chargeSpeedReduction * speedMult);
        else
            agent.MoveToCalculatedPos(chargeSpeed * speedMult);

        UpdateAnimationSpd(speedMult);
    }
    #endregion

    #region DIE
    private void StartDie()
    {
        EnemyManager.RemoveEnemy(gameObject);

        if (visualFeedback != null)
            InternalCalls.Destroy(visualFeedback);

        dieTimer = dieTime;

        Animator.Play(gameObject, "BT_Die", speedMult);
        UpdateAnimationSpd(speedMult);

        Audio.PlayAudio(gameObject, "Play_Growl_Bantha_Death");

        if (hitParticles != null)
            hitParticles.GetComponent<ParticleSystem>().Play();

        if (Core.instance != null)
        {
            Audio.PlayAudio(Core.instance.gameObject, "Play_Mando_Kill_Voice");
            //Combo
            if (PlayerResources.CheckBoon(BOONS.BOON_MASTER_YODA_FORCE))
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(300, 1.0f);
            }
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
        Counter.SumToCounterType(Counter.CounterTypes.ENEMY_BANTHA);

        DropCoins();

        if (Core.instance != null)
        {
            Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage(-PlayerHealth.healWhenKillingAnEnemy);
        }

        InternalCalls.Destroy(gameObject);
    }
    #endregion

    #region PUSH
    private void StartPush()
    {
        if (visualFeedback != null)
        {
            InternalCalls.Destroy(visualFeedback);
            visualFeedback = null;
        }

        if (BabyYoda.instance != null && pushDir != null)
        {
            Vector3 force = pushDir.normalized;
            force.y = BabyYoda.instance.pushVerticalForce;
            force.x *= BabyYoda.instance.pushHorizontalForce;
            force.z *= BabyYoda.instance.pushHorizontalForce;
            gameObject.AddForce(force * forcePushMod);

            pushTimer = 0.0f;
        }

    }
    private void UpdatePush()
    {
        pushTimer += myDeltaTime;
        if (pushTimer >= pushTime)
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
                if (Core.instance.HasStatus(STATUS_TYPE.MANDO_QUICK_DRAW))
                    AddStatus(STATUS_TYPE.BLASTER_VULN, STATUS_APPLY_TYPE.ADDITIVE, Core.instance.GetStatusData(STATUS_TYPE.MANDO_QUICK_DRAW).severity / 100, 5);
                if (Core.instance.HasStatus(STATUS_TYPE.PRIM_MOV_SPEED))
                    Core.instance.AddStatus(STATUS_TYPE.ACCELERATED, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, Core.instance.GetStatusData(STATUS_TYPE.PRIM_MOV_SPEED).severity / 100, 5, false);

            }

            BH_Bullet bullet = collidedGameObject.GetComponent<BH_Bullet>();
            if (bullet != null)
            {
                TakeDamage(bullet.GetDamage() * damageRecieveMult * BlasterVulnerability);

                Audio.PlayAudio(gameObject, "Play_Growl_Bantha_Hit");

                if (Core.instance != null && Core.instance.hud != null && currentState != STATE.DIE)
                {
                    HUD hud = Core.instance.hud.GetComponent<HUD>();

                    if (hud != null)
                    {
                        hud.AddToCombo(20, 1.0f);
                    }
                }
            }
        }
        else if (collidedGameObject.CompareTag("ChargeBullet"))
        {
            Debug.Log("Bantha charged bullet detection");

            ChargedBullet bullet = collidedGameObject.GetComponent<ChargedBullet>();
            if (bullet != null)
            {
                if (snipHitParticles != null)
                {
                    ParticleSystem sniperParticles = snipHitParticles.GetComponent<ParticleSystem>();

                    if (sniperParticles != null)
                        sniperParticles.Play();
                }

                Debug.Log("Bantha charged bullet scripts detect");

                if (currentState != STATE.DIE)
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

                    if (Core.instance != null & healthPoints <= 0f)
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

                    Audio.PlayAudio(gameObject, "Play_Growl_Bantha_Hit");

                    if (Core.instance.hud != null)
                    {
                        HUD hud = Core.instance.hud.GetComponent<HUD>();

                        if (hud != null)
                            hud.AddToCombo(55, 0.2f);
                    }
                }


            }
        }
        else if (collidedGameObject.CompareTag("WorldLimit"))
        {
            if (currentState != STATE.DIE)
                inputsList.Add(INPUT.IN_DIE);
        }
        else if (collidedGameObject.CompareTag("Player"))
        {
            if (currentState == STATE.CHARGE)
            {
                inputsList.Add(INPUT.IN_CHARGE_END);
                PlayerHealth playerHealth = collidedGameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage((int)damage, true);
                }
            }
        }
        else if (collidedGameObject.CompareTag("ExplosiveBarrel"))
        {
            SphereCollider sphereColl = collidedGameObject.GetComponent<SphereCollider>();
            if (sphereColl != null)
            {
                if (sphereColl.active)
                {
                    BH_DestructBox explosion = collidedGameObject.GetComponent<BH_DestructBox>();

                    if (explosion != null)
                    {
                        healthPoints -= explosion.explosion_damage;
                        if (currentState != STATE.DIE && healthPoints <= 0.0f)
                            inputsList.Add(INPUT.IN_DIE);
                    }
                }
            }

            if(Core.instance != null &&  currentState != STATE.DIE)
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
                if (result <= 11.0f)
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

        if (currentState != STATE.DIE)
        {
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

    public void StraightPath()
    {
        if (agent != null && Vector2.Dot(agent.GetLastVector().ToVector2(), (agent.GetDestination() - gameObject.transform.localPosition).ToVector2()) > 0.9f)
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
            currAnimationPlaySpd = newSpd;
        }
    }

    public override void PlayGrenadeHitParticles()
    {
        if (grenadeHitParticleObj != null)
        {
            ParticleSystem grenadeHitParticles = grenadeHitParticleObj.GetComponent<ParticleSystem>();

            if (grenadeHitParticles != null)
                grenadeHitParticles.Play();
        }
    }
}