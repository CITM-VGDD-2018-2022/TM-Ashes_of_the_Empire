using System;
using DiamondEngine;
using System.Collections.Generic;


public enum ENTITY_TYPE
{
    NONE = -1,

    //Player
    PLAYER,

    //Enemies
    BANTHA,
    STROMTROOPER,
    SKYTROOPER,
    TURRET,
    DEATHTROOPER,
    HEAVYTROOPER,

    //Bosses
    RANCOR,
    WAMPA,
    SKEL,
    MOFF,

    MAX_ENTITIES

}

public enum STATUS_TYPE
{
    NONE = -1,

    //General
    SLOWED,
    ACCELERATED,

    //Mando
    MOV_SPEED,
    OVERHEATCAP,
    BLASTER_DAMAGE,
    GRENADE_DAMAGE,
    SNIPER_DAMAGE,
    DMG_TO_BOSSES,
    BLAST_DMG_PER_HP,
    SNIPER_DMG_PER_HP,
    MAX_HP,
    DMG_RED,
    COMBO_DMG_RED,
    DMG_PER_HEAT,
    FALL,
    GROGU_COST,
    SKILL_HEAL,
    COMBO_FIRE_RATE,
    FIRE_RATE,
    FIRE_RATE_PERMANENT,
    COMBO_DAMAGE,
    COMBO_RED,
    COMBO_HEAL,
    HEAL_COMBO_FINNISH,
    RAW_DAMAGE,
    LIFESTEAL,
    BLOCK,
    PRIM_SPEED,
    PRIM_MOV_SPEED,
    PRIM_RANGE,
    PRIM_SLOW,
    OVERHEAT,
    PRIM_CHARGED,
    SEC_TICKDMG,
    SEC_SLOW,
    SEC_RECOVERY,
    SEC_DURATION,
    SP_NONCHARGED,
    SP_CHARGE_TIME,
    SP_HEAL,
    SP_FORCE_REGEN,
    SP_INTERVAL,
    SP_DAMAGE_CHARGED,
    GRO_FORCE_REGEN,
    GRO_PUSH,
    GRO_COVER,
    GRO_COMBO_REGEN,
    GRO_MAX_FORCE,
    GRO_COMBO_ADD,
    GRO_COMBO_GAIN,
    GRO_FORCE_PER_HP,
    REVIVE,
    REFILL_CHANCE,
    GRENADE_ON_DASH,
    SEC_RANGE,
    COMBO_UP_ACCEL,
    SNIPER_STACK_ENABLE,
    SNIPER_STACK_DMG_UP,
    SNIPER_STACK_WORK_SNIPER,
    SNIPER_STACK_BLEED,

    // Enemies
    ENEMY_DAMAGE_DOWN,
    ENEMY_VULNERABLE,
    ENEMY_DAMAGE_UP,
    ENEMY_HP_UP,
    ENEMY_BLEED,
    ENEMY_SLOWED,

    // Boons
    CAD_BANE_SOH,
    CAD_BANE_BOOTS,
    MANDO_QUICK_DRAW,
    FENNEC_SR,
    //BOBBA_AMMO
    BOSSK_STR,
    REX_SEC_BLASTER,
    REX_SEC_BLASTER_SUBSKILL,
    GREEDO_SHOOTER,
    ANAKIN_KILLSTREAK,
    ANAKIN_KILLSTREAK_SUBSKILL,
    ECHO_RECOVERY,
    ECHO_RECOVERY_SUBSKILL,
    WATTO_COOLANT,
    LUMINARA_FORCE,
    MANDO_CODE,
    ITSA_TRAP,
    WRECK_HEAVY_SHOT,
    QUICK_COMBO,
    BLASTER_VULN,
    BLAST_CANNON,
    BOUNTY_HUNTER,
    BOSSK_AMMO,
    WINDU_FORCE,
    YODA_FORCE,
    CROSS_HAIR_LUCKY_SHOT,
    AHSOKA_DET,
    BOBBA_STUN_AMMO,
    SOLO_QUICK_DRAW,
    SOLO_HEAL,
    GEOTERMAL_MARKER,
    GREEF_PAYCHECK,
}

public enum STATUS_APPLY_TYPE
{
    NONE = -1,
    BIGGER_PERCENTAGE,
    BIGGER_TIME,
    SUBSTITUTE,
    ADDITIVE,
    ADD_SEV
}

#region StatusData
public class StatusData
{
    //Constructors
    public StatusData()
    {
        statusType = STATUS_TYPE.NONE;
        applyType = STATUS_APPLY_TYPE.NONE;
        severity = 0f;
        statChange = 0f;
        remainingTime = 0f;
        isPermanent = false;
    }
    public StatusData(STATUS_TYPE statType, STATUS_APPLY_TYPE statusApplyType, float statusSeverirt, float statusTime, bool isstatusPermanent, float statusStatTaken = 0f)
    {
        statusType = statType;
        applyType = statusApplyType;
        severity = statusSeverirt;
        remainingTime = statusTime;
        isPermanent = isstatusPermanent;
        statChange = statusStatTaken;
    }

