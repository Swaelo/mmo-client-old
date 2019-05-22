using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class GameWorldStatePacketSender
{
    public static void SendEnterWorldRequest(CharacterData CharacterData)
    {
        Log.PrintOutgoingPacket("GameWorldState.EnterWorldRequest");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.EnterWorldRequest);
        Writer.WriteString(CharacterData.AccountOwner);
        Writer.WriteString(CharacterData.CharacterName);
        Writer.WriteVector3(CharacterData.CharacterPosition);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendActiveEntityRequest()
    {
        Log.PrintOutgoingPacket("GameWorldState.ActiveEntityRequest");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.ActiveEntityRequest);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendActiveItemRequest()
    {
        Log.PrintOutgoingPacket("GameWorldState.ActiveItemRequest");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.ActiveItemRequest);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendNewPlayerReady()
    {
        Log.PrintOutgoingPacket("GameWorldState.NewPlayerReady");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.NewPlayerReady);
        ConnectionManager.Instance.SendPacket(Writer);
    }
}