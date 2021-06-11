using System;
using DiamondEngine;

public class FPS_Controller : DiamondComponent
{
    public GameObject reference = null;
    public GameObject turret = null;
    public GameObject quitMenu = null;
    public GameObject win = null;
    public GameObject lose = null;
    public GameObject default_selected = null;
    public GameObject background = null;

    public float rotationSpeed = 2.0f;
    public float movementSpeed = 35.0f;
    public float mouseSens = 1.0f;
    public bool dashUp = true;

    int X_dir = 0, Z_dir = 0, lastX_dir = 0, lastZ_dir = 0;
    bool dashing = false;
    float dashCD = 0.33f;
    float dashCD_counter = 0.0f;
    float dashDuration = 0.25f;
    float dashingCounter = 0.0f;

    public void Update()
	{
        if (this.reference == null)
            return;


        if (dashCD_counter < dashCD && !dashUp && !dashing) dashCD_counter += Time.deltaTime;
        else 
        {
            dashCD_counter = 0.0f;
            dashUp = true;
        }
    
 
        if (Input.GetKey(DEKeyCode.SPACE) == KeyState.KEY_DOWN && dashUp && !dashing)
        {
            dashing = true;
            dashUp = false;
            dashingCounter = 0.0f;
        }

        if (dashing) Dash();                   
        else 
        {
            X_dir = Z_dir = 0;

            if (Input.GetKey(DEKeyCode.W) == KeyState.KEY_REPEAT)
            {
                gameObject.transform.localPosition += gameObject.transform.GetForward() * movementSpeed * Time.deltaTime;
                lastZ_dir = Z_dir = 1;
            }
            if (Input.GetKey(DEKeyCode.S) == KeyState.KEY_REPEAT)
            {
                gameObject.transform.localPosition += gameObject.transform.GetForward() * -movementSpeed * Time.deltaTime;
                lastZ_dir = Z_dir = -1;
            }
            if (Input.GetKey(DEKeyCode.A) == KeyState.KEY_REPEAT)
            {
                gameObject.transform.localPosition += gameObject.transform.GetRight() * movementSpeed * Time.deltaTime;
                lastX_dir = X_dir = 1;
            }
            if (Input.GetKey(DEKeyCode.D) == KeyState.KEY_REPEAT)
            {
                gameObject.transform.localPosition += gameObject.transform.GetRight() * -movementSpeed * Time.deltaTime;
                lastX_dir = X_dir = -1;
            }

            //Update las direction if not axis input received
            if (X_dir != 0 && Z_dir == 0) lastZ_dir = 0;
            if (Z_dir != 0 && X_dir == 0) lastX_dir = 0;
        }

        if (Input.GetMouseX() != 0 && turret != null)
            turret.transform.localRotation = Quaternion.RotateAroundAxis(Vector3.up, -Input.GetMouseX() * mouseSens * Time.deltaTime) * turret.transform.localRotation;

        if (Input.GetGamepadButton(DEControllerButton.START) == KeyState.KEY_DOWN)
        {
            if (quitMenu.IsEnabled())
                quitMenu.Enable(false);
            else {
                quitMenu.Enable(true);
                quitMenu.GetComponent<Pause>().DisplayBoons();
                background.Enable(true);
                Time.PauseGame();
                if (default_selected != null)
                    default_selected.GetComponent<Navigation>().Select();
            }

        }

        if (Input.GetKey(DEKeyCode.F1)== KeyState.KEY_DOWN && !lose.IsEnabled())
        {
            win.Enable(true);
        }
        if (Input.GetKey(DEKeyCode.F2)==KeyState.KEY_DOWN && !win.IsEnabled())
        {
            lose.Enable(true);
        }
        if (Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_DOWN && (win.IsEnabled() || lose.IsEnabled()))
        {
            SceneManager.LoadScene(1726826608);
        }
    }

    private void Dash()
    {
        if (dashingCounter < dashDuration)
        {
            dashingCounter += Time.deltaTime;
            //Diagonal movement has to be less in each axis to be equal to one axis movement
            //Last dir is used for when character is not moving --> moves in last direction
            if(Z_dir == 0 && X_dir == 0)
            {
                //Use last known direction
                if (lastZ_dir == 0 || lastX_dir == 0)
                {
                    if (lastZ_dir == 0) gameObject.transform.localPosition += gameObject.transform.GetRight() * (movementSpeed * 2) * Time.deltaTime * lastX_dir;
                    else gameObject.transform.localPosition += gameObject.transform.GetForward() * (movementSpeed * 2) * Time.deltaTime * lastZ_dir;
                }
                else
                {
                    gameObject.transform.localPosition += gameObject.transform.GetForward() * (movementSpeed * 1.3f) * Time.deltaTime * lastZ_dir;
                    gameObject.transform.localPosition += gameObject.transform.GetRight() * (movementSpeed * 1.3f) * Time.deltaTime * lastX_dir;
                }
            }
            else if(Z_dir == 0 || X_dir == 0)
            {
                if(Z_dir == 0) gameObject.transform.localPosition += gameObject.transform.GetRight() * (movementSpeed * 2) * Time.deltaTime * X_dir;
                else gameObject.transform.localPosition += gameObject.transform.GetForward() * (movementSpeed * 2) * Time.deltaTime * Z_dir;
            }
            else
            {
                gameObject.transform.localPosition += gameObject.transform.GetForward() * (movementSpeed * 1.3f) * Time.deltaTime * Z_dir;
                gameObject.transform.localPosition += gameObject.transform.GetRight() * (movementSpeed * 1.3f) * Time.deltaTime * X_dir;
            }
        }
        else
        {
            dashing = false;          
        }
    }
   
}