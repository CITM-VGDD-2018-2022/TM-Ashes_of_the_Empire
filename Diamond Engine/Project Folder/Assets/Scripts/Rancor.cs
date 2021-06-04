using System;
using DiamondEngine;

using System.Collections.Generic;

public class Rancor : Entity
{
    enum RANCOR_STATE : int
    {
        NONE = -1,
        ROAR,
        SEARCH_STATE,
        FOLLOW,
        LOADING_RUSH,
        RUSH,
        RUSH_STUN,
        HAND_SLAM,
        PROJECTILE,
        MELEE_COMBO_HIT1,
        MELEE_COMBO_HIT2,
        MELEE_COMBO_HIT3,
        DEAD
    }

    enum RANCOR_INPUT : int
    {
        NONE = -1,
        IN_ROAR,
        IN_ROAR_END,
        IN_FOLLOW_SHORT,
        IN_FOLLOW_LONG,
        IN_FOLLOW_END,
        IN_LOADING_RUSH,
        IN_RUSH,
        IN_RUSH_END,
        IN_RUSH_STUN,
        IN_RUSH_STUN_END,
        IN_HAND_SLAM,
        IN_HAND_SLAM_END,
        IN_PROJECTILE,
        IN_PROJECTILE_END,
        IN_MELEE_COMBO_1HIT,
        IN_MELEE_COMBO_2HIT,
        IN_MELEE_COMBO_3HIT,
        IN_MELEE_COMBO_END,
        IN_DEAD
    }

    private NavMeshAgent agent = null;
    public GameObject camera = null;

    //private Vector3 targetPosition = null;
    private Vector3 targetDirection = null;

    //State
    private RANCOR_STATE currentState = RANCOR_STATE.ROAR;   //NEVER SET THIS VARIABLE DIRECTLLY, ALLWAYS USE INPUTS
                                                             //Setting states directlly will break the behaviour  -Jose
    private List<RANCOR_INPUT> inputsList = new List<RANCOR_INPUT>();
    Random randomNum = new Random();

    public GameObject hitParticles = null;

    public float slerpSpeed = 5.0f;

    //Stats
    public float healthPoints = 1500.0f;          //IF INITAL HEALTH IS CHANGED, CHANGE MAX HEALTH AS WELL!
    public float maxHealthPoints = 1500.0f;

    public int attackProbability = 80;  //FROM 1 TO A 100
    public int shortFollowProbability = 90; //FROM THE PREVIOS VALUE TO HERE

    public float meleeRange = 10.0f;
    public float longRange = 22.0f;

    private float damageMult = 1f;
    public float damageRecieveMult { get; private set; } = 1f;

    //Follow
    public float shortFollowTime = 2.0f;
    public float longFollowTime = 4.0f;
    private float followTimer = 0.0f;
    public float followSpeed = 2.0f;

    public float meleeComboMovSpeed = 0.0f;

    public float jumpAttackSpeed = 2.0f;
    private Vector3 jumpAttackTarget;

    public float jumpDelayDuration = 0.0f;
    private float jumpDelayTimer = 0.0f;

    private bool startedJumping = false;

    private float meleeComboHit1Time = 0.0f;
    private float meleeComboHit2Time = 0.0f;
    private float meleeComboHit3Time = 0.0f;

    private float meleeCH1Timer = 0.0f;
    private float meleeCH2Timer = 0.0f;
    private float meleeCH3Timer = 0.0f;

    public float meleeCH1ColliderDuration = 0.0f;
    public float meleeCH2ColliderDuration = 0.0f;
    public float meleeComboHit3CollisionTimeToActivate = 0.0f;
    public float meleeComboHit3CollisionDuration = 0.0f;


    private float meleeCH1ColliderTimer = 0.0f;
    private float meleeCH2ColliderTimer = 0.0f;
    private float meleeCH3ColliderTimer = 0.0f;
    private float meleeCH3ColliderActiveTimer = 0.0f;

    private bool meleeHit3Haptic = false;

    private bool meleeShaked = false;

    private float currAnimationPlaySpd = 1f;

    //Projectile
    private float projectileTime = 0.0f;
    private float projectileTimer = 0.0f;

    public float prepareShotDuration = 0.5f;
    private float prepareShotTimer = 0.0f;

    public GameObject projectilePoint = null;
    private Vector3 target = new Vector3(0, 0, 0);

    //Hand slam
    private float handSlamTime = 0.0f;
    private float handSlamTimer = 0.0f;
    public GameObject handSlamHitBox = null;
    public GameObject handSlamWave = null;
    private float handSlamTimerToActivate = 0.0f;
    private float handSlamTimeToActivate = 1.0f;
    private bool activateWave = false;
    public float handSlamHapticStrenght = 10f;

    //Rush
    public float rushDamage = 10.0f;
    public float touchDamage = 5.0f;
    public float loadRushTime = 0.4f;
    private float loadRushTimer = 0.0f;

    private float rushRecoveryTimer = 0.0f;
    private float rushRecoveryTime = 0.0f;

    public float rushSpeed = 15.0f;
    private float rushTime = 0.0f;
    private float rushTimer = 0.0f;

