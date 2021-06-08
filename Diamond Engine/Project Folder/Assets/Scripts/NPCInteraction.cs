using System;
using DiamondEngine;



public class NPCInteraction : DiamondComponent
{


    public float maxDistance = 5;
    public GameObject interactionImage = null;
    public GameObject notificationImage = null;
    public GameObject hubTextController = null;

    private Interaction npc = Interaction.GREEF;
    public bool canInteract = false;
    public bool canUpgrade = false;

    private bool start = true;
    private bool once = true;

    public void Awake()
    {
        start = true;
        canUpgrade = false;
        if (hubTextController == null)
        {
            return;
        }
        once = true;

    }

    private void StartFunction()
    {


        HubTextController hubScript = hubTextController.GetComponent<HubTextController>();
        if (hubScript == null)
        {
            return;
        }

        switch (npc)
        {
            case Interaction.BO_KATAN:
                canInteract = hubScript.BoKatanHasInteractions();
                canUpgrade = hubScript.BoKatanCanUpgrade();
                break;
            case Interaction.GREEF:
                canInteract = hubScript.GreefHasInteractions();
                canUpgrade = hubScript.GreefCanUpgrade();
                break;
            case Interaction.ASHOKA:
                canInteract = hubScript.AshokaHasInteractions();
                canUpgrade = hubScript.AshokaCanUpgrade();
                break;
            case Interaction.CARA_DUNE:
                canInteract = hubScript.CaraDuneHasInteractions();
                canUpgrade = hubScript.CaraDuneCanUpgrade();
                break;
            case Interaction.GROGU:
                canInteract = hubScript.GroguHasInteractions();
                canUpgrade = hubScript.GroguCanUpgrade();
                break;
        }
    }

    public void Update()
    {

        if (start)
        {
            StartFunction();
            start = false;
        }
        InteractionImage();
        NotificationImage();
    }

    private void InteractionImage()
    {
        if (interactionImage == null)
            return;

        if (IsInside())
        {
            if (canInteract || canUpgrade)
            {
                interactionImage.Enable(true);
                if (npc == Interaction.GROGU && once)
                {
                    Audio.PlayAudio(gameObject, "Play_Grogu_Dialog");
                }
                else if (npc == Interaction.GREEF && once)
                {
                    Audio.PlayAudio(Core.instance.gameObject, "Play_Male_Popup");
                }
                else if (once)
                {
                    Audio.PlayAudio(gameObject, "Play_Female_Popup");
                }
                once = false;
            }
            else
            {
                interactionImage.Enable(false);
                once = true;
            }
        }
        else if (!IsInside() && interactionImage.IsEnabled())
        {
            interactionImage.Enable(false);
            once = true;
        }
    }
    private void NotificationImage()
    {
        if (notificationImage == null)
            return;

        if ((canUpgrade || canInteract) && !notificationImage.IsEnabled())
        {
            notificationImage.Enable(true);
        }
        if (!canUpgrade && !canInteract && notificationImage.IsEnabled())
        {
            notificationImage.Enable(false);
        }
    }

    public bool IsInside()
    {
        if (Core.instance == null || (hubTextController != null && hubTextController.GetComponent<HubTextController>().insideColliderTextActive))
            return false;
        Vector3 playerPos = Core.instance.gameObject.transform.globalPosition;
        Vector3 colliderPos = gameObject.transform.globalPosition;
        double distance = playerPos.DistanceNoSqrt(colliderPos);

        if (distance >= -maxDistance && distance <= maxDistance)
            return true;
        else
            return false;
    }

    public void SetEnum(Interaction interaction)
    {
        npc = interaction;
    }
    public void UpdateUpgrade()
    {
        HubTextController hubScript = hubTextController.GetComponent<HubTextController>();
        if (hubScript == null)
        {
            return;
        }

        switch (npc)
        {
            case Interaction.BO_KATAN:
                canUpgrade = hubScript.BoKatanCanUpgrade();
                break;
            case Interaction.GREEF:
                canUpgrade = hubScript.GreefCanUpgrade();
                break;
            case Interaction.ASHOKA:
                canUpgrade = hubScript.AshokaCanUpgrade();
                break;
            case Interaction.CARA_DUNE:
                canUpgrade = hubScript.CaraDuneCanUpgrade();
                break;
            case Interaction.GROGU:
                canUpgrade = hubScript.GroguCanUpgrade();
                break;
        }
    }
}