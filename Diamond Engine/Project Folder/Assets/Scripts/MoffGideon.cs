using DiamondEngine;
using System;
using System.Collections.Generic;

public class MoffGideon : Entity
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

        PRESENTATION,

        // Neutral
        IDLE,
        CHASE,
        WANDER,
        ACTION_SELECT,

        // Melee Combo
        MELEE_COMBO_1_CHARGE,
        MELEE_COMBO_1_DASH,
        MELEE_COMBO_1,
        MELEE_COMBO_2_CHARGE,
        MELEE_COMBO_2_DASH,
        MELEE_COMBO_2,
        MELEE_COMBO_3_CHARGE,
        MELEE_COMBO_3_DASH,
        MELEE_COMBO_3,
        MELEE_COMBO_4_CHARGE,
        MELEE_COMBO_4_DASH,
        MELEE_COMBO_4,
        MELEE_COMBO_5_CHARGE,
        MELEE_COMBO_5_DASH,
        MELEE_COMBO_5,
        MELEE_COMBO_6_CHARGE,
        MELEE_COMBO_6_DASH,
        MELEE_COMBO_6,

        //Other Actions
        SPAWN_ENEMIES,
        PRE_PROJECTILE_DASH_CHARGE,
        PRE_PROJECTILE_DASH,
        PROJECTILE,
        LOAD_THROW,
        THROW_SABER,
        RETRIEVE_SABER,
        POST_SABER_TIRED,

        // Prob. to delete
        DASH_BACKWARDS,
        DASH_FORWARD,
        DASH_FORWARD_LONG,


        // End
        CHANGE_PHASE,
        DEAD
    }

    enum INPUT : int
    {
        NONE = -1,
        IN_PRESENTATION,
        IN_PRESENTATION_END,

        IN_WANDER,
        IN_CHASE,
        IN_NEUTRAL,
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

        IN_PRE_PROJECTILE_DASH_CHARGE,
        IN_PRE_PROJECTILE_DASH_CHARGE_END,
        IN_PRE_PROJECTILE_DASH,
        IN_PRE_PROJECTILE_DASH_END,
        IN_PROJECTILE,
        IN_PROJECTILE_END,

        // Saber Throw
        IN_THROW_SABER,
        IN_THROW_SABER_END,
        IN_CHARGE_THROW,
        IN_CHARGE_THROW_END,
        IN_RETRIEVE_SABER,
        IN_RETRIEVE_SABER_END,

        // Prob to delete
        IN_DASH_FORWARD,
        IN_DASH_FORWARD_END,
        IN_DASH_BACKWARDS,
        IN_DASH_BACKWARDS_END,

        // Finishers
        IN_CHANGE_PHASE,
        IN_CHANGE_STATE_END,
        IN_DEAD
    }

    public class AttackMoff
    {
        public AttackMoff(string animation, GameObject go)
        {
            this.animation = animation;
            this.duration = Animator.GetAnimationDuration(go, animation) - 0.016f;
        }
        public string animation;
        public float duration;
    }

    private NavMeshAgent agent = null;
    private GameObject saber = null;
    public GameObject camera = null;
    private CameraController cam_comp = null;

    //State
    private STATE currentState = STATE.PRESENTATION;
    private List<INPUT> inputsList = new List<INPUT>();
    private PHASE currentPhase = PHASE.PHASE1;

    Random randomNum = new Random();

    public GameObject hitParticles = null;

    public float slerpSpeed = 5.0f;

    private float damageMult = 1.0f;
    public float damageRecieveMult = 1f;

    //Stats
    public float healthPoints = 8500.0f;
    public float maxHealthPoints_fase1 = 4500.0f;
    public float maxHealthPoints_fase2 = 4000.0f;

    //Public Variables
    public float followSpeed = 3f;
    public float touchDamage = 10f;
    public float distance2Melee = 10f;
    public float probWanderP2 = 40f;
    public float probWander = 60f;
    public float radiusWander = 5f;
    public float distanceProjectile = 17f;
    public float dashSpeed = 10f;
    public float dashBackWardDistance = 3f;
    public float dashForwardDistance = 10f;
    public float closerDistance = 1f;
    public float farDistance = 50f;
    public float velocityRotationShooting = 10f;
    public float zoomTimeEasing = 0.5f;
    public float baseZoom = 45f;
    public float zoomInValue = 40f;
    public float swordRange = 14f;
    public GameObject spawner1 = null;
    public GameObject spawner2 = null;
    public GameObject spawner3 = null;
    public GameObject spawner4 = null;
    public GameObject sword = null;
    public GameObject shootPoint = null;
    public int numSequencesPh2 = 3;
    public int numAtacksPh1 = 2;
    public int numAtacksPh2 = 1;

    private List<GameObject> deathtroopers = null;

    //Private Variables
    private float currAnimationPlaySpd = 1f;
    private bool soloDash = false;
    private bool invencible = false;
    private bool wander = false;
    private bool ready2Spawn = false;
    private Vector3 beginDash = null;
    private Vector3 targetDash = null;
    private bool deathTrooperSpawned = false;
    private GameObject visualFeedback = null;
    private bool justDashing = false;
    private int maxProjectiles = 7;
    private int projectiles = 0;
    private bool aiming = false;
    private List<AttackMoff> atacks = new List<AttackMoff>();
    private int nAtacks = 0;
    private int nSequences = 3;
    private bool retrieveAnim = true;
    private bool showingSaber = false;


    //Timers
    private float dieTimer = 0f;
    public float dieTime = 0.1f;
    private float neutralTimer = 0f;
    public float neutralTime = 5f;
    private float enemiesTimer = 0f;
    public float enemiesTime = 1f;
    private float comboTime = 0f;
    private float comboTimer = 0f;
    private float presentationTime = 0f;
    private float presentationTimer = 0f;
    private float changingStateTime = 0f;
    private float changingStateTimer = 0f;
    private float chargeThrowTime = 0f;
    private float chargeThrowTimer = 0f;
    public float cadencyTime = 0.2f;
    public float cadencyTimer = 0f;
    private float privateTimer = 0f;

    //Boss bar updating
    public GameObject boss_bar = null;
    public GameObject moff_mesh = null;
    private float damaged = 0.0f;
    private float limbo_health = 0.0f;
    Material bossBarMat = null;
    
    public void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        targetDash = new Vector3(0, 0, 0);
        InitEntity(ENTITY_TYPE.MOFF);

        damageMult = 1f;
        damageRecieveMult = 1f;

        if (EnemyManager.EnemiesLeft() > 0)
            EnemyManager.ClearList();

        EnemyManager.AddEnemy(gameObject);

        if (agent == null)
            Debug.Log("Null agent, add a NavMeshAgent Component");

        //comboTime = Animator.GetAnimationDuration(gameObject, "MG_Slash") - 0.016f;
        presentationTime = Animator.GetAnimationDuration(gameObject, "MG_PowerPose") - 0.016f;
        changingStateTime = Animator.GetAnimationDuration(gameObject, "MG_Rising") - 0.016f;
        dieTime = Animator.GetAnimationDuration(gameObject, "MG_Death") - 0.016f;
        chargeThrowTime = Animator.GetAnimationDuration(gameObject, "MG_SaberThrow") - 0.016f;

        atacks.Add(new AttackMoff("MG_MeleeCombo1", gameObject));
        atacks.Add(new AttackMoff("MG_MeleeCombo2", gameObject));
        atacks.Add(new AttackMoff("MG_MeleeCombo3", gameObject));
        atacks.Add(new AttackMoff("MG_MeleeCombo4", gameObject));
        atacks.Add(new AttackMoff("MG_MeleeCombo5", gameObject));
        atacks.Add(new AttackMoff("MG_MeleeCombo6", gameObject));

        enemiesTimer = enemiesTime;

        deathtroopers = new List<GameObject>();

        if(camera!=null)
            cam_comp = camera.GetComponent<CameraController>();

        bossBarMat = boss_bar.GetComponent<Material>();

        Audio.SetState("Player_State", "Alive");
        Audio.SetState("Game_State", "Moff_Guideon_Room");

        StartPresentation();
    }

    public void Update()
    {
        myDeltaTime = Time.deltaTime * speedMult;
        sword.transform.localPosition = new Vector3(0, 0, 0);

        UpdateStatuses();

        ProcessInternalInput();
        ProcessExternalInput();
        ProcessState();

        UpdateState();
    }

    //Timers go here
    private void ProcessInternalInput()
    {
        if (neutralTimer > 0)
        {
            neutralTimer -= myDeltaTime;

            if (neutralTimer <= 0)
                inputsList.Add(INPUT.IN_SEARCH);

        }

        if (enemiesTimer > 0)
        {
            enemiesTimer -= myDeltaTime;

            if (enemiesTimer <= 0)
                if(deathtroopers.Count>0)
                    ready2Spawn = true;

        }

        if (comboTimer > 0)
        {
            comboTimer -= myDeltaTime;

            if (comboTimer <= 0)
            {

                if (nAtacks > 0)
                {
                    int rand = randomNum.Next(1, 6);
                    comboTimer = atacks[rand].duration;
                    Animator.Play(gameObject, atacks[rand].animation);
                    Input.PlayHaptic(0.5f, 500);
                    UpdateAnimationSpd(speedMult);
                    nAtacks--;
                }
                else
                {
                    if (currentPhase == PHASE.PHASE1) inputsList.Add(INPUT.IN_MELEE_COMBO_END);
                    else
                    {
                        if (nSequences > 0)
                        {
                            nSequences--;
                            inputsList.Add(INPUT.IN_DASH_FORWARD);
                        }
                        else
                            inputsList.Add(INPUT.IN_MELEE_COMBO_END);
                    }
                }
            }

        }

        if (presentationTimer > 0)
        {
            presentationTimer -= myDeltaTime;
            healthPoints = (1 - (presentationTimer / presentationTime)) * maxHealthPoints_fase1;
            Debug.Log(presentationTimer.ToString());
            if (presentationTimer <= 0)
            {
                inputsList.Add(INPUT.IN_PRESENTATION_END);
                healthPoints = limbo_health = maxHealthPoints_fase1;
            }

        }

        if (changingStateTimer > 0)
        {
            changingStateTimer -= myDeltaTime;
            healthPoints = (1 - (changingStateTimer / changingStateTime)) * maxHealthPoints_fase2;
            if (changingStateTimer <= 0)
            {
                inputsList.Add(INPUT.IN_CHANGE_STATE_END);
                healthPoints = limbo_health = maxHealthPoints_fase2;
            }

        }

        if (chargeThrowTimer > 0)
        {
            chargeThrowTimer -= myDeltaTime;

            if (chargeThrowTimer <= 2.1f)
            {
                Input.PlayHaptic(0.6f, 250);
                inputsList.Add(INPUT.IN_CHARGE_THROW_END);
            }

        }
    }

    private void ProcessExternalInput()
    {
        if (currentState == STATE.SPAWN_ENEMIES && deathtroopers.Count == 0 && deathTrooperSpawned)
            inputsList.Add(INPUT.IN_NEUTRAL);

        if (currentState == STATE.CHASE && Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance && wander)
            agent.CalculateRandomPath(gameObject.transform.globalPosition, radiusWander);

        if (currentState == STATE.CHASE && Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) <= 1.0f && !wander)
            wander = true;

        if (currentState == STATE.DASH_FORWARD_LONG && Mathf.Distance(gameObject.transform.globalPosition, targetDash) <= 1f && !justDashing)
            inputsList.Add(INPUT.IN_DASH_FORWARD_END);

        if (currentState == STATE.DASH_BACKWARDS && Mathf.Distance(beginDash, gameObject.transform.globalPosition) >= dashBackWardDistance)
        {
            inputsList.Add(INPUT.IN_DASH_BACKWARDS_END);
        }

        if (currentState == STATE.DASH_FORWARD_LONG && Mathf.Distance(gameObject.transform.globalPosition, beginDash) >= dashForwardDistance && justDashing)
        {
            inputsList.Add(INPUT.IN_NEUTRAL);
            justDashing = false;
            soloDash = true;
        }

        if (currentState == STATE.PROJECTILE && projectiles == maxProjectiles)
        {
            projectiles = 0;
            inputsList.Add(INPUT.IN_NEUTRAL);
        }

        if(currentState==STATE.RETRIEVE_SABER && Mathf.Distance(gameObject.transform.globalPosition, saber.transform.globalPosition) <= 1f)
        {
            inputsList.Add(INPUT.IN_RETRIEVE_SABER_END);

        }
        if(currentState==STATE.RETRIEVE_SABER && Mathf.Distance(gameObject.transform.globalPosition, saber.transform.globalPosition) <= 3.5f && !retrieveAnim)
        {
            Animator.Play(gameObject, "MG_Recovery");
            UpdateAnimationSpd(speedMult);
            retrieveAnim = true;
        }

    }

    private void ProcessState()
    {
        while (inputsList.Count > 0)
        {
            INPUT input = inputsList[0];

            if (currentPhase == PHASE.PHASE1)
            {
                switch (currentState)
                {
                    case STATE.NONE:
                        Debug.Log("MOFF ERROR STATE");
                        break;
                    case STATE.PRESENTATION:
                        switch (input)
                        {
                            case INPUT.IN_PRESENTATION_END:
                                currentState = STATE.CHASE;
                                EndPresentation();
                                StartNeutral();
                                break;
                        }
                        break;
                    case STATE.IDLE:
                        switch (input)
                        {
                            case INPUT.IN_WANDER:
                                currentState = STATE.WANDER;
                                //EndNeutral();
                                break;

                            case INPUT.IN_CHASE:
                                currentState = STATE.CHASE;
                                //EndNeutral();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                EndNeutral();
                                StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.CHASE:
                        switch (input)
                        {
                            case INPUT.IN_WANDER:
                                currentState = STATE.WANDER;
                                //EndNeutral();
                                break;

                            case INPUT.IN_SEARCH:
                                currentState = STATE.ACTION_SELECT;
                                //EndNeutral();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.WANDER:
                        switch (input)
                        {
                            case INPUT.IN_CHASE:
                                currentState = STATE.CHASE;
                                //EndNeutral();
                                break;

                            case INPUT.IN_SEARCH:
                                currentState = STATE.ACTION_SELECT;
                                //EndNeutral();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.ACTION_SELECT:
                        switch (input)
                        {
                            case INPUT.IN_SPAWN_ENEMIES:
                                currentState = STATE.SPAWN_ENEMIES;
                                EndNeutral();
                                //StartSpawnEnemies();
                                break;

                            case INPUT.IN_PRE_PROJECTILE_DASH:
                                currentState = STATE.PRE_PROJECTILE_DASH;
                                EndNeutral();
                                //StartProjectile();
                                break;

                            case INPUT.IN_MELEE_COMBO_1_CHARGE:
                                currentState = STATE.MELEE_COMBO_1_CHARGE;
                                EndNeutral();
                                //StartDashForward();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.SPAWN_ENEMIES:
                        switch (input)
                        {
                            case INPUT.IN_SPAWN_ENEMIES_END:
                                currentState = STATE.ACTION_SELECT;
                                //EndSpawnEnemies();
                                break;

                            case INPUT.IN_NEUTRAL:
                                currentState = STATE.IDLE;
                                //EndSpawnEnemies();
                                //StartNeutral();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_1_CHARGE:
                        switch (input)
                        {
                            case INPUT.IN_MELEE_COMBO_1_DASH:
                                currentState = STATE.MELEE_COMBO_1_DASH;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_1_DASH:
                        switch (input)
                        {
                            case INPUT.IN_MELEE_COMBO_1:
                                currentState = STATE.MELEE_COMBO_1;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_1:
                        switch (input)
                        {
                            case INPUT.IN_MELEE_COMBO_2_CHARGE:
                                currentState = STATE.MELEE_COMBO_2_CHARGE;
                                break;

                            case INPUT.IN_MELEE_COMBO_END:
                                currentState = STATE.IDLE;
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_2_CHARGE:
                        switch (input)
                        {
                            case INPUT.IN_MELEE_COMBO_2_DASH:
                                currentState = STATE.MELEE_COMBO_2_DASH;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_2_DASH:
                        switch (input)
                        {
                            case INPUT.IN_MELEE_COMBO_2:
                                currentState = STATE.MELEE_COMBO_2;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_2:
                        switch (input)
                        {

                            case INPUT.IN_MELEE_COMBO_3_CHARGE:
                                currentState = STATE.MELEE_COMBO_3_CHARGE;
                                break;

                            case INPUT.IN_MELEE_COMBO_END:
                                currentState = STATE.IDLE;
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_3_CHARGE:
                        switch (input)
                        {
                            case INPUT.IN_MELEE_COMBO_3_DASH:
                                currentState = STATE.MELEE_COMBO_3_DASH;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_3_DASH:
                        switch (input)
                        {
                            case INPUT.IN_MELEE_COMBO_3:
                                currentState = STATE.MELEE_COMBO_3;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_3:
                        switch (input)
                        {

                            case INPUT.IN_MELEE_COMBO_4_CHARGE:
                                currentState = STATE.MELEE_COMBO_4_CHARGE;
                                break;

                            case INPUT.IN_MELEE_COMBO_END:
                                currentState = STATE.IDLE;
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_4_CHARGE:
                        switch (input)
                        {

                            case INPUT.IN_MELEE_COMBO_4_DASH:
                                currentState = STATE.MELEE_COMBO_4_DASH;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_4_DASH:
                        switch (input)
                        {
                            case INPUT.IN_MELEE_COMBO_4:
                                currentState = STATE.MELEE_COMBO_4;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_4:
                        switch (input)
                        {
                            case INPUT.IN_MELEE_COMBO_5_CHARGE:
                                currentState = STATE.MELEE_COMBO_5_CHARGE;
                                break;

                            case INPUT.IN_MELEE_COMBO_END:
                                currentState = STATE.IDLE;
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_5_CHARGE:
                        switch (input)
                        {
                            case INPUT.IN_MELEE_COMBO_5_DASH:
                                currentState = STATE.MELEE_COMBO_5_DASH;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_5_DASH:
                        switch (input)
                        {
                            case INPUT.IN_MELEE_COMBO_5:
                                currentState = STATE.MELEE_COMBO_5;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_5:
                        switch (input)
                        {

                            case INPUT.IN_MELEE_COMBO_6_CHARGE:
                                currentState = STATE.MELEE_COMBO_6_CHARGE;
                                break;

                            case INPUT.IN_MELEE_COMBO_END:
                                currentState = STATE.IDLE;
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_6_CHARGE:
                        switch (input)
                        {
                            case INPUT.IN_MELEE_COMBO_6_DASH:
                                currentState = STATE.MELEE_COMBO_6_DASH;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_6_DASH:
                        switch (input)
                        {
                            case INPUT.IN_MELEE_COMBO_6:
                                currentState = STATE.MELEE_COMBO_6;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.MELEE_COMBO_6:
                        switch (input)
                        {

                            case INPUT.IN_MELEE_COMBO_END:
                                currentState = STATE.IDLE;
                                break;


                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.DASH_BACKWARDS: //TODO: Prob. delete ?
                        switch (input)
                        {
                            case INPUT.IN_DASH_BACKWARDS_END:
                                currentState = STATE.IDLE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.PRE_PROJECTILE_DASH_CHARGE:
                        switch (input)
                        {

                            case INPUT.IN_PRE_PROJECTILE_DASH_CHARGE_END:
                                currentState = STATE.PRE_PROJECTILE_DASH;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.PRE_PROJECTILE_DASH:
                        switch (input)
                        {
                            case INPUT.IN_PRE_PROJECTILE_DASH_END:
                                currentState = STATE.PROJECTILE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.PROJECTILE:
                        switch (input)
                        {

                            case INPUT.IN_PROJECTILE_END:
                                currentState = STATE.IDLE;
                                //EndNeutral();
                                //StartPhaseChange();
                                break;

                            case INPUT.IN_CHANGE_PHASE:
                                currentState = STATE.CHANGE_PHASE;
                               //EndNeutral();
                                //StartPhaseChange();
                                break;
                        }
                        break;
                    case STATE.CHANGE_PHASE:
                        switch (input)
                        {
                            case INPUT.IN_CHANGE_STATE_END:
                                currentState = STATE.IDLE;
                                EndFirstStage();
                                //StartNeutral();
                                break;
                        }
                        break;
                    case STATE.DEAD:
                        {
                            Debug.Log("WARNING: IN_DEAD STATuS IN THE FIRST PHASE!");
                        }
                        break;
                    default:
                        break;
                }
            }
            //else if (currentPhase == PHASE.PHASE2)
            //{
            //    switch (currentState)
            //    {
            //        case STATE.NONE:
            //            Debug.Log("CORE ERROR STATE");
            //            break;

            //        case STATE.CHASE:
            //            switch (input)
            //            {
            //                case INPUT.IN_SEARCH:
            //                    currentState = STATE.ACTION_SELECT;
            //                    EndNeutral();
            //                    break;

            //                case INPUT.IN_DEAD:
            //                    currentState = STATE.DEAD;
            //                    EndNeutral();
            //                    StartDie();
            //                    break;
            //            }
            //            break;


            //        case STATE.ACTION_SELECT:
            //            switch (input)
            //            {
            //                case INPUT.IN_SPAWN_ENEMIES:
            //                    currentState = STATE.SPAWN_ENEMIES;
            //                    EndNeutral();
            //                    StartSpawnEnemies();
            //                    break;

            //                case INPUT.IN_PRE_PROJECTILE_DASH:
            //                    currentState = STATE.PROJECTILE;
            //                    EndNeutral();
            //                    StartProjectile();
            //                    break;

            //                case INPUT.IN_DASH_FORWARD:
            //                    currentState = STATE.DASH_FORWARD_LONG;
            //                    EndNeutral();
            //                    StartDashForward();
            //                    break;

            //                case INPUT.IN_CHARGE_THROW:
            //                    currentState = STATE.LOAD_THROW;
            //                    EndNeutral();
            //                    StartChargeThrow();
            //                    break;
            //            }
            //            break;

            //        case STATE.SPAWN_ENEMIES:
            //            switch (input)
            //            {
            //                case INPUT.IN_SPAWN_ENEMIES_END:
            //                    currentState = STATE.ACTION_SELECT;
            //                    EndSpawnEnemies();
            //                    break;

            //                case INPUT.IN_NEUTRAL:
            //                    currentState = STATE.CHASE;
            //                    EndSpawnEnemies();
            //                    StartNeutral();
            //                    break;

            //                case INPUT.IN_DEAD:
            //                    currentState = STATE.DEAD;
            //                    EndSpawnEnemies();
            //                    StartDie();
            //                    break;
            //            }
            //            break;

            //        case STATE.LOAD_THROW:
            //            switch (input)
            //            {
            //                case INPUT.IN_CHARGE_THROW_END:
            //                    currentState = STATE.THROW_SABER;
            //                    EndChargeThrow();
            //                    StartThrowSaber();
            //                    break;

            //                case INPUT.IN_DEAD:
            //                    currentState = STATE.DEAD;
            //                    EndProjectile();
            //                    StartDie();
            //                    break;
            //            }
            //            break;

            //        case STATE.THROW_SABER:
            //            switch (input)
            //            {
            //                case INPUT.IN_THROW_SABER_END:
            //                    currentState = STATE.RETRIEVE_SABER;
            //                    EndThrowSaber();
            //                    StartRetrieveSaber();
            //                    break;

            //                case INPUT.IN_DEAD:
            //                    currentState = STATE.DEAD;
            //                    EndThrowSaber();
            //                    StartDie();
            //                    break;
            //            }
            //            break;

            //        case STATE.RETRIEVE_SABER:
            //            switch (input)
            //            {
            //                case INPUT.IN_RETRIEVE_SABER_END:
            //                    currentState = STATE.CHASE;
            //                    EndRetrieveSaber();
            //                    StartNeutral();
            //                    break;

            //                case INPUT.IN_DEAD:
            //                    currentState = STATE.DEAD;
            //                    EndRetrieveSaber();
            //                    StartDie();
            //                    break;
            //            }
            //            break;

            //        case STATE.DASH_FORWARD_LONG:
            //            switch (input)
            //            {
            //                case INPUT.IN_DASH_FORWARD_END:
            //                    currentState = STATE.MELEE_COMBO_1;
            //                    EndDashForward();
            //                    StartMeleeCombo();
            //                    break;

            //                case INPUT.IN_NEUTRAL:
            //                    currentState = STATE.CHASE;
            //                    EndDashForward();
            //                    StartNeutral();
            //                    break;

            //                case INPUT.IN_DEAD:
            //                    currentState = STATE.DEAD;
            //                    EndProjectile();
            //                    StartDie();
            //                    break;
            //            }
            //            break;

            //        case STATE.MELEE_COMBO_1:
            //            switch (input)
            //            {
            //                case INPUT.IN_MELEE_COMBO_END:
            //                    currentState = STATE.DASH_BACKWARDS;
            //                    EndMeleeCombo();
            //                    StartDashBackward();
            //                    break;

            //                case INPUT.IN_DASH_FORWARD:
            //                    currentState = STATE.DASH_FORWARD_LONG;
            //                    EndMeleeCombo();
            //                    StartDashForward();
            //                    break;

            //                case INPUT.IN_DEAD:
            //                    currentState = STATE.DEAD;
            //                    EndProjectile();
            //                    StartDie();
            //                    break;
            //            }
            //            break;

            //        case STATE.DASH_BACKWARDS:
            //            switch (input)
            //            {
            //                case INPUT.IN_DASH_BACKWARDS_END:
            //                    currentState = STATE.CHASE;
            //                    EndDashBackward();
            //                    StartNeutral();
            //                    break;

            //                case INPUT.IN_DEAD:
            //                    currentState = STATE.DEAD;
            //                    EndProjectile();
            //                    StartDie();
            //                    break;
            //            }
            //            break;

            //        default:
            //            Debug.Log("NEED TO ADD STATE TO MOFF GIDEON");
            //            break;
            //    }
            //}
            inputsList.RemoveAt(0);
        }
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case STATE.NONE:
                Debug.Log("GIDEON ERROR STATE");
                break;

            case STATE.CHASE:
                UpdateNeutral();
                break;

            case STATE.ACTION_SELECT:
                SelectAction();
                break;

            case STATE.SPAWN_ENEMIES:
                UpdateSpawnEnemies();
                break;

            case STATE.MELEE_COMBO_1:
                UpdateMeleeCombo();
                break;

            case STATE.DASH_BACKWARDS:
                UpdateDashBackward();
                break;

            //case STATE.DASH_FORWARD_LONG:
            //    UpdateDashForward();
            //    break;

            case STATE.PROJECTILE:
                UpdateProjectile();
                break;

            case STATE.DEAD:
                UpdateDie();
                break;

            case STATE.LOAD_THROW:
                UpdateChargeThrow();
                break;

            case STATE.THROW_SABER:
                UpdateThrowSaber();
                break;

            case STATE.RETRIEVE_SABER:
                UpdateRetrieveSaber();
                break;

            case STATE.PRESENTATION:
                UpdatePresentation();
                break;

            case STATE.CHANGE_PHASE:
                UpdateChangeState();
                break;
        }
        limbo_health = Mathf.Lerp(limbo_health, healthPoints, 0.01f);
        if (boss_bar != null)
        {

            if (bossBarMat != null)
            {
                if (currentPhase == PHASE.PHASE1)
                {
                    bossBarMat.SetFloatUniform("length_used", healthPoints / maxHealthPoints_fase1);
                    bossBarMat.SetFloatUniform("limbo", limbo_health / maxHealthPoints_fase1);
                }
                else if (currentPhase == PHASE.PHASE2)
                {
                    bossBarMat.SetFloatUniform("length_used", healthPoints / maxHealthPoints_fase2);
                    bossBarMat.SetFloatUniform("limbo", limbo_health / maxHealthPoints_fase2);
                }
            }
            else
                Debug.Log("Boss Bar component was null!!");

        }
        if (damaged > 0.01f)
        {
            damaged = Mathf.Lerp(damaged, 0.0f, 0.1f);
        }
        else
        {
            damaged = 0.0f;
        }
        if (moff_mesh != null)
        {
            Material moffMeshMat = moff_mesh.GetComponent<Material>();

            if (moffMeshMat != null)
            {
                moffMeshMat.SetFloatUniform("damaged", damaged);
            }
            else
                Debug.Log("Moff Mesh Material was null!!");
        }
    }

    private void SelectAction()
    {
        if (currentPhase == PHASE.PHASE1)
        {
            if (ready2Spawn)
                inputsList.Add(INPUT.IN_SPAWN_ENEMIES);

            else if (Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) <= distanceProjectile)
                inputsList.Add(INPUT.IN_DASH_FORWARD);

            else
                inputsList.Add(INPUT.IN_PRE_PROJECTILE_DASH);
        }
        else
        {
            if (ready2Spawn)
            {
                inputsList.Add(INPUT.IN_SPAWN_ENEMIES);
                return;
            }

            if (Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) <= closerDistance)
                inputsList.Add(INPUT.IN_DASH_FORWARD);

            else if (Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) <= farDistance)
                inputsList.Add(INPUT.IN_CHARGE_THROW);

            else
            {
                inputsList.Add(INPUT.IN_DASH_FORWARD);
                justDashing = true;

            }

        }
        Debug.Log("Selecting Action");
    }

    #region PRESENTATION

    private void StartPresentation()
    {
        Animator.Play(gameObject, "MG_PowerPose", speedMult);
        UpdateAnimationSpd(speedMult);

        //Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Lightsaber_Turn_On");

        presentationTimer = presentationTime;

        if (cam_comp != null)
        {
            cam_comp.Zoom(baseZoom, zoomTimeEasing);
            cam_comp.target = this.gameObject;
        }
        invencible = true;

        Input.PlayHaptic(0.9f, 2200);
    }


    private void UpdatePresentation()
    {
        healthPoints += 100;
        Debug.Log("Presentation");
    }


    private void EndPresentation()
    {
        if(cam_comp!=null)
            cam_comp.target = Core.instance.gameObject;
        invencible = false;
    }

    #endregion

    #region PHASE_CHANGE

    private void StartPhaseChange()
    {
        Animator.Play(gameObject, "MG_Rising", speedMult);
        UpdateAnimationSpd(speedMult);

        //Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Lightsaber_Turn_On");

        changingStateTimer = changingStateTime;

        healthPoints = maxHealthPoints_fase2;

        


        if (cam_comp != null)
        {
            cam_comp.target = this.gameObject;
        }

        Input.PlayHaptic(0.7f, 1000);

        invencible = true;
    }


    private void UpdateChangeState()
    {

        Debug.Log("Changing State");
       
        if (changingStateTimer <= 2.5f && !showingSaber)
        {
            showingSaber = true;
            Audio.PlayAudio(gameObject, "Play_Moff_Gideon_Lightsaber_Turn_On");
            Audio.SetState("Game_State", "Moff_Gideon_Phase_2");
            sword.Enable(true);
            if (camera != null)
            {
                Shake3D shake = camera.GetComponent<Shake3D>();
                if (shake != null)
                {
                    shake.StartShaking(2f, 0.12f);
                    Input.PlayHaptic(2f, 400);
                }
            }
        }
    }


    private void EndFirstStage()
    {
        currentPhase = PHASE.PHASE2;
        enemiesTimer = enemiesTime;
        probWander = probWanderP2;
        followSpeed = 6.8f;
        distance2Melee = 8f;
        distanceProjectile = 10f;
        dashSpeed = 16f;
        dashBackWardDistance = 4f;
        dashForwardDistance = 8f;
        closerDistance = 6f;
        touchDamage = 20f;
        if (cam_comp != null)
            cam_comp.target = Core.instance.gameObject;
        invencible = false;
        showingSaber = false;

    }

    #endregion

    #region NEUTRAL

    private void StartNeutral()
    {
        if(currentPhase == PHASE.PHASE1) Animator.Play(gameObject, "MG_RunPh1", speedMult);
        else if(currentPhase == PHASE.PHASE2) Animator.Play(gameObject, "MG_RunPh2", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Moff_Gideon_Footsteps");
        neutralTimer = neutralTime;
        wander = false;

        if (randomNum.Next(1, 100) <= probWander && !soloDash)
        {
            wander = true;
            agent.CalculateRandomPath(gameObject.transform.globalPosition, radiusWander);
        }

    }


    private void UpdateNeutral()
    {
        if (agent != null && !wander)
        {
            agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
        }

        //Move character
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(followSpeed * speedMult);

        Debug.Log("Neutral");
    }


    private void EndNeutral()
    {
        soloDash = false;
        Audio.StopOneAudio(gameObject, "Play_Moff_Gideon_Footsteps");
    }

    #endregion

    #region DASH_FORWARD
    private void StartDashForward()
    {
        targetDash = Core.instance.gameObject.transform.globalPosition;
        beginDash = gameObject.transform.globalPosition;
        Animator.Play(gameObject, "MG_Dash", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Dash");
    }

    private void UpdateDashForward()
    {
        LookAt(targetDash);
        if (Mathf.Distance(gameObject.transform.globalPosition, targetDash) >= 1f) MoveToPosition(targetDash, dashSpeed * speedMult);
        Debug.Log("Dash Forward");
    }

    private void EndDashForward()
    {

    }

    #endregion

    #region MELEE_COMBO
    private void StartMeleeCombo()
    {
        if (currentPhase == PHASE.PHASE1)
        {
            nAtacks = numAtacksPh1;
            sword.Enable(true);
        }
        else
        {
            nAtacks = numAtacksPh2;
        }
        int rand = randomNum.Next(1, 6);
        sword.EnableCollider();
        Animator.Play(gameObject, atacks[rand].animation, speedMult);
        UpdateAnimationSpd(speedMult);
        comboTimer = atacks[rand].duration;
        Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Lightsaber_Whoosh");
        nAtacks--;
        Input.PlayHaptic(0.5f, 500);
    }

    private void UpdateMeleeCombo()
    {
        LookAt(Core.instance.gameObject.transform.globalPosition);
        Debug.Log("Combo");
    }

    private void EndMeleeCombo()
    {
        sword.DisableCollider();
    }

    #endregion

    #region DASH_BACKWARD
    private void StartDashBackward()
    {
        nSequences = numSequencesPh2;
        if (currentPhase == PHASE.PHASE1) sword.Enable(false);
        beginDash = gameObject.transform.globalPosition;
        Audio.PlayAudio(gameObject, "MG_Dash");
    }

    private void UpdateDashBackward()
    {
        gameObject.transform.localPosition += -1 * gameObject.transform.GetForward() * dashSpeed * myDeltaTime * speedMult;

        Debug.Log("Dash Backward");
    }

    private void EndDashBackward()
    {

    }

    #endregion

    #region PROJECTILE

    private void StartProjectile()
    {
        Animator.Play(gameObject, "MG_Shoot", speedMult);
        UpdateAnimationSpd(speedMult);
        cadencyTimer = cadencyTime;
        aiming = true;
        privateTimer = 1f;
        cam_comp.Zoom(zoomInValue, zoomTimeEasing);
    }

    private void UpdateProjectile()
    {
        Debug.Log("Projectile");

        if (aiming)
        {
            LookAt(Core.instance.gameObject.transform.globalPosition);

            if (privateTimer > 0)
            {
                privateTimer -= myDeltaTime;

                if (privateTimer <= 0)
                    aiming = false;
            }

            return;
        }

        gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, Quaternion.RotateAroundAxis(Vector3.up, 50), 1f * myDeltaTime * speedMult);

        if (cadencyTimer > 0)
        {
            cadencyTimer -= myDeltaTime;

            if (cadencyTimer <= 0)
            {
                Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Shot");
                cadencyTimer = cadencyTime;
                projectiles++;
                GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/1606118587.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, shootPoint.transform.globalScale);
                Input.PlayHaptic(0.3f, 100);
            }

        }
    }

    private void EndProjectile()
    {
        cam_comp.Zoom(baseZoom, zoomTimeEasing);
    }



    #endregion

    #region SPAWN_ENEMIES

    private void StartSpawnEnemies()
    {
        if (currentPhase == PHASE.PHASE1)
        {
            invencible = true;
            Animator.Play(gameObject, "MG_EnemySpawnerPh1", speedMult);
        }
        else if (currentPhase == PHASE.PHASE2) Animator.Play(gameObject, "MG_EnemySpawnPh2", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Spawn_Enemies");
        if (cam_comp != null)
            cam_comp.target = this.gameObject;
        privateTimer = 0.5f;

        Input.PlayHaptic(0.8f, 600);
    }

    private void UpdateSpawnEnemies()
    {
        Debug.Log("Spawning Enemies");
        if (privateTimer > 0)
        {
            privateTimer -= myDeltaTime;

            if (privateTimer <= 0)
                if(ready2Spawn)
                {
                    SpawnEnemies();
                    privateTimer = 1.5f;
                    ready2Spawn = false;
                    deathTrooperSpawned = true;
                }
                else
                {
                    if (cam_comp != null)
                        cam_comp.target = Core.instance.gameObject;

                    if (currentPhase == PHASE.PHASE2) inputsList.Add(INPUT.IN_NEUTRAL);
                }

        }
    }

    private void EndSpawnEnemies()
    {
        enemiesTimer = enemiesTime;
        invencible = false;
        deathTrooperSpawned = false;
    }

    #endregion

    #region THROW SABER

    private void StartChargeThrow()
    {
        chargeThrowTimer = chargeThrowTime;
        cam_comp.Zoom(zoomInValue, zoomTimeEasing);
        visualFeedback = InternalCalls.CreatePrefab("Library/Prefabs/1137197426.prefab", gameObject.transform.globalPosition, gameObject.transform.globalRotation, new Vector3(0.3f, 1f, 0.01f));
        Animator.Play(gameObject, "MG_SaberThrow");
        UpdateAnimationSpd(speedMult);
    }


    private void UpdateChargeThrow()
    {
        Debug.Log("Charge Throw");
        LookAt(Core.instance.gameObject.transform.globalPosition);
        if (visualFeedback.transform.globalScale.z < 1.0)
        {
            visualFeedback.transform.localScale = new Vector3(0.3f, 1.0f, Mathf.Lerp(visualFeedback.transform.localScale.z, 1.0f, myDeltaTime * (chargeThrowTime / chargeThrowTimer)));
            visualFeedback.transform.localRotation = gameObject.transform.globalRotation;
        }
    }


    private void EndChargeThrow()
    {
        InternalCalls.Destroy(visualFeedback);
        visualFeedback = null;
    }

    private void StartThrowSaber()
    {
        saber = InternalCalls.CreatePrefab("Library/Prefabs/1894242407.prefab", shootPoint.transform.globalPosition, new Quaternion(0,0,0), new Vector3(1.0f, 1.0f, 1.0f));

        if (saber != null)
        {
            MoffGideonSword moffGideonSword = saber.GetComponent<MoffGideonSword>();

            if(moffGideonSword != null)
            {
                moffGideonSword.ThrowSword((Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition).normalized, swordRange);
                saber.Enable(true);
                sword.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                sword.transform.localRotation = new Quaternion(-90, 0, 90);
                inputsList.Add(INPUT.IN_THROW_SABER_END);
            }
        }

        Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Lightsaber_Throw");
    }


    private void UpdateThrowSaber()
    {
        Debug.Log("Throw Saber");
    }


    private void EndThrowSaber()
    {

    }

    private void StartRetrieveSaber()
    {
        if (currentPhase == PHASE.PHASE1) Animator.Play(gameObject, "MG_RunPh1", speedMult);
        else if (currentPhase == PHASE.PHASE2) Animator.Play(gameObject, "MG_RunPh2", speedMult);
        UpdateAnimationSpd(speedMult);
        privateTimer = 1f;
    }


    private void UpdateRetrieveSaber()
    {
        agent.CalculatePath(gameObject.transform.globalPosition, saber.transform.globalPosition);
        agent.MoveToCalculatedPos(speedMult * followSpeed);
        LookAt(agent.GetDestination());
        if (privateTimer > 0f)
        {
            privateTimer -= myDeltaTime;
            if (privateTimer <= 0f)
            {
                retrieveAnim = false;
            }
        }
        Debug.Log("Retrieve Saber");
    }


    private void EndRetrieveSaber()
    {
        InternalCalls.Destroy(saber);
        saber = null;
        sword.transform.localScale = new Vector3(1, 1, 1);
        cam_comp.Zoom(baseZoom, zoomTimeEasing);
        retrieveAnim = true;
    }

    #endregion

    #region DIE
    private void StartDie()
    {
        dieTimer = dieTime;
        Animator.Play(gameObject, "MG_Death", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Moff_Gideon_Lightsaber_Turn_Off");
        Audio.PlayAudio(gameObject, "Play_Moff_Gideon_Death");
        Audio.PlayAudio(gameObject, "Play_Victory_Music");

        Input.PlayHaptic(1f, 1000);

        if (cam_comp != null)
            cam_comp.target = this.gameObject;
        if (visualFeedback != null)
            InternalCalls.Destroy(visualFeedback);

        for(int i = 0; i < deathtroopers.Count; ++i)
        {
            if (deathtroopers[i] != null)
                InternalCalls.Destroy(deathtroopers[i]);
        }
        deathtroopers.Clear();
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

    public void Die()
    {
        Debug.Log("MOFF'S DEAD");
        Counter.SumToCounterType(Counter.CounterTypes.MOFFGIDEON);
        EnemyManager.RemoveEnemy(gameObject);

        Animator.Pause(gameObject);
        Audio.StopAudio(gameObject);
        Input.PlayHaptic(0.3f, 3);
        InternalCalls.Destroy(gameObject);
        if (cam_comp != null)
            cam_comp.target = Core.instance.gameObject;
    }
    #endregion

    public void LookAt(Vector3 pointToLook)
    {
        Vector3 direction = pointToLook - gameObject.transform.globalPosition;
        direction = direction.normalized;
        float angle = (float)Math.Atan2(direction.x, direction.z);

        if (Math.Abs(angle * Mathf.Rad2Deg) < 1.0f)
            return;

        Quaternion dir = Quaternion.RotateAroundAxis(Vector3.up, angle);

        float rotationSpeed = myDeltaTime * slerpSpeed;

        Quaternion desiredRotation = Quaternion.Slerp(gameObject.transform.localRotation, dir, rotationSpeed);

        gameObject.transform.localRotation = desiredRotation;

    }

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
            Debug.Log("GIDEON HP: " + healthPoints.ToString());

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

            if (Core.instance.hud != null  && currentState != STATE.DEAD)
            {
                HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                if (hudComponent != null)
                {
                    hudComponent.AddToCombo(25, 0.95f);
                }
            }

            if (currentState != STATE.DEAD && healthPoints <= 0.0f)
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
            Debug.Log("Rancor HP: " + healthPoints.ToString());
            //damaged = 1.0f;
            //CHANGE FOR APPROPIATE RANCOR HIT
            Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Hit_Phase_1");

            if (Core.instance.hud != null && currentState != STATE.DEAD)
            {
                HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                if (hudComponent != null)
                {
                    hudComponent.AddToCombo(55, 0.25f);
                }
            }
        }
        else if (collidedGameObject.CompareTag("WorldLimit"))
        {
            if (currentState != STATE.DEAD)
            {
                inputsList.Add(INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("Player"))
        {
            if (currentState == STATE.DEAD) return;

            if (currentState == STATE.DASH_FORWARD_LONG && justDashing)
                inputsList.Add(INPUT.IN_NEUTRAL);


            float damageToPlayer = touchDamage;

            PlayerHealth playerHealth = collidedGameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)(damageToPlayer * damageMult));
            }
            Debug.Log(damageToPlayer.ToString() + " " + damageMult.ToString());

        }
        else if (collidedGameObject.CompareTag("Wall"))
        {
            //if (currentState == STATE.DASH_FORWARD_LONG && justDashing)
            //    inputsList.Add(INPUT.IN_NEUTRAL);

            //else if (currentState == STATE.DASH_FORWARD_LONG && !justDashing)
            //    inputsList.Add(INPUT.IN_DASH_FORWARD_END);

            //else if (currentState == STATE.DASH_BACKWARDS)
            //    inputsList.Add(INPUT.IN_DASH_BACKWARDS_END);

        }
    }

    public void TakeDamage(float damage)
    {
        if (invencible) return;

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
            healthPoints -= damage * mod; 
            if (currentPhase == PHASE.PHASE1)
            {
                Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Hit_Phase_1");
            }
            else if (currentPhase == PHASE.PHASE2)
            {
                Audio.PlayAudio(gameObject, "Play_Moff_Guideon_Hit_Phase_2");
            }
            Debug.Log("Moff damage" + damage.ToString());
            if (currentState != STATE.DEAD)
            {
                healthPoints -= damage * Core.instance.DamageToBosses;
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
                if (healthPoints <= 0.0f)
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

    private void UpdateAnimationSpd(float newSpd)
    {
        if (currAnimationPlaySpd != newSpd)
        {
            Animator.SetSpeed(gameObject, newSpd);
            currAnimationPlaySpd = newSpd;
        }
    }

    #region STATUS_SYSTEM

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

    private void SpawnEnemies()
    {
        SpawnDeathrooper(spawner1);
        SpawnDeathrooper(spawner2);
        SpawnDeathrooper(spawner3);
        SpawnDeathrooper(spawner4);
    }

    private void SpawnDeathrooper(GameObject spawnPoint)
    {
        if (spawnPoint == null)
            return;

        //Spawn Enemy
        GameObject deathtrooper = InternalCalls.CreatePrefab("Library/Prefabs/1439379622.prefab", spawnPoint.transform.globalPosition, gameObject.transform.localRotation, new Vector3(1.0f, 1.0f, 1.0f));

        if (deathtrooper != null)
        {
            deathtroopers.Add(deathtrooper);

            Deathtrooper deathtrooperScript = deathtrooper.GetComponent<Deathtrooper>();

            if (deathtrooperScript != null)
                deathtrooperScript.moffGideon = this;
        }

        //Play Particles
        ParticleSystem particleSystem = spawnPoint.GetComponent<ParticleSystem>();

        if (particleSystem != null)
            particleSystem.Play();
    }

    public void RemoveDeathrooperFromList(GameObject deathtrooper)
    {
        if (deathtrooper == null)
            return;

        for (int i = 0; i < deathtroopers.Count; ++i)
        {
            if (deathtroopers[i].GetUid() == deathtrooper.GetUid())
            {
                deathtroopers.RemoveAt(i);
                return;
            }
        }
    }

    public override bool IsDying()
    {
        return currentState == STATE.DEAD;
    }

    public void MoveToPosition(Vector3 positionToReach, float speed)
    {
        Vector3 direction = positionToReach - gameObject.transform.localPosition;

        gameObject.transform.localPosition += direction.normalized * speed * myDeltaTime;
    }

}