using System;
using DiamondEngine;
using System.Collections.Generic;

public class AimBot : DiamondComponent
{
    enum ObjectiveType
    {
        IS_PROP,
        IS_ENEMY,
        NONE
    }
    public float startAimConeAngle = 90.0f;//this will only be used on the inspector, to change the actual angle use the method
    public float maxRange = 50.0f; //maximum distance at wich it will
    public float angleWeight = 0.5f;
    public float threadWeight = 0.5f;
    public float distanceWeight = 0.5f;
    float dotMin = 0;
    int lastFrameEnemyCount = 0;
    int lastFramePropCount = 0;
    public GameObject shootPoint = null;
    GameObject myCurrentObjective = null;
    public bool isShooting = false;

    ObjectiveType objectiveType = ObjectiveType.NONE;
    public void Awake()
    {
        objectiveType = ObjectiveType.NONE;
        dotMin = 0;
        lastFrameEnemyCount = 0;
        lastFramePropCount = 0;
        myCurrentObjective = null;
        isShooting = false;
        ChangeConeAngle(startAimConeAngle);
    }

    public void Update()
    {

        if (!isShooting && myCurrentObjective != null)
        {
            myCurrentObjective = null;
            objectiveType = ObjectiveType.NONE;
        }

        if (isShooting)
        {
            if (myCurrentObjective == null)
            {
                SearchForNewObjective();
            }
            else if (EnemyManager.currentEnemies != null)
            {
                //if((EnemyManager.currentEnemies != null && lastFrameEnemyCount != EnemyManager.currentEnemies.Count) || 
                //    (EnemyManager.currentDestructibleProps != null && lastFramePropCount != EnemyManager.currentDestructibleProps.Count)) //at least one of the lists has changed
                if (lastFrameEnemyCount != EnemyManager.currentEnemies.Count && objectiveType == ObjectiveType.IS_ENEMY)
                {
                    bool containsObj = false;
                    foreach (GameObject enemy in EnemyManager.currentEnemies)
                    {
                        if (enemy.GetUid() == myCurrentObjective.GetUid())
                        {
                            containsObj = true;
                            break;
                        }
                    }

                    if (!containsObj)
                    {
                        SearchForNewObjective();
                    }
                }
                else if (objectiveType == ObjectiveType.IS_PROP)
                {

                    if (EnemyManager.currentDestructibleProps != null)
                    {
                        if (lastFramePropCount != EnemyManager.currentDestructibleProps.Count)
                        {

                            bool containsObj = false;
                            foreach (GameObject prop in EnemyManager.currentDestructibleProps)
                            {
                                if (prop.GetUid() == myCurrentObjective.GetUid())
                                {
                                    containsObj = true;
                                    break;
                                }
                            }

                            if (!containsObj)
                            {
                                SearchForNewObjective();
                            }
                        }
                    }
                    else
                    {
                        SearchForNewObjective();
                    }
                }

            }
        }


        //Update enemy number on last frame
        if (EnemyManager.currentEnemies != null)
        {
            lastFrameEnemyCount = EnemyManager.currentEnemies.Count;
        }
        else
        {
            lastFrameEnemyCount = 0;
        }
        //Update prop number on last frame
        if (EnemyManager.currentDestructibleProps != null)
        {
            lastFramePropCount = EnemyManager.currentDestructibleProps.Count;
        }
        else
        {
            lastFramePropCount = 0;
        }
    }

    //takes the overall cone angle as an input (an angle of 90 means 45 degrees to each side of the player forward)
    public void ChangeConeAngle(float newAngle)
    {
        float newAngleRad = newAngle * (1 / Mathf.Rad2Deg);
        dotMin = (float)Math.Cos(newAngleRad * 0.5f);
        //Debug.Log("New Dot Min set to: " + dotMin + " From Angle: " + newAngle);
    }

