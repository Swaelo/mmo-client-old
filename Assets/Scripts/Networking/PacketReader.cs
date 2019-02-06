using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ServerPacketType
{
    ConsoleMessage = 1, //message from server to display a message in the console log window
    PlayerMessage = 2,  //message from server to display a players chat message in the chat window

    RegisterReply = 3,  //reply from server if our account registration was successful
    LoginReply = 4, //reply from server if our account login was successful
    CreateCharacterReply = 5,    //reply from server if our new character creation was successful

    SendCharacterData = 6,  //reply from server with all our created characters information
    PlayerEnterWorld = 7,    //server telling us to enter the game world with our selected character
    PlayerUpdatePosition = 8,   //server giving us another players updated position information

    SpawnOtherPlayer = 9,   //server telling us to spawn another clients character into our world
    RemoveOtherPlayer = 10  //server telling us to remove a disconnected clients character from the world 
}

public class PacketReader : MonoBehaviour
{
    public static PacketReader instance;    //Initialise our singleton instance reference for easy global access, and register the packet handler functions for each packet type
    private PlayerPrefabs PrefabList; //Used to spawn in clients when instructed to
   // private MenuInterface UIElements;
    private ServerConnection Connection;

    //Each type of packet is mapped through the dictionary, executed through the delegate so each packet type is automatically passed on to its corresponding handler function
    private delegate void Packet(byte[] PacketData);
    private Dictionary<int, Packet> Packets;
    [SerializeField] private MenuHandler UI;

    private void Awake()
    {
        instance = this;    //store singleton instance
        RegisterPacketHandlers();   //register all packet handler functions into the dictionary
        PrefabList = GetComponent<PlayerPrefabs>();   //grab the prefab list so we can use it to spawn players into the world
     //   UIElements = GetComponent<MenuInterface>();
        Connection = GetComponent<ServerConnection>();
    }

    //Maps each packet handler function into the dictionary to its corresponding packet type
    private void RegisterPacketHandlers()
    {
        Packets = new Dictionary<int, Packet>();

        Packets.Add((int)ServerPacketType.ConsoleMessage, HandleConsoleMessage);
        Packets.Add((int)ServerPacketType.PlayerMessage, HandlePlayerMessage);

        Packets.Add((int)ServerPacketType.RegisterReply, HandleRegisterReply);
        Packets.Add((int)ServerPacketType.LoginReply, HandleLoginReply);
        Packets.Add((int)ServerPacketType.CreateCharacterReply, HandleCreateCharacterReply);

        Packets.Add((int)ServerPacketType.SendCharacterData, HandleSendCharacterData);
        Packets.Add((int)ServerPacketType.PlayerEnterWorld, HandlePlayerEnterWorld);
        Packets.Add((int)ServerPacketType.PlayerUpdatePosition, HandlePlayerUpdatePosition);

        Packets.Add((int)ServerPacketType.SpawnOtherPlayer, HandleSpawnOtherPlayer);
        Packets.Add((int)ServerPacketType.RemoveOtherPlayer, HandleRemoveOtherPlayer);
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
            return;
        //Pass packet onto correct handler function
        Packet NewPacket;
        if (Packets.TryGetValue(PacketType, out NewPacket))
            NewPacket.Invoke(PacketData);
    }

    //Recieves a message from the console to be displayed in our console log window
    private void HandleConsoleMessage(byte[] PacketData)
    {
        //Extract the information we need from the packet
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        string Message = PacketReader.ReadString();
        //Display the message to the console
        ChatWindow.Instance.DisplaySystemMessage(Message);
        //Close the packet reader
        PacketReader.Dispose();
    }

    //message from server to display a players chat message in the chat window
    private void HandlePlayerMessage(byte[] PacketData)
    {
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        string Sender = PacketReader.ReadString();
        string Message = PacketReader.ReadString();
        UI.ChatWindowObject.GetComponent<ChatWindow>().DisplayPlayerMessage(Sender, Message);
        PacketReader.Dispose();
    }

    //Gets a reply from the server letting us know if our account registration request was successfull or not
    private void HandleRegisterReply(byte[] PacketData)
    {
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        int RegisterSuccess = PacketReader.ReadInteger();   //1 = success, 0 = failure
        string ReplyMessage = PacketReader.ReadString();   //tells us what went wrong if the account was not able to be created
        ChatWindow.Instance.DisplayReplyMessage(RegisterSuccess == 1, ReplyMessage); //show the reply message to the console log
        //Update the menu UI state based on if the account was created successfully or not
        if (RegisterSuccess == 1)
            UI.RegisterSuccess();
        else
            UI.RegisterFail();
    }

    //Gets a reply from the server letting us know if our account login request was successful or not
    private void HandleLoginReply(byte[] PacketData)
    {
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        int LoginSuccess = PacketReader.ReadInteger();   //1 = success, 0 = failure
        string ReplyMessage = PacketReader.ReadString();   //tells us what went wrong if the account was not able to be logged in to
        PacketReader.Dispose();
        ChatWindow.Instance.DisplayReplyMessage(LoginSuccess == 1, ReplyMessage);
        //Update the menu UI state based on if the account was logged into successfully or not
        if (LoginSuccess == 1)
            UI.LoginSuccess();
        else
            UI.LoginFail();
    }

