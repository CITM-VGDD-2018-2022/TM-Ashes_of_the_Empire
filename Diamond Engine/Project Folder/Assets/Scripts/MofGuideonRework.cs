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
        RANDOM_DASH,
        ACTION_SELECT,

        // Melee Combo
        MELEE_COMBO_1_DASH,
        MELEE_COMBO_1,
        MELEE_COMBO_2_DASH,
        MELEE_COMBO_2,
        MELEE_COMBO_3_DASH,
        MELEE_COMBO_3,
        MELEE_COMBO_4_DASH,
        MELEE_COMBO_4,
        MELEE_COMBO_5_DASH,
        MELEE_COMBO_5,
        MELEE_COMBO_6_DASH,
        MELEE_COMBO_6,

        //Other Actions
        SPAWN_ENEMIES,
        PRE_BURST_DASH,
        BURST_1,
        BURST_2,
        PREPARE_THROW_SABER,
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

        IN_RANDOM_DASH,
        IN_CHASE,
        IN_SEARCH,

        // Melee combo
        IN_MELEE_COMBO_1_CHARGE,
        IN_MELEE_COMBO_1_DASH,
        IN_MELEE_COMBO_1,
        IN_MELEE_COMBO_2_CHARGE,
        IN_MELEE_COMBO_2_DASH,
        IN_MELEE_COMBO_2,
        IN_MELEE_COMBO_3_CHARGE,
        IN_MELEE_COMBO_3_DASH,
        IN_MELEE_COMBO_3,
        IN_MELEE_COMBO_4_CHARGE,
        IN_MELEE_COMBO_4_DASH,
        IN_MELEE_COMBO_4,
        IN_MELEE_COMBO_5_CHARGE,
        IN_MELEE_COMBO_5_DASH,
        IN_MELEE_COMBO_5,
        IN_MELEE_COMBO_6_CHARGE,
        IN_MELEE_COMBO_6_DASH,
        IN_MELEE_COMBO_6,
        IN_MELEE_COMBO_END,

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

    //Decision making
    public float probRandomDash_P1 = 20.0f;
    public float probMeleeCombo_P1 = 20.0f;
    public float probBurst_P1 = 60.0f;

    public float probRandomDash_P2 = 20.0f;
    public float probMeleeCombo_P2 = 20.0f;
    public float probBurst_P2 = 40.0f;
    public float probLightDash = 20.0f; //Onlly phase 2

    public float minProjectileDistance = 17.0f;
    public float maxMeleeDistance = 5.0f;


    //Chase
    public float chaseDuration = 2.0f;
    private float chaseTimer = 0.0f;

    public float chaseSpeed = 3.0f;

    //Random dash
    public float dashSpeed = 10.0f;
    public float dashDistance = 7.0f;

    private float dashTimer = 0.0f;


    //Melee combo
    public float comboLongDashDistance = 10.0f;
    public float comboLongDashSpeed = 5.0f;

    public float comboShortDashDistance = 10.0f;
    public float comboShortDashSpeed = 5.0f;

    private float comboDashTimer = 0.0f;

        //TODO add hit vars

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

    //Throw saber
    public float saberThrowDuration = 2.0f;

    public void Update()
	{

	}

}