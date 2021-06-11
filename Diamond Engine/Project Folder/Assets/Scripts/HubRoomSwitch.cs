using System;
using System.Collections.Generic;
using DiamondEngine;

public class HubRoomSwitch : DiamondComponent
{
	public int nextRoomUID = -1;
	public bool isHubScene = false;



	public void OnExecuteButton()
    {
        GameObject blackFade = InternalCalls.FindObjectWithName("BlackFade");
        if (blackFade != null)
        {
            BlackFade.StartFadeIn(ChangeScene);
        }
        else
        {
            ChangeScene();
        }
    }

    private void ChangeScene()
    {
        if (isHubScene == true)
        {
            PlayerResources.ResetRunBoons();
            PlayerResources.SetRunCoins(0);
            RoomSwitch.ClearStaticData();
        }

        //PlayerResources.ResetRunBoons(); //TODO: This has a bug when starting the run at the HUB
        if (Core.instance != null)
            Core.instance.SaveBuffs();

        if (nextRoomUID != -1)
            SceneManager.LoadScene(nextRoomUID);
        else
            RoomSwitch.SwitchRooms();


        if (EnvironmentSourceLocate.instance != null)
            Audio.StopAudio(EnvironmentSourceLocate.instance.gameObject);

        if (nextRoomUID == 1341124272)
        {
            Audio.PlayAudio(EnvironmentSourceLocate.instance.gameObject, "Play_Level_1_Ambience");
            Audio.SetState("Game_State", "Run");
            Audio.SetState("Player_State", "Alive");
            Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Combat");
        }
        //RoomSwitch.PlayLevelEnvironment();
    }
}