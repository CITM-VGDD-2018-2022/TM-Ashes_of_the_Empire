using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;


public class Wampa : Bosseslv2
{
    enum STATE : int
    {
        NONE = -1,
        SEARCH_STATE,
        FOLLOW,
        WANDER,
        FAST_RUSH,
        SLOW_RUSH,
        PROJECTILE,
        JUMP_SLAM,
        BOUNCE_RUSH,
        PRESENTATION,
        DEAD
    }

    enum INPUT : int
    {
        NONE = -1,
        IN_FOLLOW,
        IN_FOLLOW_END,
        IN_WANDER,
        IN_WANDER_END,
        IN_FAST_RUSH,
        IN_FAST_RUSH_END,
        IN_SLOW_RUSH,
        IN_SLOW_RUSH_END,
        IN_PROJECTILE,
        IN_PROJECTILE_END,
        IN_JUMPSLAM,
        IN_JUMPSLAM_END,
        IN_BOUNCERUSH,
        IN_BOUNCERUSH_END,
        IN_PRESENTATION,
        IN_PRESENTATION_END,
        IN_DEAD
    }

    public GameObject wampaMesh = null;
    private STATE currentState = STATE.PRESENTATION;
    private List<INPUT> inputsList = new List<INPUT>();
    public bool firstSorrowRoar = false;
    private bool firstFrame = true;
    public bool angry = false;
    float healthPointsAux = 0.0f;
    public bool rushCrash = false;
    float damaged = 0.0f;

    public override void Awake()
    {
        base.Awake();

        InitEntity(ENTITY_TYPE.WAMPA);
        EnemyManager.AddEnemy(gameObject);
        angry = false;

        agent = gameObject.GetComponent<NavMeshAgent>();

        //Animator.Play(gameObject, "");
        Audio.SetState("Game_State", "Wampa_Skel_Room");

        companion = InternalCalls.FindObjectWithName("Skel");
        limboHealth = 0f;
        damaged = 0.0f;
        if (EnvironmentSourceLocate.instance != null)
        {
            Audio.StopAudio(EnvironmentSourceLocate.instance.gameObject);
            Audio.PlayAudio(EnvironmentSourceLocate.instance.gameObject, "Play_Wampa_Skel_Ambience");
        }

    }

    public void Update()
    {
        if (firstFrame)
        {
            companion = InternalCalls.FindObjectWithName("Skel");
            firstFrame = false;
            StartPresentation();
        }
        myDeltaTime = Time.deltaTime * speedMult;
        if (angry == false)
        {
            healthPointsAux = healthPoints;
        }
        UpdateStatuses();

        ProcessInternalInput();
        ProcessExternalInput();
        ProcessState();

        UpdateState();

        if (firstSorrowRoar)
        {
            firstSorrowRoar = false;
            Audio.PlayAudio(gameObject, "Play_Wampa_When_Skel_Dies");
        }
    }

    private void ProcessInternalInput()
    {
        if (walkingTimer > 0)
        {
            walkingTimer -= myDeltaTime;

            if (walkingTimer <= 0)
            {
                if (currentState == STATE.FOLLOW)
                    inputsList.Add(INPUT.IN_FOLLOW_END);
                else if (currentState == STATE.WANDER)
                    inputsList.Add(INPUT.IN_WANDER_END);
            }
        }

        if (fastChasingTimer > 0)
        {
            fastChasingTimer -= myDeltaTime;

            if (fastChasingTimer <= 0)
            {
                inputsList.Add(INPUT.IN_FAST_RUSH_END);
            }
        }

        if (presentationTimer > 0.0f)
        {
            if (angry == false)
            {
                presentationTimer -= myDeltaTime;
                healthPoints = (1 - (presentationTimer / presentationTime)) * maxHealthPoints;

                if (presentationTimer <= 0.0f)
                {
                    inputsList.Add(INPUT.IN_PRESENTATION_END);
                    healthPoints = maxHealthPoints;
                    limboHealth = healthPoints;
                }
            }
            else
            {
                presentationTimer -= myDeltaTime;
                healthPoints = healthPointsAux + (1 - (presentationTimer / presentationTime)) * (maxHealthPointsAngry * 0.25f);

                if (presentationTimer <= 0.0f)
                {
                    inputsList.Add(INPUT.IN_PRESENTATION_END);
                    healthPoints = healthPointsAux + (maxHealthPointsAngry * 0.25f);
                    limboHealth = healthPoints;
                    if (healthPoints > maxHealthPointsAngry)
                    {
                        healthPoints = maxHealthPointsAngry;
                    }
                }
            }
        }

        if (slowChasingTimer > 0)
        {
            slowChasingTimer -= myDeltaTime;

            if (slowChasingTimer <= 0)
            {
                inputsList.Add(INPUT.IN_SLOW_RUSH_END);
            }
        }

        if (restingTimer > 0)
        {
            restingTimer -= myDeltaTime;

            if (restingTimer <= 0)
            {
                resting = false;
            }
        }

    }

