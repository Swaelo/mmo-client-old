// ================================================================================================================================
// File:        PacketHandler.cs
// Description: Instructions are sent to us from the server in groups, all bunched together in a single network packet
//              This class aims to break apart the network packet and process each section accordingly
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketHandler : MonoBehaviour
{
    public static PacketHandler Instance = null;
    private void Awake()
    {
        Instance = this;
        RegisterPacketHandlers();
    }

    public void HandleServerPacket(byte[] PacketData)
    {
        //We need to essentially keep reading information from the packet until there is nothing left, at the start of each section of data will be the packet type identifier so you know what
        //comes next and how it should be handled

        //Open up the network packet
        PacketReader Reader = new PacketReader(PacketData);

        //Display how much information there is to be read from this packet
        //Log.Print(Reader.BytesLeft() + " bytes of data left to read from the packet");

        while (!Reader.FinishedReading())
        {
            int PacketType = Reader.ReadInt();
            if (PacketHandlers.TryGetValue(PacketType, out PacketHandle PacketHandle))
                PacketHandle.Invoke(Reader);
        }
    }

    //Have a function for handling each different section of data that can appear in a network packet
    public delegate void PacketHandle(PacketReader Reader);
    public Dictionary<int, PacketHandle> PacketHandlers = new Dictionary<int, PacketHandle>();
    private void RegisterPacketHandlers()
    {
        PacketHandlers.Add((int)ServerPacketType.AccountRegistrationReply, AccountManagementPacketHandler.HandleAccountRegistrationReply);
        PacketHandlers.Add((int)ServerPacketType.AccountLoginReply, AccountManagementPacketHandler.HandleLoginReply);
        PacketHandlers.Add((int)ServerPacketType.CharacterCreationReply, AccountManagementPacketHandler.HandleCreateCharacterReply);
        PacketHandlers.Add((int)ServerPacketType.CharacterDataReply, AccountManagementPacketHandler.HandleCharacterData);

        PacketHandlers.Add((int)ServerPacketType.ActivePlayerList, GameWorldStatePacketHandler.HandleActivePlayerList);
        PacketHandlers.Add((int)ServerPacketType.ActiveEntityList, GameWorldStatePacketHandler.HandleActiveEntityList);
        PacketHandlers.Add((int)ServerPacketType.ActiveItemList, GameWorldStatePacketHandler.HandleActiveItemList);

        PacketHandlers.Add((int)ServerPacketType.SpawnItem, ItemManagementPacketHandler.HandleSpawnItem);
        PacketHandlers.Add((int)ServerPacketType.RemoveItem, ItemManagementPacketHandler.HandleRemoveItem);

        PacketHandlers.Add((int)ServerPacketType.EntityUpdates, EntityManagementPacketHandler.HandleEntityUpdates);
        PacketHandlers.Add((int)ServerPacketType.RemoveEntities, EntityManagementPacketHandler.HandleRemoveEntities);

        PacketHandlers.Add((int)ServerPacketType.PlayerChatMessage, PlayerCommunicationPacketHandler.HandlePlayerMessage);

        PacketHandlers.Add((int)ServerPacketType.PlayerUpdate, PlayerManagementPacketHandler.HandlePlayerUpdate);
        PacketHandlers.Add((int)ServerPacketType.SpawnPlayer, PlayerManagementPacketHandler.HandleSpawnPlayer);
        PacketHandlers.Add((int)ServerPacketType.RemovePlayer, PlayerManagementPacketHandler.HandleRemovePlayer);

        PacketHandlers.Add((int)ServerPacketType.PlayerInventoryItems, InventoryEquipmentManagementPacketHandler.HandlePlayerInventory);
        PacketHandlers.Add((int)ServerPacketType.PlayerEquipmentItems, InventoryEquipmentManagementPacketHandler.HandlePlayerEquipment);
        PacketHandlers.Add((int)ServerPacketType.PlayerActionBarAbilities, InventoryEquipmentManagementPacketHandler.HandlePlayerActionBar);
        PacketHandlers.Add((int)ServerPacketType.PlayerTotalItemUpdate, InventoryEquipmentManagementPacketHandler.HandlePlayerTotalItemUpdate);
    }
}
