using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ServerPacketType
{
    Message = 1,    //server sends a message to the client <int:PacketType, string:Message>
    EnterGame = 2,   //server sends character info data to the client and tells them to spawn into the game <int:PacketType, float:XPos, float:YPos, float:ZPos, float:XRot, float:YRot, float:ZRot, float:WRot>
    SpawnExternalClient = 3, //Gets a message from the server to spawn someone elses character into the game world <int:PacketType, float:XPos, float:YPos, float:ZPos, float:XRot, float:YRot, float:ZRot, float:WRot>
    UpdatePlayer = 4,    //Gets a message from the server with one of the other clients updated position and rotation data <int:PacketType, string:AccountName, vector3:position, vector4:rotation>
    RemovePlayer = 5,   //Server lets us know one of the other players has disconnected from the game
    PlayerMessage = 6,  //Server tells us all of the chat messages sent by other players to be displayed in the chat window
    GroundItems = 7 //When we join the game the server lets us know about all the items that are currently on the ground in the game world
}

public class PacketReader : MonoBehaviour
{
    public static PacketReader instance;    //Initialise our singleton instance reference for easy global access, and register the packet handler functions for each packet type
    private PlayerPrefabs PrefabList; //Used to spawn in clients when instructed to
    private MenuInterface UIElements;
    private ServerConnection Connection;

    //Each type of packet is mapped through the dictionary, executed through the delegate so each packet type is automatically passed on to its corresponding handler function
    private delegate void Packet(byte[] PacketData);
    private Dictionary<int, Packet> Packets;

    private void Awake()
    {
        instance = this;    //store singleton instance
        RegisterPacketHandlers();   //register all packet handler functions into the dictionary
        PrefabList = GetComponent<PlayerPrefabs>();   //grab the prefab list so we can use it to spawn players into the world
        UIElements = GetComponent<MenuInterface>();
        Connection = GetComponent<ServerConnection>();
    }

    //Maps each packet handler function into the dictionary to its corresponding packet type
    private void RegisterPacketHandlers()
    {
        Packets = new Dictionary<int, Packet>();
        Packets.Add((int)ServerPacketType.Message, HandleMessage);
        Packets.Add((int)ServerPacketType.EnterGame, HandleEnterGame);
        Packets.Add((int)ServerPacketType.SpawnExternalClient, HandleSpawnOther);
        Packets.Add((int)ServerPacketType.UpdatePlayer, HandlePlayerUpdate);
        Packets.Add((int)ServerPacketType.RemovePlayer, HandleRemovePlayer);
        Packets.Add((int)ServerPacketType.PlayerMessage, HandlePlayerMessage);
        Packets.Add((int)ServerPacketType.GroundItems, HandleGroundItems);
    }

    //Gets a packet from the server and sends it onto whatever function its mapped to in the dictionary
    public void HandlePacket(byte[] PacketData)
    {
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        //Close the packet reader
        PacketReader.Dispose();
        //Ignore empty packets
        if (PacketType == 0)
        {
            Console.Instance.Print("ignoring network packet with type 0");
            return;
        }
        //Pass packet onto correct handler function
        Packet NewPacket;
        if (Packets.TryGetValue(PacketType, out NewPacket))
            NewPacket.Invoke(PacketData);
    }


    private void HandleMessage(byte[] PacketData)
    {
        //Extract the information we need from the packet
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        string Message = PacketReader.ReadString();
        //Display the message to the console
        Console.Instance.Print("Server: " + Message);
        //Close the packet reader
        PacketReader.Dispose();
    }

    //Gets message from the server to enter into the game world <int:PacketType, float:XPos, float:YPos, float:ZPos, float:XRot, float:YRot, float:ZRot, float:WRot>
    private void HandleEnterGame(byte[] PacketData)
    {
        //Extract the information we need from the server packet
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        string AccountName = PacketReader.ReadString();
        Vector3 CharacterPosition = new Vector3(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());
        Quaternion CharacterRotation = new Quaternion(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());
        //Close the packet reader
        PacketReader.Dispose();
        //Now spawn the player into the game world
        Console.Instance.Print("Entering game...");
        GameObject NewPlayer = Instantiate(PrefabList.ClientPlayer, CharacterPosition, CharacterRotation);
        //Disable the scene camera
        GameObject.Find("Menu Camera").SetActive(false);
        UIElements.ShowGameUI(AccountName);
        NewPlayer.name = AccountName;
        Connection.CurrentPlayer = NewPlayer;
        //Disable any remaining UI elements that nolonger need to be there
        UIElements.HideAccountUI();
    }

