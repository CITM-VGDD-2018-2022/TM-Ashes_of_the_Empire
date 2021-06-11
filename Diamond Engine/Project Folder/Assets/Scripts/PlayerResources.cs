using System;
using DiamondEngine;
using System.Collections.Generic;

public enum BOONS
{
    BOON_ANAKIN_KILL_STREAK = 0,
    BOON_BOKATAN_RESILENCE,
    BOON_BOSSK_STRENGTH,
    BOON_BOUNTY_HUNTER_SKILLS,
    BOON_CADBANE_ROCKET_BOOTS,
    BOON_CADBANE_SLEIGHT,
    BOON_ECHO_QUICK_RECOVERY,
    BOON_GREEF_PAYCHECK,
    BOON_ITS_A_TRAP,
    BOON_MANDALORIAN_QUICK_DRAW,
    BOON_REX_SECOND_BLASTER,
    BOON_SOLO_QUICK_DRAW,
    BOON_WATTOS_COOLANT,
    BOON_WRECKER_HEAVY_SHOT,
    BOON_WRECKER_RESILIENCE,
    BOON_BLAST_CANNON_MOUTHPIECE,
    BOON_GREEDO_QUICKSHOOTER,
    BOON_MANDALORIAN_QUICK_COMBO,
    BOON_AHSOKA_DETERMINATION,
    BOON_FENNEC_SNIPER_RIFLE,
    BOON_BOBBA_FETT_STUN_AMM,
    BOON_BOSSK_SPECIAL_AMMO,
    BOON_GEOTERMAL_MARKER,
    BOON_MASTER_LUMINARA_FORCE,
    BOON_MASTER_WINDU_FORCE,
    BOON_MASTER_YODA_FORCE,
    BOON_MANDALORIAN_CODE,
    CROSSHAIR_LUCKY_SHOT,
    BOON_MAX
}

public static class PlayerResources
{
    static int beskarCounter = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("beskarCounter") : 0;
    static int macaronCounter = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("macaronCounter") : 0;
    static int milkCounter = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("milkCounter") : 0;
    static int scrapCounter = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("scrapCounter") : 0;
    static int runCoins = 0;
    static bool[] accquiredBoons = new bool[(int)BOONS.BOON_MAX];
    static Dictionary<BOONS, int> boonCounter = new Dictionary<BOONS, int>();


    public static int GetResourceCount(RewardType type, Type boonType = null)
    {
        int auxCounter = 0;

        switch (type)
        {
            case RewardType.REWARD_BOON:
                auxCounter = boonCounter.Count;
                break;

            case RewardType.REWARD_BESKAR:
                auxCounter = beskarCounter;
                break;

            case RewardType.REWARD_MACARON:
                auxCounter = macaronCounter;
                break;

            case RewardType.REWARD_SCRAP:
                auxCounter = scrapCounter;
                break;

            case RewardType.REWARD_MILK:
                auxCounter = milkCounter;
                break;
        }

        return auxCounter;
    }

    public static int GetBoonAmount(BOONS boon)
    {
        if (boonCounter.ContainsKey(boon))
            return boonCounter[boon];
        else
            return 0;
    }
    public static void ResetHoldingBoons()
    {
        for (int i = 0; i < accquiredBoons.Length; i++)
        {
            accquiredBoons[i] = false;
        }
    }

    public static void AddBoon(BOONS newBoon)
    {
        if (!boonCounter.ContainsKey(newBoon))
            boonCounter.Add(newBoon, 1);
        else
            boonCounter[newBoon] += 1;
    }

    public static bool CheckBoon(BOONS newBoon)
    {
        return boonCounter.ContainsKey(newBoon);
    }