    private float rushStunDuration = 1.2f;
    private float rushStunTimer = 0.0f;

    private bool startRush = false;


    //Die
    private float dieTime = 0.0f;
    private float dieTimer = 0.0f;

    //Roar
    private float roarTime = 0.0f;
    private float roarTimer = 0.0f;
    private bool roarShaked = false;

    //Boss bar updating
    public GameObject boss_bar = null;
    public GameObject rancor_mesh = null;
    private float damaged = 0.0f;
    private float limbo_health = 0.0f;

    private bool start = false;

    //Particles timers
    private float runTime = 0.0f;
    private float dustTime = 0.0f;
    private bool toggleLegParticle;

    private bool impact = true;
    enum PARTICLES : int
    {
        NONE = -1,
        TRAILLEFT,
        TRAILRIGHT,
        IMPACT,
        RUSH,
        SWINGLEFT,
        SWINGRIGHT,
        HANDSLAM,
        ROAR
    }
    RancorParticles rancorParticles = null;
    private void Start()
    {
        followTimer = shortFollowTime;

        meleeComboHit1Time = Animator.GetAnimationDuration(gameObject, "RN_MeleeComboP1") - 0.016f;
        meleeComboHit2Time = Animator.GetAnimationDuration(gameObject, "RN_MeleeComboP2") - 0.016f;
        meleeComboHit3Time = Animator.GetAnimationDuration(gameObject, "RN_MeleeComboP3") - 0.016f;

        roarTime = Animator.GetAnimationDuration(gameObject, "RN_Roar") - 0.016f;

        projectileTime = Animator.GetAnimationDuration(gameObject, "RN_ProjectileThrow") - 0.016f;

        rushRecoveryTime = Animator.GetAnimationDuration(gameObject, "RN_RushRecover") - 0.016f;

        handSlamTime = Animator.GetAnimationDuration(gameObject, "RN_HandSlam") - 0.016f;

        rushTime = Animator.GetAnimationDuration(gameObject, "RN_Rush") - 0.016f;
        //Debug.Log("RUSH TiME: " + rushTime.ToString());

        //rushStunDuration = Animator.GetAnimationDuration(gameObject, "RN_RushRecover") - 0.016f;

        dieTime = Animator.GetAnimationDuration(gameObject, "RN_Die") - 0.016f;

        damaged = 0.0f;
        runTime = (Animator.GetAnimationDuration(gameObject, "RN_Walk")) / 2.0f;
        dustTime = (Animator.GetAnimationDuration(gameObject, "RN_Walk")) / 4.0f;
        rancorParticles = gameObject.GetComponent<RancorParticles>();
        toggleLegParticle = true;
        StartRoar();

    }

    public void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        InitEntity(ENTITY_TYPE.RANCOR);

        damageMult = 1f;

        if (EnemyManager.EnemiesLeft() > 0)
            EnemyManager.ClearList();

        EnemyManager.AddEnemy(gameObject);

        if (agent == null)
            Debug.Log("Null agent, add a NavMeshAgent Component");

