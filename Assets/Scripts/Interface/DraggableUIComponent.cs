// ================================================================================================================================
// File:        DraggableUIComponent.cs
// Description: Used for Inventory Bag Slots, Equipment Gear Slots, Action Bar slots etc
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum UIComponentType
{
    InventorySlot,  //Item slots in the players inventory
    EquipmentSlot,  //Gear slots in the players equipment
    ActionBarSlot   //Ability gem slots on the players action bar
}

public class DraggableUIComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image ItemIcon;  //Icon shown on the UI for this item
    public Text ItemName;   //Name shown on the UI for this item
    public ItemData ItemData = null;   //Data about this item
    public int ItemID = -1;
    public DraggableUIControl UIManager; //UI Manager class used to access other UI components
    public bool MouseInside = false;    //Is the mouse currently hovering over this UI component
    public UIComponentType UIType;  //What type of UI component is this
    public EquipmentSlot EquipSlotType; //What type of equipment can be equipped here (if this is an equipment slot)
    public int InventorySlotNumber = 0; //If this is an inventory slot, which one is it
    public int ActionBarSlotNumber = 0; //If this is an action bar slot, which one is it
    public TooltipDisplay TooltipDisplayManager = null; //If assigned, displays the UI components item info in the target tooltip display object

    //Called when pointer click interaction changes with the UI component
    public void OnPointerClick(PointerEventData eventData)
    {
        //Check for when the user right clicks on one of the UI components
        //if (eventData.button == PointerEventData.InputButton.Right)
        //{
        //    //Right click action depends on what type of UI component we are dealing with
        //    switch(UIType)
        //    {
        //        case (UIComponentType.InventorySlot):
        //            {
        //                //Right-Click an empty inventory slot, do nothing
        //                if (ItemData == null)
        //                    return;
        //                else
        //                {
        //                    switch(ItemData.Type)
        //                    {
        //                        case (ItemType.Consumable):
        //                            //Right-Click a consumable, drink it

        //                            break;
        //                        case (ItemType.Equipment):
        //                            //Right-Click a piece of equipment

        //                            break;
        //                        case (ItemType.AbilityGem):
        //                            {
        //                                //Right-Click an ability gem
        //                            }
        //                            break;
        //                    }
        //                }
        //            }
        //            return;

        //        case (UIComponentType.EquipmentSlot):
        //            Log.Print("right clicked equipment slot.");
        //            return;

        //        case (UIComponentType.ActionBarSlot):
        //            Log.Print("right clicked action bar slot.");
        //            return;
        //    }
        //}
    }

    //Called when the mouse starts dragging from this UI component
    public void OnPointerDown(PointerEventData eventData)
    {
        //If theres an item in this slot and we arent already dragging then pick it up
        if(ItemData != null && !UIManager.DraggingComponent)
        {
            UIManager.StartDragging(this);
        }
    }

    //Called when the mouse has finished dragging this UI component
    public void OnPointerUp(PointerEventData eventData)
    {
        if (UIManager.DraggingComponent)
            UIManager.StopDragging(this);
    }

    //Called when the mouse starts hoving over this UI component
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Keep track of when the mouse is and isnt currently hovering over each dragging UI component seperately
        MouseInside = true;

        //Only start showing the tooltip window if theres an item to show the details of
        if(ItemData == null)
        {
            //Hide the tooltip display if there is no item in this bag slot
            TooltipDisplayManager.SetVisibility(false);
        }
        else
        {
            //Show the tooltip window and update it with all the items information
            TooltipDisplayManager.SetVisibility(true);
            TooltipDisplayManager.UpdateName(ItemData.DisplayName);
            TooltipDisplayManager.UpdateDetails(ItemData.Description);
        }
    }

    //Called when the mouse goes away, no longer hoving over this UI component
    public void OnPointerExit(PointerEventData eventData)
    {
        //Keep track of when the mouse is and isnt currently hovering over each dragging UI component seperately
        MouseInside = false;
        //Hide the tooltip info window
        TooltipDisplayManager.SetVisibility(false);
    }

    //Make the component visible/invisible
    public void SetVisibility(bool Visible)
    {
        ItemIcon.sprite = Visible ? ItemData.Icon : null;
        ItemIcon.color = Visible ? Color.white : Color.clear;
        ItemName.text = Visible ? ItemData.DisplayName : "";
    }

    public void UpdateUIDisplay()
    {
        ItemIcon.sprite = ItemData ? ItemData.Icon : null;
        ItemIcon.color = ItemData ? Color.white : Color.clear;
        ItemName.text = ItemData ? ItemData.DisplayName : "";
    }
}
