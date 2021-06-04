using System;
using DiamondEngine;

public class EndLevelRewardSpawn : DiamondComponent
{
    public bool trigger = false;
    public float rotSpeedDegSec = 1.0f;
    float rotationAngle = 0.0f;
    public float verticalSpeedMultiplier = 0.5f;
    bool goingUp = true;
    float animTime = 0.0f;
    public float movementSpeed = 1.0f;

    private float timer = 0.0f;
    public float timeToStarMovingToPlayer = 5.0f;

    public GameObject particleSystemObj = null;
    private ParticleSystem particleSystem = null;

    public void Awake()
    {
        timer = timeToStarMovingToPlayer;

        if(particleSystemObj != null)
        {
            particleSystem = particleSystemObj.GetComponent<ParticleSystem>();
        }
    }

    public void OnTriggerEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject != null)
        {
            if(collidedGameObject.CompareTag("Player"))
            {
                Core.instance.gameObject.GetComponent<PlayerHealth>().SetInvincible(false);
                trigger = true;
                Audio.PlayAudio(gameObject, "Play_UI_Boon_Pickup");
            }
            else
            {
                timer = 0.0f;
            }
        }
    }

    public void AdvanceVerticalMovement(Vector3 initialPos)
    {
        if (goingUp)
        {
            animTime += Time.deltaTime * verticalSpeedMultiplier;
        }
        else
        {
            animTime -= Time.deltaTime * verticalSpeedMultiplier;
        }

        if (animTime > 1.0f)
        {
            goingUp = false;
            animTime = 1.0f;
        }
        else if (animTime < 0.0f)
        {
            goingUp = true;
            animTime = 0.0f;

        }
        float yPos = ParametricBlend(animTime);

        Vector3 newPos = new Vector3(gameObject.transform.localPosition.x, initialPos.y, gameObject.transform.localPosition.z);

        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            newPos.y += yPos * movementSpeed;

            //Debug.Log("Boon timer: " + timer.ToString());
        }
        else
        {
            if (Core.instance != null)
            {
                Vector3 movementVector = (Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition).normalized * movementSpeed * Time.deltaTime;
                newPos.x += movementVector.x;
                newPos.y += movementVector.y;
                newPos.z += movementVector.z;
            }
        }

        gameObject.transform.localPosition = newPos;
    }

    public void AdvanceRotation()
    {
        rotationAngle += rotSpeedDegSec * Time.deltaTime;
        Vector3 axis = new Vector3(0.0f, 1.0f, 0.0f);
        gameObject.transform.localRotation = Quaternion.RotateAroundAxis(axis, rotationAngle);
    }

    public float ParametricBlend(float t) => ((t * t) / (2.0f * ((t * t) - t) + 1.0f));

    public void PlayParticles()
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }
}