        Audio.SetState("Game_State", "Rancor_Room");
    }

    public void Update()
    {
        if (start == false)
        {
            Start();
            start = true;
        }

        myDeltaTime = Time.deltaTime * speedMult;
        UpdateStatuses();

        ProcessInternalInput();
        ProcessExternalInput();
        ProcessState();

        UpdateState();
    }

    //Timers go here
    private void ProcessInternalInput()
    {
        if (followTimer > 0)
        {
            followTimer -= myDeltaTime;

            if (followTimer <= 0)
            {
                inputsList.Add(RANCOR_INPUT.IN_FOLLOW_END);
            }
        }

        if (meleeCH1Timer > 0)
        {
            meleeCH1Timer -= myDeltaTime;

            if (meleeCH1Timer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_MELEE_COMBO_2HIT);
        }

        if (meleeCH2Timer > 0)
        {
            meleeCH2Timer -= myDeltaTime;

            if (meleeCH2Timer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_MELEE_COMBO_3HIT);
        }

        if (meleeCH3Timer > 0)
        {
            meleeCH3Timer -= myDeltaTime;

            if (meleeCH3Timer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_MELEE_COMBO_END);
        }

        if (projectileTimer > 0)
        {
            projectileTimer -= myDeltaTime;

            if (projectileTimer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_PROJECTILE_END);
        }


        if (handSlamTimer > 0)
        {
            handSlamTimer -= myDeltaTime;

            if (handSlamTimer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_HAND_SLAM_END);
        }

        if (loadRushTimer > 0)
        {
            loadRushTimer -= myDeltaTime;

            if (loadRushTimer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_RUSH);
        }

        if (rushTimer > 0)
        {
            rushTimer -= myDeltaTime;

            if (rushTimer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_RUSH_END);
        }


        if (rushStunTimer > 0)
        {
            rushStunTimer -= myDeltaTime;

            if (rushStunTimer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_RUSH_STUN_END);
        }

        if (roarTimer > 0)
        {
            roarTimer -= myDeltaTime;
            healthPoints = (1 - (roarTimer / roarTime)) * maxHealthPoints;
            //Debug.Log("Rancor health: " + healthPoints.ToString());
            if (roarTimer <= 0)
            {
                healthPoints = maxHealthPoints;
                limbo_health = healthPoints;
                inputsList.Add(RANCOR_INPUT.IN_ROAR_END);
                //Debug.Log("finishing roar");
            }
        }

        if (rushRecoveryTimer > 0)
        {
            rushRecoveryTimer -= myDeltaTime;

            if (rushRecoveryTimer <= 0)
            {
                inputsList.Add(RANCOR_INPUT.IN_RUSH_STUN_END);
            }
        }
    }

    private void ProcessExternalInput()
    {

    }

    private void ProcessState()
    {
        while (inputsList.Count > 0)
        {
            RANCOR_INPUT input = inputsList[0];

            switch (currentState)
            {
                case RANCOR_STATE.NONE:
                    Debug.Log("RANCOR ERROR STATE");
                    break;

                case RANCOR_STATE.SEARCH_STATE:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_FOLLOW_SHORT:
                            currentState = RANCOR_STATE.FOLLOW;
                            StartShortFollow();
                            break;

                        case RANCOR_INPUT.IN_FOLLOW_LONG:
                            currentState = RANCOR_STATE.FOLLOW;
                            StartLongFollow();
                            break;

                        case RANCOR_INPUT.IN_LOADING_RUSH:
                            currentState = RANCOR_STATE.LOADING_RUSH;
                            StartLoadingRush();
                            break;

                        case RANCOR_INPUT.IN_HAND_SLAM:
                            currentState = RANCOR_STATE.HAND_SLAM;
                            StartHandSlam();
                            break;

                        case RANCOR_INPUT.IN_PROJECTILE:
                            currentState = RANCOR_STATE.PROJECTILE;
                            StartProjectile();
                            break;

                        case RANCOR_INPUT.IN_MELEE_COMBO_1HIT:
                            currentState = RANCOR_STATE.MELEE_COMBO_HIT1;
                            StartMCHit1();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.ROAR:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_ROAR_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            EndRoar();
                            break;
                    }
                    break;

                case RANCOR_STATE.FOLLOW:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_FOLLOW_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            EndFollow();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            EndFollow();
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.LOADING_RUSH:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_RUSH:
                            currentState = RANCOR_STATE.RUSH;
                            StartRush();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.RUSH:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_RUSH_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            EndRush();
                            break;

                        case RANCOR_INPUT.IN_RUSH_STUN:
                            currentState = RANCOR_STATE.RUSH_STUN;
                            EndRush();
                            StartRushStun();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            EndRush();
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.RUSH_STUN:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_RUSH_STUN_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            EndRushStun();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            EndRushStun();
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.HAND_SLAM:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_HAND_SLAM_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            EndHandSlam();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            EndHandSlam();
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.PROJECTILE:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_PROJECTILE_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            EndProjectile();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            EndProjectile();
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.MELEE_COMBO_HIT1:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_MELEE_COMBO_2HIT:
                            currentState = RANCOR_STATE.MELEE_COMBO_HIT2;
                            EndMCHit1();
                            StartMCHit2();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            EndMCHit1();
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.MELEE_COMBO_HIT2:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_MELEE_COMBO_3HIT:
                            currentState = RANCOR_STATE.MELEE_COMBO_HIT3;
                            EndMCHit2();
                            StartMCHit3();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            EndMCHit2();
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.MELEE_COMBO_HIT3:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_MELEE_COMBO_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            EndMCHit3();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            EndMCHit3();
                            StartDie();
                            break;
                    }
                    break;

                default:
                    Debug.Log("NEED TO ADD STATE TO RANCOR");
                    break;
            }
            inputsList.RemoveAt(0);
        }
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case RANCOR_STATE.NONE:
                Debug.Log("RANCOR ERROR STATE");
                break;

            case RANCOR_STATE.ROAR:
                UpdateRoar();
                break;

            case RANCOR_STATE.SEARCH_STATE:
                SelectAction();
                break;

            case RANCOR_STATE.FOLLOW:
                UpdateFollow();
                break;

            case RANCOR_STATE.LOADING_RUSH:
                UpdateLoadingRush();
                break;

            case RANCOR_STATE.RUSH:
                UpdateRush();
                break;

            case RANCOR_STATE.RUSH_STUN:
                UpdateRushStun();
                break;

            case RANCOR_STATE.HAND_SLAM:
                UpdateHandSlam();
                break;

            case RANCOR_STATE.PROJECTILE:
                UpdateProjectile();
                break;

            case RANCOR_STATE.MELEE_COMBO_HIT1:
                UpdateMCHit1();
                break;

            case RANCOR_STATE.MELEE_COMBO_HIT2:
                UpdateMCHit2();
                break;

            case RANCOR_STATE.MELEE_COMBO_HIT3:
                UpdateMCHit3();
                break;

            case RANCOR_STATE.DEAD:
                UpdateDie();
                break;
        }
        limbo_health = Mathf.Lerp(limbo_health, healthPoints, 0.01f);
        if (boss_bar != null)
        {
            Material bossBarMat = boss_bar.GetComponent<Material>();

            if (bossBarMat != null)
            {
                bossBarMat.SetFloatUniform("length_used", healthPoints / maxHealthPoints);
                bossBarMat.SetFloatUniform("limbo", limbo_health / maxHealthPoints);
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
        if (rancor_mesh != null)
        {
            Material rancorMeshMat = rancor_mesh.GetComponent<Material>();

            if (rancorMeshMat != null)
            {
                rancorMeshMat.SetFloatUniform("damaged", damaged);
            }
            else
                Debug.Log("Rancor Mesh Material was null!!");
        }
        if (rancorParticles.rush.playing && currentState != RANCOR_STATE.RUSH && currentState != RANCOR_STATE.RUSH_STUN)
            PlayParticles(PARTICLES.RUSH);
    }

    private void SelectAction()
    {
        int decision = randomNum.Next(1, 100);

        //Debug.Log("Decision value: " + decision.ToString());

        if (decision <= attackProbability)
        {
            //Do all distance checks
            float distance = Mathf.Distance(Core.instance.gameObject.transform.localPosition, gameObject.transform.localPosition);

            if (distance <= meleeRange)
            {
                decision = randomNum.Next(1, 100);

                if (decision <= 50)
                    inputsList.Add(RANCOR_INPUT.IN_MELEE_COMBO_1HIT);

                else
                    inputsList.Add(RANCOR_INPUT.IN_HAND_SLAM);
            }

            else if (distance > meleeRange && distance <= longRange)
            {
                decision = randomNum.Next(1, 100);

                if (decision <= 50)
                    inputsList.Add(RANCOR_INPUT.IN_PROJECTILE);

                else
                    inputsList.Add(RANCOR_INPUT.IN_LOADING_RUSH);
            }
            else
            {
                //Projectile
                inputsList.Add(RANCOR_INPUT.IN_PROJECTILE);
            }
        }
        else if (decision > attackProbability && decision <= shortFollowProbability)
            inputsList.Add(RANCOR_INPUT.IN_FOLLOW_SHORT);

        else if (decision > shortFollowProbability)
            inputsList.Add(RANCOR_INPUT.IN_FOLLOW_LONG);
    }


    #region MELEE_COMBO
    private void StartMCHit1()
    {
        meleeCH1Timer = meleeComboHit1Time;
        meleeCH1ColliderTimer = meleeCH1ColliderDuration;

        Animator.Play(gameObject, "RN_MeleeComboP1", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Rancor_Melee_1rst_Punch");
        LookAt(Core.instance.gameObject.transform.globalPosition);
    }

    private void UpdateMCHit1()
    {
        //Debug.Log("Combo hit 1");

        if (meleeCH1ColliderTimer > 0.0f)
        {
            meleeCH1ColliderTimer -= myDeltaTime;

            if (meleeCH1ColliderTimer <= 0.0f)
            {
                //InternalCalls.CreatePrefab("Library/Prefabs/1846472793.prefab", gameObject.transform.localPosition, gameObject.transform.localRotation, gameObject.transform.localScale);
            }
        }

        if (meleeCH1Timer < (meleeComboHit1Time / 4) + 0.2f && impact)
        {
            PlayParticles(PARTICLES.SWINGRIGHT);
            impact = false;
        }

        LookAt(Core.instance.gameObject.transform.globalPosition);
        gameObject.transform.localPosition += gameObject.transform.GetForward().normalized * meleeComboMovSpeed * myDeltaTime;

        UpdateAnimationSpd(speedMult);
    }

    private void EndMCHit1()
    {
        impact = true;
    }


    private void StartMCHit2()
    {
        meleeCH2Timer = meleeComboHit2Time;
        meleeCH2ColliderTimer = meleeCH2ColliderDuration;

        Animator.Play(gameObject, "RN_MeleeComboP2", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Rancor_Melee_2nd_Punch");

        LookAt(Core.instance.gameObject.transform.globalPosition);
    }

    private void UpdateMCHit2()
    {
       // Debug.Log("Combo hit 2");
        if (meleeCH2ColliderTimer > 0.0f)
        {
            meleeCH2ColliderTimer -= myDeltaTime;

            if (meleeCH2ColliderTimer <= 0.0f)
            {
                //InternalCalls.CreatePrefab("Library/Prefabs/1846472793.prefab", gameObject.transform.localPosition, gameObject.transform.localRotation, gameObject.transform.localScale);
            }
        }

        if (meleeCH2Timer < (meleeComboHit2Time / 2) + 0.2f && impact)
        {
            PlayParticles(PARTICLES.SWINGLEFT);
            impact = false;
        }

        LookAt(Core.instance.gameObject.transform.globalPosition);
        gameObject.transform.localPosition += gameObject.transform.GetForward().normalized * meleeComboMovSpeed * myDeltaTime;

        UpdateAnimationSpd(speedMult);
    }

    private void EndMCHit2()
    {
        impact = true;
    }


    private void StartMCHit3()
    {
        meleeCH3Timer = meleeComboHit3Time;
        meleeCH3ColliderTimer = meleeComboHit3CollisionTimeToActivate;
        meleeCH3ColliderActiveTimer = meleeComboHit3CollisionDuration;

        Animator.Play(gameObject, "RN_MeleeComboP3", speedMult);
        UpdateAnimationSpd(speedMult);

        jumpAttackTarget = Core.instance.gameObject.transform.globalPosition;
        LookAt(jumpAttackTarget);

        jumpDelayTimer = jumpDelayDuration;

        meleeHit3Haptic = true;
    }

    private void UpdateMCHit3()
    {
        //Debug.Log("Combo hit 3");

        if (meleeCH3ColliderTimer > 0.0f)
        {
            meleeCH3ColliderTimer -= myDeltaTime;

            if (meleeCH3ColliderTimer <= 0.0f)
            {
                Audio.PlayAudio(gameObject, "Play_Rancor_Area_Impact");

                Vector3 pos = gameObject.transform.localPosition;
                pos.y += 3;

                RancorJumpCollider jumpColl = InternalCalls.CreatePrefab("Library/Prefabs/376114835.prefab", pos, gameObject.transform.localRotation, gameObject.transform.localScale).GetComponent<RancorJumpCollider>();

                if (jumpColl != null)
                {
                    jumpColl.deltaTimeMult = speedMult;
                    jumpColl.damage = (int)(jumpColl.damage * damageMult);
                }

                //gameObject.DisableCollider();
            }

        }
        if (meleeCH3Timer < 1.95f && !meleeShaked)
        {
            meleeShaked = true;
            Shake3D shake = camera.GetComponent<Shake3D>();
            if (shake != null)
            {
                shake.StartShaking(0.8f, 0.1f);

            }

        }
        if (meleeCH3Timer < 1.5f)
        {
            if (meleeHit3Haptic)
            {
                meleeHit3Haptic = false;
                Input.PlayHaptic(0.8f, 500);
                //Debug.Log("Hpatic Jump");

            }
        }
        if (meleeCH3Timer < (meleeComboHit3Time / 2) + 0.2f && impact)
        {
            PlayParticles(PARTICLES.IMPACT);
            impact = false;
        }

        if (jumpDelayTimer > 0.0f)
        {
            jumpDelayTimer -= myDeltaTime;

            if (jumpDelayTimer <= 0.0f)
            {
                //TODO: Disable collider component
                startedJumping = true;
                //gameObject.EnableCollider();
            }

        }

        if (startedJumping)
        {
            float x = Mathf.Lerp(gameObject.transform.localPosition.x, jumpAttackTarget.x, jumpAttackSpeed * myDeltaTime);
            float z = Mathf.Lerp(gameObject.transform.localPosition.z, jumpAttackTarget.z, jumpAttackSpeed * myDeltaTime);
            gameObject.transform.localPosition = new Vector3(x, gameObject.transform.localPosition.y, z);
        }
        LookAt(jumpAttackTarget);

        UpdateAnimationSpd(speedMult);
    }

    private void EndMCHit3()
    {
        startedJumping = false;
        impact = true;
        meleeShaked = false;
    }

    #endregion

    #region FOLLOW

    private void StartShortFollow()
    {
        //Start walk animation
        Animator.Play(gameObject, "RN_Walk", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "PLay_Rancor_Footsteps");
        //Search point
        followTimer = shortFollowTime;
        toggleLegParticle = true;

    }


    private void StartLongFollow()
    {
        //Start walk animation
        //Search point

        Animator.Play(gameObject, "RN_Walk", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "PLay_Rancor_Footsteps");
        followTimer = longFollowTime;
        toggleLegParticle = true;
    }


    private void UpdateFollow()
    {
        if (agent != null)
            agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);

        //Move character
        if (agent != null && Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance)
        {
            LookAt(agent.GetDestination());
            agent.MoveToCalculatedPos(followSpeed * speedMult);
        }

        dustTime += myDeltaTime;
        if (dustTime >= runTime)
        {
            if (toggleLegParticle)
                PlayParticles(PARTICLES.TRAILLEFT);
            else
                PlayParticles(PARTICLES.TRAILRIGHT);
            toggleLegParticle = !toggleLegParticle;
            dustTime = 0.0f;
        }

        //Debug.Log("Following");
        UpdateAnimationSpd(speedMult);
    }


    private void EndFollow()
    {
        Audio.StopAudio(gameObject);
        Animator.Play(gameObject, "RN_Idle", speedMult);
        UpdateAnimationSpd(speedMult);
        //Debug.Log("End Following");
    }

    #endregion

    #region PROJECTILE

    private void StartProjectile()
    {
        projectileTimer = projectileTime;

        Animator.Play(gameObject, "RN_ProjectileThrow", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Rancor_Throw");
        //add timer to spawn projectiles

        prepareShotTimer = prepareShotDuration;
    }

    private void UpdateProjectile()
    {
        if (prepareShotTimer > 0.0f)
        {
            prepareShotTimer -= myDeltaTime;

            LookAt(Core.instance.gameObject.transform.globalPosition);

            if (prepareShotTimer <= 0.0f)
            {
                if (projectilePoint != null && Core.instance != null)
                {
                    Vector3 pos = projectilePoint.transform.globalPosition;
                    Vector3 scale = new Vector3(1f, 1f, 1f);

                    GameObject projectile = InternalCalls.CreatePrefab("Library/Prefabs/1225675544.prefab", pos, Quaternion.identity, scale);
                    RancorProjectile projectileScript = null;
                    if (projectile != null)
                    {
                        projectileScript = projectile.GetComponent<RancorProjectile>();

                        if (projectileScript != null)
                        {
                            projectileScript.damage = (int)(projectileScript.damage * damageMult);
                        }

                        RancorProjectile rancorParticles = projectile.GetComponent<RancorProjectile>();

                        if (rancorParticles != null)
                            rancorParticles.targetPos = Core.instance.gameObject.transform.globalPosition - new Vector3(-3f, -1f, 0f);
                    }

                    projectile = InternalCalls.CreatePrefab("Library/Prefabs/1225675544.prefab", pos, Quaternion.identity, scale);

                    if (projectile != null)
                    {
                        projectileScript = projectile.GetComponent<RancorProjectile>();

                        if (projectileScript != null)
                        {
                            projectileScript.damage = (int)(projectileScript.damage * damageMult);
                        }

                        RancorProjectile rancorParticles = projectile.GetComponent<RancorProjectile>();

                        if (rancorParticles != null)
                            rancorParticles.targetPos = Core.instance.gameObject.transform.globalPosition - new Vector3(3f, -1f, 0f);
                    }
                    projectile = InternalCalls.CreatePrefab("Library/Prefabs/1225675544.prefab", pos, Quaternion.identity, scale);

                    if (projectile != null)
                    {
                        projectileScript = projectile.GetComponent<RancorProjectile>();

                        if (projectileScript != null)
                        {
                            projectileScript.damage = (int)(projectileScript.damage * damageMult);
                        }

                        RancorProjectile rancorParticles = projectile.GetComponent<RancorProjectile>();

                        if (rancorParticles != null)
                            rancorParticles.targetPos = Core.instance.gameObject.transform.globalPosition - new Vector3(0f, -1f, 0f);
                    }
                }
            }
        }

        //Debug.Log("Projectile");
        UpdateAnimationSpd(speedMult);
    }

    private void EndProjectile()
    {
        Audio.StopAudio(gameObject);
    }

    #endregion

    #region HAND_SLAM

    private void StartHandSlam()
    {
        handSlamTimer = handSlamTime;
        Animator.Play(gameObject, "RN_HandSlam", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Rancor_Hand_Slam");

        handSlamTimerToActivate = 0.0f;

        if (handSlamHitBox != null)
        {
            RancorHandSlamHitCollider handSlamColl = handSlamHitBox.GetComponent<RancorHandSlamHitCollider>();

            if (handSlamColl != null)
            {
                handSlamColl.RestartCollider();
                handSlamColl.myDeltaTimeMult = speedMult;
            }
        }
        activateWave = true;

    }


    private void UpdateHandSlam()
    {

        handSlamTimerToActivate += myDeltaTime;
        if (handSlamTimerToActivate < 0.5f)
        {
            LookAt(Core.instance.gameObject.transform.globalPosition);
        }
        if (handSlamTimerToActivate > handSlamTimeToActivate)
        {
            if (handSlamHitBox == null)
                return;
            //Actiavate collider
            handSlamHitBox.Enable(true); //Enable collider not gameobject
            //Temporal while colliders can't set active
            //handSlamHitBox.transform.localPosition = handSlamHitBoxPos;
            //handSlamHitBox.transform.localRotation = Quaternion.RotateAroundAxis(Vector3.up, 3.14159f);
            //----

        }

        if (handSlamTimer < 0.9f)
        {
            if (handSlamHitBox == null)
                return;
            handSlamHitBox.Enable(false);


        }

        if (handSlamTimer < 0.9f)
        {
            if (activateWave)
            {
                RancorHandSlamWave handSlamWave = InternalCalls.CreatePrefab("Library/Prefabs/1923485827.prefab", gameObject.transform.localPosition, gameObject.transform.localRotation, new Vector3(0.767f, 0.225f, 1.152f)).GetComponent<RancorHandSlamWave>();

                if (handSlamWave != null)
                {
                    handSlamWave.damage = (int)(handSlamWave.damage * damageMult);
                }

                Input.PlayHaptic(.8f, 350);
                PlayParticles(PARTICLES.HANDSLAM);
                activateWave = false;
            }
        }

        float prevAnimationSpd = currAnimationPlaySpd;
        UpdateAnimationSpd(speedMult);

        if (currAnimationPlaySpd != prevAnimationSpd && handSlamHitBox != null)
        {
            RancorHandSlamHitCollider handSlamColl = handSlamHitBox.GetComponent<RancorHandSlamHitCollider>();

            if (handSlamColl != null)
            {
                handSlamColl.myDeltaTimeMult = speedMult;
            }
        }

        //Debug.Log("Hand slam");
    }


    private void EndHandSlam()
    {
        Audio.StopAudio(gameObject);

    }

    #endregion

    #region LOADING_RUSH
    private void StartLoadingRush()
    {
        loadRushTimer = loadRushTime;

        //Animator.Play(gameObject, "RN_LoadingRush"); //THIS ANIMATION DOESNT EXIST, BUT ITS NEEDED

    }
    private void UpdateLoadingRush()
    {
        //Debug.Log("Loading Rush");

        LookAt(Core.instance.gameObject.transform.globalPosition); //RANCOR POINTING PLAYER WHILE LOADING
    }
    #endregion

    #region RUSH

    private void StartRush()
    {
        rushTimer = rushTime;
        Animator.Play(gameObject, "RN_Rush", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Rancor_Hand_Slam");
        targetDirection = Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition; //RANCOR CALCULATES RUSH DIRECTION
        startRush = true;
        PlayParticles(PARTICLES.RUSH);
    }


    private void UpdateRush()
    {
        //Debug.Log("Rush");

        if (rushTimer < 2.9f)
        {
            if (startRush)
            {
                Audio.PlayAudio(gameObject, "Play_Rancor_Rush_Steps");
                startRush = false;
            }

            //TODO: Maybe this should go with Time.deltaTime?

            gameObject.transform.localPosition += targetDirection.normalized * rushSpeed * myDeltaTime; //ADD SPEED IN RUSH DIRECTION
        }

        UpdateAnimationSpd(speedMult);
    }


    private void EndRush()
    {
        Audio.StopAudio(gameObject);
    }


    private void StartRushStun()
    {
        rushRecoveryTimer = rushRecoveryTime;
        rushStunTimer = rushStunDuration;
        Animator.Play(gameObject, "RN_RushRecover", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Rancor_Recovery");

        Shake3D shake = camera.GetComponent<Shake3D>();
        if (shake != null)
        {
            shake.StartShaking(1f, 0.1f);
        }

    }

    private void UpdateRushStun()
    {
       // Debug.Log("Rush Stun");
       // Debug.Log(rushStunTimer.ToString());

        UpdateAnimationSpd(speedMult);
    }


    private void EndRushStun()
    {
        Audio.StopAudio(gameObject);
        PlayParticles(PARTICLES.RUSH);
    }

    #endregion

    #region ROAR
    private void StartRoar()
    {
        Animator.Play(gameObject, "RN_Roar", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Rancor_Roar_Presentation");
        Input.PlayHaptic(0.9f, (int)roarTime * 1000);
        roarTimer = roarTime;
    }

    private void UpdateRoar()
    {
        Shake3D shake = camera.GetComponent<Shake3D>();
        if (shake != null && roarTimer <= 2.5 && !roarShaked)
        {
            shake.StartShaking(2f, 0.1f);
            roarShaked = true;
            PlayParticles(PARTICLES.ROAR);
        }

        UpdateAnimationSpd(speedMult);
    }

    private void EndRoar()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region DIE
    private void StartDie()
    {
        dieTimer = dieTime;

        Counter.SumToCounterType(Counter.CounterTypes.RANCOR);
        Animator.Play(gameObject, "RN_Die", 1.0f);
        UpdateAnimationSpd(1f);

        Audio.PlayAudio(gameObject, "Play_Rancor_Death");
        Audio.PlayAudio(gameObject, "Play_Victory_Music");

        if (hitParticles != null)
        {
            ParticleSystem hitParticle = hitParticles.GetComponent<ParticleSystem>();

            if (hitParticle != null)
                hitParticle.Play();

        }

        EnemyManager.RemoveEnemy(gameObject);
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
       // Debug.Log("RANCOR'S DEAD");

        Animator.Pause(gameObject);
        Audio.StopAudio(gameObject);
        Input.PlayHaptic(0.3f, 3);
        InternalCalls.Destroy(gameObject);
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
            if (currentState != RANCOR_STATE.DEAD)
            {
                float damageToBoss = 0f;

                if (Core.instance != null)
                {
                    if (Core.instance.HasStatus(STATUS_TYPE.MANDO_QUICK_DRAW))
                        AddStatus(STATUS_TYPE.BLASTER_VULN, STATUS_APPLY_TYPE.ADDITIVE, Core.instance.GetStatusData(STATUS_TYPE.MANDO_QUICK_DRAW).severity / 100, 5);
                    if (Core.instance.HasStatus(STATUS_TYPE.PRIM_MOV_SPEED))
                        Core.instance.AddStatus(STATUS_TYPE.ACCELERATED, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, Core.instance.GetStatusData(STATUS_TYPE.PRIM_MOV_SPEED).severity / 100, 5, false);
                }
                BH_Bullet bulletScript = collidedGameObject.GetComponent<BH_Bullet>();

                if (bulletScript != null)
                {
                    damageToBoss += bulletScript.GetDamage();
                }
                else
                {
                    Debug.Log("The collider with tag Bullet didn't have a bullet Script!!");
                }
                if (Core.instance != null)
                    damageToBoss *= Core.instance.DamageToBosses * BlasterVulnerability;
                //if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION_INCREASE_DAMAGE_TO_BOSS))
                //{
                //    damageToBoss *= (1.0f + Skill_Tree_Data.GetMandoSkillTree().A6_increaseDamageToBossAmount);
                //}

                TakeDamage(damageToBoss * damageRecieveMult);
                //Debug.Log("Rancor HP: " + healthPoints.ToString());
                damaged = 1.0f;
                //CHANGE FOR APPROPIATE RANCOR HIT
                Audio.PlayAudio(gameObject, "Play_Rancor_Hit");

                if (Core.instance.hud != null && currentState != RANCOR_STATE.DEAD)
                {
                    HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                    if (hudComponent != null)
                    {
                        hudComponent.AddToCombo(25, 0.95f);
                    }
                }

                if (currentState != RANCOR_STATE.DEAD && healthPoints <= 0.0f)
                {
                    inputsList.Add(RANCOR_INPUT.IN_DEAD);
                }
            }

        }
        else if (collidedGameObject.CompareTag("ChargeBullet"))
        {
            if (currentState != RANCOR_STATE.DEAD)
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

                    damageToBoss += bulletScript.GetDamage() * damageMult;

                }
                else
                {
                    Debug.Log("The collider with tag Bullet didn't have a bullet Script!!");
                }

                //if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION_INCREASE_DAMAGE_TO_BOSS))
                //{
                //    damageToBoss *= (1.0f + Skill_Tree_Data.GetMandoSkillTree().A6_increaseDamageToBossAmount);
                //}

                if (Core.instance != null)
                    damageToBoss *= Core.instance.DamageToBosses;
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
                //Debug.Log("Rancor HP: " + healthPoints.ToString());
                damaged = 1.0f;
                //CHANGE FOR APPROPIATE RANCOR HIT
                Audio.PlayAudio(gameObject, "Play_Rancor_Hit");

                if (Core.instance.hud != null && currentState != RANCOR_STATE.DEAD)
                {
                    HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                    if (hudComponent != null)
                    {
                        hudComponent.AddToCombo(55, 0.25f);
                    }
                }

                if (currentState != RANCOR_STATE.DEAD && healthPoints <= 0.0f)
                {
                    inputsList.Add(RANCOR_INPUT.IN_DEAD);
                }
            }
        }
        else if (collidedGameObject.CompareTag("WorldLimit"))
        {
            if (currentState != RANCOR_STATE.DEAD)
            {
                inputsList.Add(RANCOR_INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("Wall"))
        {
            if (currentState == RANCOR_STATE.RUSH)
                inputsList.Add(RANCOR_INPUT.IN_RUSH_STUN);
        }
        else if (collidedGameObject.CompareTag("Player"))
        {
            if (currentState == RANCOR_STATE.DEAD) return;

            float damageToPlayer = touchDamage;

            if (currentState == RANCOR_STATE.RUSH)
            {
                damageToPlayer = rushDamage;
                inputsList.Add(RANCOR_INPUT.IN_RUSH_END);
            }

            PlayerHealth playerHealth = collidedGameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage((int)(damageToPlayer * damageMult));

        }
        else if (collidedGameObject.CompareTag("WallSkill"))
        {
            if (currentState == RANCOR_STATE.RUSH)
                inputsList.Add(RANCOR_INPUT.IN_RUSH_STUN);
        }
    }

    private void PlayParticles(PARTICLES particleType)
    {
        if (rancorParticles == null)
        {
            //Debug.Log("Rancor Particles not found!");
            return;
        }
        ParticleSystem particle = null;
        switch (particleType)
        {
            case PARTICLES.NONE:
                break;
            case PARTICLES.TRAILLEFT:
                particle = rancorParticles.trailLeft;
                if (particle != null)
                    particle.Play();
                else
                    Debug.Log("Rancor Trail Left particle not found!");
                break;
            case PARTICLES.TRAILRIGHT:
                particle = rancorParticles.trailRight;
                if (particle != null)
                    particle.Play();
                else
                    Debug.Log("Rancor Trail Left particle not found!");
                break;
            case PARTICLES.IMPACT:
                particle = rancorParticles.impact;
                if (particle != null)
                    particle.Play();
                else
                    Debug.Log("Rancor Impact particle not found!");
                break;
            case PARTICLES.RUSH:
                particle = rancorParticles.rush;
                if (particle != null && !particle.playing)
                    particle.Play();
                else if (particle != null && particle.playing)
                    particle.Stop();
                else
                    Debug.Log("Rancor Rush particle not found!");
                break;
            case PARTICLES.SWINGLEFT:
                particle = rancorParticles.swingLeft;
                if (particle != null)
                    particle.Play();
                else
                    Debug.Log("Rancor Swing Left particle not found!");
                break;
            case PARTICLES.SWINGRIGHT:
                particle = rancorParticles.swingRight;
                if (particle != null)
                    particle.Play();
                else
                    Debug.Log("Rancor Swing Right particle not found!");
                break;
            case PARTICLES.HANDSLAM:
                particle = rancorParticles.handSlam;
                if (particle != null)
                    particle.Play();
                else
                    Debug.Log("Rancor Hand Slam particle not found!");
                break;
            case PARTICLES.ROAR:
                particle = rancorParticles.roar;
                if (particle != null)
                    particle.Play();
                else
                    Debug.Log("Rancor Roar particle not found!");
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        if (!DebugOptionsHolder.bossDmg)
        {
            //Debug.Log("Rancor damage" + damage.ToString());
            if (currentState != RANCOR_STATE.DEAD)
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
                    inputsList.Add(RANCOR_INPUT.IN_DEAD);
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

    public override bool IsDying()
    {
        return currentState == RANCOR_STATE.DEAD;
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
}