using System;
using DiamondEngine;

public class DeathTrooperBullet : DiamondComponent
{
    public float speed = 60.0f;
    public float maxLifeTime = 5.0f;

    private float currentLifeTime = 0.0f;

    public float range_squared = 0.0f;
    public Vector3 initial_pos = new Vector3(0.0f, 0.0f, 0.0f);
    public bool use_range_instead_of_time = false;

    private float actual_range_squared = 0.0f;

    public float yVel = 0.0f;
    public float damage = 9.0f;

    private bool started = false;
    private bool triggered = false;
    private float timer = 0;

    private GameObject destroyOBJ = null;
    private GameObject mesh = null;
    private Entity entity = null;

    public ParticleSystem destroyPar = null;
    private string tagToAvoid = "None";

    public int numBullets = 2;
    public float bulletsSeparation = 0.0f;

    private bool createT = true;

    private GameObject goToAvoid = null;

    public void Update()
    {
        if (started == false)
        {
            mesh = this.gameObject.GetChild("bulletmesh");
            destroyOBJ = this.gameObject.GetChild("STimpact Particle");
            started = true;
            initial_pos = gameObject.transform.globalPosition;
            //InternalCalls.CreatePrefab("Library/Prefabs/1564072956.prefab", gameObject.transform.globalPosition, gameObject.transform.globalRotation, gameObject.transform.globalScale);
        }

        if (!use_range_instead_of_time)
            currentLifeTime += Time.deltaTime;
        else
            actual_range_squared = (gameObject.transform.globalPosition - initial_pos).magnitude;

        // gameObject.transform.localPosition += gameObject.transform.GetForward() * (speed * Time.deltaTime);
        if (!triggered)
        {
            gameObject.transform.localPosition += gameObject.transform.GetForward() * speed * Time.deltaTime;
        }
        else
        {
            if (mesh.IsEnabled() == true)
            {
                mesh.Enable(false);
            }
            timer += Time.deltaTime;
            gameObject.SetVelocity(new Vector3(0, 0, 0));

        }
        //yVel -= Time.deltaTime / 15.0f;
        //gameObject.transform.localPosition += (Vector3.up * yVel);

        if (triggered == true)
        {
            if (/*timer > 0.5 &&*/ gameObject != null)
            {
                InternalCalls.Destroy(gameObject);
                if (createT)
                    CreateT();
            }
        }
        else if (((!use_range_instead_of_time && currentLifeTime >= maxLifeTime) || (use_range_instead_of_time && actual_range_squared >= range_squared)) && gameObject != null)
        {
            InternalCalls.Destroy(gameObject);
            if (createT)
                CreateT();
        }


    }

    public void SetGameObjectToAvoid(GameObject _goToAvoid)
    {
        goToAvoid = _goToAvoid;
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        int uidToAvoid = goToAvoid != null ? goToAvoid.GetUid() : -1;
        if (triggered == false && uidToAvoid != triggeredGameObject.GetUid() && !triggeredGameObject.CompareTag("Deathtrooper") && !triggeredGameObject.CompareTag("Enemy") && !triggeredGameObject.CompareTag("StormTrooperBullet") && !triggeredGameObject.CompareTag("Player"))
        {
            triggered = true;

            if (destroyOBJ != null)
            {
                destroyPar = destroyOBJ.GetComponent<ParticleSystem>();

                if (destroyPar != null)
                    destroyPar.Play();
            }
        }
        else if (triggeredGameObject.CompareTag("Player"))
        {
            createT = false;

            triggered = true;
            if (destroyOBJ != null)
            {
                destroyPar = destroyOBJ.GetComponent<ParticleSystem>();

                if (destroyPar != null)
                    destroyPar.Play();
            }
        }

    }

    public float GetDamage()
    {

        return damage;
    }

    public void SetEntity(Entity myEntity)
    {
        entity = myEntity;

        if (entity.HasStatus(STATUS_TYPE.PRIM_RANGE))
            range_squared *= (1 + entity.GetStatusData(STATUS_TYPE.PRIM_RANGE).severity / 100);
    }

    public void SetTagToAvoid(string _tagToAvoid)
    {
        tagToAvoid = _tagToAvoid;
    }

    public void CreateT()
    {
        //T LEFT
        Vector3 currentPosition1 = gameObject.transform.globalPosition - gameObject.transform.GetRight().normalized - gameObject.transform.GetForward().normalized * (bulletsSeparation / numBullets);

        for (int i = 0; i < numBullets; i++)
        {
            GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/1002585568.prefab", currentPosition1, gameObject.transform.globalRotation * Quaternion.RotateAroundAxis(Vector3.up, 90 * Mathf.Deg2RRad), null);
            if (bullet != null)
            {
                bullet.GetComponent<BH_Bullet>().damage = damage;
                bullet.transform.localPosition += bullet.transform.GetForward().normalized * 1.25f;
                currentPosition1 += gameObject.transform.GetForward().normalized * bulletsSeparation;
            }
        }
        //T RIGHT
        Vector3 currentPosition2 = gameObject.transform.globalPosition + gameObject.transform.GetRight().normalized + gameObject.transform.GetForward().normalized * (bulletsSeparation / numBullets);

        for (int i = 0; i < numBullets; i++)
        {
            GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/1002585568.prefab", currentPosition2, gameObject.transform.globalRotation * Quaternion.RotateAroundAxis(Vector3.up, -90 * Mathf.Deg2RRad), null);
            if (bullet != null)
            {
                bullet.GetComponent<BH_Bullet>().damage = damage;
                bullet.transform.localPosition += bullet.transform.GetForward().normalized * 1.25f;
                currentPosition2 -= gameObject.transform.GetForward().normalized * bulletsSeparation;
            }
        }
    }
}