using System;
using System.Collections.Generic;
using DiamondEngine;

public class Bosseslv2 : Entity
{
    public NavMeshAgent agent = null;

    public Random randomNum = new Random();

    public GameObject camera = null;

    public float slerpSpeed = 5.0f;

    //Public Variables
    public float probFollow = 60.0f;
    public float probWander = 40.0f;
    public GameObject projectilePoint = null;
    public Vector3 targetPos = new Vector3(0, 0, 0);
    public float distanceProjectile = 15.0f;
    public float wanderRange = 7.5f;
    public GameObject colliderJumpSlam = null;
    public GameObject colliderBounceRush = null;
    public GameObject colliderRush = null;
    public GameObject jumpPositionIndicator = null;
    public GameObject bossHealth = null;
    public Material healthbarMaterial = null;
    public float limboHealth = 0.0f;
    public float maxHealthPoints = 0.0f;
    public GameObject leftSpike = null;
    public GameObject rightSpike = null;
    public float angleDispersion = 15f;


    //Private Variables
    public bool resting = false;
    public bool firstShot = true;
    public bool secondShot = false;
    private bool catchingProjectile = false;
    private float angle = 10f;

    //Stats
    public float healthPoints = 1920.0f;
    public float speed = 3.5f;
    public float fastRushSpeed = 14.0f;
    public float slowRushSpeed = 7.0f;
    protected float damageMult = 1f;
    public float damageRecieveMult { get; protected set; } = 1f;

    //Timers
    public float walkingTime = 4.0f;
    public float walkingTimer = 0.0f;
    public float fastChasingTime = 0.5f;
    public float fastChasingTimer = 0.0f;
    public float slowChasingTime = 3.5f;
    public float slowChasingTimer = 0.0f;
    public float shootingTime = 0.5f;
    public float shootingTimer = 0.0f;
    public float dieTime = 2.0f;
    public float dieTimer = 0.0f;
    public float restingTime = 3.0f;
    public float restingTimer = 0.0f;
    public float bounceRushTime = 4.0f;
    public float bounceRushTimer = 0.0f;
    private float currAnimationPlaySpd = 1f;
    public float presentationTimer = 0f;
    public float presentationTime = 1f;
    private float catchProjectileTimer = 0f;
    private float ballTimer = 0f;
    private float ballTime = 0.5f;
    public float trailTimer = 0.0f;
    public float trailTime = 0.0f;

    //Atacks
    public float projectileAngle = 30.0f;
    public float projectileRange = 6.0f;
    public float projectileDamage = 10.0f;
    public float rushDamage = 15.0f;
    public float damageBounceRush = 20f;
    private bool wampaRushCharged = false;
    private float wampaRushChargeCounter = 0.0f;
    List<GameObject> projectiles = new List<GameObject>();

    //Jump Slam
    private JUMPSLAM jumpslam = JUMPSLAM.NONE;
    private float jumpslamTimer = 0.0f;
    private float chargeTime = 1f;
    private float upTime = 0.3f;
    private float fallingTime = 1.0f;
    private float recoveryTime = 0.73f;
    public float totalJumpSlamTimer = 0.0f;
    public float totalJumpSlamTime = 3.03f;

    //Bounce Rush
    public GameObject initTarget;
    private GameObject finalTarget;
    private GameObject currentTarget;
    private bool returnToInitTarget = false;
    private float startBounceRushTime = 0.0f;
    public bool finalBounce = false;

    private bool firstThrow = true;
    public GameObject companion = null;
    private bool rushOnce = true;

    private bool jumpOnce = true;

    public GameObject trailParticlesObj = null;
    protected ParticleSystem trailParticles = null;
    public GameObject slamParticlesObj = null;
    protected ParticleSystem slamParticles = null;
    public GameObject impactParticlesObj = null;
    protected ParticleSystem impactParticles = null;
    public GameObject ballParticlesObj = null;
    protected ParticleSystem ballParticles = null;

    enum JUMPSLAM : int
    {
        NONE = -1,
        CHARGE,
        UP,
        FALLING,
        RECOVERY
    }

