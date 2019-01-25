using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;

public class ServerConnection : MonoBehaviour
{
    private TcpClient ClientSocket = null;  //This clients connection to the game server
    public NetworkStream ClientStream = null;  //Stream of data between server and client
    private bool IsConnected = false;   //Is this client connected to the server right now
    private byte[] ASyncBuffer; //asynchronous packet buffer, streams in data from the server over time
    private byte[] PacketBuffer;    //data from the asynchronous buffer is copied into here once the stream of data has been completely sent through the socket
    private bool ShouldHandleData = false;  //Are we listening in for packets from the server right now
    private string ConsoleMessage = ""; //Update constantly checks the value of this string, when its not empty the value is displayed to the console log then reset, this is so SocketThread can set this value
    
    public GameObject CurrentPlayer = null;
    public Dictionary<string, GameObject> OtherPlayers = new Dictionary<string, GameObject>(); //Keep a list of the other players currently playing the game

    //When trying to connect to the server, it times out after 3 seconds
    private float ConnectionTimeout = 0.0f;
    //Different menu UI elements need to be hidden and shown depending on the connection status
    private MenuInterface UIElements;
    //Set to true from the socket thread, once connection to the server has been established so we know to change what UI elements are being show
    private bool FinishedConnecting = false;

    private void Awake()
    {
        //Assign the UI element reference so UI elements can be configured depending on status of server connection
        UIElements = GetComponent<MenuInterface>();
    }

    private void Update()
    {
        //Whenever a message has been stored here from the socket thread, we need to display it then reset the string 
        if (ConsoleMessage != "")
        {
            Console.Instance.Print(ConsoleMessage);
            ConsoleMessage = "";
        }

        //Wait until we receive a new packet from the server
        if(ShouldHandleData)
        {
            //Pass the packet onto the handler class
            PacketReader.instance.HandlePacket(PacketBuffer);
            //Start waiting for the next packet from the server
            ShouldHandleData = false;
        }

        //If the connection timeout is active, we need to count it down
        if(ConnectionTimeout > 0.0f)
        {
            //count down the timeout
            ConnectionTimeout -= Time.deltaTime;
            //If connection timeout has been reached, stop trying to connect to the server
            if(ConnectionTimeout <= 0.0f)
            {
                Console.Instance.Print("Server connection timed out");
                //Hide the connecting waiting animation and put the connection menu back up
                UIElements.HideConnectingAnimation();
                UIElements.ShowConnectionMenu();
                ClientSocket = null;
                IsConnected = false;
            }
        }

        //Once the connection to the server has been established, we need to change what UI elements are being show
        if(FinishedConnecting)
        {
            FinishedConnecting = false;
            UIElements.HideConnectingAnimation();
            UIElements.ShowAccountUI();
            //Send a mesage to the server saying hello
            PacketSender.instance.SendServerMessage("Hello server");
        }
    }

    //Attempts to form a connection with the game server
    public void ConnectToServer(string IPAddress, int ServerPort)
    {
        Console.Instance.Print("Connecting to server at: " + IPAddress + ":" + ServerPort);

        //Check to make sure the socket is clear and ready for use
        if(ClientSocket != null)
        {
            //If the socket is already connected to the server we dont need to do anything
            if(ClientSocket.Connected || IsConnected)
            {
                Console.Instance.Print("Already connected to the server");
                return;
            }

            //If the connection is not active, then we just need to clean up the socket before we use it
            Console.Instance.Print("Cleaning up client socket");
            ClientSocket.Close();
            ClientSocket = null;
        }

        //Establish a connection to the game server
        Console.Instance.Print("Establishing connection...");
        ConnectionTimeout = 3.0f;   //Start a 3 second timer, after which the connection will time out
        //Hide the connection UI elements and display the connecting waiting animation
        UIElements.HideConnectionMenu();
        UIElements.ShowConnectingAnimation();
        ClientSocket = new TcpClient();
        ClientSocket.ReceiveBufferSize = 4096;
        ClientSocket.SendBufferSize = 4096;
        ClientSocket.NoDelay = false;
        Array.Resize(ref ASyncBuffer, 8192);
        ClientSocket.BeginConnect(IPAddress, ServerPort, new AsyncCallback(ConnectionResult), ClientSocket);
        IsConnected = true;
       // PacketSender.instance.SendServerMessage("Hey server!");
    }

    //Callback trigger which recieves information regarding if the server connection was successful or not
    private void ConnectionResult(IAsyncResult Result)
    {
        if(ClientSocket != null)
        {
            ClientSocket.EndConnect(Result);
            if(!ClientSocket.Connected)
            {
                ConsoleMessage = "Failed to connect";
                IsConnected = false;
                return;
            }
            ConsoleMessage = "Connected";
            ConnectionTimeout = 0.0f;   //halt the timeout timer
            //Hide the connection waiting animation and show the account management UI elements
            FinishedConnecting = true;
            ClientSocket.NoDelay = true;
            ClientStream = ClientSocket.GetStream();    //Start streaming in data from the server, sending it all into the OnReceive function
            ClientStream.BeginRead(ASyncBuffer, 0, 8192, ReadPacket, null);
        }
    }

    //Reads packets sent from the server
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
        if(PacketSize == 0)
        {
            Console.Instance.Print("Connection to the server was lost");
            ClientSocket.Close();
            return;
        }

        //Start listening for new packets again
        ShouldHandleData = true;
        ClientStream.BeginRead(ASyncBuffer, 0, 8192, ReadPacket, null);
    }

    //Close the connection to the server when the game is closed
    private void OnApplicationQuit()
    {
        //If the application is quit while the player is logged into the game we need to quickly tell the server our current character data so it can be backed up into the database for next time
        if(CurrentPlayer != null)
            PacketSender.instance.SendCharacterData(CurrentPlayer.transform.position, CurrentPlayer.transform.rotation);
        //Close the connection if we are connected to the server
        if (ClientSocket != null)
            ClientSocket.Close();
    }
}