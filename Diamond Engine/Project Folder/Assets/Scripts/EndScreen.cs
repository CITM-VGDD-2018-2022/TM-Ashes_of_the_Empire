using System;
using DiamondEngine;

public class EndScreen : DiamondComponent
{
    public GameObject result = null;
    public GameObject banthaPanel = null;
    public GameObject banthaKills = null;
    public GameObject stormsPanel = null;
    public GameObject stormsKills = null;
    public GameObject skytrooperPanel = null;
    public GameObject skytrooperKills = null;
    public GameObject laserturretPanel = null;
    public GameObject laserturretKills = null;
    public GameObject heavytrooperPanel = null;
    public GameObject heavytrooperKills = null;
    public GameObject deathtrooperPanel = null;
    public GameObject deathtrooperKills = null;
    public GameObject rancorPanel = null;
    public GameObject wampaPanel = null;
    public GameObject skelPanel = null;
    public GameObject wampaandskelPanel = null;
    public GameObject moffPanel = null;
    public GameObject comboTxt = null;
    public GameObject currencyTxt = null;
    public GameObject levelsTxt = null;
    public GameObject boonDisplayObject = null;

    private bool firstFrame = true;

    public void Awake()
    {
        Counter.firstRun = false;

        if (boonDisplayObject == null)
        {
            boonDisplayObject = InternalCalls.FindObjectWithName("Boons Prefab");

            if (boonDisplayObject == null)
                Debug.Log("Null Boon object");
        }
        DiamondPrefs.SaveData();
    }

    public void OnExecuteButton()
    {
        if (gameObject.Name == "Continue")
        {
            StaticVariablesInit.InitStaticVars();
            Time.ResumeGame();
            Audio.SetState("Game_State", "Run");
            if (MusicSourceLocate.instance != null)
            {
                Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
            }
            if (Core.instance != null)
                Core.instance.SaveBuffs();
            SceneManager.LoadScene(518261031);

        }
        else if (gameObject.Name == "Quit")
        {
            Time.ResumeGame();
            if (Core.instance != null)
                Core.instance.SaveBuffs();
            SceneManager.LoadScene(1726826608);
        }
    }
    public void Update()
    {
        if (firstFrame && gameObject.Name == "End Scene")
        {
            firstFrame = false;
            Counter.isFinalScene = false;
            DisplayResults();
        }
    }

