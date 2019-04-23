// ================================================================================================================================
// File:        PlayerInventoryControl.cs
// Description: Allows the player to view and manage their inventory, use and equip items from their bag and pickup and drop items in the game world
// ================================================================================================================================

using System;
using UnityEngine;

public class PlayerInventoryControl : MonoBehaviour
{
    //Class is a singleton so the inventory can be managed easily from anywhere in the codebase
    public static PlayerInventoryControl Instance = null;
    private void Awake() { Instance = this; }

    //References to the UI components used to display the inventory to the player
    private GameObject InventoryUIPanel;
    public GameObject[] InventorySlotPanels;

    private void Start()
    {
        //Assign references to the inventory ui objects
        InventoryUIPanel = GameObject.Find("Inventory Panel");
        InventorySlotPanels = new GameObject[9];
        for(int i = 0; i < 9; i++)
        {
            string NextPanelName = "Bag Slot " + (i + 1);
            InventorySlotPanels[i] = GameObject.Find(NextPanelName);
        }
    }

    public void UseInventoryItem(int ItemSlot)
    {
        //Grab the InventorySlot component of the slot that has been used
        InventorySlot InventorySlot = InventorySlotPanels[ItemSlot - 1].GetComponent<InventorySlot>();
        
        //Check if there is an item in this slot
        if(InventorySlot.Item)
        {
            //Find out what type of item this is
            ItemType Type = InventorySlot.Item.Type;

            //Right Clicking items in the players inventory has different effects depending on what type of item it is
            switch (Type)
            {
                //Consumables are simply used
                case (ItemType.Consumable):
                    InventorySlot.Use();
                    l.og("Player is consuming their " + InventorySlot.Item.Name);
                    PacketManager.Instance.SendRemoveInventoryItem(PlayerManager.Instance.GetCurrentPlayerName(), ItemSlot);
                    break;

                //Equipment items are equipped onto the player
                case (ItemType.Equipment):
                    //Check which slot this item belongs to
                    EquipmentSlot Slot = InventorySlot.Item.Slot;
                    string SlotName = Slot.ToString() + " Slot";
                    //Make sure this gear slot is available for use
                    EquipSlot GearSlot = PlayerEquipmentControl.Instance.GetEquipmentPanel(Slot).GetComponent<EquipSlot>();
                    bool SlotAvailable = GearSlot.SlotAvailable();
                    if(SlotAvailable)
                    {
                        //Tell the server to move this item from our inventory to our equipment screen
                        PacketManager.Instance.SendEquipItemRequest(PlayerManager.Instance.GetCurrentPlayerName(), ItemSlot, InventorySlot.Item.ID, Slot);
                        //Locally equip the item into the equipment screen
                        GearSlot.UpdateItem(InventorySlot.Item);
                        //Locally remove the item from the inventory
                        InventorySlot.SetEmpty();
                    }
                    break;

                //Weapons are equipped onto the player
                case (ItemType.Weapon):
                    l.og("Playing is trying to wield their " + InventorySlot.Item.Name);
                    break;
            }
        }
    }
}
