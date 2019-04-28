// ================================================================================================================================
// File:        EquipmentItemUITooltip.cs
// Description: Displays information about whatever item in the players inventory is currently being moused over
// ================================================================================================================================

using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentItemUITooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject TooltipDisplay = null;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Only show the tooltip window is theres an item equipped in this gear slot
        ItemData MouseoverItemData = GetComponent<EquipSlot>().EquippedItem;
        if(MouseoverItemData == null)
            TooltipDisplay.SetActive(false);
        else
        {
            TooltipDisplay.SetActive(true);
            TooltipDisplay.GetComponent<TooltipDisplay>().UpdateName(MouseoverItemData.DisplayName);
            TooltipDisplay.GetComponent<TooltipDisplay>().UpdateDetails(MouseoverItemData.Description);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipDisplay.SetActive(false);
    }
}
