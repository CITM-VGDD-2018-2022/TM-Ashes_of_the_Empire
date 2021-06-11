using System;
using DiamondEngine;

public class DynamicProps : DiamondComponent
{
    public enum STATE : int
    {
        NONE = -1,
        IDLE,
        WAIT,
        MOVE,
        ROTATE,
        SCREAM,
    }

    //INSPECTOR FLOAT INT GAMEOBJECT BOOl PUBLIC LO DEMAS PRIVATE
    private STATE currentState = STATE.NONE;

    //Variables
    public int RotateAxis = 0;
    public int MoveAxis = 0;
    public float RotateAngle = 90;
    public float moveSpeed = 2.0f;
    public float rotateSpeed = 0.200f;

    //Timers
    private float idleTimer = 0.0f;
    private float screamTimer = 0.0f;
    private float waitTimer = 0.0f;
    private float rotateTimer = 0.0f;

    //Action times
    private float idleTime = 0.0f;
    private float screamTime = 0.0f;
    private float rotateTime = 0.0f;

    private Vector3 initialPos;
    private Quaternion initialRot;

    public bool reset;
    public bool start_Idle = false;
    public bool is_atat = false;


    public float waitSeconds = 2.0f;

    public int STATUS;
    public void Awake()
    {


        if (gameObject.tag == "LittleMen")
        {
            StartIdle();
            currentState = STATE.IDLE;
        }
        else
            currentState = STATE.WAIT;

        if (!start_Idle)
            SwitchState(STATE.MOVE);

        initialPos = new Vector3(gameObject.transform.localPosition);
        initialRot = gameObject.transform.localRotation;

        reset = false;

        //SCREAM
        if (!is_atat)
            screamTime = Animator.GetAnimationDuration(gameObject, "PD_Scream");

        //IDLE
        idleTime = waitSeconds;

        //ROTATE
        rotateTime = RotateAngle * Mathf.Deg2RRad / rotateSpeed;
    }

    public void DebugState()
    {
        STATUS = (int)currentState;
    }

    public void Update()
    {
        UpdateState();
        DebugState();
    }

    public void UpdateState()
    {
        switch (currentState)
        {
            case STATE.IDLE:
                UpdateIdle();
                break;

            case STATE.WAIT:
                UpdateWait(waitSeconds);
                break;

            case STATE.MOVE:
                UpdateMove(MoveAxis);
                break;

            case STATE.ROTATE:
                UpdateRotate(RotateAxis, RotateAngle);
                break;

            case STATE.SCREAM:
                UpdateScream();
                break;

            default:
                break;
        }
    }

    public void SwitchState(STATE nextState)
    {
        switch (currentState)
        {
            case STATE.IDLE:
                switch (nextState)
                {
                    case STATE.SCREAM:
                        currentState = STATE.SCREAM;
                        StartScream();
                        break;
                    case STATE.MOVE:
                        currentState = STATE.MOVE;
                        StartMove();
                        break;
                }
                break;

            case STATE.WAIT:
                switch (nextState)
                {
                    case STATE.ROTATE:
                        currentState = STATE.ROTATE;
                        StartRotate();
                        break;

                    case STATE.MOVE:
                        currentState = STATE.MOVE;
                        StartMove();
                        break;
                    case STATE.SCREAM:
                        currentState = STATE.SCREAM;
                        StartScream();
                        break;
                }
                break;

            case STATE.MOVE:
                switch (nextState)
                {
                    case STATE.IDLE:
                        currentState = STATE.IDLE;
                        StartIdle();
                        break;
                    case STATE.WAIT:
                        currentState = STATE.WAIT;
                        StartWait();
                        break;

                    case STATE.ROTATE:
                        currentState = STATE.ROTATE;
                        StartRotate();
                        break;

                    case STATE.SCREAM:
                        currentState = STATE.SCREAM;
                        break;
                }
                break;

            case STATE.ROTATE:
                switch (nextState)
                {
                    case STATE.MOVE:
                        currentState = STATE.MOVE;
                        StartMove();
                        break;
                }
                break;

            case STATE.SCREAM:
                switch (nextState)
                {
                    case STATE.IDLE:
                        currentState = STATE.IDLE;
                        StartIdle();
                        break;

                    case STATE.WAIT:
                        currentState = STATE.WAIT;
                        StartWait();
                        break;

                    case STATE.MOVE:
                        currentState = STATE.MOVE;
                        StartMove();
                        break;
                }
                break;

            default:
                break;
        }
    }