    public float severity;
    public float statChange;
    public float remainingTime;
    public bool isPermanent;
    public STATUS_APPLY_TYPE applyType;
    public STATUS_TYPE statusType;
}

#endregion

public class Entity : DiamondComponent
{
    protected ENTITY_TYPE eType = ENTITY_TYPE.NONE;

    private Dictionary<STATUS_TYPE, StatusData> statuses = new Dictionary<STATUS_TYPE, StatusData>();
    private List<STATUS_TYPE> statusToDelete = new List<STATUS_TYPE>();

    protected float speedMult = 1f;
    protected float myDeltaTime = 1f;
    protected float MovspeedMult = 1f;
    public float OverheatMult = 1f;
    public float RawDamageMult = 1f;
    public float BlasterDamageMult = 1f;
    public float GrenadeDamageMult = 1f;
    public float SniperDamageMult = 1f;
    public float DamageToBosses = 1f;
    public float BlasterDamagePerHpMult = 1f;
    public float SniperDamagePerHpMult = 1f;

    public float DamagePerHeatMult = 1f;
    public float DamageRed = 1f;
    public float GroguCost = 1f;
    public float FireRateMult = 1f;
    protected float SecTickDamage = 1f;
    protected float sniperShotIntervalModifier = 1f;
    public float MaxForceModifier = 0f;
    public float ForceRegentPerHPMod = 0f;
    public float BlasterVulnerability = 1f;

    protected virtual void InitEntity(ENTITY_TYPE myType)
    {
        eType = myType;
        speedMult = 1f;
        BlasterVulnerability = 1f;

    }

    #region ENTITY TYPES


    public ENTITY_TYPE GetEntityType()
    {
        return eType;
    }

    public bool IsEnemy()
    {
        return eType == ENTITY_TYPE.BANTHA || eType == ENTITY_TYPE.STROMTROOPER || eType == ENTITY_TYPE.SKYTROOPER || eType == ENTITY_TYPE.TURRET || eType == ENTITY_TYPE.DEATHTROOPER || eType == ENTITY_TYPE.HEAVYTROOPER;
    }

    public bool IsBoss()
    {
        return eType == ENTITY_TYPE.RANCOR || eType == ENTITY_TYPE.WAMPA || eType == ENTITY_TYPE.SKEL || eType == ENTITY_TYPE.MOFF;
    }

    public bool IsPlayer()
    {
        return eType == ENTITY_TYPE.PLAYER;
    }

    #endregion


    #region STATUS PUBLIC CALLS
    public void AddStatus(STATUS_TYPE statusType, STATUS_APPLY_TYPE applyType, float percentage, float time, bool isPermanent = false)
    {
        if (CheckStatusApply(ref statusType, ref applyType, ref percentage, ref time, ref isPermanent) == false)
            return;

        statuses.Add(statusType, InitStatus(statusType, applyType, percentage, time, isPermanent));
    }

    public void RemoveStatus(STATUS_TYPE statusType, bool affectPermanents = false)
    {
        if (statuses.ContainsKey(statusType) == true)
        {
            if (statuses[statusType].isPermanent == false || affectPermanents == true)
            {
                DeleteStatus(statusType);
            }
        }

    }

    public void ClearStatuses(bool affectPermanents = false)
    {
        var mapKeys = statuses.Keys;
        foreach (STATUS_TYPE statusType in mapKeys)
        {
            if (statuses[statusType].isPermanent == false || affectPermanents == true)
            {
                statusToDelete.Add(statusType);
            }
        }

        for (int i = 0; i < statusToDelete.Count; i++)
        {
            RemoveStatus(statusToDelete[i]);
        }
        statusToDelete.Clear();
    }

    public bool HasStatus(STATUS_TYPE stat)
    {
        return statuses.ContainsKey(stat);
    }

    public bool HasAnyStatus()
    {
        return statuses.Count > 0;
    }
    public bool HasNegativeStatus()
    {
        int counter = 0;
        if (HasStatus(STATUS_TYPE.ACCELERATED))
            counter++;
        if (HasStatus(STATUS_TYPE.ENEMY_DAMAGE_UP))
            counter++;
        if (HasStatus(STATUS_TYPE.ENEMY_HP_UP))
            counter++;

        return statuses.Count > counter;
    }
    public int StatusCount()
    {
        return statuses.Count;
    }
    public float GetStatusRemainingTime(STATUS_TYPE stat)
    {
        float ret = 0f;

        if(statuses.ContainsKey(stat) == true)
        {
            ret = statuses[stat].remainingTime;
        }

        return ret;
    }

