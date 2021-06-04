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


        if (MusicSourceLocate.instance != null)
        {
            Audio.SetState("Player_State", "Alive");
            Audio.SetState("Game_State", "NONE");
            Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
            Debug.Log("Exploring");
        }

        //RoomSwitch.PlayLevelEnvironment();
    }
}