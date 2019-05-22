using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class AccountManagementPacketSender
{
    public static void SendRegisterRequest(string Username, string Password)
    {
        Log.PrintOutgoingPacket("AccountManagement.AccountRegistrationRequest");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.AccountRegistrationRequest);
        Writer.WriteString(Username);
        Writer.WriteString(Password);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendLoginRequest(string Username, string Password)
    {
        Log.PrintOutgoingPacket("AccountManagement.AccountLoginRequest");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.AccountLoginRequest);
        Writer.WriteString(Username);
        Writer.WriteString(Password);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendCreateCharacterRequest(string AccountName, string CharacterName, bool IsMale)
    {
        Log.PrintOutgoingPacket("AccountManagement.CharacterCreationRequest");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.CharacterCreationRequest);
        Writer.WriteString(AccountName);
        Writer.WriteString(CharacterName);
        Writer.WriteInt(IsMale ? 1 : 0);
        ConnectionManager.Instance.SendPacket(Writer);
    }

    public static void SendCharacterDataRequest(string Username)
    {
        Log.PrintOutgoingPacket("AccountManagement.CharacterDataRequest");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.CharacterDataRequest);
        Writer.WriteString(Username);
        ConnectionManager.Instance.SendPacket(Writer);
    }
}