    public static int AddResourceBy1(RewardType type, Type boonType = null)
    {
        int resourceLeft = 0;

        switch (type)
        {
            //case RewardType.REWARD_BOON:
            //    resourceLeft = AddBoonToMap(1, boonType);
            //    break;

            case RewardType.REWARD_BESKAR:
                resourceLeft = ++beskarCounter;
                DiamondPrefs.Write("beskarCounter", beskarCounter);
                break;

            case RewardType.REWARD_MACARON:
                resourceLeft = ++macaronCounter;
                DiamondPrefs.Write("macaronCounter", macaronCounter);
                break;

            case RewardType.REWARD_SCRAP:
                resourceLeft = ++scrapCounter;
                DiamondPrefs.Write("scrapCounter", scrapCounter);
                break;

            case RewardType.REWARD_MILK:
                resourceLeft = ++milkCounter;
                DiamondPrefs.Write("milkCounter", milkCounter);
                break;
        }

        return resourceLeft;
    }

    public static int AddResourceByValue(RewardType type, int num)
    {
        int resourceLeft = 0;

        switch (type)
        {
            case RewardType.REWARD_BESKAR:
                beskarCounter += num;
                resourceLeft = beskarCounter;
                DiamondPrefs.Write("beskarCounter", beskarCounter);
                break;

            case RewardType.REWARD_MACARON:
                macaronCounter += num;
                resourceLeft = macaronCounter;
                DiamondPrefs.Write("macaronCounter", macaronCounter);
                break;

            case RewardType.REWARD_SCRAP:
                scrapCounter += num;
                resourceLeft = scrapCounter;
                DiamondPrefs.Write("scrapCounter", scrapCounter);
                break;

            case RewardType.REWARD_MILK:
                milkCounter += num;
                resourceLeft = milkCounter;
                DiamondPrefs.Write("milkCounter", milkCounter);
                break;
        }

        return resourceLeft;
    }

    public static int SubstractResource(RewardType type, int total_to_substract, Type boonType = null)
    {
        int resourceLeft = 0;
        if (total_to_substract <= 0)
            return resourceLeft;

        switch (type)
        {
            //case RewardType.REWARD_BOON:
            //    resourceLeft = AddBoonToMap(-total_to_substract, boonType);
            //    break;

            case RewardType.REWARD_BESKAR:
                if (beskarCounter > 0)
                {
                    beskarCounter = beskarCounter - total_to_substract;
                    resourceLeft = beskarCounter;
                    DiamondPrefs.Write("beskarCounter", beskarCounter);
                }
                break;

            case RewardType.REWARD_MACARON:
                if (macaronCounter > 0)
                {
                    macaronCounter = macaronCounter - (total_to_substract);
                    resourceLeft = macaronCounter;
                    DiamondPrefs.Write("macaronCounter", macaronCounter);
                }
                break;

            case RewardType.REWARD_SCRAP:
                if (scrapCounter > 0)
                {
                    scrapCounter = scrapCounter - total_to_substract;
                    resourceLeft = scrapCounter;
                    DiamondPrefs.Write("scrapCounter", scrapCounter);
                }
                break;

            case RewardType.REWARD_MILK:
                if (milkCounter > 0)
                {
                    milkCounter = milkCounter - total_to_substract;
                    resourceLeft = milkCounter;
                    DiamondPrefs.Write("milkCounter", milkCounter);
                }
                break;
        }

        return resourceLeft;
    }

    public static int GetRunCoins()
    {
        return runCoins;
    }

    public static void AddRunCoins(int val)
    {
        runCoins += val;

        for (int i = 0; i < val; i++)
        {
            Counter.SumToCounterType(Counter.CounterTypes.RUN_COINS);
        }
    }

    public static void SetRunCoins(int val)
    {
        runCoins = val;

        for (int i = 0; i < val; i++)
        {
            Counter.SumToCounterType(Counter.CounterTypes.RUN_COINS);
        }
    }

    public static void ResetRunBoons()
    {
        boonCounter.Clear();
    }

    public static int GetBoonsAmount()
    {
        if(boonCounter != null)
            return boonCounter.Count;

        return 0;
    }

    public static void ResetResources()
    {
        beskarCounter = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("beskarCounter") : 0;
        macaronCounter = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("macaronCounter") : 0;
        milkCounter = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("milkCounter") : 0;
        scrapCounter = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("scrapCounter") : 0;
        runCoins = 0;
    }

}