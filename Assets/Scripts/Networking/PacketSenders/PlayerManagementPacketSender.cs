using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class PlayerManagementPacketSender
{
    public static void SendPlayerUpdate(Vector3 Position, Quaternion Rotation)
    {
        //Log.PrintOutgoingPacket("PlayerManagement.SendPlayerUpdate");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerUpdate);
        Writer.WriteString(PlayerManager.Instance.LocalPlayer.CurrentCharacter.CharacterName);
        Writer.WriteVector3(Position);
        Writer.WriteQuaternion(Rotation);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendPlayerAttack(Vector3 Position, Vector3 Scale, Quaternion Rotation)
    {
        Log.PrintOutgoingPacket("PlayerManagement.SendPlayerAttack");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerAttack);
        Writer.WriteVector3(Position);
        Writer.WriteVector3(Scale);
        Writer.WriteQuaternion(Rotation);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendDisconnectNotice()
    {
        Log.PrintOutgoingPacket("PlayerManagement.SendDisconnectionNotice");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.DisconnectionNotice);
        ConnectionManager.Instance.SendPacket(Writer);
    }
}