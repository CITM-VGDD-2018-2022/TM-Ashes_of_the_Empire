using System;
using DiamondEngine;

public enum RewardType
{
    REWARD_BOON,
    REWARD_BESKAR,
    REWARD_MACARON,
    REWARD_SCRAP,
    REWARD_MILK
}

public class EndLevelRewards
{
    public struct EndLevelReward
    {
        public EndLevelReward(int index, RewardType rewardType)
        {
            boonIndex = index;
            type = rewardType;
        }

        public int boonIndex;
        public RewardType type;
    }

    int[] rewardChances = new int[5] { 80, 5, 5, 5, 5 }; // Reward chances, by order being boons, beskar, macarons, scraps and milk

    EndLevelReward firstReward; // Could probably make an array for this... UwU
    EndLevelReward secondReward;
    EndLevelReward thirdReward;

    EndLevelRewardsButtons firstButton = null; // Could probably make an array for this... UwU
    EndLevelRewardsButtons secondButton = null;
    EndLevelRewardsButtons thirdButton = null;

    public GameObject rewardMenu = null;

    public GameResources selectedReward = null;

    public void GenerateRewardPipeline()
    {
        firstReward  = SelectRewards();
        secondReward = SelectRewards();
        thirdReward  = SelectRewards();

        CreatePopUpGameObject();

        firstButton  = rewardMenu.GetChild("FirstRewardButton").GetComponent<EndLevelRewardsButtons>();
        secondButton = rewardMenu.GetChild("SecondRewardButton").GetComponent<EndLevelRewardsButtons>();
        thirdButton  = rewardMenu.GetChild("ThirdRewardButton").GetComponent<EndLevelRewardsButtons>();

        if (firstButton != null && firstButton.gameObject != null)
            firstButton.gameObject.GetComponent<Navigation>().Select(); // Line added because buttons crashed. For... some reason

        Core.instance.gameObject.GetComponent<PlayerHealth>().SetInvincible(true);
    }

    public GameResources GetSelectedReward()
    {
        if (CheckRewardSelected())
        {
            CleanAllElements();
            Core.instance.startAvailable = true;
        }

        return selectedReward;
    }

    public EndLevelReward SelectRewards()
    {
        // Do logic about dynamic percentage change (need the player stats to know)
        EndLevelReward newReward;
        int randomNum = new Random().Next(100);
        int chanceAcumulation = 0;

        for (int i = 0; i < 5; i++)
        {
            chanceAcumulation += rewardChances[i];

            if (randomNum <= chanceAcumulation)
            {
                switch (i)
                {
                    case 0:
                        newReward = new EndLevelReward(RequestRandomBoon(), RewardType.REWARD_BOON);
                        break;

                    case 1:
                        newReward = new EndLevelReward(-1, RewardType.REWARD_BESKAR);
                        break;

                    case 2:
                        newReward = new EndLevelReward(-1, RewardType.REWARD_MACARON);
                        break;

                    case 3:
                        newReward = new EndLevelReward(-1, RewardType.REWARD_SCRAP);
                        break;

                    case 4:
                        newReward = new EndLevelReward(-1, RewardType.REWARD_MILK);
                        break;

                    default:
                        newReward = new EndLevelReward(RequestRandomBoon(), RewardType.REWARD_BOON); // Spawn boon, just in case
                        break;
                }

                return newReward;

            }

        }

        newReward = new EndLevelReward(RequestRandomBoon(), RewardType.REWARD_BOON); // Spawn boon, just in case, but we should always quit from the previous return
        return newReward;
    }

    public int RequestRandomBoon()
    {
        float randomPick = (float)(new Random().NextDouble() * BoonDataHolder.boonTotalWeights) - BoonDataHolder.boonType[1].rngChanceWeight;

        for (int i = 0; i < BoonDataHolder.boonType.Length; i++)
        {
            if (randomPick < BoonDataHolder.boonType[i].rngChanceWeight)
            {
                return i;
            }
        }

        return BoonDataHolder.boonType.Length - 1;
    }

    void CreatePopUpGameObject()
    {
        GameObject canvas = InternalCalls.FindObjectWithName("Canvas");

        rewardMenu = InternalCalls.CreatePrefab("Library/Prefabs/18131542.prefab", new Vector3(0.0f, 0.0f, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f));

        if (rewardMenu == null)
            return;

        GameObject firstText = rewardMenu.GetChild("FirstRewardText");
        GameObject secondText = rewardMenu.GetChild("SecondRewardText");
        GameObject thirdText = rewardMenu.GetChild("ThirdRewardText");

        GameObject firstImage = rewardMenu.GetChild("FirstRewardImage");
        GameObject secondImage = rewardMenu.GetChild("SecondRewardImage");
        GameObject thirdImage = rewardMenu.GetChild("ThirdRewardImage");

        if (canvas == null)
            canvas = InternalCalls.CreatePrefab("Library/Prefabs/1965121116.prefab", new Vector3(0.0f, 0.0f, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f));

        rewardMenu.SetParent(canvas);

        firstImage.GetComponent<Image2D>().AssignLibrary2DTexture(GetRewardTexture(firstReward));
        secondImage.GetComponent<Image2D>().AssignLibrary2DTexture(GetRewardTexture(secondReward));
        thirdImage.GetComponent<Image2D>().AssignLibrary2DTexture(GetRewardTexture(thirdReward));

        firstText.GetComponent<Text>().text  = RewardText(firstReward);
        secondText.GetComponent<Text>().text = RewardText(secondReward);
        thirdText.GetComponent<Text>().text  = RewardText(thirdReward);

        ChangeMusic();

        Core.instance.startAvailable = false;
        return;
    }

