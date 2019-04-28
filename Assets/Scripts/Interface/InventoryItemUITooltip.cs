// ================================================================================================================================
// File:        InventoryItemUITooltip.cs
// Description: Displays information about whatever item in the players inventory is currently being moused over
// ================================================================================================================================

using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryItemUITooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject TooltipDisplay = null;   //The inventories ItemTooltip UI object

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Only show the tooltip window if theres an item in this bag slot
        ItemData MouseoverItemData = GetComponent<InventorySlot>().BagItemData;
        if(MouseoverItemData == null)
        {
            //Hide it if the mouse is over an empty bag slot
            TooltipDisplay.SetActive(false);
        }
        else
        {
            //Show the tooltip then update all its information
            TooltipDisplay.SetActive(true);
            TooltipDisplay.GetComponent<TooltipDisplay>().UpdateName(MouseoverItemData.DisplayName);
            TooltipDisplay.GetComponent<TooltipDisplay>().UpdateDetails(MouseoverItemData.Description);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Hide the item mouseover tooltip
        TooltipDisplay.SetActive(false);
    }
}
