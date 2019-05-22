using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class EntityManagementPacketHandler
{
    public static void HandleEntityUpdates(PacketReader Reader)
    {
        Log.PrintIncomingPacket("EntityManagement.HandleEntityUpdates");
        int EntityCount = Reader.ReadInt();
        for (int i = 0; i < EntityCount; i++)
        {
            //Extract the information for each entity as we loop through them all
            string EntityID = Reader.ReadString();
            Vector3 EntityPosition = Reader.ReadVector3();
            Quaternion EntityRotation = Reader.ReadQuaternion();
            int EntityHealth = Reader.ReadInt();
            //Send each entities updated values to the entity manager to be handled
            EntityManager.UpdateEntity(EntityID, EntityPosition, EntityRotation, EntityHealth);
        }
    }

    public static void HandleRemoveEntities(PacketReader Reader)
    {
        Log.PrintIncomingPacket("EntityManagement.RemoveEntities");
        int EntityCount = Reader.ReadInt();
        //Loop through and remove them all
        for (int i = 0; i < EntityCount; i++)
        {
            string EntityID = Reader.ReadString();
            EntityManager.RemoveEntity(EntityID);
        }
    }
}