    //assigns a new objective if there is any objective in range, otherwise myCurrentObjective is null
    public void SearchForNewObjective()
    {
        if (EnemyManager.currentEnemies == null)
        {
            myCurrentObjective = null;
            objectiveType = ObjectiveType.NONE;
            return;
        }


        //Test
        //if (spawnComponent.currentEnemies.Count > 0)
        //    myCurrentObjective = spawnComponent.currentEnemies[0];
        //end test

        //Debug.Log("Searching for a new objective!");
        KeyValuePair<float, GameObject> weightedObj = new KeyValuePair<float, GameObject>(float.NegativeInfinity, null);

        //Debug.Log("Enemies searching num: " + EnemyManager.currentEnemies.Count.ToString());
        for (int i = 0; i < EnemyManager.currentEnemies.Count; ++i)
        {
            float targetWeight = GetTargetWeight(EnemyManager.currentEnemies[i], maxRange, dotMin, false);

            if (targetWeight > weightedObj.Key)
            {
                //float hitDistance = 0.0f;
                //GameObject hit = InternalCalls.RayCast(shootPoint.transform.globalPosition + shootPoint.transform.GetRight() * 0.15f,
                //       (EnemyManager.currentEnemies[i].transform.globalPosition - shootPoint.transform.globalPosition).normalized, maxRange, ref hitDistance);
                //if (hit.GetUid() == EnemyManager.currentEnemies[i].GetUid())
                //{
                weightedObj = new KeyValuePair<float, GameObject>(targetWeight, EnemyManager.currentEnemies[i]);
                objectiveType = ObjectiveType.IS_ENEMY;
                //}
            }
        }
        if (EnemyManager.currentDestructibleProps != null)
        {
            //Debug.Log("Prop searching num: " + EnemyManager.currentDestructibleProps.Count.ToString());
            for (int i = 0; i < EnemyManager.currentDestructibleProps.Count; ++i)
            {
                float targetWeight = GetTargetWeight(EnemyManager.currentDestructibleProps[i], maxRange, dotMin, true);

                if (targetWeight > weightedObj.Key)
                {
                    //float hitDistance = 0.0f;
                    //GameObject hit = InternalCalls.RayCast(shootPoint.transform.globalPosition + shootPoint.transform.GetRight() * 0.15f, 
                    //    (EnemyManager.currentDestructibleProps[i].transform.globalPosition- shootPoint.transform.globalPosition).normalized, maxRange, ref hitDistance);
                    //if (hit.GetUid() == EnemyManager.currentEnemies[i].GetUid())
                    //{
                    weightedObj = new KeyValuePair<float, GameObject>(targetWeight, EnemyManager.currentDestructibleProps[i]);
                    objectiveType = ObjectiveType.IS_PROP;
                    //}
                }
            }
        }

        if (weightedObj.Key != float.NegativeInfinity)
        {
            myCurrentObjective = weightedObj.Value;
            //Debug.Log("Objective found: " + myCurrentObjective.name);
            //Debug.Log("New Objecte! Position: " + weightedObj.Value.Name.ToString());
        }
        else
        {
            //Debug.Log("No Objective found!!");
            myCurrentObjective = null;
            objectiveType = ObjectiveType.NONE;
            //Debug.Log("No suitable objective found!");
        }
    }

    //Search for objective raw. Returns a game object given max search dist and an angle in degrees for the aim cone
    public GameObject SearchForNewObjRaw(float coneAngleDeg, float maxSearchDist)
    {
        if (EnemyManager.currentEnemies == null)
        {
            return null;
        }

        float newAngleRad = coneAngleDeg * (1 / Mathf.Rad2Deg);
        float newDotMin = (float)Math.Cos(newAngleRad * 0.5f);

        //Debug.Log("Searching for a new objective!");
        KeyValuePair<float, GameObject> weightedObj = new KeyValuePair<float, GameObject>(float.NegativeInfinity, null);

        //Debug.Log("Enemies searching num: " + EnemyManager.currentEnemies.Count.ToString());
        for (int i = 0; i < EnemyManager.currentEnemies.Count; ++i)
        {
            float targetWeight = GetTargetWeight(EnemyManager.currentEnemies[i], maxRange, newDotMin, false);


            if (targetWeight > weightedObj.Key)
            {
                weightedObj = new KeyValuePair<float, GameObject>(targetWeight, EnemyManager.currentEnemies[i]);
            }
        }
        if (EnemyManager.currentDestructibleProps != null)
        {
            for (int i = 0; i < EnemyManager.currentDestructibleProps.Count; ++i)
            {
                float targetWeight = GetTargetWeight(EnemyManager.currentDestructibleProps[i], maxRange, newDotMin, true);


                if (targetWeight > weightedObj.Key)
                {
                    weightedObj = new KeyValuePair<float, GameObject>(targetWeight, EnemyManager.currentDestructibleProps[i]);
                }
            }
        }

        if (weightedObj.Key != float.NegativeInfinity)
        {
            return weightedObj.Value;
        }
        else
        {
            //   Debug.Log("No Objective found!!");
            return null;
        }


    }

