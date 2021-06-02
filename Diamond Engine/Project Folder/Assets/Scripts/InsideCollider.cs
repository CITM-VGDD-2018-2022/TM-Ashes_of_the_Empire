using System;
using DiamondEngine;

public class InsideCollider : DiamondComponent
{
    public float maxDistance = 5;
    public GameObject colliderPosition = null;
    public GameObject displayText = null;
    public GameObject displaySecondaryText = null;
    public GameObject selectButton = null;
    public GameObject hubTextController = null;

    public void Update()
    {
        if (displayText == null)
            return;

        if (IsInside() && displayText.IsEnabled() == false && (displaySecondaryText == null || displaySecondaryText.IsEnabled() == false)) 
        {
            displayText.EnableNav(true);

            if (hubTextController != null)
                hubTextController.GetComponent<HubTextController>().insideColliderTextActive = true;

            if (selectButton != null)
            {
                Navigation navComponent = selectButton.GetComponent<Navigation>();

                if (navComponent != null)
                    navComponent.Select();
            }
        }
        else if (!IsInside() && displayText.IsEnabled())
        {
            displayText.EnableNav(false);
            if (hubTextController != null)
                hubTextController.GetComponent<HubTextController>().insideColliderTextActive = false;
        }
        else if (!IsInside() && displaySecondaryText!=null && displaySecondaryText.IsEnabled())
        {
            displaySecondaryText.EnableNav(false);
            if (hubTextController != null)
                hubTextController.GetComponent<HubTextController>().insideColliderTextActive = false;
        }
       
    }

    public bool IsInside()
    {
        if (Core.instance == null || colliderPosition == null)
            return false;

        Vector3 playerPos = Core.instance.gameObject.transform.globalPosition;
        Vector3 colliderPos = colliderPosition.transform.globalPosition;
        double distance = playerPos.DistanceNoSqrt(colliderPos);

        if (distance >= -maxDistance && distance <= maxDistance)
            return true;
        else
            return false;
    }
}