    protected override void InitEntity(ENTITY_TYPE myType)
    {
        eType = myType;
        speedMult = 1f;
        myDeltaTime = Time.deltaTime;
        damageMult = 1f;
        damageRecieveMult = 1f;
    }

    public virtual void Awake()
    {
        Debug.Log("Health 2 Wampa " + healthPoints.ToString());
        maxHealthPoints = healthPoints;
        jumpOnce = true;

        if (gameObject.CompareTag("Skel"))
        {
            startBounceRushTime = Animator.GetAnimationDuration(gameObject, "Skel_RushStart") - 0.016f;
            chargeTime = Animator.GetAnimationDuration(gameObject, "Skel_Jump_P1") - 0.05f;
            Debug.Log("Rush Start: " + startBounceRushTime.ToString());
            companion = InternalCalls.FindObjectWithName("WampaBoss");
            presentationTime = 2 * (Animator.GetAnimationDuration(gameObject, "Skel_Roar") - 0.016f);
        }
        else if (gameObject.CompareTag("Wampa"))
        {
            presentationTime = Animator.GetAnimationDuration(gameObject, "WP_Roar") - 0.016f;
            colliderRush = gameObject.GetChild("Rush");
        }
        rushOnce = true;
        angle = angleDispersion;

        if (trailParticlesObj != null)
        {
            trailParticles = trailParticlesObj.GetComponent<ParticleSystem>();
        }

        if (ballParticlesObj != null)
        {
            ballParticles = ballParticlesObj.GetComponent<ParticleSystem>();
        }

        if (impactParticlesObj != null)
        {
            impactParticles = impactParticlesObj.GetComponent<ParticleSystem>();
        }

        if (slamParticlesObj != null)
        {
            slamParticles = slamParticlesObj.GetComponent<ParticleSystem>();
        }

        if (bossHealth != null)
        {
            healthbarMaterial = bossHealth.GetComponent<Material>();
        }
    }

    #region PROJECTILE
    public void StartProjectile()
    {
        Debug.Log("Starting Throwing Projectile");
        catchProjectileTimer = 0.7f;
        shootingTimer = shootingTime;
        firstShot = true;
        Animator.Play(gameObject, "WP_Projectile", speedMult);
        UpdateAnimationSpd(speedMult);
        firstThrow = true;
    }
    public void UpdateProjectile()
    {
        LookAt(Core.instance.gameObject.transform.globalPosition);

        if (catchProjectileTimer > 0f)
        {
            catchProjectileTimer -= myDeltaTime;
            if (catchProjectileTimer < 0f)
            {
                if (leftSpike != null)
                    leftSpike.Enable(true);

                if (rightSpike != null)
                    rightSpike.Enable(true);

                catchingProjectile = true;
            }
        }

        if (!catchingProjectile) return;

        if (shootingTimer > 0.0f)
        {
            shootingTimer -= myDeltaTime;
            if (shootingTimer < 0.0f)
            {
                if (projectilePoint != null)
                {
                    if (firstThrow)
                        Audio.PlayAudio(gameObject, "Play_Wampa_Projectile_Throw_1");
                    else
                        Audio.PlayAudio(gameObject, "Play_Wampa_Projectile_Throw_2");
                    firstThrow = false;

                    float angleIncrement = angleDispersion / 2;
                    float currentAngle = -(angleDispersion * 0.5f);

                    projectiles.Add(InternalCalls.CreatePrefab("Library/Prefabs/1547931276.prefab", projectilePoint.transform.globalPosition, projectilePoint.transform.globalRotation * Quaternion.RotateAroundAxis(Vector3.up, -90 * Mathf.Deg2RRad), null));
                    projectiles.Add(InternalCalls.CreatePrefab("Library/Prefabs/1547931276.prefab", projectilePoint.transform.globalPosition, projectilePoint.transform.globalRotation * Quaternion.RotateAroundAxis(Vector3.up, -90 * Mathf.Deg2RRad), null));
                    projectiles.Add(InternalCalls.CreatePrefab("Library/Prefabs/1547931276.prefab", projectilePoint.transform.globalPosition, projectilePoint.transform.globalRotation * Quaternion.RotateAroundAxis(Vector3.up, -90 * Mathf.Deg2RRad), null));

                    if (projectiles.Count != 0)
                    {
                        for (int i = 0; i < projectiles.Count; ++i)
                        {
                            if (projectiles[i] != null)
                            {
                                projectiles[i].transform.localRotation *= Quaternion.RotateAroundAxis(Vector3.up, currentAngle * Mathf.Deg2RRad);
                            }
                            currentAngle += angleIncrement;
                        }
                    }
                    projectiles.Clear();

                    if (firstShot)
                    {
                        shootingTimer = shootingTime;
                        firstShot = false;
                        UpdateAnimationSpd(speedMult);

                        if (rightSpike != null)
                            rightSpike.Enable(false);
                    }
                    else
                    {
                        if (leftSpike != null)
                            leftSpike.Enable(false);

                        secondShot = true;
                        catchingProjectile = false;
                    }
                    Audio.PlayAudio(gameObject, "Play_Wampa_Ice_Pick");
                }
            }
        }
        if (projectilePoint == null) Debug.Log("Prohjectile null");

        UpdateAnimationSpd(speedMult);
    }

