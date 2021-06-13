using System;
using DiamondEngine;

public class TutoTriggerSwitch : DiamondComponent
{
    public int roomUidToLoad = -1;
    public bool start = true;

    private void Start()
    {
        Audio.SetState("Game_State", "Run");
        Audio.SetState("Player_State", "Alive");
        if (MusicSourceLocate.instance != null)
        {
            Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Health", "Healthy");
            Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
        }
    }
    public void Update()
    {
        if (start)
        {
            Start();
            start = false;
        }
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (roomUidToLoad != -1 && triggeredGameObject.CompareTag("Player"))
        {
            if (Counter.firstTutorial)
            {
                PlayerResources.AddResourceBy1(RewardType.REWARD_BESKAR);
                PlayerResources.AddResourceBy1(RewardType.REWARD_MACARON);
                PlayerResources.AddResourceBy1(RewardType.REWARD_SCRAP);
                PlayerResources.AddResourceBy1(RewardType.REWARD_MILK);
            }
            if (EnvironmentSourceLocate.instance != null)
                Audio.PlayAudio(EnvironmentSourceLocate.instance.gameObject, "Play_UI_Boon_Pickup");
            StaticVariablesInit.InitStaticVars();
            Core.instance.LockInputs(true);
            Counter.firstTutorial = false;
            DebugOptionsHolder.godModeActive = false;
            BlackFade.StartFadeIn( () => 
            {
                Core.instance.LockInputs(false);
                SceneManager.LoadScene(roomUidToLoad); 
            });
            
        }

    }

}