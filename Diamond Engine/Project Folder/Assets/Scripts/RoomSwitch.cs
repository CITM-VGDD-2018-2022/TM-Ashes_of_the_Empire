using System;
using System.Collections.Generic;
using DiamondEngine;

public class RoomDataHolder
{
    public List<int> visited = new List<int>(); //I think this is a fucking pointer-type shit? i don't know, i don't think this is working, im out
    public int bossScene = 0;
    public int preBossScene = 0;

    public int shopRoom = 0;

    public RoomDataHolder(List<int> initList, int final, int preBoss = 0, int shop = 0)
    {
        //visited.Clear();
        visited = new List<int>(initList);
        bossScene = final;

        preBossScene = preBoss;
        shopRoom = shop;
    }

    public void ClearData(List<int> initList)
    {
        //visited.Clear();
        visited = new List<int>(initList);
    }

    //public bool isFinalScene = false;
}

public static class RoomSwitch
{
    public enum LEVELS
    {
        ONE,
        TWO,
        THREE,
        MAX,
    }

    public readonly static List<List<int>> originalLevelPools = new List<List<int>>()
    {
        new List<int>()
        {
            1940308114,
            769097826,
            1796967585,
        },

        new List<int>()
        {
            614298780,
            2043151660,
            1984495527,
            1992514962,
        },

        new List<int>()
        {
            573625925,  //Room 2
            1529493583, //Should remove one of these
        },

    };

    public readonly static List<RoomDataHolder> levelLists = new List<RoomDataHolder>()
    {
        new RoomDataHolder(originalLevelPools[0], 308281119, 0, 605149455),
        new RoomDataHolder(originalLevelPools[1], 880545183, 0, 1812779980),
        new RoomDataHolder(originalLevelPools[2], 2069575406, 518439386),
    };

    private readonly static int defaultErrorShop = 605149455;

    private static int roomID = 0;
    public static int currentroom = 0;

    //public static int index = 0;
    static Random randomGenerator = new Random();

    public static LEVELS currentLevelIndicator = LEVELS.ONE;

    public static LEVELS GetLevelIndicator()
    {
        return currentLevelIndicator;
    }

    public static LEVELS GetNextLevel()
    {
        switch (currentLevelIndicator)
        {
            case LEVELS.ONE:
                return LEVELS.TWO;
            case LEVELS.TWO:
                return LEVELS.THREE;
            case LEVELS.THREE:
                return LEVELS.MAX;
            default:
                return LEVELS.MAX;
        }
    }

    public static void SwitchRooms()
    {

        EnemyManager.ClearList();
        int index = (int)currentLevelIndicator;
        if (Core.instance != null)
            Core.instance.SaveBuffs();

        //Load Post Boss Room
        if (index > 0 && currentLevelIndicator <= LEVELS.MAX && levelLists[index - 1].bossScene == currentroom  /*&& currentLevelIndicator == LEVELS.TWO*/)
        {
            int shopToLoad = (levelLists[index - 1].shopRoom != 0) ? levelLists[index - 1].shopRoom : defaultErrorShop;

            SceneManager.LoadScene(shopToLoad);
            currentroom = shopToLoad;
            return;
        }

        //Switch Rooms
        if (levelLists[index].visited.Count > 0)
        {
            int roomnumber = randomGenerator.Next(0, levelLists[index].visited.Count);
            roomID = levelLists[index].visited[roomnumber];
            currentroom = roomID;
            levelLists[index].visited.Remove(currentroom);
            SceneManager.LoadScene(roomID);

            PlayLevelEnvironment();
        }
        //Switch Level
        else
        {

            if(levelLists[index].preBossScene != 0 && currentroom != levelLists[index].preBossScene)
            {
                currentroom = levelLists[index].preBossScene;
                SceneManager.LoadScene(levelLists[index].preBossScene);
                PlayLevelEnvironment();
            }
            else
            {
                levelLists[index].visited = new List<int>(originalLevelPools[index]);

                currentLevelIndicator++;
                //IMPORTANT: isFinalScene is needed to know when to change from a room to the end scene to show statistics

                if (currentLevelIndicator == LEVELS.MAX)
                    Counter.isFinalScene = true;

                currentroom = levelLists[index].bossScene;
                SceneManager.LoadScene(levelLists[index].bossScene);
                PlayMusicBoss((LEVELS)index);
            }

            
        }

    }

    public static void ClearStaticData()
    {
        currentLevelIndicator = LEVELS.ONE;
        currentroom = 0;

        for (int i = 0; i < (int)LEVELS.MAX; i++)
        {
            levelLists[i].ClearData(originalLevelPools[i]);
        }

        Counter.gameResult = Counter.GameResult.NONE;   // This is morally wrong. But we don't have a scene manager
    }

    public static void PlayLevelEnvironment()
    {
        Audio.StopAudio(EnvironmentSourceLocate.instance.gameObject);
        switch (currentLevelIndicator)
        {
            case LEVELS.ONE:
                Audio.PlayAudio(EnvironmentSourceLocate.instance.gameObject, "Play_Level_1_Ambience");
                Audio.SetState("Game_State", "Run");
                Audio.SetState("Player_State", "Alive");
                Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Combat");
                break;
            case LEVELS.TWO:
                Audio.PlayAudio(EnvironmentSourceLocate.instance.gameObject, "Play_Level_2_Ambience");
                Audio.SetState("Game_State", "Run_2");
                Audio.SetState("Player_State", "Alive");
                Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Combat");
                break;
            case LEVELS.THREE:
                Audio.PlayAudio(EnvironmentSourceLocate.instance.gameObject, "Play_Spaceship_Interior_Ambience");
                Audio.SetState("Player_State", "Alive");
                Audio.SetState("Game_State", "Run_3");
                Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Combat");
                break;
        }
    }
    public static void PlayMusicBoss(LEVELS index)
    {
        switch (index)
        {
            case LEVELS.ONE:
                Audio.SetState("Player_State", "Alive");
                Audio.SetState("Game_State", "Rancor_Room");
                break;
            case LEVELS.TWO:
                Audio.SetState("Player_State", "Alive");
                Audio.SetState("Game_State", "Wampa_Skel_Room");
                break;
            case LEVELS.THREE:
                Audio.SetState("Player_State", "Alive");
                Audio.SetState("Game_State", "Moff_Guideon_Room");
                break;
        }
    }
    public static void OnPlayerDeath()
    {
        Counter.gameResult = Counter.GameResult.DEFEAT;
        Audio.StopAudio(EnvironmentSourceLocate.instance.gameObject);

        ToWinLoseScene();
    }

    public static void OnPlayerWin()
    {
        Counter.gameResult = Counter.GameResult.VICTORY;
        Audio.StopAudio(EnvironmentSourceLocate.instance.gameObject);
        DebugOptionsHolder.goToNextLevel = false;

        ToWinLoseScene();
    }

    public static void OnPlayerQuit()
    {
        Counter.gameResult = Counter.GameResult.NONE;

        //TODo: Maybe this should go to HUB?
        //if (Core.instance != null)
        //    Core.instance.SaveBuffs();

        //EnemyManager.ClearList();

        //ToHub();
        ToWinLoseScene();
    }

    private static void ToWinLoseScene()
    {
        if (Core.instance != null)
        {
            Core.instance.SaveBuffs();
            Core.instance.RemoveBoons();
        }


        EnemyManager.ClearList();

        SceneManager.LoadScene(821370213);
    }

    public static bool IsFinalBoss()
    {
        if (true)
        {
            return true;
        }

        return false;
    }

}