    public void EndProjectile()
    {
        firstShot = true;
        secondShot = false;
        resting = true;
        restingTimer = restingTime;
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region RUSH
    public void StartFastRush()
    {
        wampaRushCharged = false;
        wampaRushChargeCounter = 0.0f;
        fastChasingTimer = fastChasingTime;
        Debug.Log("Fast Rush");
        Animator.Play(gameObject, "WP_Rush", speedMult);
        UpdateAnimationSpd(speedMult);

        //Audio.PlayAudio(gameObject, "Play_Wampa_Rush");
        Animator.Play(gameObject, "WP_Roar", speedMult * 2);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Wampa_Roar_Presentation");

        if (colliderRush != null)
        {
            colliderRush.GetComponent<AtackBosslv2>().active = true;
        }
    }
    public void UpdateFastRush()
    {
        if (!wampaRushCharged)
        {
            wampaRushChargeCounter += Time.deltaTime;
            if (wampaRushChargeCounter >= 0.49f)
            {
                if (trailParticles != null)
                    trailParticles.Play();
                wampaRushCharged = true;
            }
        }
        else
        {
            agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
            LookAt(agent.GetDestination());
            agent.MoveToCalculatedPos(fastRushSpeed * speedMult);
            //Debug.Log("Rush");

            UpdateAnimationSpd(speedMult);
            if (gameObject.CompareTag("Skel") && rushOnce)
            {
                Audio.PlayAudio(gameObject, "Play_Skel_In_Progress");
                rushOnce = false;
            }
        }
    }

    public void EndFastRush()
    {
        if (gameObject.CompareTag("Wampa"))
            Audio.StopOneAudio(gameObject, "Play_Wampa_Rush");
        else if (gameObject.CompareTag("Skel"))
        {
            Audio.StopAudio(gameObject);
            Audio.PlayAudio(gameObject, "Play_Skel_Rush_Recovery");
        }
        rushOnce = true;
        colliderBounceRush.GetComponent<AtackBosslv2>().active = false;
    }

    public void StartSlowRush()
    {
        slowChasingTimer = slowChasingTime;
        Debug.Log("Slow Rush");
        Animator.Play(gameObject, "WP_Rush", speedMult);
        UpdateAnimationSpd(speedMult);
        if (gameObject.CompareTag("Wampa"))
            Audio.PlayAudio(gameObject, "Play_Wampa_Rush");
        else if (gameObject.CompareTag("Skel"))
            Audio.PlayAudio(gameObject, "Play_Skel_Preparation");
    }
    public void UpdateSlowRush()
    {
        //Debug.Log("Slow Rush");
        agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(slowRushSpeed * speedMult);

        UpdateAnimationSpd(speedMult);
        if (gameObject.CompareTag("Skel") && rushOnce)
        {
            Audio.PlayAudio(gameObject, "Play_Skel_In_Progress");
            rushOnce = false;
        }
    }

    public void EndSlowRush()
    {
        resting = true;
        restingTimer = restingTime;

        if (colliderRush != null)
        {
            colliderRush.GetComponent<AtackBosslv2>().active = false;
        }

        if (gameObject.CompareTag("Wampa"))
            Audio.StopOneAudio(gameObject, "Play_Wampa_Rush");
        else if (gameObject.CompareTag("Skel"))
        {
            Audio.StopAudio(gameObject);
            Audio.PlayAudio(gameObject, "Play_Skel_Rush_Recovery");
        }
        rushOnce = true;

        if (trailParticles != null)
            trailParticles.Stop();
    }
    #endregion

    #region BOUNCE RUSH

    public void StartBounceRush()
    {
        Audio.PlayAudio(gameObject, "Play_Skel_Preparation");
        Animator.Play(gameObject, "Skel_Rush_Start", speedMult);
        UpdateAnimationSpd(speedMult);
        bounceRushTimer = bounceRushTime;
        GameObject nearestSkelColumn = Level2BossRoom.columns[0];
        GameObject nearestPlayerColumn = Level2BossRoom.columns[0];
        float nerestDistance = 10000f;
        foreach (GameObject column in Level2BossRoom.columns)
        {
            if (column == null) continue;
            float distance = Mathf.Distance(gameObject.transform.globalPosition, column.transform.globalPosition);
            if (nerestDistance > distance)
            {
                nerestDistance = distance;
                nearestSkelColumn = column;
            }
        }

        nerestDistance = 10000f;
        foreach (GameObject column in Level2BossRoom.columns)
        {
            if (column == null) continue;
            float distance = Mathf.Distance(Core.instance.gameObject.transform.globalPosition, column.transform.globalPosition);
            if (nerestDistance > distance)
            {
                nerestDistance = distance;
                nearestPlayerColumn = column;
            }
        }

        initTarget = nearestSkelColumn;
        finalTarget = nearestPlayerColumn;//.GetComponent<TargetColumn>().GetTarget(gameObject.transform.globalPosition);
        currentTarget = initTarget;
        returnToInitTarget = false;

        if (colliderBounceRush != null)
        {
            colliderBounceRush.GetComponent<AtackBosslv2>().active = true;
        }
        ballTimer = ballTime;
    }

    public void UpdateBounceRush()
    {
        if (ballTimer > 0f)
        {
            ballTimer -= myDeltaTime;
            if (ballTimer <= 0f)
            {
                Animator.Play(gameObject, "Skel_Rush");
                UpdateAnimationSpd(speedMult);

            }
        }

        float distance = Mathf.Distance(gameObject.transform.globalPosition, currentTarget.transform.globalPosition);
        if (distance > 2f)
        {
            MoveToPosition(currentTarget.transform.globalPosition, 20f);
            LookAt(currentTarget.transform.globalPosition);
            if (currentTarget == finalTarget)
            {
                returnToInitTarget = true;
                if (ballParticles != null)
                    ballParticles.Play();
            }
        }
        else if (currentTarget != finalTarget && !returnToInitTarget)
        {
            currentTarget = finalTarget;
            if (ballParticles != null)
                ballParticles.Play();
        }
        else if (currentTarget == finalTarget && returnToInitTarget)
        {
            currentTarget = initTarget;
            finalBounce = true;
            if (ballParticles != null)
                ballParticles.Play();
        }

        UpdateAnimationSpd(speedMult);
        if (gameObject.CompareTag("Skel") && rushOnce)
        {
            Audio.PlayAudio(gameObject, "Play_Skel_Rush_In_Progress");
            rushOnce = false;
        }
    }

    public void EndBounceRush()
    {
        Debug.Log("END BOUNCE RUSH");
        resting = true;
        restingTimer = restingTime;

        Animator.Play(gameObject, "Skel_Rush_Recover");
        UpdateAnimationSpd(speedMult);

        if (colliderBounceRush != null)
        {
            colliderBounceRush.GetComponent<AtackBosslv2>().active = false;
        }
        Audio.StopAudio(gameObject);
        Audio.PlayAudio(gameObject, "Play_Skel_Rush_Recovery");
        rushOnce = true;
    }

    #endregion

    #region JUMP SLAM

    public void StartJumpSlam()
    {
        Debug.Log("Starting Jumping");
        jumpslam = JUMPSLAM.CHARGE;
        jumpslamTimer = chargeTime;
        totalJumpSlamTimer = totalJumpSlamTime;
        if (gameObject.CompareTag("Skel"))
        {
            Animator.Play(gameObject, "Skel_Jump_P1", speedMult);
            UpdateAnimationSpd(speedMult);
        }
    }

    public void UpdateJumpSlam()
    {
        LookAt(Core.instance.gameObject.transform.globalPosition);
        //Debug.Log("Jump Slam");
        switch (jumpslam)
        {
            case JUMPSLAM.CHARGE:
                //Debug.Log("Jump Slam: Charge");
                if (jumpslamTimer > 0)
                {
                    if (jumpOnce)
                    {
                        Audio.PlayAudio(gameObject, "Play_Skel_Jump_Preparation");
                        jumpOnce = false;
                    }
                    jumpslamTimer -= myDeltaTime;

                    if (jumpslamTimer <= 0)
                    {
                        jumpslamTimer = upTime;
                        jumpslam = JUMPSLAM.UP;

                        if (gameObject.CompareTag("Skel"))
                        {
                            Animator.Play(gameObject, "Skel_Jump_P2", speedMult);
                            UpdateAnimationSpd(speedMult);
                            Audio.StopAudio(gameObject);
                            Audio.PlayAudio(gameObject, "Play_Skel_Jump_In_Progress");
                            if (jumpPositionIndicator != null)
                            {
                                jumpPositionIndicator.Enable(true);
                                jumpPositionIndicator.transform.localPosition = Core.instance.gameObject.transform.globalPosition;
                            }

                        }
                        else if (gameObject.CompareTag("Wampa"))
                        {

                        }

                    }
                }

                //Audio.PlayAudio(gameObject, "Play_Wampa_Skell_Jump_Snow");
                break;

            case JUMPSLAM.UP:
                //Debug.Log("Jump Slam: Up");
                if (jumpslamTimer > 0)
                {
                    gameObject.transform.localPosition += Vector3.up * 50f * myDeltaTime;
                    Vector3 positionVec = (Core.instance.gameObject.transform.globalPosition - gameObject.transform.localPosition) * myDeltaTime;
                    gameObject.transform.localPosition += new Vector3(positionVec.x, 0.0f, positionVec.z);

                    jumpslamTimer -= myDeltaTime;

                    if (Mathf.Distance(jumpPositionIndicator.transform.localPosition, Core.instance.gameObject.transform.globalPosition) > 0.3f)
                    {
                        jumpPositionIndicator.transform.localPosition += (Core.instance.gameObject.transform.globalPosition - jumpPositionIndicator.transform.localPosition) * 0.1f;
                    }

                    if (jumpslamTimer <= 0)
                    {
                        jumpslamTimer = fallingTime;
                        jumpslam = JUMPSLAM.FALLING;
                        jumpPositionIndicator.transform.localPosition = targetPos = Core.instance.gameObject.transform.globalPosition;

                        float speed = Mathf.Distance(targetPos, gameObject.transform.globalPosition) / fallingTime;
                        if (colliderJumpSlam != null)
                        {
                            colliderJumpSlam.GetComponent<AtackBosslv2>().active = true;
                        }
                    }
                }
                break;

            case JUMPSLAM.FALLING:
                //Debug.Log("Jump Slam: Falling");
                if (jumpslamTimer > 0)
                {
                    MoveToPosition(targetPos, speed * 10);
                    jumpslamTimer -= myDeltaTime;

                    if (jumpslamTimer <= 0 || Mathf.Distance(targetPos, gameObject.transform.globalPosition) <= 0.5f)
                    {
                        jumpslamTimer = recoveryTime;
                        jumpslam = JUMPSLAM.RECOVERY;

                        if (gameObject.CompareTag("Skel"))
                        {
                            Animator.Play(gameObject, "Skel_Jump_P3", speedMult);
                            UpdateAnimationSpd(speedMult);
                            jumpOnce = true;
                            Audio.StopAudio(gameObject);
                            Audio.PlayAudio(gameObject, "Play_Skel_Jump_Impact");
                            if (impactParticles != null)
                            {
                                impactParticles.Play();
                            }

                            if (slamParticles != null)
                                slamParticles.Play();
                        }
                    }
                }
                //Audio.PlayAudio(gameObject, "Play_Wampa_Skell_Jump_Impact");
                break;

            case JUMPSLAM.RECOVERY:
                //Debug.Log("Jump Slam: Recovery");
                if (jumpslamTimer > 0)
                {
                    jumpslamTimer -= myDeltaTime;

                    if (jumpslamTimer <= 0)
                    {
                        jumpslam = JUMPSLAM.NONE;
                    }
                }
                break;

            case JUMPSLAM.NONE:
            default:
                Debug.Log("Jump slam motion ended");
                break;
        }

        UpdateAnimationSpd(speedMult);
    }

    public void EndJumpSlam()
    {
        resting = true;
        restingTimer = restingTime;

        if (jumpPositionIndicator != null)
        {
            jumpPositionIndicator.Enable(false);
        }

        if (colliderJumpSlam != null)
        {
            colliderJumpSlam.GetComponent<AtackBosslv2>().active = false;
        }
        Audio.StopAudio(gameObject);
    }

    #endregion

    #region FOLLOW
    public void StartFollowing()
    {
        walkingTimer = walkingTime;
        if (gameObject.CompareTag("Skel"))
        {
            Animator.Play(gameObject, "Skel_Walk", speedMult);
            UpdateAnimationSpd(speedMult);
            Audio.PlayAudio(gameObject, "Play_Skel_Footsteps");
        }
        else if (gameObject.CompareTag("Wampa"))
        {
            Animator.Play(gameObject, "WP_Walk", speedMult);
            UpdateAnimationSpd(speedMult);
            Audio.PlayAudio(gameObject, "Play_Wampa_Footsteps");
        }

        trailTimer = trailTime;
    }
    public void UpdateFollowing()
    {
        if (trailTimer > 0.0f)
        {
            trailTimer -= Time.deltaTime;

            if (trailTimer <= 0.0f)
            {
                if (trailParticles != null)
                {
                    trailParticles.Play();
                }
                trailTimer = trailTime;
            }
        }

        agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(speed * speedMult);
        //Debug.Log("Following player");

        UpdateAnimationSpd(speedMult);
    }

    public void EndFollowing()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region WANDER
    public void StartWander()
    {
        walkingTimer = walkingTime;
        agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);
        if (gameObject.CompareTag("Skel"))
        {
            Animator.Play(gameObject, "Skel_Walk", speedMult);
            UpdateAnimationSpd(speedMult);
            Audio.PlayAudio(gameObject, "Play_Skel_Footsteps");
        }
        else if (gameObject.CompareTag("Wampa"))
        {

            Animator.Play(gameObject, "WP_Walk", speedMult);
            UpdateAnimationSpd(speedMult);
            Audio.PlayAudio(gameObject, "Play_Wampa_Footsteps");
        }

        trailTimer = trailTime;
    }
    public void UpdateWander()
    {
        if (trailTimer > 0.0f)
        {
            trailTimer -= Time.deltaTime;

            if (trailTimer <= 0.0f)
            {
                if (trailParticles != null)
                {
                    trailParticles.Play();
                }
                trailTimer = trailTime;
            }
        }

        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(speed * speedMult);
        //Debug.Log("Following player");

        UpdateAnimationSpd(speedMult);
    }

