using System;
using DiamondEngine;

public class CinematicDialog : DiamondComponent
{
	public GameObject cinematicTextController;

	private float timerDialog;
	public float timeDialog;

	private bool startDialog = false;
	private int countDialogs = 0;
	public void Awake()
    {
		startDialog = false;
    }

	public void Update()
	{
        if (!startDialog || cinematicTextController == null)
        {
			return;
        }
		
		timerDialog += Time.deltaTime;

		if (timerDialog >= timeDialog)
		{
			TextController textController = cinematicTextController.GetComponent<TextController>();
			if (textController != null)
			{
				textController.OnExecuteButton();
				countDialogs++;
			}
			timerDialog = 0;
		}
		if(countDialogs == 2)
        {
			startDialog = false;
			CinematicManager.instance.cinematicDialogue.Enable(false);
        }
	}

	public void StartDialog()
    {
		startDialog = true;
		TextController textController = cinematicTextController.GetComponent<TextController>();
		if (textController != null)
		{
			textController.PauseGame(false);
		}
	}

}