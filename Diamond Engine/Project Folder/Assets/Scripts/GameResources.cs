using System;
using DiamondEngine;
using System.Collections.Generic;

public class GameResources
{
    public GameResources(int _id, int _meshTextureID, RewardType _type, float _weight, string _description)
    {
        libraryTextureID = _id;
        meshTextureID = _meshTextureID;
        resourceType = _type;
        rewardDescription = _description;
        rngChanceWeight = _weight;
    }

    public virtual void Use() { }

    public int libraryTextureID;
    public int meshTextureID;
    public RewardType resourceType;
    public string rewardDescription;
    public float rngChanceWeight;
    public string name = "No name Boon";
    public ShopPrice price = ShopPrice.SHOP_FREE;
}

#region RESOURCES
public class BeskarResource : GameResources
{
    public BeskarResource() : base(1068400135, 1408880572, RewardType.REWARD_BESKAR, -1.0f, "The metal of the mandalorian people, second to none in the galaxy.") { }

    public override void Use()
    {
        PlayerResources.AddResourceBy1(RewardType.REWARD_BESKAR);
    }
}

public class MacaronResource : GameResources
{
    public MacaronResource() : base(694717696, 1358726487, RewardType.REWARD_MACARON, -1.0f, "Just a macaron. Grogu does love them, though.") { }

    public override void Use()
    {
        PlayerResources.AddResourceBy1(RewardType.REWARD_MACARON);
    }
}

public class ScrapResource : GameResources
{
    public ScrapResource() : base(268510122, 139772254, RewardType.REWARD_SCRAP, -1.0f, "Remains of powerful imperial technology.") { }

    public override void Use()
    {
        PlayerResources.AddResourceBy1(RewardType.REWARD_SCRAP);
    }
}

public class MilkResource : GameResources
{
    public MilkResource() : base(1314015482, 1389584724, RewardType.REWARD_MILK, -1.0f, "Sweet and tasty blue milk, a true delicacy.") { }

    public override void Use()
    {
        PlayerResources.AddResourceBy1(RewardType.REWARD_MILK);
    }
}
#endregion

// WE SHOULD PROBABLY CHANGE THE NAMES TO THE BOON'S ACTUAL NAME

#region BOONS
//Each time you kill an enemy heal +1 HP. - Bo Katan’s resilience
public class BokatanResilence : GameResources
{
    public BokatanResilence() : base(1885100100, 1175325466, RewardType.REWARD_BOON, 1.0f, "Each enemy kill heals 1 HP")
    {
        name = "Bo-Katan's Resilence";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance.gameObject.GetComponent<PlayerHealth>() != null)
        {
            int currentLifeSteal = Core.instance.gameObject.GetComponent<PlayerHealth>().IncrementHealingWhenKillingEnemy(1);
            Debug.Log("LifeSteal increased to: " + currentLifeSteal);

            PlayerResources.AddBoon(BOONS.BOON_BOKATAN_RESILENCE);
        }
        else
        {
            Debug.Log("ERROR!! Din Djarin has no player health component");
        }
    }
}

//+20% max HP. - Wrecker’s resilience
public class WreckerResilence : GameResources
{
    public WreckerResilence() : base(1789396100, 1586965694, RewardType.REWARD_BOON, 1.0f, "Increments +20 health")
    {
        name = "Wrecker's Resilence";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance.gameObject.GetComponent<PlayerHealth>() != null)
        {
            int toIncrement = 20;
            int currentMaxHp = Core.instance.gameObject.GetComponent<PlayerHealth>().IncrementMaxHpValue(toIncrement);
            int currentHp = Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage(-toIncrement);
            Debug.Log("Max HP increased to: " + currentMaxHp);
            Debug.Log("Curr HP increased to: " + currentHp);

            PlayerResources.AddBoon(BOONS.BOON_WRECKER_RESILIENCE);
        }
        else
        {
            Debug.Log("ERROR!! Din Djarin has no player health component");
        }
    }
}

