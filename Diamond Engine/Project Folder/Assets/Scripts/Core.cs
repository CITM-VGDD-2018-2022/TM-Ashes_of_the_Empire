using DiamondEngine;
using System;
using System.Collections.Generic;

public class Core : Entity
{
    public static Core instance;
    private static Dictionary<STATUS_TYPE, StatusData> PlayerStatuses = new Dictionary<STATUS_TYPE, StatusData>();
    public static List<STATUS_TYPE> boons = new List<STATUS_TYPE>();

    public enum STATE : int
    {
        NONE = -1,
        IDLE,
        MOVE,
        DASH,
        SHOOTING,
        SHOOT,
        GADGET_SHOOT_CHARGE,
        GADGET_SHOOT,
        GADGET_SHOOT_END,
        CHARGING_SEC_SHOOT,
        SECONDARY_SHOOT,
        SECONDARY_SHOOT_END,
        DEAD
    }

    enum INPUT : int
    {
        IN_IDLE,
        IN_MOVE,
        IN_DASH,
        IN_DASH_END,
        IN_SHOOTING,
        IN_SHOOTING_END,
        IN_SHOOT,
        IN_SHOOT_END,
        IN_GADGET_SHOOT_START,
        IN_GADGET_SHOOT_LAUNCH,
        IN_GADGET_SHOOT_END,
        IN_GADGET_SHOOT_ANIM_END,
        IN_CHARGE_SEC_SHOOT,
        IN_CHARGE_SEC_SHOOT_END,
        IN_SEC_SHOOT_END,
        IN_SEC_SHOOT_ANIM_END,
        IN_DEAD,
        IN_DEAD_END
    }

    enum PARTICLES : int
    {
        NONE = -1,
        DUST,
        MUZZLE,
        JETPACK,
        IMPACT,
        SNIPER,
        GRENADE,
        HEAL,
        SNIPER_CHARGE,
        SNIPER_MUZZEL
    }

    enum ACTION : int
    {
        NONE = -1,
        SHOOT_BLASTER,
        SHOOT_SNIPER,
        SHOOT_GRENADE
    }

    enum FLOOR_TYPE : int
    {
        NONE = -1,
        SAND,
        SNOW,
        METAL,
        WATER,
        STONE
    }

    enum TUTO_STATES : int
    {
        NONE = -1,
        DASH,
        BLASTER,
        PUSH,
        SNIPER,
        WALL
    }

    public GameObject shootPoint = null;
    public GameObject sniperShootPoint = null;
    public GameObject hud = null;

    public GameObject rifle = null;
    public GameObject blaster = null;

    private bool scriptStart = true;
    //State
    private STATE currentState = STATE.NONE;   //NEVER SET THIS VARIABLE DIRECTLLY, ALLWAYS USE INPUTS
                                               //Setting states directlly will break the behaviour  -Jose
    private List<INPUT> inputsList = new List<INPUT>();

    private bool rightTriggerPressed = false;

    // Movement
    public float rotationSpeed = 2.0f;
    public float movementSpeed = 35.0f;
    public float mouseSens = 1.0f;
    public float afterShootDelay = 2.5f;
    public float afterShootMult = 1.5f;
    private double angle = 0.0f;
    private float runTime = 0.0f;
    private float dustTime = 0.0f;
    private float currAnimationPlaySpd = 0f;
    private float footStepTimer = 0f;

    //Skill
    private bool skill_damageReductionDashActive = false;
    private float skill_damageReductionDashTimer = 0.0f;
    private bool skill_groguIncreaseDamageActive = false;
    private float skill_groguIncreaseDamageTimer = 0.0f;
    public float skill_SoloHeal = 0.0f;
    public int ShopDiscount = 0;
    // Dash
    public float dashCD = 0.33f;
    public float dashDuration = 0.2f;
    public float dashDistance = 6f;
    private float dashSpeed = 30.0f;
    public int maxDashNumber = 1;
    public float betweeMultiplenDashesCD = 0.12f;
    private float dashTimer = 0.0f;
    private float dashCDTimer = 0.0f;
    private float dashCDModifier = 1.0f;
    private int currentDashes = 1;
    private float betweenDashesCDTimer = 0.0f;
    private float dashStartYPos = 0.0f;
    private bool dashAvailable = true;
    private float timeSinceLastDash = 0.0f;
    public GameObject RayCastObj = null;
    private Vector3 dashDirection = new Vector3(0, 0, 0);
    private Quaternion preDashRotation = new Quaternion(0, 0, 0, 1);
    

    // Shooting
    public float baseFireRate = 0.2f;
    private float currFireRate = 0f;
    public float heatReductionSpeed = 22.5f;
    public float dashHeatReductionMult = 0.75f;
    public float onGrenadeHeatReduction = 5f;
    public float onSniperHeatReduction = 10f;
    private float shootingTimer = 0.0f;
    private bool primaryWeaponOverHeat = false;

    private bool hasShot = false;

    public float fireRateAfterDashRecoverRatio = 2.0f;
    //private float secondaryRateRecoverCap = 0.0f;

    public float fireRateMultCap = 0.0f;
    private int deathZone = 15000;
    private float normalShootSpeed = 0.0f;
    private float gadgetShootSkill = 0.0f;
    private float stopShootingTime = 0f;

    //Grenades
    private static float grenadesFireRate;
    private float grenadesFireRateTimer = 0.0f;
    private bool grenade_reloading = false;
    private float grenade_onDash = 5.0f;
    private Material grenadeCooldownIcon = null;
    private Material sniperBullet1 = null;
    private Material sniperBullet2 = null;
    private GameObject myGrenade1 = null;
    private GameObject myGrenade2 = null;
    public float minGrenadeForwardForceMult = 1f;
    public float maxGrenadeForwardForceMult = 1f;
    public float minGrenadeUpForce = 1f;
    public float maxGrenadeUpForce = 1f;
    public float grenadeLaunchMaxTime = 0f;
    public float grenadeMinLauncTime = 0.2f;
    private float grenadeLaunchMinTimer = 0.0f;
    private float grenadeMaxLaunchTimer = 0f;
    private bool canLaunchGrenade = false;
    private float grenadeThrowAnimTime = 0f;
    private float grenadeThrowAnimMult = 0f;
    private float grenadeThrowAnimTimer = 0f;
    private bool playedThrowAudio = false;

    // Secondary Shoot (sniper)
    public float timeToPerfectCharge = 0.424f;
    public int framesForPerfectCharge = 4;
    private float timeToPerfectChargeEnd = 0f;
    public float bulletRechargeTime = 0f;
    public int numberOfBullets = 0;
    private int currentBullets = 0;
    private float totalRechargeTime = 0f;
    private float sniperRechargeTimer = 0f;
    private float chargeTimer = 0.0f;
    public float chargedBulletDmg = 0f;
    public float perfectShotDmgMult = 1.4f;
    public float overShotDmgMult = 1.05f;
    public Vector3 defaultSniperLaserColor = new Vector3(1, 0, 0);
    private Vector3 currSniperLaserColor = new Vector3(1, 0, 0);
    public float overheatTimeBeeping = 0.08f;
    private float changeColorSniperTimer = 0f;
    private float sniperShotTimer = 0.0f;
    private float snipShootAnimTime = 0f;
    private float snipShootAnimTimer = 0f;
    private float snipPrepareAnimTime = 0f;
    private float snipPrepareAnimTimer = 0f;
    private bool canShootSniper = false;
    public float sniperStartAngleRays = 35f;

    //Die
    private float dieTimer = 0.0f;
    public float dieTime = 0.0f; //Setted in Awake

    //Animations
    private float shootAnimationTotalTime = 0.0f;
    public float sniperAimAnimMult = 1.5f;
    public float luckyMod = 1f;

    //Controller Variables
    int verticalInput = 0;
    int horizontalInput = 0;
    Vector3 gamepadInput = Vector3.zero;
    public bool lockInputs = false;
    private bool lockAttacks = false;

    //For Pause
    public GameObject background = null;
    public GameObject pause = null;

    private Vector3 mySpawnPos = new Vector3(0.0f, 0.0f, 0.0f);

    AimBot myAimbot = null;

    private static float bulletDamage = 9f;

    private float bulletDamageDefault = 9f;

    //Audio
    public GameObject secSound = null;

    //Telemetry
    int numberOfShots = 0;
    int damageTaken = 0;
    int timesFellOfMap = 0;
    float distanceMoved = 0f;
    float timeOfRoom = 0f;

    private float old_hp = 0.0f;

    private bool snipercharge = true;

    private ACTION lastAction = ACTION.NONE;
    private bool hasDashed = false;

    private FLOOR_TYPE floorType = FLOOR_TYPE.NONE;

    private TUTO_STATES tuto_state = TUTO_STATES.NONE;
    public float fallDamage = 15f;
    private string lastFootsteps = "";
    private bool updateSniper = false;

    public GameObject fastParticleObject = null;
    public GameObject slowParticleObject = null;

    public bool startAvailable = true;
    public void Awake()
    {
        #region VARIABLES WITH DEPENDENCIES

        //bulletDamage *= 2.0f;  //Please do not delete this, it's for quick debugging purposes

        // INIT VARIABLES WITH DEPENDENCIES //
        //Animation
        shootAnimationTotalTime = 0.288f;

        //Dash - if scene doesnt have its values
        //dashDuration = 0.2f;
        //dashDistance = 4.5f;
        lastFootsteps = "Play_Footsteps_Stone_Mando";
        sniperRechargeTimer = 0; // TODO if sniper recharge sfx sounds during cinematic, try uncomment this line
        updateSniper = false;
        #endregion

        #region SHOOT

        baseFireRate = Math.Max(baseFireRate, 0.1f);

        normalShootSpeed = shootAnimationTotalTime / baseFireRate;
        gadgetShootSkill = shootAnimationTotalTime / grenadeMinLauncTime;

        myAimbot = gameObject.GetComponent<AimBot>();

        currFireRate = baseFireRate;

        timeToPerfectChargeEnd = timeToPerfectCharge + (0.016f * framesForPerfectCharge);

        currSniperLaserColor = defaultSniperLaserColor;

        totalRechargeTime = numberOfBullets * bulletRechargeTime;
        currentBullets = numberOfBullets;

        grenadeThrowAnimTime = Animator.GetAnimationDuration(gameObject, "GrenadeThrow");
        grenadeThrowAnimMult = (grenadeThrowAnimTime / grenadeLaunchMaxTime);

        snipShootAnimTime = Animator.GetAnimationDuration(gameObject, "SniperShotP2");
        snipPrepareAnimTime = Animator.GetAnimationDuration(gameObject, "SniperShotP1") / sniperAimAnimMult;

        grenadesFireRate = 4.0f;
        #endregion

        #region DASH

        // Dash
        dashTimer = 0.0f;
        maxDashNumber = Math.Max(maxDashNumber, 1);
        currentDashes = maxDashNumber;
        dashSpeed = dashDistance / dashDuration;

        #endregion

        #region OTHERS

        //player instance
        instance = this;

        //Controller
        deathZone = 15000;

        currentState = STATE.IDLE;
        Animator.Play(gameObject, "Idle", speedMult);
        UpdateAnimationSpd(1f);

        lockInputs = false;

        Debug.Log("Start!");
        mySpawnPos = new Vector3(gameObject.transform.globalPosition.x, gameObject.transform.globalPosition.y, gameObject.transform.globalPosition.z);
        runTime = Animator.GetAnimationDuration(gameObject, "Run") / 2;

        if (rifle != null)
            rifle.Enable(false);

        if (blaster != null)
            blaster.Enable(false);

        dieTime = Animator.GetAnimationDuration(gameObject, "Die") - 0.016f - 1f;
        ShopDiscount = 0;
        #endregion

        #region EVENTS
        PlayerHealth.onPlayerDeath += DeadInput;
        #endregion

        #region STATUS_SYSTEM

        InitEntity(ENTITY_TYPE.PLAYER);

        #endregion

    }

    public void Update()
    {
        // Placeholder for Start() function
        if (scriptStart == true)
        {
            ConfigFunctionality.UpdateDisplayText();
            lastAction = ACTION.NONE;
            hasDashed = false;
            dashCDModifier = 1;
            LoadBuffs();
            hud = InternalCalls.FindObjectWithName("HUD");

            if (hud == null)
                Debug.Log("Core: HUD not found");

            pause = InternalCalls.FindObjectWithName("PauseMenu");

            if (pause == null)
                Debug.Log("Core: Pause menu not found");

            background = InternalCalls.FindObjectWithName("Background");

            if (pause == null)
                Debug.Log("Core: Background not found");

            GameObject grenadeCooldown = InternalCalls.FindObjectWithName("GrenadeCooldownIcon");
            if (grenadeCooldown != null)
                grenadeCooldownIcon = grenadeCooldown.GetComponent<Material>();


            GameObject sniper1Cooldown = InternalCalls.FindObjectWithName("SniperCooldown1");
            if (sniper1Cooldown != null)
                sniperBullet1 = sniper1Cooldown.GetComponent<Material>();

            GameObject sniper2Cooldown = InternalCalls.FindObjectWithName("SniperCooldown2");
            if (sniper2Cooldown != null)
                sniperBullet2 = sniper2Cooldown.GetComponent<Material>();

            if (myGrenade1 == null)
            {
                myGrenade1 = InternalCalls.CreatePrefab("Library/Prefabs/660835192.prefab", shootPoint.transform.globalPosition + shootPoint.transform.GetForward().normalized * 0.5f, shootPoint.transform.globalRotation, null);
                if (myGrenade1 != null)
                {
                    SlowGrenade grenadeComp = myGrenade1.GetComponent<SlowGrenade>();

                    if (grenadeComp != null)
                    {
                        grenadeComp.GrenadeInit(shootPoint.transform.globalPosition + shootPoint.transform.GetForward().normalized * 0.5f, shootPoint.transform.globalRotation, Vector3.zero);
                        grenadeComp.GrenadeFinish();
                    }

                    myGrenade1.Enable(false);
                }
            }

            if (secSound == null)
            {
                secSound = InternalCalls.FindObjectWithName("SecSound");

                if (secSound == null)
                    Debug.Log("Core: Sec Sound GO not found");
            }



            GameObject lockInputsScene = InternalCalls.FindObjectWithName("LockInputsBool");

            if (lockInputsScene != null)
                lockAttacks = true;

            DisableBlaster();

            grenade_onDash = 10.0f;

            //SkillDataTree -> LoadStaticBuffs();
            old_hp = PlayerHealth.currMaxHealth;
            //Start();
            scriptStart = false;

            return;
        }

        #region UPDATE STUFF

        myDeltaTime = Time.deltaTime * speedMult;

        UpdateControllerInputs();

        UpdateStatuses();

        #endregion

        #region STATE MACHINE

        ProcessInternalInput();
        ProcessExternalInput();

        ProcessState();

        UpdateState();


        #endregion

        if (old_hp < PlayerHealth.currHealth)
            PlayParticles(PARTICLES.HEAL);
        old_hp = PlayerHealth.currHealth;

    }


