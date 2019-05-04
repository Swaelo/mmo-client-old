// ================================================================================================================================
// File:        ConnectionManager.cs
// Description: Manages the clients network connection to the master game server and all networking communication that happens with it
// ================================================================================================================================

using System;
using System.Net.Sockets;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    //Singleton Instance
    public static ConnectionManager Instance = null;
    private void Awake() { Instance = this; }

    //Server Connection Configuration
    public string ServerIP = "192.168.0.8";
    public int ServerPort = 5500;

    //Active Connection Objects
    private TcpClient ClientSocket = null;
    public NetworkStream ClientStream = null;

    //Packet Data Buffer Storage
    private byte[] ASyncBuffer;
    private byte[] PacketBuffer;
    private bool ShouldHandleData = false;

    //Current Connection Status
    public bool IsConnected = false;
    private float ConnectionTimeout = 3f;
    private bool TryingToConnect = false;

    //Attempts to establish a connection to the game server, automatically retries every 3 seconds
    public void TryConnect()
    {
        l.og("Connecting to server...");
        TryingToConnect = true;
        ClientSocket = new TcpClient();
        ClientSocket.ReceiveBufferSize = 4096;
        ClientSocket.SendBufferSize = 4096;
        ClientSocket.NoDelay = false;
        Array.Resize(ref ASyncBuffer, 8192);
        ClientSocket.BeginConnect(ServerIP, ServerPort, new AsyncCallback(ConnectionResult), ClientSocket);
        MenuPanelDisplayManager.Instance.DisplayPanel("Waiting Panel");
        MenuPanelDisplayManager.Instance.DisplayGameTitle();
    }

    public void Update()
    {
        //Start automatically trying to connect to the server if absolutely nothing is happening
        if (!TryingToConnect && !IsConnected)
            TryConnect();

        //If we are still waiting for establish a connection to the server, check if its been done yet or not
        if (TryingToConnect && IsConnected)
        {
            //Enable the main menu display once the server has been connected to
            MenuPanelDisplayManager.Instance.DisplayPanel("Main Menu Panel");
            l.og("Connected to Server.");
            TryingToConnect = false;
        }

        //Wait for connection timeout if the connection hasnt been made yet
        if (TryingToConnect && !IsConnected)
        {
            ConnectionTimeout -= Time.deltaTime;
            if (ConnectionTimeout <= 0f)
            {
                //When the timer expires reset it and try connecting to the server again
                l.og("Server connection timed out, trying again.");
                ConnectionTimeout = 3f;
                TryConnect();
            }
        }

        //Receive data from the server while the connection is active and send data to the PacketManager to be handled accordingly
        if (IsConnected && ShouldHandleData)
        {
            PacketManager.Instance.HandlePacket(PacketBuffer);
            ShouldHandleData = false;
        }
    }
    
    //Callback Event triggered when trying to establish a new connection to the server
    private void ConnectionResult(IAsyncResult Result)
    {
        //If the socket object doesnt exist then something is seriously wrong
        if (ClientSocket != null)
        {
            //End the ongoing connection request now that we have connected to the game server
            ClientSocket.EndConnect(Result);

            //Double check the connection hasnt been dropped yet for some reason
            if (!ClientSocket.Connected)
            {
                IsConnected = false;
                return;
            }

            //Confirm the connection has been established
            IsConnected = true;
            ClientSocket.NoDelay = true;

            //Start handling packets sent to us from the server
            ClientStream = ClientSocket.GetStream();
            ClientStream.BeginRead(ASyncBuffer, 0, 8192, ReadPacket, null);
        }
    }

    //Reads packet data sent to us from the server, then sends it onto the PacketHandler class to be processed
    private void ReadPacket(IAsyncResult Result)
    {
        //Do nothing if no connection at all exists
        if (ClientSocket == null)
        {
            l.og("Ignoring ReadPacket function call, the socket connection does not exist.");
            return;
        }

        //Copy the data from the ASyncBuffer over to the PacketBuffer object
        int PacketSize = ClientStream.EndRead(Result);
        PacketBuffer = null;
        Array.Resize(ref PacketBuffer, PacketSize);
        Buffer.BlockCopy(ASyncBuffer, 0, PacketBuffer, 0, PacketSize);

        //Make sure no packets are received with a size of 0, this usually means the connection to the server has been lost for some reason
        if (PacketSize == 0)
        {
            CloseConnection("Received network packet of size 0");
            return;
        }

        //Start listening to data sent to us from the game server again
        ShouldHandleData = true;
        ClientStream.BeginRead(ASyncBuffer, 0, 8192, ReadPacket, null);
    }

    //Closes the current connection to the game server
    public void CloseConnection(string Reason)
    {
        l.og("Closing Server Connection, " + Reason);
        ClientSocket.Close();
    }
}