    private void ProcessExternalInput()
    {
        if (currentState == STATE.WANDER && Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= 1.0f)
        {
            agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);
        }

        if (currentState == STATE.PROJECTILE && secondShot)
        {
            inputsList.Add(INPUT.IN_PROJECTILE_END);
        }

        if (!companion.IsEnabled() && !angry)
        {
            inputsList.Add(INPUT.IN_PRESENTATION);
            presentationTimer = presentationTime;
            WampaAngry();
            angry = true;
            firstSorrowRoar = true;
        }
        if (rushCrash)
        {
            if (currentState == STATE.FAST_RUSH || currentState == STATE.SLOW_RUSH)
            {
                inputsList.Add(INPUT.IN_SLOW_RUSH_END);
                Input.PlayHaptic(0.5f, 400);
            }
            rushCrash = false;
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
                    break;

                case STATE.PRESENTATION:
                    switch (input)
                    {
                        case INPUT.IN_PRESENTATION_END:
                            currentState = STATE.SEARCH_STATE;
                            EndPresentation();
                            break;
                    }
                    break;

                case STATE.SEARCH_STATE:
                    switch (input)
                    {
                        case INPUT.IN_PROJECTILE:
                            currentState = STATE.PROJECTILE;
                            StartProjectile();
                            break;

                        case INPUT.IN_FAST_RUSH:
                            currentState = STATE.FAST_RUSH;
                            StartFastRush();
                            break;

                        case INPUT.IN_FOLLOW:
                            currentState = STATE.FOLLOW;
                            StartFollowing();
                            break;
                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartWander();
                            break;
                        case INPUT.IN_PRESENTATION:
                            currentState = STATE.PRESENTATION;
                            StartPresentation();
                            break;
                    }
                    break;

                case STATE.FOLLOW:
                    switch (input)
                    {
                        case INPUT.IN_FOLLOW_END:
                            currentState = STATE.SEARCH_STATE;
                            EndFollowing();
                            break;
                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDie();
                            break;
                        case INPUT.IN_PRESENTATION:
                            currentState = STATE.PRESENTATION;
                            EndFollowing();
                            StartPresentation();
                            break;
                    }
                    break;

                case STATE.PROJECTILE:
                    switch (input)
                    {
                        case INPUT.IN_PROJECTILE_END:
                            currentState = STATE.SEARCH_STATE;
                            EndProjectile();
                            break;
                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDie();
                            break;
                        case INPUT.IN_PRESENTATION:
                            currentState = STATE.PRESENTATION;
                            EndProjectile();
                            StartPresentation();
                            break;
                    }
                    break;
                case STATE.FAST_RUSH:
                    switch (input)
                    {
                        case INPUT.IN_FAST_RUSH_END:
                            currentState = STATE.SLOW_RUSH;
                            StartSlowRush();
                            break;
                        case INPUT.IN_SLOW_RUSH_END:
                            currentState = STATE.SEARCH_STATE;
                            EndSlowRush();
                            break;
                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDie();
                            break;
                        case INPUT.IN_PRESENTATION:
                            currentState = STATE.PRESENTATION;
                            EndSlowRush();
                            StartPresentation();
                            break;
                    }
                    break;

                case STATE.SLOW_RUSH:
                    switch (input)
                    {
                        case INPUT.IN_SLOW_RUSH_END:
                            currentState = STATE.SEARCH_STATE;
                            EndSlowRush();
                            break;
                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDie();
                            break;
                        case INPUT.IN_PRESENTATION:
                            currentState = STATE.PRESENTATION;
                            EndSlowRush();
                            StartPresentation();
                            break;
                    }
                    break;

                case STATE.WANDER:
                    switch (input)
                    {
                        case INPUT.IN_WANDER_END:
                            currentState = STATE.SEARCH_STATE;
                            EndWander();
                            break;
                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDie();
                            break;
                        case INPUT.IN_PRESENTATION:
                            currentState = STATE.PRESENTATION;
                            EndWander();
                            StartPresentation();
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
            case STATE.DEAD:
                UpdateDie();
                break;
            case STATE.FOLLOW:
                if (!Core.instance.IsMandoDead()) UpdateFollowing();
                break;
            case STATE.PROJECTILE:
                UpdateProjectile();
                break;
            case STATE.FAST_RUSH:
                UpdateFastRush();
                break;
            case STATE.SLOW_RUSH:
                UpdateSlowRush();
                break;
            case STATE.WANDER:
                if (!Core.instance.IsMandoDead()) UpdateWander();
                break;
            case STATE.PRESENTATION:
                UpdatePresentation();
                break;
            case STATE.SEARCH_STATE:
                SelectAction();
                break;
        }

        limboHealth = Mathf.Lerp(limboHealth, healthPoints, 0.01f);
        if (healthbarMaterial != null)
        {
            healthbarMaterial.SetFloatUniform("length_used", healthPoints / maxHealthPoints);
            healthbarMaterial.SetFloatUniform("limbo", limboHealth / maxHealthPoints);
        }

        if (damaged > 0.01f)
        {
            damaged = Mathf.Lerp(damaged, 0.0f, 0.1f);
        }
        else
        {
            damaged = 0.0f;
        }
        if (wampaMesh != null)
        {
            Material wampaMeshMat = wampaMesh.GetComponent<Material>();

            if (wampaMeshMat != null)
            {
                wampaMeshMat.SetFloatUniform("damaged", damaged);
            }
        }
    }

