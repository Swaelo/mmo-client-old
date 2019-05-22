using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class ItemManagementPacketHandler
{
    public static void HandleSpawnItem(PacketReader Reader)
    {
        Log.PrintIncomingPacket("ItemManagement.SpawnItemPickup");
        //Read in the items information values
        int ItemNumber = Reader.ReadInt();
        int ItemID = Reader.ReadInt();
        Vector3 ItemLocation = Reader.ReadVector3();

        //Use the ItemManager class to add this object into the game world as a new pickup object
        ItemManager.Instance.AddItemPickup(ItemNumber, ItemID, ItemLocation);
    }

    public static void HandleRemoveItem(PacketReader Reader)
    {
        Log.PrintIncomingPacket("ItemManagement.RemoveItemPickup");
         //Get the universal ItemID of the item thats being removed, then use that to tell the ItemManager to remove it from the game world
         int ItemID = Reader.ReadInt();
        ItemManager.Instance.RemoveItemPickup(ItemID);
    }
}