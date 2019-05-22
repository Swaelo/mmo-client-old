using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class PlayerCommunicationPacketSender
{
    public static void SendPlayerMessage(string Message)
    {
        Log.PrintOutgoingPacket("PlayerCommunication.SendPlayerMessage");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerChatMessage);
        Writer.WriteString(PlayerManager.Instance.LocalPlayer.CurrentCharacter.CharacterName);
        Writer.WriteString(Message);
        ConnectionManager.Instance.SendPacket(Writer);
    }
}