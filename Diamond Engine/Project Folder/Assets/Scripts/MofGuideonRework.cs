using System;
using DiamondEngine;

using System.Collections.Generic;

public class MofGuideonRework : Entity
{

    enum PHASE : int
    {
        NONE = -1,
        PHASE1,
        PHASE2
    }

    enum STATE : int
    {
        NONE = -1,
        START, // 1st frame init
        PRESENTATION,

        // Neutral
        IDLE,
        CHASE,
        ACTION_SELECT,

        // Melee Combo
        MELEE_COMBO_1_CHARGE,
        MELEE_COMBO_1_DASH,
        MELEE_COMBO_1,
        MELEE_COMBO_2_DASH,
        MELEE_COMBO_2,
        MELEE_COMBO_3_DASH,
        MELEE_COMBO_3,
        MELEE_COMBO_4_CHARGE,
        MELEE_COMBO_4_DASH,
        MELEE_COMBO_4,
        MELEE_COMBO_5_DASH,
        MELEE_COMBO_5,
        MELEE_COMBO_6_DASH,
        MELEE_COMBO_6,

        SPAWN_ENEMIES,

        PRE_BURST_DASH,
        BURST_1,
        BURST_2,

        THROW_SABER,
        RETRIEVE_SABER,

        // End
        CHANGE_PHASE,
        DEAD
    }

    enum INPUT : int
    {
        NONE = -1,
        IN_PRESENTATION,
        IN_PRESENTATION_END,

        IN_CHASE,
        IN_ACTION_SELECT,

        // Melee combo
        IN_MELEE_COMBO_1_CHARGE,
        IN_MELEE_COMBO_1_DASH,
        IN_MELEE_COMBO_1,
        IN_MELEE_COMBO_2,
        IN_MELEE_COMBO_3,
        IN_MELEE_COMBO_4_CHARGE,
        IN_MELEE_COMBO_4_DASH,
        IN_MELEE_COMBO_4,
        IN_MELEE_COMBO_5,
        IN_MELEE_COMBO_6,
        IN_MELEE_CHARGE_END,
        IN_MELEE_HIT_END,
        IN_MELEE_DASH_END,

        IN_SPAWN_ENEMIES,
        IN_SPAWN_ENEMIES_END,

        IN_PRE_BURST_DASH,
        IN_PRE_BURST_DASH_END,
        IN_BURST1,
        IN_BURST2,
        IN_BURST_END,

        // Saber Throw
        IN_THROW_SABER,
        IN_THROW_SABER_END,
        IN_CHARGE_THROW,
        IN_CHARGE_THROW_END,
        IN_RETRIEVE_SABER,
        IN_RETRIEVE_SABER_END,

        // Finishers
        IN_CHANGE_PHASE,
        IN_PHASE_CHANGE_END,
        IN_DEAD
    }

    private NavMeshAgent agent = null;
    private GameObject saber = null;

    public GameObject camera = null;
    private CameraController cameraComp = null;


    //State
    private STATE currentState = STATE.NONE;
    private List<INPUT> inputsList = new List<INPUT>();
    private PHASE currentPhase = PHASE.PHASE1;
    private bool start = false;

    Random decisionGenerator = new Random();

    public GameObject hitParticles = null;

    public float slerpRotationSpeed = 5.0f;
    private Vector3 targetPosition = new Vector3(0.0f, 0.0f, 0.0f);

    public float maxHealthPoints1 = 4500.0f;
    public float maxHealthPoints2 = 4500.0f;
    private float currentHealthPoints = 0.0f; //Set in start
    private float limboHealth = 0.0f;
    private float damageMult = 1.0f;
    public float damageRecieveMult = 1f;

    // Animations
    private float currAnimationPlaySpd = 1f;

    //Decision making
    public float probMeleeCombo_P1 = 20.0f;
    public float probBurst_P1 = 60.0f;

    public float probMeleeCombo_P2 = 20.0f;
    public float probBurst_P2 = 40.0f;
    public float probLightDash = 20.0f; //Onlly phase 2

    public float minProjectileDistance = 17.0f;
    public float maxMeleeDistance = 5.0f;

    // Presentation
    private float presentationTime = 0f;
    private float presentationTimer = 0f;