    public void EndWander()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region DIE
    public void StartDie()
    {
        Audio.StopAudio(gameObject);
        dieTimer = dieTime;
        if (gameObject.CompareTag("Skel"))
        {
            Animator.Play(gameObject, "Skel_Die", speedMult);
            UpdateAnimationSpd(speedMult);
            Audio.PlayAudio(gameObject, "Play_Skel_Death");
            Counter.SumToCounterType(Counter.CounterTypes.SKEL);
            if (companion.GetComponent<Wampa>().healthPoints <= 0.0f)
            {
                Audio.PlayAudio(gameObject, "Play_Victory_Music");
            }
            Debug.Log("BOOLEAN IS TRUE");
        }
        else if (gameObject.CompareTag("Wampa"))
        {
            Animator.Play(gameObject, "WP_Dead", speedMult);
            UpdateAnimationSpd(speedMult);
            Audio.PlayAudio(gameObject, "Play_Wampa_Death");
            Counter.SumToCounterType(Counter.CounterTypes.WAMPA);
            if (companion.GetComponent<Skel>().healthPoints <= 0.0f)
            {
                Audio.PlayAudio(gameObject, "Play_Victory_Music");
            }
        }

        if (jumpPositionIndicator != null && jumpPositionIndicator.IsEnabled())
        {
            jumpPositionIndicator.Enable(false);
        }

        Debug.Log("Dying");
    }
    public void UpdateDie()
    {
        if (dieTimer > 0.0f)
        {
            dieTimer -= myDeltaTime;

            if (dieTimer <= 0.0f)
            {
                EndDie();
            }
        }
        //Debug.Log("Dying");

        UpdateAnimationSpd(speedMult);
    }

