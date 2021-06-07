using System;
using DiamondEngine;

public class QuitConfirmation : DiamondComponent
{
	public GameObject confirmScreen = null;
	public GameObject bigBrother = null;
	//private Pause aux = null;
	public void OnExecuteButton()
	{
		if (gameObject.Name == "Cancel")
		{
			bigBrother.EnableNav(true);
			confirmScreen.EnableNav(false);
		}
		if (gameObject.Name == "QuittoDesktop")
			InternalCalls.CloseGame();
		if (gameObject.Name == "QuittoMenu")
		{
			if(GameSceneManager.instance != null)
            {
				GameSceneManager.instance.DeactivateBoon();
            }
			Time.ResumeGame();
			if (EnvironmentSourceLocate.instance != null)
				Audio.StopAudio(EnvironmentSourceLocate.instance.gameObject);
			Audio.SetState("Game_State", "HUB");
			RoomSwitch.OnPlayerQuit();
		}
		if (gameObject.Name == "QuittoMenuHUB")
		{
			Time.ResumeGame();
			if (Core.instance != null)
				Core.instance.SaveBuffs();
			if (EnvironmentSourceLocate.instance != null)
				Audio.StopAudio(EnvironmentSourceLocate.instance.gameObject);
			SceneManager.LoadScene(1726826608);
		}

	}
	public void Update()
	{

	}

}