    //Chase
    public float chaseDuration = 2.0f;
    private float chaseTimer = 0.0f;

    public float chaseSpeed = 3.0f;

    //Melee combo
    public float comboChargeDuration = 0.5f;
    private float comboChargeTimer = 0.0f;

    public float comboLongDashDistance = 10.0f;
    public float comboLongDashSpeed = 5.0f;

    public float comboShortDashDistance = 10.0f;
    public float comboShortDashSpeed = 5.0f;

    private float comboDashTimer = 0.0f;

    private float meleeHit1Duration = 0.0f;
    private float meleeHit2Duration = 0.0f;
    private float meleeHit3Duration = 0.0f;
    private float meleeHit4Duration = 0.0f;
    private float meleeHit5Duration = 0.0f;
    private float meleeHit6Duration = 0.0f;

    private float meleeHitTimer = 0.0f;

    public float meleeHit1Damage = 2.0f;
    public float meleeHit2Damage = 2.0f;
    public float meleeHit3Damage = 2.0f;

    public float meleeHit4Damage = 2.0f;
    public float meleeHit5Damage = 2.0f;
    public float meleeHit6Damage = 2.0f;


    //Enemy spawn
    public float enemySpawnCooldown = 15.0f;
    private float enemySpawnTimer = 0.0f;

    public GameObject spawner1 = null;
    public GameObject spawner2 = null;
    public GameObject spawner3 = null;
    public GameObject spawner4 = null;
    public GameObject spawner5 = null;
    public GameObject spawner6 = null;
    private SortedDictionary<float, GameObject> spawnPoints = null;

    public float baseEnemySpawnDelay = 0f;
    public float maxEnemySpawnDelay = 0f;
    public float spawnEnemyTime = 0f;
    private float spawnEnemyTimer = 0f;
    public float enemySkillTime = 0f;
    private float enemySkillTimer = 0f;
    private bool ableToSpawnEnemies = true;

    //Pre burst dash
    public float preBurstDashSpeed = 10.0f;
    public float preBurstDashDistance = 7.0f;

    private float preBurstDashTimer = 0.0f;

    //Burst 1
    public int shotNumber = 3;
    private int shotTimes = 0;

    public float timeBetweenShots = 0.3f;
    private float timeBetweenShotsTimer = 0.0f;

    public float timeBeforeShoot = 1.0f;
    private float timeBeforeShootTimer = 0.0f;


    //Burst 2
    //TODO


    //Prepare throw saber
    public float prepSaberThrowDuration = 3.0f;
    private float prepSaberThrowTimer = 0.0f;
    private GameObject chargeVisualFeedback = null;

    //Throw saber
    public float saberThrowDuration = 2.0f;
    private float saberThrowTimer = 0.0f;

    private float saberThrowAnimDuration = 0.0f;
    private float saberThrowAnimTimer = 0.0f;

    // Change Phase
    public float changingPhaseTime = 0f;
    private float changingPhaseTimer = 0f;

    //Die
    public float dieTime = 0f;
    private float dieTimer = 0f;

    // Boss Bar
    public GameObject bossBar = null;
    public GameObject moffMesh = null;
    private float damaged = 0.0f;
    private Material bossBarMat = null;

    public void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        InitEntity(ENTITY_TYPE.MOFF);

        if (EnemyManager.EnemiesLeft() > 0)
            EnemyManager.ClearList();
        EnemyManager.AddEnemy(gameObject);

        Audio.SetState("Player_State", "Alive");
        Audio.SetState("Game_State", "Moff_Guideon_Room");

        //Boss Bar
        if (bossBar != null)
            bossBarMat = bossBar.GetComponent<Material>();


        //Spawners
        spawner1 = InternalCalls.FindObjectWithName("DefaultSpawnPoint1");
        spawner2 = InternalCalls.FindObjectWithName("DefaultSpawnPoint2");
        spawner3 = InternalCalls.FindObjectWithName("DefaultSpawnPoint3");
        spawner4 = InternalCalls.FindObjectWithName("DefaultSpawnPoint4");
        spawner5 = InternalCalls.FindObjectWithName("DefaultSpawnPoint5");
        spawner6 = InternalCalls.FindObjectWithName("DefaultSpawnPoint6");

