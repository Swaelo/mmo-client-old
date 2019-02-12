using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;

public class ServerConnection : MonoBehaviour
{
    public static ServerConnection Instance;
    private void Awake() { Instance = this; }

    private TcpClient ClientSocket = null;  //This clients connection to the game server
    public NetworkStream ClientStream = null;  //Stream of data between server and client
    private bool IsConnected = false;   //Is this client connected to the server right now
    public bool IsServerConnected() { return IsConnected; }
    private byte[] ASyncBuffer; //asynchronous packet buffer, streams in data from the server over time
    private byte[] PacketBuffer;    //data from the asynchronous buffer is copied into here once the stream of data has been completely sent through the socket
    private bool ShouldHandleData = false;  //Are we listening in for packets from the server right now

    private Dictionary<string, GameObject> OtherPlayers = new Dictionary<string, GameObject>(); //Keep a list of the other players currently playing the game

    private float ConnectionTimeout = -1; //How long we will continue waiting until we consider this server connection attempt a failure
    private bool TryingToConnect = false;

    public void AddOtherPlayer(string PlayerName, GameObject PlayerObject)
    {
        ChatWindow.Instance.DisplaySystemMessage("Add " + PlayerName + " to other players");
        OtherPlayers.Add(PlayerName, PlayerObject);
    }

    public void RemoveOtherPlayer(string PlayerName)
    {
        ChatWindow.Instance.DisplaySystemMessage("Remove " + PlayerName + " from other players");
        GameObject OtherPlayer = OtherPlayers[PlayerName];
        OtherPlayers.Remove(PlayerName);
        GameObject.Destroy(OtherPlayer);
    }

    public GameObject GetOtherPlayer(string PlayerName)
    {
        ChatWindow.Instance.DisplaySystemMessage("Get " + PlayerName + " from other players");
        return OtherPlayers[PlayerName];
    }

    //Attempts to establish a connection with the game server
    public void TryConnect()
    {
        //Set up the tcp client
        ClientSocket = new TcpClient();
        ClientSocket.ReceiveBufferSize = 4096;
        ClientSocket.SendBufferSize = 4096;
        ClientSocket.NoDelay = false;
        //Set our buffers where packet data will be streamed to
        Array.Resize(ref ASyncBuffer, 8192);
        //Try to connect to the server
        ClientSocket.BeginConnect("172.193.56.42", 5500, new AsyncCallback(ConnectionResult), ClientSocket);
    }

    //Severs the connection to the game server
    public void CloseConnection()
    {
        ClientSocket.Close();
    }

    private void Update()
    {
        //Start trying to connect to the server if we havnt yet
        if(!IsConnected && !TryingToConnect)
        {
            TryingToConnect = true;
            ChatWindow.Instance.DisplaySystemMessage("Connecting to game server...");
            ConnectionTimeout = 3;
            TryConnect();
        }

        //If we are trying to establish a connection, check if it has been successful
        if(TryingToConnect && IsServerConnected())
        {
            ChatWindow.Instance.DisplaySystemMessage("Connected!");
            MenuStateManager.GetCurrentMenuStateObject().GetComponent<MenuComponentObjects>().ToggleAllBut("Waiting Animation", false);
            TryingToConnect = false;
            IsConnected = true;
        }

        //If we are still waiting, trying to connect, count down the timeout limit
        if(TryingToConnect && !IsConnected)
        {
            ConnectionTimeout -= Time.deltaTime;
            if(ConnectionTimeout <= 0.0f)
            {
                //Try again if the timer has runout
                ChatWindow.Instance.DisplayErrorMessage("Server Connection timed out, trying again...");
                ConnectionTimeout = 3;
                TryConnect();
            }
        }

        //Recieve packets from the server
        if(IsConnected && ShouldHandleData)
        {
            //Pass the packet onto the handler class
            PacketReader.instance.HandlePacket(PacketBuffer);
            //Start waiting for the next packet from the server
            ShouldHandleData = false;
        }
    }

    //Callback trigger providing information about an attempted server connection attempt
    private void ConnectionResult(IAsyncResult Result)
    {
        if(ClientSocket != null)
        {
            //Once the connection to the server has been made
            ClientSocket.EndConnect(Result);
            //If the connection completely failed then just close the socket
            if (!ClientSocket.Connected)
            {
                IsConnected = false;
                return;
            }
            //Not that we have not established a connection with the game server
            IsConnected = true;
            ClientSocket.NoDelay = true;
            //Start listening to packets from the server
            ClientStream = ClientSocket.GetStream();
            ClientStream.BeginRead(ASyncBuffer, 0, 8192, ReadPacket, null);
        }
    }

    ////Reads packets sent from the server
    void ReadPacket(IAsyncResult Result)
    {
        //make sure connection is still open
        if (ClientSocket == null)
            return;

        //Read in the data that was sent from the server
        int PacketSize = ClientStream.EndRead(Result);
        //Reinitialize the packet buffer to match the size of the packet that was sent
        PacketBuffer = null;
        Array.Resize(ref PacketBuffer, PacketSize);
        //Copy the packet data into the buffer
        Buffer.BlockCopy(ASyncBuffer, 0, PacketBuffer, 0, PacketSize);

        //If the packet size is 0 the connection to the server has probably been lost
        if (PacketSize == 0)
        {
            ChatWindow.Instance.DisplayErrorMessage("Connection to the server was lost");
            ClientSocket.Close();
            return;
        }

        //Start listening for new packets again
        ShouldHandleData = true;
        ClientStream.BeginRead(ASyncBuffer, 0, 8192, ReadPacket, null);
    }
}