    //server tells us if our character creation was succesful
    private void HandleCreateCharacterReply(byte[] PacketData)
    {
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        int CreationSuccess = PacketReader.ReadInteger();
        string ReplyMessage = PacketReader.ReadString();
        PacketReader.Dispose();
        //If the character was not able to be created, return to character creation menu and print error message to the console
        ChatWindow.Instance.DisplayReplyMessage(CreationSuccess == 1, ReplyMessage);
        if (CreationSuccess == 0)
        {
            UI.CreateCharacterFailure();
            return;
        }
        UI.CreateCharacterSuccess();
    }

    //Gets a packet from the server with info on every character registered to our account so far
    private void HandleSendCharacterData(byte[] PacketData)
    {
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        //Get how many characters info is in this packet
        int CharacterCount = PacketReader.ReadInteger();
        //If the character count is 0, we must send the client straight to the character creation screen
        if(CharacterCount == 0)
        {
            UI.NoCharactersCreated();
            return;
        }

        //Otherwise we need to loop through however many characters exists in this account and extract all of the information for each of them
        for(int i = 0; i < CharacterCount; i++)
        {
            //Extract all the information and save it into a characterdata object structure
            CharacterData Data = new CharacterData();
            Data.Account = PacketReader.ReadString();
            Data.Position = new Vector3(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());
            Data.Name = PacketReader.ReadString();
            Data.Experience = PacketReader.ReadInteger();
            Data.ExperienceToLevel = PacketReader.ReadInteger();
            Data.Level = PacketReader.ReadInteger();
            Data.IsMale = PacketReader.ReadInteger() == 1;
            //Save it into the correct character data slot
            UI.SaveCharacterData(i + 1, Data);
            //UI.SetSelectedCharacterData(i + 1);
        }
        //All information has been extracted successfully
        UI.SetCreatedCharacterCount(CharacterCount);
        PacketReader.Dispose();
        UI.CharacterDataLoaded();
    }
    
    //server tells us to enter into the game world
    private void HandlePlayerEnterWorld(byte[] PacketData)
    {
        //Spawn our character into the world
        CharacterData Data = UI.GetSelectedCharacterData();
        GameObject NewPlayer = Instantiate(PrefabList.ClientPlayer, Data.Position, Quaternion.identity);
        GameObject.Find("Main Camera").SetActive(false);
        UI.WorldEntered();
        NewPlayer.name = Data.Name;
        PlayerInfo.CharacterName = Data.Name;
        PlayerInfo.AccountName = Data.Account;
        PlayerInfo.PlayerObject = NewPlayer;
        PlayerInfo.GameState = GameStates.PlayingGame;

        //Spawn all the other players characters into the world too
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        int OtherPlayerCount = PacketReader.ReadInteger();
        for(int i = 0; i < OtherPlayerCount; i++)
        {
            string CharacterName = PacketReader.ReadString();
            Vector3 CharacterPosition = new Vector3(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());
            GameObject OtherPlayer = Instantiate(PrefabList.ExternalPlayer, CharacterPosition, Quaternion.identity);
            OtherPlayer.GetComponentInChildren<TextMesh>().text = CharacterName;
            OtherPlayer.name = CharacterName;
            Connection.OtherPlayers.Add(CharacterName, OtherPlayer);
        }
        PacketReader.Dispose();
    }

    //server giving us another characters updated position information
    private void HandlePlayerUpdatePosition(byte[] PacketData)
    {
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();    //read in the packet type
        string CharacterName = PacketReader.ReadString();
        Vector3 CharacterPosition = new Vector3(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());
        Quaternion CharacterRotation = new Quaternion(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());
        PacketReader.Dispose();

        //Client dont need to update themselves
        if (CharacterName == PlayerInfo.CharacterName)
            return;
        //Otherwise we find this character in the world and update their position value
        Connection.OtherPlayers[CharacterName].GetComponent<ExternalPlayerMovement>().UpdatePosition(CharacterPosition, CharacterRotation);
    }

    //server tells us to spawn someone elses character into our world
    private void HandleSpawnOtherPlayer(byte[] PacketData)
    {
        //Extract packet information
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        string CharacterName = PacketReader.ReadString();
        Vector3 CharacterPosition = new Vector3(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());
        PacketReader.Dispose();

        //Spawn the other player into the world
        GameObject OtherPlayer = Instantiate(PrefabList.ExternalPlayer, CharacterPosition, Quaternion.identity);
        OtherPlayer.GetComponentInChildren<TextMesh>().text = CharacterName;
        OtherPlayer.name = CharacterName;
        Connection.OtherPlayers.Add(CharacterName, OtherPlayer);
    }
    
    //server tells us to remove another clients character from our game world
    private void HandleRemoveOtherPlayer(byte[] PacketData)
    {
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        string CharacterName = PacketReader.ReadString();
        PacketReader.Dispose();
        //remove the player from the world and the list of other players
        GameObject OtherPlayer = Connection.OtherPlayers[CharacterName];
        Connection.OtherPlayers.Remove(CharacterName);
        GameObject.Destroy(OtherPlayer);
    }
}