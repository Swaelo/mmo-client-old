using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class ItemManagementPacketSender
{
    public static void SendTakeItemRequest(string CharacterName, ItemData ItemData, int ItemID)
    {
        Log.PrintOutgoingPacket("ItemManagement.TakeItemRequest");
        //Create the new packet to send to the server
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerTakeItemRequest);

        //Write the characters name and the items data into the network packet
        Writer.WriteString(CharacterName);
        Writer.WriteInt(ItemData.ItemNumber);
        Writer.WriteInt(ItemID);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendRemoveInventoryItem(string PlayerName, int BagSlot)
    {
        Log.PrintOutgoingPacket("ItemManagement.RemoveInventoryItem");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.RemoveInventoryItem);
        Writer.WriteString(PlayerName);
        Writer.WriteInt(BagSlot);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendEquipItemRequest(string CharacterName, int BagSlot, EquipmentSlot GearSlot)
    {
        Log.PrintOutgoingPacket("ItemManagement.EquipItemRequest");
        //Define the new packet to send
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.EquipInventoryItem);

        //Write the characters name, the bag slot number where the item is and the equipment slot type where they are going to equip the item to
        Writer.WriteString(CharacterName);
        Writer.WriteInt(BagSlot);
        Writer.WriteInt((int)GearSlot);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendUnequipItemRequest(string CharacterName, int BagSlot, EquipmentSlot GearSlot)
    {
        Log.PrintOutgoingPacket("ItemManagement.UnequipItemRequest");
        //Start the packet writer
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.UnequipItem);

        //Write in who is unequipping the item, what slot they are unequipping from, and what inventory slot the item wants to be placed into (default -1 value places the item into the first available inventory slot)
        Writer.WriteString(CharacterName);
        Writer.WriteInt((int)GearSlot);
        Writer.WriteInt(BagSlot);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendMoveInventoryItem(string CharacterName, int OriginalBagSlot, int DestinationBagSlot)
    {
        Log.PrintOutgoingPacket("ItemManagement.MoveInventoryItem");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerMoveInventoryItem);
        Writer.WriteString(CharacterName);
        Writer.WriteInt(OriginalBagSlot);
        Writer.WriteInt(DestinationBagSlot);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendSwapInventoryItem(string CharacterName, int FirstBagSlot, int SecondBagSlot)
    {
        Log.PrintOutgoingPacket("ItemManagement.SwapInventoryItems");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerSwapInventoryItems);
        Writer.WriteString(CharacterName);
        Writer.WriteInt(FirstBagSlot);
        Writer.WriteInt(SecondBagSlot);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendSwapEquipmentItem(string CharacterName, int BagSlot, EquipmentSlot GearSlot)
    {
        Log.PrintOutgoingPacket("ItemManagement.SwapEquipmentItems");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerSwapEquipmentItem); //Packet type
        Writer.WriteString(CharacterName); //Who is swapping items around their inventory/equipment
        Writer.WriteInt(BagSlot);   //What bag slot stores the item to newly be equipped, where the previously equipped item will now be stored
        Writer.WriteInt((int)GearSlot);    //The type of equipment slot being dealt with
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendDropInventoryItem(string CharacterName, int BagSlot, Vector3 DropLocation)
    {
        Log.PrintOutgoingPacket("ItemManagement.DropInventoryItem");
        //Create packet and write the packet type
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerDropItem);
        //Write in the location where the item is being dropped from
        Writer.WriteInt(1); //1 = dropped from inventory
                            //Now write the players name and the bag slot number of the item that is being dropped, and where they are dropping it
        Writer.WriteString(CharacterName);
        Writer.WriteInt(BagSlot);
        Writer.WriteVector3(DropLocation);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendDropEquipmentItem(string CharacterName, EquipmentSlot GearSlot, Vector3 DropLocation)
    {
        Log.PrintOutgoingPacket("ItemManagement.DropEquipmentItem");
        //Create packet and write the packet type
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerDropItem);
        //Write in the location where the item is being dropped from
        Writer.WriteInt(2); //2 = dropped from equipment
                            //write the rest of the data and send the packet to the server
        Writer.WriteString(CharacterName);
        Writer.WriteInt((int)GearSlot);
        Writer.WriteVector3(DropLocation);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendEquipAbility(string CharacterName, int BagSlot, int BarSlot)
    {
        Log.PrintOutgoingPacket("ItemManagement.EquipAbilityGem");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerEquipAbility);
        Writer.WriteString(CharacterName);
        Writer.WriteInt(BagSlot);
        Writer.WriteInt(BarSlot);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendSwapEquipAbility(string CharacterName, int BagSlot, int BarSlot)
    {
        Log.PrintOutgoingPacket("ItemManagement.SwapEquipAbilityGem");
        //Define a new packet to send
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerSwapEquipAbility);

        //Fill it with the right data
        Writer.WriteString(CharacterName);
        Writer.WriteInt(BagSlot);
        Writer.WriteInt(BarSlot);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendUnequipAbility(string CharacterName, int BagSlot, int BarSlot)
    {
        Log.PrintOutgoingPacket("ItemManagement.UnequipAbilityGem");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerUnequipAbility);
        Writer.WriteString(CharacterName);
        Writer.WriteInt(BagSlot);
        Writer.WriteInt(BarSlot);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendSwapAbilities(string CharacterName, int FirstBarSlot, int SecondBarSlot)
    {
        Log.PrintOutgoingPacket("ItemManagement.SwapAbilityGems");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerSwapAbilities);
        Writer.WriteString(CharacterName);
        Writer.WriteInt(FirstBarSlot);
        Writer.WriteInt(SecondBarSlot);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendMoveAbility(string CharacterName, int BarSlot, int DestinationBarSlot)
    {
        Log.PrintOutgoingPacket("ItemManagement.MoveAbilityGem");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerMoveAbility);

        Writer.WriteString(CharacterName);
        Writer.WriteInt(BarSlot);
        Writer.WriteInt(DestinationBarSlot);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendDropAbility(string CharacterName, int BarSlot, Vector3 DropLocation)
    {
        Log.PrintOutgoingPacket("ItemManagement.DropAbilityGem");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerDropAbility);

        Writer.WriteString(CharacterName);
        Writer.WriteInt(BarSlot);
        Writer.WriteVector3(DropLocation);
        ConnectionManager.Instance.SendPacket(Writer);
    }
}