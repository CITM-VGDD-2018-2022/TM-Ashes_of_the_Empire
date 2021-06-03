using System;
using System.Collections.Generic;
using DiamondEngine;

public class TextController : DiamondComponent
{
	public GameObject gui = null;
	public GameObject dialog = null;
	public GameObject text = null;
	public GameObject nameText = null;
	//public GameObject mandoimage = null;
	//public GameObject otherimage = null;
	public GameObject list_of_dialogs = null;
	public int dialog_index = -1;

	public GameObject img_mando_serious = null;
	public GameObject img_mando_ashamed = null;
	public GameObject img_mando_angry = null;
	public GameObject img_mando_threatening = null;
	public GameObject img_mando_joking = null;
	public GameObject img_mando_happy = null;
	public GameObject img_grogu_serious = null;
	public GameObject img_grogu_laughing = null;
	public GameObject img_grogu_curious = null;
	public GameObject img_grogu_scared = null;
	public GameObject img_bokatan_serious = null;
	public GameObject img_bokatan_angry = null;
	public GameObject img_cara_serious = null;
	public GameObject img_cara_angry1 = null;
	public GameObject img_cara_angry2 = null;
	public GameObject img_cara_ashamed = null;
	public GameObject img_greef_serious = null;
	public GameObject img_greef_laughing = null;
	public GameObject img_greef_thinker = null;
	public GameObject img_greef_ashamed = null;
	public GameObject img_ahsoka_serious = null;
	public GameObject img_ahsoka_thinker = null;
	public GameObject img_ahsoka_melancholic = null;
	public GameObject img_moff_serious = null;
	public GameObject img_moff_jester = null;
	public GameObject img_moff_angry = null;
	public GameObject img_moff_threatening1 = null;
	public GameObject img_moff_threatening2 = null;
	public GameObject img_moff_defeated = null;


	private int index = -1;
	private bool gui_not_enabled = true;

	private List<String> texts;
	private String charName = "";
	private List<bool> images;
	private List<DialogImages> dialogImages;

	private DialogImages lastImage = DialogImages.IMG_NONE;
	private DialogImages currentImage = DialogImages.IMG_NONE;
	private bool startMenu = true;
	private bool finished = false;

	private float timer = 0.05f;

	public void Awake()
    {
		if (gui == null)
			gui = InternalCalls.FindObjectWithName("HUD");

		timer = Time.deltaTime;
    }

	public void OnExecuteButton()
	{
		if (startMenu==true)
			return;
        if (++index < texts.Count)
        {
			if(text!=null)
				text.GetComponent<Text>().text = texts[index];

			if (index < dialogImages.Count && index > -1)
			{
				currentImage = dialogImages[index];
				if (lastImage != currentImage)
				{
					TurnOffImage(lastImage);
					TurnOnImage(currentImage);
					lastImage = currentImage;
				}
			}


			if (images[index] == true)
            {
				if (nameText != null)
					nameText.GetComponent<Text>().text = "Mando";
				/*if (mandoimage != null)
				{
					mandoimage.Enable(true);
					if (nameText != null)
						nameText.GetComponent<Text>().text = "Mando";
				}
				if(otherimage!=null)
					otherimage.Enable(false);*/
            }
            else
            {
				if (nameText != null)
					nameText.GetComponent<Text>().text = charName;
				/*if (mandoimage!=null)
					mandoimage.Enable(false);
				if (otherimage != null)
				{
					otherimage.Enable(true);
					if(nameText != null)
						nameText.GetComponent<Text>().text = charName;
				}*/
            }

        }

        if (index >= texts.Count && gui_not_enabled)
        {
            index = -1;
			/*if(mandoimage!=null)
				mandoimage.Enable(true);
			if (otherimage != null)
				otherimage.Enable(true);*/
			if (text != null)
				text.GetComponent<Text>().text = " ";
			if (dialog != null)
				dialog.Enable(false);
			if (nameText != null)
				nameText.GetComponent<Text>().text = " ";
            finished = true;
            if (gui != null)
            {
                gui.Enable(true);
                gui.GetComponent<HUD>().ResetCombo();
                gui_not_enabled = false;
            }
            Time.ResumeGame();
			TurnOffImage(lastImage);
			TurnOffImage(currentImage);
			//InternalCalls.Destroy(dialog);
		}
    }
    public void Update()
	{
		if (gui == null && Core.instance != null)
			gui = Core.instance.hud;

		if (startMenu == true && dialog != null && list_of_dialogs != null && dialog_index >= 0)
		{
			//Debug.Log(dialog_index.ToString());

			if (gui != null)
			{
				gui.Enable(false);
			}
			startMenu = false;
			Time.PauseGame();
			texts = list_of_dialogs.GetComponent<List_Of_Dialogs>().GetListOfDialog((uint)dialog_index);
			charName = list_of_dialogs.GetComponent<List_Of_Dialogs>().GetDialogName((uint)dialog_index);
			images = list_of_dialogs.GetComponent<List_Of_Dialogs>().GetListOfOrder((uint)dialog_index);
			dialogImages = list_of_dialogs.GetComponent<List_Of_Dialogs>().GetListOfImages((uint)dialog_index);
			OnExecuteButton();
		}
		else if (finished == true)
		{
			//Debug.Log(finished.ToString());

			startMenu = true;
			finished = false;
		}
	}