        //Get anim durations
        presentationTime = Animator.GetAnimationDuration(gameObject, "MG_PowerPose");

        saberThrowAnimDuration = Animator.GetAnimationDuration(gameObject, "MG_SaberThrow");

        meleeHit1Duration = Animator.GetAnimationDuration(gameObject, "MG_MeleeCombo1");
        meleeHit2Duration = Animator.GetAnimationDuration(gameObject, "MG_MeleeCombo2");
        meleeHit3Duration = Animator.GetAnimationDuration(gameObject, "MG_MeleeCombo3");
        meleeHit4Duration = Animator.GetAnimationDuration(gameObject, "MG_MeleeCombo4");
        meleeHit5Duration = Animator.GetAnimationDuration(gameObject, "MG_MeleeCombo5");
        meleeHit6Duration = Animator.GetAnimationDuration(gameObject, "MG_MeleeCombo6");
    }

    protected override void InitEntity(ENTITY_TYPE myType)
    {
        eType = myType;
        speedMult = 1f;
        BlasterVulnerability = 1f;

        damageMult = 1f;
        damageRecieveMult = 1f;
    }

    public void Update()
    {
        if (start == false)
        {
            start = true;
        }

        myDeltaTime = Time.deltaTime * speedMult;

        UpdateStatuses();

        ProcessInternalInput();
        ProcessExternalInput();
        ProcessState();

        UpdateDamaged();
    }

    //Timers go here
    private void ProcessInternalInput()
    {
        //Presentation
        if (presentationTimer > 0)
        {
            presentationTimer -= myDeltaTime;
            if (presentationTimer <= 0)
            {
                inputsList.Add(INPUT.IN_PRESENTATION_END);
            }

        }

        // Chase 
        if (chaseTimer > 0)
        {
            chaseTimer -= myDeltaTime;

            if (chaseTimer <= 0)
                inputsList.Add(INPUT.IN_ACTION_SELECT);
        }

        // Melee combo
        if (comboDashTimer > 0)
        {
            comboDashTimer -= myDeltaTime;

            if (comboDashTimer <= 0)
                inputsList.Add(INPUT.IN_MELEE_DASH_END);
        }


        if (meleeHitTimer > 0)
        {
            meleeHitTimer -= myDeltaTime;

            if (meleeHitTimer <= 0)
                inputsList.Add(INPUT.IN_MELEE_HIT_END);
        }

        if (comboChargeTimer > 0)
        {
            comboChargeTimer -= myDeltaTime;

            if (comboChargeTimer <= 0)
                inputsList.Add(INPUT.IN_MELEE_HIT_END);
        }

        //Spawn Enemies
        if (enemySkillTimer > 0 && EnemyManager.EnemiesLeft() <= 1)
        {
            enemySkillTimer -= myDeltaTime;

            if (enemySkillTimer <= 0)
            {
                ableToSpawnEnemies = true;
            }
        }

        if (spawnEnemyTimer > 0)
        {
            spawnEnemyTimer -= myDeltaTime;

            if (spawnEnemyTimer <= 0)
            {
                inputsList.Add(INPUT.IN_SPAWN_ENEMIES_END);
            }
        }

        //Die
        if (changingPhaseTimer > 0.0f)
        {
            changingPhaseTimer -= myDeltaTime;

            if (changingPhaseTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_PHASE_CHANGE_END);
            }
        }


        //Die
        if (dieTimer > 0.0f)
        {
            dieTimer -= myDeltaTime;

            if (dieTimer <= 0.0f)
            {
                Die();
            }
        }

    }

    private void ProcessExternalInput()
    {

    }

    private void ProcessState()
    {

    }

    #region PRESENTATION
    private void StartPresentation()
    {
        Animator.Play(gameObject, "MG_PowerPose", speedMult);
        UpdateAnimationSpd(speedMult);

        presentationTimer = presentationTime;

        if (cameraComp != null)
        {
            //TODO: Start cinematic
            //cameraComp.Zoom(baseZoom, zoomTimeEasing);
            //cameraComp.target = this.gameObject;
        }

        CalculateSpawnersScore();
        var mapValues = spawnPoints.Values;
        foreach (GameObject spawner in mapValues)
        {
            if (spawner != null)
            {
                SpawnPoint mySpawnPoint = spawner.GetComponent<SpawnPoint>();

                if (mySpawnPoint != null)
                {
                    mySpawnPoint.SetSpawnTypes(true, false, false, false, false, false);
                }
            }
        }

        Input.PlayHaptic(0.9f, 2200);
    }

    private void UpdatePresentation()
    {
        currentHealthPoints = Mathf.Lerp(currentHealthPoints, maxHealthPoints1, 1f - (presentationTimer / presentationTime));
    }


    private void EndPresentation()
    {
        //if (cameraComp != null)
        //    cameraComp.target = Core.instance.gameObject;

        currentHealthPoints = limboHealth = maxHealthPoints1;
    }

    #endregion

    #region CHANGE_PHASE
    private void StartPhaseChange()
    {
        Animator.Play(gameObject, "MG_Rising", speedMult);
        UpdateAnimationSpd(speedMult);

        changingPhaseTimer = changingPhaseTime;

        var mapValues = spawnPoints.Values;
        foreach (GameObject spawner in mapValues)
        {
            if (spawnPoints != null)
            {
                SpawnPoint mySpawnPoint = spawner.GetComponent<SpawnPoint>();

                if (mySpawnPoint != null)
                {
                    mySpawnPoint.SetSpawnTypes(false, false, false, false, true, false);
                }
            }
        }

        Input.PlayHaptic(0.7f, 1000);

    }


    private void UpdatePhaseChange()
    {
        //if (changingStateTimer <= 2.5f && !showingSaber)
        //{
        //    showingSaber = true;
        //    Audio.PlayAudio(gameObject, "Play_Moff_Gideon_Lightsaber_Turn_On");
        //    Audio.SetState("Game_State", "Moff_Gideon_Phase_2");
        //    sword.Enable(true);
        //    if (camera != null)
        //    {
        //        Shake3D shake = camera.GetComponent<Shake3D>();
        //        if (shake != null)
        //        {
        //            shake.StartShaking(2f, 0.12f);
        //            Input.PlayHaptic(2f, 400);
        //        }
        //    }
        //}

        currentHealthPoints = Mathf.Lerp(currentHealthPoints, maxHealthPoints2, 1f - (changingPhaseTimer / changingPhaseTime));
    }


    private void EndPhaseChange()
    {
        currentPhase = PHASE.PHASE2;
        enemySkillTimer = enemySkillTime;
        currentHealthPoints = maxHealthPoints2;
    }

