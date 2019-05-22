using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class InventoryEquipmentManagementPacketSender
{
    public static void SendPlayerInventoryRequest()
    {
        Log.PrintOutgoingPacket("InventoryEquipmentManagement.PlayerInventoryRequest");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerInventoryRequest);
        Writer.WriteString(PlayerManager.Instance.GetCurrentPlayerName());
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendPlayerEquipmentRequest()
    {
        Log.PrintOutgoingPacket("InventoryEquipmentManagement.PlayerEquipmentRequest");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerEquipmentRequest);
        Writer.WriteString(PlayerManager.Instance.GetCurrentPlayerName());
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendPlayerActionBarRequest()
    {
        Log.PrintOutgoingPacket("InventoryEquipmentManagement.PlayerActionBarRequest");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerActionBarRequest);
        Writer.WriteString(PlayerManager.Instance.GetCurrentPlayerName());
        ConnectionManager.Instance.SendPacket(Writer);
    }
}