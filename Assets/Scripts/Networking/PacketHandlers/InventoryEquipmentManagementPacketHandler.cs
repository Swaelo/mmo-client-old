using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class InventoryEquipmentManagementPacketHandler
{
    public static void HandlePlayerInventory(PacketReader Reader)
    {
        Log.PrintIncomingPacket("InventoryEquipmentManagement.HandleCharacterInventory");
        //Grab all the inventory slot UI components from the scene
        DraggableUIComponent[] ItemSlots = DraggableUIControl.Instance.InventorySlots;
        //Loop through reading the data for each item in the characters inventory, updating the item slots where nessacery
        for (int i = 0; i < 9; i++)
        {
            //Read each slots values
            int ItemNumber = Reader.ReadInt();
            int ItemID = Reader.ReadInt();

            //Empty the slots with nothing in them
            if (ItemNumber == 0)
            {
                ItemSlots[i].ItemData = null;
                ItemSlots[i].UpdateUIDisplay();
            }
            //Display the slots that arent empty
            else
            {
                ItemSlots[i].ItemData = ItemList.Instance.GetItemData(ItemNumber);
                ItemSlots[i].EquipSlotType = ItemSlots[i].ItemData.Slot;
                ItemSlots[i].ItemID = ItemID;
                ItemSlots[i].UpdateUIDisplay();
            }
        }

        //Inventory updated, now request the characters equipment information
        //InventoryEquipmentManagementPacketSender.SendPlayerEquipmentRequest();
    }

    public static void HandlePlayerEquipment(PacketReader Reader)
    {
        Log.PrintIncomingPacket("InventoryEquipmentManagement.HandleCharacterEquipment");
        //Loop through and read the info about any items in the players equipment slots
        for (int i = 0; i < 13; i++)
        {
            //Read in each items equipment slot, item number and ID
            EquipmentSlot ItemSlot = (EquipmentSlot)Reader.ReadInt();
            int ItemNumber = Reader.ReadInt();
            int ItemID = Reader.ReadInt();

            //Skip past the empty equipment slots
            if (ItemNumber == 0)
                continue;

            //Fetch the items information from the master item list
            ItemData ItemData = ItemList.Instance.GetItemData(ItemNumber);

            //Update the UI and character model to show the piece of gear being equipped
            DraggableUIComponent GearUISlot = DraggableUIControl.Instance.GetEquipmentSlot(ItemSlot);
            GearUISlot.ItemData = ItemData;
            GearUISlot.ItemID = ItemID;
            GearUISlot.UpdateUIDisplay();
            PlayerManager.Instance.GetCurrentCharacterObject().GetComponent<PlayerItemEquip>().EquipItem(ItemSlot, ItemData.Name);
        }

        //Characters equipment has been handled, now ask for the action bar status
        //InventoryEquipmentManagementPacketSender.SendPlayerActionBarRequest();
    }

    public static void HandlePlayerActionBar(PacketReader Reader)
    {
        Log.PrintIncomingPacket("InventoryEquipmentManagement.HandleCharacterActionBar");
        //Grab all the action bar slot UI components from the scene
        DraggableUIComponent[] ActionBarSlots = DraggableUIControl.Instance.ActionBarSlots;
        //Loop through reading in each action bar items data, updating the slots as needed
        for (int i = 0; i < 5; i++)
        {
            //Read each slots values
            int ItemNumber = Reader.ReadInt();
            int ItemID = Reader.ReadInt();

            //Empty the action bar slots with nothing within them
            if (ItemNumber == 0)
            {
                ActionBarSlots[i].ItemData = null;
                ActionBarSlots[i].UpdateUIDisplay();
            }
            else
            //Display action bar slots with abilities within them
            {
                ActionBarSlots[i].ItemData = ItemList.Instance.GetItemData(ItemNumber);
                ActionBarSlots[i].ItemID = ItemID;
                ActionBarSlots[i].UpdateUIDisplay();
            }
        }

        //Now everything is ready and set up correctly, tell the server we have finished loading into the game successfully
        GameWorldStatePacketSender.SendNewPlayerReady();
    }

    public static void HandlePlayerTotalItemUpdate(PacketReader Reader)
    {
        Log.PrintIncomingPacket("InventoryEquipmentManagement.HandleCharacterEverything");
        //Read and update the contents of the characters inventory
        DraggableUIComponent[] ItemSlots = DraggableUIControl.Instance.InventorySlots;
        for (int i = 0; i < 9; i++)
        {
            //Read the values of the item in each inventory slot
            int ItemNumber = Reader.ReadInt();
            int ItemID = Reader.ReadInt();

            //Empty the slots with nothing in them
            if (ItemNumber == 0)
            {
                ItemSlots[i].ItemData = null;
                ItemSlots[i].UpdateUIDisplay();
            }
            else
            //Display the slots with items in them
            {
                ItemSlots[i].ItemData = ItemList.Instance.GetItemData(ItemNumber);
                ItemSlots[i].EquipSlotType = ItemSlots[i].ItemData.Slot;
                ItemSlots[i].ItemID = ItemID;
                ItemSlots[i].UpdateUIDisplay();
            }
        }

        //Loop through the characters equipment slots, updating the UI / character model as we go along
        for (int i = 0; i < 13; i++)
        {
            //Read the initial item values from the packet data
            EquipmentSlot GearSlot = (EquipmentSlot)Reader.ReadInt();
            int ItemNumber = Reader.ReadInt();
            int ItemID = Reader.ReadInt();

            //Empty UI slots / remove equipment from the player with empty equipment slots
            if (ItemNumber == 0)
            {
                //Update the UI
                DraggableUIComponent EquipmentSlot = DraggableUIControl.Instance.GetEquipmentSlot(GearSlot);
                EquipmentSlot.ItemData = null;
                EquipmentSlot.UpdateUIDisplay();
                //Remove gear from the character
                PlayerManager.Instance.GetCurrentCharacterObject().GetComponent<PlayerItemEquip>().UnequipItem(GearSlot);
            }
            //Update UI slots / add equipment to the player with filled equipment slots
            else
            {
                //Get the rest of the items data
                ItemData ItemData = ItemList.Instance.GetItemData(ItemNumber);
                //Update the UI
                DraggableUIComponent EquipmentSlot = DraggableUIControl.Instance.GetEquipmentSlot(GearSlot);
                EquipmentSlot.ItemData = ItemData;
                EquipmentSlot.ItemID = ItemID;
                EquipmentSlot.UpdateUIDisplay();
                //Equip the gear onto the character model
                PlayerManager.Instance.GetCurrentCharacterObject().GetComponent<PlayerItemEquip>().EquipItem(GearSlot, ItemData.Name);
            }
        }

        //Read and update the contents of the characters action bar
        DraggableUIComponent[] AbilitySlots = DraggableUIControl.Instance.ActionBarSlots;
        for (int i = 0; i < 5; i++)
        {
            //read each items values
            int ItemNumber = Reader.ReadInt();
            int ItemID = Reader.ReadInt();

            //Empty slots
            if (ItemNumber == 0)
            {
                AbilitySlots[i].ItemData = null;
                AbilitySlots[i].UpdateUIDisplay();
            }
            //Filled Slots
            else
            {
                AbilitySlots[i].ItemData = ItemList.Instance.GetItemData(ItemNumber);
                AbilitySlots[i].ItemID = ItemID;
                AbilitySlots[i].UpdateUIDisplay();
            }
        }
    }
}