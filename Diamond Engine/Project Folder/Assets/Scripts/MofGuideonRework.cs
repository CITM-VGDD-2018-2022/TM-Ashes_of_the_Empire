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
        IN_CHANGE_STATE_END,
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


    //Throw saber
    public float saberThrowDuration = 2.0f;
    private float saberThrowTimer = 0.0f;

    private float saberThrowAnimDuration = 0.0f;
    private float saberThrowAnimTimer = 0.0f;

    public void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        InitEntity(ENTITY_TYPE.MOFF);

        if (EnemyManager.EnemiesLeft() > 0)
            EnemyManager.ClearList();
        EnemyManager.AddEnemy(gameObject);

        Audio.SetState("Player_State", "Alive");
        Audio.SetState("Game_State", "Moff_Guideon_Room");

        spawner1 = InternalCalls.FindObjectWithName("DefaultSpawnPoint1");
        spawner2 = InternalCalls.FindObjectWithName("DefaultSpawnPoint2");
        spawner3 = InternalCalls.FindObjectWithName("DefaultSpawnPoint3");
        spawner4 = InternalCalls.FindObjectWithName("DefaultSpawnPoint4");
        spawner5 = InternalCalls.FindObjectWithName("DefaultSpawnPoint5");
        spawner6 = InternalCalls.FindObjectWithName("DefaultSpawnPoint6");
        CalculateSpawnersScore();

        //Get anim durations
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
    }

    //Timers go here
    private void ProcessInternalInput()
    {
        if (chaseTimer > 0)
        {
            chaseTimer -= myDeltaTime;

            if (chaseTimer <= 0)
                inputsList.Add(INPUT.IN_ACTION_SELECT);
        }


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

    }

    private void ProcessExternalInput()
    {

    }

    private void ProcessState()
    {

    }

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

                    //TODO: Add take damage
                    //TakeDamage(damageToTake);
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