    private void SelectAction()
    {
        if (!Core.instance.IsMandoDead())
        {
            if (resting)
            {
                int decision = randomNum.Next(1, 100);
                if (decision <= 60)
                    inputsList.Add(INPUT.IN_FOLLOW);
                else
                    inputsList.Add(INPUT.IN_WANDER);
            }
            else
            {
                if (Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) >= distanceProjectile)
                    inputsList.Add(INPUT.IN_PROJECTILE);
                else
                    inputsList.Add(INPUT.IN_FAST_RUSH);
            }
        }
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
            BH_Bullet bulletComp = collidedGameObject.GetComponent<BH_Bullet>();

            if (bulletComp != null)
            {
                float damageToBoss = bulletComp.GetDamage();

                if (Core.instance != null)
                    damageToBoss *= Core.instance.DamageToBosses;
                TakeDamage(damageToBoss * damageRecieveMult * BlasterVulnerability);
                damaged = 1.0f;
            }

            if (Core.instance.hud != null && currentState != STATE.DEAD)
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(25, 0.95f);
            }
        }
        else if (collidedGameObject.CompareTag("ChargeBullet"))
        {

            ChargedBullet bulletComp = collidedGameObject.GetComponent<ChargedBullet>();
            Audio.PlayAudio(gameObject, "Play_Sniper_Hit");
            if (bulletComp != null)
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

                float damageToBoss = bulletComp.GetDamage() * damageMult;
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
                damaged = 1.0f;
            }

            if (Core.instance.hud != null && currentState != STATE.DEAD)
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(55, 0.25f);
            }
        }
        else if (collidedGameObject.CompareTag("WorldLimit"))
        {
            if (currentState != STATE.DEAD)
            {
                inputsList.Add(INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("Wall"))
        {
            if (currentState == STATE.FAST_RUSH || currentState == STATE.SLOW_RUSH)
                inputsList.Add(INPUT.IN_SLOW_RUSH_END);
        }
        else if (collidedGameObject.CompareTag("Player"))
        {

        }
    }

    public override void TakeDamage(float damage)
    {
        if (!DebugOptionsHolder.bossDmg)
        {

            if (currentState != STATE.DEAD)
            {
                Audio.PlayAudio(gameObject, "Play_Wampa_Hit");
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
                    inputsList.Add(INPUT.IN_DEAD);
                }
            }
        }
    }

    private void WampaAngry()
    {
        speed = 6.0f;
        fastRushSpeed = 15.0f;
        slowRushSpeed = 10.0f;
        restingTime = 1.0f;
        maxHealthPoints = maxHealthPointsAngry;
    }
}