	private void TurnOffImage(DialogImages lastImg)
	{
		//Debug.Log(lastImg.ToString());
		switch (lastImg)
		{
			case DialogImages.IMG_MANDO_SERIOUS:
				if (img_mando_serious != null) img_mando_serious.Enable(false);
				break;
			case DialogImages.IMG_MANDO_JOKING:
				if (img_mando_joking != null) img_mando_joking.Enable(false);
				break;
			case DialogImages.IMG_MANDO_HAPPY:
				if (img_mando_happy != null) img_mando_happy.Enable(false);
				break;
			case DialogImages.IMG_MANDO_ASHAMED:
				if (img_mando_ashamed != null) img_mando_ashamed.Enable(false);
				break;
			case DialogImages.IMG_MANDO_ANGRY:
				if (img_mando_angry != null) img_mando_angry.Enable(false);
				break;
			case DialogImages.IMG_MANDO_THREATENING:
				if (img_mando_threatening != null) img_mando_threatening.Enable(false);
				break;
			case DialogImages.IMG_GROGU_SERIOUS:
				if (img_grogu_serious != null) img_grogu_serious.Enable(false);
				break;
			case DialogImages.IMG_GROGU_LAUGHING:
				if (img_grogu_laughing != null) img_grogu_laughing.Enable(false);
				break;
			case DialogImages.IMG_GROGU_SCARED:
				if (img_grogu_scared != null) img_grogu_scared.Enable(false);
				break;
			case DialogImages.IMG_GROGU_CURIOUS:
				if (img_grogu_curious != null) img_grogu_curious.Enable(false);
				break;
			case DialogImages.IMG_GREEF_SERIOUS:
				if (img_greef_serious != null) img_greef_serious.Enable(false);
				break;
			case DialogImages.IMG_GREEF_ASHAMED:
				if (img_greef_ashamed != null) img_greef_ashamed.Enable(false);
				break;
			case DialogImages.IMG_GREEF_LAUGHING:
				if (img_greef_laughing != null) img_greef_laughing.Enable(false);
				break;
			case DialogImages.IMG_GREEF_THINKER:
				if (img_greef_thinker != null) img_greef_thinker.Enable(false);
				break;
			case DialogImages.IMG_CARA_SERIOUS:
				if (img_cara_serious != null) img_cara_serious.Enable(false);
				break;
			case DialogImages.IMG_CARA_ASHAMED:
				if (img_cara_ashamed != null) img_cara_ashamed.Enable(false);
				break;
			case DialogImages.IMG_CARA_ANGRY1:
				if (img_cara_angry1 != null) img_cara_angry1.Enable(false);
				break;
			case DialogImages.IMG_CARA_ANGRY2:
				if (img_cara_angry2 != null) img_cara_angry2.Enable(false);
				break;
			case DialogImages.IMG_BOKATAN_SERIOUS:
				if (img_bokatan_serious != null) img_bokatan_serious.Enable(false);
				break;
			case DialogImages.IMG_BOKATAN_ANGRY:
				if (img_bokatan_angry != null) img_bokatan_angry.Enable(false);
				break;
			case DialogImages.IMG_AHSOKA_SERIOUS:
				if (img_ahsoka_serious != null) img_ahsoka_serious.Enable(false);
				break;
			case DialogImages.IMG_AHSOKA_MELANCHOLIC:
				if (img_ahsoka_melancholic != null) img_ahsoka_melancholic.Enable(false);
				break;
			case DialogImages.IMG_AHSOKA_THINKER:
				if (img_ahsoka_thinker != null) img_ahsoka_thinker.Enable(false);
				break;
			case DialogImages.IMG_MOFF_SERIOUS:
				if (img_moff_serious != null) img_moff_serious.Enable(false);
				break;
			case DialogImages.IMG_MOFF_JESTER:
				if (img_moff_jester != null) img_moff_jester.Enable(false);
				break;
			case DialogImages.IMG_MOFF_ANGRY:
				if (img_moff_angry != null) img_moff_angry.Enable(false);
				break;
			case DialogImages.IMG_MOFF_THREATENING1:
				if (img_moff_threatening1 != null) img_moff_threatening1.Enable(false);
				break;
			case DialogImages.IMG_MOFF_THREATENING2:
				if (img_moff_threatening2 != null) img_moff_threatening2.Enable(false);
				break;
			case DialogImages.IMG_MOFF_DEFEATED:
				if (img_moff_defeated != null) img_moff_defeated.Enable(false);
				break;
			default:
				break;
		}
	}