    float GetTargetWeight(GameObject target, float myMaxRange, float minDotProduct, bool isProp)
    {
        float newDistanceWeight = 0.0f;
        float newAngleWeight = 0.0f;
        float newThreadWeight = 0.0f;

        Vector3 myPos = new Vector3(gameObject.transform.globalPosition.x, 0.0f, gameObject.transform.globalPosition.z);
        Vector3 myForward = new Vector3(gameObject.transform.GetForward().x, 0.0f, gameObject.transform.GetForward().z).normalized;
        Vector3 targetPos = new Vector3(target.transform.globalPosition.x, 0.0f, target.transform.globalPosition.z);
        Vector3 targetVector = targetPos - myPos;//note it is not normalized
        float distance = targetVector.magnitude;
        Vector3 targetDirection = targetVector.normalized;
        float targetDot = Vector3.Dot(myForward, targetDirection);


        if (distance <= myMaxRange)
        {
            newDistanceWeight = ((myMaxRange - distance) / myMaxRange) * distanceWeight;//number between 0 & 1 being 1 when the distance is the smallest *distance weight
            //newDistanceWeight = (1 / distance) * distanceWeight;//TODO simplification
        }

        if (targetDot >= minDotProduct)
        {
            newAngleWeight = Mathf.InvLerp(minDotProduct, 1.0f, targetDot) * angleWeight;
            //newAngleWeight = targetDot * angleWeight;//TODO simplification
        }

        if (isProp)
        {
            newThreadWeight = GetThreadWeightRaw(target, 3.0f) * threadWeight;
        }


        if (newDistanceWeight != 0.0f && newAngleWeight != 0.0f)
            return newDistanceWeight + newAngleWeight + newThreadWeight;
        else
            return float.NegativeInfinity;
    }

    private float GetThreadWeightRaw(GameObject target, float targetRadius = 1.0f)
    {
        const int maxEnemiesThreadLvl = 4; //TODO alex change this const when needed
        int enemiesInsideRange = 0;
        float threadPerEnemy = 1.0f / maxEnemiesThreadLvl;
        float threadLvl = 0.0f;
        float radiusPow = targetRadius * targetRadius;
        for (int i = 0; i < EnemyManager.currentEnemies.Count; ++i)
        {
            if (enemiesInsideRange > maxEnemiesThreadLvl)
                break;

            //if (EnemyManager.currentEnemies[i] == target) //Not needed as props are in a separate list from enemies, uncomment if we want to use this method in entites too
            //    continue;

            float squareDist = target.transform.globalPosition.DistanceNoSqrt(EnemyManager.currentEnemies[i].transform.globalPosition);

            if (squareDist <= radiusPow)
            {
                //enemy is inside range
                enemiesInsideRange += 1;
            }
        }



        threadLvl += threadPerEnemy * enemiesInsideRange;
        threadLvl = Math.Min(threadLvl, 1.0f);
        return threadLvl;
    }

    public bool HasObjective()
    {
        return (myCurrentObjective != null);
    }

    public void RotateToObjective()
    {
        if (!HasObjective())
            return;

        //Debug.Log("Rotating to the objective!");

        Vector3 targetDire = (myCurrentObjective.transform.globalPosition - gameObject.transform.globalPosition);
        targetDire.y = 0.0f;
        targetDire = targetDire.normalized;
        float angle = (float)Math.Atan2(targetDire.x, targetDire.z);
        Quaternion dir = Quaternion.RotateAroundAxis(Vector3.up, angle);
        gameObject.transform.localRotation = dir;
        //Debug.Log("ROTATE ANGLE: " + angle.ToString());

    }


}