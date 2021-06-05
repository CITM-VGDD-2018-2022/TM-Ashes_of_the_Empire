using System;
using DiamondEngine;

public class GameSceneManager : DiamondComponent
{
    GameObject rewardObject = null;
    EndLevelRewardSpawn rewardSpawnComponent = null;
    EndLevelRewards rewardMenu = null;
    GameResources rewardData = null;
    Vector3 rewardInitialPos = new Vector3(0.0f, 0.0f, 0.0f);

    public static GameSceneManager instance = null;

    // THIS IS A VERY INCOMPLETE MODULE THAT IS CURRENTLY NOT MANAGING SCENE. HOWEVER, WE HAVE TO DO THAT IN A CLOSED PLACED. AND SOMEONE HAS TO START IT. YEET TIME :3
    // THIS SCRIPT SHOULD NOT BE DELETED AND RELOADED CONSTANTLY, CODE IS THOUGHT TO BE IN AN OBJECT THAT TAKES CARE OF ALL SCENE MANAGING DURING ALL GAME DURATION

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            InternalCalls.Destroy(this.gameObject);
            return;
        }

        if (instance == null)
            instance = this;

        Debug.Log("I'm cunt number: " + gameObject.pointer.ToString());

        rewardObject = InternalCalls.CreatePrefab("Library/Prefabs/1394471616.prefab", new Vector3(rewardInitialPos.x, rewardInitialPos.y, rewardInitialPos.z), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f));

        if (rewardObject != null)
        {
            rewardObject.SetParent(gameObject);
            rewardObject.Enable(false);
            rewardSpawnComponent = rewardObject.GetComponent<EndLevelRewardSpawn>();
        }
    }

    public void Update()
    {
        if (rewardMenu != null)
        {
            rewardData = rewardMenu.GetSelectedReward();

            if (rewardData != null)
            {
                EnemyManager.ClearList();

                rewardInitialPos = Core.instance.gameObject.transform.globalPosition + new Vector3(2.5f, 1.0f, 2.5f);
                rewardObject.transform.localPosition = rewardInitialPos;

                rewardObject.AssignLibraryTextureToMaterial(rewardData.meshTextureID, "diffuseTexture");
                rewardObject.Enable(true);
                if (rewardSpawnComponent != null)
                {
                    rewardSpawnComponent.PlayParticles();
                }
                Core.instance.lockInputs = false;
                Audio.PlayAudio(rewardObject, "Play_UI_Boon_Pickup");

                if (rewardMenu != null)
                {
                    InternalCalls.Destroy(rewardMenu.rewardMenu);
                    rewardMenu = null;
                }
            }
        }

        if (rewardData != null && rewardObject != null && rewardSpawnComponent != null)
        {

            rewardSpawnComponent.AdvanceVerticalMovement(rewardInitialPos);
            rewardSpawnComponent.AdvanceRotation();

            if (rewardSpawnComponent.trigger == true)
            {

                ApplyReward();
                if (InternalCalls.FindObjectWithName("BlackFade") != null)
                {
                    BlackFade.StartFadeIn(ChangeScene);
                }
                else
                {
                    ChangeScene();
                }
                rewardSpawnComponent.trigger = false;
            }
        }

        if (DebugOptionsHolder.goToNextRoom == true)
        {
            Debug.Log("Change scene");
            ChangeScene();
            DebugOptionsHolder.goToNextRoom = false;
        }

        if (DebugOptionsHolder.goToNextLevel == true)
        {
            Debug.Log("Change scene");
            ChangeScene();
            //DebugOptionsHolder.goToNextLevel = false;
        }

        // We should clean boons when ending a run :3
        //PlayerResources.ResetRunBoons();

    }


    private void ChangeScene()
    {
        Debug.Log("LEAVING SCENE");
        if (Core.instance != null)
            Core.instance.SaveBuffs();

        if (!Counter.isFinalScene)
            RoomSwitch.SwitchRooms();
        else
        {
            RoomSwitch.OnPlayerWin();
        }

        Debug.Log("SAVING SCENE");

    }

    public void LevelEnd()
    {
        if (PlayerResources.CheckBoon(BOONS.BOON_BOUNTY_HUNTER_SKILLS))
        {
            PlayerResources.AddRunCoins(2);
        }
        if (Core.instance != null)
            Audio.PlayAudio(Core.instance.gameObject, "Play_Mando_Clean_Room_Voice");
        Counter.SumToCounterType(Counter.CounterTypes.LEVELS);
        rewardMenu = new EndLevelRewards();
        Core.instance.BlockInIdle();
        rewardMenu.GenerateRewardPipeline();
    }

    public void ApplyReward()
    {
        if (rewardData != null)
        {
            rewardData.Use();
            if (Core.instance != null)
            {
                if (Core.instance.HasStatus(STATUS_TYPE.BOUNTY_HUNTER))
                    PlayerResources.AddRunCoins((int)(Core.instance.GetStatusData(STATUS_TYPE.BOUNTY_HUNTER).severity));
            }

            if (rewardObject != null)
                rewardObject.Enable(false);

            if (rewardSpawnComponent != null)
                rewardSpawnComponent.trigger = false;

            rewardData = null;
        }
    }

    public void OnApplicationQuit()
    {
        instance = null;
    }

    public void DeactivateBoon()
    {
        if (rewardObject.IsEnabled())
            rewardObject.Enable(false);
        if (rewardMenu != null)
        {
            InternalCalls.Destroy(rewardMenu);
            rewardMenu = null;
        }
        rewardData = null;


    }
}