    void DisplayResults()
    {
        Debug.Log("Intro display results");
        if (Counter.gameResult == Counter.GameResult.VICTORY)
            result.GetComponent<Text>().text = "            VICTORY!";
        else if (Counter.gameResult == Counter.GameResult.DEFEAT)
            result.GetComponent<Text>().text = "            DEFEAT!";
        else
            result.GetComponent<Text>().text = "            RETREAT!";

        if (banthaKills != null)
        {
            //Update Bantha PANEL
            if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_BANTHA))
            {
                banthaKills.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.ENEMY_BANTHA].amount.ToString();
            }
            else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_BANTHA))
            {
                banthaPanel.Enable(false);
            }
        }

        if (stormsKills != null)
        {
            //Update Stormtrooper PANEL
            if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_STORMTROOPER))
            {
                stormsKills.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.ENEMY_STORMTROOPER].amount.ToString();
            }
            else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_STORMTROOPER))
            {
                stormsPanel.Enable(false);
            }
        }

        if (skytrooperKills != null)
        {
            //Update Skytrooper PANEL
            if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_SKYTROOPER))
            {
                skytrooperKills.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.ENEMY_SKYTROOPER].amount.ToString();
            }
            else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_SKYTROOPER))
            {
                skytrooperPanel.Enable(false);
            }

        }

        if (laserturretKills != null)
        {
            //Update Laser Turret PANEL
            if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_LASER_TURRET))
            {
                laserturretKills.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.ENEMY_LASER_TURRET].amount.ToString();
            }
            else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_LASER_TURRET))
            {
                laserturretPanel.Enable(false);
            }
        }

        if (heavytrooperKills != null)
        {
            //Update Heavytrooper PANEL
            if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_HEAVYTROOPER))
            {
                heavytrooperKills.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.ENEMY_HEAVYTROOPER].amount.ToString();
            }
            else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_HEAVYTROOPER))
            {
                heavytrooperPanel.Enable(false);
            }
        }

        if (deathtrooperKills != null)
        {
            //Update Deathtrooper PANEL
            if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_DEATHTROOPER))
            {
                deathtrooperKills.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.ENEMY_DEATHTROOPER].amount.ToString();
            }
            else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_DEATHTROOPER))
            {
                deathtrooperPanel.Enable(false);
            }
        }

        if (rancorPanel != null)
        {
            //Update Rancor PANEL
            if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.RANCOR))
            {
                rancorPanel.Enable(true);
            }
            else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.RANCOR))
            {
                rancorPanel.Enable(false);
            }
        }

        if (wampaandskelPanel != null && skelPanel != null && wampaPanel != null)
        {
            //Update Wampa and Skel PANEL
            if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.WAMPA) && Counter.GameCounters.ContainsKey(Counter.CounterTypes.SKEL))
            {
                Debug.Log("Wampa&Skel");
                wampaandskelPanel.Enable(true);
                skelPanel.Enable(false);
                wampaPanel.Enable(false);
            }
            else if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.WAMPA) && !Counter.GameCounters.ContainsKey(Counter.CounterTypes.SKEL))
            {
                Debug.Log("Wampa");
                wampaPanel.Enable(true);
                skelPanel.Enable(false);
                wampaandskelPanel.Enable(false);
            }
            else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.WAMPA) && Counter.GameCounters.ContainsKey(Counter.CounterTypes.SKEL))
            {
                Debug.Log("Skel");
                skelPanel.Enable(true);
                wampaPanel.Enable(false);
                wampaandskelPanel.Enable(false);
            }
            else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.WAMPA) && !Counter.GameCounters.ContainsKey(Counter.CounterTypes.SKEL))
            {
                Debug.Log("NONE");
                skelPanel.Enable(false);
                wampaPanel.Enable(false);
                wampaandskelPanel.Enable(false);
            }
        }

        if (moffPanel != null)
        {
            //Update Moff Gideon PANEL
            if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.MOFFGIDEON))
            {
                moffPanel.Enable(true);
            }
            else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.MOFFGIDEON))
            {
                moffPanel.Enable(false);
            }
        }

        //Update Combo PANEL
        if (comboTxt != null)
        {
            comboTxt.GetComponent<Text>().text = Counter.maxCombo.ToString();
        }

        //Update Currency PANEL
        if (currencyTxt != null)
        {
            if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.RUN_COINS))
            {
                currencyTxt.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.RUN_COINS].amount.ToString();
            }
            else
            {
                currencyTxt.GetComponent<Text>().text = "0";
            }
        }

        //Update Levels PANEL
        if (levelsTxt != null)
        {
            if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.LEVELS))
            {
                levelsTxt.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.LEVELS].amount.ToString();
            }
            else
            {
                levelsTxt.GetComponent<Text>().text = "0";
            }
        }

        if (boonDisplayObject != null)
        {
            BoonDisplay boonDisplay = boonDisplayObject.GetComponent<BoonDisplay>();

            if (boonDisplay != null)
            {
                int boonDisplayIndex = 0;

                for (int i = 0; i < (int)BOONS.BOON_MAX; i++)
                {
                    if (PlayerResources.CheckBoon((BOONS)i))
                    {
                        //Boon image
                        int boonTextureId = BoonDataHolder.boonType[i].libraryTextureID;

                        //Boon count for text
                        int boonCount = PlayerResources.GetBoonAmount((BOONS)i);

                        boonDisplay.SetBoon(boonDisplayIndex, boonTextureId, boonCount);

                        boonDisplayIndex++;
                    }

                }
            }
            else
            {
                Debug.Log("Null boon display");
            }
        }
    }
}