    //Timers go here
    private void ProcessInternalInput()
    {
        if (dashTimer > 0)
        {
            dashTimer -= myDeltaTime;

            if (dashTimer <= 0)
            {
                // Debug.Log(dashTimer.ToString());

                GameObject hit = null;
                if (RayCastObj != null)
                {
                    float hitDistance = 0;
                    hit = InternalCalls.RayCast(RayCastObj.transform.globalPosition, RayCastObj.transform.GetForward(), 100, ref hitDistance);

                }
                else
                    Debug.Log("Raycast obj == null");

                if (hit != null)
                {
                    //Debug.Log(hit.tag);

                    if (hit.CompareTag("Gap"))
                    {
                        dashTimer = 0.1f;
                    }
                    else
                        inputsList.Add(INPUT.IN_DASH_END);

                }
                else
                {
                    inputsList.Add(INPUT.IN_DASH_END);
                    Debug.Log("Problem with raycast");
                }
            }
        }

        if (dashCDTimer > 0)
        {
            dashCDTimer -= myDeltaTime * dashCDModifier;
            betweenDashesCDTimer -= myDeltaTime * dashCDModifier;

            if (dashCDTimer <= 0f)
                currentDashes = maxDashNumber;

            if (currentDashes > 0 && betweenDashesCDTimer <= 0f)
                dashAvailable = true;

        }

        if (shootingTimer > 0 || stopShootingTime <= 0f)
        {
            shootingTimer -= myDeltaTime;

            if (shootingTimer <= 0 && stopShootingTime <= 0f)
            {
                inputsList.Add(INPUT.IN_SHOOT);
            }
        }

        if (grenadeLaunchMinTimer > 0)
        {
            grenadeLaunchMinTimer -= myDeltaTime;

            if (grenadeLaunchMinTimer <= 0)
                canLaunchGrenade = true;
        }

        if (grenadeMaxLaunchTimer > 0)
        {
            grenadeMaxLaunchTimer -= myDeltaTime;

            if (grenadeMaxLaunchTimer <= 0)
                inputsList.Add(INPUT.IN_GADGET_SHOOT_LAUNCH);
        }

        if (skill_damageReductionDashActive && skill_damageReductionDashTimer > 0)
        {
            skill_damageReductionDashTimer -= myDeltaTime;

            if (skill_damageReductionDashTimer <= 0)
                skill_damageReductionDashActive = false;
        }

        if (skill_groguIncreaseDamageActive && skill_groguIncreaseDamageTimer > 0)
        {
            skill_groguIncreaseDamageTimer -= myDeltaTime;

            if (skill_groguIncreaseDamageTimer <= 0)
                skill_groguIncreaseDamageActive = false;
        }

        //Debug.Log("State: " + currentState.ToString());

        if (sniperShotTimer > 0.0f)
        {
            sniperShotTimer -= myDeltaTime;

            if (sniperShotTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_IDLE);
            }
        }
        if (snipShootAnimTimer > 0.0f)
        {
            snipShootAnimTimer -= myDeltaTime;

            if (snipShootAnimTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_SEC_SHOOT_ANIM_END);
            }
        }
        if (grenadeThrowAnimTimer > 0.0f || this.currentState == STATE.GADGET_SHOOT_END)
        {
            grenadeThrowAnimTimer -= myDeltaTime;

            if (grenadeThrowAnimTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_GADGET_SHOOT_ANIM_END);
            }
        }
        if (snipPrepareAnimTimer > 0.0f)
        {
            snipPrepareAnimTimer -= myDeltaTime;

            if (snipPrepareAnimTimer <= 0.0f)
            {
                canShootSniper = true;
                PlayParticles(PARTICLES.SNIPER_CHARGE);
            }
        }

        float grenadeDeltaTime = Math.Max(myDeltaTime, Time.deltaTime * 0.5f);
        if (HasStatus(STATUS_TYPE.SEC_RECOVERY))
            grenadesFireRateTimer -= grenadeDeltaTime * (1 + GetStatusData(STATUS_TYPE.SEC_RECOVERY).severity / 100);
        else
            grenadesFireRateTimer -= grenadeDeltaTime;

        timeOfRoom += myDeltaTime;
        timeSinceLastDash += myDeltaTime;

        if (HasStatus(STATUS_TYPE.GRENADE_ON_DASH))
            grenade_onDash += myDeltaTime;

        if (dieTimer > 0.0f)
        {
            dieTimer -= myDeltaTime;
            if (dieTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DEAD_END);
            }
        }
    }

    public void refreshCooldowns()
    {
        //sniperRechargeTimer = 0;
        //grenadesFireRateTimer = 0;
    }
    //Controler inputs go here
    private void ProcessExternalInput()
    {

        bool isPrimaryOverHeat = false;
        if (hud != null)
        {
            isPrimaryOverHeat = hud.GetComponent<HUD>().IsPrimaryOverheated();

            if (isPrimaryOverHeat == true && primaryWeaponOverHeat == false)
                Audio.PlayAudio(secSound, "Play_Mando_Blaster_Overcharged");
            else if (isPrimaryOverHeat == false && primaryWeaponOverHeat == true)
                Audio.PlayAudio(secSound, "Play_Blaster_Cooldown_Finished");
            primaryWeaponOverHeat = isPrimaryOverHeat;
        }

        if (!lockInputs && tuto_state == TUTO_STATES.NONE)
        {
            if ((Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_DOWN || Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_REPEAT) && isPrimaryOverHeat == false && lockAttacks == false)
            {
                stopShootingTime = 0f;
                inputsList.Add(INPUT.IN_SHOOTING);
            }
            else if ((Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_UP || Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_IDLE || isPrimaryOverHeat == true) && hasShot == true && lockAttacks == false)
            {
                if (CanStopShooting() == true || isPrimaryOverHeat == true || (this.currentState != STATE.SHOOT && this.currentState != STATE.SHOOTING))
                {
                    inputsList.Add(INPUT.IN_SHOOTING_END);
                    hasShot = false;
                }
                else if (CanStopShooting() == false)
                {
                    stopShootingTime += myDeltaTime;
                    Animator.Play(gameObject, "Shoot", 0.01f);

                    if (blaster != null)
                    {
                        blaster.Enable(true);
                        Animator.Play(blaster, "Shoot", 0.01f);
                    }

                    UpdateAnimationSpd(0.01f);
                }
            }


            if ((Input.GetGamepadButton(DEControllerButton.B) == KeyState.KEY_DOWN) && currentBullets > 0 && lockAttacks == false)
            {
                inputsList.Add(INPUT.IN_CHARGE_SEC_SHOOT);
            }
            else if ((Input.GetGamepadButton(DEControllerButton.B) == KeyState.KEY_UP || (Input.GetGamepadButton(DEControllerButton.B) == KeyState.KEY_IDLE && currentState == STATE.CHARGING_SEC_SHOOT)) && canShootSniper == true && lockAttacks == false)
            {
                inputsList.Add(INPUT.IN_CHARGE_SEC_SHOOT_END);
            }

            if (Input.GetRightTrigger() > 0 && rightTriggerPressed == false && dashAvailable == true)
            {
                inputsList.Add(INPUT.IN_DASH);
                rightTriggerPressed = true;
            }
            else if (Input.GetRightTrigger() == 0 && rightTriggerPressed == true)
                rightTriggerPressed = false;


            if ((Input.GetKey(DEKeyCode.LSHIFT) == KeyState.KEY_REPEAT && Input.GetKey(DEKeyCode.LALT) == KeyState.KEY_REPEAT &&
                Input.GetKey(DEKeyCode.D) == KeyState.KEY_DOWN && myDeltaTime != 0.0f)/* || Input.GetGamepadButton(DEControllerButton.BACK) == KeyState.KEY_DOWN*/)
            {
                InternalCalls.CreateUIPrefab("Library/Prefabs/1342862578.prefab", new Vector3(0, 0, 0), new Quaternion(0, 0, 0), new Vector3(1, 1, 1));
            }

            if (Input.GetKey(DEKeyCode.T) == KeyState.KEY_DOWN)
            {
                Debug.Log("Amount of bullets fired: " + numberOfShots.ToString());
                Debug.Log("Time to complete room: " + timeOfRoom.ToString() + " seconds");
                Debug.Log("Times fallen of the map: " + timesFellOfMap.ToString());
                Debug.Log("Total distance moved: " + distanceMoved.ToString() + " mandos");
            }

            if (Input.GetGamepadButton(DEControllerButton.Y) == KeyState.KEY_DOWN && grenadesFireRateTimer <= 0.0f && lockAttacks == false)
            {
                inputsList.Add(INPUT.IN_GADGET_SHOOT_START);
            }
            else if ((Input.GetGamepadButton(DEControllerButton.Y) == KeyState.KEY_UP || (Input.GetGamepadButton(DEControllerButton.Y) == KeyState.KEY_IDLE && currentState == STATE.GADGET_SHOOT_CHARGE)) && canLaunchGrenade == true)
            {
                inputsList.Add(INPUT.IN_GADGET_SHOOT_LAUNCH);
            }


            if (IsJoystickMoving() == true)
                inputsList.Add(INPUT.IN_MOVE);

            else if (currentState == STATE.MOVE && IsJoystickMoving() == false)
                inputsList.Add(INPUT.IN_IDLE);
        }
        else if (tuto_state == TUTO_STATES.DASH)
        {
            if (Input.GetRightTrigger() > 0 && rightTriggerPressed == false && dashAvailable == true)
            {
                inputsList.Add(INPUT.IN_DASH);
                rightTriggerPressed = true;
            }
            else if (Input.GetRightTrigger() == 0 && rightTriggerPressed == true)
                rightTriggerPressed = false;


            if ((Input.GetKey(DEKeyCode.LSHIFT) == KeyState.KEY_REPEAT && Input.GetKey(DEKeyCode.LALT) == KeyState.KEY_REPEAT &&
                Input.GetKey(DEKeyCode.D) == KeyState.KEY_DOWN && myDeltaTime != 0.0f)/* || Input.GetGamepadButton(DEControllerButton.BACK) == KeyState.KEY_DOWN*/)
            {
                InternalCalls.CreateUIPrefab("Library/Prefabs/1342862578.prefab", new Vector3(0, 0, 0), new Quaternion(0, 0, 0), new Vector3(1, 1, 1));
            }

            if (Input.GetKey(DEKeyCode.T) == KeyState.KEY_DOWN)
            {
                Debug.Log("Amount of bullets fired: " + numberOfShots.ToString());
                Debug.Log("Time to complete room: " + timeOfRoom.ToString() + " seconds");
                Debug.Log("Times fallen of the map: " + timesFellOfMap.ToString());
                Debug.Log("Total distance moved: " + distanceMoved.ToString() + " mandos");
            }

            if (IsJoystickMoving() == true)
                inputsList.Add(INPUT.IN_MOVE);

            else if (currentState == STATE.MOVE && IsJoystickMoving() == false)
                inputsList.Add(INPUT.IN_IDLE);
        }
        else if (tuto_state == TUTO_STATES.BLASTER || tuto_state == TUTO_STATES.PUSH)
        {
            if ((Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_DOWN || Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_REPEAT) && isPrimaryOverHeat == false && lockAttacks == false)
            {
                stopShootingTime = 0f;
                inputsList.Add(INPUT.IN_SHOOTING);
            }
            else if ((Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_UP || Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_IDLE || isPrimaryOverHeat == true) && hasShot == true && lockAttacks == false)
            {
                if (CanStopShooting() == true || isPrimaryOverHeat == true || (this.currentState != STATE.SHOOT && this.currentState != STATE.SHOOTING))
                {
                    inputsList.Add(INPUT.IN_SHOOTING_END);
                    hasShot = false;
                }
                else if (CanStopShooting() == false)
                {
                    stopShootingTime += myDeltaTime;
                    Animator.Play(gameObject, "Shoot", 0.01f);

                    if (blaster != null)
                    {
                        blaster.Enable(true);
                        Animator.Play(blaster, "Shoot", 0.01f);
                    }

                    UpdateAnimationSpd(0.01f);
                }
            }

            if (Input.GetRightTrigger() > 0 && rightTriggerPressed == false && dashAvailable == true)
            {
                inputsList.Add(INPUT.IN_DASH);
                rightTriggerPressed = true;
            }
            else if (Input.GetRightTrigger() == 0 && rightTriggerPressed == true)
                rightTriggerPressed = false;


            if ((Input.GetKey(DEKeyCode.LSHIFT) == KeyState.KEY_REPEAT && Input.GetKey(DEKeyCode.LALT) == KeyState.KEY_REPEAT &&
                Input.GetKey(DEKeyCode.D) == KeyState.KEY_DOWN && myDeltaTime != 0.0f) /*|| Input.GetGamepadButton(DEControllerButton.BACK) == KeyState.KEY_DOWN*/)
            {
                InternalCalls.CreateUIPrefab("Library/Prefabs/1342862578.prefab", new Vector3(0, 0, 0), new Quaternion(0, 0, 0), new Vector3(1, 1, 1));
            }

            if (Input.GetKey(DEKeyCode.T) == KeyState.KEY_DOWN)
            {
                Debug.Log("Amount of bullets fired: " + numberOfShots.ToString());
                Debug.Log("Time to complete room: " + timeOfRoom.ToString() + " seconds");
                Debug.Log("Times fallen of the map: " + timesFellOfMap.ToString());
                Debug.Log("Total distance moved: " + distanceMoved.ToString() + " mandos");
            }

            if (IsJoystickMoving() == true)
                inputsList.Add(INPUT.IN_MOVE);

            else if (currentState == STATE.MOVE && IsJoystickMoving() == false)
                inputsList.Add(INPUT.IN_IDLE);
        }
        else if (tuto_state == TUTO_STATES.SNIPER)
        {
            if ((Input.GetGamepadButton(DEControllerButton.B) == KeyState.KEY_DOWN) && currentBullets > 0 && lockAttacks == false)
            {
                inputsList.Add(INPUT.IN_CHARGE_SEC_SHOOT);
            }
            else if ((Input.GetGamepadButton(DEControllerButton.B) == KeyState.KEY_UP || (Input.GetGamepadButton(DEControllerButton.B) == KeyState.KEY_IDLE && currentState == STATE.CHARGING_SEC_SHOOT)) && canShootSniper == true && lockAttacks == false)
            {
                inputsList.Add(INPUT.IN_CHARGE_SEC_SHOOT_END);
            }

            if (Input.GetRightTrigger() > 0 && rightTriggerPressed == false && dashAvailable == true)
            {
                inputsList.Add(INPUT.IN_DASH);
                rightTriggerPressed = true;
            }
            else if (Input.GetRightTrigger() == 0 && rightTriggerPressed == true)
                rightTriggerPressed = false;


            if ((Input.GetKey(DEKeyCode.LSHIFT) == KeyState.KEY_REPEAT && Input.GetKey(DEKeyCode.LALT) == KeyState.KEY_REPEAT &&
                Input.GetKey(DEKeyCode.D) == KeyState.KEY_DOWN && myDeltaTime != 0.0f) /*|| Input.GetGamepadButton(DEControllerButton.BACK) == KeyState.KEY_DOWN*/)
            {
                InternalCalls.CreateUIPrefab("Library/Prefabs/1342862578.prefab", new Vector3(0, 0, 0), new Quaternion(0, 0, 0), new Vector3(1, 1, 1));
            }

            if (Input.GetKey(DEKeyCode.T) == KeyState.KEY_DOWN)
            {
                Debug.Log("Amount of bullets fired: " + numberOfShots.ToString());
                Debug.Log("Time to complete room: " + timeOfRoom.ToString() + " seconds");
                Debug.Log("Times fallen of the map: " + timesFellOfMap.ToString());
                Debug.Log("Total distance moved: " + distanceMoved.ToString() + " mandos");
            }

            if (IsJoystickMoving() == true)
                inputsList.Add(INPUT.IN_MOVE);

            else if (currentState == STATE.MOVE && IsJoystickMoving() == false)
                inputsList.Add(INPUT.IN_IDLE);
        }
        else if (tuto_state == TUTO_STATES.WALL)
        {
            if ((Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_DOWN || Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_REPEAT) && isPrimaryOverHeat == false && lockAttacks == false)
            {
                stopShootingTime = 0f;
                inputsList.Add(INPUT.IN_SHOOTING);
            }
            else if ((Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_UP || Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_IDLE || isPrimaryOverHeat == true) && hasShot == true && lockAttacks == false)
            {
                if (CanStopShooting() == true || isPrimaryOverHeat == true || (this.currentState != STATE.SHOOT && this.currentState != STATE.SHOOTING))
                {
                    inputsList.Add(INPUT.IN_SHOOTING_END);
                    hasShot = false;
                }
                else if (CanStopShooting() == false)
                {
                    stopShootingTime += myDeltaTime;
                    Animator.Play(gameObject, "Shoot", 0.01f);

                    if (blaster != null)
                    {
                        blaster.Enable(true);
                        Animator.Play(blaster, "Shoot", 0.01f);
                    }

                    UpdateAnimationSpd(0.01f);
                }
            }


            if ((Input.GetGamepadButton(DEControllerButton.B) == KeyState.KEY_DOWN) && currentBullets > 0 && lockAttacks == false)
            {
                inputsList.Add(INPUT.IN_CHARGE_SEC_SHOOT);
            }
            else if ((Input.GetGamepadButton(DEControllerButton.B) == KeyState.KEY_UP || (Input.GetGamepadButton(DEControllerButton.B) == KeyState.KEY_IDLE && currentState == STATE.CHARGING_SEC_SHOOT)) && canShootSniper == true && lockAttacks == false)
            {
                inputsList.Add(INPUT.IN_CHARGE_SEC_SHOOT_END);
            }

            if (Input.GetRightTrigger() > 0 && rightTriggerPressed == false && dashAvailable == true)
            {
                inputsList.Add(INPUT.IN_DASH);
                rightTriggerPressed = true;
            }
            else if (Input.GetRightTrigger() == 0 && rightTriggerPressed == true)
                rightTriggerPressed = false;


            if ((Input.GetKey(DEKeyCode.LSHIFT) == KeyState.KEY_REPEAT && Input.GetKey(DEKeyCode.LALT) == KeyState.KEY_REPEAT &&
                Input.GetKey(DEKeyCode.D) == KeyState.KEY_DOWN && myDeltaTime != 0.0f)/* || Input.GetGamepadButton(DEControllerButton.BACK) == KeyState.KEY_DOWN*/)
            {
                InternalCalls.CreateUIPrefab("Library/Prefabs/1342862578.prefab", new Vector3(0, 0, 0), new Quaternion(0, 0, 0), new Vector3(1, 1, 1));
            }

            if (Input.GetKey(DEKeyCode.T) == KeyState.KEY_DOWN)
            {
                Debug.Log("Amount of bullets fired: " + numberOfShots.ToString());
                Debug.Log("Time to complete room: " + timeOfRoom.ToString() + " seconds");
                Debug.Log("Times fallen of the map: " + timesFellOfMap.ToString());
                Debug.Log("Total distance moved: " + distanceMoved.ToString() + " mandos");
            }

            if (IsJoystickMoving() == true)
                inputsList.Add(INPUT.IN_MOVE);

            else if (currentState == STATE.MOVE && IsJoystickMoving() == false)
                inputsList.Add(INPUT.IN_IDLE);
        }


        if (grenadesFireRateTimer > 0.0f && grenadeCooldownIcon != null)
        {
            if (HasStatus(STATUS_TYPE.CAD_BANE_BOOTS))
                GetStatusData(STATUS_TYPE.CAD_BANE_BOOTS).statChange = 0;
            grenadeCooldownIcon.SetFloatUniform("currentGrenadeCooldown", grenadesFireRate - grenadesFireRateTimer);
            grenadeCooldownIcon.SetFloatUniform("maxGrenadeCooldown", grenadesFireRate);
        }
        else if (grenade_reloading && grenadesFireRateTimer <= 0.0f)
        {
            GetStatusData(STATUS_TYPE.CAD_BANE_BOOTS).statChange = -(1 + GetStatusData(STATUS_TYPE.CAD_BANE_BOOTS).severity / 100);

            PlayParticles(PARTICLES.GRENADE);
            grenade_reloading = false;
            Audio.PlayAudio(secSound, "Play_Grenade_Cooldown_Finish");
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
                    Debug.Log("CORE ERROR STATE");
                    break;

                case STATE.IDLE:
                    switch (input)
                    {
                        case INPUT.IN_MOVE:
                            currentState = STATE.MOVE;
                            StartMove();
                            break;

                        case INPUT.IN_DASH:
                            currentState = STATE.DASH;
                            StartDash();
                            break;

                        case INPUT.IN_SHOOTING:
                            currentState = STATE.SHOOTING;
                            StartShooting();
                            break;


                        case INPUT.IN_GADGET_SHOOT_START:
                            currentState = STATE.GADGET_SHOOT_CHARGE;
                            StartGadgetCharge();
                            break;

                        case INPUT.IN_CHARGE_SEC_SHOOT:
                            currentState = STATE.CHARGING_SEC_SHOOT;
                            StartSecCharge();
                            break;

                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDead();
                            break;
                    }
                    break;

                case STATE.MOVE:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            MoveEnd();
                            StartIdle();
                            break;

                        case INPUT.IN_DASH:
                            currentState = STATE.DASH;
                            MoveEnd();
                            StartDash();
                            break;

                        case INPUT.IN_SHOOTING:
                            currentState = STATE.SHOOTING;
                            MoveEnd();
                            StartShooting();
                            break;

                        case INPUT.IN_GADGET_SHOOT_START:
                            currentState = STATE.GADGET_SHOOT_CHARGE;
                            MoveEnd();
                            StartGadgetCharge();
                            break;


                        case INPUT.IN_CHARGE_SEC_SHOOT:
                            currentState = STATE.CHARGING_SEC_SHOOT;
                            MoveEnd();
                            StartSecCharge();
                            break;

                        case INPUT.IN_DEAD:
                            MoveEnd();
                            currentState = STATE.DEAD;
                            StartDead();
                            break;
                    }
                    break;

                case STATE.DASH:
                    switch (input)
                    {
                        case INPUT.IN_DASH_END:
                            currentState = STATE.IDLE;
                            EndDash();
                            StartIdle();
                            break;

                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDead();
                            break;
                    }
                    break;

                case STATE.SHOOTING:
                    switch (input)
                    {
                        case INPUT.IN_DASH:
                            currentState = STATE.DASH;
                            StartDash();
                            break;

                        case INPUT.IN_SHOOTING_END:
                            currentState = STATE.IDLE;
                            EndShooting();
                            StartIdle();
                            break;

                        case INPUT.IN_SHOOT:
                            currentState = STATE.SHOOT;
                            StartShoot();
                            break;

                        case INPUT.IN_GADGET_SHOOT_START:
                            currentState = STATE.GADGET_SHOOT_CHARGE;
                            EndShooting();
                            StartGadgetCharge();
                            break;

                        case INPUT.IN_CHARGE_SEC_SHOOT:
                            currentState = STATE.CHARGING_SEC_SHOOT;
                            EndShooting();
                            StartSecCharge();
                            break;

                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDead();
                            break;
                    }
                    break;

                case STATE.SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_SHOOT_END:
                            currentState = STATE.SHOOTING;
                            StartShooting();
                            break;

                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDead();
                            break;
                    }
                    break;

                case STATE.CHARGING_SEC_SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            EndShootCharge();
                            StartIdle();
                            break;

                        case INPUT.IN_CHARGE_SEC_SHOOT_END:
                            currentState = STATE.SECONDARY_SHOOT;
                            StartSecondaryShoot();
                            EndShootCharge();
                            break;

                        case INPUT.IN_DASH:
                            currentState = STATE.DASH;
                            EndShootCharge();
                            StartDash();
                            break;

                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            EndShootCharge();
                            StartDead();
                            break;
                    }
                    break;

                case STATE.SECONDARY_SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_SEC_SHOOT_END:
                            currentState = STATE.SECONDARY_SHOOT_END;
                            break;

                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDead();
                            break;
                    }
                    break;
                case STATE.SECONDARY_SHOOT_END:
                    switch (input)
                    {
                        case INPUT.IN_SEC_SHOOT_ANIM_END:
                            currentState = STATE.IDLE;
                            DisableBlaster();
                            StartIdle();
                            break;

                        case INPUT.IN_MOVE:
                            currentState = STATE.MOVE;
                            StartMove();
                            break;

                        case INPUT.IN_DASH:
                            currentState = STATE.DASH;
                            StartDash();
                            break;

                        case INPUT.IN_SHOOTING:
                            currentState = STATE.SHOOTING;
                            StartShooting();
                            break;

                        case INPUT.IN_GADGET_SHOOT_START:
                            currentState = STATE.GADGET_SHOOT_CHARGE;
                            StartGadgetCharge();
                            break;

                        case INPUT.IN_CHARGE_SEC_SHOOT:
                            currentState = STATE.CHARGING_SEC_SHOOT;
                            StartSecCharge();
                            break;

                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDead();
                            break;
                    }
                    break;

                case STATE.GADGET_SHOOT_CHARGE:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            EndGadgetCharge();
                            StartIdle();
                            break;

                        case INPUT.IN_GADGET_SHOOT_LAUNCH:
                            currentState = STATE.GADGET_SHOOT;
                            EndGadgetCharge();
                            StartGadgetShoot();
                            break;

                        case INPUT.IN_DASH:
                            currentState = STATE.DASH;
                            EndShootCharge();
                            StartDash();
                            break;

                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDead();
                            break;
                    }
                    break;

                case STATE.GADGET_SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_GADGET_SHOOT_END:
                            currentState = STATE.GADGET_SHOOT_END;
                            EndGadgetShoot();
                            break;

                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDead();
                            break;
                    }
                    break;

                case STATE.GADGET_SHOOT_END:
                    switch (input)
                    {
                        case INPUT.IN_GADGET_SHOOT_ANIM_END:
                            currentState = STATE.IDLE;
                            StartIdle();
                            break;

                        case INPUT.IN_MOVE:
                            currentState = STATE.MOVE;
                            StartMove();
                            break;

                        case INPUT.IN_DASH:
                            currentState = STATE.DASH;
                            StartDash();
                            break;

                        case INPUT.IN_SHOOTING:
                            currentState = STATE.SHOOTING;
                            StartShooting();
                            break;

                        case INPUT.IN_GADGET_SHOOT_START:
                            currentState = STATE.GADGET_SHOOT_CHARGE;
                            StartGadgetCharge();
                            break;

                        case INPUT.IN_CHARGE_SEC_SHOOT:
                            currentState = STATE.CHARGING_SEC_SHOOT;
                            StartSecCharge();
                            break;

                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDead();
                            break;
                    }
                    break;
                case STATE.DEAD:
                    switch (input)
                    {
                        case INPUT.IN_DEAD_END:
                            EndDead();
                            break;
                    }
                    break;
                default:
                    Debug.Log("NEED TO ADD STATE TO CORE SWITCH");
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
                ReducePrimaryWeaponHeat();
                UpdateSecondaryShotAmmo();
                UpdateIdle();
                break;
            case STATE.MOVE:
                UpdateMove();
                ReducePrimaryWeaponHeat();
                UpdateSecondaryShotAmmo();
                break;
            case STATE.DASH:
                UpdateDash();
                ReducePrimaryWeaponHeat(dashHeatReductionMult);
                UpdateSecondaryShotAmmo();
                break;
            case STATE.SHOOTING:
                UpdateShooting();
                UpdateSecondaryShotAmmo();
                break;
            case STATE.SHOOT:
                UpdateSecondaryShotAmmo();
                break;
            case STATE.CHARGING_SEC_SHOOT:
                UpdateSecondaryShootCharge();
                ReducePrimaryWeaponHeat();
                break;
            case STATE.SECONDARY_SHOOT:
                ReducePrimaryWeaponHeat();
                break;
            case STATE.GADGET_SHOOT:
                ReducePrimaryWeaponHeat();
                UpdateSecondaryShotAmmo();
                UpdateGadgetShoot();
                break;
            case STATE.GADGET_SHOOT_CHARGE:
                ReducePrimaryWeaponHeat();
                UpdateSecondaryShotAmmo();
                UpdateGadgetCharge();
                break;
            case STATE.GADGET_SHOOT_END:
                ReducePrimaryWeaponHeat();
                UpdateSecondaryShotAmmo();
                UpdateGadgetCharge();
                break;
            case STATE.SECONDARY_SHOOT_END:
                ReducePrimaryWeaponHeat();
                ReducePrimaryWeaponHeat();
                break;
            case STATE.DEAD:
                UpdateDead();
                break;
            default:
                Debug.Log("NEED TO ADD STATE TO CORE");
                break;
        }

    }

    #region IDLE
    private void StartIdle()
    {
        Animator.Resume(this.gameObject);
        Animator.Play(gameObject, "Idle", speedMult);
        UpdateAnimationSpd(speedMult);
    }

    private void UpdateIdle()
    {
        UpdateAnimationSpd(speedMult);
    }
    #endregion

    #region NORMAL SHOOT

    private void StartShooting()
    {
        shootingTimer = GetCurrentFireRate();


        if (currFireRate != shootingTimer)
        {
            normalShootSpeed = shootAnimationTotalTime / currFireRate;
            currFireRate = shootingTimer;
            numberOfShots += 1;
        }

        Animator.Play(gameObject, "Shoot", normalShootSpeed * speedMult);

        if (blaster != null)
        {
            blaster.Enable(true);
            Animator.Play(blaster, "Shoot", normalShootSpeed * speedMult);
        }

        UpdateAnimationSpd(normalShootSpeed * speedMult);

        PlayParticles(PARTICLES.MUZZLE);
        if (myAimbot != null)
        {
            myAimbot.isShooting = true;
            myAimbot.SearchForNewObjective();
        }
    }


    private void UpdateShooting()
    {
        if (myAimbot != null && myAimbot.HasObjective())
            myAimbot.RotateToObjective();
        else if (IsJoystickMoving() == true)
            RotatePlayer();

        UpdateAnimationSpd(normalShootSpeed * speedMult);
    }

    private void EndShooting()
    {
        if (myAimbot != null)
            myAimbot.isShooting = false;

        DisableBlaster();
    }

    private void DisableBlaster()
    {
        if (blaster != null)
            blaster.Enable(false);
    }

    private bool CanStopShooting()
    {
        //bool ret = false;
        //if (currFireRate <= baseFireRate * afterShootMult)
        //{
        //    ret = stopShootingTime > afterShootDelay;
        //}
        //else
        //{
        //    ret = stopShootingTime > afterShootDelay * (currFireRate / baseFireRate) * afterShootMult;
        //}

        return stopShootingTime > afterShootDelay;
    }

    private void StartShoot()
    {
        inputsList.Add(INPUT.IN_SHOOT_END);
        hasShot = true;
        if (shootPoint == null)
        {
            Debug.Log("Shootpoint reference is null!");
            return;
        }

        //Audio.StopAudio(gameObject);
        Audio.PlayAudio(shootPoint, "Play_Blaster_Shoot_Mando");
        Input.PlayHaptic(.3f, 10);
        if (hud != null)
        {
            hud.GetComponent<HUD>().ShootSwapImage(true);
        }
        GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/1606118587.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, shootPoint.transform.globalScale);
        if (bullet != null)
        {
            AddPrimaryHeat();

            bullet.GetComponent<BH_Bullet>().damage = bulletDamage * GetBlasterDamageMod();
            bullet.GetComponent<BH_Bullet>().SetEntity(this);
        }
    }

    private void StartGadgetCharge()
    {
        grenadeMaxLaunchTimer = grenadeLaunchMaxTime;
        grenadeLaunchMinTimer = grenadeMinLauncTime;
        canLaunchGrenade = false;
        playedThrowAudio = false;

        if (blaster != null)
        {
            blaster.Enable(false);
        }

        grenadeThrowAnimMult = (grenadeThrowAnimTime / grenadeLaunchMaxTime);
        grenadeThrowAnimTimer = grenadeThrowAnimTime / (grenadeThrowAnimMult * speedMult);

        Animator.Play(gameObject, "GrenadeThrow", grenadeThrowAnimMult * speedMult);
        UpdateAnimationSpd(grenadeThrowAnimMult * speedMult);

    }

    private void UpdateGadgetCharge()
    {
        PlayGrenadeThrowSound();

        UpdateAnimationSpd(grenadeThrowAnimMult * speedMult);
    }

    private void EndGadgetCharge()
    {
        //if(grenadeMaxLaunchTimer > 0f)
        //{
        //    float animTimeElapsed = grenadeThrowAnimTime - grenadeThrowAnimTimer;

        //    grenadeThrowAnimMult = 1f;

        //    grenadeThrowAnimTimer = (grenadeThrowAnimTime - animTimeElapsed) / (grenadeThrowAnimMult * speedMult);

        //    UpdateAnimationSpd(grenadeThrowAnimMult * speedMult);
        //    PlayGrenadeThrowSound();
        //}

    }

    private void StartGadgetShoot()
    {
        lastAction = ACTION.SHOOT_GRENADE;

        if (blaster != null)
        {
            blaster.Enable(false);
        }

        grenadesFireRateTimer = grenadesFireRate;

        if (HasStatus(STATUS_TYPE.REX_SEC_BLASTER) && GetStatusData(STATUS_TYPE.REX_SEC_BLASTER_SUBSKILL).severity < 20)
        {
            AddStatus(STATUS_TYPE.REX_SEC_BLASTER_SUBSKILL, STATUS_APPLY_TYPE.ADDITIVE, GetStatusData(STATUS_TYPE.REX_SEC_BLASTER).severity, 5);
        }

        grenade_reloading = true;
        if (HasStatus(STATUS_TYPE.CAD_BANE_BOOTS))
            GetStatusData(STATUS_TYPE.CAD_BANE_BOOTS).statChange = 1 + GetStatusData(STATUS_TYPE.CAD_BANE_BOOTS).severity / 100;

        ReducePrimaryWeaponHeat(onGrenadeHeatReduction);

        inputsList.Add(INPUT.IN_GADGET_SHOOT_END);
    }

    private void UpdateGadgetShoot()
    {
        UpdateAnimationSpd(speedMult);
    }

    private void EndGadgetShoot()
    {
        if (shootPoint == null)
        {
            Debug.Log("Shootpoint reference is null!");
            return;
        }

        if (blaster != null)
            blaster.Enable(false);

        Input.PlayHaptic(2f, 30);

        Vector3 baseScale = new Vector3(0.2f, 0.2f, 0.2f);

        Vector3 scale = baseScale;
        if (HasStatus(STATUS_TYPE.BOBBA_STUN_AMMO))
            scale *= (1 + GetStatusData(STATUS_TYPE.BOBBA_STUN_AMMO).severity / 100);

        float buttonPressedTimeNorm = 1f - (grenadeMaxLaunchTimer / grenadeLaunchMaxTime);
        buttonPressedTimeNorm *= buttonPressedTimeNorm;

        GameObject thrownGrenade = null;
        Vector3 forceDir = shootPoint.transform.GetForward();
        Quaternion grenadeRotation = shootPoint.transform.globalRotation;

        GameObject aimHelpTarget = GetAimbotObjective(5f, myAimbot.maxRange * buttonPressedTimeNorm);

        if (aimHelpTarget != null)
        {
            Vector3 direction = (aimHelpTarget.transform.globalPosition - shootPoint.transform.globalPosition).normalized;
            float angle = (float)Math.Atan2(direction.x, direction.z);

            if (Math.Abs(angle * Mathf.Rad2Deg) < 1.0f)
                return;

            grenadeRotation = Quaternion.Slerp(grenadeRotation, Quaternion.RotateAroundAxis(Vector3.up, angle), 1f);
        }

        Vector3 myForce = forceDir * Mathf.Lerp(minGrenadeForwardForceMult, maxGrenadeForwardForceMult, buttonPressedTimeNorm);
        myForce.y = Mathf.Lerp(minGrenadeUpForce, maxGrenadeUpForce, buttonPressedTimeNorm);

        grenadeMaxLaunchTimer = 0f;
        grenadeLaunchMinTimer = 0f;

        if (myGrenade1 == null)
        {
            thrownGrenade = myGrenade1 = InternalCalls.CreatePrefab("Library/Prefabs/660835192.prefab", shootPoint.transform.globalPosition + shootPoint.transform.GetForward() * 0.5f, shootPoint.transform.globalRotation, null);
        }
        else if (myGrenade1 != null && myGrenade1.IsEnabled() == false)
        {
            thrownGrenade = myGrenade1;
            myGrenade1.Enable(true);
        }
        else if (myGrenade1 != null && myGrenade1.IsEnabled() == true && myGrenade2 == null)
        {
            thrownGrenade = myGrenade2 = InternalCalls.CreatePrefab("Library/Prefabs/660835192.prefab", shootPoint.transform.globalPosition + shootPoint.transform.GetForward() * 0.5f, shootPoint.transform.globalRotation, null);
        }
        else if (myGrenade1 != null && myGrenade1.IsEnabled() == true && myGrenade2 != null && myGrenade2.IsEnabled() == false)
        {
            thrownGrenade = myGrenade2;
            myGrenade2.Enable(true);

        }
        else
        {
            Debug.Log("Mando needs more grenades!");
            return;
        }

        if (thrownGrenade != null)
        {
            SlowGrenade grenadeComp = thrownGrenade.GetComponent<SlowGrenade>();

            if (grenadeComp != null)
            {
                grenadeComp.GrenadeInit(shootPoint.transform.globalPosition + shootPoint.transform.GetForward().normalized * 1.5f, grenadeRotation, myForce);
            }

            thrownGrenade.transform.localScale = scale;
        }


    }

    public void IncreaseNormalShootDamage(float percent)
    {
        bulletDamage += (bulletDamage * percent);
    }

    #endregion

    #region SPECIAL SHOOT
    private void StartSecCharge()
    {
        chargeTimer = 0f;
        changeColorSniperTimer = 0f;
        canShootSniper = false;
        snipPrepareAnimTimer = snipPrepareAnimTime;
        Animator.Play(gameObject, "SniperShotP1", speedMult * sniperAimAnimMult);
        Audio.PlayAudio(gameObject, "Play_Sniper_Charge");
        if (rifle != null)
        {
            rifle.Enable(true);
            Animator.Play(rifle, "SniperShotP1", speedMult * sniperAimAnimMult);
        }

    }

    private void EndShootCharge()
    {
        chargeTimer = 0f;
        changeColorSniperTimer = 0f;

        PlayParticles(PARTICLES.SNIPER_CHARGE, true);
        Audio.StopOneAudio(gameObject, "Play_Sniper_Charge");

        if (rifle != null)
            rifle.Enable(false);

    }

    private void UpdateSecondaryShootCharge()
    {
        if (IsJoystickMoving() == true)
            RotatePlayer(0.25f);

        if (chargeTimer < timeToPerfectCharge)
        {
            currSniperLaserColor = defaultSniperLaserColor;

        }
        else if (chargeTimer > timeToPerfectCharge && chargeTimer < timeToPerfectChargeEnd * sniperShotIntervalModifier)
        {
            currSniperLaserColor = new Vector3(1, 1, 0);
        }
        else if (chargeTimer > timeToPerfectChargeEnd * sniperShotIntervalModifier)
        {
            changeColorSniperTimer += myDeltaTime;


            if (changeColorSniperTimer > 0f && changeColorSniperTimer < overheatTimeBeeping)
            {
                currSniperLaserColor = defaultSniperLaserColor;
            }
            else
            {
                currSniperLaserColor = new Vector3(1, 1, 1);

                if (changeColorSniperTimer > overheatTimeBeeping * 2)
                    changeColorSniperTimer = 0f;
            }

        }

        if (sniperShootPoint != null && myAimbot != null && canShootSniper == true)
        {
            float currAngle = Mathf.LerpAngle(sniperStartAngleRays, 0f, chargeTimer / timeToPerfectCharge) * Mathf.Deg2RRad;
            float rayWidth = Mathf.Lerp(7f, 3f, Math.Min(chargeTimer / timeToPerfectCharge, 1f));

            float hitDistanceRay1 = myAimbot.maxRange;
            float hitDistanceRay2 = myAimbot.maxRange;

            Vector3 sniperDir1 = Vector3.RotateAroundQuaternion(Quaternion.RotateAroundAxis(Vector3.up, currAngle), sniperShootPoint.transform.GetForward());
            Vector3 sniperDir2 = Vector3.RotateAroundQuaternion(Quaternion.RotateAroundAxis(Vector3.up, -currAngle), sniperShootPoint.transform.GetForward());

            InternalCalls.RayCast(sniperShootPoint.transform.globalPosition + (sniperShootPoint.transform.GetForward() * 1.5f), sniperDir1, myAimbot.maxRange * 2, ref hitDistanceRay1);
            InternalCalls.RayCast(sniperShootPoint.transform.globalPosition + (sniperShootPoint.transform.GetForward() * 1.5f), sniperDir2, myAimbot.maxRange * 2, ref hitDistanceRay2);
            //hitDistance = Math.Min(hitDistance, Mathf.Lerp(0, myAimbot.maxRange, chargeTimer / timeToPerfectCharge));

            InternalCalls.DrawRay(sniperShootPoint.transform.globalPosition, sniperShootPoint.transform.globalPosition + (sniperDir1 * hitDistanceRay1), currSniperLaserColor, rayWidth);
            InternalCalls.DrawRay(sniperShootPoint.transform.globalPosition, sniperShootPoint.transform.globalPosition + (sniperDir2 * hitDistanceRay2), currSniperLaserColor, rayWidth);
        }

        if (canShootSniper == true)
        {
            if (HasStatus(STATUS_TYPE.SP_NONCHARGED))
            {
                float modifier = 1 + GetStatusData(STATUS_TYPE.SP_CHARGE_TIME).severity / 100;
                chargeTimer += myDeltaTime * modifier;
            }
            else
            {
                chargeTimer += myDeltaTime;
            }

        }

    }

    private void StartSecondaryShoot()
    {
        bool perfectShot = false;
        //Animator.Play(gameObject, "Shoot", normalShootSpeed * speedMult);
        Animator.Play(gameObject, "SniperShotP2");
        snipShootAnimTimer = snipShootAnimTime;

        UpdateAnimationSpd(normalShootSpeed * speedMult);

        if (chargeTimer > timeToPerfectCharge && chargeTimer < timeToPerfectChargeEnd * sniperShotIntervalModifier)
        {
            perfectShot = true;
            Debug.Log("Frame Perfect Charge!");
        }

        ShootChargeShot(perfectShot, chargeTimer);
    }

    private void ShootChargeShot(bool perfectShot, float chargeTimer)
    {
        inputsList.Add(INPUT.IN_SEC_SHOOT_END);
        --currentBullets;
        sniperRechargeTimer += bulletRechargeTime;
        updateSniper = true;
        if (shootPoint == null)
        {
            Debug.Log("Shootpoint reference is null!");
            return;
        }

        //Audio.StopAudio(gameObject);
        Audio.PlayAudio(shootPoint, "Play_Sniper_Shoot_Mando");
        Input.PlayHaptic(.5f, 10);

        GameObject aimHelpTarget = GetAimbotObjective(15f);
        PlayParticles(PARTICLES.SNIPER_CHARGE, true);
        PlayParticles(PARTICLES.SNIPER_MUZZEL);
        snipercharge = true;
        GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/739906161.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, null);
        if (bullet != null)
        {
            //Debug.Log("Charged Bullet Shot!");
            ReducePrimaryWeaponHeat(onSniperHeatReduction);

            ChargedBullet chrBulletComp = bullet.GetComponent<ChargedBullet>();

            if (chrBulletComp != null)
            {
                float sniperBulletDamage = chargedBulletDmg;

                if (perfectShot == true)
                {
                    sniperBulletDamage *= perfectShotDmgMult;
                    bullet.transform.localScale *= perfectShotDmgMult;
                    if (HasStatus(STATUS_TYPE.SP_DAMAGE_CHARGED))
                    {
                        float modifier = 1 + GetStatusData(STATUS_TYPE.SP_DAMAGE_CHARGED).severity / 100;
                        sniperBulletDamage *= modifier;
                    }
                }
                else if (chargeTimer > timeToPerfectChargeEnd * sniperShotIntervalModifier)
                {
                    sniperBulletDamage = (chargedBulletDmg * overShotDmgMult);
                }
                else
                {
                    float dmgMult = Mathf.InvLerp(0.0f, timeToPerfectCharge, chargeTimer);

                    dmgMult = Math.Max(dmgMult, 0.15f);
                    sniperBulletDamage = (chargedBulletDmg * dmgMult);

                    sniperBulletDamage = Math.Max(sniperBulletDamage, 1f);

                    if (HasStatus(STATUS_TYPE.SP_NONCHARGED))
                    {
                        float modifier = 1 + GetStatusData(STATUS_TYPE.SP_NONCHARGED).severity / 100;
                        sniperBulletDamage *= modifier;
                    }
                }
                Debug.Log("Charge Bullet dmg: " + sniperBulletDamage.ToString());

                chrBulletComp.InitChargedBullet(sniperBulletDamage * GetSniperDamageMod());

                if (aimHelpTarget != null)
                {
                    chrBulletComp.SetTarget(aimHelpTarget.transform.globalPosition);
                }

            }


        }
    }

    public void RefillSniper()
    {
        sniperRechargeTimer -= bulletRechargeTime;
        if (sniperRechargeTimer <= 0f)
            sniperRechargeTimer = 0f;
    }
    #endregion

    #region DASH
    private void StartDash()
    {
        //Audio.StopAudio(gameObject);
        Audio.PlayAudio(gameObject, "Play_Dash");
        Animator.Play(gameObject, "Dash", speedMult);
        hasDashed = true;
        DisableBlaster();
        UpdateAnimationSpd(speedMult);

        dashTimer = dashDuration;
        dashStartYPos = gameObject.transform.localPosition.y;

        preDashRotation = this.gameObject.transform.localRotation;

        if (IsJoystickMoving() == true)
        {
            dashDirection = WorldDirFromGamepadInput().normalized;
            RotatePlayer();
        }
        else
        {
            dashDirection = this.gameObject.transform.GetForward();
        }

        PlayParticles(PARTICLES.JETPACK);
        if (HasStatus(STATUS_TYPE.GRENADE_ON_DASH) && grenade_onDash >= GetStatusData(STATUS_TYPE.GRENADE_ON_DASH).severity)
        {
            grenade_onDash = 0;

            if (shootPoint == null)
            {
                Debug.Log("Shootpoint reference is null!");
                return;
            }

            Quaternion rotation = Quaternion.RotateAroundAxis(Vector3.up, 3.14159f);
            Input.PlayHaptic(2f, 30);
            Vector3 scale = new Vector3(0.2f, 0.2f, 0.2f);
            Vector3 pos = new Vector3(shootPoint.transform.globalPosition.x, shootPoint.transform.globalPosition.y - 1f, shootPoint.transform.globalPosition.z - 0.8f);
            GameObject thrownGrenade = InternalCalls.CreatePrefab("Library/Prefabs/660835192.prefab", pos, shootPoint.transform.globalRotation * rotation, scale);


            if (thrownGrenade != null)
            {
                SlowGrenade grenadeComp = thrownGrenade.GetComponent<SlowGrenade>();

                if (grenadeComp != null)
                {
                    grenadeComp.GrenadeInit(pos, shootPoint.transform.globalRotation * rotation, Vector3.zero, true);
                }

            }

        }


    }

    private void UpdateDash()
    {
        StopPlayer();
        //gameObject.AddForce(gameObject.transform.GetForward().normalized * dashSpeed);
        //PlayParticles(PARTICLES.JETPACK);
        UpdateAnimationSpd(speedMult);

        gameObject.transform.localPosition = gameObject.transform.localPosition + dashDirection * dashSpeed * myDeltaTime;
        distanceMoved += dashSpeed * myDeltaTime;
    }

    private void EndDash()
    {
        dashCDTimer = dashCD;
        betweenDashesCDTimer = betweeMultiplenDashesCD;
        dashAvailable = false;
        --currentDashes;
        timeSinceLastDash = 0.01f;
        //Debug.Log(dashCDTimer.ToString());
        //Debug.Log(dashAvaliable.ToString());

        StopPlayer();
        gameObject.transform.localPosition.y = dashStartYPos;
        this.gameObject.transform.localRotation = preDashRotation;

        //if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY_DAMAGE_REDUCTION_DASH))
        //{
        //    skill_damageReductionDashActive = true;
        //    skill_damageReductionDashTimer = Skill_Tree_Data.GetMandoSkillTree().U4_seconds;
        //}

        PlayParticles(PARTICLES.JETPACK, true);
    }

    #endregion

    #region DIE
    private void StartDead()
    {
        dieTimer = dieTime;
        Animator.Play(gameObject, "Die");
        Audio.PlayAudio(gameObject, "Play_Mando_Death");
        Audio.SetState("Player_State", "Dead");
        HUD hudComp = hud.GetComponent<HUD>();

        if (hudComp != null && hudComp.gameObject != null)
        {
            hudComp.gameObject.Enable(false);
        }


        if (rifle != null)
            rifle.Enable(false);

        if (blaster != null)
            blaster.Enable(false);
    }
    private void UpdateDead()
    { }

    private void EndDead()
    {
        BlackFade.StartFadeIn(gameObject.GetComponent<PlayerHealth>().Die);
    }

    private void DeadInput()
    {
        inputsList.Add(INPUT.IN_DEAD);
    }
    #endregion

    #region MOVE AND ROTATE PLAYER
    private void StartMove()
    {
        Animator.Play(gameObject, "Run", speedMult);
        UpdateAnimationSpd(speedMult);

        PlayFootstepsAudio();
    }

    private void UpdateMove()
    {
        UpdateAnimationSpd(speedMult);

        footStepTimer += myDeltaTime;

        if (footStepTimer > 0.33f)
        {
            //Audio.StopAudio(gameObject);

            PlayFootstepsAudio();

            footStepTimer = 0f;
        }


        RotatePlayer();
        dustTime += myDeltaTime;
        if (dustTime >= runTime)
        {
            PlayParticles(PARTICLES.DUST);
            dustTime = 0;
        }

        //gameObject.SetVelocity(gameObject.transform.GetForward() * movementSpeed);
        gameObject.transform.localPosition = gameObject.transform.localPosition + gameObject.transform.GetForward().normalized * movementSpeed * MovspeedMult * myDeltaTime;
        distanceMoved += movementSpeed * myDeltaTime;
    }
    private void MoveEnd()
    {
        //Audio.StopAudio(gameObject);
    }

    private void StopPlayer()
    {
        // Debug.Log("Stoping");
        gameObject.SetVelocity(new Vector3(0, 0, 0));
    }

    private void RotatePlayer(float slerpValue = 1f)
    {
        //Calculate player rotation
        Vector3 aX = new Vector3(gamepadInput.x, 0, (gamepadInput.y + 1) * -1);
        Vector3 aY = new Vector3(0, 0, 1);
        aX = Vector3.Normalize(aX);
        if (aX == Vector3.zero)
            return;

        if (aX.x >= 0)
        {
            angle = Math.Acos(Vector3.Dot(aX, aY));
        }
        else if (aX.x < 0)
        {
            angle = -Math.Acos(Vector3.Dot(aX, aY));
        }

        //Convert angle from world view to orthogonal view
        angle += 0.785398f; //Rotate 45 degrees to the right

        gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, Quaternion.RotateAroundAxis(Vector3.up, (float)-angle), slerpValue);

    }
    #endregion

    #region UTILITIES
    //returns vector direction in world space from the gamepad input, or Vector3.zero if gamepad joystick is idle
    public Vector3 WorldDirFromGamepadInput()
    {
        //Calculate player rotation
        Vector3 aX = new Vector3(gamepadInput.x, 0, (gamepadInput.y + 1) * -1);
        Vector3 aY = new Vector3(0, 0, 1);
        aX = Vector3.Normalize(aX);
        if (aX == Vector3.zero)
            return Vector3.zero;

        if (aX.x >= 0)
        {
            angle = Math.Acos(Vector3.Dot(aX, aY));
        }
        else if (aX.x < 0)
        {
            angle = -Math.Acos(Vector3.Dot(aX, aY));
        }

        //Convert angle from world view to orthogonal view
        angle += 0.785398f; //Rotate 45 degrees to the right

        Quaternion rotation = Quaternion.RotateAroundAxis(Vector3.up, (float)-angle);

        return (rotation * aY);

    }

    private void UpdateAnimationSpd(float newSpd)
    {
        if (currAnimationPlaySpd != newSpd)
            Animator.SetSpeed(gameObject, newSpd);
    }

    private void PlayFootstepsAudio()
    {
        Audio.StopOneAudio(this.gameObject, lastFootsteps);
        if (floorType == FLOOR_TYPE.STONE)
        {
            Audio.PlayAudio(this.gameObject, "Play_Footsteps_Stone_Mando");
            lastFootsteps = "Play_Footsteps_Stone_Mando";
        }
        else if (floorType == FLOOR_TYPE.SAND)
        {
            Audio.PlayAudio(this.gameObject, "Play_Footsteps_Sand_Mando");
            lastFootsteps = "Play_Footsteps_Sand_Mando";
        }
        else if (floorType == FLOOR_TYPE.METAL)
        {
            Audio.PlayAudio(this.gameObject, "Play_Footsteps_Metal_Platform_Mando");
            lastFootsteps = "Play_Footsteps_Metal_Platform_Mando";
        }
        else if (floorType == FLOOR_TYPE.SNOW)
        {
            Audio.PlayAudio(this.gameObject, "Play_Footsteps_Snow_Mando");
            lastFootsteps = "Play_Footsteps_Snow_Mando";
        }
        else if (floorType == FLOOR_TYPE.WATER)
        {
            Audio.PlayAudio(this.gameObject, "Play_Footsteps_Water_Mando");
            lastFootsteps = "Play_Footsteps_Water_Mando";
        }

    }

    private void PlayGrenadeThrowSound()
    {
        if (grenadeThrowAnimTimer <= grenadeThrowAnimTime * 0.5f && playedThrowAudio == false)
        {
            playedThrowAudio = true;
            Audio.PlayAudio(gameObject, "Play_Mando_Grenade_Throw");
        }
    }

    private void UpdateControllerInputs()
    {
        //Check if user is moving joystick
        //if (IsJoystickMoving() == true)
        //{
        verticalInput = Input.GetLeftAxisX();
        horizontalInput = Input.GetLeftAxisY();

        gamepadInput = new Vector3(horizontalInput, -verticalInput, 0f);
        //}

        //else
        //gamepadInput = new Vector3(0f, 0f, 0f);

        if (Input.GetGamepadButton(DEControllerButton.START) == KeyState.KEY_DOWN && startAvailable)
        {
            Audio.StopAudio(gameObject);

            if (pause != null)
            {
                pause.EnableNav(true);
                Audio.PlayAudio(gameObject, "Play_UI_Pause");
                pause.GetComponent<Pause>().DisplayBoons();
                pause.GetComponent<Pause>().HideShop();
                background.Enable(true);
                Time.PauseGame();
            }

            else
                Debug.Log("Need to add pause GO");
        }
    }

    private bool IsJoystickMoving()
    {
        return gamepadInput.magnitude > deathZone;
    }

    private float GetCurrentFireRate()
    {
        float ret = baseFireRate;

        float currentHeat = hud.GetComponent<HUD>().GetPrimaryHeat();

        if (currentHeat <= 0f)
            currentHeat = 0.01f;

        ret = (currentHeat * (baseFireRate * fireRateMultCap)) / hud.GetComponent<HUD>().GetPrimaryMaxHeat();


        //Debug.Log("Firerate: " + ret.ToString());

        ret = Math.Min(ret, baseFireRate * fireRateMultCap * 0.5f);
        ret *= FireRateMult;
        ret = Math.Max(ret, baseFireRate * 0.8f);



        return ret;
    }

    private void AddPrimaryHeat()
    {
        if (hud != null)
            hud.GetComponent<HUD>().AddPrimaryHeatShot();
    }

    private void ReducePrimaryWeaponHeat(float stateMult = 0f)
    {
        float newValue = heatReductionSpeed * myDeltaTime;
        float refreshMult = 1.0f + stateMult;

        if (hud != null)
            hud.GetComponent<HUD>().ReducePrimaryHeat(newValue * refreshMult);

    }

    private void UpdateSecondaryShotAmmo()
    {
        if (sniperRechargeTimer >= 0f || currentBullets < numberOfBullets)
        {
            sniperRechargeTimer -= myDeltaTime;

            //TODO: This works with just 2 bullets
            if (sniperBullet2 != null)
            {
                float numberToUpdate = sniperRechargeTimer < bulletRechargeTime ? bulletRechargeTime : 1 / Math.Abs(bulletRechargeTime - sniperRechargeTimer);

                sniperBullet2.SetFloatUniform("currentBulletCooldown", numberToUpdate);
                sniperBullet2.SetFloatUniform("maxBulletCooldown", bulletRechargeTime);
            }
            if (sniperBullet1 != null)
            {

                float numberToUpdate = sniperRechargeTimer <= 0 ? bulletRechargeTime : bulletRechargeTime - sniperRechargeTimer;

                if (sniperRechargeTimer > bulletRechargeTime)
                    numberToUpdate = 0f;

                sniperBullet1.SetFloatUniform("currentBulletCooldown", numberToUpdate);
                sniperBullet1.SetFloatUniform("maxBulletCooldown", bulletRechargeTime);
            }

            if (sniperRechargeTimer <= ((numberOfBullets - currentBullets - 1) * bulletRechargeTime))
            {
                currentBullets++;
                PlayParticles(PARTICLES.SNIPER);
                if (updateSniper)
                    Audio.PlayAudio(secSound, "Play_Sniper_Cooldown_Finish");
            }


        }

    }

    public bool IsDashing()
    {
        return currentState == STATE.DASH;
    }

    public float GetSniperMaxDamage()
    {
        return chargedBulletDmg;
    }

    //Angles in deg, returns null if nothing is found
    public GameObject GetAimbotObjective(float angle, float range = 0f)
    {
        GameObject ret = null;

        if (myAimbot != null)
        {
            float searchRange = range == 0f ? myAimbot.maxRange : range;

            ret = myAimbot.SearchForNewObjRaw(angle, searchRange);
        }

        return ret;
    }

    public void ReduceComboOnHit(int hitDamage, float comboSubstractionMult = 1.5f)
    {
        if (hud != null)
        {
            HUD myHud = hud.GetComponent<HUD>();

            if (myHud != null)
                myHud.SubstractToCombo(hitDamage * comboSubstractionMult);

        }
    }

    #endregion


    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        //Debug.Log("CS: Collided object: " + gameObject.tag + ", Collider: " + collidedGameObject.tag);
        //Debug.Log("Collided by tag: " + collidedGameObject.tag);

        if (currentState != STATE.DASH)
        {
            if (collidedGameObject.CompareTag("StormTrooperBullet"))
            {
                //InternalCalls.Destroy(gameObject);
                PlayParticles(PARTICLES.IMPACT);
                BH_Bullet bulletScript = collidedGameObject.GetComponent<BH_Bullet>();
                Audio.PlayAudio(gameObject, "Play_Mando_Hit");

                if (bulletScript != null)
                {
                    int damageFromBullet = 0;

                    if (skill_damageReductionDashActive)
                        damageFromBullet = (int)(bulletScript.damage * (1.0f/* - Skill_Tree_Data.GetMandoSkillTree().U4_damageReduction*/));
                    else
                        damageFromBullet = (int)bulletScript.damage;

                    PlayerHealth healthScript = gameObject.GetComponent<PlayerHealth>();

                    if (healthScript != null)
                        healthScript.TakeDamage(damageFromBullet);

                    damageTaken += damageFromBullet;
                }
            }
            if (collidedGameObject.CompareTag("DeathTrooperBullet"))
            {
                //InternalCalls.Destroy(gameObject);
                PlayParticles(PARTICLES.IMPACT);
                DeathTrooperBullet bulletScript = collidedGameObject.GetComponent<DeathTrooperBullet>();
                Audio.PlayAudio(gameObject, "Play_Mando_Hit");

                if (bulletScript != null)
                {
                    int damageFromBullet = 0;

                    if (skill_damageReductionDashActive)
                        damageFromBullet = (int)(bulletScript.damage * (1.0f/* - Skill_Tree_Data.GetMandoSkillTree().U4_damageReduction*/));
                    else
                        damageFromBullet = (int)bulletScript.damage;

                    PlayerHealth healthScript = gameObject.GetComponent<PlayerHealth>();

                    if (healthScript != null)
                        healthScript.TakeDamage(damageFromBullet);

                    damageTaken += damageFromBullet;
                }
            }
            if (collidedGameObject.CompareTag("Bantha"))
            {
                //InternalCalls.Destroy(gameObject);
                Audio.PlayAudio(gameObject, "Play_Mando_Ramming_Hit");

                Enemy enemy = collidedGameObject.GetComponent<Enemy>();

                if (enemy != null)
                {
                    float damage = enemy.damage;

                    if (damage != 0)
                    {
                        int damageFromEnemy = 0;
                        if (skill_damageReductionDashActive)
                            damageFromEnemy = (int)(damage * (1.0f /*- Skill_Tree_Data.GetMandoSkillTree().U4_damageReduction*/));
                        else
                            damageFromEnemy = (int)damage;

                        PlayerHealth playerHealth = gameObject.GetComponent<PlayerHealth>();
                        if (playerHealth != null)
                            playerHealth.TakeDamage(damageFromEnemy);

                        damageTaken += damageFromEnemy;
                    }
                }
            }
            else if (collidedGameObject.CompareTag("ExplosiveBarrel"))
            {
                SphereCollider sphereColl = collidedGameObject.GetComponent<SphereCollider>();

                if (sphereColl != null)
                {
                    if (sphereColl.active)
                        gameObject.GetComponent<PlayerHealth>().TakeDamage((int)(collidedGameObject.GetComponent<BH_DestructBox>().explosion_damage * 0.5f));
                }
            }
            else if (collidedGameObject.CompareTag("MoffSwing"))
            {
                MoffSwing swing = collidedGameObject.GetComponent<MoffSwing>();

                Debug.Log("Collided w/ Moff Swing!");

                if (swing != null)
                {
                    float damage = swing.damage;

                    if (damage != 0 && swing.hasCollided == false)
                    {
                        Audio.PlayAudio(gameObject, "Play_Mando_Hit");

                        swing.hasCollided = true;

                        int damageFromEnemy = (int)damage;
                        PlayerHealth playerHealth = gameObject.GetComponent<PlayerHealth>();
                        if (playerHealth != null)
                            playerHealth.TakeDamage(damageFromEnemy);

                        damageTaken += damageFromEnemy;
                    }
                }
            }
            else if (collidedGameObject.CompareTag("MoffBullet"))
            {
                PlayParticles(PARTICLES.IMPACT);
                BH_Bullet bulletScript = collidedGameObject.GetComponent<BH_Bullet>();
                Audio.PlayAudio(gameObject, "Play_Mando_Hit");

                if (bulletScript != null)
                {
                    int damageFromBullet = 0;

                    if (skill_damageReductionDashActive)
                        damageFromBullet = (int)(bulletScript.damage * (1.0f/* - Skill_Tree_Data.GetMandoSkillTree().U4_damageReduction*/));
                    else
                        damageFromBullet = (int)bulletScript.damage;

                    PlayerHealth healthScript = gameObject.GetComponent<PlayerHealth>();

                    if (healthScript != null)
                        healthScript.TakeDamage(damageFromBullet);

                    damageTaken += damageFromBullet;
                }
            }
            else if (collidedGameObject.CompareTag("WaterFloor"))
            {
                floorType = FLOOR_TYPE.WATER;
            }
            else if (collidedGameObject.CompareTag("MetalFloor"))
            {
                floorType = FLOOR_TYPE.METAL;
            }
            else if (collidedGameObject.CompareTag("SnowFloor"))
            {
                floorType = FLOOR_TYPE.SNOW;
            }
            else if (collidedGameObject.CompareTag("StoneFloor"))
            {
                floorType = FLOOR_TYPE.STONE;
            }
            else if (collidedGameObject.CompareTag("SandFloor"))
            {
                floorType = FLOOR_TYPE.SAND;
            }
        }
    }

    public void OnCollisionStay(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("MoffSwing"))
        {
            MoffSwing swing = collidedGameObject.GetComponent<MoffSwing>();

            Debug.Log("Collided STAY w/ Moff Swing!");

            if (swing != null)
            {
                float damage = swing.damage;

                if (damage != 0 && swing.hasCollided == false)
                {
                    Audio.PlayAudio(gameObject, "Play_Mando_Hit");

                    swing.hasCollided = true;

                    int damageFromEnemy = (int)damage;
                    PlayerHealth playerHealth = gameObject.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                        playerHealth.TakeDamage(damageFromEnemy);

                    damageTaken += damageFromEnemy;
                }
            }
        }
    }


    public void OnTriggerEnter(GameObject triggeredGameObject)
    {

        if (triggeredGameObject.CompareTag("Zone1"))
        {
            tuto_state = TUTO_STATES.DASH;
            BabyYoda babyYoda = GetBabyYoda();
            if (babyYoda != null)
                babyYoda.tutoState = BabyYoda.TUTO_YODA_STATES.LOCKED;
            Debug.Log("Entering Zone 1");
        }

        if (triggeredGameObject.CompareTag("Zone2"))
        {
            Zone2();
        }

        if (triggeredGameObject.CompareTag("Zone3"))
        {
            tuto_state = TUTO_STATES.PUSH;
            BabyYoda babyYoda = GetBabyYoda();
            if (babyYoda != null)
                babyYoda.tutoState = BabyYoda.TUTO_YODA_STATES.PUSH;
            Debug.Log("Entering Zone 3");
        }

        if (triggeredGameObject.CompareTag("Zone4"))
        {
            tuto_state = TUTO_STATES.SNIPER;
            BabyYoda babyYoda = GetBabyYoda();
            if (babyYoda != null)
                babyYoda.tutoState = BabyYoda.TUTO_YODA_STATES.LOCKED;
            Debug.Log("Entering Zone 4");
        }

        if (triggeredGameObject.CompareTag("Zone5"))
        {
            tuto_state = TUTO_STATES.WALL;
            BabyYoda babyYoda = GetBabyYoda();
            if (babyYoda != null)
                babyYoda.tutoState = BabyYoda.TUTO_YODA_STATES.WALL;
            Debug.Log("Entering Zone 5");
        }

        if (triggeredGameObject.CompareTag("Zone6"))
        {
            tuto_state = TUTO_STATES.NONE;
            BabyYoda babyYoda = GetBabyYoda();
            if (babyYoda != null)
                babyYoda.tutoState = BabyYoda.TUTO_YODA_STATES.NONE;
            Debug.Log("Entering Zone 6");
        }
    }

    private BabyYoda GetBabyYoda()
    {
        GameObject grogu = gameObject.parent.GetChild("Grogu");
        BabyYoda babyYoda = null;
        if (grogu != null)
            babyYoda = grogu.GetComponent<BabyYoda>();
        return babyYoda;
    }

    public void GetCoin()
    {
        PlayerResources.AddRunCoins(10);
        Audio.PlayAudio(gameObject, "Play_UI_Credit_Pickup");

        if (hud != null)
        {
            HUD hudComponent = hud.GetComponent<HUD>();

            if (hudComponent != null)
                hudComponent.UpdateCurrency(PlayerResources.GetRunCoins());
        }
    }

    public void RespawnOnFall()
    {

        gameObject.transform.localPosition = mySpawnPos;
        PlayerHealth myHealth = gameObject.GetComponent<PlayerHealth>();
        if (myHealth != null)
        {
            int finalHealthSubstract = 0;
            if (HasStatus(STATUS_TYPE.FALL))
            {
                finalHealthSubstract = (int)(fallDamage / 2);
            }
            else
                finalHealthSubstract = (int)(fallDamage);

            if (PlayerHealth.currHealth - finalHealthSubstract <= 0)
            {
                finalHealthSubstract = PlayerHealth.currHealth - 1;
            }

            myHealth.TakeDamage(finalHealthSubstract, true); //TODO whats the right amount we have to substract?
            damageTaken += finalHealthSubstract;
            timesFellOfMap += 1;

        }
    }

    private void PlayParticles(PARTICLES particletype, bool stopParticle = false)
    {
        PlayerParticles myParticles = gameObject.GetComponent<PlayerParticles>();
        ParticleSystem particle = null;

        switch (particletype)
        {
            case PARTICLES.NONE:
                break;
            case PARTICLES.DUST:
                if (myParticles != null)
                {
                    particle = myParticles.dust;
                    if (particle != null)
                    {
                        if (stopParticle == false)
                            particle.Play();
                        else
                            particle.Stop();
                    }
                    else
                        Debug.Log("Jetpack particle not found");
                }
                else
                    Debug.Log("Component Particles not found");
                break;

            case PARTICLES.IMPACT:
                if (myParticles != null)
                {
                    particle = myParticles.impact;
                    if (particle != null)
                    {
                        if (stopParticle == false)
                            particle.Play();
                        else
                            particle.Stop();
                    }
                    //else
                    //    Debug.Log("Jetpack particle not found");
                }
                else
                    Debug.Log("Component Particles not found");
                break;

            case PARTICLES.JETPACK:
                if (myParticles != null)
                {
                    particle = myParticles.jetpack;
                    if (particle != null)
                    {
                        if (stopParticle == false)
                            particle.Play();
                        else
                            particle.Stop();
                    }
                    else
                        Debug.Log("Jetpack Rush particle not found!");

                }
                else
                    Debug.Log("Component Particles not found");
                break;

            case PARTICLES.MUZZLE:
                if (myParticles != null)
                {
                    particle = myParticles.muzzle;
                    if (particle != null)
                    {
                        if (stopParticle == false)
                            particle.Play();
                        else
                            particle.Stop();
                    }
                    else
                        Debug.Log("Jetpack particle not found");
                }
                else
                    Debug.Log("Component Particles not found");
                break;
            case PARTICLES.GRENADE:
                if (myParticles != null)
                {
                    particle = myParticles.grenade;
                    if (particle != null)
                    {
                        if (stopParticle == false)
                            particle.Play();
                        else
                            particle.Stop();
                    }
                    else
                        Debug.Log("Grenade particle not found");
                }
                else
                    Debug.Log("Component Particles not found");
                break;
            case PARTICLES.SNIPER:
                if (myParticles != null)
                {
                    particle = myParticles.sniper;
                    if (particle != null)
                    {
                        if (stopParticle == false)
                            particle.Play();
                        else
                            particle.Stop();
                    }
                    else
                        Debug.Log("Sniper particle not found");
                }
                else
                    Debug.Log("Component Particles not found");
                break;
            case PARTICLES.HEAL:
                if (myParticles != null)
                {
                    particle = myParticles.heal;
                    if (particle != null)
                    {
                        if (stopParticle == false)
                            particle.Play();
                        else
                            particle.Stop();
                    }
                    else
                        Debug.Log("Heal particle not found");
                }
                else
                    Debug.Log("Component Particles not found");
                break;
            case PARTICLES.SNIPER_CHARGE:
                if (myParticles != null)
                {
                    particle = myParticles.sniperCharge;
                    if (particle != null)
                    {
                        if (stopParticle == false)
                            particle.Play();
                        else
                            particle.Stop();
                    }
                    else
                        Debug.Log("Sniper charge particle not found");
                }
                else
                    Debug.Log("Component Particles not found");
                break;
            case PARTICLES.SNIPER_MUZZEL:
                if (myParticles != null)
                {
                    particle = myParticles.sniperMuzzel;
                    if (particle != null)
                    {
                        if (stopParticle == false)
                            particle.Play();
                        else
                            particle.Stop();
                    }
                    else
                        Debug.Log("Sniper muzzel particle not found");
                }
                else
                    Debug.Log("Component Particles not found");
                break;
        }
    }

    /*public void SetSkill(int skillTree, int SkillNumber)
    {
        if (skillTree == (int)Skill_Tree_Data.SkillTreesNames.MANDO)
        {
            if (SkillNumber == (int)Skill_Tree_Data.MandoSkillNames.UTILITY_INCREASE_DAMAGE_WHEN_GROGU)
            {
                skill_groguIncreaseDamageActive = true;
                skill_groguIncreaseDamageTimer = Skill_Tree_Data.GetMandoSkillTree().U3_duration;
            }
        }
    }*/

    public float GetBlasterDamageMod()
    {
        //We apply modifications to the damage based on the skill actives in the talent tree
        float supercharged = 1;
        if (HasStatus(STATUS_TYPE.PRIM_CHARGED))
        {
            Random rand = new Random();
            float result = rand.Next(1, 101);
            if (result <= GetStatusData(STATUS_TYPE.PRIM_CHARGED).severity)
                supercharged = 2;
        }
        float ChadBaneModifier = 1;
        float AfterDashModifier = 1;

        if (lastAction != ACTION.SHOOT_BLASTER)
        {
            lastAction = ACTION.SHOOT_BLASTER;

            if (HasStatus(STATUS_TYPE.REX_SEC_BLASTER) && GetStatusData(STATUS_TYPE.REX_SEC_BLASTER_SUBSKILL).severity < 20)
            {
                AddStatus(STATUS_TYPE.REX_SEC_BLASTER_SUBSKILL, STATUS_APPLY_TYPE.ADDITIVE, GetStatusData(STATUS_TYPE.REX_SEC_BLASTER).severity, 5);
            }

            if (HasStatus(STATUS_TYPE.CAD_BANE_SOH))
            {
                ChadBaneModifier = 1 + GetStatusData(STATUS_TYPE.CAD_BANE_SOH).severity / 100;
                Debug.Log("Chad Bane Mod = " + ChadBaneModifier.ToString());
            }
        }

        float QuickDraw = 1;
        if (hasDashed)
        {
            hasDashed = false;
            AfterDashModifier += 0.25f;
            if (HasStatus(STATUS_TYPE.MANDO_QUICK_DRAW))
                QuickDraw += GetStatusData(STATUS_TYPE.MANDO_QUICK_DRAW).severity / 100;
        }


        float Damage = BlasterDamageMult * BlasterDamagePerHpMult * DamagePerHeatMult * RawDamageMult * supercharged * ChadBaneModifier * AfterDashModifier * QuickDraw;
        if (HasStatus(STATUS_TYPE.BLAST_CANNON))
        {
            if (hud != null)
            {
                float currHeat = hud.GetComponent<HUD>().GetPrimaryHeat();
                float maxHeat = hud.GetComponent<HUD>().GetPrimaryMaxHeat();
              //  Debug.Log(currHeat.ToString() + " " + maxHeat.ToString());
                if (currHeat / maxHeat >= 0.75f)
                {
                    Damage *= 2;

                }

            }
        }


        //if (skill_groguIncreaseDamageActive)
        //{
        //    damageWithSkills *= (1.0f /*+ Skill_Tree_Data.GetMandoSkillTree().U3_increasedDamagePercentage*/);
        //}

        //if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION_EXTRA_DAMAGE_LOW_HEALTH))
        //{
        //    float HPMissing = 1.0f - ((float)PlayerHealth.currHealth / (float)PlayerHealth.currMaxHealth);

        //    float local_damageAmount = Skill_Tree_Data.GetMandoSkillTree().A7_extraDamageAmount;
        //    float local_HPStep = Skill_Tree_Data.GetMandoSkillTree().A7_extraDamageHPStep;
        //    damageWithSkills += (damageWithSkills * local_damageAmount * (HPMissing / local_HPStep));
        //}
        return Damage;
    }
    public float GetSniperDamageMod()
    {
        //We apply modifications to the damage based on the skill actives in the talent tree
        float ChadBaneModifier = 1;
        if (lastAction != ACTION.SHOOT_SNIPER)
        {
            lastAction = ACTION.SHOOT_SNIPER;
            if (HasStatus(STATUS_TYPE.REX_SEC_BLASTER) && GetStatusData(STATUS_TYPE.REX_SEC_BLASTER_SUBSKILL).severity < 20)
            {
                AddStatus(STATUS_TYPE.REX_SEC_BLASTER_SUBSKILL, STATUS_APPLY_TYPE.ADDITIVE, GetStatusData(STATUS_TYPE.REX_SEC_BLASTER).severity, 5);
            }
            if (HasStatus(STATUS_TYPE.CAD_BANE_SOH))
            {
                ChadBaneModifier = 1 + GetStatusData(STATUS_TYPE.CAD_BANE_SOH).severity / 100;
                Debug.Log("Chad Bane Mod = " + ChadBaneModifier.ToString());
            }

        }

        float Damage = SniperDamageMult * SniperDamagePerHpMult * DamagePerHeatMult * RawDamageMult * ChadBaneModifier * luckyMod;
        luckyMod = 1;
        return Damage;
    }
    public float GetGrenadeDamageMod()
    {
        //We apply modifications to the damage based on the skill actives in the talent tree

        float Damage = DamagePerHeatMult * GrenadeDamageMult * RawDamageMult + SecTickDamage;

        return Damage;
    }
    public void LockInputs(bool locked)
    {
        if (locked)
        {
            lockInputs = true;
            inputsList.Add(INPUT.IN_IDLE);
        }
        else
        {
            lockInputs = false;
        }
    }

    public void OnApplicationQuit()
    {
        bulletDamage = bulletDamageDefault;
    }

    public STATE GetSate()
    {
        return currentState;
    }

    public void SaveBuffs()
    {
        Debug.Log("SAVE STATUSES");
        copyBuffs(ref PlayerStatuses);
    }

    public void LoadBuffs()
    {
        Debug.Log("LOAD STATUSES");
        Debug.Log(PlayerStatuses.Count.ToString());
        var mapKeys = PlayerStatuses.Keys;
        foreach (STATUS_TYPE statusType in mapKeys)
        {
            if (PlayerStatuses.ContainsKey(statusType) == false)
                continue;
            Debug.Log(PlayerStatuses[statusType].statusType.ToString());
            Debug.Log(PlayerStatuses[statusType].applyType.ToString());
            Debug.Log(PlayerStatuses[statusType].severity.ToString());
            Debug.Log(PlayerStatuses[statusType].remainingTime.ToString());
            Debug.Log(PlayerStatuses[statusType].isPermanent.ToString());

            AddStatus(PlayerStatuses[statusType].statusType, PlayerStatuses[statusType].applyType, PlayerStatuses[statusType].severity, 1, PlayerStatuses[statusType].isPermanent);

        }
    }
    public void RemoveBoons()
    {
        Debug.Log("REMOVING BOONS" + boons.Count.ToString());
        for (int i = 0; i < boons.Count; i++)
        {
            Debug.Log(boons[i].ToString());
            PlayerStatuses.Remove(boons[i]);
            boons.RemoveAt(i);
            i--;
        }
    }
    public void ResetBuffs()
    {
        PlayerStatuses.Clear();
    }

    protected override void InitEntity(ENTITY_TYPE myType)
    {
        eType = myType;
        speedMult = 1f;
        MovspeedMult = 1f;
        OverheatMult = 1f;
        BlasterDamageMult = 1f;
        GrenadeDamageMult = 1f;
        SniperDamageMult = 1f;
        DamageToBosses = 1f;
        BlasterDamagePerHpMult = 1f;
        SniperDamagePerHpMult = 1f;
        DamagePerHeatMult = 1f;
        DamageRed = 1f;
        GroguCost = 1f;
        myDeltaTime = Time.deltaTime;
        FireRateMult = 1f;
        RawDamageMult = 1f;
        SecTickDamage = 1f;
        sniperShotIntervalModifier = 1f;
        ForceRegentPerHPMod = 1f;
    }

    protected override void OnInitStatus(ref StatusData statusToInit)
    {
        switch (statusToInit.statusType)
        {
            case STATUS_TYPE.SLOWED:
                {
                    this.speedMult -= statusToInit.severity;

                    if (speedMult < 0.1f)
                    {
                        statusToInit.severity = statusToInit.severity - (Math.Abs(this.speedMult) + 0.1f);

                        speedMult = 0.1f;
                    }

                    this.myDeltaTime = Time.deltaTime * speedMult;
                    if (slowParticleObject != null)
                    {
                        ParticleSystem slowParticlePart = slowParticleObject.GetComponent<ParticleSystem>();

                        if (slowParticlePart != null)
                        {
                            if (speedMult < 1f)
                                slowParticlePart.Play();
                            else
                                slowParticlePart.Stop();
                        }

                    }

                }
                break;
            case STATUS_TYPE.ACCELERATED:
                {
                    this.speedMult += statusToInit.severity;

                    this.myDeltaTime = Time.deltaTime * speedMult;

                    if (fastParticleObject != null)
                    {
                        ParticleSystem fastParticlePart = fastParticleObject.GetComponent<ParticleSystem>();

                        if (fastParticlePart != null)
                        {
                            if (speedMult > 1f)
                                fastParticlePart.Play();
                            else
                                fastParticlePart.Stop();
                        }
                    }
                }
                break;
            case STATUS_TYPE.COMBO_UP_ACCEL:
                {
                    this.speedMult += statusToInit.severity;

                    this.myDeltaTime = Time.deltaTime * speedMult;

                    if (fastParticleObject != null)
                    {
                        ParticleSystem fastParticlePart = fastParticleObject.GetComponent<ParticleSystem>();

                        if (fastParticlePart != null)
                        {
                            if (speedMult > 1f)
                                fastParticlePart.Play();
                            else
                                fastParticlePart.Stop();
                        }
                    }
                }
                break;
            case STATUS_TYPE.REX_SEC_BLASTER_SUBSKILL:
                {
                    statusToInit.statChange = statusToInit.severity / 100;

                    speedMult += statusToInit.statChange;
                    Debug.Log("mult = " + speedMult.ToString() + "statChange = " + statusToInit.statChange.ToString());
                    myDeltaTime = Time.deltaTime * speedMult;

                    if (fastParticleObject != null)
                    {
                        ParticleSystem fastParticlePart = fastParticleObject.GetComponent<ParticleSystem>();

                        if (fastParticlePart != null)
                        {
                            if (speedMult > 1f)
                                fastParticlePart.Play();
                            else
                                fastParticlePart.Stop();
                        }
                    }

                }
                break;
            case STATUS_TYPE.ANAKIN_KILLSTREAK_SUBSKILL:
                {
                    statusToInit.statChange = statusToInit.severity / 100;

                    speedMult += statusToInit.statChange;
                    Debug.Log("mult = " + speedMult.ToString() + "statChange = " + statusToInit.statChange.ToString());
                    myDeltaTime = Time.deltaTime * speedMult;

                    if (fastParticleObject != null)
                    {
                        ParticleSystem fastParticlePart = fastParticleObject.GetComponent<ParticleSystem>();

                        if (fastParticlePart != null)
                        {
                            if (speedMult > 1f)
                                fastParticlePart.Play();
                            else
                                fastParticlePart.Stop();
                        }
                    }
                }
                break;
            case STATUS_TYPE.ECHO_RECOVERY_SUBSKILL:
                {
                    statusToInit.statChange = statusToInit.severity / 100;

                    speedMult += statusToInit.statChange;
                    Debug.Log("mult = " + speedMult.ToString() + "statChange = " + statusToInit.statChange.ToString());
                    myDeltaTime = Time.deltaTime * speedMult;

                    if (fastParticleObject != null)
                    {
                        ParticleSystem fastParticlePart = fastParticleObject.GetComponent<ParticleSystem>();

                        if (fastParticlePart != null)
                        {
                            if (speedMult > 1f)
                                fastParticlePart.Play();
                            else
                                fastParticlePart.Stop();
                        }
                    }
                }
                break;
            case STATUS_TYPE.ENEMY_DAMAGE_DOWN:
                {

                }
                break;
            case STATUS_TYPE.MOV_SPEED:
                {
                    statusToInit.statChange = statusToInit.severity / 100;
                    this.MovspeedMult += statusToInit.statChange;
                    //  Debug.Log(this.MovspeedMult.ToString());
                }
                break;

            case STATUS_TYPE.OVERHEATCAP:
                {
                    statusToInit.statChange = (statusToInit.severity) / 100;
                    this.OverheatMult += statusToInit.statChange;
                    // Debug.Log(this.OverheatMult.ToString());
                }
                break;
            case STATUS_TYPE.BLASTER_DAMAGE:
                {
                    statusToInit.statChange = (statusToInit.severity) / 100;
                    this.BlasterDamageMult += statusToInit.statChange;
                    // Debug.Log(this.DamageMult.ToString());
                }
                break;
            case STATUS_TYPE.GRENADE_DAMAGE:
                {
                    statusToInit.statChange = (statusToInit.severity) / 100;
                    this.GrenadeDamageMult += statusToInit.statChange;
                    // Debug.Log(this.DamageMult.ToString());
                }
                break;
            case STATUS_TYPE.SNIPER_DAMAGE:
                {
                    statusToInit.statChange = (statusToInit.severity) / 100;
                    this.SniperDamageMult += statusToInit.statChange;
                    // Debug.Log(this.DamageMult.ToString());
                }
                break;
            case STATUS_TYPE.DMG_TO_BOSSES:
                {
                    statusToInit.statChange = (statusToInit.severity) / 100;
                    this.DamageToBosses += statusToInit.statChange;
                    //  Debug.Log(this.DamageToBosses.ToString());
                }
                break;
            case STATUS_TYPE.MAX_HP:
                {
                    PlayerHealth myHealth = gameObject.GetComponent<PlayerHealth>();
                    if (myHealth != null && PlayerHealth.currMaxHealth == 100)
                    {
                        statusToInit.statChange = statusToInit.severity * PlayerHealth.currMaxHealth / 100;
                        myHealth.SetMaxHPValue((int)(PlayerHealth.currMaxHealth + statusToInit.statChange), true);
                    }

                }
                break;
            case STATUS_TYPE.MANDO_CODE:
                {

                    this.RawDamageMult += statusToInit.severity / 100;

                }
                break;
            case STATUS_TYPE.DMG_RED:
                {
                    statusToInit.statChange = 1 * (statusToInit.severity) / 100;
                    this.DamageRed += statusToInit.statChange;
                    Debug.Log("Damage red = " + this.DamageRed.ToString());

                }
                break;
            case STATUS_TYPE.COMBO_DMG_RED:
                {
                    statusToInit.statChange = 1 * (statusToInit.severity) / 100;
                    this.DamageRed += statusToInit.statChange;
                    Debug.Log("Damage red = " + this.DamageRed.ToString());

                }
                break;
            case STATUS_TYPE.BOSSK_STR:
                {
                    statusToInit.statChange = 1 * (statusToInit.severity) / 100;
                    this.DamageRed += statusToInit.statChange;
                    Debug.Log("Damage red = " + this.DamageRed.ToString());

                }
                break;
            case STATUS_TYPE.GROGU_COST:
                {
                    statusToInit.statChange = statusToInit.severity / 100;
                    this.GroguCost += statusToInit.statChange;
                }
                break;
            case STATUS_TYPE.FIRE_RATE:
                {

                    statusToInit.statChange = statusToInit.severity / 100;
                    this.FireRateMult += statusToInit.statChange;

                }
                break;
            case STATUS_TYPE.GREEDO_SHOOTER:
                {
                    statusToInit.statChange = statusToInit.severity / 100;
                    this.FireRateMult += statusToInit.statChange;
                }
                break;
            case STATUS_TYPE.FIRE_RATE_PERMANENT:
                {

                    statusToInit.statChange = statusToInit.severity / 100;
                    this.FireRateMult += statusToInit.statChange;


                }
                break;
            case STATUS_TYPE.RAW_DAMAGE:
                {

                    statusToInit.statChange = (statusToInit.severity) / 100;
                    this.RawDamageMult += statusToInit.statChange;
                    // Debug.Log(this.DamageMult.ToString());
                }
                break;
            case STATUS_TYPE.SEC_TICKDMG:
                {
                    statusToInit.statChange = statusToInit.severity;
                    this.SecTickDamage += statusToInit.statChange;
                }
                break;
            case STATUS_TYPE.SP_INTERVAL:
                {
                    statusToInit.statChange = statusToInit.severity / 100;
                    sniperShotIntervalModifier += statusToInit.statChange;
                }
                break;
            case STATUS_TYPE.FENNEC_SR:
                {
                    statusToInit.statChange = statusToInit.severity / 100;
                    sniperShotIntervalModifier += statusToInit.statChange;
                }
                break;
            case STATUS_TYPE.GRO_MAX_FORCE:
                {
                    statusToInit.statChange = statusToInit.severity;
                    MaxForceModifier += statusToInit.statChange;
                }
                break;
            case STATUS_TYPE.WATTO_COOLANT:
                {
                    statusToInit.statChange = statusToInit.severity / 100;
                    dashCDModifier += statusToInit.statChange;
                }
                break;
            case STATUS_TYPE.SOLO_HEAL:
                {
                    statusToInit.statChange = statusToInit.severity;
                    skill_SoloHeal = statusToInit.statChange;
                }
                break;
            case STATUS_TYPE.GREEF_PAYCHECK:
                {
                    ShopDiscount = (int)statusToInit.severity;
                    Debug.Log(ShopDiscount.ToString());
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
            case STATUS_TYPE.BLAST_DMG_PER_HP:
                {

                    //PlayerHealth myHealth = Core.instance.gameObject.GetComponent<PlayerHealth>();
                    //if (myHealth != null)
                    //{
                    float missingHealth = PlayerHealth.currMaxHealth - PlayerHealth.currHealth;
                    float missingHealtPercentage = missingHealth / PlayerHealth.currMaxHealth;
                    BlasterDamagePerHpMult = 1 + missingHealtPercentage;
                    //  Debug.Log("TESTING / dmg per missing hp = " + DamagePerHpMult.ToString());
                    // }


                }
                break;
            case STATUS_TYPE.SNIPER_DMG_PER_HP:
                {

                    //PlayerHealth myHealth = Core.instance.gameObject.GetComponent<PlayerHealth>();
                    //if (myHealth != null)
                    //{
                    float missingHealth = PlayerHealth.currMaxHealth - PlayerHealth.currHealth;
                    float missingHealtPercentage = missingHealth / PlayerHealth.currMaxHealth;
                    SniperDamagePerHpMult = 1 + missingHealtPercentage;
                    //  Debug.Log("TESTING / dmg per missing hp = " + DamagePerHpMult.ToString());
                    // }


                }
                break;
            case STATUS_TYPE.DMG_PER_HEAT:
                {

                    if (instance.hud != null)
                    {
                        float currentheat = Core.instance.hud.GetComponent<HUD>().primaryWeaponHeat;
                        float HeatPercentage = currentheat / Core.instance.hud.GetComponent<HUD>().primaryWeaponMaxHeat;
                        DamagePerHeatMult = 1 + HeatPercentage;
                        //    Debug.Log("TESTING / dmg per heat = " + DamagePerHeatMult.ToString());
                    }


                }
                break;
            case STATUS_TYPE.GRO_FORCE_PER_HP:
                {
                    float missingHealth = PlayerHealth.currMaxHealth - PlayerHealth.currHealth;
                    float missingHealthPercentage = missingHealth / PlayerHealth.currMaxHealth * 10;
                    ForceRegentPerHPMod = missingHealthPercentage;

                    //    Debug.Log("TESTING / dmg per heat = " + DamagePerHeatMult.ToString());

                }
                break;
            case STATUS_TYPE.CAD_BANE_BOOTS:
                {
                    speedMult += statusToUpdate.statChange;

                    myDeltaTime = Time.deltaTime * speedMult;
                    statusToUpdate.statChange = 0;
                    //    Debug.Log("TESTING / dmg per heat = " + DamagePerHeatMult.ToString());

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
                    if (slowParticleObject != null)
                    {
                        ParticleSystem slowParticlePart = slowParticleObject.GetComponent<ParticleSystem>();

                        if (slowParticlePart != null && speedMult >= 1f)
                        {
                            slowParticlePart.Stop();
                        }
                    }
                }
                break;
            case STATUS_TYPE.ACCELERATED:
                {
                    this.speedMult -= statusToDelete.severity;

                    this.myDeltaTime = Time.deltaTime * speedMult;
                    if (fastParticleObject != null && speedMult <= 1f)
                    {
                        ParticleSystem fastParticlePart = fastParticleObject.GetComponent<ParticleSystem>();

                        if (fastParticlePart != null)
                        {
                            fastParticlePart.Stop();
                        }

                    }
                }
                break;
            case STATUS_TYPE.COMBO_UP_ACCEL:
                {
                    this.speedMult -= statusToDelete.severity;

                    this.myDeltaTime = Time.deltaTime * speedMult;

                    if (fastParticleObject != null && speedMult <= 1f)
                    {
                        ParticleSystem fastParticlePart = fastParticleObject.GetComponent<ParticleSystem>();

                        if (fastParticlePart != null)
                        {
                            fastParticlePart.Stop();
                        }
                    }
                }
                break;
            case STATUS_TYPE.REX_SEC_BLASTER_SUBSKILL:
                {
                    this.speedMult -= statusToDelete.statChange;

                    this.myDeltaTime = Time.deltaTime * speedMult;

                    if (fastParticleObject != null && speedMult <= 1f)
                    {
                        ParticleSystem fastParticlePart = fastParticleObject.GetComponent<ParticleSystem>();

                        if (fastParticlePart != null)
                        {
                            fastParticlePart.Stop();
                        }
                    }
                }
                break;
            case STATUS_TYPE.ANAKIN_KILLSTREAK_SUBSKILL:
                {
                    this.speedMult -= statusToDelete.statChange;

                    this.myDeltaTime = Time.deltaTime * speedMult;

                    if (fastParticleObject != null && speedMult <= 1f)
                    {
                        ParticleSystem fastParticlePart = fastParticleObject.GetComponent<ParticleSystem>();

                        if (fastParticlePart != null)
                        {
                            fastParticlePart.Stop();
                        }
                    }
                }
                break;
            case STATUS_TYPE.ECHO_RECOVERY_SUBSKILL:
                {
                    this.speedMult -= statusToDelete.statChange;

                    this.myDeltaTime = Time.deltaTime * speedMult;

                    if (fastParticleObject != null && speedMult <= 1f)
                    {
                        ParticleSystem fastParticlePart = fastParticleObject.GetComponent<ParticleSystem>();

                        if (fastParticlePart != null)
                        {
                            fastParticlePart.Stop();
                        }
                    }
                }
                break;
            case STATUS_TYPE.ENEMY_DAMAGE_DOWN:
                {
                }
                break;
            case STATUS_TYPE.MOV_SPEED:
                {
                    this.MovspeedMult -= statusToDelete.statChange;
                    //  Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.OVERHEATCAP:
                {
                    this.OverheatMult -= statusToDelete.statChange;
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.BLASTER_DAMAGE:
                {
                    this.BlasterDamageMult -= statusToDelete.statChange;
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.GRENADE_DAMAGE:
                {
                    this.GrenadeDamageMult -= statusToDelete.statChange;
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.SNIPER_DAMAGE:
                {
                    this.SniperDamageMult -= statusToDelete.statChange;
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.DMG_TO_BOSSES:
                {
                    this.DamageToBosses -= statusToDelete.statChange;
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.BLAST_DMG_PER_HP:
                {
                    this.BlasterDamagePerHpMult = 1;
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.SNIPER_DMG_PER_HP:
                {
                    this.SniperDamagePerHpMult = 1;
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.MAX_HP:
                {
                    PlayerHealth myHealth = gameObject.GetComponent<PlayerHealth>();
                    myHealth.SetMaxHPValue((int)(PlayerHealth.currMaxHealth - statusToDelete.statChange));
                }
                break;
            case STATUS_TYPE.MANDO_CODE:
                {
                    //PlayerHealth myHealth = gameObject.GetComponent<PlayerHealth>();
                    //myHealth.SetMaxHPValue((int)(PlayerHealth.currMaxHealth - statusToDelete.statChange));


                    this.RawDamageMult -= statusToDelete.severity / 100;
                }
                break;
            case STATUS_TYPE.DMG_RED:
                {
                    this.DamageRed -= statusToDelete.statChange;
                    Debug.Log("Damage red = " + this.DamageRed.ToString());
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.COMBO_DMG_RED:
                {
                    this.DamageRed -= statusToDelete.statChange;
                    Debug.Log("Damage red = " + this.DamageRed.ToString());

                }
                break;
            case STATUS_TYPE.BOSSK_STR:
                {
                    this.DamageRed -= statusToDelete.statChange;
                    Debug.Log("Damage red = " + this.DamageRed.ToString());

                }
                break;
            case STATUS_TYPE.DMG_PER_HEAT:
                {
                    this.ForceRegentPerHPMod = 0;
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.GRO_FORCE_PER_HP:
                {
                    this.DamagePerHeatMult = 1;
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.GROGU_COST:
                {
                    this.GroguCost -= statusToDelete.statChange;
                    //    Debug.Log(this.MovspeedMult.ToString());
                }
                break;
            case STATUS_TYPE.FIRE_RATE:
                {
                    this.FireRateMult -= statusToDelete.statChange;
                }
                break;
            case STATUS_TYPE.GREEDO_SHOOTER:
                {
                    this.FireRateMult -= statusToDelete.statChange;
                }
                break;
            case STATUS_TYPE.FIRE_RATE_PERMANENT:
                {
                    this.FireRateMult -= statusToDelete.statChange;


                }
                break;
            case STATUS_TYPE.RAW_DAMAGE:
                {
                    Debug.Log("delete raw damage buff");

                    this.RawDamageMult -= statusToDelete.statChange;
                    if (this.RawDamageMult > 2)
                        this.RawDamageMult = 1;
                }
                break;
            case STATUS_TYPE.SEC_TICKDMG:
                {

                    this.SecTickDamage -= statusToDelete.statChange;
                }
                break;
            case STATUS_TYPE.SP_INTERVAL:
                {
                    sniperShotIntervalModifier -= statusToDelete.statChange;
                }
                break;
            case STATUS_TYPE.FENNEC_SR:
                {
                    sniperShotIntervalModifier -= statusToDelete.statChange;
                }
                break;
            case STATUS_TYPE.GRO_MAX_FORCE:
                {
                    MaxForceModifier = 0;
                }
                break;
            case STATUS_TYPE.WATTO_COOLANT:
                {
                    dashCDModifier -= statusToDelete.statChange;
                }
                break;
            case STATUS_TYPE.SOLO_HEAL:
                {

                    skill_SoloHeal = 0;
                }
                break;
            case STATUS_TYPE.GREEF_PAYCHECK:
                {
                    ShopDiscount = 0;
                }
                break;
            default:
                break;
        }
    }

    public void NotifyEnemyDeath(GameObject enemy)
    {
        if (myGrenade1 != null)
        {
            if (myGrenade1.IsEnabled() == true)
            {
                SlowGrenade grenadeComp = myGrenade1.GetComponent<SlowGrenade>();

                if (grenadeComp != null)
                {
                    grenadeComp.EnemyDied(enemy);
                }
            }

        }

        if (myGrenade2 != null)
        {
            if (myGrenade2.IsEnabled() == true)
            {
                SlowGrenade grenadeComp = myGrenade2.GetComponent<SlowGrenade>();

                if (grenadeComp != null)
                {
                    grenadeComp.EnemyDied(enemy);
                }
            }
        }
    }

    public override bool IsDying()
    {
        return IsMandoDead();
    }

    public bool IsMandoDead()
    {
        return (currentState == STATE.DEAD);
    }

    public void SetMySpawnPosition(Vector3 newPos)
    {
        mySpawnPos = newPos;
    }
    public Vector3 GetMySpawnPosition()
    {
        return mySpawnPos;
    }

    public void BlockInIdle()
    {
        lockInputs = true;
        currentState = STATE.IDLE;
        DisableBlaster();
        StartIdle();
    }

    public int GetTutoState()
    {
        return (int)tuto_state;
    }

    public void Zone2()
    {
        tuto_state = TUTO_STATES.BLASTER;
        BabyYoda babyYoda = GetBabyYoda();
        if (babyYoda != null)
            babyYoda.tutoState = BabyYoda.TUTO_YODA_STATES.LOCKED;
        Debug.Log("Entering Zone 2");
    }

    public void OnDestroy()
    {
        instance = null;
    }
}