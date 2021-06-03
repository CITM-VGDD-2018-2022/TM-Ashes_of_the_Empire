using System;
using DiamondEngine;

public class TutoDestroyEnemies : DiamondComponent
{

	private bool cleared = false;
	public GameObject spawnPoint;
	public GameObject SpawnManager;
	public bool isLastZone = false;
	public void Update()
	{
		
	}

	public void OnTriggerEnter(GameObject triggeredGameObject)
	{
		if (triggeredGameObject.CompareTag("Player") && !cleared)
		{
            if (isLastZone)
            {
				SpawnManager.EnableNav(true);
				EnemyManager.ClearList();
				cleared = true;
				Core.instance.SetMySpawnPosition(spawnPoint.transform.globalPosition);
				DebugOptionsHolder.godModeActive = true;
			}
            else
            {
				Core.instance.SetMySpawnPosition(spawnPoint.transform.globalPosition);
			}
		}
	}
}