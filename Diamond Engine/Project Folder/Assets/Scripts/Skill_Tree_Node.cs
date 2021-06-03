using System;
using DiamondEngine;

public class Skill_Tree_Node : DiamondComponent
{

    public enum NODE_STATE
    {
        UNLOCKED,
        LOCKED,
        OWNED,
    };

    #region Style
    public int buttonPressed = 0;
    public int unlockedButtonHovered = 0;
    public int unlockedButtonUnhovered = 0;

    public int lockedButtonHovered = 0;
    public int lockedButtonUnhovered = 0;

    public int ownedButtonHovered = 0;
    public int ownedButtonUnhovered = 0;
    #endregion

    #region Logic
    public string skill_name = "";

    public GameObject children_1 = null;
    public GameObject children_2 = null;
    public GameObject parent_1 = null;
    public GameObject parent_2 = null;
    public GameObject oppositeNode = null;

    public GameObject text_description = null;
    public GameObject hub_skill_controller = null; 

    private Skills skill = null;

    public bool isRootNode = false;
    private NODE_STATE _state;

    public int node_type = 0;

    public GameObject panelImage;
    public int skillTreeName;
    public int skillTreeNumber;

    public NODE_STATE state 
    {
        get { return _state; }
        set 
        {
            _state = value;
            switch (value)
            {
                case NODE_STATE.UNLOCKED:                    
                    gameObject.GetComponent<Button>().ChangeSprites(buttonPressed, unlockedButtonHovered, unlockedButtonUnhovered);
                    break;
                case NODE_STATE.LOCKED:
                    gameObject.GetComponent<Button>().ChangeSprites(lockedButtonHovered, lockedButtonHovered, lockedButtonUnhovered);
                    break;
                case NODE_STATE.OWNED:
                    gameObject.GetComponent<Button>().ChangeSprites(buttonPressed, ownedButtonHovered, ownedButtonUnhovered);
                    break;
                default:
                    break;
            }
        }
    }
    #endregion

    public void Awake()
    {
        UnlockTreeAfterRun();

        AssignCharacteristics();
    }

    public void Reset()
    {
        UnlockTreeAfterRun();
        if (children_1 != null)
            children_1.GetComponent<Skill_Tree_Node>().Reset();
        if (children_2 != null)
            children_2.GetComponent<Skill_Tree_Node>().Reset();
    }

    //Remember the skills that have already been bought before the run
    private void UnlockTreeAfterRun()
    {
        if (Skill_Tree_Data.IsEnabled(skillTreeName, skillTreeNumber)) //Edge case: The node is already OWNED
        {
            state = NODE_STATE.OWNED;
        }
        else if (oppositeNode != null) //Edge case: Nodes with opposite
        {
            if (oppositeNode.GetComponent<Skill_Tree_Node>().skillTreeNumber > skillTreeNumber) //oppositeNode = right node
            {
                if (Skill_Tree_Data.IsEnabled(skillTreeName, skillTreeNumber + 1))
                    state = NODE_STATE.LOCKED;
                else if (isRootNode)
                    state = NODE_STATE.UNLOCKED;
                else if (Skill_Tree_Data.IsEnabled(skillTreeName, skillTreeNumber - 1)) //Check if parent is enabled
                    state = NODE_STATE.UNLOCKED;
                else
                    state = NODE_STATE.LOCKED;
            }
            else if (oppositeNode.GetComponent<Skill_Tree_Node>().skillTreeNumber < skillTreeNumber) //oppositeNode = left node
            {
                if (Skill_Tree_Data.IsEnabled(skillTreeName, skillTreeNumber + -1))
                    state = NODE_STATE.LOCKED;
                else if (isRootNode) 
                    state = NODE_STATE.UNLOCKED;
                else if (Skill_Tree_Data.IsEnabled(skillTreeName, skillTreeNumber - 2)) //Check if parent is enabled
                    state = NODE_STATE.UNLOCKED;
                else
                    state = NODE_STATE.LOCKED;
            }
            else
            {
                state = NODE_STATE.LOCKED;
            }
        }
        else //Edge case: Nodes without opposite
        {
            if (isRootNode && Skill_Tree_Data.IsEnabled(skillTreeName, skillTreeNumber) == false)
                state = NODE_STATE.UNLOCKED;
            else if(parent_2 == null) //Only has one parent
            {
                if (Skill_Tree_Data.IsEnabled(skillTreeName, skillTreeNumber - 1)) //Parent is already OWNED
                    state = NODE_STATE.UNLOCKED;
                else
                    state = NODE_STATE.LOCKED;
            }
            else if(parent_1 != null && parent_2 != null) //It has two parents
            {
                if (Skill_Tree_Data.IsEnabled(skillTreeName, skillTreeNumber - 1) || Skill_Tree_Data.IsEnabled(skillTreeName, skillTreeNumber - 2))
                    state = NODE_STATE.UNLOCKED;
                else
                    state = NODE_STATE.LOCKED;
            }
            else //Both parents are NULL
            {
                state = NODE_STATE.LOCKED; 
            }
        }
    }