public class CadBaneSoH : GameResources
{
    public CadBaneSoH() : base(405709743, 1749591820, RewardType.REWARD_BOON, 1.0f, "First attack with a different weapon from last attack deals +33% damage.")
    {
        name = "Cad Bane’s Sleight of Hand";
        price = ShopPrice.SHOP_CHEAP;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.CAD_BANE_SOH, STATUS_APPLY_TYPE.ADDITIVE, 30f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.CAD_BANE_SOH))
                Core.boons.Add(STATUS_TYPE.CAD_BANE_SOH);

            PlayerResources.AddBoon(BOONS.BOON_CADBANE_SLEIGHT);
        }
        //else
        //{
        //    Debug.Log("ERROR!! Din Djarin has no player health component");
        //}
    }
}


public class CadBaneBoots : GameResources
{
    public CadBaneBoots() : base(1747641289, 1770856290, RewardType.REWARD_BOON, 1.0f, "+5% permanent hasted state while grenade is on CD.")
    {
        name = "Cad Bane’s Rocket Boots";
        price = ShopPrice.SHOP_CHEAP;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.CAD_BANE_BOOTS, STATUS_APPLY_TYPE.ADDITIVE, 5f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.CAD_BANE_BOOTS))
                Core.boons.Add(STATUS_TYPE.CAD_BANE_BOOTS);

            PlayerResources.AddBoon(BOONS.BOON_CADBANE_ROCKET_BOOTS);
        }
    }
}

public class MandoQuickDraw : GameResources
{
    public MandoQuickDraw() : base(821239276, 837843542, RewardType.REWARD_BOON, 1.0f, "First attack after a dash does 25% more damage")
    {
        name = "Mandalorian's Quick Draw";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.MANDO_QUICK_DRAW, STATUS_APPLY_TYPE.ADDITIVE, 25f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.MANDO_QUICK_DRAW))
                Core.boons.Add(STATUS_TYPE.MANDO_QUICK_DRAW);

            PlayerResources.AddBoon(BOONS.BOON_MANDALORIAN_QUICK_DRAW);
        }

    }
}

public class BosskStrength : GameResources
{
    public BosskStrength() : base(1309422332, 1363306893, RewardType.REWARD_BOON, 1.0f, "Mitigate 10% of the damage received (rounded down).")
    {
        name = "Bossk's Strength";
        price = ShopPrice.SHOP_CHEAP;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.BOSSK_STR, STATUS_APPLY_TYPE.ADDITIVE, 10f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.BOSSK_STR))
                Core.boons.Add(STATUS_TYPE.BOSSK_STR);

            PlayerResources.AddBoon(BOONS.BOON_BOSSK_STRENGTH);
        }
    }
}
public class RexSecBlaster : GameResources
{
    public RexSecBlaster() : base(1417560378, 238792139, RewardType.REWARD_BOON, 1.0f, "When switching weapons, you get 5% additive haste for 5 seconds, stackable up to 20%.")
    {
        name = "Rex's Second Blaster";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.REX_SEC_BLASTER, STATUS_APPLY_TYPE.ADDITIVE, 5f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.REX_SEC_BLASTER))
                Core.boons.Add(STATUS_TYPE.REX_SEC_BLASTER);

            PlayerResources.AddBoon(BOONS.BOON_REX_SECOND_BLASTER);
        }
    }
}

public class GreedoShooter : GameResources
{
    public GreedoShooter() : base(1721324939, 1893664948, RewardType.REWARD_BOON, 1.0f, "+20% fire rate on the primary weapon")
    {
        name = "Greedo's Quick Shooter";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.GREEDO_SHOOTER, STATUS_APPLY_TYPE.ADDITIVE, -20f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.GREEDO_SHOOTER))
                Core.boons.Add(STATUS_TYPE.GREEDO_SHOOTER);

            PlayerResources.AddBoon(BOONS.BOON_GREEDO_QUICKSHOOTER);
        }
    }
}

