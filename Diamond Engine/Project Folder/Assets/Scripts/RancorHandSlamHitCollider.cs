using DiamondEngine;

class RancorHandSlamHitCollider : DiamondComponent
{
    private int damage = 25;

    private Vector3 initPos = new Vector3(0f, 3.5f, 2.0f);
    private Vector3 finalPos = new Vector3(0f, 0.4f, 2.0f);
    private float speed = 1.5f;
    private float timer = 0.0f;
    public float myDeltaTimeMult = 1f;

    private bool start = true;

    public void Update()
    {
        if (start)
        {
            start = false;
            gameObject.transform.localPosition = initPos;
            timer = 0f;
            gameObject.transform.localRotation = Quaternion.RotateAroundAxis(Vector3.up, 3.14159f);

        }

        if (timer < 1)
        {

            timer += Time.deltaTime * speed * myDeltaTimeMult;
        }
        else
        {
            timer = 1f;
        }
        float y = Mathf.Lerp(gameObject.transform.localPosition.y, finalPos.y, timer);
        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, y, gameObject.transform.localPosition.z);

    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Player"))
        {
            PlayerHealth health = triggeredGameObject.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);
        }
    }

    public void RestartCollider()
    {
        start = true;
        myDeltaTimeMult = 1f;
    }


}