    public void TriggerAction(string tag)
    {
        switch (tag)
        {
            case "Rotate90":
                SwitchState(STATE.ROTATE);
                break;

            case "Rotate-90":
                SwitchState(STATE.ROTATE);
                break;

            case "Scream":
                SwitchState(STATE.SCREAM);
                break;

            case "Wait":
                SwitchState(STATE.WAIT);
                break;

            case "Reset":
                reset = true;
                SwitchState(STATE.WAIT);
                break;
            case "First_Pos":
                SwitchState(STATE.ROTATE);
                break;
            case "Second_Pos":
                SwitchState(STATE.ROTATE);
                break;
            case "Top":
                SwitchState(STATE.WAIT);
                moveSpeed = -Math.Abs(moveSpeed);
                break;
            case "Bottom":
                //SwitchState(STATE.MOVE);
                moveSpeed = Math.Abs(moveSpeed);
                break;
            case "Point1":
                RotateAngle = 74.203f;
                rotateTime = RotateAngle * Mathf.Deg2RRad / rotateSpeed;
                SwitchState(STATE.ROTATE);
                break;
            case "Point2":
                RotateAngle = 45.160f;
                rotateTime = RotateAngle * Mathf.Deg2RRad / rotateSpeed;
                SwitchState(STATE.ROTATE);
                break;
            case "Point3":
                reset = true;
                SwitchState(STATE.IDLE);
                break;
            case "Top2":
                moveSpeed = -Math.Abs(moveSpeed);
                break;
            case "Bottom2":
                SwitchState(STATE.WAIT);
                moveSpeed = Math.Abs(moveSpeed);
                break;

            default:
                break;
        }
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        TriggerAction(triggeredGameObject.tag);
    }

    public void OnTriggerExit()
    {

    }

    #region IDLE
    public void StartIdle()
    {
        idleTimer = waitSeconds;

        if (is_atat)
            Animator.Play(gameObject, "AT-AT_Idle");
        else
            Animator.Play(gameObject, "PD_Idle");
    }

    public void UpdateIdle()
    {
        if (idleTimer > 0.0f)
        {
            idleTimer -= Time.deltaTime;

            if (idleTimer <= 0.0f)
            {
                SwitchState(STATE.SCREAM);

                if (reset)
                    Reset();
            }
        }
    }

    #endregion

    #region WAIT
    public void StartWait()
    {
        waitTimer = waitSeconds;
        if (is_atat)
            Animator.Play(gameObject, "AT-AT_Idle");
        else
            Animator.Play(gameObject, "PD_Idle");
    }

    public void UpdateWait(float seconds)
    {
        if (waitTimer > 0.0f)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0.0f)
            {
                SwitchState(STATE.MOVE);
            }
        }
    }
    #endregion

    #region MOVE
    public void StartMove()
    {
        if (is_atat)
            Animator.Play(gameObject, "AT-AT_Walk");
        else
            Animator.Play(gameObject, "PD_Walk");
    }

    public void UpdateMove(int axis)
    {
        if (axis == 0)
        {
            //Vector3 newPos = new Vector3(gameObject.transform.localPosition.x + moveSpeed * Time.deltaTime, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);

            gameObject.transform.localPosition += gameObject.transform.GetRight().normalized * moveSpeed * Time.deltaTime;
        }
        else if (axis == 1)
        {
            Vector3 newPos = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y + moveSpeed * Time.deltaTime, gameObject.transform.localPosition.z);

            gameObject.transform.localPosition = newPos;
        }
        else
        {
            //Vector3 newPos = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z + moveSpeed * Time.deltaTime);

            gameObject.transform.localPosition += gameObject.transform.GetForward().normalized * moveSpeed * Time.deltaTime;
        }

    }
    #endregion

    #region ROTATE
    public void StartRotate()
    {
        if (is_atat)
            Animator.Play(gameObject, "AT-AT_Walk");
        else
            Animator.Play(gameObject, "PD_Rotate");

        if (rotateTime != 0.0f)
            rotateTimer = rotateTime;
        else
            rotateTimer = 1.0f;

    }

    public void UpdateRotate(int axis, float angle)
    {

        if (rotateTimer > 0.0f)
        {
            rotateTimer -= Time.deltaTime;

            if (rotateTimer <= 0.0f)
            {
                SwitchState(STATE.MOVE);
                return;
            }
        }

        if (axis == 0)
            gameObject.transform.localRotation *= Quaternion.RotateAroundAxis(Vector3.right, rotateSpeed * Time.deltaTime);

        else if (axis == 1)
            gameObject.transform.localRotation *= Quaternion.RotateAroundAxis(Vector3.up, rotateSpeed * Time.deltaTime);

        else
            gameObject.transform.localRotation *= Quaternion.RotateAroundAxis(Vector3.forward, rotateSpeed * Time.deltaTime);


    }

    #endregion

    #region SCREAM
    public void StartScream()
    {
        screamTimer = screamTime;

        if (!is_atat)
            Animator.Play(gameObject, "PD_Scream");
    }

    public void UpdateScream()
    {
        if (gameObject.tag == "LittleMen")
        {
            if (screamTimer > 0.0f)
            {
                screamTimer -= Time.deltaTime;

                if (screamTimer <= 0.0f)
                {
                    SwitchState(STATE.IDLE);
                }
            }
        }
    }
    #endregion

    public void Reset()
    {
        gameObject.transform.localPosition = initialPos;
        gameObject.transform.localRotation = initialRot;

        SwitchState(STATE.MOVE);
    }
}