	private void TurnOnImage(DialogImages newImg)
	{
		//Debug.Log(newImg.ToString());
		switch (newImg)
		{
			case DialogImages.IMG_MANDO_SERIOUS:
				if (img_mando_serious != null) img_mando_serious.Enable(true);
				break;
			case DialogImages.IMG_MANDO_JOKING:
				if (img_mando_joking != null) img_mando_joking.Enable(true);
				break;
			case DialogImages.IMG_MANDO_HAPPY:
				if (img_mando_happy != null) img_mando_happy.Enable(true);
				break;
			case DialogImages.IMG_MANDO_ASHAMED:
				if (img_mando_ashamed != null) img_mando_ashamed.Enable(true);
				break;
			case DialogImages.IMG_MANDO_ANGRY:
				if (img_mando_angry != null) img_mando_angry.Enable(true);
				break;
			case DialogImages.IMG_MANDO_THREATENING:
				if (img_mando_threatening != null) img_mando_threatening.Enable(true);
				break;
			case DialogImages.IMG_GROGU_SERIOUS:
				if (img_grogu_serious != null) img_grogu_serious.Enable(true);
				break;
			case DialogImages.IMG_GROGU_LAUGHING:
				if (img_grogu_laughing != null) img_grogu_laughing.Enable(true);
				break;
			case DialogImages.IMG_GROGU_SCARED:
				if (img_grogu_scared != null) img_grogu_scared.Enable(true);
				break;
			case DialogImages.IMG_GROGU_CURIOUS:
				if (img_grogu_curious != null) img_grogu_curious.Enable(true);
				break;
			case DialogImages.IMG_GREEF_SERIOUS:
				if (img_greef_serious != null) img_greef_serious.Enable(true);
				break;
			case DialogImages.IMG_GREEF_ASHAMED:
				if (img_greef_ashamed != null) img_greef_ashamed.Enable(true);
				break;
			case DialogImages.IMG_GREEF_LAUGHING:
				if (img_greef_laughing != null) img_greef_laughing.Enable(true);
				break;
			case DialogImages.IMG_GREEF_THINKER:
				if (img_greef_thinker != null) img_greef_thinker.Enable(true);
				break;
			case DialogImages.IMG_CARA_SERIOUS:
				if (img_cara_serious != null) img_cara_serious.Enable(true);
				break;
			case DialogImages.IMG_CARA_ASHAMED:
				if (img_cara_ashamed != null) img_cara_ashamed.Enable(true);
				break;
			case DialogImages.IMG_CARA_ANGRY1:
				if (img_cara_angry1 != null) img_cara_angry1.Enable(true);
				break;
			case DialogImages.IMG_CARA_ANGRY2:
				if (img_cara_angry2 != null) img_cara_angry2.Enable(true);
				break;
			case DialogImages.IMG_BOKATAN_SERIOUS:
				if (img_bokatan_serious != null) img_bokatan_serious.Enable(true);
				break;
			case DialogImages.IMG_BOKATAN_ANGRY:
				if (img_bokatan_angry != null) img_bokatan_angry.Enable(true);
				break;
			case DialogImages.IMG_AHSOKA_SERIOUS:
				if (img_ahsoka_serious != null) img_ahsoka_serious.Enable(true);
				break;
			case DialogImages.IMG_AHSOKA_MELANCHOLIC:
				if (img_ahsoka_melancholic != null) img_ahsoka_melancholic.Enable(true);
				break;
			case DialogImages.IMG_AHSOKA_THINKER:
				if (img_ahsoka_thinker != null) img_ahsoka_thinker.Enable(true);
				break;
			case DialogImages.IMG_MOFF_SERIOUS:
				if (img_moff_serious != null) img_moff_serious.Enable(true);
				break;
			case DialogImages.IMG_MOFF_JESTER:
				if (img_moff_jester != null) img_moff_jester.Enable(true);
				break;
			case DialogImages.IMG_MOFF_ANGRY:
				if (img_moff_angry != null) img_moff_angry.Enable(true);
				break;
			case DialogImages.IMG_MOFF_THREATENING1:
				if (img_moff_threatening1 != null) img_moff_threatening1.Enable(true);
				break;
			case DialogImages.IMG_MOFF_THREATENING2:
				if (img_moff_threatening2 != null) img_moff_threatening2.Enable(true);
				break;
			case DialogImages.IMG_MOFF_DEFEATED:
				if (img_moff_defeated != null) img_moff_defeated.Enable(true);
				break;
			default:
				break;
		}
	}
}