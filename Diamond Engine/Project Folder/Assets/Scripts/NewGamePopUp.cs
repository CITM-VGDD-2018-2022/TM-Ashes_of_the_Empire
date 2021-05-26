using System;
using DiamondEngine;

public class NewGamePopUp : DiamondComponent
{
	public GameObject popUpNewGame = null;
    public GameObject mainMenu = null;
	public bool confirmNewGame = false;
    public void OnExecuteButton()
	{
        if (confirmNewGame)
        {
            StartMenu.StartFadeIn();
        }
        else
        {
            popUpNewGame.EnableNav(false);
            mainMenu.EnableNav(true);
        }
    }

}