// ================================================================================================================================
// File:        ConnectionManager.cs
// Description: Manages the clients network connection to the master game server and all networking communication that happens with it
// ================================================================================================================================

using System;
using System.Net.Sockets;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    //Singleton instance class, only needs to be one, and this makes it easy to access from anywhere in the code
    public static ConnectionManager Instance = null;
    private void Awake() { Instance = this; }

    //Server connection settings
    public string ServerIP = "192.168.1.102";
    public int ServerPort = 5500;

    //Current server connection and datastream used to communicate with it
    private TcpClient ServerConnection = null;
    public NetworkStream DataStream = null;

    //Buffers to store data as its recieved from the server
    private byte[] ASyncBuffer = null;
    private byte[] PacketBuffer = null;

    //Current connection status
    public bool TryingToConnect = false;
    public bool IsConnected = false;
    public bool ConnectionAnnounced = false;
    public bool ShouldHandleData = false;

    //Attempts to establish a new connection with the game server
    public void TryConnect(string IPAddress)
    {
        Log.PrintChatMessage("connecting to server...");
        TryingToConnect = true;
        ServerConnection = new TcpClient();
        ServerConnection.ReceiveBufferSize = 4096;
        ServerConnection.SendBufferSize = 4096;
        ServerConnection.NoDelay = false;
        Array.Resize(ref ASyncBuffer, 8192);
        ServerConnection.BeginConnect(IPAddress, ServerPort, new AsyncCallback(ConnectionResult), ServerConnection);
    }

    public void Update()
    {
        if(TryingToConnect && IsConnected)
        {
            //Announce that we have connected to the server when it first happens
            MenuPanelDisplayManager.Instance.DisplayPanel("Main Menu Panel");
            Log.PrintChatMessage("connected!");
            TryingToConnect = false;
        }

        if(IsConnected && ShouldHandleData)
        {
            //Process packet data when its received from the server
            ShouldHandleData = false;
            PacketHandler.Instance.HandleServerPacket(PacketBuffer);
        }
    }

    //Callback triggered when we first establish a connection to the game server
    private void ConnectionResult(IAsyncResult result)
    {
        if(ServerConnection != null)
        {
            //End the connection request as we are now connected
            ServerConnection.EndConnect(result);

            //double check everything connected all right
            if(!ServerConnection.Connected)
            {
                IsConnected = false;
                return;
            }

            //note now that we have connected successfully
            IsConnected = true;
            ServerConnection.NoDelay = true;

            //open up the data stream and start listening for incoming packets sent to us from the game server
            DataStream = ServerConnection.GetStream();
            DataStream.BeginRead(ASyncBuffer, 0, 8192, ReadPacket, null);
        }
    }

    //Callback triggered when we finish recieving a new packet of information from the game server
    private void ReadPacket(IAsyncResult result)
    {
        if(ServerConnection == null)
        {
            //double check the server connection is still up and running
            Log.PrintChatMessage("socket connection lost, cant read packet");
            return;
        }

        //Copy the packet data over into a new array so the asyncbuffer can immediately be used to stream in data from the server again
        int PacketSize = DataStream.EndRead(result);
        PacketBuffer = null;
        Array.Resize(ref PacketBuffer, PacketSize);
        Buffer.BlockCopy(ASyncBuffer, 0, PacketBuffer, 0, PacketSize);
        
        if(PacketSize == 0)
        {
            //if the connection was shut down from the servers end then the sudden end in the datastream will result in recieving an empty network packet
            CloseConnection("recieve packet size 0");
            return;
        }

        //note that we now have a packet of data that needs to be handled, and start listening in for new data from the server
        ShouldHandleData = true;
        DataStream.BeginRead(ASyncBuffer, 0, 8192, ReadPacket, null);
    }

    //Severs our current connection to the game server
    public void CloseConnection(string reason)
    {
        Log.PrintChatMessage("closing server connection: " + reason);
        ServerConnection.Close();
    }

    //Sends a packet of data to the game server
    public void SendPacket(PacketWriter Writer)
    {
        DataStream.Write(Writer.ToArray(), 0, Writer.ToArray().Length);
    }
}