    //Gets message from the server to spawn another players character into the game world <int:PacketType, float:XPos, float:YPos, float:ZPos, float:XRot, float:YRot, float:ZRot, float:WRot>
    private void HandleSpawnOther(byte[] PacketData)
    {
        //Extract the information we need from the server packet
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);    
        int PacketType = PacketReader.ReadInteger();    //read packet type
        string AccountName = PacketReader.ReadString(); //read account name
        Vector3 CharacterPosition = new Vector3(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());
        Quaternion CharacterRotation = new Quaternion(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());
        //close the packet reader
        PacketReader.Dispose();
        //Now spawn the other players character into the game world
        GameObject OtherPlayer = Instantiate(PrefabList.ExternalPlayer, CharacterPosition, CharacterRotation);
        //Set the players name above their head
        OtherPlayer.GetComponentInChildren<TextMesh>().text = AccountName;
        OtherPlayer.name = AccountName;
        //store them in the list of players sorted by their player names
        Connection.OtherPlayers.Add(AccountName, OtherPlayer);
    }

    //Gets a message from the server with one of the other clients updated position and rotation data <int:PacketType, string:AccountName, vector3:position, vector4:rotation>
    private void HandlePlayerUpdate(byte[] PacketData)
    {
        //Extract the information we need from the packet
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();    //read in the packet type
        string AccountName = PacketReader.ReadString(); //read in the account name of the player we need to update
        Vector3 NewPosition = new Vector3(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());    //read in the new position values
        PacketReader.Dispose(); //close the reader
        //clients dont need to be updating themselves so just ignore the server when it tells us to do that
        if(AccountName == Connection.CurrentPlayer.name)
            return;
        //Find this player character and update their values
        Connection.OtherPlayers[AccountName].GetComponent<ExternalPlayerMovement>().UpdatePosition(NewPosition);
    }

    //Server lets us know one of the other players has disconnected from the game
    private void HandleRemovePlayer(byte[] PacketData)
    {
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();    //read in the packet type
        string AccountName = PacketReader.ReadString(); //read in the account name of the player we need to update
        GameObject Player = Connection.OtherPlayers[AccountName];
        Connection.OtherPlayers.Remove(AccountName);
        GameObject.Destroy(Player);
    }

    //Server tells us all of the chat messages sent by other players to be displayed in the chat window
    private void HandlePlayerMessage(byte[] PacketData)
    {
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();    //read in the packet type
        string AccountName = PacketReader.ReadString();
        string Message = PacketReader.ReadString();
        UIElements.ChatWindowObject.GetComponent<ChatWindow>().ExternalMessage(AccountName, Message);
    }

    //When we join the game the server lets us know about all the items that are currently on the ground in the game world
    private void HandleGroundItems(byte[] PacketData)
    {
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();    //read in the packet type
        int ItemCount = PacketReader.ReadInteger(); //read in how many items are in the world
        Console.Instance.Print("There are " + ItemCount + " items on the ground");
        //Grab the list all the items will be stored in
        GroundItems ItemList = GetComponent<GroundItems>();
        for(int iter = 0; iter < ItemCount; iter++)
        {
            //read in the information for each item there is stored
            int ItemID = PacketReader.ReadInteger();
            //read the position data where the item should be spawned at
            Vector3 ItemPosition = new Vector3(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());
            Quaternion ItemRotation = new Quaternion(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());
            //find the info about the object were about to spawn
            GameObject CurrentItem = GetComponent<ItemPrefabs>().ItemPrefabList[ItemID-1];
            string ItemName = CurrentItem.GetComponent<GameItem>().ItemName;
            //spawn this object into the game world and add it to the list of items in the world
            Console.Instance.Print(ItemName + " is on the ground");
            GameObject NewItem = GameObject.Instantiate(CurrentItem, ItemPosition, ItemRotation);
            ItemList.AddItem(NewItem.GetComponent<GameItem>());
        }
    }
}