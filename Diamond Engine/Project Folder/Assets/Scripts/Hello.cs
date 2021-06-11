using System;
using DiamondEngine;

public class Hello : DiamondComponent
{

	public GameObject helloWorld = null;

    public void OnExecuteCheckbox(bool checkbox_active)
    {
    }
    public void OnExecuteButton()
    {
        SceneManager.LoadScene(1076838722);
    }
    public void Update()
	{
	}


}