#endregion

    #region DIE_ACTION
    private void StartDie()
    {
        dieTimer = dieTime;
        Animator.Play(gameObject, "MG_Death", speedMult);
        UpdateAnimationSpd(speedMult);

        Audio.PlayAudio(gameObject, "Play_Moff_Gideon_Lightsaber_Turn_Off");
        Audio.PlayAudio(gameObject, "Play_Moff_Gideon_Death");
        Audio.PlayAudio(gameObject, "Play_Victory_Music");

        Input.PlayHaptic(1f, 1000);

        if (cameraComp != null)
            cameraComp.target = this.gameObject;
        if (chargeVisualFeedback != null)
            InternalCalls.Destroy(chargeVisualFeedback);

        //TODO: Delete Deathtroopers!
    }

    private void UpdateDie()
    { }

    public void Die()
    {
        Counter.SumToCounterType(Counter.CounterTypes.MOFFGIDEON);
        EnemyManager.RemoveEnemy(gameObject);

        Animator.Pause(gameObject);
        Audio.StopAudio(gameObject);
        Input.PlayHaptic(0.3f, 3);
        if (cameraComp != null)
            cameraComp.target = Core.instance.gameObject;

        InternalCalls.Destroy(gameObject);
    }

    #endregion

    #region HIT_EVENTS

    public void TakeDamage(float damage)
    {
        if (!DebugOptionsHolder.bossDmg)
        {
            float mod = 1;
            if (Core.instance != null && Core.instance.HasStatus(STATUS_TYPE.GEOTERMAL_MARKER))
            {
                if (HasNegativeStatus())
                {
                    mod = 1 + GetStatusData(STATUS_TYPE.GEOTERMAL_MARKER).severity / 100;
                }
            }
            currentHealthPoints -= damage * mod;
            Debug.Log("Moff damage" + damage.ToString());
            if (currentState != STATE.DEAD)
            {
                currentHealthPoints -= damage * Core.instance.DamageToBosses;
                if (Core.instance != null)
                {
                    if (Core.instance.HasStatus(STATUS_TYPE.WRECK_HEAVY_SHOT) && HasStatus(STATUS_TYPE.SLOWED))
                        AddStatus(STATUS_TYPE.ENEMY_SLOWED, STATUS_APPLY_TYPE.SUBSTITUTE, Core.instance.GetStatusData(STATUS_TYPE.WRECK_HEAVY_SHOT).severity / 100, 3f);

                    if (Core.instance.HasStatus(STATUS_TYPE.LIFESTEAL))
                    {
                        Random rand = new Random();
                        float result = rand.Next(1, 101);
                        if (result <= 10)
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
                if (currentHealthPoints <= 0.0f)
                {
                    if (currentPhase == PHASE.PHASE1)
                    {
                        inputsList.Add(INPUT.IN_CHANGE_PHASE);
                    }
                    else
                        inputsList.Add(INPUT.IN_DEAD);
                }
            }
        }
    }

    private void UpdateDamaged()
    {
        limboHealth = Mathf.Lerp(limboHealth, currentHealthPoints, 0.01f);

        if (bossBarMat != null)
        {
            if (currentPhase == PHASE.PHASE1)
            {
                bossBarMat.SetFloatUniform("length_used", currentHealthPoints / maxHealthPoints1);
                bossBarMat.SetFloatUniform("limbo", limboHealth / maxHealthPoints1);
            }
            else if (currentPhase == PHASE.PHASE2)
            {
                bossBarMat.SetFloatUniform("length_used", currentHealthPoints / maxHealthPoints2);
                bossBarMat.SetFloatUniform("limbo", limboHealth / maxHealthPoints2);
            }
        }

        //TODO: Make moff shine red
        //if (damaged > 0.01f)
        //{
        //    damaged = Mathf.Lerp(damaged, 0.0f, 0.1f);
        //}
        //else
        //{
        //    damaged = 0.0f;
        //}

        //if (moff_mesh != null)
        //{
        //    Material moffMeshMat = moff_mesh.GetComponent<Material>();

        //    if (moffMeshMat != null)
        //    {
        //        moffMeshMat.SetFloatUniform("damaged", damaged);
        //    }

        //}
    }

    #endregion

    #region COLLISION EVENTS
    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("Bullet"))
        {
            if (Core.instance != null)
            {
                if (Core.instance.HasStatus(STATUS_TYPE.MANDO_QUICK_DRAW))
                    AddStatus(STATUS_TYPE.BLASTER_VULN, STATUS_APPLY_TYPE.ADDITIVE, Core.instance.GetStatusData(STATUS_TYPE.MANDO_QUICK_DRAW).severity / 100, 5);
                if (Core.instance.HasStatus(STATUS_TYPE.PRIM_MOV_SPEED))
                    Core.instance.AddStatus(STATUS_TYPE.ACCELERATED, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, Core.instance.GetStatusData(STATUS_TYPE.PRIM_MOV_SPEED).severity / 100, 5, false);
            }

            float damageToBoss = 0f;

            BH_Bullet bulletScript = collidedGameObject.GetComponent<BH_Bullet>();

            if (bulletScript != null)
            {
                damageToBoss += bulletScript.damage;
            }
            else
            {
                Debug.Log("The collider with tag Bullet didn't have a bullet Script!!");
            }

            TakeDamage(damageToBoss * damageRecieveMult * BlasterVulnerability);
            Debug.Log("GIDEON HP: " + currentHealthPoints.ToString());

            PlayHitAudio();

            if (Core.instance.hud != null && currentState != STATE.DEAD)
            {
                HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                if (hudComponent != null)
                {
                    hudComponent.AddToCombo(25, 0.95f);
                }
            }

            if (currentState != STATE.DEAD && currentHealthPoints <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("ChargeBullet"))
        {
            float damageToBoss = 0f;

            ChargedBullet bulletScript = collidedGameObject.GetComponent<ChargedBullet>();

            if (bulletScript != null)
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

                damageToBoss += bulletScript.damage * damageMult;
            }
            else
            {
                Debug.Log("The collider with tag Bullet didn't have a bullet Script!!");
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

            TakeDamage(damageToBoss);
            Debug.Log("Rancor HP: " + currentHealthPoints.ToString());

            PlayHitAudio();

            if (Core.instance.hud != null && currentState != STATE.DEAD)
            {
                HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                if (hudComponent != null)
                {
                    hudComponent.AddToCombo(55, 0.25f);
                }
            }
        }
        else if (collidedGameObject.CompareTag("Player"))
        {
            //TODO: Make him do the Melee Combo! / Deal Damage when dashing through

        }
        else if (collidedGameObject.CompareTag("Wall"))
        {
            //TODO: Stop any kind of dash!
        }
        else if (collidedGameObject.CompareTag("WorldLimit"))
        {
            if (currentState != STATE.DEAD)
            {
                inputsList.Add(INPUT.IN_DEAD);
            }
        }
    }

    private void PlayHitAudio()
    {
        if (currentPhase == PHASE.PHASE1)
        {
            Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Hit_Phase_1");
            Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Intimidation_Phase_1");
        }
        else if (currentPhase == PHASE.PHASE2)
        {
            Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Hit_Phase_2");
            Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Intimidation_Phase_2");
        }
    }

    #endregion

    #region SPAWN_ENEMIES
    private void StartSpawnEnemies()
    {
        CalculateSpawnersScore();

        if (currentPhase == PHASE.PHASE1)
        {
            Animator.Play(gameObject, "MG_EnemySpawnerPh1", speedMult);
        }
        else if (currentPhase == PHASE.PHASE2)
        {
            Animator.Play(gameObject, "MG_EnemySpawnPh2", speedMult);
        }

        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Spawn_Enemies");
        if (cameraComp != null)
            cameraComp.target = this.gameObject;

        Input.PlayHaptic(0.8f, 600);

        SpawnEnemies();
    }

    private void UpdateSpawnEnemies()
    {
        //Debug.Log("Spawning Enemies");
        UpdateAnimationSpd(speedMult);
    }

    private void EndSpawnEnemies()
    {
        enemySkillTimer = enemySkillTime;

        if (cameraComp != null)
            cameraComp.target = Core.instance.gameObject;
    }

    private void SpawnEnemies()
    {
        spawnEnemyTimer = spawnEnemyTime;

        // The 2 closests spawns are selected
        var spawnPointEnum = spawnPoints.GetEnumerator();

        spawnPointEnum.MoveNext();
        SpawnEnemy(spawnPointEnum.Current.Value);

        spawnPointEnum.MoveNext();
        SpawnEnemy(spawnPointEnum.Current.Value);

        ableToSpawnEnemies = false;
    }

    private void SpawnEnemy(GameObject spawnPoint)
    {
        Debug.Log("Spawning enemy... ");

        if (spawnPoint == null)
        {
            Debug.Log("Spawning point was null!!! ");
            return;
        }

        SpawnPoint mySpawnPoint = spawnPoint.GetComponent<SpawnPoint>();

        if (mySpawnPoint != null)
        {
            Random seed = new Random();

            float delay = (float)((seed.NextDouble() * maxEnemySpawnDelay) + baseEnemySpawnDelay);

            mySpawnPoint.QueueSpawnEnemy(delay);
        }
    }

    private void CalculateSpawnersScore()
    {
        if (spawnPoints != null)
            spawnPoints.Clear();
        else
            spawnPoints = new SortedDictionary<float, GameObject>();

        if (spawner1 != null)
        {
            spawnPoints.Add(gameObject.transform.globalPosition.DistanceNoSqrt(spawner1.transform.globalPosition), spawner1);
        }
        if (spawner2 != null)
        {
            spawnPoints.Add(gameObject.transform.globalPosition.DistanceNoSqrt(spawner2.transform.globalPosition), spawner2);
        }
        if (spawner3 != null)
        {
            spawnPoints.Add(gameObject.transform.globalPosition.DistanceNoSqrt(spawner3.transform.globalPosition), spawner3);
        }
        if (spawner4 != null)
        {
            spawnPoints.Add(gameObject.transform.globalPosition.DistanceNoSqrt(spawner4.transform.globalPosition), spawner4);
        }
        if (spawner5 != null)
        {
            spawnPoints.Add(gameObject.transform.globalPosition.DistanceNoSqrt(spawner5.transform.globalPosition), spawner5);
        }
        if (spawner6 != null)
        {
            spawnPoints.Add(gameObject.transform.globalPosition.DistanceNoSqrt(spawner6.transform.globalPosition), spawner6);
        }

    }


    #endregion

    #region STATUS

    protected override void OnInitStatus(ref StatusData statusToInit)
    {
        switch (statusToInit.statusType)
        {
            case STATUS_TYPE.SLOWED:
                {
                    if (this.speedMult <= 0.1f)
                        statusToInit.severity = 0f;

                    this.speedMult -= statusToInit.severity;

                    if (speedMult < 0.1f)
                    {
                        statusToInit.severity = statusToInit.severity - (Math.Abs(this.speedMult) + 0.1f);

                        speedMult = 0.1f;
                    }

                    this.myDeltaTime = Time.deltaTime * speedMult;

                }
                break;
            case STATUS_TYPE.ENEMY_SLOWED:
                {
                    if (this.speedMult <= 0.1f)
                        statusToInit.severity = 0f;

                    this.speedMult -= statusToInit.severity;

                    if (speedMult < 0.1f)
                    {
                        statusToInit.severity = statusToInit.severity - (Math.Abs(this.speedMult) + 0.1f);

                        speedMult = 0.1f;
                    }

                    this.myDeltaTime = Time.deltaTime * speedMult;

                }
                break;
            case STATUS_TYPE.ACCELERATED:
                {
                    this.speedMult += statusToInit.severity;

                    this.myDeltaTime = Time.deltaTime * speedMult;
                }
                break;
            case STATUS_TYPE.ENEMY_DAMAGE_DOWN:
                {
                    this.damageMult -= statusToInit.severity;
                }
                break;
            case STATUS_TYPE.ENEMY_VULNERABLE:
                {
                    this.damageRecieveMult += statusToInit.severity;
                }
                break;
            default:
                break;
        }
    }

    protected override void OnUpdateStatus(StatusData statusToUpdate)
    {
        switch (statusToUpdate.statusType)
        {
            case STATUS_TYPE.ENEMY_BLEED:
                {
                    float damageToTake = statusToUpdate.severity * Time.deltaTime;

                    TakeDamage(damageToTake);
                }
                break;

            default:
                break;
        }
    }


    protected override void OnDeleteStatus(StatusData statusToDelete)
    {
        switch (statusToDelete.statusType)
        {
            case STATUS_TYPE.SLOWED:
                {
                    this.speedMult += statusToDelete.severity;

                    this.myDeltaTime = Time.deltaTime * speedMult;
                }
                break;
            case STATUS_TYPE.ENEMY_SLOWED:
                {
                    this.speedMult += statusToDelete.severity;

                    this.myDeltaTime = Time.deltaTime * speedMult;
                }
                break;
            case STATUS_TYPE.ACCELERATED:
                {
                    this.speedMult -= statusToDelete.severity;

                    this.myDeltaTime = Time.deltaTime * speedMult;
                }
                break;
            case STATUS_TYPE.ENEMY_DAMAGE_DOWN:
                {
                    this.damageMult += statusToDelete.severity;
                }
                break;
            case STATUS_TYPE.ENEMY_VULNERABLE:
                {
                    this.damageRecieveMult -= statusToDelete.severity;
                }
                break;
            default:
                break;
        }
    }

    #endregion

    #region HELPERS
    private void UpdateAnimationSpd(float newSpd)
    {
        if (currAnimationPlaySpd != newSpd)
        {
            Animator.SetSpeed(gameObject, newSpd);
            currAnimationPlaySpd = newSpd;
        }
    }
    public override bool IsDying()
    {
        return currentState == STATE.DEAD;
    }

    #endregion
}