public class FennecSniper : GameResources
{
    public FennecSniper() : base(25534683, 16741109, RewardType.REWARD_BOON, 1.0f, "Double rifle sweet spot time window (non-stackable).")
    {
        name = "Fennec's Sniper Rifle";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.FENNEC_SR, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, 100f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.FENNEC_SR))
                Core.boons.Add(STATUS_TYPE.FENNEC_SR);

            PlayerResources.AddBoon(BOONS.BOON_FENNEC_SNIPER_RIFLE);
        }
    }
}

public class AnakinKillstreak : GameResources
{
    public AnakinKillstreak() : base(96086378, 694089819, RewardType.REWARD_BOON, 1.0f, "While over a combo of 3, +5% additive Hasted")
    {
        name = "Anakin's Kill Streak";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.ANAKIN_KILLSTREAK, STATUS_APPLY_TYPE.ADDITIVE, 5f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.ANAKIN_KILLSTREAK))
                Core.boons.Add(STATUS_TYPE.ANAKIN_KILLSTREAK);

            PlayerResources.AddBoon(BOONS.BOON_ANAKIN_KILL_STREAK);
        }
    }
}

public class EchoRecovery : GameResources
{
    public EchoRecovery() : base(65608412, 912158733, RewardType.REWARD_BOON, 1.0f, " +10% additive hasted state for 5 seconds after losing a combo.")
    {
        name = "Echo's Quick Recovery";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.ECHO_RECOVERY, STATUS_APPLY_TYPE.ADDITIVE, 10f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.ECHO_RECOVERY))
                Core.boons.Add(STATUS_TYPE.ECHO_RECOVERY);

            PlayerResources.AddBoon(BOONS.BOON_ECHO_QUICK_RECOVERY);
        }
    }
}

public class WattoCoolant : GameResources
{
    public WattoCoolant() : base(1941353969, 116140543, RewardType.REWARD_BOON, 1.0f, "Dash cooldown is reduced by 40%")
    {
        name = "Watto's Coolant";
        price = ShopPrice.SHOP_CHEAP;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.WATTO_COOLANT, STATUS_APPLY_TYPE.ADDITIVE, 40f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.WATTO_COOLANT))
                Core.boons.Add(STATUS_TYPE.WATTO_COOLANT);

            PlayerResources.AddBoon(BOONS.BOON_WATTOS_COOLANT);
        }
    }
}

public class LuminaraForce : GameResources
{
    public LuminaraForce() : base(895016668, 1554417768, RewardType.REWARD_BOON, 1.0f, "Grogu’s dynamic covers lasts +5 seconds")
    {
        name = "Luminara's Force Control";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.LUMINARA_FORCE, STATUS_APPLY_TYPE.ADDITIVE, 4f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.LUMINARA_FORCE))
                Core.boons.Add(STATUS_TYPE.LUMINARA_FORCE);

            PlayerResources.AddBoon(BOONS.BOON_MASTER_LUMINARA_FORCE);
        }
    }
}
public class MandoCode : GameResources
{
    public MandoCode() : base(298014769, 325237486, RewardType.REWARD_BOON, 1.0f, "Deal +20% damage but have -20% less max health (stackable)")
    {
        name = "The Mandalorian's Code";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.MANDO_CODE, STATUS_APPLY_TYPE.ADDITIVE, 20f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.MANDO_CODE))
                Core.boons.Add(STATUS_TYPE.MANDO_CODE);

            PlayerHealth myHealth = Core.instance.gameObject.GetComponent<PlayerHealth>();
            if (myHealth != null)
            {
                
                myHealth.SetMaxHPValue((int)(PlayerHealth.currMaxHealth - 20 * PlayerHealth.currMaxHealth / 100), false);

            }
            PlayerResources.AddBoon(BOONS.BOON_MANDALORIAN_CODE);
        }
    }
}


