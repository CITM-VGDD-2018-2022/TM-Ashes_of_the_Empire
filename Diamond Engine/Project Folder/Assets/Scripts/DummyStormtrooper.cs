using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class DummyStormtrooper : Enemy
{
    enum STATE : int
    {
        NONE = -1,
        IDLE,
        PUSHED,
        SHOOT,
        HIT,
        DIE
    }

    enum INPUT : int
    {
        IN_IDLE,
        IN_IDLE_END,
        IN_PUSHED,
        IN_SHOOT,
        IN_HIT,
        IN_DIE,
        IN_PLAYER_IN_RANGE
    }

    //State
    private STATE currentState = STATE.NONE;

    private List<INPUT> inputsList = new List<INPUT>();

    public GameObject shootPoint = null;
    public GameObject blaster = null;

    public bool canShoot = true;

    //Action times
    public float idleTime = 5.0f;
    private float dieTime = 3.0f;
    public float timeBewteenShots = 0.5f;
    public float timeBewteenSequences = 0.5f;
    public float timeBewteenStates = 1.5f;

    //Speeds
    public float bulletSpeed = 10.0f;
    private bool skill_slowDownActive = false;
    private float currAnimationPlaySpd = 1.0f;

    //Timers
    public float idleTimer = 0.0f;
    private float shotTimer = 0.0f;
    private float sequenceTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float statesTimer = 0.0f;
    private float pushTimer = 0.0f;
    private float skill_slowDownTimer = 0.0f;

    //Action variables
    int shotTimes = 0;
    public int maxShots = 2;
    private int shotSequences = 0;
    public int maxSequences = 2;

    //force
    public float forcePushMod = 1;

    //Death point
    public GameObject deathPoint = null;

    private StormTrooperParticles myParticles = null;

    private bool start = true;
    public void Awake()
    {
        InitEntity(ENTITY_TYPE.STROMTROOPER);

        currentState = STATE.IDLE;
        Animator.Play(gameObject, "ST_Idle", 1.0f);
        if (blaster != null)
            Animator.Play(blaster, "ST_Idle");

        UpdateAnimationSpd(1.0f);

        targetPosition = null;

        shotTimes = 0;
        shotSequences = 0;

        dieTime = Animator.GetAnimationDuration(gameObject, "ST_Die");

        ParticleSystem spawnparticles = null;

        myParticles = gameObject.GetComponent<StormTrooperParticles>();
        if (myParticles != null)
            spawnparticles = myParticles.spawn;

        if (spawnparticles != null)
            spawnparticles.Play();

    }

    public void Update()
    {
        if (start)
        {
            EnemyManager.AddEnemy(this.gameObject);
            start = false;
        }

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
        if (currentState != STATE.DIE && canShoot)
        {
            if (InRange(Core.instance.gameObject.transform.globalPosition, detectionRange) && idleTimer <= 0.0f)
            {
                if (Core.instance != null && currentState != STATE.SHOOT)
                {
                    inputsList.Add(INPUT.IN_PLAYER_IN_RANGE);
                    LookAt(Core.instance.gameObject.transform.globalPosition);
                }
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

                case STATE.SHOOT:
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
        Animator.Play(gameObject, "ST_Idle", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "ST_Idle");
        UpdateAnimationSpd(speedMult);
    }

    private void UpdateIdle()
    {
        if (idleTimer > 0.0f)
            idleTimer -= Time.deltaTime;

        UpdateAnimationSpd(speedMult);
    }
    #endregion

    #region SHOOT
    private void StartShoot()
    {
        statesTimer = timeBewteenStates;
        Animator.Play(gameObject, "ST_Idle", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "ST_Idle");
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
                    shotTimer = timeBewteenShots;
                }
                //Second Timer
                else
                {
                    //Reboot times
                    shotTimes = 0;
                    shotSequences = 0;
                    inputsList.Add(INPUT.IN_IDLE);
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
                        Animator.Play(blaster, "ST_Idle");
                    UpdateAnimationSpd(speedMult);

                    //End of second shot of the first sequence
                    if (shotSequences < maxSequences)
                    {
                        sequenceTimer = timeBewteenSequences;
                        shotTimes = 0;
                        //Start of pause between sequences
                    }
                    //End of second shot of the second sequence
                    else
                    {
                        statesTimer = timeBewteenStates;
                        inputsList.Add(INPUT.IN_IDLE);
                        idleTimer = idleTime;
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
                shotTimer = timeBewteenShots;
            }
        }

        LookAt(Core.instance.gameObject.transform.globalPosition);
        UpdateAnimationSpd(speedMult);
    }

    private void Shoot()
    {
        GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/1635392825.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, shootPoint.transform.globalScale);
        bullet.GetComponent<BH_Bullet>().damage = damage;

        Animator.Play(gameObject, "ST_Shoot", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "ST_Shoot");
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "PLay_Blaster_Stormtrooper");
        shotTimes++;
    }

    private void PlayerDetected()
    {
        Audio.PlayAudio(gameObject, "Play_Enemy_Detection");
        if (myParticles != null && myParticles.alert != null)
            myParticles.alert.Play();
    }
    #endregion

    #region DIE
    private void StartDie()
    {
        dieTimer = dieTime;

        Animator.Play(gameObject, "ST_Die", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "ST_Die");
        UpdateAnimationSpd(speedMult);

        Audio.PlayAudio(gameObject, "Play_Stormtrooper_Death");
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
            dieTimer -= Time.deltaTime;

            if (dieTimer <= 0.0f)
                Die();
        }

        UpdateAnimationSpd(speedMult);
    }

    private void Die()
    {
        float dist = (deathPoint.transform.globalPosition - gameObject.transform.globalPosition).magnitude;
        Vector3 forward = gameObject.transform.GetForward();
        forward = forward.normalized * (-dist);
        InternalCalls.CreatePrefab("Library/Prefabs/230945350.prefab", new Vector3(gameObject.transform.globalPosition.x + forward.x, gameObject.transform.globalPosition.y, gameObject.transform.globalPosition.z + forward.z), Quaternion.identity, new Vector3(1, 1, 1));

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
                if (Core.instance.HasStatus(STATUS_TYPE.PRIM_SLOW))
                    AddStatus(STATUS_TYPE.SLOWED, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, Core.instance.GetStatusData(STATUS_TYPE.PRIM_SLOW).severity / 100, 2, false);
            if (Core.instance != null)
                if (Core.instance.HasStatus(STATUS_TYPE.PRIM_MOV_SPEED))
                    AddStatus(STATUS_TYPE.ACCELERATED, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, Core.instance.GetStatusData(STATUS_TYPE.PRIM_MOV_SPEED).severity / 100, 5, false);
            BH_Bullet bullet = collidedGameObject.GetComponent<BH_Bullet>();

            if (bullet != null)
                TakeDamage(bullet.GetDamage() * damageRecieveMult);

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
        else if (collidedGameObject.CompareTag("ChargeBullet"))
        {
            ChargedBullet bullet = collidedGameObject.GetComponent<ChargedBullet>();

            if (bullet != null && currentState != STATE.DIE)
            {
                float vulerableSev = 0.2f;
                float vulerableTime = 4.5f;
                STATUS_APPLY_TYPE applyType = STATUS_APPLY_TYPE.BIGGER_PERCENTAGE;
                float damageMult = 1f;

                if (myParticles != null && myParticles.sniperHit != null)
                    myParticles.sniperHit.Play();

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
                    if (Core.instance.HasStatus(STATUS_TYPE.SNIPER_STACK_BLEED))
                    {
                        StatusData bleedData = Core.instance.GetStatusData(STATUS_TYPE.SNIPER_STACK_BLEED);
                        float chargedBulletMaxDamage = Core.instance.GetSniperMaxDamage();

                        damageMult *= bleedData.remainingTime;
                        this.AddStatus(STATUS_TYPE.ENEMY_BLEED, STATUS_APPLY_TYPE.ADD_SEV, (chargedBulletMaxDamage * bleedData.severity) / vulerableTime, vulerableTime);
                    }
                    if (Core.instance.HasStatus(STATUS_TYPE.SNIPER_STACK_WORK_SNIPER))
                    {
                        vulerableSev += Core.instance.GetStatusData(STATUS_TYPE.SNIPER_STACK_WORK_SNIPER).severity;
                        damageMult = damageRecieveMult;
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

                Audio.PlayAudio(gameObject, "Play_Stormtrooper_Hit");

                if (Core.instance.hud != null && currentState != STATE.DIE)
                {
                    HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                    if (hudComponent != null)
                        hudComponent.AddToCombo(55, 0.25f);
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

        if (currentState != STATE.DIE)
        {
            if (myParticles != null && myParticles.hit != null)
                myParticles.hit.Play();

            healthPoints -= damage;

            if (Core.instance != null)
            {
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