using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class Deathtrooper : Enemy
{
    public enum STATE : int
    {
        NONE = -1,
        IDLE,
        WANDER,
        RUN,
        PUSHED,
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
        IN_SHOOT,
        IN_SHOOT_END,
        IN_HIT,
        IN_DIE,
        IN_PLAYER_IN_RANGE,
        IN_SHOT_RANGE
    }

    //State
    private STATE currentState = STATE.NONE;

    private List<INPUT> inputsList = new List<INPUT>();

    public GameObject shootPoint = null;
    public GameObject shotgun = null;

    //Timers
    private float idleTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float pushTimer = 0.0f;
    private float skill_slowDownTimer = 0.0f;
    private float recoilTimer = 0.0f;
    private float betweenStatesTimer = 0.0f;
    private float betweenBurstsTimer = 0.0f;
    private float currAnimationPlaySpd = 1f;

    //Action times
    public float idleTime = 5.0f;
    private float dieTime = 3.0f;
    public float timeBewteenBursts = 0.8f;
    public float betweenStatesTime = 1.5f;
    private float recoilTime = 0.0f;

    //Speeds
    public float wanderSpeed = 3.5f;
    public float runningSpeed = 7.5f;
    private bool skill_slowDownActive = false;
    public float recoilSpeed = 0.0f;

    //Ranges
    public float wanderRange = 7.5f;
    public float shotRange = 12.5f;
    public float recoilDistance = 0.0f;

    //Action variables
    private int shotsShooted = 0;
    //public int maxShots = 4;
    //public float dispersionAngleDeg = 0.0f;
    private bool canShoot = true;

    //push
    public float forcePushMod = 1f;
    public float PushStun = 2;

    //hit particles
    public GameObject hitParticlesObj = null;
    private ParticleSystem hitParticle = null;
    public GameObject shotgunParticlesObj = null;
    private ParticleSystem shotgunParticle = null;
    public GameObject sniperHitParticleObj = null;
    private ParticleSystem sniperParticle = null;
    public GameObject grenadeHitParticleObj = null;
    private ParticleSystem grenadeHitParticle = null;

    public void Awake()
    {
        InitEntity(ENTITY_TYPE.DEATHTROOPER);
        EnemyManager.AddEnemy(gameObject);

        agent = gameObject.GetComponent<NavMeshAgent>();
        targetPosition = null;

        currentState = STATE.IDLE;
        Animator.Play(gameObject, "DTH_Idle", speedMult);
        if (shotgun != null)
            Animator.Play(shotgun, "DTH_Idle", speedMult);
        UpdateAnimationSpd(speedMult);

        idleTimer = idleTime;
        dieTime = Animator.GetAnimationDuration(gameObject, "DTH_Die");

        recoilTime = recoilDistance / recoilSpeed;
        if (hitParticlesObj != null)
            hitParticle = hitParticlesObj.GetComponent<ParticleSystem>();
        //else
        //Debug.Log("Hit particles gameobject not found!");

        if (shotgunParticlesObj != null)
            shotgunParticle = shotgunParticlesObj.GetComponent<ParticleSystem>();

        if (sniperHitParticleObj != null)
            sniperParticle = sniperHitParticleObj.GetComponent<ParticleSystem>();

        if (grenadeHitParticleObj != null)
            grenadeHitParticle = grenadeHitParticleObj.GetComponent<ParticleSystem>();

        //else
        //{
        //    //Debug.Log("Shotgun particles gameobject not found!");
        //}
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
            if (Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance)
            {
                inputsList.Add(INPUT.IN_IDLE);
            }
        }

        /*if (skill_slowDownActive)
        {
            skill_slowDownTimer += myDeltaTime;
            if (skill_slowDownTimer >= Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownDuration)
            {
                skill_slowDownTimer = 0.0f;
                skill_slowDownActive = false;
            }
        }*/
    }

    //All events from outside the stormtrooper
    private void ProcessExternalInput()
    {
        if (currentState != STATE.DIE)
        {
            if (Core.instance.gameObject == null)
                return;

            if (InRange(Core.instance.gameObject.transform.globalPosition, detectionRange))
            {
                if (InRange(Core.instance.gameObject.transform.globalPosition, shotRange) && canShoot)
                    inputsList.Add(INPUT.IN_SHOT_RANGE);
                else
                    inputsList.Add(INPUT.IN_PLAYER_IN_RANGE);
            }
            else
            {
                if (currentState == STATE.RUN)
                    inputsList.Add(INPUT.IN_RUN_END);
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
                    Debug.Log("DEATHTROOPER ERROR STATE");
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
                            PlayerDetected();
                            StartRun();
                            break;

                        case INPUT.IN_SHOT_RANGE:
                            currentState = STATE.SHOOT;
                            StartShoot();
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
                            PlayerDetected();
                            StartRun();
                            break;

                        case INPUT.IN_SHOT_RANGE:
                            currentState = STATE.SHOOT;
                            WanderEnd();
                            StartShoot();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            StartPush();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            WanderEnd();
                            StartDie();
                            break;
                    }
                    break;

                case STATE.RUN:
                    switch (input)
                    {
                        case INPUT.IN_RUN_END:
                            currentState = STATE.IDLE;
                            RunEnd();
                            StartIdle();
                            break;

                        case INPUT.IN_SHOT_RANGE:
                            currentState = STATE.SHOOT;
                            RunEnd();
                            StartShoot();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            RunEnd();
                            StartPush();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            RunEnd();
                            StartDie();
                            break;

                    }
                    break;

                case STATE.SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_SHOOT_END:
                            currentState = STATE.RUN;
                            StartRun();
                            break;

                        case INPUT.IN_RUN:
                            currentState = STATE.RUN;
                            StartRun();
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
                case STATE.PUSHED:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            StartIdle();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;
                    }
                    break;
                default:
                    Debug.Log("NEED TO ADD STATE TO DEATHTROOPER SWITCH");
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
            case STATE.SHOOT:
                UpdateShoot();
                break;
            case STATE.PUSHED:
                UpdatePush();
                break;
            case STATE.DIE:
                UpdateDie();
                break;
            default:
                Debug.Log("NEED TO ADD STATE TO DEATHTROOPER");
                break;
        }
    }

    #region IDLE
    private void StartIdle()
    {
        //Debug.Log("DEATHTROOPER IDLE");
        idleTimer = idleTime;
        Animator.Play(gameObject, "DTH_Idle", speedMult);
        Animator.Play(shotgun, "DTH_Idle", speedMult);
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
        //Debug.Log("DEATHTROOPER WANDER");
        agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);

        Animator.Play(gameObject, "DTH_Wander", speedMult);
        if (shotgun != null)
            Animator.Play(shotgun, "DTH_Wander", speedMult);

        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Deathtrooper_Wander");
    }
    private void UpdateWander()
    {
        LookAt(agent.GetDestination());
        //if (skill_slowDownActive)
        //    agent.MoveToCalculatedPos(wanderSpeed * (1 - Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownAmount));
        //else 
        agent.MoveToCalculatedPos(wanderSpeed * speedMult);

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
        //Debug.Log("DEATHTROOPER RUN");
        Animator.Play(gameObject, "DTH_Run", speedMult);
        if (shotgun != null)
            Animator.Play(shotgun, "DTH_Run", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Deathtrooper_Run");

    }
    private void UpdateRun()
    {
        agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(runningSpeed * speedMult);
        UpdateAnimationSpd(speedMult);
    }
    private void RunEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region SHOOT
    private void StartShoot()
    {
        //Debug.Log("DEATHTROOPER SHOOT");
        Animator.Play(gameObject, "DTH_Idle", speedMult);
        if (shotgun != null)
            Animator.Play(shotgun, "DTH_Idle", speedMult);
        UpdateAnimationSpd(speedMult);
        betweenStatesTimer = betweenStatesTime;
    }

    private void UpdateShoot()
    {
        LookAt(Core.instance.gameObject.transform.globalPosition);

        if (betweenStatesTimer > 0.0f)
        {
            betweenStatesTimer -= myDeltaTime;
            if (betweenStatesTimer <= 0.0f)
            {
                ShotgunShoot();
                betweenBurstsTimer = timeBewteenBursts;

                if (shotgunParticle != null)
                    shotgunParticle.Play();
            }

        }

        if (recoilTimer > 0.0f)
        {
            recoilTimer -= Time.deltaTime;

            //Recoil
            targetPosition = gameObject.transform.globalPosition - gameObject.transform.GetForward().normalized * recoilDistance;
            agent.CalculatePath(gameObject.transform.globalPosition, targetPosition);
            agent.MoveToCalculatedPos(recoilSpeed);

            if (recoilTimer <= 0.0f && shotsShooted >= 2)
            {
                inputsList.Add(INPUT.IN_SHOOT_END);
                shotsShooted = 0;
            }
        }

        if (betweenBurstsTimer > 0.0f)
        {
            betweenBurstsTimer -= myDeltaTime;

            if (betweenBurstsTimer <= 0.0f)
            {
                ShotgunShoot();
            }
        }

        UpdateAnimationSpd(speedMult);
    }

    private void ShotgunShoot()
    {
       
        GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/550070139.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, null);
        bullet.GetComponent<DeathTrooperBullet>().damage = damage * damageMult;
        bullet.GetComponent<DeathTrooperBullet>().SetTagToAvoid("Deathtrooper");
        bullet.GetComponent<DeathTrooperBullet>().SetGameObjectToAvoid(this.gameObject);

        recoilTimer = recoilTime;
        shotsShooted++;



        Animator.Play(gameObject, "DTH_ShootRecoil", speedMult * 2.0f);
        if(shotgun != null)
            Animator.Play(shotgun, "DTH_ShootRecoil", speedMult * 2.0f);
        Audio.PlayAudio(gameObject, "Play_Deathtrooper_Recoil");
        
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Deathtrooper_Shot");
    }
    private void PlayerDetected()
    {
        //Debug.Log("SKYTROOPER PLAYER DETECTED");
        Audio.PlayAudio(gameObject, "Play_Deathtrooper_Enemy_Detection");
    }
    #endregion

    #region DIE
    private void StartDie()
    {
        EnemyManager.RemoveEnemy(gameObject);
        dieTimer = dieTime;

        Animator.Play(gameObject, "DTH_Die");
        if (shotgun != null)
            Animator.Play(shotgun, "DTH_Die");

        Audio.PlayAudio(gameObject, "Play_Deathtrooper_Death");
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
        Vector3 forward = gameObject.transform.GetForward();
        Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage(-PlayerHealth.healWhenKillingAnEnemy);
        InternalCalls.CreatePrefab("Library/Prefabs/230945350.prefab", new Vector3(gameObject.transform.globalPosition.x + forward.x, gameObject.transform.globalPosition.y, gameObject.transform.globalPosition.z + forward.z), Quaternion.identity, new Vector3(1, 1, 1));

        Counter.SumToCounterType(Counter.CounterTypes.ENEMY_DEATHTROOPER);
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
                if (Core.instance.HasStatus(STATUS_TYPE.MANDO_QUICK_DRAW))
                    AddStatus(STATUS_TYPE.BLASTER_VULN, STATUS_APPLY_TYPE.ADDITIVE, Core.instance.GetStatusData(STATUS_TYPE.MANDO_QUICK_DRAW).severity / 100, 5);
                if (Core.instance.HasStatus(STATUS_TYPE.PRIM_MOV_SPEED))
                    Core.instance.AddStatus(STATUS_TYPE.ACCELERATED, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, Core.instance.GetStatusData(STATUS_TYPE.PRIM_MOV_SPEED).severity / 100, 5, false);
            }
            BH_Bullet bullet = collidedGameObject.GetComponent<BH_Bullet>();

            if (bullet != null)
            {
                TakeDamage(bullet.damage * damageRecieveMult * BlasterVulnerability);

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

            if (bullet != null && currentState != STATE.DIE)
            {
                Audio.PlayAudio(gameObject, "Play_Stormtrooper_Hit");

                if (Core.instance.hud != null)
                {
                    Core.instance.hud.GetComponent<HUD>().AddToCombo(55, 0.25f);
                }

                if (sniperParticle != null)
                    sniperParticle.Play();

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

                if (Core.instance != null && healthPoints <= 0.0f)
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
            //if (myParticles != null && myParticles.hit != null)
            //    myParticles.hit.Play();
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

        Audio.PlayAudio(gameObject, "Play_Deathtrooper_Hit");
        float mod = 1;
        if(Core.instance != null && Core.instance.HasStatus(STATUS_TYPE.GEOTERMAL_MARKER))
        {
            if(HasNegativeStatus())
            {
                mod = 1 + GetStatusData(STATUS_TYPE.GEOTERMAL_MARKER).severity / 100;
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
            hitParticle.Play();

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
    private void UpdateAnimationSpd(float newSpd)
    {
        if (currAnimationPlaySpd != newSpd)
        {
            Animator.SetSpeed(gameObject, newSpd);
            Animator.SetSpeed(shotgun, newSpd);
            currAnimationPlaySpd = newSpd;
        }
    }
    public STATE GetCurrentSate()
    {
        return currentState;
    }

    public override void PlayGrenadeHitParticles()
    {
        if (grenadeHitParticle != null)
        {
            grenadeHitParticle.Play();
        }
    }

}