public class ItsATrap : GameResources
{
    public ItsATrap() : base(561759673, 549360236, RewardType.REWARD_BOON, 1.0f, "Traps deal -33% damage to Mando")
    {
        name = "It's a trap!";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.ITSA_TRAP, STATUS_APPLY_TYPE.ADDITIVE, -33f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.ITSA_TRAP))
                Core.boons.Add(STATUS_TYPE.ITSA_TRAP);

            PlayerResources.AddBoon(BOONS.BOON_ITS_A_TRAP);
        }
    }
}
public class WreckHeavyShot : GameResources
{
    public WreckHeavyShot() : base(678900863, 544813248, RewardType.REWARD_BOON, 1.0f, "Hitting an enemy with a slowed status applies +25% non-stackable extra slowed status for 3 seconds.")
    {
        name = "Wrecker's Heavy Shot";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.WRECK_HEAVY_SHOT, STATUS_APPLY_TYPE.ADDITIVE, 25f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.WRECK_HEAVY_SHOT))
                Core.boons.Add(STATUS_TYPE.WRECK_HEAVY_SHOT);

            PlayerResources.AddBoon(BOONS.BOON_WRECKER_HEAVY_SHOT);
        }
    }
}

public class MandoQuickCombo : GameResources
{
    public MandoQuickCombo() : base(1246718882, 291438838, RewardType.REWARD_BOON, 1.0f, "Hitting an enemy with the primary weapon makes them take +5 % more damage from the primary weapon for 5 seconds. % stacks up to 100%")
    {
        name = "Mandalorian Quick Combo";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.QUICK_COMBO, STATUS_APPLY_TYPE.ADDITIVE, 5f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.QUICK_COMBO))
                Core.boons.Add(STATUS_TYPE.QUICK_COMBO);

            PlayerResources.AddBoon(BOONS.BOON_MANDALORIAN_QUICK_COMBO);
        }
    }
}
public class BlastCannonMouthpiece : GameResources
{
    public BlastCannonMouthpiece() : base(1675434209, 955589072, RewardType.REWARD_BOON, 1.0f, "Primary weapon does double damage while on 75% or more heat.")
    {
        name = "Blast Cannon Mouthpiece";
        price = ShopPrice.SHOP_CHEAP;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.BLAST_CANNON, STATUS_APPLY_TYPE.ADDITIVE, 100f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.BLAST_CANNON))
                Core.boons.Add(STATUS_TYPE.BLAST_CANNON);

            PlayerResources.AddBoon(BOONS.BOON_BLAST_CANNON_MOUTHPIECE);
        }
    }
}

public class BountyHunter : GameResources
{
    public BountyHunter() : base(463877375, 306967447, RewardType.REWARD_BOON, 1.0f, "Every time a room is cleared, +10 gold is received.")
    {
        name = "Bounty Hunter Negotiation Skills";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.BOUNTY_HUNTER, STATUS_APPLY_TYPE.ADDITIVE, 10f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.BOUNTY_HUNTER))
                Core.boons.Add(STATUS_TYPE.BOUNTY_HUNTER);

            PlayerResources.AddBoon(BOONS.BOON_BOUNTY_HUNTER_SKILLS);
        }
    }
}
public class BosskSpecialAmmo : GameResources
{
    public BosskSpecialAmmo() : base(1238459342, 1764866745, RewardType.REWARD_BOON, 1.0f, "Grenade does double damage as long as more than one enemy is getting hit by it.")
    {
        name = "Bossk Special Ammo";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.BOSSK_AMMO, STATUS_APPLY_TYPE.ADDITIVE, 100f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.BOSSK_AMMO))
                Core.boons.Add(STATUS_TYPE.BOSSK_AMMO);

            PlayerResources.AddBoon(BOONS.BOON_BOSSK_SPECIAL_AMMO);
        }
    }
}

public class WinduForceControl : GameResources
{
    public WinduForceControl() : base(569941476, 1976002657, RewardType.REWARD_BOON, 1.0f, "When you kill an enemy, gain Force points equal to 5% of max Force")
    {
        name = "Master Windu's Force Control";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.WINDU_FORCE, STATUS_APPLY_TYPE.ADDITIVE, 5f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.WINDU_FORCE))
                Core.boons.Add(STATUS_TYPE.WINDU_FORCE);

            PlayerResources.AddBoon(BOONS.BOON_MASTER_WINDU_FORCE);
        }
    }
}

