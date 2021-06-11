using System;
using DiamondEngine;
using System.Collections.Generic;

public class SpawnManager : DiamondComponent
{
    public static SpawnManager instance = null;
    public List<GameObject> availableSpawnPoints = null;

    public bool spawningWaves = true;

    private int enemiesToSpawn = 0;
    private int enemiesLeftInWave = 0;
    public int maxEnemiesPerWave = 0;
    public int enemyIncreasePerWave = 0;
    private int wave = 0;
    public int maxWaves = 1;

    public float delayInWaveSpawn = 0.0f;

    public int enemiesLeftToNextWave = 2;

    private float waveTimer = 0.0f;
    public float timeBetweenWaves = 1.0f;

    public float maxDelayTime = 1.0f;
    private int maxDelayTimeSec = 0;
    private float maxDelayTimePoint = 0f;

    private bool endWave = false;

    public void Awake()
    {
        instance = this;
        availableSpawnPoints = new List<GameObject>();

        enemiesToSpawn = maxEnemiesPerWave;

        EnemyManager.ClearList();

        maxDelayTimeSec = (int)maxDelayTime;
        maxDelayTimePoint = maxDelayTime - (float)maxDelayTimeSec;
        endWave = false;

        waveTimer = delayInWaveSpawn;
    }

    public void Update()
    {
        if (spawningWaves == false)
            return;

        waveTimer -= Time.deltaTime;

        if (((waveTimer <= 0.0f && (EnemyManager.EnemiesLeft() <= enemiesLeftToNextWave || enemiesLeftInWave > 0)) 
            || EnemyManager.EnemiesLeft() == 0) && EnemyManager.awaitingForEnemiesToSpawn == 0)
        {
            SpawnWave();
            waveTimer = 0.0f;

            if (enemiesToSpawn > 0)
            {
                waveTimer = delayInWaveSpawn;
            }
            else
            {
                ++wave;

                if (wave < maxWaves)
                {
                    waveTimer = timeBetweenWaves;
                    maxEnemiesPerWave += enemyIncreasePerWave;
                    enemiesToSpawn = maxEnemiesPerWave;
                }
                else
                {
                    endWave = true;
                }
            }
        }

    }

    public void AddSpawnPoint(GameObject spawnPoint)
    {
        if (availableSpawnPoints == null)
            availableSpawnPoints = new List<GameObject>();

        availableSpawnPoints.Add(spawnPoint);
    }

    public void RemoveSpawnPoint(GameObject spawnPoint)
    {
        for (int i = 0; i < availableSpawnPoints.Count; ++i)
        {
            if (availableSpawnPoints[i].GetUid() == spawnPoint.GetUid())
            {
                availableSpawnPoints.RemoveAt(i);
                return;
            }
        }
    }

    public void ClearSpawnPointsList()
    {
        availableSpawnPoints.Clear();
    }

    private void SpawnWave()
    {
        List<GameObject> spawnPoints = new List<GameObject>();

        for (int i = 0; i < availableSpawnPoints.Count; ++i)
        {
            spawnPoints.Add(availableSpawnPoints[i]);
        }

        Random randomizer = new Random();

        int pendingEnemies = 0;
        if (enemiesToSpawn > availableSpawnPoints.Count)
        {
            //we should not maximize the amount of enemies to the maximum spawn points, it will be solved
            pendingEnemies = enemiesToSpawn - availableSpawnPoints.Count;
            enemiesToSpawn = availableSpawnPoints.Count;
        }

        for (int i = 0; i < enemiesToSpawn; ++i)
        {
            int index = randomizer.Next(0, spawnPoints.Count);

            GameObject spawnPoint = spawnPoints[index];
            SpawnPoint spawnScript = spawnPoint.GetComponent<SpawnPoint>();

            if (spawnScript != null)
            {
                float delay = (float)randomizer.NextDouble();
                int retryNum = 0;

                while (delay > maxDelayTimePoint)
                {
                    delay = (float)randomizer.NextDouble();
                    ++retryNum;

                    if (retryNum > 5)
                        delay = maxDelayTimePoint;
                }

                delay = (float)(randomizer.Next(0, maxDelayTimeSec) + delay);

                spawnScript.QueueSpawnEnemy(delay);

            }

            for (int j = 0; j < spawnPoints.Count; ++j)
            {
                if (spawnPoint.GetUid() == spawnPoints[j].GetUid())
                {
                    spawnPoints.RemoveAt(j);
                    break;
                }
            }
        }

        enemiesToSpawn = enemiesLeftInWave = pendingEnemies;
    }

    public void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public int GetCurrentWave()
    {
        return wave;
    }

    public bool IsOnEndWave()
    {
        return endWave;
    }

    public bool AreEnemiesLeftToSpawn()
    {
        return enemiesLeftInWave > 0 || !endWave;
    }
}