    public void EndDie()
    {
        Debug.Log("DEAD");
        limboHealth = 0.0f;
        SetLifebarToZero();

        EnemyManager.RemoveEnemy(gameObject);
        if (gameObject.CompareTag("Wampa") && companion != null)
        {
            companion.GetComponent<Skel>().firstSorrowRoar = true;
        }
        else if (gameObject.CompareTag("Skel") && companion != null)
        {
            companion.GetComponent<Wampa>().firstSorrowRoar = true;
        }
        gameObject.Enable(false);
        //companion = null;
        //InternalCalls.Destroy(gameObject);
    }
    #endregion

    #region PRESENTATION
    public void StartPresentation()
    {
        presentationTimer = presentationTime;

        if (gameObject.CompareTag("Skel"))
        {
            Animator.Play(gameObject, "Skel_Roar", speedMult);
            UpdateAnimationSpd(speedMult);
            //Audio.PlayAudio(gameObject, "Play_Skel_Death");
        }
        else if (gameObject.CompareTag("Wampa"))
        {
            Animator.Play(gameObject, "WP_Roar", speedMult);
            UpdateAnimationSpd(speedMult);
            Audio.PlayAudio(gameObject, "Play_Wampa_Roar_Presentation");
        }

        if (camera != null)
        {
            Shake3D shake = camera.GetComponent<Shake3D>();
            if (shake != null)
            {
                shake.StartShaking(1.5f, 0.2f);
                Input.PlayHaptic(0.9f, 800);
            }
        }

        Debug.Log("Start Presentation");
    }
    public void UpdatePresentation()
    {
        Debug.Log("Presentation");
        LookAt(Core.instance.gameObject.transform.globalPosition);
        UpdateAnimationSpd(speedMult);
    }

