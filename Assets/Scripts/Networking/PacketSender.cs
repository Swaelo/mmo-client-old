using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClientPacketType
{
    Message = 1,    //client sends a message to the server <int:PacketType, string:Message>
    Register = 2,   //clients sends request to the server to register a new account <int:PacketType, string:Username, string:Password>
    Login = 3,   //client sends a request to the server to log into an account <int:PacketType, string:Username, string:Password>
    PlayerUpdate = 4,    //clients updating the server on their position and rotation information <int:PacketType, string:AccountName, vector3:position, vector4:rotation>
    Disconnect = 5,  //tell the server when we disconnect from the game
    PlayerMessage = 6   //sends the clients chat message to the server to be sent to all other clients
}

//sends packets to the game server
public class PacketSender : MonoBehaviour
{
    public static PacketSender instance;
    private ServerConnection connection;
    private void Awake() { instance = this; connection = GetComponent<ServerConnection>(); }

    //Sends a packet to the server after its been filled with data from one of the other functions in this class
    private void SendPacket(byte[] PacketData)
    {
        ByteBuffer.ByteBuffer PacketWriter = new ByteBuffer.ByteBuffer();   //start the packet writer
        PacketWriter.WriteBytes(PacketData);    //fill it with data
        //Make sure the connection to the server is still open
        if(!connection.ClientStream.CanRead)
        {
            Console.Instance.Print("couldnt send packet, connection to the server is no longer open");
            return;
        }
        //send the packet to the server
        connection.ClientStream.Write(PacketWriter.ToArray(), 0, PacketWriter.ToArray().Length);
        PacketWriter.Dispose(); //close the packet writer
    }

    //Sends a message to the server <int:PacketType, string:Message>
    public void SendServerMessage(string Message)
    {
        //Make the packet that will be sent
        ByteBuffer.ByteBuffer PacketWriter = new ByteBuffer.ByteBuffer();
        PacketWriter.WriteInteger((int)ClientPacketType.Message);
        PacketWriter.WriteString(Message);
        //Send the packet to the server
        SendPacket(PacketWriter.ToArray());
        //Close the packet writer
        PacketWriter.Dispose(); //close the packet writer
    }

    //Sends a request to the server to register a new account <int:PacketType, string:Username, string:Password>
    public void SendRegisterRequest(string username, string password)
    {
        Console.Instance.Print("Sending request to register a new account for " + username);
        ByteBuffer.ByteBuffer PacketWriter = new ByteBuffer.ByteBuffer();   //start the packet writer
        PacketWriter.WriteInteger((int)ClientPacketType.Register);  //write the packet type
        //write the account credentials
        PacketWriter.WriteString(username);
        PacketWriter.WriteString(password);
        SendPacket(PacketWriter.ToArray()); //send the packet to the server
        PacketWriter.Dispose(); //close the packet writer
    }

    //Sends a request to the server to log into an account <int:PacketType, string:Username, string:Password>
    public void SendLoginRequest(string username, string password)
    {
        Console.Instance.Print("Sending request to log into the " + username + " account");
        ByteBuffer.ByteBuffer PacketWriter = new ByteBuffer.ByteBuffer();   //start the packet writer
        PacketWriter.WriteInteger((int)ClientPacketType.Login); //write the packet type
        //write the account credentials
        PacketWriter.WriteString(username);
        PacketWriter.WriteString(password);
        SendPacket(PacketWriter.ToArray()); //send the packet to the server
        PacketWriter.Dispose(); //close the packet writer
    }

    //Sends information to the server updating them on our players current location <int:PacketType, string:AccountName, vector3:position, vector4:rotation>
    public void SendPlayerUpdate(Vector3 Position)
    {
        ByteBuffer.ByteBuffer PacketWriter = new ByteBuffer.ByteBuffer();   //start the packet writer
        PacketWriter.WriteInteger((int)ClientPacketType.PlayerUpdate);  //write the packet type
        PacketWriter.WriteString(connection.CurrentPlayer.name);
        //write the position data
        PacketWriter.WriteFloat(Position.x);
        PacketWriter.WriteFloat(Position.y);
        PacketWriter.WriteFloat(Position.z);
        //send the packet and close the writer
        SendPacket(PacketWriter.ToArray());
        PacketWriter.Dispose();
    }

    //Tells the server we are disconnecting from the game now
    public void SendDisconnectNotice()
    {
        ByteBuffer.ByteBuffer PacketWriter = new ByteBuffer.ByteBuffer();
        PacketWriter.WriteInteger((int)ClientPacketType.Disconnect);
        PacketWriter.WriteString(connection.CurrentPlayer.name);
        SendPacket(PacketWriter.ToArray());
        PacketWriter.Dispose();
    }

    public void SendChatMessage(string Message)
    {
        ByteBuffer.ByteBuffer PacketWriter = new ByteBuffer.ByteBuffer();
        PacketWriter.WriteInteger((int)ClientPacketType.PlayerMessage);
        PacketWriter.WriteString(connection.CurrentPlayer.name);
        PacketWriter.WriteString(Message);
        SendPacket(PacketWriter.ToArray());
        PacketWriter.Dispose();
    }
}