// ================================================================================================================================
// File:        PacketSender.cs
// Description: Whenever the client needs to communicate with the game server, it will be done with one of the functions in here
// Author:      Harley Laurie          
// Notes:       This needs to be split up into multiple seperate classes soon, this file is way too big
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClientPacketType
{
    AccountRegistrationRequest = 1,
    AccountLoginRequest = 2,
    CharacterCreationRequest = 3,
    CharacterDataRequest = 4,
    EnterWorldRequest = 5,
    ActiveEntityRequest = 6,
    NewPlayerReady = 7,
    PlayerChatMessage = 8,
    PlayerUpdate = 9,
    DisconnectionNotice = 10,
    ConnectionCheckReply = 11
}

//sends packets to the game server
public class PacketSenderLogic : MonoBehaviour
{
    public static PacketSenderLogic Instance;
    private ServerConnection connection;
    private void Awake() { Instance = this; connection = GetComponent<ServerConnection>(); }

    //Sends a packet to the server after its been filled with data from one of the other functions in this class
    private void SendPacket(byte[] PacketData)
    {
        //Make sure the connection to the server is still open
        if(!connection.IsServerConnected() || !connection.ClientStream.CanRead)
        {
            ChatWindow.Instance.DisplayErrorMessage("couldnt send packet, connection to the server is no longer open");
            return;
        }
        //send the packet to the server
        connection.ClientStream.Write(PacketData, 0, PacketData.Length);
    }

    //Sends our chat message to the server to be delivered to all the other clients
    public void SendPlayerMessage(string Message)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerChatMessage);
        Writer.WriteString(PlayerInfo.CharacterName);
        Writer.WriteString(Message);
        SendPacket(Writer.ToArray());
    }

    //Sends a request to the server to register a new account <int:PacketType, string:Username, string:Password>
    public void SendRegisterRequest(string username, string password)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.AccountRegistrationRequest);  //write the packet type
        //write the account credentials
        Writer.WriteString(username);
        Writer.WriteString(password);
        SendPacket(Writer.ToArray()); //send the packet to the server
    }

    //Sends a request to the server to log into an account <int:PacketType, string:Username, string:Password>
    public void SendLoginRequest(string username, string password)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.AccountLoginRequest); //write the packet type
        //write the account credentials
        Writer.WriteString(username);
        Writer.WriteString(password);
        SendPacket(Writer.ToArray()); //send the packet to the server
    }

    //Sends a request to the server to create a new character registered to our account
    public void SendCreateCharacterRequest(string AccountName, string CharacterName, bool IsMale)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.CharacterCreationRequest); //write the packet type
        Writer.WriteString(AccountName);
        Writer.WriteString(CharacterName);
        Writer.WriteInt(IsMale ? 1 : 0);
        SendPacket(Writer.ToArray());
    }

    //Sends a request to the server for us to be sent into the game world with the selected character
    public void SendEnterWorldRequest(CharacterData Data)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.EnterWorldRequest); //write the packet type
        Writer.WriteString(Data.Account);
        Writer.WriteString(Data.Name);
        Writer.WriteFloat(Data.Position.x);
        Writer.WriteFloat(Data.Position.y);
        Writer.WriteFloat(Data.Position.z);
        SendPacket(Writer.ToArray());
    }

    //Sends a request to get the 2nd set of information from the server, the entity list
    public void SendNewClientGetEntities()
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.ActiveEntityRequest);
        SendPacket(Writer.ToArray());
    }

    public void SendNewPlayerReady()
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.NewPlayerReady);
        SendPacket(Writer.ToArray());
    }

    //Sends a request to the server to return all the data about any characters we have created so far
    public void SendGetCharacterDataRequest(string username)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.CharacterDataRequest); //write the packet type
        Writer.WriteString(username);
        SendPacket(Writer.ToArray());
    }
    
    //Sends information to the server updating them on our players current location <int:PacketType, string:AccountName, vector3:position, vector4:rotation>
    public void SendPlayerUpdate(Vector3 Position, Quaternion Rotation)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerUpdate);  //write the packet type
        Writer.WriteString(PlayerInfo.CharacterName);
        //write the position data
        Writer.WriteVector3(Position);
        //write rotation data
        Writer.WriteQuaternion(Rotation);
        //send the packet and close the writer
        SendPacket(Writer.ToArray());
    }

    //Tells the server where our attack landed
    public void SendPlayerAttack(Vector3 Position, Vector3 Scale, Quaternion Rotation)
    {
        //l.og("sending player attack");
        //ByteBuffer.ByteBuffer PacketWriter = new ByteBuffer.ByteBuffer();   //start the packet writer
        //PacketWriter.WriteInteger((int)ClientPacketType.PlayerMeleeAttack);
        //PacketWriter.WriteFloat(Position.x);
        //PacketWriter.WriteFloat(Position.y);
        //PacketWriter.WriteFloat(Position.z);
        //PacketWriter.WriteFloat(Scale.x);
        //PacketWriter.WriteFloat(Scale.y);
        //PacketWriter.WriteFloat(Scale.z);
        //PacketWriter.WriteFloat(Rotation.x);
        //PacketWriter.WriteFloat(Rotation.y);
        //PacketWriter.WriteFloat(Rotation.z);
        //PacketWriter.WriteFloat(Rotation.w);
        //SendPacket(PacketWriter.ToArray());
        //PacketWriter.Dispose();
    }

    //Tells the server we are leaving the game now
    public void SendDisconnectNotice()
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.DisconnectionNotice);  //write the packet type
        //send and close the packet
        SendPacket(Writer.ToArray());
    }

    //Tells the server we are logging out of this user account
    public void SendAccountLogoutNotice()
    {
        //l.og("sending account logout notice");
        //Console.Instance.Print("telling the server our players updated position information");
        //ByteBuffer.ByteBuffer PacketWriter = new ByteBuffer.ByteBuffer();   //start the packet writer
        //PacketWriter.WriteInteger((int)ClientPacketType.AccountLogoutNotice);  //write the packet type
        //send and close the packet
        //SendPacket(PacketWriter.ToArray());
        //PacketWriter.Dispose();
    }

    //Replies to the servers connection check request
    public void SendConnectionCheckReply()
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.ConnectionCheckReply);  //write the packet type
        //send and close the packet
        SendPacket(Writer.ToArray());
    }
}