    public void EndPresentation()
    {

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

    public void MoveToPosition(Vector3 positionToReach, float speed)
    {
        Vector3 direction = positionToReach - gameObject.transform.localPosition;

        gameObject.transform.localPosition += direction.normalized * speed * myDeltaTime;
    }

    private void PlayAnimation(string animName)
    {
        if (gameObject.CompareTag("Skel"))
        {
            Animator.Play(gameObject, animName, speedMult);
            UpdateAnimationSpd(speedMult);
        }
        else if (gameObject.CompareTag("Wampa"))
        {
            Animator.Play(gameObject, animName, speedMult);
            UpdateAnimationSpd(speedMult);
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

    public virtual void TakeDamage(float damage)
    { }

    public override bool IsDying()
    {
        return healthPoints <= 0f;
    }

    private void SetLifebarToZero()
    {
        if (healthbarMaterial != null)
        {
            healthbarMaterial.SetFloatUniform("length_used", 0.0f);
            healthbarMaterial.SetFloatUniform("limbo", 0.0f);
        }
        else
            Debug.Log("Boss Bar component was null!!");
    }

    /*    public void SetJumpValues(float charge, float up, float down, float recovery)
        {
            chargeTime = charge;
            upTime = up;
            fallingTime = down;
            recoveryTime = recovery;
            totalJumpSlamTime = chargeTime + upTime + fallingTime + recoveryTime;
        }*/

}