    string RewardText(EndLevelReward reward)
    {
        string text = null;

        switch (reward.type)
        {
            case RewardType.REWARD_BOON:
                text = BoonDataHolder.boonType[reward.boonIndex].rewardDescription;
                break;

            case RewardType.REWARD_BESKAR:
                text = new BeskarResource().rewardDescription;
                break;

            case RewardType.REWARD_MACARON:
                text = new MacaronResource().rewardDescription;
                break;

            case RewardType.REWARD_SCRAP:
                text = new ScrapResource().rewardDescription;
                break;

            case RewardType.REWARD_MILK:
                text = new MilkResource().rewardDescription;
                break;

        }

        return text;
    }

    public int GetRewardTexture(EndLevelReward reward)
    {

        int textureId = 0;

        switch (reward.type)
        {
            case RewardType.REWARD_BOON:
                textureId = BoonDataHolder.boonType[reward.boonIndex].libraryTextureID;
                break;

            case RewardType.REWARD_BESKAR:
                textureId = new BeskarResource().libraryTextureID;
                break;

            case RewardType.REWARD_MACARON:
                textureId = new MacaronResource().libraryTextureID;
                break;

            case RewardType.REWARD_SCRAP:
                textureId = new ScrapResource().libraryTextureID;
                break;

            case RewardType.REWARD_MILK:
                textureId = new MilkResource().libraryTextureID;
                break;

        }

        return textureId;

    }

    public bool CheckRewardSelected()   // I know. Shut up
    {
        if (firstButton != null && firstButton.pressed)
        {
            selectedReward = ConvertRewardtoRewardResource(firstReward);
            return true;
        }
        else if (secondButton != null && secondButton.pressed)
        {
            selectedReward = ConvertRewardtoRewardResource(secondReward);
            return true;
        }
        else if (thirdButton != null && thirdButton.pressed)
        {
            selectedReward = ConvertRewardtoRewardResource(thirdReward);
            return true;
        }

        return false;
    }

    public GameResources ConvertRewardtoRewardResource(EndLevelReward reward)
    {
        GameResources selectedResource = null;

        switch (reward.type)
        {
            case RewardType.REWARD_BOON:
                selectedResource = BoonDataHolder.boonType[reward.boonIndex];
                break;

            case RewardType.REWARD_BESKAR:
                selectedResource = new BeskarResource();
                break;

            case RewardType.REWARD_MACARON:
                selectedResource = new MacaronResource();
                break;

            case RewardType.REWARD_SCRAP:
                selectedResource = new ScrapResource();
                break;

            case RewardType.REWARD_MILK:
                selectedResource = new MilkResource();
                break;

        }

        return selectedResource;
    }

    public void CleanAllElements()
    {
        firstButton = null;
        secondButton = null;
        thirdButton = null;
        InternalCalls.Destroy(InternalCalls.FindObjectWithName("EndLevelReward"));
    }

    private void ChangeMusic()
    {
        int pos = 0;
        pos = (((int)RoomSwitch.currentLevelIndicator) - 1 < 0) ? 0 : ((int)RoomSwitch.currentLevelIndicator) - 1; 
        if (RoomSwitch.currentroom != RoomSwitch.levelLists[pos].bossScene)
        {
            if (RoomSwitch.currentLevelIndicator == RoomSwitch.LEVELS.ONE)
            {
                Audio.SetState("Game_State", "Run");
                Audio.SetState("Player_State", "Alive");
                if (MusicSourceLocate.instance != null)
                    Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
            }
            else if (RoomSwitch.currentLevelIndicator == RoomSwitch.LEVELS.TWO)
            {
                Audio.SetState("Game_State", "Run_2");
                Audio.SetState("Player_State", "Alive");
                if (MusicSourceLocate.instance != null)
                    Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
            }
            else if (RoomSwitch.currentLevelIndicator == RoomSwitch.LEVELS.THREE)
            {
                Audio.SetState("Game_State", "Run_3");
                Audio.SetState("Player_State", "Alive");
                if (MusicSourceLocate.instance != null)
                    Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
            }
        }
        else
        {
            if (pos == (int)RoomSwitch.LEVELS.ONE)
            {
                Audio.SetState("Game_State", "Run");
                Audio.SetState("Player_State", "Alive");
                if (MusicSourceLocate.instance != null)
                    Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
            }
            else if (pos == (int)RoomSwitch.LEVELS.TWO)
            {
                Audio.SetState("Game_State", "Run_2");
                Audio.SetState("Player_State", "Alive");
                if (MusicSourceLocate.instance != null)
                    Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
            }
            else if (pos == (int)RoomSwitch.LEVELS.THREE)
            {
                Audio.SetState("Game_State", "Run_3");
                Audio.SetState("Player_State", "Alive");
                if (MusicSourceLocate.instance != null)
                    Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
            }
        }
    }
}