    public StatusData GetStatusData(STATUS_TYPE stat)
    {
        StatusData ret = new StatusData();

        if (statuses.ContainsKey(stat) == true)
        {
            ret = statuses[stat];
        }

        return ret;
    }

    #endregion


    private bool CheckStatusApply(ref STATUS_TYPE statusType, ref STATUS_APPLY_TYPE applyType, ref float percentage, ref float time, ref bool isPermanent)
    {
        bool ret = true;

        switch (applyType)
        {
            case STATUS_APPLY_TYPE.BIGGER_PERCENTAGE:
                {
                    if (statuses.ContainsKey(statusType) == true)
                    {
                        if (statuses[statusType].severity < percentage)
                        {
                            DeleteStatus(statusType);
                        }
                        else
                        {
                            ret = false;
                        }
                    }
                }
                break;
            case STATUS_APPLY_TYPE.BIGGER_TIME:
                {
                    if (statuses.ContainsKey(statusType) == true)
                    {
                        if (statuses[statusType].remainingTime < time)
                        {
                            DeleteStatus(statusType);
                        }
                        else
                        {
                            ret = false;
                        }
                    }
                }
                break;
            case STATUS_APPLY_TYPE.SUBSTITUTE:
                {
                    if (statuses.ContainsKey(statusType) == true)
                    {
                        DeleteStatus(statusType);
                    }
                }
                break;
            case STATUS_APPLY_TYPE.ADD_SEV:
                {
                    if (statuses.ContainsKey(statusType) == true)
                    {
                        percentage += statuses[statusType].severity;

                        DeleteStatus(statusType);
                    }
                }
                break;
            case STATUS_APPLY_TYPE.ADDITIVE:
                {
                    if (statuses.ContainsKey(statusType) == true)
                    {
                        percentage += statuses[statusType].severity;
                        time += statuses[statusType].remainingTime;

                        DeleteStatus(statusType);
                    }
                }
                break;
            default:
                break;
        }


        return ret;
    }

    private StatusData InitStatus(STATUS_TYPE statusType, STATUS_APPLY_TYPE applyType, float percentage, float time, bool isPermanent)
    {
        StatusData statuserstatusData = new StatusData(statusType, applyType, percentage, time, isPermanent);

        OnInitStatus(ref statuserstatusData);

        return statuserstatusData;
    }

    protected virtual void OnInitStatus(ref StatusData statusToInit)
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
            case STATUS_TYPE.ACCELERATED:
                {
                    this.speedMult += statusToInit.severity;

                    this.myDeltaTime = Time.deltaTime * speedMult;
                }
                break;
            case STATUS_TYPE.BLASTER_VULN:
                {

                    statusToInit.statChange = statusToInit.severity / 100;
                    if(statusToInit.statChange > 1) statusToInit.statChange = 1;
                    this.BlasterVulnerability += statusToInit.statChange;
                }
                break;

            default:
                break;
        }
    }

    protected virtual void OnUpdateStatus(StatusData statusToUpdate)
    {
        switch (statusToUpdate.statusType)
        {
            
            default:
                break;
        }
    }

    protected virtual void OnDeleteStatus(StatusData statusToDelete)
    {
        switch (statusToDelete.statusType)
        {
            case STATUS_TYPE.SLOWED:
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
            case STATUS_TYPE.BLASTER_VULN:
                {
                    this.BlasterVulnerability -= statusToDelete.severity;
                }
                break;
            default:
                break;
        }
    }

    protected void UpdateStatuses()
    {
        var mapKeys = statuses.Keys;
        foreach (STATUS_TYPE statusType in mapKeys)
        {
            if (statuses.ContainsKey(statusType) == false)
                continue;

            statuses[statusType].remainingTime -= Time.deltaTime;

            if (statuses[statusType].remainingTime > 0f || statuses[statusType].isPermanent)
            {
                OnUpdateStatus(statuses[statusType]);
            }
            else
            {
                statusToDelete.Add(statusType);
            }

        }

        for (int i = 0; i < statusToDelete.Count; i++)
        {
            RemoveStatus(statusToDelete[i]);
        }
        statusToDelete.Clear();

    }

    private void DeleteStatus(STATUS_TYPE statusType, bool triggerOnDeleteEffects = true)
    {
        if (statuses.ContainsKey(statusType) == true)
        {
            if (triggerOnDeleteEffects == true)
                OnDeleteStatus(statuses[statusType]);

            statuses.Remove(statusType);
        }

    }

    protected void copyBuffs(ref Dictionary<STATUS_TYPE, StatusData> newstatuses)
    {
        newstatuses = statuses;
    }

    public virtual bool IsDying()
    {
        return true;
    }

}