// ================================================================================================================================
// File:        InventorySlot.cs
// Description:
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    //Components to display the item in this bag slot to the user interface
    public Image ItemIcon;
    public Text ItemName;
    //Data about what item is being held in this bag slot and how to render it to the UI and in the game world
    public ItemData BagItemData = null;

    //Takes an item and starts storing it in this inventory slot
    public void StoreItem(ItemData NewItem)
    {
        l.og("Store Item: " + NewItem.DisplayName);
        BagItemData = NewItem;
        UpdateUIDisplay();
    }

    //Removes whatever item is being stored in this inventory slot
    public void RemoveItem()
    {
        BagItemData = null;
        UpdateUIDisplay();
    }

    //Updates the UI display of the players inventory screen
    private void UpdateUIDisplay()
    {
        ItemIcon.sprite = BagItemData ? BagItemData.Icon : null;
        ItemIcon.color = BagItemData ? Color.white : Color.clear;
        ItemName.text = BagItemData ? BagItemData.DisplayName : "";
    }

    //Items are used when they are right clicked on from the inventory UI
    public void Use()
    {
        if(BagItemData)
            BagItemData.Use();
    }
}