    public void Update()
	{
        if (skill == null)
            return;

        if (gameObject.GetComponent<Navigation>().is_selected == false)
            return;

        if (hub_skill_controller == null)
            return;

        if (hub_skill_controller.GetComponent<HubSkillTreeController>().skill_selected == skill)
            return;

        hub_skill_controller.GetComponent<HubSkillTreeController>().skill_selected = skill;

        if (text_description != null)
            text_description.GetComponent<Text>().text = skill.description;

        if(panelImage != null)
            panelImage.GetComponent<Hub_PanelImage>().UpdateIcon(skillTreeName, skillTreeNumber - 1);                  

        if (Input.GetGamepadButton(DEControllerButton.Y) == KeyState.KEY_DOWN)
        {
            Debug.Log("Beskar: ");
            PlayerResources.AddResourceBy1(RewardType.REWARD_BESKAR);
            Debug.Log("Beskar: " + PlayerResources.GetResourceCount(RewardType.REWARD_BESKAR));
        }
    }

    private void AssignCharacteristics()
    {
        if (SkillDictionary.skill_type.ContainsKey(skill_name))
        {
            Type t = SkillDictionary.skill_type[skill_name];
            skill = (Skills)Activator.CreateInstance(t);
            skill.AssignCharacteristics();
        }
        else
        {
            Debug.Log("ERROR: Skill doesn't exist: " + skill_name);
        }

        if (node_type == 0)
            skill.type_of_price = RewardType.REWARD_BESKAR;
        else if (node_type == 1)
            skill.type_of_price = RewardType.REWARD_MACARON;
        else
            skill.type_of_price = RewardType.REWARD_SCRAP;        
    }

