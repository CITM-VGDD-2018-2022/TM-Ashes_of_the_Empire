using System;
using DiamondEngine;
using System.Collections.Generic;

public class EnemyManager : DiamondComponent
{
    public static List<GameObject> currentEnemies = null;
    public static List<GameObject> currentDestructibleProps = null; //props will not count as enemies (barrels)

    public static int awaitingForEnemiesToSpawn = 0;

    public static void AddEnemy(GameObject enemy)
    {
        if (currentEnemies == null)
            currentEnemies = new List<GameObject>();

        currentEnemies.Add(enemy);
    }

    public static void AddProp(GameObject prop)
    {
        if (currentDestructibleProps == null)
            currentDestructibleProps = new List<GameObject>();

        currentDestructibleProps.Add(prop);
    }

    public static bool RemoveEnemy(GameObject enemy)
    {
        bool ret = false;

        if (enemy == null)
            return false;

        if (currentEnemies == null)
        {
            return false;
        }

        for (int i = 0; i < currentEnemies.Count; ++i)
        {
            if (currentEnemies[i].GetUid() == enemy.GetUid())
            {
                if(Core.instance != null)
                {
                    Core.instance.NotifyEnemyDeath(currentEnemies[i]);
                }

                currentEnemies.RemoveAt(i);
                ret = true;


            }
        }

        if (ret == false)
        {
            return ret;
        }

        bool enemiesLeftToSpawn = false;

        if (SpawnManager.instance != null)
        {
            enemiesLeftToSpawn = SpawnManager.instance.AreEnemiesLeftToSpawn();
        }

        if (currentEnemies.Count == 0 && awaitingForEnemiesToSpawn == 0 && enemiesLeftToSpawn == false && GameSceneManager.instance != null)
        {
            GameSceneManager.instance.StartLevelEnd();
        }

        return ret;
    }

    public static bool RemoveProp(GameObject prop)
    {
        bool ret = false;

        if (prop == null)
            return false;

        if (currentDestructibleProps == null)
        {
            return false;
        }

        for (int i = 0; i < currentDestructibleProps.Count; ++i)
        {
            if (currentDestructibleProps[i].GetUid() == prop.GetUid())
            {
                currentDestructibleProps.RemoveAt(i);
                ret = true;
            }
        }

        if (ret == false)
        {
            return ret;
        }
        return ret;
    }

    public static void ClearList()
    {
        if (currentEnemies != null)
        {
            currentEnemies.Clear();
            currentEnemies = null;
        }
        awaitingForEnemiesToSpawn = 0;

        if(currentDestructibleProps!=null)
        {
            currentDestructibleProps.Clear();
            currentDestructibleProps = null;
        }
    }

    public static int EnemiesLeft()
    {
        if (currentEnemies != null)
            return currentEnemies.Count;
        else
            return 0;
    }
}