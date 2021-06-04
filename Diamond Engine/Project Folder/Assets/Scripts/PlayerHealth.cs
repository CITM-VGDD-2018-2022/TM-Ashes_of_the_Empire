using System;
using DiamondEngine;

public class PlayerHealth : DiamondComponent
{
    public static Action onPlayerDeath;

    public static int currMaxHealth { get; private set; }
    public static int currHealth { get; private set; }

    //Bo Katan’s resilience: each time you kill an enemy heal +1 HP.
    //0 means this boon is not working else heal the amount stored here
    public static int healWhenKillingAnEnemy { get; private set; }
    public GameObject character_mesh = null;

    private bool die = false;
    private float damaged = 0.0f;

    private bool invincible = false;

    public void Awake()
    {
        if (currHealth <= 0 && currMaxHealth <= 0)
            ResetMaxAndCurrentHPToDefault();
    }

    public void Update()
    {
        if(Input.GetKey(DEKeyCode.P) == KeyState.KEY_DOWN)
        {
            ResetMaxAndCurrentHPToDefault();
        }

        if (die && !DebugOptionsHolder.godModeActive)
        {
            if (Core.instance != null && Core.instance.HasStatus(STATUS_TYPE.REVIVE) && Core.instance.GetStatusData(STATUS_TYPE.REVIVE).severity == 1)
            {
                HealPercentMax(0.5f);
                Core.instance.GetStatusData(STATUS_TYPE.REVIVE).severity = 0;
                die = false;
            }
            else
            {
                Core.instance.GetStatusData(STATUS_TYPE.REVIVE).severity = 1;

                die = false;
                //Die();
            }

        }
        if (damaged > 0.01f)
        {
            damaged = Mathf.Lerp(damaged, 0.0f, 0.1f);
        }
        else
        {
            damaged = 0.0f;
        }
        if (character_mesh != null)
        {
            character_mesh.GetComponent<Material>().SetFloatUniform("damaged", damaged);
        }

        if (Input.GetKey(DEKeyCode.COMMA) == KeyState.KEY_DOWN) Debug.Log(currHealth.ToString());
        if (Input.GetKey(DEKeyCode.M) == KeyState.KEY_DOWN) SetMaxHPValue(50, true);
    }

    //Increments the max Hp by the percentatge given as a parameter (1 = 100% 0 = 0%) It can also be negative to substract HP
    public int IncrementMaxHpPercent(float percent, bool alsoRestoreAllHP = false)
    {
        currMaxHealth += (int)(currMaxHealth * percent);

        if (currMaxHealth < 1)
            currMaxHealth = 1;

        if (alsoRestoreAllHP || currHealth > currMaxHealth)
        {
            currHealth = currMaxHealth;
        }

        if (Core.instance.hud != null)
            Core.instance.hud.GetComponent<HUD>().UpdateHP(currHealth, currMaxHealth);
        return currMaxHealth;
    }

    //Increments the max Hp by the value given as a parameter. It can also be negative to substract HP
    public int IncrementMaxHpValue(int sumMaxHP, bool alsoRestoreAllHP = false)
    {
        currMaxHealth += sumMaxHP;

        if (currMaxHealth < 1)
            currMaxHealth = 1;

        if (alsoRestoreAllHP || currHealth > currMaxHealth)
        {
            currHealth = currMaxHealth;
        }

        if (Core.instance.hud != null)
            Core.instance.hud.GetComponent<HUD>().UpdateHP(currHealth, currMaxHealth);
        return currMaxHealth;
    }

    //Sets Max Hp to the value given
    public void SetMaxHPValue(int newMaxHP, bool alsoRestoreAllHP = false)
    {
        currMaxHealth = newMaxHP;

        if (currMaxHealth < 1)
            currMaxHealth = 1;

        if (alsoRestoreAllHP || currHealth > currMaxHealth)
        {
            currHealth = currMaxHealth;
        }
        if (Core.instance.hud != null)
            Core.instance.hud.GetComponent<HUD>().UpdateHP(currHealth, currMaxHealth);
    }


    //Adds a value to the current healing when killing an enemy. It can also be negative to substract Healing
    public int IncrementHealingWhenKillingEnemy(int increment)
    {
        healWhenKillingAnEnemy += increment;

        if (healWhenKillingAnEnemy < 0)
            healWhenKillingAnEnemy = 0;

        return healWhenKillingAnEnemy;
    }

    //Sets the value of the current healing when killing an enemy.
    public void SetHealingWhenKillingEnemy(int newHealing)
    {
        healWhenKillingAnEnemy = newHealing;

        if (healWhenKillingAnEnemy < 0)
            healWhenKillingAnEnemy = 0;
    }

    //Increments the current Hp by the percentatge given as a parameter (1 = 100% 0 = 0%) It can also be negative to take percentual damage
    public void HealPercent(float percent)
    {
        if (DebugOptionsHolder.godModeActive || invincible || die)
            return;

        currHealth += (int)(currHealth * percent);

        if (currHealth > currMaxHealth) currHealth = currMaxHealth;

        if (Core.instance.hud != null)
            Core.instance.hud.GetComponent<HUD>().UpdateHP(currHealth, currMaxHealth);

        if (currHealth <= 0)
        {
            die = true;
            onPlayerDeath?.Invoke();

        }
    }
    //Increments the current Hp by the percentatge given as a parameter (1 = 100% 0 = 0%) It can also be negative to take percentual damage
    public void HealPercentMax(float percentofMaxHp)
    {
        if (DebugOptionsHolder.godModeActive || invincible || die)
            return;

        currHealth += (int)(currMaxHealth * percentofMaxHp);
        currHealth = Math.Min(currMaxHealth, currHealth);

        if (currHealth > currMaxHealth) currHealth = currMaxHealth;

        if (Core.instance.hud != null)
            Core.instance.hud.GetComponent<HUD>().UpdateHP(currHealth, currMaxHealth);

        if (currHealth <= 0)
        {
            die = true;
            onPlayerDeath?.Invoke();
        }
    }

