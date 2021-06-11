using System;
using System.Collections.Generic;
using DiamondEngine;


public class GroguFPManager : DiamondComponent
{
	public GameObject point1 = null;
	public GameObject point2 = null;
	public GameObject point3 = null;
	public GameObject point4 = null;
	public GameObject point5 = null;
	public GameObject point6 = null;

	private Dictionary<int, List<GameObject>> followPointMap = new Dictionary<int, List<GameObject>>();

	private int maxPriority = 4;
	private bool started = false;

	public void Start()
	{
		followPointMap.Add(1, new List<GameObject>());
		followPointMap.Add(2, new List<GameObject>());
		followPointMap.Add(3, new List<GameObject>());
		followPointMap.Add(4, new List<GameObject>());

		followPointMap[point1.GetComponent<GroguFollowPoints>().priority].Add(point1);
		followPointMap[point2.GetComponent<GroguFollowPoints>().priority].Add(point2);
		followPointMap[point3.GetComponent<GroguFollowPoints>().priority].Add(point3);
		followPointMap[point4.GetComponent<GroguFollowPoints>().priority].Add(point4);
		followPointMap[point5.GetComponent<GroguFollowPoints>().priority].Add(point5);
		followPointMap[point6.GetComponent<GroguFollowPoints>().priority].Add(point6);
	}

	public void Update()
    {
        if (started == false)
        {
			Start();
			started = true;
        }
    }

	public Vector3 GetPointToFollow(Vector3 pos)
	{
		Vector3 ret = Vector3.zero;

		for (int i = 1; i <= maxPriority; ++i)
        {
			List<GameObject> followPoints = followPointMap[i];
			bool found = false;

            for (int j = 0; j < followPoints.Count; j++)
            {
				if (followPoints[j].GetComponent<GroguFollowPoints>().IsBlocked() == false)
                {
					found = true;

					if (ret != Vector3.zero || ret.Distance(pos) > followPoints[j].GetComponent<Transform>().globalPosition.Distance(pos))
						ret = followPoints[j].transform.globalPosition;
				}
			}

			if (found == true)
				return ret;
		}

		return ret;
	}

}