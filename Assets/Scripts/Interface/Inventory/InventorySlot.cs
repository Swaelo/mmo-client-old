// ================================================================================================================================
// File:        InventorySlot.cs
// Description:
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    //Child objects containing the Icon Image and Name Text components
    public GameObject IconObject;
    public GameObject TextObject;
    private Image ItemIcon;
    private Text ItemName;
    //The item object itself
    public Item Item;

    private void Start()
    {
        //Grab the components used to display this item in the bag then update its display
        ItemIcon = IconObject.GetComponent<Image>();
        ItemName = TextObject.GetComponent<Text>();
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        ItemIcon.sprite = Item ? Item.Icon : null;
        ItemIcon.color = Item ? Color.white : Color.clear;
        ItemName.text = Item ? Item.Name : "";
    }

    public void UpdateItem(Item NewItem)
    {
        Item = NewItem;
        UpdateDisplay();
    }

    public void SetEmpty()
    {
        Item = null;
        UpdateDisplay();
    }

    //Items are used when they are right clicked from the inventory
    public void Use()
    {
        if(Item)
            Item.Use();
    }
}