    //When current HP drops to 0, Die() Method is called
    public void SetCurrentHP(int newCurrentHP)
    {
        if (DebugOptionsHolder.godModeActive || invincible || die)
            return;

        currHealth = newCurrentHP;

        if (currHealth > currMaxHealth) currHealth = currMaxHealth;

        if (Core.instance.hud != null)
            Core.instance.hud.GetComponent<HUD>().UpdateHP(currHealth, currMaxHealth);

        if (currHealth <= 0)
        {
            die = true;
            onPlayerDeath?.Invoke();
        }
    }

    //Also works as a HEAL AMOUNT when taking negative damage ;) When current HP drops to 0, Die() Method is called
    public int TakeDamage(int damage, bool ignoreDashInv = false)
    {
        if (DebugOptionsHolder.godModeActive || Core.instance.lockInputs || invincible || die)
            return currHealth;

        if (Core.instance != null)
        {
            if (Core.instance.HasStatus(STATUS_TYPE.BLOCK))
            {
                Random rand = new Random();
                float result = rand.Next(1, 101);
                if (result <= Core.instance.GetStatusData(STATUS_TYPE.BLOCK).severity)
                    return currHealth;
            }

            if (Core.instance.HasStatus(STATUS_TYPE.REFILL_CHANCE))
            {
                Random rand = new Random();
                float result = rand.Next(1, 101);
                if (result <= Core.instance.GetStatusData(STATUS_TYPE.REFILL_CHANCE).severity)
                    Core.instance.refreshCooldowns();
            }
            if (Core.instance.HasStatus(STATUS_TYPE.SOLO_QUICK_DRAW))
            {
                float toHeal = damage * Core.instance.GetStatusData(STATUS_TYPE.SOLO_QUICK_DRAW).severity / 100;
                Core.instance.AddStatus(STATUS_TYPE.SOLO_HEAL, STATUS_APPLY_TYPE.SUBSTITUTE, toHeal, 4);
            }
        }

        if (Core.instance != null && damage > 0 && ignoreDashInv == false)
        {

            if (Core.instance.IsDashing())
                return currHealth;
        }

        //if(Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.DEFENSE_CHANCE_AVOID_DAMAGE))
        //{
        //    if (ChanceToAvoidDamage(Skill_Tree_Data.GetMandoSkillTree().D8_changeToAvoidDamage))
        //    {
        //        Debug.Log("Damage missed!");
        //        return currHealth; //We have avoided damage with a skill
        //    }
        //}        

        if (Core.instance != null)
        {
            int damageTaken = (int)(damage * Core.instance.DamageRed);

            if (PlayerResources.CheckBoon(BOONS.BOON_BOSSK_STRENGTH))
            {
                float damageReduced = (damage * (Core.instance.GetStatusData(STATUS_TYPE.BOSSK_STR).severity / 100f));

                damageTaken = (int)(damage * Core.instance.DamageRed) - (int)(Math.Max(damageReduced, 1f));
            }

            currHealth -= damageTaken;
        }


        if (currHealth <= 0)
        {
            die = true;
            onPlayerDeath?.Invoke();
        }
        if (currHealth > currMaxHealth) currHealth = currMaxHealth;

        if (Core.instance.hud != null)
        {
            HUD playerHud = Core.instance.hud.GetComponent<HUD>();

            if (playerHud != null && !die)
                playerHud.UpdateHP(currHealth, currMaxHealth);

            if (damage > 0)
                Core.instance.ReduceComboOnHit(damage);
        }

        if (currHealth < 75 && currHealth >= 50 && MusicSourceLocate.instance != null)
        {
            Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Health", "Injured");
        }
        else if (currHealth < 50 && currHealth > 0 && MusicSourceLocate.instance != null)
        {
            Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Health", "Critical");
        }
        damaged = (damage > 0) ? 1.0f : damaged;
        //Debug.Log("Current health: " + currHealth.ToString());
        return currHealth;
    }


    public void Die()
    {
        ResetMaxAndCurrentHPToDefault();
        //TODO die
        // Set as defeat:
        RoomSwitch.OnPlayerDeath();

    }

    public void TutorialDie()
    {
        ResetMaxAndCurrentHPToDefault();
        //gameObject.transform.localPosition = Core.instance.GetMySpawnPosition();
        SceneManager.LoadScene(200199708);
    }
    public static void ResetMaxAndCurrentHPToDefault()
    {
        healWhenKillingAnEnemy = 0;

        currHealth = currMaxHealth = 100; //TODO set the starting health here for now
    }

    //chanceToAvoid must be a number between 0 and 100 (%)
    private bool ChanceToAvoidDamage(int chanceToAvoid)
    {
        Random rnd = new Random();
        int randomNum = rnd.Next(1, 101);
        return chanceToAvoid >= randomNum;
    }


    public void ToggleNoClip(bool val)
    {
        if (val)
        {
            //Core.instance.gameObject.GetComponent<CapsuleCollider>.active = false;
            //TODO: rigidbody gravity
        }
        else
        {
            //Core.instance.gameObject.GetComponent<CapsuleCollider>().active = true;
            //TODO: rigidbody gravity
        }
    }


    public void SetInvincible(bool set)
    {
        invincible = set;
    }
}