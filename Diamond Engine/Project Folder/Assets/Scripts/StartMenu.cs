using System;
using DiamondEngine;

public class StartMenu : DiamondComponent
{
    public GameObject popUpNewGame = null;
    public GameObject menuButtons = null;
    public GameObject options = null;
    public GameObject background = null;
    public GameObject default_selected = null;
    public GameObject optionsButton = null;
    public GameObject newGameButton = null;

    private bool musicplayed = false;

    public void Awake()
    {
        if (gameObject.Name != "New Game" && gameObject.Name != "Options")
            return;
        DiamondPrefs.LoadData();
        if (DiamondPrefs.ReadBool("loadData") == true)
            return;
        if(gameObject.Name == "New Game" && optionsButton!=null)
        {
            gameObject.GetComponent<Navigation>().SetDownNavButton(optionsButton);
            gameObject.GetComponent<Navigation>().Select();
        }
        else if(gameObject.Name == "Options" && newGameButton != null)
        {
            gameObject.GetComponent<Navigation>().SetUpNavButton(newGameButton);
        }
        ConfigFunctionality.UpdateDisplayText();
    }
    public void OnExecuteButton()
    {
        
        if (gameObject.Name == "New Game" || gameObject.Name == "Load Game")
        {
            if (gameObject.Name == "New Game")
            {
                DiamondPrefs.LoadData();
                if (DiamondPrefs.ReadBool("loadData") == true && popUpNewGame != null && menuButtons != null)
                {
                    popUpNewGame.EnableNav(true);
                    menuButtons.EnableNav(false);
                    if (default_selected != null)
                        default_selected.GetComponent<Navigation>().Select();
                    DiamondPrefs.Clear();
                    return;
                }
                DiamondPrefs.Clear();
            }
            else
            {
                DiamondPrefs.LoadData();
                if (DiamondPrefs.ReadBool("loadData") == false)
                    return;
                Counter.firstRun = false;
            }

            StartFadeIn();
        }
        else if (gameObject.Name == "Options")
        {
            menuButtons.EnableNav(false);
            options.EnableNav(true);
            background.EnableNav(true);
            if (default_selected != null)
                default_selected.GetComponent<Navigation>().Select();
        }
        else if (gameObject.Name == "Quit")
        {
            options.EnableNav(true);
            menuButtons.EnableNav(false);
            if (default_selected != null)
                default_selected.GetComponent<Navigation>().Select();
        }
    }
    public void Update()
    {
        if (!musicplayed)
        {
            Audio.SetState("Player_State", "Alive");
            Audio.SetState("Game_State", "Menu");
            musicplayed = true;
        }
    }

    public static void StartFadeIn()
    {
        if (InternalCalls.FindObjectWithName("BlackFade") != null)
        {
            BlackFade.StartFadeIn(StartGame);
        }
        else
        {
            StartGame();

        }
    }
    public static void StartGame()
    {
        DiamondPrefs.Write("reset", true);
        if (Core.instance != null)
            Core.instance.SaveBuffs();
        SceneManager.LoadScene(1564453141);


        if (MusicSourceLocate.instance != null)
        {
            Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
        }
    }

}