public class YodaForceControl : GameResources
{
    public YodaForceControl() : base(840154977, 1331072530, RewardType.REWARD_BOON, 1.0f, "Grogu’s push pushes double the range.")
    {
        name = "Master Yoda's Force Control";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.YODA_FORCE, STATUS_APPLY_TYPE.ADDITIVE, 2f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.YODA_FORCE))
                Core.boons.Add(STATUS_TYPE.YODA_FORCE);

            PlayerResources.AddBoon(BOONS.BOON_MASTER_YODA_FORCE);
        }
    }
}

public class CrossHairLuckyShot : GameResources
{
    public CrossHairLuckyShot() : base(218162333, 550263805, RewardType.REWARD_BOON, 1.0f, "After impacting a sniper shot, you have a 33% chance to recover ammo, and next shot deals +33% more damage.")
    {
        name = "Crosshair's Lucky Shot";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.CROSS_HAIR_LUCKY_SHOT, STATUS_APPLY_TYPE.ADDITIVE, 33f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.CROSS_HAIR_LUCKY_SHOT))
                Core.boons.Add(STATUS_TYPE.CROSS_HAIR_LUCKY_SHOT);

            PlayerResources.AddBoon(BOONS.CROSSHAIR_LUCKY_SHOT);
        }
    }
}
public class AhsokaDetermination : GameResources
{
    public AhsokaDetermination() : base(724681876, 2053106866, RewardType.REWARD_BOON, 1.0f, "Killing an enemy with a sniper shot refills the bullet")
    {
        name = "Ahsoka's Determination";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.AHSOKA_DET, STATUS_APPLY_TYPE.ADDITIVE, 33f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.AHSOKA_DET))
                Core.boons.Add(STATUS_TYPE.AHSOKA_DET);

            PlayerResources.AddBoon(BOONS.BOON_AHSOKA_DETERMINATION);
        }
    }
}