    public void OnExecuteButton()
    {
        if (state == NODE_STATE.OWNED)
            return;

        if (state == NODE_STATE.LOCKED)
        {
            Debug.Log("Locked node");
            Audio.PlayAudio(gameObject, "Play_UI_Skill_Tree_Lock");
            return;
        }


        if (skill.type_of_price == RewardType.REWARD_BESKAR)
        {
            if (PlayerResources.GetResourceCount(RewardType.REWARD_BESKAR) < skill.price)
            {
                Audio.PlayAudio(gameObject, "Play_UI_Skill_Tree_Lock");
                Debug.Log("You don't have enough Beskar!");
                return;
            }
            else
            {
                PlayerResources.SubstractResource(RewardType.REWARD_BESKAR, skill.price);
                Debug.Log("Beskar: " + PlayerResources.GetResourceCount(RewardType.REWARD_BESKAR));
            }
        }

        if (skill.type_of_price == RewardType.REWARD_MACARON)
        {
            if (PlayerResources.GetResourceCount(RewardType.REWARD_MACARON) < skill.price)
            {
                Audio.PlayAudio(gameObject, "Play_UI_Skill_Tree_Lock");
                Debug.Log("You don't have enough Macarons!");
                return;
            }
            else
            {
                PlayerResources.SubstractResource(RewardType.REWARD_MACARON, skill.price);
                Debug.Log("Macarons: " + PlayerResources.GetResourceCount(RewardType.REWARD_MACARON));
            }
        }

        if (skill.type_of_price == RewardType.REWARD_SCRAP)
        {
            if (PlayerResources.GetResourceCount(RewardType.REWARD_SCRAP) < skill.price)
            {
                Audio.PlayAudio(gameObject, "Play_UI_Skill_Tree_Lock");
                Debug.Log("You don't have enough Scrap!");
                return;
            }
            else
            {
                PlayerResources.SubstractResource(RewardType.REWARD_SCRAP, skill.price);
                Debug.Log("Scrap: " + PlayerResources.GetResourceCount(RewardType.REWARD_SCRAP));
            }
        }
        skill.Use();
        addSkill(skillTreeName,skill_name);
        state = NODE_STATE.OWNED;

        if (children_1 != null)
            children_1.GetComponent<Skill_Tree_Node>().state = NODE_STATE.UNLOCKED;

        if (children_2 != null)
            children_2.GetComponent<Skill_Tree_Node>().state = NODE_STATE.UNLOCKED;

        if(oppositeNode != null)
            oppositeNode.GetComponent<Skill_Tree_Node>().state = NODE_STATE.LOCKED;
    }

