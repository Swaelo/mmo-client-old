// ================================================================================================================================
// File:        EquipSlot.cs
// Description: 
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class EquipSlot : MonoBehaviour
{
    public GameObject IconObject;
    
    private Image ItemIcon;
    private Item Item = null;

    //Checks if there is any item currently equipped in this slot
    public bool SlotAvailable()
    {
        return Item == null;
    }

    //Assigns the item icon references and updates the sprite icon display
    private void Start()
    {
        ItemIcon = IconObject.GetComponent<Image>();
        UpdateDisplay();
    }

    //Updates the icon display
    public void UpdateDisplay()
    {
        ItemIcon.sprite = Item ? Item.Icon : null;
        ItemIcon.color = Item ? Color.white : Color.clear;
    }

    //Stores a new item in the slot
    public void UpdateItem(Item NewItem)
    {
        Item = NewItem;
        UpdateDisplay();
    }

    //Removes any currently equipped item from the slot
    public void SetEmpty()
    {
        Item = null;
        UpdateDisplay();
    }

    //Items are unequipped when they are right clicked from the equipment screen
    public void Use()
    {
        
    }
}
