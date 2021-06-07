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

        PRE_BURST_CHARGE,
        PRE_BURST_DASH,
        BURST_1,
        LIGHTNING_DASH_CHARGE,
        LIGHTNING_DASH,
        LIGHTNING_DASH_TIRED,

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
        IN_CHASE_END,
        IN_ACTION_SELECT,

        // Melee combo
        IN_MELEE_COMBO_1_CHARGE,
        IN_MELEE_CHARGE_END,
        IN_MELEE_HIT_END,
        IN_MELEE_DASH_END,

        //Enemy spawn
        IN_SPAWN_ENEMIES,
        IN_SPAWN_ENEMIES_END,

        //Burst
        IN_PRE_BURST_CHARGE,
        IN_PRE_BURST_CHARGE_END,
        IN_PRE_BURST_DASH,
        IN_PRE_BURST_DASH_END,
        IN_BURST1,
        IN_BURST2,
        IN_BURST_END,

        //Lightning dash
        IN_LIGHTNING_DASH_CHARGE,
        IN_LIGHTNING_DASH_CHARGE_END,
        IN_LIGHTNING_DASH_TIRED,
        IN_LIGHTNING_DASH_TIRED_END,

        // Saber Throw
        IN_THROW_SABER,
        IN_THROW_SABER_END,
        IN_CHARGE_THROW,
        IN_RETRIEVE_SABER,
        IN_RETRIEVE_SABER_END,

        // Finishers
        IN_PHASE_CHANGE_END,
        IN_DEAD
    }

    enum PARTICLES : int
    {
        NONE = -1,
        DASH,
        AURA,
        SWORD_SWING
    }

    private NavMeshAgent agent = null;

    public GameObject shootPoint = null;

    public GameObject camera = null;
    private CameraController cameraComp = null;

    public GameObject cape = null;
    public GameObject capeMesh = null;

    public GameObject saber = null;
    public GameObject saberMeshObj = null;
    private MeshRenderer saberMesh = null;
    private Material saberMaterial = null;

    public GameObject gun = null;
    public GameObject gunMeshObj = null;
    private MeshRenderer gunMesh = null;


    // Particles
    public GameObject dashParticleObj = null;
    private ParticleSystem dashParticle = null;

    public GameObject auraParticleObj = null;
    private ParticleSystem auraParticle = null;

    public GameObject auraBurstParticleObj = null;
    private ParticleSystem auraBurstParticle = null;

    public GameObject saberParticlesObj = null;
    private ParticleSystem saberParticles = null;

    //State
    private STATE currentState = STATE.START;
    private List<INPUT> inputsList = new List<INPUT>();
    private PHASE currentPhase = PHASE.PHASE1;
    private bool start = false;

    Random decisionGenerator = new Random();

    //public GameObject hitParticles = null;

    public float slerpRotationSpeed = 5.0f;
    private Vector3 targetPosition = new Vector3(0.0f, 0.0f, 0.0f);

    public float maxHealthPoints1 = 4500.0f;
    public float maxHealthPoints2 = 4500.0f;
    public float currentHealthPoints { get; private set; } = 0.0f;
    private float limboHealth = 0.0f;
    private float damageMult = 1.0f;
    public float damageRecieveMult = 1f;

    private bool straightPath = false;

    private bool isInvencible = false;

    // Animations
    private float currAnimationPlaySpd = 1f;

    //Decision making
    public float probSpawnEnemies_P1 = 40.0f;
    public float maxProbMeleeCombo_P1 = 75.0f;
    public float maxProbBurst_P1 = 75.0f;
    public float minProbBurst_P1 = 25.0f;

    public float minBurstDistance = 12.0f;
    public float maxMeleeDistance = 6.0f;

    //Phase 2
    public float probMeleeCombo_P2 = 20.0f;
    public float probBurst_P2 = 40.0f;
    public float probLightDash = 20.0f; //Onlly phase 2


    // Presentation
    private float presentationTime = 0f;
    private float presentationTimer = 0f;

    //Chase
    public float endChaseDistance = 1.5f;

    public float chaseDuration = 2.0f;
    private float chaseTimer = 0.0f;

    public float chaseSpeed_P1 = 3.0f;
    public float chaseSpeed_P2 = 3.0f;

    //Melee combo
    private const float LAST_FRAME_CONST = 0.05f;
    public float comboChargeDuration = 0.5f;
    private float comboChargeTimer = 0.0f;
    private float chargeComboSpdMult = 1f;

    public float comboDirectionTime = 0.3f;
    private float comboDirectionTimer = 0.0f;

    public float comboLongDashDistance = 10.0f;
    public float comboLongDashSpeed = 5.0f;
    private bool stopDashing = false;
    private Vector3 correctSwingPos = Vector3.zero;
    private float swingCorrectionAngle = 0f;

    public float comboShortDashDistance = 10.0f;
    public float comboShortDashSpeed = 5.0f;

    private float comboDashTimer = 0.0f;
    private float comboDashTime = 0.0f;

    private float meleeHit1Duration = 0.0f;
    private float meleeHit2Duration = 0.0f;
    private float meleeHit3Duration = 0.0f;
    private float meleeHit4Duration = 0.0f;
    private float meleeHit5Duration = 0.0f;
    private float meleeHit6Duration = 0.0f;

    public float meleeHit1SpdMult = 1.0f;
    public float meleeHit2SpdMult = 1.0f;
    public float meleeHit3SpdMult = 1.0f;
    public float meleeHit4SpdMult = 1.0f;
    public float meleeHit5SpdMult = 1.0f;
    public float meleeHit6SpdMult = 1.0f;

    private float meleeHitTimer = 0.0f;

    public float meleeHit1Damage = 2.0f;
    public float meleeHit2Damage = 2.0f;
    public float meleeHit3Damage = 2.0f;
    public float meleeHit4Damage = 2.0f;
    public float meleeHit5Damage = 2.0f;
    public float meleeHit6Damage = 2.0f;

    private float meleeHit1SwingTime = 0.0f;
    private float meleeHit2SwingTime = 0.0f;
    private float meleeHit3SwingTime = 0.0f;
    private float meleeHit4SwingTime = 0.0f;
    private float meleeHit5SwingTime = 0.0f;
    private float meleeHit6SwingTime = 0.0f;

    private float meleeHitSwingTimer = 0.0f;
    private bool launchSwing = false;

    //Enemy spawn
    public float spawnAnimationSpdMult_P1 = 1f;

    public int enemiesToSpawn_P1 = 2;
    public int enemiesToSpawn_P2 = 2;

    private GameObject spawner1 = null;
    private GameObject spawner2 = null;
    private GameObject spawner3 = null;
    private GameObject spawner4 = null;
    private GameObject spawner5 = null;
    private GameObject spawner6 = null;
    private SortedDictionary<float, GameObject> spawnPoints = null;

    public float baseEnemySpawnDelay = 0f;
    public float maxEnemySpawnDelay = 0f;
    public float spawnEnemyTime = 0f;
    private float spawnEnemyTimer = 0f;
    public float enemySkillTime = 0f;
    private float enemySkillTimer = 0f;
    private bool ableToSpawnEnemies = true;

    //Pre burst charge
    private float preBurstChargeDuration = 0.0f; //TODO: NEED TO ADD ANIM
    private float preBurstChargeTimer = 0.0f;

    //Pre burst dash
    public float preBurstDashSpeed = 10.0f;
    public float preBurstDashDistance = 7.0f;
    public float preBurstAngleDash = 45f;

    private float preBurstDashTimer = 0.0f;

    //Burst 1
    public float bulletDamage = 5.0f;

    public int numBurstBullets = 3;

    public float timeBetweenShots = 0.5f;
    public float timeToStartBurst = 1.5f;

    private int shotTimes = 0;
    private float shotTimer = 0.0f;
    private float toStartBurstTimer = 0.0f;


    //Burst 2
    //TODO

    //Lightning dash charge
    public float lightningDashChargeDuration = 0.5f;
    private float lightningDashChargeDurationTimer = 0.0f;

    public float lightningDashDirectionTime = 0.3f;
    private float lightningDashDirectionTimer = 0.0f;

    //Lightining dash
    public float lightningDashLength = 50.0f;
    public float lightningDashSpeed = 50.0f;
    private float lightningDashDuration = 0.0f;

    //Lightning dash tired
    public float lightningDashTiredDuration = 0.5f;
    private float lightningDashTiredDurationTimer = 0.0f;

    //Prepare throw saber
    public float prepSaberThrowDuration = 3.0f;
    private float prepSaberThrowTimer = 0.0f;
    private GameObject chargeVisualFeedback = null;

    //Throw saber
    public float saberThrowDuration = 2.0f;
    private float saberThrowTimer = 0.0f;   //Need to add

    private float saberThrowAnimDuration = 0.0f;
    private float saberThrowAnimTimer = 0.0f;

    // Change Phase
    private float changingPhaseTime = 0f;
    private float changingPhaseTimer = 0f;

    //Die
    private float dieTime = 0f;
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

        changingPhaseTime = Animator.GetAnimationDuration(gameObject, "MG_Rising");

        dieTime = Animator.GetAnimationDuration(gameObject, "MG_Death");

        saberThrowAnimDuration = Animator.GetAnimationDuration(gameObject, "MG_SaberThrow") - 0.016f;

        meleeHit1Duration = Animator.GetAnimationDuration(gameObject, "MG_MeleeCombo1") * (1 / meleeHit1SpdMult) * (1 / meleeHit1SpdMult);
        meleeHit2Duration = Animator.GetAnimationDuration(gameObject, "MG_MeleeCombo2") * (1 / meleeHit2SpdMult) * (1 / meleeHit2SpdMult);
        meleeHit3Duration = Animator.GetAnimationDuration(gameObject, "MG_MeleeCombo3") * (1 / meleeHit3SpdMult);
        meleeHit4Duration = Animator.GetAnimationDuration(gameObject, "MG_MeleeCombo4") * (1 / meleeHit4SpdMult) * (1 / meleeHit4SpdMult);
        meleeHit5Duration = Animator.GetAnimationDuration(gameObject, "MG_MeleeCombo5") * (1 / meleeHit5SpdMult) * (1 / meleeHit5SpdMult);
        meleeHit6Duration = Animator.GetAnimationDuration(gameObject, "MG_MeleeCombo6") * (1 / meleeHit6SpdMult) * (1 / meleeHit6SpdMult);

        meleeHit1SwingTime = meleeHit1Duration * (1 / meleeHit1SpdMult);
        meleeHit2SwingTime = meleeHit2Duration * (1 / meleeHit2SpdMult);
        meleeHit3SwingTime = meleeHit3Duration * (1 / meleeHit3SpdMult) * 0.5f;
        meleeHit4SwingTime = meleeHit4Duration * (1 / meleeHit4SpdMult);
        meleeHit5SwingTime = meleeHit5Duration * (1 / meleeHit5SpdMult);
        meleeHit6SwingTime = meleeHit6Duration * (1 / meleeHit6SpdMult);

        chargeComboSpdMult = Animator.GetAnimationDuration(gameObject, "MG_Swing") / comboChargeDuration;

        saberThrowAnimDuration = Animator.GetAnimationDuration(gameObject, "MG_SaberThrow");
        //preBurstChargeDuration GetAnimationDuration


        // Particles
        if (dashParticleObj != null)
        {
            dashParticle = dashParticleObj.GetComponent<ParticleSystem>();
        }

        if (auraParticleObj != null)
        {
            auraParticle = auraParticleObj.GetComponent<ParticleSystem>();
        }

        if (auraBurstParticleObj != null)
        {
            auraBurstParticle = auraBurstParticleObj.GetComponent<ParticleSystem>();
        }

        if(saberParticlesObj != null)
        {
            saberParticles = saberParticlesObj.GetComponent<ParticleSystem>();
        }


        damaged = 0.0f;

        //Props
        if (cape != null)
            Animator.Pause(cape);

        if (gunMeshObj != null)
        {
            gunMesh = gunMeshObj.GetComponent<MeshRenderer>();
        }

        DeActivateGun();

        if (saberMeshObj != null)
        {
            saberMesh = saberMeshObj.GetComponent<MeshRenderer>();
            saberMaterial = saberMeshObj.GetComponent<Material>();
        }

        DeActivateSaber();

        inputsList.Add(INPUT.IN_PRESENTATION);
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

        if (currentPhase == PHASE.PHASE1)
        {
            ProcessState_P1();
            UpdateState_P1();
        }

        else
        {
            ProcessState_P2();
            UpdateState_P2();
        }


        UpdateDamaged();
    }

    #region STATE_MACHINE
    //Timers go here
    private void ProcessInternalInput()
    {
        //Presentation
        if (presentationTimer > 0)
        {
            presentationTimer -= myDeltaTime;
            if (presentationTimer <= 0)
            {
                Debug.Log("In presentation end");
                inputsList.Add(INPUT.IN_PRESENTATION_END);
            }

        }

        // Chase 
        if (chaseTimer > 0)
        {
            chaseTimer -= myDeltaTime;

            if (chaseTimer <= 0)
                inputsList.Add(INPUT.IN_CHASE_END);
        }

        // Melee combo
        if (comboDashTimer > 0)
        {
            comboDashTimer -= myDeltaTime;

            if (comboDashTimer <= 0 || Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) < agent.stoppingDistance || stopDashing == true)
            {
                inputsList.Add(INPUT.IN_MELEE_DASH_END);
                stopDashing = false;
                comboDashTimer = 0f;
            }
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
                inputsList.Add(INPUT.IN_MELEE_CHARGE_END);
        }

        if (meleeHitSwingTimer > 0)
        {
            meleeHitSwingTimer -= myDeltaTime;

            if (meleeHitSwingTimer <= 0)
                launchSwing = true;
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

        //Burst
        if (preBurstChargeTimer > 0)
        {
            preBurstChargeTimer -= myDeltaTime;

            if (preBurstChargeTimer <= 0)
            {
                inputsList.Add(INPUT.IN_PRE_BURST_CHARGE_END);
            }
        }

        if (preBurstDashTimer > 0)
        {
            preBurstDashTimer -= myDeltaTime;

            if (preBurstDashTimer <= 0 || Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) < agent.stoppingDistance || stopDashing == true)
                inputsList.Add(INPUT.IN_PRE_BURST_DASH_END);
        }

        if (shotTimes >= numBurstBullets && shotTimer <= 0.0f)
        {
            inputsList.Add(INPUT.IN_BURST_END);
            Debug.Log("In burst end input");
        }

        //Lightning dash
        if (lightningDashChargeDurationTimer > 0.0f)
        {
            lightningDashChargeDurationTimer -= myDeltaTime;

            if (lightningDashChargeDurationTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_LIGHTNING_DASH_CHARGE_END);
            }
        }

        if (currentState == STATE.LIGHTNING_DASH && lightningDashDuration > 0.0f)
        {
            lightningDashDuration -= myDeltaTime;
            if (Mathf.Distance(gameObject.transform.globalPosition, targetPosition) <= agent.stoppingDistance || lightningDashDuration < 0.0f)
            {
                inputsList.Add(INPUT.IN_LIGHTNING_DASH_TIRED);
            }
        }

        if (lightningDashTiredDurationTimer > 0.0f)
        {
            lightningDashTiredDurationTimer -= myDeltaTime;

            if (lightningDashTiredDurationTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_LIGHTNING_DASH_TIRED_END);
            }
        }

        //Throw 
        if (prepSaberThrowTimer > 0.0f)
        {
            prepSaberThrowTimer -= myDeltaTime;

            if (prepSaberThrowTimer <= 0.0f)
                inputsList.Add(INPUT.IN_THROW_SABER);
        }

        if (saberThrowAnimTimer > 0.0f)
        {
            saberThrowAnimTimer -= myDeltaTime;

            if (saberThrowAnimTimer <= 0.0f)
                inputsList.Add(INPUT.IN_RETRIEVE_SABER);
        }

        //Change phase
        if (changingPhaseTimer > 0.0f)
        {
            changingPhaseTimer -= myDeltaTime;

            if (changingPhaseTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_PHASE_CHANGE_END);
            }
        }


        //Dead
        if (dieTimer > 0.0f)
        {
            dieTimer -= myDeltaTime;

            if (dieTimer <= 0.0f)
            {
                Die();
            }
        }
    }

    private void ProcessState_P1()
    {
        while (inputsList.Count > 0)
        {
            INPUT input = inputsList[0];

            switch (currentState)
            {
                case STATE.NONE:
                    Debug.Log("Moff gideon ERROR STATE");
                    break;

                case STATE.START:
                    switch (input)
                    {
                        case INPUT.IN_PRESENTATION:
                            StartPresentation();

                            currentState = STATE.PRESENTATION;
                            break;
                    }
                    break;

                case STATE.PRESENTATION:
                    switch (input)
                    {
                        case INPUT.IN_PRESENTATION_END:
                            EndPresentation();
                            StartChase_P1();
                            currentState = STATE.CHASE;
                            break;
                    }
                    break;

                case STATE.CHASE:
                    switch (input)
                    {
                        case INPUT.IN_CHASE_END:
                            EndChase_P1();
                            currentState = STATE.ACTION_SELECT;
                            break;

                        case INPUT.IN_DEAD:
                            currentState = STATE.CHANGE_PHASE;
                            EndChase_P1();
                            StartPhaseChange(); //Change for die or something
                            break;
                    }
                    break;

                case STATE.ACTION_SELECT:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_COMBO_1_CHARGE:
                            StartMeleeCombo1Charge();
                            currentState = STATE.MELEE_COMBO_1_CHARGE;
                            break;

                        case INPUT.IN_PRE_BURST_CHARGE:
                            StartBurstCharge();
                            currentState = STATE.PRE_BURST_CHARGE;
                            break;

                        case INPUT.IN_SPAWN_ENEMIES:
                            StartSpawnEnemies();
                            currentState = STATE.SPAWN_ENEMIES;
                            break;

                        case INPUT.IN_DEAD:
                            StartPhaseChange(); //Change for die or something
                            currentState = STATE.CHANGE_PHASE;
                            break;

                    }
                    break;

                case STATE.MELEE_COMBO_1_CHARGE:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_CHARGE_END:
                            EndMeleeCombo1Charge();
                            StartMeleeComboDash1();
                            currentState = STATE.MELEE_COMBO_1_DASH;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeCombo1Charge();
                            StartPhaseChange(); //Change for die or something
                            currentState = STATE.CHANGE_PHASE;
                            break;
                    }
                    break;

                case STATE.MELEE_COMBO_1_DASH:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_DASH_END:
                            EndMeleeComboDash1();
                            StartMeleeComboHit1();
                            currentState = STATE.MELEE_COMBO_1;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboDash1();
                            StartPhaseChange(); //Change for die or something
                            currentState = STATE.CHANGE_PHASE;
                            break;
                    }
                    break;
                case STATE.MELEE_COMBO_1:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_HIT_END:
                            EndMeleeComboHit1();
                            StartMeleeComboDash2();
                            currentState = STATE.MELEE_COMBO_2_DASH;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboHit1();
                            StartPhaseChange(); //Change for die or something
                            currentState = STATE.CHANGE_PHASE;
                            break;
                    }
                    break;
                case STATE.MELEE_COMBO_2_DASH:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_DASH_END:
                            EndMeleeComboDash2();
                            StartMeleeComboHit2();
                            currentState = STATE.MELEE_COMBO_2;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboDash2();
                            StartPhaseChange(); //Change for die or something
                            currentState = STATE.CHANGE_PHASE;
                            break;
                    }
                    break;
                case STATE.MELEE_COMBO_2:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_HIT_END:
                            EndMeleeComboHit2();
                            StartMeleeComboDash3();
                            currentState = STATE.MELEE_COMBO_3_DASH;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboHit2();
                            StartPhaseChange(); //Change for die or something
                            currentState = STATE.CHANGE_PHASE;
                            break;
                    }
                    break;
                case STATE.MELEE_COMBO_3_DASH:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_DASH_END:
                            EndMeleeComboDash3();
                            StartMeleeComboHit3();
                            currentState = STATE.MELEE_COMBO_3;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboDash3();
                            StartPhaseChange(); //Change for die or something
                            currentState = STATE.CHANGE_PHASE;
                            break;
                    }
                    break;
                case STATE.MELEE_COMBO_3:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_HIT_END:
                            EndMeleeComboHit3();
                            StartChase_P1();
                            currentState = STATE.CHASE;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboHit3();
                            StartPhaseChange(); //Change for die or something
                            currentState = STATE.CHANGE_PHASE;
                            break;
                    }
                    break;

                case STATE.SPAWN_ENEMIES:
                    switch (input)
                    {
                        case INPUT.IN_SPAWN_ENEMIES_END:
                            EndSpawnEnemies();
                            StartChase_P1();
                            currentState = STATE.CHASE;
                            break;

                        case INPUT.IN_DEAD:
                            EndSpawnEnemies();
                            StartPhaseChange(); //Change for die or something
                            currentState = STATE.CHANGE_PHASE;
                            break;
                    }
                    break;

                case STATE.PRE_BURST_CHARGE:
                    switch (input)
                    {
                        case INPUT.IN_PRE_BURST_CHARGE_END:
                            EndBurstCharge();
                            StartBurstDash();
                            currentState = STATE.PRE_BURST_DASH;
                            break;

                        case INPUT.IN_DEAD:
                            EndBurstCharge();
                            StartPhaseChange(); //Change for die or something
                            currentState = STATE.CHANGE_PHASE;
                            break;
                    }
                    break;

                case STATE.PRE_BURST_DASH:
                    switch (input)
                    {
                        case INPUT.IN_PRE_BURST_DASH_END:
                            EndBurstDash();
                            StartBurst_P1();
                            currentState = STATE.BURST_1;
                            break;

                        case INPUT.IN_DEAD:
                            EndBurstDash();
                            StartPhaseChange(); //Change for die or something
                            currentState = STATE.CHANGE_PHASE;
                            break;
                    }
                    break;

                case STATE.BURST_1:
                    switch (input)
                    {
                        case INPUT.IN_BURST_END:
                            EndBurst_P1();
                            StartChase_P1();
                            currentState = STATE.CHASE;
                            break;

                        case INPUT.IN_DEAD:
                            EndBurst_P1();
                            StartPhaseChange(); //Change for die or something
                            currentState = STATE.CHANGE_PHASE;
                            break;
                    }
                    break;

                case STATE.CHANGE_PHASE:
                    switch (input)
                    {
                        case INPUT.IN_PHASE_CHANGE_END:
                            EndPhaseChange();
                            StartChase_P2();
                            currentState = STATE.CHASE;
                            break;
                    }
                    break;
            }
            inputsList.RemoveAt(0);
        }
    }

    private void ProcessState_P2()
    {
        while (inputsList.Count > 0)
        {
            INPUT input = inputsList[0];

            switch (currentState)
            {
                case STATE.NONE:
                    Debug.Log("Moff gideon ERROR STATE");
                    break;

                case STATE.START:
                    switch (input)
                    {
                        case INPUT.IN_PRESENTATION:
                            StartPresentation();

                            currentState = STATE.PRESENTATION;
                            break;
                    }
                    break;

                case STATE.PRESENTATION:
                    switch (input)
                    {
                        case INPUT.IN_PRESENTATION_END:
                            EndPresentation();
                            StartChase_P2();
                            currentState = STATE.CHASE;
                            break;
                    }
                    break;

                case STATE.CHASE:
                    switch (input)
                    {
                        case INPUT.IN_CHASE_END:
                            EndChase_P2();
                            currentState = STATE.ACTION_SELECT;
                            break;

                        case INPUT.IN_DEAD:
                            EndChase_P2();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;

                case STATE.ACTION_SELECT:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_COMBO_1_CHARGE:
                            StartMeleeCombo1Charge();
                            currentState = STATE.MELEE_COMBO_1_CHARGE;
                            break;

                        case INPUT.IN_PRE_BURST_CHARGE:
                            StartBurstCharge();
                            currentState = STATE.PRE_BURST_CHARGE;
                            break;

                        case INPUT.IN_SPAWN_ENEMIES:
                            StartSpawnEnemies();
                            currentState = STATE.SPAWN_ENEMIES;
                            break;

                        case INPUT.IN_LIGHTNING_DASH_CHARGE:
                            StartLightningDashCharge();
                            currentState = STATE.LIGHTNING_DASH_CHARGE;
                            break;

                        case INPUT.IN_DEAD:
                            StartDie(); //Change for die or something
                            currentState = STATE.DEAD;
                            break;

                    }
                    break;

                case STATE.MELEE_COMBO_1_CHARGE:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_CHARGE_END:
                            EndMeleeCombo1Charge();
                            StartMeleeComboDash1();
                            currentState = STATE.MELEE_COMBO_1_DASH;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeCombo1Charge();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;

                case STATE.MELEE_COMBO_1_DASH:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_DASH_END:
                            EndMeleeComboDash1();
                            StartMeleeComboHit1();
                            currentState = STATE.MELEE_COMBO_1;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboDash1();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;
                case STATE.MELEE_COMBO_1:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_HIT_END:
                            EndMeleeComboHit1();
                            StartMeleeComboDash2();
                            currentState = STATE.MELEE_COMBO_2_DASH;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboHit1();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;
                case STATE.MELEE_COMBO_2_DASH:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_DASH_END:
                            EndMeleeComboDash2();
                            StartMeleeComboHit2();
                            currentState = STATE.MELEE_COMBO_2;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboDash2();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;
                case STATE.MELEE_COMBO_2:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_HIT_END:
                            EndMeleeComboHit2();
                            StartMeleeComboDash3();
                            currentState = STATE.MELEE_COMBO_3_DASH;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboHit2();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;
                case STATE.MELEE_COMBO_3_DASH:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_DASH_END:
                            EndMeleeComboDash3();
                            StartMeleeComboHit3();
                            currentState = STATE.MELEE_COMBO_3;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboDash3();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;
                case STATE.MELEE_COMBO_3:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_HIT_END:
                            EndMeleeComboHit3();
                            StartMeleeCombo4Charge();
                            currentState = STATE.MELEE_COMBO_4_CHARGE;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboHit3();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;

                case STATE.MELEE_COMBO_4_CHARGE:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_CHARGE_END:
                            EndMeleeCombo4Charge();
                            StartMeleeComboDash4();
                            currentState = STATE.MELEE_COMBO_4_DASH;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeCombo4Charge();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;

                case STATE.MELEE_COMBO_4_DASH:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_DASH_END:
                            EndMeleeComboDash4();
                            StartMeleeComboHit4();
                            currentState = STATE.MELEE_COMBO_4;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboDash1();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;
                case STATE.MELEE_COMBO_4:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_HIT_END:
                            EndMeleeComboHit4();
                            StartMeleeComboDash5();
                            currentState = STATE.MELEE_COMBO_5_DASH;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboHit4();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;
                case STATE.MELEE_COMBO_5_DASH:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_DASH_END:
                            EndMeleeComboDash5();
                            StartMeleeComboHit5();
                            currentState = STATE.MELEE_COMBO_5;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboDash5();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;
                case STATE.MELEE_COMBO_5:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_HIT_END:
                            EndMeleeComboHit5();
                            StartMeleeComboDash6();
                            currentState = STATE.MELEE_COMBO_6_DASH;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboHit5();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;
                case STATE.MELEE_COMBO_6_DASH:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_DASH_END:
                            EndMeleeComboDash6();
                            StartMeleeComboHit6();
                            currentState = STATE.MELEE_COMBO_6;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboDash6();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;

                case STATE.MELEE_COMBO_6:
                    switch (input)
                    {
                        case INPUT.IN_MELEE_HIT_END:
                            EndMeleeComboHit6();
                            StartChase_P2();
                            currentState = STATE.CHASE;
                            break;

                        case INPUT.IN_DEAD:
                            EndMeleeComboHit6();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;

                case STATE.SPAWN_ENEMIES:
                    switch (input)
                    {
                        case INPUT.IN_SPAWN_ENEMIES_END:
                            EndSpawnEnemies();
                            StartChase_P2();
                            currentState = STATE.CHASE;
                            break;

                        case INPUT.IN_DEAD:
                            EndSpawnEnemies();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;

                case STATE.PRE_BURST_CHARGE:
                    switch (input)
                    {
                        case INPUT.IN_PRE_BURST_CHARGE_END:
                            EndBurstCharge();
                            StartBurstDash();
                            currentState = STATE.PRE_BURST_DASH;
                            break;

                        case INPUT.IN_DEAD:
                            EndBurstCharge();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;

                case STATE.PRE_BURST_DASH:
                    switch (input)
                    {
                        case INPUT.IN_PRE_BURST_DASH_END:
                            EndBurstDash();
                            StartBurst_P2();
                            currentState = STATE.BURST_1;
                            break;

                        case INPUT.IN_DEAD:
                            EndBurstDash();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;

                case STATE.BURST_1:
                    switch (input)
                    {
                        case INPUT.IN_BURST_END:
                            EndBurst_P2();
                            StartChase_P2();
                            currentState = STATE.CHASE;
                            break;

                        case INPUT.IN_DEAD:
                            EndBurst_P2();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;

                case STATE.LIGHTNING_DASH_CHARGE:
                    switch (input)
                    {
                        case INPUT.IN_LIGHTNING_DASH_CHARGE_END:
                            EndLightningDashCharge();
                            StartLightningDash();
                            currentState = STATE.LIGHTNING_DASH;
                            break;

                        case INPUT.IN_DEAD:
                            EndLightningDashCharge();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;

                case STATE.LIGHTNING_DASH:
                    switch (input)
                    {
                        case INPUT.IN_LIGHTNING_DASH_TIRED:
                            EndLightningDash();
                            StartLightningDashTired();
                            currentState = STATE.LIGHTNING_DASH_TIRED;
                            break;

                        case INPUT.IN_DEAD:
                            EndLightningDash();
                            StartDie();
                            currentState = STATE.DEAD;
                            break;
                    }
                    break;

                case STATE.LIGHTNING_DASH_TIRED:
                    {
                        switch (input)
                        {
                            case INPUT.IN_LIGHTNING_DASH_TIRED_END:
                                EndLightningDash();
                                StartChase_P2();
                                currentState = STATE.CHASE;
                                break;

                            case INPUT.IN_DEAD:
                                EndLightningDash();
                                StartDie();
                                currentState = STATE.DEAD;
                                break;
                        }
                    }
                    break;

                case STATE.CHANGE_PHASE:
                    switch (input)
                    {
                        case INPUT.IN_PHASE_CHANGE_END:
                            EndPhaseChange();
                            StartChase_P2();
                            currentState = STATE.CHASE;
                            break;
                    }
                    break;

                case STATE.DEAD:
                    //TODO: Fill this bad boy
                    break;
            }
            inputsList.RemoveAt(0);
        }
    }

    #region UpdateState_P1
    private void UpdateState_P1()
    {
        switch (currentState)
        {
            case STATE.NONE:
                Debug.Log("MOFF GIDEON ERROR STATE");
                break;

            case STATE.PRESENTATION:
                UpdatePresentation();
                break;

            case STATE.CHASE:
                UpdateChase_P1();
                break;

            case STATE.ACTION_SELECT:
                UpdateActionSelect_P1();
                break;

            case STATE.MELEE_COMBO_1_CHARGE:
                UpdateMeleeCombo1Charge();
                break;

            case STATE.MELEE_COMBO_1_DASH:
                UpdateMeleeComboDash1();
                break;

            case STATE.MELEE_COMBO_1:
                UpdateMeleeComboHit1();
                break;

            case STATE.MELEE_COMBO_2_DASH:
                UpdateMeleeComboDash2();
                break;

            case STATE.MELEE_COMBO_2:
                UpdateMeleeComboHit2();
                break;

            case STATE.MELEE_COMBO_3_DASH:
                UpdateMeleeComboDash3();
                break;

            case STATE.MELEE_COMBO_3:
                UpdateMeleeComboHit3();
                break;

            case STATE.SPAWN_ENEMIES:
                UpdateSpawnEnemies();
                break;

            case STATE.PRE_BURST_CHARGE:
                UpdateBurstCharge();
                break;

            case STATE.PRE_BURST_DASH:
                UpdateBurstDash();
                break;

            case STATE.BURST_1:
                UpdateBurst_P1();
                break;

            case STATE.CHANGE_PHASE:
                UpdatePhaseChange();
                break;

            case STATE.DEAD:
                UpdateDie();
                break;
            default:
                Debug.Log("MOFF GIDEON ERROR STATE");
                break;
        }
    }
    #endregion

    #region UpdateState_P2
    private void UpdateState_P2()
    {
        switch (currentState)
        {
            case STATE.NONE:
                Debug.Log("MOFF GIDEON ERROR STATE");
                break;

            case STATE.CHASE:
                UpdateChase_P2();
                break;

            case STATE.ACTION_SELECT:
                UpdateActionSelect_P2();
                break;

            case STATE.MELEE_COMBO_1_CHARGE:
                UpdateMeleeCombo1Charge();
                break;

            case STATE.MELEE_COMBO_1_DASH:
                UpdateMeleeComboDash1();
                break;

            case STATE.MELEE_COMBO_1:
                UpdateMeleeComboHit1();
                break;

            case STATE.MELEE_COMBO_2_DASH:
                UpdateMeleeComboDash2();
                break;

            case STATE.MELEE_COMBO_2:
                UpdateMeleeComboHit2();
                break;

            case STATE.MELEE_COMBO_3_DASH:
                UpdateMeleeComboDash3();
                break;

            case STATE.MELEE_COMBO_3:
                UpdateMeleeComboHit3();
                break;

            case STATE.MELEE_COMBO_4_CHARGE:
                UpdateMeleeCombo4Charge();
                break;

            case STATE.MELEE_COMBO_4_DASH:
                UpdateMeleeComboDash4();
                break;

            case STATE.MELEE_COMBO_4:
                UpdateMeleeComboHit4();
                break;

            case STATE.MELEE_COMBO_5_DASH:
                UpdateMeleeComboDash5();
                break;

            case STATE.MELEE_COMBO_5:
                UpdateMeleeComboHit5();
                break;

            case STATE.MELEE_COMBO_6_DASH:
                UpdateMeleeComboDash6();
                break;

            case STATE.MELEE_COMBO_6:
                UpdateMeleeComboHit6();
                break;

            case STATE.SPAWN_ENEMIES:
                UpdateSpawnEnemies();
                break;

            case STATE.PRE_BURST_CHARGE:
                UpdateBurstCharge();
                break;

            case STATE.PRE_BURST_DASH:
                UpdateBurstDash();
                break;

            case STATE.BURST_1:
                UpdateBurst_P2();
                break;

            case STATE.CHANGE_PHASE:
                UpdatePhaseChange();
                break;

            case STATE.DEAD:
                UpdateDie();
                break;
            default:
                Debug.Log("MOFF GIDEON ERROR STATE");
                break;
        }
    }
    #endregion

    #endregion

    #region ACTION_SELECT
    private void UpdateActionSelect_P1()
    {
        int decision = decisionGenerator.Next(1, 100);

        if (ableToSpawnEnemies == true)
        {
            if (decision <= probSpawnEnemies_P1)
                inputsList.Add(INPUT.IN_SPAWN_ENEMIES);

            else
                DecideAttack();
        }

        else
            DecideAttack();
    }


    private void UpdateActionSelect_P2()    //TODO: ADD LIGHTNING DASH PROVAVILITY
    {
        int decision = decisionGenerator.Next(1, 100);

        if (ableToSpawnEnemies == true)
        {
            if (decision <= probSpawnEnemies_P1)
                inputsList.Add(INPUT.IN_SPAWN_ENEMIES);

            else
                DecideAttack();
        }

        else
            DecideAttack();
    }


    private void DecideAttack()
    {
        float distance = Mathf.Distance(Core.instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition);
        float decision = decisionGenerator.Next(1, 100);

        if (distance >= minBurstDistance)
        {
            if (decision <= maxProbBurst_P1)
                inputsList.Add(INPUT.IN_PRE_BURST_CHARGE);

            else
                inputsList.Add(INPUT.IN_MELEE_COMBO_1_CHARGE);
        }

        else if (distance <= maxMeleeDistance)
        {
            if (decision <= maxProbMeleeCombo_P1)
                inputsList.Add(INPUT.IN_MELEE_COMBO_1_CHARGE);

            else
                inputsList.Add(INPUT.IN_PRE_BURST_CHARGE);
        }

        else
        {
            float probability = Mathf.Lerp(maxMeleeDistance, minBurstDistance, distance) * 100;

            if (maxProbBurst_P1 * probability < minProbBurst_P1)
                probability = minProbBurst_P1 / maxProbBurst_P1;


            if (decision <= maxProbBurst_P1 * probability)
                inputsList.Add(INPUT.IN_PRE_BURST_CHARGE);

            else
                inputsList.Add(INPUT.IN_MELEE_COMBO_1_CHARGE);
        }
    }
    #endregion

    #region CHASE
    #region CHASE_P1
    private void StartChase_P1()
    {
        chaseTimer = chaseDuration;

        Animator.Play(gameObject, "MG_RunPh1_Final", speedMult);
        if (saber != null)
        {
            DeActivateSaber();
        }

        if (gun != null)
        {
            DeActivateGun();
        }

        UpdateAnimationSpd(speedMult);



        Debug.Log("Start chase");
    }

    private void UpdateChase_P1()
    {
        if (agent != null && Core.instance != null)
        {
            agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
        }

        if (Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) < endChaseDistance)
            inputsList.Add(INPUT.IN_CHASE_END);

        Mathf.LookAt(ref this.gameObject.transform, agent.GetDestination());
        agent.MoveToCalculatedPos(chaseSpeed_P1 * speedMult);

        UpdateAnimationSpd(speedMult);

        Debug.Log("Update chase");
    }

    private void EndChase_P1()
    {
        chaseTimer = 0.0f;
        Debug.Log("End chase");
    }
    #endregion

    #region CHASE_P2
    private void StartChase_P2()
    {
        chaseTimer = chaseDuration;

        Animator.Play(gameObject, "MG_RunPh2", speedMult);
        if (saber != null)
        {
            ActivateSaber();
        }

        if (gun != null)
        {
            DeActivateGun();
        }

        UpdateAnimationSpd(speedMult);
    }

    private void UpdateChase_P2()
    {
        if (agent != null && Core.instance != null)
        {
            agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
        }

        if (Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) < endChaseDistance)
            inputsList.Add(INPUT.IN_CHASE_END);

        Mathf.LookAt(ref this.gameObject.transform, agent.GetDestination());
        agent.MoveToCalculatedPos(chaseSpeed_P2 * speedMult);

        UpdateAnimationSpd(speedMult);
    }

    private void EndChase_P2()
    {
        chaseTimer = 0.0f;
    }
    #endregion

    #endregion

    #region MELEE_COMBO
    #region MELEE_COMBO_CHARGE
    private void StartMeleeCombo1Charge()
    {
        comboChargeTimer = comboChargeDuration;
        comboDirectionTimer = comboDirectionTime;

        Animator.Play(gameObject, "MG_Swing", speedMult * chargeComboSpdMult);
        if (saber != null)
        {
            ActivateSaber();
            Animator.Play(saber, "MG_Swing", speedMult * chargeComboSpdMult);

            if (saberMaterial != null)
            {
                saberMaterial.SetFloatUniform("shineColorValue", 0);
                saberMaterial.SetVectorUniform("shineColor", new Vector3(0.9f, 1.0f, 0.0f));
            }
        }
        if (gun != null)
        {
            DeActivateGun();
        }

        UpdateAnimationSpd(speedMult * chargeComboSpdMult);
    }

    private void UpdateMeleeCombo1Charge()
    {
        if (saberMaterial != null)
        {
            float shineValue = comboChargeTimer / comboChargeDuration;

            saberMaterial.SetFloatUniform("shineColorValue", shineValue);
        }

        if (comboDirectionTimer > 0.0f)
        {
            comboDirectionTimer -= myDeltaTime;

            if (comboDirectionTimer > LAST_FRAME_CONST)
            {
                if (Core.instance != null)
                {
                    Vector3 direction = Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition;

                    if (Mathf.Distance(Core.instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition) > comboLongDashDistance)
                    {
                        targetPosition = direction.normalized * comboLongDashDistance + gameObject.transform.globalPosition;
                    }
                    else
                    {
                        targetPosition = Core.instance.gameObject.transform.globalPosition;
                    }

                    agent.CalculatePath(gameObject.transform.globalPosition, targetPosition);
                }
            }
        }

        Mathf.LookAt(ref this.gameObject.transform, agent.GetDestination());
        UpdateAnimationSpd(speedMult * chargeComboSpdMult);
    }

    private void EndMeleeCombo1Charge()
    {
        if (saberMaterial != null)
            saberMaterial.SetFloatUniform("shineColorValue", 0);
    }


    private void StartMeleeCombo4Charge()
    {
        comboChargeTimer = comboChargeDuration;
        comboDirectionTimer = comboDirectionTime;

        Animator.Play(gameObject, "MG_Swing", speedMult * chargeComboSpdMult);
        if (saber != null)
        {
            ActivateSaber();
            Animator.Play(saber, "MG_Swing", speedMult * chargeComboSpdMult);

            if (saberMaterial != null)
            {
                saberMaterial.SetFloatUniform("shineColorValue", 0);
                saberMaterial.SetVectorUniform("shineColor", new Vector3(0.9f, 1.0f, 0.0f));
            }
        }
        if (gun != null)
        {
            DeActivateGun();
        }

        UpdateAnimationSpd(speedMult * chargeComboSpdMult);
    }

    private void UpdateMeleeCombo4Charge()
    {
        if (saberMaterial != null)
        {
            float shineValue = comboChargeTimer / comboChargeDuration;

            saberMaterial.SetFloatUniform("shineColorValue", shineValue);
        }

        if (comboDirectionTimer > 0.0f)
        {
            comboDirectionTimer -= myDeltaTime;

            if (comboDirectionTimer > LAST_FRAME_CONST)
            {
                if (Core.instance != null)
                {
                    Vector3 direction = Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition;

                    if (Mathf.Distance(Core.instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition) > comboLongDashDistance)
                    {
                        targetPosition = direction.normalized * comboLongDashDistance + gameObject.transform.globalPosition;
                    }
                    else
                    {
                        targetPosition = Core.instance.gameObject.transform.globalPosition;
                    }

                    agent.CalculatePath(gameObject.transform.globalPosition, targetPosition);
                }
            }
        }

        Mathf.LookAt(ref this.gameObject.transform, agent.GetDestination());
        UpdateAnimationSpd(speedMult * chargeComboSpdMult);
    }

    private void EndMeleeCombo4Charge()
    {
        if (saberMaterial != null)
            saberMaterial.SetFloatUniform("shineColorValue", 0);
    }

    #endregion

    #region MELEE_COMBO_DASH

    //Long dash 1
    private void StartMeleeComboDash1()
    {
        Debug.Log("Start melee combo dash 1");

        stopDashing = false;
        comboDashTimer = comboDashTime = (comboLongDashDistance / comboLongDashSpeed) * speedMult;

        Animator.Play(gameObject, "MG_Dash", speedMult);
        if (saber != null)
        {
            DeActivateSaber();
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult);

        //PLAY AUDIOS
        //Audio.PlayAudio(gameObject, "AAAAAAAAAAAA");

        //StraightPath();   //IF WE NEED TO DO SOMETHING WITH NOT STRAIGHT PATHS

        PlayParticles(PARTICLES.DASH);

        Mathf.LookAt(ref this.gameObject.transform, agent.GetDestination());
    }

    private void UpdateMeleeComboDash1()
    {
        Debug.Log("Update melee combo dash 1");

        if (comboDashTimer > comboDashTime * 0.5f && Core.instance != null)
        {
            correctSwingPos = Core.instance.gameObject.transform.globalPosition;
        }


        agent.MoveToCalculatedPos(comboLongDashSpeed * speedMult);

        UpdateAnimationSpd(speedMult);
    }

    private void EndMeleeComboDash1()
    {
        Debug.Log("End melee combo dash 1");
    }

    //Short dash 2
    private void StartMeleeComboDash2()
    {
        Debug.Log("Start melee combo dash 2");

        stopDashing = false;
        comboDashTimer = comboDashTime = (comboShortDashDistance / comboShortDashSpeed) * speedMult;

        Animator.Play(gameObject, "MG_Dash", speedMult);
        if (saber != null)
        {
            DeActivateSaber();
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult);

        //PLAY AUDIOS
        //Audio.PlayAudio(gameObject, "AAAAAAAAAAAA");

        //StraightPath();   //IF WE NEED TO DO SOMETHING WITH NOT STRAIGHT PATHS

        PlayParticles(PARTICLES.DASH);

        Vector3 direction = Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition;
        if (Mathf.Distance(Core.instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition) > comboShortDashDistance)
        {
            targetPosition = direction.normalized * comboShortDashDistance + gameObject.transform.globalPosition;
        }
        else
        {
            targetPosition = Core.instance.gameObject.transform.globalPosition;
        }

        agent.CalculatePath(gameObject.transform.globalPosition, targetPosition);
        Mathf.LookAt(ref this.gameObject.transform, agent.GetDestination());

    }

    private void UpdateMeleeComboDash2()
    {
        Debug.Log("Update melee combo dash 2");

        if (comboDashTimer > comboDashTime * 0.5f && Core.instance != null)
        {
            correctSwingPos = Core.instance.gameObject.transform.globalPosition;
        }

        agent.MoveToCalculatedPos(comboShortDashSpeed * speedMult);

        UpdateAnimationSpd(speedMult);
    }

    private void EndMeleeComboDash2()
    {
        Debug.Log("End melee combo dash 2");
    }

    //Short dash 3
    private void StartMeleeComboDash3()
    {
        Debug.Log("Start melee combo dash 3");


        stopDashing = false;
        comboDashTimer = comboDashTime = (comboShortDashDistance / comboShortDashSpeed) * speedMult;

        Animator.Play(gameObject, "MG_Dash", speedMult);
        if (saber != null)
        {
            DeActivateSaber();
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult);

        //PLAY AUDIOS
        //Audio.PlayAudio(gameObject, "AAAAAAAAAAAA");

        //StraightPath();   //IF WE NEED TO DO SOMETHING WITH NOT STRAIGHT PATHS

        PlayParticles(PARTICLES.DASH);

        Vector3 direction = Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition;
        if (Mathf.Distance(Core.instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition) > comboShortDashDistance)
        {
            targetPosition = direction.normalized * comboShortDashDistance + gameObject.transform.globalPosition;
        }
        else
        {
            targetPosition = Core.instance.gameObject.transform.globalPosition;
        }

        agent.CalculatePath(gameObject.transform.globalPosition, targetPosition);
        Mathf.LookAt(ref this.gameObject.transform, agent.GetDestination());

    }

    private void UpdateMeleeComboDash3()
    {
        Debug.Log("Update melee combo dash 3");

        if (comboDashTimer > comboDashTime * 0.5f && Core.instance != null)
        {
            correctSwingPos = Core.instance.gameObject.transform.globalPosition;
        }

        agent.MoveToCalculatedPos(comboShortDashSpeed * speedMult);

        Mathf.LookAt(ref this.gameObject.transform, agent.GetDestination());

        UpdateAnimationSpd(speedMult);
    }

    private void EndMeleeComboDash3()
    {
        Debug.Log("End melee combo dash 3");
    }

    //Long dash 4
    private void StartMeleeComboDash4()
    {
        Debug.Log("Start melee combo dash 4");

        stopDashing = false;
        comboDashTimer = comboDashTime = (comboLongDashDistance / comboLongDashSpeed) * speedMult;

        Animator.Play(gameObject, "MG_Dash", speedMult);
        if (saber != null)
        {
            DeActivateSaber();
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult);

        //PLAY AUDIOS
        //Audio.PlayAudio(gameObject, "AAAAAAAAAAAA");

        //StraightPath();   //IF WE NEED TO DO SOMETHING WITH NOT STRAIGHT PATHS

        PlayParticles(PARTICLES.DASH);

        Mathf.LookAt(ref this.gameObject.transform, agent.GetDestination());

    }

    private void UpdateMeleeComboDash4()
    {
        Debug.Log("Update melee combo dash 4");

        if (comboDashTimer > comboDashTime * 0.5f && Core.instance != null)
        {
            correctSwingPos = Core.instance.gameObject.transform.globalPosition;
        }

        agent.MoveToCalculatedPos(comboLongDashSpeed * speedMult);

        UpdateAnimationSpd(speedMult);
    }

    private void EndMeleeComboDash4()
    {
        Debug.Log("End melee combo dash 4");
    }

    //Short dash 5
    private void StartMeleeComboDash5()
    {
        Debug.Log("Start melee combo dash 5");

        stopDashing = false;
        comboDashTimer = comboDashTime = (comboShortDashDistance / comboShortDashSpeed) * speedMult;

        Animator.Play(gameObject, "MG_Dash", speedMult);
        if (saber != null)
        {
            DeActivateSaber();
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult);

        //PLAY AUDIOS
        //Audio.PlayAudio(gameObject, "AAAAAAAAAAAA");

        //StraightPath();   //IF WE NEED TO DO SOMETHING WITH NOT STRAIGHT PATHS

        PlayParticles(PARTICLES.DASH);

        Vector3 direction = Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition;
        if (Mathf.Distance(Core.instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition) > comboShortDashDistance)
        {
            targetPosition = direction.normalized * comboShortDashDistance + gameObject.transform.globalPosition;
        }
        else
        {
            targetPosition = Core.instance.gameObject.transform.globalPosition;
        }



        agent.CalculatePath(gameObject.transform.globalPosition, targetPosition);
        Mathf.LookAt(ref this.gameObject.transform, agent.GetDestination());

    }

    private void UpdateMeleeComboDash5()
    {
        Debug.Log("Update melee combo dash 5");

        if (comboDashTimer > comboDashTime * 0.5f && Core.instance != null)
        {
            correctSwingPos = Core.instance.gameObject.transform.globalPosition;
        }

        agent.MoveToCalculatedPos(comboShortDashSpeed * speedMult);

        UpdateAnimationSpd(speedMult);
    }

    private void EndMeleeComboDash5()
    {
        Debug.Log("End melee combo dash 5");
    }

    //Short dash 6
    private void StartMeleeComboDash6()
    {
        Debug.Log("Start melee combo dash 6");

        stopDashing = false;
        comboDashTimer = comboDashTime = (comboShortDashDistance / comboShortDashSpeed) * speedMult;

        Animator.Play(gameObject, "MG_Dash", speedMult);
        if (saber != null)
        {
            DeActivateSaber();
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult);

        //PLAY AUDIOS
        //Audio.PlayAudio(gameObject, "AAAAAAAAAAAA");

        //StraightPath();   //IF WE NEED TO DO SOMETHING WITH NOT STRAIGHT PATHS

        PlayParticles(PARTICLES.DASH);

        Vector3 direction = Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition;
        if (Mathf.Distance(Core.instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition) > comboShortDashDistance)
        {
            targetPosition = direction.normalized * comboShortDashDistance + gameObject.transform.globalPosition;
        }
        else
        {
            targetPosition = Core.instance.gameObject.transform.globalPosition;
        }
        agent.CalculatePath(gameObject.transform.globalPosition, targetPosition);
        Mathf.LookAt(ref this.gameObject.transform, agent.GetDestination());

    }

    private void UpdateMeleeComboDash6()
    {
        Debug.Log("Update melee combo dash 6");

        if (comboDashTimer > comboDashTime * 0.5f && Core.instance != null)
        {
            correctSwingPos = Core.instance.gameObject.transform.globalPosition;
        }

        agent.MoveToCalculatedPos(comboShortDashSpeed * speedMult);

        UpdateAnimationSpd(speedMult);
    }

    private void EndMeleeComboDash6()
    {
        Debug.Log("End melee combo dash 6");
    }
    #endregion

    #region MELEE_COMBO_HIT

    //Hit 1
    private void StartMeleeComboHit1()
    {
        meleeHitTimer = meleeHit1Duration;
        meleeHitSwingTimer = meleeHit1SwingTime;

        Animator.Play(gameObject, "MG_MeleeCombo1", speedMult * meleeHit1SpdMult);
        if (saber != null)
        {
            ActivateSaber(true);
            Animator.Play(saber, "MG_MeleeCombo1", speedMult * meleeHit1SpdMult);
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult * meleeHit1SpdMult);

        CalculateSwingCorrectingAngle();

        Debug.Log("Start melee combo hit 1");
    }

    private void UpdateMeleeComboHit1()
    {
        if (meleeHitSwingTimer > 0)
        {
            gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, Quaternion.RotateAroundAxis(Vector3.up, swingCorrectionAngle), 1f - (meleeHitSwingTimer / meleeHit1SwingTime));
        }

        if (launchSwing == true)
        {
            SpawnSwing((int)meleeHit1Damage, gameObject.transform.GetForward());
            launchSwing = false;
        }

        UpdateAnimationSpd(speedMult * meleeHit1SpdMult);
    }

    private void EndMeleeComboHit1()
    {
        Debug.Log("End melee combo hit 1");
    }

    //Hit 2
    private void StartMeleeComboHit2()
    {
        meleeHitTimer = meleeHit2Duration;
        meleeHitSwingTimer = meleeHit2SwingTime;

        Animator.Play(gameObject, "MG_MeleeCombo2", speedMult * meleeHit2SpdMult);
        if (saber != null)
        {
            ActivateSaber(true);
            Animator.Play(saber, "MG_MeleeCombo2", speedMult * meleeHit2SpdMult);
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult * meleeHit2SpdMult);

        CalculateSwingCorrectingAngle();

    }

    private void UpdateMeleeComboHit2()
    {
        if (meleeHitSwingTimer > 0)
        {
            gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, Quaternion.RotateAroundAxis(Vector3.up, swingCorrectionAngle), 1f - (meleeHitSwingTimer / meleeHit2SwingTime));
        }

        if (launchSwing == true)
        {
            SpawnSwing((int)meleeHit2Damage, gameObject.transform.GetForward());
            launchSwing = false;
        }

        UpdateAnimationSpd(speedMult * meleeHit2SpdMult);
    }

    private void EndMeleeComboHit2()
    {
        Debug.Log("End melee combo hit 2");
    }

    //Hit3
    private void StartMeleeComboHit3()
    {
        meleeHitTimer = meleeHit3Duration;
        meleeHitSwingTimer = meleeHit3SwingTime;

        Animator.Play(gameObject, "MG_MeleeCombo3", speedMult * meleeHit3SpdMult);
        if (saber != null)
        {
            ActivateSaber(true);
            Animator.Play(saber, "MG_MeleeCombo3", speedMult * meleeHit3SpdMult);
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult * meleeHit3SpdMult);

        CalculateSwingCorrectingAngle();

    }

    private void UpdateMeleeComboHit3()
    {
        if (meleeHitSwingTimer > 0)
        {
            gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, Quaternion.RotateAroundAxis(Vector3.up, swingCorrectionAngle), 1f - (meleeHitSwingTimer / meleeHit3SwingTime));
        }

        if (launchSwing == true)
        {
            SpawnSwing((int)meleeHit3Damage, gameObject.transform.GetForward());
            launchSwing = false;
        }

        UpdateAnimationSpd(speedMult * meleeHit3SpdMult);
    }

    private void EndMeleeComboHit3()
    {
        Debug.Log("End melee combo hit 3");

        if (currentPhase == PHASE.PHASE1)
            StopParticles(PARTICLES.DASH);
    }

    //Hit4
    private void StartMeleeComboHit4()
    {
        meleeHitTimer = meleeHit4Duration;
        meleeHitSwingTimer = meleeHit4SwingTime;

        Animator.Play(gameObject, "MG_MeleeCombo4", speedMult * meleeHit4SpdMult);
        if (saber != null)
        {
            ActivateSaber(true);
            Animator.Play(saber, "MG_MeleeCombo4", speedMult * meleeHit4SpdMult);
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult * meleeHit4SpdMult);

        CalculateSwingCorrectingAngle();

    }

    private void UpdateMeleeComboHit4()
    {
        if (meleeHitSwingTimer > 0)
        {
            gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, Quaternion.RotateAroundAxis(Vector3.up, swingCorrectionAngle), 1f - (meleeHitSwingTimer / meleeHit4SwingTime));
        }

        if (launchSwing == true)
        {
            SpawnSwing((int)meleeHit4Damage, gameObject.transform.GetForward());
            launchSwing = false;
        }

        UpdateAnimationSpd(speedMult * meleeHit4SpdMult);
    }

    private void EndMeleeComboHit4()
    {
        Debug.Log("End melee combo hit 4");
    }

    //Hit5
    private void StartMeleeComboHit5()
    {
        meleeHitTimer = meleeHit5Duration;
        meleeHitSwingTimer = meleeHit5SwingTime;

        Animator.Play(gameObject, "MG_MeleeCombo5", speedMult * meleeHit5SpdMult);
        if (saber != null)
        {
            ActivateSaber(true);
            Animator.Play(saber, "MG_MeleeCombo5", speedMult * meleeHit5SpdMult);
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult * meleeHit5SpdMult);

        CalculateSwingCorrectingAngle();

    }

    private void UpdateMeleeComboHit5()
    {
        if (meleeHitSwingTimer > 0)
        {
            gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, Quaternion.RotateAroundAxis(Vector3.up, swingCorrectionAngle), 1f - (meleeHitSwingTimer / meleeHit5SwingTime));
        }

        if (launchSwing == true)
        {
            SpawnSwing((int)meleeHit5Damage, gameObject.transform.GetForward());
            launchSwing = false;
        }

        UpdateAnimationSpd(speedMult * meleeHit5SpdMult);
    }

    private void EndMeleeComboHit5()
    {
        Debug.Log("End melee combo hit 5");
    }

    //Hit6
    private void StartMeleeComboHit6()
    {
        meleeHitTimer = meleeHit6Duration;
        meleeHitSwingTimer = meleeHit6SwingTime;

        Animator.Play(gameObject, "MG_MeleeCombo6", speedMult * meleeHit6SpdMult);
        if (saber != null)
        {
            ActivateSaber(true);
            Animator.Play(saber, "MG_MeleeCombo6", speedMult * meleeHit6SpdMult);
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult * meleeHit6SpdMult);

        CalculateSwingCorrectingAngle();

    }

    private void UpdateMeleeComboHit6()
    {
        if (meleeHitSwingTimer > 0)
        {
            gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, Quaternion.RotateAroundAxis(Vector3.up, swingCorrectionAngle), 1f - (meleeHitSwingTimer / meleeHit6SwingTime));
        }

        if (launchSwing == true)
        {
            SpawnSwing((int)meleeHit6Damage, gameObject.transform.GetForward());
            launchSwing = false;
        }

        UpdateAnimationSpd(speedMult * meleeHit6SpdMult);
    }

    private void EndMeleeComboHit6()
    {
        Debug.Log("End melee combo hit 6");
        StopParticles(PARTICLES.DASH);
    }

    //Rotation angle in rad
    private void SpawnSwing(int damage, Vector3 direction, float rotationAngle = 0f, float speedMult = 1f, float timeMult = 1f)
    {
        GameObject swing = InternalCalls.CreatePrefab("Library/Prefabs/1157666758.prefab", gameObject.transform.globalPosition - gameObject.transform.GetForward().normalized, gameObject.transform.globalRotation, new Vector3(1f, 0.07f, 1f));

        if (swing != null)
        {
            if (rotationAngle != 0f)
                swing.transform.localRotation = Quaternion.Slerp(swing.transform.localRotation, Quaternion.RotateAroundAxis(Vector3.up, rotationAngle), 1f);

            MoffSwing swingScript = swing.GetComponent<MoffSwing>();

            if (swingScript != null)
            {
                swingScript.SetDamage((int)(damage * damageMult));
                swingScript.SetDirection(direction.normalized);
                swingScript.SetMultipliers(speedMult, timeMult);
            }
        }
    }

    private void CalculateSwingCorrectingAngle()
    {
        Vector3 direction = correctSwingPos - gameObject.transform.globalPosition;
        direction = direction.normalized;
        swingCorrectionAngle = (float)Math.Atan2(direction.x, direction.z);

        if (Math.Abs(swingCorrectionAngle * Mathf.Rad2Deg) < 1.0f)
        {
            swingCorrectionAngle = 0f;
        }
    }

    #endregion

    #endregion

    #region BURST
    //Burst charge

    private void StartBurstCharge()
    {
        preBurstChargeTimer = comboChargeDuration;

        Animator.Play(gameObject, "MG_Swing", speedMult);
        if (saber != null)
        {
            ActivateSaber();
            Animator.Play(saber, "MG_Swing", speedMult);

            if (saberMaterial != null)
            {
                saberMaterial.SetFloatUniform("shineColorValue", 0);
                saberMaterial.SetVectorUniform("shineColor", new Vector3(0.5f, 0.0f, 1.0f));
            }
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult);
    }

    private void UpdateBurstCharge()
    {
        if (saberMaterial != null)
        {
            float shineValue = preBurstChargeTimer / comboChargeDuration;

            saberMaterial.SetFloatUniform("shineColorValue", shineValue);
        }

        UpdateAnimationSpd(speedMult);
    }

    private void EndBurstCharge()
    {
        if (saberMaterial != null)
            saberMaterial.SetFloatUniform("shineColorValue", 0);
    }


    //Burst dash
    private void StartBurstDash()
    {
        Debug.Log("Start burst dash");

        stopDashing = false;
        preBurstDashTimer = (preBurstDashDistance / preBurstDashSpeed) * speedMult;

        Animator.Play(gameObject, "MG_Dash", speedMult);
        if (saber != null)
        {
            DeActivateSaber();
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult);

        float random = (float)decisionGenerator.NextDouble();
        Vector3 pointToGo = Vector3.zero;
        float rotationAngle = 0f;

        if (random <= 0.5f)
        {
            rotationAngle = preBurstAngleDash;
        }
        else
        {
            rotationAngle = -preBurstAngleDash;
        }

        pointToGo = this.gameObject.transform.globalPosition + Vector3.RotateAroundQuaternion(Quaternion.RotateAroundAxis(Vector3.up, rotationAngle * Mathf.Deg2RRad), this.gameObject.transform.GetForward()).normalized * preBurstDashDistance;

        agent.CalculatePath(this.gameObject.transform.globalPosition, pointToGo);

        //PLAY AUDIOS
        //Audio.PlayAudio(gameObject, "AAAAAAAAAAAA");

        //StraightPath();   //IF WE NEED TO DO SOMETHING WITH NOT STRAIGHT PATHS

        PlayParticles(PARTICLES.DASH);

        Mathf.LookAt(ref this.gameObject.transform, agent.GetDestination());

    }

    private void UpdateBurstDash()
    {
        agent.MoveToCalculatedPos(comboShortDashSpeed * speedMult);

        UpdateAnimationSpd(speedMult);
    }

    private void EndBurstDash()
    {
        Debug.Log("End burst dash");
    }

    //Burst
    //P1
    private void StartBurst_P1()
    {
        Debug.Log("Start burst");
        Animator.Play(gameObject, "MG_PowerPose", speedMult);
        if (saber != null)
        {
            DeActivateSaber();
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult);

        toStartBurstTimer = timeToStartBurst;
        shotTimer = timeBetweenShots;


        Mathf.LookAt(ref gameObject.transform, Core.instance.gameObject.transform.globalPosition);

    }

    private void UpdateBurst_P1()
    {
        Debug.Log("Update burst");

        if (toStartBurstTimer > 0.0f)
        {
            toStartBurstTimer -= myDeltaTime;
        }
        else
        {
            if (shotTimer > 0.0f)
            {
                shotTimer -= myDeltaTime;

                if (shotTimer <= 0.0f)
                    Shoot();
            }
        }

        Mathf.LookAt(ref gameObject.transform, Core.instance.gameObject.transform.globalPosition);

        UpdateAnimationSpd(speedMult);
    }

    private void EndBurst_P1()
    {
        shotTimes = 0;
        StopParticles(PARTICLES.DASH);
    }

    private void Shoot()
    {
        Animator.Play(gameObject, "MG_Shoot", speedMult);
        if (saber != null)
        {
            DeActivateSaber();
        }
        if (gun != null)
        {
            ActivateGun();
            Animator.Play(gun, "MG_Shoot", speedMult);
        }
        UpdateAnimationSpd(speedMult);

        GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/1855132637.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, null);

        if (bullet != null)
        {
            BH_Bullet bulletScript = bullet.GetComponent<BH_Bullet>();

            if (bulletScript != null)
            {
                bulletScript.damage = bulletDamage;
                bulletScript.SetGameObjectToAvoid(this.gameObject);
            }
        }

        shotTimes++;


        if (shotTimes < numBurstBullets)
            shotTimer = timeBetweenShots;
    }

    //P2   PLACEHOLDER, NEED TO CHANGE FUNCTIONALITY
    private void StartBurst_P2()
    {
        Debug.Log("Start burst");
        Animator.Play(gameObject, "MG_PowerPose", speedMult);
        if (saber != null)
        {
            DeActivateSaber();
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult);

        toStartBurstTimer = timeToStartBurst;
        shotTimer = timeBetweenShots;


        Mathf.LookAt(ref gameObject.transform, Core.instance.gameObject.transform.globalPosition);
    }

    private void UpdateBurst_P2()
    {
        Debug.Log("Update burst");

        if (toStartBurstTimer > 0.0f)
        {
            toStartBurstTimer -= myDeltaTime;
        }
        else
        {
            if (shotTimer > 0.0f)
            {
                shotTimer -= myDeltaTime;

                if (shotTimer <= 0.0f)
                    Shoot();
            }
        }

        Mathf.LookAt(ref gameObject.transform, Core.instance.gameObject.transform.globalPosition);

        UpdateAnimationSpd(speedMult);
    }

    private void EndBurst_P2()
    {
        shotTimes = 0;
        StopParticles(PARTICLES.DASH);
    }

    #endregion

    #region LIGTHNING_DASH
    //Lightning dash charge
    private void StartLightningDashCharge()
    {
        Debug.Log("Start lightning dash charge");

        lightningDashChargeDurationTimer = lightningDashChargeDuration;
        lightningDashDirectionTimer = lightningDashDirectionTime;

        Animator.Play(gameObject, "MG_Swing", speedMult);
        if (saber != null)
        {
            ActivateSaber();
            Animator.Play(saber, "MG_Swing", speedMult);
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult);
    }

    private void UpdateLightningDashCharge()
    {
        Debug.Log("Update lightning dash charge");

        if (lightningDashDirectionTimer > 0.0f)
        {
            lightningDashDirectionTimer -= myDeltaTime;

            if (lightningDashDirectionTimer > 0.1f)
            {
                if (Core.instance != null)
                {
                    Vector3 direction = Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition;
                    targetPosition = direction.normalized * lightningDashLength + gameObject.transform.globalPosition;
                    agent.CalculatePath(gameObject.transform.globalPosition, targetPosition);
                    Mathf.LookAt(ref this.gameObject.transform, agent.GetDestination());
                }
            }
        }

        UpdateAnimationSpd(speedMult);
    }

    private void EndLightningDashCharge()
    {
        Debug.Log("End lightning dash charge");
    }


    //Lightning dash
    private void StartLightningDash()
    {
        Debug.Log("Start lightning dash");

        lightningDashDuration = (lightningDashLength / lightningDashSpeed) * speedMult;

        Animator.Play(gameObject, "MG_Dash", speedMult);
        if (saber != null)
        {
            DeActivateSaber();
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult);

        //PLAY AUDIOS
        //Audio.PlayAudio(gameObject, "AAAAAAAAAAAA");

        //StraightPath();   //IF WE NEED TO DO SOMETHING WITH NOT STRAIGHT PATHS

        Mathf.LookAt(ref this.gameObject.transform, agent.GetDestination());

    }

    private void UpdateLightningDash()
    {
        Debug.Log("Update lightning dash");

        agent.MoveToCalculatedPos(lightningDashSpeed * speedMult);

        UpdateAnimationSpd(speedMult);
    }

    private void EndLightningDash()
    {
        Debug.Log("End lightning dashh");
    }

    //Lightning dash tired
    private void StartLightningDashTired()
    {
        Debug.Log("Start lightning dash tired");

        Audio.StopAudio(gameObject);

        lightningDashTiredDurationTimer = lightningDashTiredDuration;

        //TIRED ANIMATION AND AUDIO + STUN PARTICLES
        Animator.Play(gameObject, "MG_Idle", speedMult);
        if (saber != null)
        {
            DeActivateSaber();
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult);
        //Audio.PlayAudio(gameObject, "AAAAAAAAAAAAA");
        //stun.Play();
    }

    private void UpdateLightningDashTired()
    {
        Debug.Log("Update lightning dash tired");

        UpdateAnimationSpd(speedMult);

        //STUN PARTICLES
        //if (stun.playing == false)
        //    stun.Play();
    }

    private void EndLightningDashTired()
    {
        Debug.Log("End lightning dash tired");
    }
    #endregion

    #region SPAWN_ENEMIES
    private void StartSpawnEnemies()
    {
        spawnEnemyTimer = spawnEnemyTime;

        CalculateSpawnersScore();

        if (currentPhase == PHASE.PHASE1)
        {
            Animator.Play(gameObject, "MG_EnemySpawnerPh1", speedMult * spawnAnimationSpdMult_P1);
            if (saber != null)
            {
                DeActivateSaber();
            }
            if (gun != null)
            {
                DeActivateGun();
            }
        }
        else if (currentPhase == PHASE.PHASE2)
        {
            Animator.Play(gameObject, "MG_EnemySpawnPh2", speedMult);
            if (saber != null)
            {
                ActivateSaber();
                Animator.Play(saber, "MG_EnemySpawnPh2", speedMult);
            }
            if (gun != null)
            {
                DeActivateGun();
            }
        }
        UpdateAnimationSpd(speedMult);

        Audio.PlayAudio(gameObject, "Play_Moff_Gideon_Spawn_Enemies");
        if (cameraComp != null)
            cameraComp.target = this.gameObject;

        Input.PlayHaptic(0.8f, 600);

        PlayParticles(PARTICLES.AURA);

        isInvencible = true;

        SpawnEnemies();
    }

    private void UpdateSpawnEnemies()
    {
        //Debug.Log("Spawning Enemies");
        if (currentPhase == PHASE.PHASE1)
        {
            UpdateAnimationSpd(speedMult * spawnAnimationSpdMult_P1);
        }
        else if (currentPhase == PHASE.PHASE2)
        {
            UpdateAnimationSpd(speedMult);
        }
    }

    private void EndSpawnEnemies()
    {
        enemySkillTimer = enemySkillTime;

        if (cameraComp != null)
            cameraComp.target = Core.instance.gameObject;

        isInvencible = false;

        StopParticles(PARTICLES.AURA);
    }

    private void SpawnEnemies()
    {
        var spawnPointEnum = spawnPoints.GetEnumerator();
        float prevDelay = 0f;

        for (int i = 0; i < enemiesToSpawn_P1; ++i)
        {
            if (i > spawnPoints.Count - 1)
            {
                if (i % spawnPoints.Count == 0)
                    spawnPointEnum = spawnPoints.GetEnumerator();
            }

            spawnPointEnum.MoveNext();
            prevDelay = SpawnEnemy(spawnPointEnum.Current.Value, prevDelay);
        }

        ableToSpawnEnemies = false;
    }

    private float SpawnEnemy(GameObject spawnPoint, float minDelay = 0f)
    {
        Debug.Log("Spawning enemy... ");

        float delay = 0f;

        if (spawnPoint == null)
        {
            Debug.Log("Spawning point was null!!! ");
            return delay;
        }

        SpawnPoint mySpawnPoint = spawnPoint.GetComponent<SpawnPoint>();

        if (mySpawnPoint != null)
        {
            Random seed = new Random();

            delay = (float)((seed.NextDouble() * maxEnemySpawnDelay) + baseEnemySpawnDelay);

            if (minDelay != 0f)
            {
                if (Math.Abs(delay - minDelay) < baseEnemySpawnDelay)
                {
                    if (minDelay > delay)
                    {
                        delay = Math.Max(delay - baseEnemySpawnDelay, baseEnemySpawnDelay);
                    }
                    else
                    {
                        delay += baseEnemySpawnDelay;
                    }

                }
            }

            mySpawnPoint.QueueSpawnEnemy(delay);
        }

        return delay;
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

    #region PRESENTATION
    private void StartPresentation()
    {
        Animator.Play(gameObject, "MG_CapeOff", speedMult);
        if (saber != null)
        {
            DeActivateSaber();
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        if (cape != null)
        {
            Animator.Resume(cape);
            Animator.Play(cape, "MG_Cape_Off", speedMult);
        }

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

        PlayParticles(PARTICLES.AURA);

        isInvencible = true;
        Input.PlayHaptic(0.9f, 2200);
    }

    private void UpdatePresentation()
    {
        currentHealthPoints = Mathf.Lerp(0, maxHealthPoints1, 1f - (presentationTimer / presentationTime));
    }


    private void EndPresentation()
    {
        //if (cameraComp != null)
        //    cameraComp.target = Core.instance.gameObject;

        if (capeMesh != null)
        {
            MeshRenderer capeMeshRend = capeMesh.GetComponent<MeshRenderer>();

            if (capeMeshRend != null)
            {
                capeMeshRend.active = false;
            }

        }

        isInvencible = false;

        StopParticles(PARTICLES.AURA);

        currentHealthPoints = limboHealth = maxHealthPoints1;
    }

    #endregion

    #region CHANGE_PHASE
    private void StartPhaseChange()
    {
        Animator.Play(gameObject, "MG_Rising", speedMult);
        if (saber != null)
        {
            ActivateSaber();
            Animator.Play(saber, "MG_Rising", speedMult);
        }
        if (gun != null)
        {
            DeActivateGun();
        }
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

        PlayParticles(PARTICLES.AURA);

        isInvencible = true;

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

        currentHealthPoints = Mathf.Lerp(0, maxHealthPoints2, 1f - (changingPhaseTimer / changingPhaseTime));
    }


    private void EndPhaseChange()
    {
        currentPhase = PHASE.PHASE2;
        enemySkillTimer = enemySkillTime;
        currentHealthPoints = limboHealth = maxHealthPoints2;

        Audio.SetState("Game_State", "Moff_Gideon_Phase_2");

        isInvencible = false;

        StopParticles(PARTICLES.AURA);

    }

    #endregion

    #region DIE_ACTION
    private void StartDie()
    {
        dieTimer = dieTime;
        Animator.Play(gameObject, "MG_Death", speedMult);
        if (saber != null)
        {
            DeActivateSaber();
        }
        if (gun != null)
        {
            DeActivateGun();
        }
        UpdateAnimationSpd(speedMult);

        Counter.SumToCounterType(Counter.CounterTypes.MOFFGIDEON);

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

            if (isInvencible == true)
            {
                Audio.PlayAudio(gameObject, "Play_Moff_Gideon_Phase_Change_Hit");
                return;
            }

            damaged = 1.0f;

            float mod = 1f;
            if (Core.instance != null && Core.instance.HasStatus(STATUS_TYPE.GEOTERMAL_MARKER))
            {
                if (HasNegativeStatus())
                {
                    mod = 1f + GetStatusData(STATUS_TYPE.GEOTERMAL_MARKER).severity / 100;
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
                    inputsList.Add(INPUT.IN_DEAD);
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

        if (damaged > 0.01f)
        {
            damaged = Mathf.Lerp(damaged, 0.0f, 0.2f);
        }
        else
        {
            damaged = 0.0f;
        }

        if (moffMesh != null)
        {
            Material moffMeshMat = moffMesh.GetComponent<Material>();

            if (moffMeshMat != null)
            {
                moffMeshMat.SetFloatUniform("damaged", damaged);
            }

        }
    }

    #endregion

    #region COLLISION EVENTS
    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("Bullet"))
        {
            if (isInvencible == true)
            {
                Audio.PlayAudio(gameObject, "Play_Moff_Gideon_Phase_Change_Hit");
                return;
            }


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

            if (isInvencible == true)
            {
                //Play Audio no hit
                Audio.PlayAudio(gameObject, "Play_Moff_Gideon_Phase_Change_Hit");
                return;
            }

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
            stopDashing = true;
        }
        else if (collidedGameObject.CompareTag("Wall"))
        {
            //TODO: Stop any kind of dash!
            stopDashing = true;
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
            Audio.PlayAudio(gameObject, "Play_Moff_Gideon_Hit_Phase_1");
            Audio.PlayAudio(gameObject, "Play_Moff_Gideon_Intimidation_Phase_1");
        }
        else if (currentPhase == PHASE.PHASE2)
        {
            Audio.PlayAudio(gameObject, "Play_Moff_Gideon_Hit_Phase_2");
            Audio.PlayAudio(gameObject, "Play_Moff_Gideon_Intimidation_Phase_2");
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

    #region ANIMATIONS
    private void UpdateAnimationSpd(float newSpd)
    {
        if (currAnimationPlaySpd != newSpd)
        {
            Animator.SetSpeed(gameObject, newSpd);

            if (cape != null)
                Animator.SetSpeed(cape, newSpd);

            if (saber != null)
                Animator.SetSpeed(saber, newSpd);

            if (gun != null)
                Animator.SetSpeed(gun, newSpd);


            currAnimationPlaySpd = newSpd;
        }
    }
    #endregion


    #region PARTICLESs
    private void PlayParticles(PARTICLES particletype, bool forceRestart = false)
    {
        switch (particletype)
        {
            case PARTICLES.NONE:
                break;
            case PARTICLES.DASH:
                if (dashParticle != null)
                {
                    if (dashParticle.playing == false || forceRestart == true)
                        dashParticle.Play();
                }
                else
                    Debug.Log("Dash Particles not found");
                break;

            case PARTICLES.AURA:
                if (auraParticle != null)
                {
                    if (auraParticle.playing == false || forceRestart == true)
                        auraParticle.Play();
                }
                else
                    Debug.Log("Aura Particles not found");

                if (auraBurstParticle != null)
                {
                    if (auraBurstParticle.playing == false || forceRestart == true)
                        auraBurstParticle.Play();
                }
                else
                    Debug.Log("Aura Burst Particles not found");
                break;

            case PARTICLES.SWORD_SWING:
                if (saberParticles != null)
                {
                    if (saberParticles.playing == false || forceRestart == true)
                        saberParticles.Play();
                }
                else
                    Debug.Log("Aura Particles not found");
                break;
        }
    }

    private void StopParticles(PARTICLES particletype)
    {
        switch (particletype)
        {
            case PARTICLES.NONE:
                break;
            case PARTICLES.DASH:
                if (dashParticle != null)
                {
                    dashParticle.Stop();
                }
                else
                    Debug.Log("Dash Particles not found");
                break;

            case PARTICLES.AURA:
                if (auraParticle != null)
                {
                    auraParticle.Stop();
                }
                else
                    Debug.Log("Aura Particles not found");

                if (auraBurstParticle != null)
                {
                    auraBurstParticle.Stop();
                }
                else
                    Debug.Log("Aura Burst Particles not found");
                break;

            case PARTICLES.SWORD_SWING:
                if (saberParticles != null)
                {
                    saberParticles.Stop();
                }
                else
                    Debug.Log("Aura Particles not found");
                break;
        }
    }

    #endregion


    #region PROPS
    private void ActivateGun()
    {
        if (gun == null || gunMesh == null)
            return;


        gunMesh.active = true;
        Animator.Pause(gun);

    }

    private void DeActivateGun()
    {
        if (gun == null || gunMesh == null)
            return;


        gunMesh.active = false;

        Animator.Resume(gun);
    }


    private void ActivateSaber(bool playSwingParticles = false)
    {
        if (saber == null || saberMesh == null)
            return;

        saberMesh.active = true;

        Animator.Resume(saber);

        if(playSwingParticles == true)
        {
            PlayParticles(PARTICLES.SWORD_SWING, true);
        }
    }

    private void DeActivateSaber()
    {
        if (saber == null || saberMesh == null)
            return;

        saberMesh.active = false;

        Animator.Pause(saber);

        StopParticles(PARTICLES.SWORD_SWING);
    }

    #endregion


    public override bool IsDying()
    {
        return currentState == STATE.DEAD;
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

    #endregion
}