    public static void addSkill(int skillTree, string name)
    {
        Debug.Log(skillTree.ToString() + " / " + skillTree.ToString());
        switch(skillTree)
        {
            case 1:
                switch (name)
                {
                    case "GForceReg":
                        Core.instance.AddStatus(STATUS_TYPE.GRO_FORCE_REGEN, STATUS_APPLY_TYPE.SUBSTITUTE, 1, 0, true);
                      //  Debug.Log("GForceReg not implemented yet");
                        break;
                    case "GPushRange":
                        Core.instance.AddStatus(STATUS_TYPE.GRO_PUSH, STATUS_APPLY_TYPE.SUBSTITUTE, 3, 0, true);
                      //  Debug.Log("GPushRange not implemented yet");
                        break;
                    case "GForceDur":
                        Core.instance.AddStatus(STATUS_TYPE.GRO_COVER, STATUS_APPLY_TYPE.SUBSTITUTE, 3, 0, true);
                      //  Debug.Log("GForceDur heat not implemented yet");
                        break;
                    case "GThrshComboForce":
                        Core.instance.AddStatus(STATUS_TYPE.GRO_COMBO_REGEN, STATUS_APPLY_TYPE.SUBSTITUTE, 4, 0, true);
                      //  Debug.Log("GThrshComboForce not implemented yet");
                        break;
                    case "GMaxForce":
                        Core.instance.AddStatus(STATUS_TYPE.GRO_MAX_FORCE, STATUS_APPLY_TYPE.SUBSTITUTE, 10, 0, true);
                       // Debug.Log("GMaxForce not implemented yet");
                        break;
                    case "GSingComboForce":
                        Core.instance.AddStatus(STATUS_TYPE.GRO_COMBO_ADD, STATUS_APPLY_TYPE.SUBSTITUTE, 2, 0, true);
                       // Debug.Log("GSingComboForce not implemented yet");
                        break;
                    case "GComboGain":
                        Core.instance.AddStatus(STATUS_TYPE.GRO_COMBO_GAIN, STATUS_APPLY_TYPE.SUBSTITUTE, 30, 0, true);
                       // Debug.Log("GComboGain not implemented yet");
                        break;
                    case "GHPForceReg":
                        Core.instance.AddStatus(STATUS_TYPE.GRO_FORCE_PER_HP, STATUS_APPLY_TYPE.SUBSTITUTE, 1, 0, true);
                       // Debug.Log("GHPForceReg heat not implemented yet");
                        break;
                }
                break;
            case 2:
                switch (name)
                {
                    #region utility
                    case "UDamageHeat":
                        //Debug.Log("Damage * heat not implemented yet");
                        Core.instance.AddStatus(STATUS_TYPE.DMG_PER_HEAT, STATUS_APPLY_TYPE.SUBSTITUTE, 1, 0, true);
                        break;
                    case "UMovSpd":
                        //Debug.Log("MovementSpeed not implemented yet");
                        Core.instance.AddStatus(STATUS_TYPE.MOV_SPEED, STATUS_APPLY_TYPE.SUBSTITUTE, 20, 0, true);
                        break;
                    case "USlowDamage":
                        Core.instance.AddStatus(STATUS_TYPE.REFILL_CHANCE, STATUS_APPLY_TYPE.SUBSTITUTE, 100, 0, true);
                        break;
                    case "UFallDmgRed":
                        Core.instance.AddStatus(STATUS_TYPE.FALL, STATUS_APPLY_TYPE.SUBSTITUTE, 1, 0, true);
                      //  Debug.Log("Damage reduction after dash not implemented yet");
                        break;
                    case "UOverheat":
                        //Debug.Log("More Shots before overheat not implemented yet");
                        Core.instance.AddStatus(STATUS_TYPE.OVERHEATCAP, STATUS_APPLY_TYPE.ADDITIVE, -20, 0, true);
                        break;
                    case "URedGroguCost":
                      //  Debug.Log("Grogu Cost reduced");
                        Core.instance.AddStatus(STATUS_TYPE.GROGU_COST, STATUS_APPLY_TYPE.SUBSTITUTE, -20, 0, true);
                      //  Debug.Log("Grogu reduction cost not implemented yet");
                        break;
                    case "UHeal":
                        // Debug.Log("Gogu heal not implemented yet");
                        Core.instance.AddStatus(STATUS_TYPE.SKILL_HEAL, STATUS_APPLY_TYPE.SUBSTITUTE, 5, 1, true);
                        break;
                    case "UDash":
                        //Debug.Log("Double dash not implemented yet");
                        Core.instance.AddStatus(STATUS_TYPE.GRENADE_ON_DASH, STATUS_APPLY_TYPE.SUBSTITUTE, 10, 1, true);

                        break;
                    #endregion
                    #region Aggression
                    case "ABlasterDmg":
                       // Debug.Log("Damage not implemented yet");
                        Core.instance.AddStatus(STATUS_TYPE.BLASTER_DAMAGE, STATUS_APPLY_TYPE.ADDITIVE, 15, 0, true);
                        break;
                    case "AComboFireRate":
                        // Debug.Log("Combo * speed not implemented yet");
                        Core.instance.AddStatus(STATUS_TYPE.COMBO_FIRE_RATE, STATUS_APPLY_TYPE.ADDITIVE, 20, 0, true);
                        break;
                    case "AComboDmg":
                       // Debug.Log("Combo * damage not implemented yet");
                        Core.instance.AddStatus(STATUS_TYPE.COMBO_DAMAGE, STATUS_APPLY_TYPE.SUBSTITUTE, 20, 0, true);
                        break;
                    case "AGrenadeDmg":
                        //Debug.Log("Critical chance not implemented yet");
                        Core.instance.AddStatus(STATUS_TYPE.GRENADE_DAMAGE, STATUS_APPLY_TYPE.ADDITIVE, 15, 0, true);

                        break;
                    case "ASniperDmg":
                        //Debug.Log("Critical damage not implemented yet");
                        Core.instance.AddStatus(STATUS_TYPE.SNIPER_DAMAGE, STATUS_APPLY_TYPE.SUBSTITUTE, 15, 0, true);

                        break;
                    case "ADmgBos":
                        Core.instance.AddStatus(STATUS_TYPE.DMG_TO_BOSSES, STATUS_APPLY_TYPE.SUBSTITUTE, 20, 0, true);
                       // Debug.Log("Bonus damage to bosses not implemented yet");
                        break;
                    case "AHPMissDmgBlaster":
                        Core.instance.AddStatus(STATUS_TYPE.BLAST_DMG_PER_HP, STATUS_APPLY_TYPE.SUBSTITUTE, 1, 0, true);
                        //Debug.Log("Damage * missing hp not implemented yet");
                        break;
                    case "AHPMissDmgSniper":
                        Core.instance.AddStatus(STATUS_TYPE.SNIPER_DMG_PER_HP, STATUS_APPLY_TYPE.SUBSTITUTE, 1, 0, true);
                        //  Debug.Log("Crit * missing hp not implemented yet");
                        break;
                    #endregion
                    #region Defense
                    case "DMaxHPS":
                        Core.instance.AddStatus(STATUS_TYPE.MAX_HP, STATUS_APPLY_TYPE.SUBSTITUTE, 10, 0, true);
                        // Debug.Log("Max Hp not implemented yet");
                        break;
                    case "DDmgRed":
                        Core.instance.AddStatus(STATUS_TYPE.DMG_RED, STATUS_APPLY_TYPE.SUBSTITUTE, -10, 0, true);

                        //   Debug.Log("Dmg reduction not implemented yet");
                        break;
                    case "DComboDmgRed":
                        Core.instance.AddStatus(STATUS_TYPE.COMBO_RED, STATUS_APPLY_TYPE.SUBSTITUTE, 20, 0, true);
                      //  Debug.Log("Combo * damage red not implemented yet");
                        break;
                    case "DComboHeal":
                        Core.instance.AddStatus(STATUS_TYPE.COMBO_HEAL, STATUS_APPLY_TYPE.SUBSTITUTE, 2, 0, true);
                     //   Debug.Log("Combo * heal not implemented yet");
                        break;
                    case "DComboFnshHeal":
                        Core.instance.AddStatus(STATUS_TYPE.HEAL_COMBO_FINNISH, STATUS_APPLY_TYPE.SUBSTITUTE, 10, 0, true);
                        //Debug.Log("Heal on combo finish not implemented yet");
                        break;
                    case "DHeal":
                        Core.instance.AddStatus(STATUS_TYPE.LIFESTEAL, STATUS_APPLY_TYPE.SUBSTITUTE, 5, 0, true);
                        //Debug.Log("Lifesteal not implemented yet");
                        break;
                    case "DHealEfct":
                      //  Debug.Log("Healing effects increased not implemented yet");
                        Core.instance.AddStatus(STATUS_TYPE.REVIVE, STATUS_APPLY_TYPE.SUBSTITUTE, 1, 0, true);
                        break;
                    case "DAvoidDmg":
                        Core.instance.AddStatus(STATUS_TYPE.BLOCK, STATUS_APPLY_TYPE.SUBSTITUTE, 5, 0, true);
                     //   Debug.Log("chance to avoid damage not implemented yet");
                        break;
                        #endregion
                }
                break;
            case 3:
                switch (name)
                {
                    #region primary

                    case "PMovSpd":
                        Core.instance.AddStatus(STATUS_TYPE.PRIM_SPEED, STATUS_APPLY_TYPE.SUBSTITUTE, 30, 1, true);
                        break;
                    case "PProjRange":
                        Core.instance.AddStatus(STATUS_TYPE.PRIM_MOV_SPEED, STATUS_APPLY_TYPE.SUBSTITUTE, 20, 1, true);
                        break;
                    case "PSlowEnem":
                        Core.instance.AddStatus(STATUS_TYPE.PRIM_SLOW, STATUS_APPLY_TYPE.SUBSTITUTE, 20, 1, true);
                        break;
                    case "PHeat":
                        Core.instance.AddStatus(STATUS_TYPE.OVERHEAT, STATUS_APPLY_TYPE.SUBSTITUTE, 30, 1, true);
                        //WIP
                        break;
                    case "PChargedBullet":
                        Core.instance.AddStatus(STATUS_TYPE.PRIM_CHARGED, STATUS_APPLY_TYPE.SUBSTITUTE, 5, 1, true);
                        break;
                    case "PIncDmg":
                        Core.instance.AddStatus(STATUS_TYPE.BLASTER_DAMAGE, STATUS_APPLY_TYPE.ADDITIVE, 25, 0, true);

                        break;
                    case "PRateFire":
                        Core.instance.AddStatus(STATUS_TYPE.OVERHEATCAP, STATUS_APPLY_TYPE.ADDITIVE, -30, 0, true);
                        Core.instance.AddStatus(STATUS_TYPE.FIRE_RATE_PERMANENT, STATUS_APPLY_TYPE.ADDITIVE, -10, 0, true);

                        break;
                    #endregion
                    #region secondary
                    case "SeStatEfct":
                        Core.instance.AddStatus(STATUS_TYPE.SEC_TICKDMG, STATUS_APPLY_TYPE.ADDITIVE, 2, 0, true);
                        break;
                    case "SeSlow":
                        Core.instance.AddStatus(STATUS_TYPE.SEC_SLOW, STATUS_APPLY_TYPE.SUBSTITUTE, 20, 1, true);
                        break;
                    case "SeDelay":
                        Core.instance.AddStatus(STATUS_TYPE.SEC_RECOVERY, STATUS_APPLY_TYPE.SUBSTITUTE, 30, 1, true);
                        break;
                    case "SeUseRange":
                        Core.instance.AddStatus(STATUS_TYPE.SEC_RANGE, STATUS_APPLY_TYPE.SUBSTITUTE, 30, 1, true);
                        Debug.Log("rework on grenade?");
                        break;
                    case "SeDuration":
                        Core.instance.AddStatus(STATUS_TYPE.SEC_DURATION, STATUS_APPLY_TYPE.SUBSTITUTE, 30, 1, true);
                        break;
                    case "SeDmgInc":
                        Core.instance.AddStatus(STATUS_TYPE.GRENADE_DAMAGE, STATUS_APPLY_TYPE.ADDITIVE, 50, 1, true);
                        break;
                    #endregion
                    #region special
                    case "SpNonCharged":
                        Core.instance.AddStatus(STATUS_TYPE.SP_NONCHARGED, STATUS_APPLY_TYPE.SUBSTITUTE, 50, 0, true);

                        break;
                    case "SpChargeTime":
                        Core.instance.AddStatus(STATUS_TYPE.SP_CHARGE_TIME, STATUS_APPLY_TYPE.SUBSTITUTE, 30, 0, true);

                        break;
                    case "SpHeal":
                        Core.instance.AddStatus(STATUS_TYPE.SP_HEAL, STATUS_APPLY_TYPE.SUBSTITUTE, 5, 0, true);

                        break;
                    case "SpRegForce":
                        Core.instance.AddStatus(STATUS_TYPE.SP_FORCE_REGEN, STATUS_APPLY_TYPE.SUBSTITUTE, 5, 0, true);

                        break;
                    case "SpMaxIntDmg":
                        Core.instance.AddStatus(STATUS_TYPE.SP_INTERVAL, STATUS_APPLY_TYPE.SUBSTITUTE, 50, 0, true);

                        break;
                    case "SpMaxDmg":
                        Core.instance.AddStatus(STATUS_TYPE.SP_DAMAGE_CHARGED, STATUS_APPLY_TYPE.SUBSTITUTE, 100, 0, true);

                        break;
                        #endregion

                }
                break;
        }
        
    }
}