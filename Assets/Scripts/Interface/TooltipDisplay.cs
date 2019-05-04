// ================================================================================================================================
// File:        TooltipDisplay.cs
// Description: Used to update the item tooltips information
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipDisplay : MonoBehaviour
{
    //Use the inspector to assign the two gameobjects used to detail the item tooltip
    public GameObject TooltipItemNameObject = null;
    public GameObject TooltipItemDetailObject = null;

    private void Start()
    {
        //The item tooltip should be disabled to begin with
        gameObject.SetActive(false);
    }

    public void UpdateName(string NewName)
    {
        //Only update the name if we have a reference to its object
        if (TooltipItemNameObject != null)
            TooltipItemNameObject.GetComponent<TextMeshProUGUI>().text = NewName;
    }

    public void UpdateDetails(string NewDetails)
    {
        //Only update the details if we have a refernce to its object
        if (TooltipItemDetailObject != null)
            TooltipItemDetailObject.GetComponent<TextMeshProUGUI>().text = NewDetails;
    }

    //Toggles the visibility of the tooltip display object
    public void SetVisibility(bool Visible)
    {
        TooltipItemNameObject.SetActive(Visible);
        TooltipItemDetailObject.SetActive(Visible);
        GetComponent<Image>().color = Visible ? Color.white : Color.clear;
    }
}
