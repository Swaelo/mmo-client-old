// ================================================================================================================================
// File:        EquipSlot.cs
// Description: Handles everything and stores all the relevant information for a piece of equipment worn on the character
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour
{
    //Components to display the item equipped in this slot to the user interface
    public Image ItemIcon;
    public Text ItemName;
    //Define the types of items that are allowed to be equipped to this gear slot
    public EquipmentSlot GearSlotType = EquipmentSlot.NULL;
    //and detail everything about items that are equipped in the slot currently
    public ItemData EquippedItem = null;

    //Takes an item and equips it into this gear slot
    public void EquipItem(ItemData NewItem)
    {
        //NewItem.ItemNumber == 0 means there is no item equipped in this gear slot
        if (NewItem == null || NewItem.ItemNumber == 0)
            EquippedItem = null;
        else
            EquippedItem = NewItem;
        
        UpdateDisplay();
    }

    //Takes off whatever piece of equipment is in this slot and places it back into the players inventory
    public void RemoveItem()
    {
        EquippedItem = null;
        UpdateDisplay();
    }

    //Updates the UI to show what is being equipped in this slot currently
    private void UpdateDisplay()
    {
        ItemIcon.sprite = EquippedItem ? EquippedItem.Icon : null;
        ItemIcon.color = EquippedItem ? Color.white : Color.clear;
    }
}
