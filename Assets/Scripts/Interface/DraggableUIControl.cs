// ================================================================================================================================
// File:        DraggableUIControl.cs
// Description: Handles control over all the draggable UI components and their interactions with each other
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class DraggableUIControl : MonoBehaviour
{
    //Singleton class so it can easily be accessed anywhere in the code
    public static DraggableUIControl Instance = null;
    private void Awake() { Instance = this; }

    public bool DraggingComponent = false;  //Is the mouse currently dragging a UI component
    public DraggableUIComponent CurrentComponent = null;    //The UI component currently being dragged
    public GameObject DraggingObject = null;    //The UI component rendering beneath the mouse cursor the item currently being dragged around
    public Image DraggingIcon = null;   //icon for dragging component
    public Text DraggingName = null;    //display name for dragging component
    public DraggableUIComponent[] InventorySlots;   //All the slots in the players inventory
    public DraggableUIComponent[] EquipmentSlots;   //All the different gear slots in the players equipment screen
    public DraggableUIComponent[] ActionBarSlots;   //All the ability gem slots on the players action bar

    //Returns an inventory item slot with the given inventory bag slot number
    public DraggableUIComponent GetInventorySlot(int BagSlotNumber)
    {
        //Check all of the slots in the characters inventory
        foreach(DraggableUIComponent InventorySlot in InventorySlots)
        {
            //Return the slot which has the matching BagSlotNumber value
            if (InventorySlot.InventorySlotNumber == BagSlotNumber)
                return InventorySlot;
        }

        //Couldnt find the matching inventory slot
        return null;
    }

    //Returns an equipment slot with the matching EquipmentSlot type
    public DraggableUIComponent GetEquipmentSlot(EquipmentSlot EquipmentSlotType)
    {
        //Check all of the characters equipment slots
        foreach(DraggableUIComponent EquipmentSlot in EquipmentSlots)
        {
            //Return the equipment slot which has the matching EquipmentSlot type
            if (EquipmentSlot.EquipSlotType == EquipmentSlotType)
                return EquipmentSlot;
        }

        //Couldnt find the matching equipment slot
        return null;
    }

    //Returns an action bar slot with the matching action bar slot number
    public DraggableUIComponent GetActionBarSlot(int ActionBarSlotNumber)
    {
        //Check all of the characters action bar slots
        foreach(DraggableUIComponent ActionBarSlot in ActionBarSlots)
        {
            //Return the action bar slot with the matching slot number
            if (ActionBarSlot.ActionBarSlotNumber == ActionBarSlotNumber)
                return ActionBarSlot;
        }

        //Couldnt find the matching action bar slot
        return null;
    }

    private void Start()
    {
        //Initially want the dragging item icon to be hidden from view
        DraggingObject.SetActive(false);
    }

    private void Update()
    {
        //Position the dragging component icon at the mouse location while dragging items around
        if (DraggingComponent)
            DraggingObject.transform.position = Input.mousePosition;
    }

    //Triggered by interacting with UI components while playing the game
    public void StartDragging(DraggableUIComponent UIComponent)
    {
        //l.og("start dragging " + UIComponent.transform.name);
        DraggingComponent = true;
        CurrentComponent = UIComponent;
        CurrentComponent.SetVisibility(false);
        DraggingObject.SetActive(true);
        DraggingName.text = UIComponent.ItemData.DisplayName;
        DraggingIcon.sprite = UIComponent.ItemData.Icon;
    }

    //Trigger once you release the mouse after dragging an item around the UI
    public void StopDragging(DraggableUIComponent DraggingSlot)
    {
        //Stop dragging the icon around, place it back where it came from in the UI
        DraggingComponent = false;
        CurrentComponent.SetVisibility(true);
        CurrentComponent = null;
        DraggingObject.SetActive(false);

        //First we need to check if the item the user stopped dragging was dragged out from their inventory, from their equipment screen or from the action bar
        switch (DraggingSlot.UIType)
        {
            //Items dragged out of the inventory
            case (UIComponentType.InventorySlot):
                StopDraggingFromInventory(DraggingSlot);
                break;

            //Gear dragged out of the equipment screen
            case (UIComponentType.EquipmentSlot):
                StopDraggingFromEquipment(DraggingSlot);
                break;

            //Gem dragged off the ability bar
            case (UIComponentType.ActionBarSlot):
                StopDraggingFromAbilityBar(DraggingSlot);
                break;
        }
    }

    //Handles actions to be taken after finished dragging an item somewhere out of the players inventory
    private void StopDraggingFromInventory(DraggableUIComponent DraggingSlot)
    {
        //First figure if, which one of the inventory slots they stopped dragging this item over
        for(int i = 0; i < InventorySlots.Length; i++)
        {
            //Ignore this inventory slot if this isnt where the mouse is
            if (!InventorySlots[i].MouseInside)
                continue;

            //If they stopped dragging over the same inventory slot they started from, nothing needs to be done
            if(DraggingSlot == InventorySlots[i])
            {
                //Dragged item from inventory back to the same bag slot it came from, do nothing
                return;
            }

            //Second check if they stopped dragging it on top of some other empty slot in the inventory
            if (InventorySlots[i].ItemData == null)
            {
                //Dragged item from inventory to a different empty bag slot, move the item to that bag slot
                PacketManager.Instance.SendMoveInventoryItem(PlayerManager.Instance.GetCurrentPlayerName(), DraggingSlot.InventorySlotNumber, InventorySlots[i].InventorySlotNumber);
                return;
            }
            //Otherwise, we know there is an item here so the two items should swap positions
            else
            {
                //Dragged item to another full bag space, swap the items positions in the inventory
                PacketManager.Instance.SendSwapInventoryItem(PlayerManager.Instance.GetCurrentPlayerName(), DraggingSlot.InventorySlotNumber, InventorySlots[i].InventorySlotNumber);
                return;
            }
        }

        //If it wasnt found the user stopped dragging the item over an inventory slot, next check if they released the item over any equipment slot
        for(int i = 0; i < EquipmentSlots.Length; i++)
        {
            //Ignore this equipment slot if this isnt where the mouse is
            if (!EquipmentSlots[i].MouseInside)
                continue;

            //Do nothing if they drag a piece of equipment onto the slot of another equipment type
            if(DraggingSlot.ItemData.Slot != EquipmentSlots[i].EquipSlotType)
            {
                //Dragged item from inventory to an incompatible equipment gear slot, do nothing
                l.og("1: " + DraggingSlot.ItemData.Name + " from inventory to " + EquipmentSlots[i].EquipSlotType + " incompatible gear slot, do nothing.");
                return;
            }
            else
            {
                //If the equipment slot is empty, then the item is simply equipped here to this slot
                if (EquipmentSlots[i].ItemData == null)
                {
                    //Dragged item from inventory to an empty equipment gear slot, equip the item in this gear slot
                    l.og("2: " + DraggingSlot.ItemData.Name + " from inventory to " + EquipmentSlots[i].EquipSlotType + " empty+compatible gear slot, equip it here.");

                    PacketManager.Instance.SendEquipItemRequest(PlayerManager.Instance.GetCurrentPlayerName(), DraggingSlot.InventorySlotNumber, EquipmentSlots[i].EquipSlotType);
                    return;
                }
                //Otherwise, the current piece needs to be removed and replaced with the piece that was dragged on
                else
                {
                    //Dragged item from inventory to a filled equipment gear slot, equip the new item to that slot and unequip the previous item placing it where the previous item was being stored
                    l.og("3: " + DraggingSlot.ItemData.Name + " from inventory to " + EquipmentSlots[i].EquipSlotType + " full+compatible gear slot, swap them.");
                    PacketManager.Instance.SendPlayerSwapEquipmentItem(PlayerManager.Instance.GetCurrentPlayerName(), DraggingSlot.InventorySlotNumber, EquipmentSlots[i].EquipSlotType);
                    return;
                }
            }
        }

        //If it wasnt found the user stopped dragging the item over any inventory or equipment gear slot, finally check if they released the item over one of the action bar slots
        for(int i = 0; i < ActionBarSlots.Length; i++)
        {
            //Ignore this action bar slot if this isnt where the mouse is
            if (!ActionBarSlots[i].MouseInside)
                continue;

            //If they drag a non ability gem item into onto one of the action bar slots then do nothing
            if(DraggingSlot.ItemData.Type != ItemType.AbilityGem)
            {
                //Dragged an item from the inventory onto the action bar that wasnt an ability gem, do nothing
                return;
            }
            else
            {
                if(ActionBarSlots[i].ItemData == null)
                {
                    //Dragged an ability gem from the inventory onto an empty action bar slot, equip the ability
                    PacketManager.Instance.SendCharacterEquipAbility(PlayerManager.Instance.GetCurrentPlayerName(), DraggingSlot.InventorySlotNumber, ActionBarSlots[i].ActionBarSlotNumber);
                    return;
                }
                else
                {
                    //Dragged an ability gem from the inventory onto a filled action bar slot, swap the gems positions around
                    PacketManager.Instance.SendCharacterSwapEquipAbility(PlayerManager.Instance.GetCurrentPlayerName(), DraggingSlot.InventorySlotNumber, ActionBarSlots[i].ActionBarSlotNumber);
                    return;
                }
            }
        }
        
        //Dragged an item from the inventory to the game world, remove the item from the players inventory and drop it at their current position
        PacketManager.Instance.SendDropInventoryItem(PlayerManager.Instance.GetCurrentPlayerName(), DraggingSlot.InventorySlotNumber, PlayerManager.Instance.GetCurrentCharacterLocation());
    }

    //Handles actions to be taken after finished dragging a piece of gear somewhere out of the players equipment screen
    private void StopDraggingFromEquipment(DraggableUIComponent DraggingSlot)
    {
        //First check if they stopped dragging the item into one of their inventory slots
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            //Ignore this slot if it isnt where the mouse is right now
            if (!InventorySlots[i].MouseInside)
                continue;

            //Drag from equipment to empty inventory slot
            if (InventorySlots[i].ItemData == null)
            {
                //Dragged a piece of equipment to an empty inventory slot, unequip the item and store it in this bag slot
                PacketManager.Instance.SendUnequipItemRequest(PlayerManager.Instance.GetCurrentPlayerName(), DraggingSlot.EquipSlotType, InventorySlots[i].InventorySlotNumber);
                return;
            }
            //Drag from equipment to filled inventory slot
            else
            {
                //When dragging item into a filled inventory slot, the items locations are swapped if the item in the bag slot equips into the same gear slot as the item being dragged to it
                if(InventorySlots[i].EquipSlotType == DraggingSlot.EquipSlotType)
                {
                    //Dragged a piece of equipment to an already filled inventory slot with an item of the same gear slot type, swap the items positions around
                    PacketManager.Instance.SendPlayerSwapEquipmentItem(PlayerManager.Instance.GetCurrentPlayerName(), InventorySlots[i].InventorySlotNumber, DraggingSlot.EquipSlotType);
                    return;
                }
                else
                {
                    //Dragged a piece of equipment to an already filled inventory slot with an item of some different gear slot type, do nothing
                    return;
                }
            }
        }

        //Second, check if they stopped dragging the item into one of the other equipment slots
        for (int i = 0; i < EquipmentSlots.Length; i++)
        {
            //Ignore this slot if it isnt where the mouse is right now
            if (!EquipmentSlots[i].MouseInside)
                continue;

            //Drag from equipment to same equipment slot
            if (EquipmentSlots[i] == DraggingSlot)
            {
                //Dragged a piece of equipment back onto the same gear slot that it came from, do nothing
                return;
            }
            else
            {
                //Dragged a piece of equipment onto a different equipment gear slot, do nothing
                return;
            }
        }

        //Thirdly, check if they stopped dragging the item onto one of the action bar slots
        for (int i = 0; i < ActionBarSlots.Length; i++)
        {
            //Ignore this action bar slot if it isnt where the mouse is right now
            if (!ActionBarSlots[i].MouseInside)
                continue;
            
            //Dragged a piece of equipment onto the action bar, do nothing
            return;
        }
        
        //Dragged a piece of equipment out into the game world, unequip that item and drop it at the characters current location
        PacketManager.Instance.SendDropEquipmentItem(PlayerManager.Instance.GetCurrentPlayerName(), DraggingSlot.EquipSlotType, PlayerManager.Instance.GetCurrentCharacterLocation());
    }

    //Handles actions to be taken after finished dragging a gem somewhere off of the players ability bar
    private void StopDraggingFromAbilityBar(DraggableUIComponent DraggingSlot)
    {
        //First check if they stopped dragging the ability bar gem onto one of the inventory slots
        for (int i = 0; i < InventorySlots.Length; i++)
        {
            //Ignore this inventory bag slot if the mouse isnt here
            if (!InventorySlots[i].MouseInside)
                continue;

            if (InventorySlots[i].ItemData == null)
            {
                //Dragged an ability gem to an empty inventory slot, unequip the ability and store it in that bag slot
                PacketManager.Instance.SendCharacterUnequipAbility(PlayerManager.Instance.GetCurrentPlayerName(), InventorySlots[i].InventorySlotNumber, DraggingSlot.ActionBarSlotNumber);
                return;
            }
            else
            {
                if(InventorySlots[i].ItemData.Type == ItemType.AbilityGem)
                {
                    //Dragged an ability gem to an already filled bag space containing some other ability gem, swap the gems locations around
                    PacketManager.Instance.SendCharacterSwapEquipAbility(PlayerManager.Instance.GetCurrentPlayerName(), InventorySlots[i].InventorySlotNumber, DraggingSlot.ActionBarSlotNumber);
                    return;
                }
                else
                {
                    //Dragged an ability gem to an already filled bag space containing some other type of item, do nothing
                    return;
                }
            }
        }

        //Second check if they stopped dragging the ability gem onto one of the equipment slots
        for (int i = 0; i < EquipmentSlots.Length; i++)
        {
            //Ignore this equipment slot if ths mouse isnt here
            if (!EquipmentSlots[i].MouseInside)
                continue;
            
            //Deragged an ability gem to one of the equipment gear slots, do nothing
            return;
        }

        //Third check if they stopped dragging the ability gem onto one of the other ability gem slots
        for (int i = 0; i < ActionBarSlots.Length; i++)
        {
            //Ignore this action bar slot if the mouse isnt there
            if (!ActionBarSlots[i].MouseInside)
                continue;

            if (ActionBarSlots[i].ItemData == null)
            {
                //Dragged an ability gem to one of the other empty action bar slots, move the ability to this slot
                PacketManager.Instance.SendCharacterMoveAbility(PlayerManager.Instance.GetCurrentPlayerName(), DraggingSlot.ActionBarSlotNumber, ActionBarSlots[i].ActionBarSlotNumber);
                return;
            }
            else
            {
                //Dragged an ability gem to one of the other nonempty action bar slots, swap the two abilities positions on the bar
                PacketManager.Instance.SendCharacterSwapAbilities(PlayerManager.Instance.GetCurrentPlayerName(), DraggingSlot.ActionBarSlotNumber, ActionBarSlots[i].ActionBarSlotNumber);
                return;
            }
        }
        
        //Dragged an ability gem into the game world, unequip the ability and drop the item into the game world at the characters current location
        PacketManager.Instance.SendDropActionBarItem(PlayerManager.Instance.GetCurrentPlayerName(), DraggingSlot.ActionBarSlotNumber, PlayerManager.Instance.GetCurrentCharacterLocation());
        return;
    }
}