public class BobbaStunAmmo : GameResources
{
    public BobbaStunAmmo() : base(535810690, 23397652, RewardType.REWARD_BOON, 1.0f, " Proc ratio and area on grenade boosted by 50%")
    {
        name = "Bobba Fett's Stun Ammo";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.BOBBA_STUN_AMMO, STATUS_APPLY_TYPE.ADDITIVE, 50f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.BOBBA_STUN_AMMO))
                Core.boons.Add(STATUS_TYPE.BOBBA_STUN_AMMO);

            PlayerResources.AddBoon(BOONS.BOON_BOBBA_FETT_STUN_AMM);
        }
    }
}
public class SoloQuickDraw : GameResources
{
    public SoloQuickDraw() : base(65111559, 1276597892, RewardType.REWARD_BOON, 1.0f, "When hit, heal 25% of the damage received if an enemy is hit back in less than 4 seconds.")
    {
        name = "Solo's Quick Draw";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.SOLO_QUICK_DRAW, STATUS_APPLY_TYPE.ADDITIVE, 50, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.SOLO_QUICK_DRAW))
                Core.boons.Add(STATUS_TYPE.SOLO_QUICK_DRAW);

            PlayerResources.AddBoon(BOONS.BOON_SOLO_QUICK_DRAW);
        }
    }
}
public class GeotermalMarker : GameResources
{
    public GeotermalMarker() : base(1781189074, 1596613015, RewardType.REWARD_BOON, 1.0f, "Any enemy affected by any kind of negative status takes +33% damage")
    {
        name = "Geotermal Marker";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.GEOTERMAL_MARKER, STATUS_APPLY_TYPE.ADDITIVE, 33f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.GEOTERMAL_MARKER))
                Core.boons.Add(STATUS_TYPE.GEOTERMAL_MARKER);

            PlayerResources.AddBoon(BOONS.BOON_GEOTERMAL_MARKER);
        }
    }
}
public class GreefPaycheck : GameResources
{
    public GreefPaycheck() : base(38657284, 531288084, RewardType.REWARD_BOON, 1.0f, "75% Discount on the next item bought in the shop")
    {
        name = "Greef's Paycheck";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.GREEF_PAYCHECK, STATUS_APPLY_TYPE.ADDITIVE, 1, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.GREEF_PAYCHECK))
                Core.boons.Add(STATUS_TYPE.GREEF_PAYCHECK);

            PlayerResources.AddBoon(BOONS.BOON_GREEF_PAYCHECK);
        }
    }
}
#endregion
static class BoonDataHolder
{
    static BoonDataHolder()
    {
        boonType[(int)BOONS.BOON_ANAKIN_KILL_STREAK] =         new AnakinKillstreak();
        boonType[(int)BOONS.BOON_BOKATAN_RESILENCE] =          new BokatanResilence();
        boonType[(int)BOONS.BOON_BOSSK_STRENGTH] =             new BosskStrength();
        boonType[(int)BOONS.BOON_BOUNTY_HUNTER_SKILLS] =       new BountyHunter();
        boonType[(int)BOONS.BOON_CADBANE_ROCKET_BOOTS] =       new CadBaneBoots();
        boonType[(int)BOONS.BOON_CADBANE_SLEIGHT] =            new CadBaneSoH();
        boonType[(int)BOONS.BOON_ECHO_QUICK_RECOVERY] =        new EchoRecovery();
        boonType[(int)BOONS.BOON_GREEF_PAYCHECK] =             new GreefPaycheck();     
        boonType[(int)BOONS.BOON_ITS_A_TRAP] =                 new ItsATrap();
        boonType[(int)BOONS.BOON_MANDALORIAN_QUICK_DRAW] =     new MandoQuickDraw();
        boonType[(int)BOONS.BOON_REX_SECOND_BLASTER] =         new RexSecBlaster();
        boonType[(int)BOONS.BOON_SOLO_QUICK_DRAW] =            new SoloQuickDraw();     
        boonType[(int)BOONS.BOON_WATTOS_COOLANT] =             new WattoCoolant();
        boonType[(int)BOONS.BOON_WRECKER_HEAVY_SHOT] =         new WreckHeavyShot();
        boonType[(int)BOONS.BOON_WRECKER_RESILIENCE] =         new WreckerResilence();
        boonType[(int)BOONS.BOON_BLAST_CANNON_MOUTHPIECE] =    new BlastCannonMouthpiece();
        boonType[(int)BOONS.BOON_GREEDO_QUICKSHOOTER] =        new GreedoShooter();
        boonType[(int)BOONS.BOON_MANDALORIAN_QUICK_COMBO] =    new MandoQuickCombo();
        boonType[(int)BOONS.BOON_FENNEC_SNIPER_RIFLE] =        new FennecSniper();
        boonType[(int)BOONS.BOON_BOBBA_FETT_STUN_AMM] =        new BobbaStunAmmo();    
        boonType[(int)BOONS.BOON_BOSSK_SPECIAL_AMMO] =         new BosskSpecialAmmo();
        boonType[(int)BOONS.BOON_GEOTERMAL_MARKER] =           new GeotermalMarker(); 
        boonType[(int)BOONS.BOON_MASTER_LUMINARA_FORCE] =      new LuminaraForce();
        boonType[(int)BOONS.BOON_MASTER_WINDU_FORCE] =         new WinduForceControl();
        boonType[(int)BOONS.BOON_MASTER_YODA_FORCE] =          new YodaForceControl();
        boonType[(int)BOONS.BOON_MANDALORIAN_CODE] =           new MandoCode();
        boonType[(int)BOONS.CROSSHAIR_LUCKY_SHOT] =            new CrossHairLuckyShot();
        boonType[(int)BOONS.BOON_AHSOKA_DETERMINATION] =       new AhsokaDetermination();

        for (int i = 0; i < boonType.Length; i++)
        {
            float auxWeight = boonType[i].rngChanceWeight;
            boonType[i].rngChanceWeight = boonTotalWeights;
            boonTotalWeights += auxWeight;
        }
    }

    public static GameResources[] boonType = new GameResources[(int)BOONS.BOON_MAX];

    public static float boonTotalWeights;

}
