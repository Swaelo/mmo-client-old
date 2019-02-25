// ================================================================================================================================
// File:        PacketReader.cs
// Description: Receives all packets sent to us from the server and performs whatever tasks are required based on the packet type
// Author:      Harley Laurie          
// Notes:       This needs to be split up into multiple seperate classes soon, this file is way too big
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    SpawnActiveEntityList = 9,  //server gives us a list of all the active entities in the game for us to spawn in
    SendEntityUpdates = 10, //server is giving us the updated info for all the entities active in the game right now
    UpdateEntityHealth = 11,

    SpawnOtherPlayer = 12,   //server telling us to spawn another clients character into our world
    RemoveOtherPlayer = 13  //server telling us to remove a disconnected clients character from the world
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

    private void Awake()
    {
        instance = this;    //store singleton instance
        RegisterPacketHandlers();   //register all packet handler functions into the dictionary
        GameObject System = GameObject.Find("System");
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

        Packets.Add((int)ServerPacketType.SpawnActiveEntityList, HandleSpawnActiveEntityList);
        Packets.Add((int)ServerPacketType.SendEntityUpdates, HandleEntityUpdates);
        Packets.Add((int)ServerPacketType.UpdateEntityHealth, HandleUpdateEntityHealth);

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
        ChatWindow.Log("handle console message");
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
        ChatWindow.Log("handle player message");
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        string Sender = PacketReader.ReadString();
        string Message = PacketReader.ReadString();

        ChatWindow.Instance.DisplayPlayerMessage(Sender, Message);
        PacketReader.Dispose();
    }

    //Gets a reply from the server letting us know if our account registration request was successfull or not
    private void HandleRegisterReply(byte[] PacketData)
    {
        ChatWindow.Log("handle register reply");
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        int RegisterSuccess = PacketReader.ReadInteger();   //1 = success, 0 = failure
        string ReplyMessage = PacketReader.ReadString();   //tells us what went wrong if the account was not able to be created
        ChatWindow.Instance.DisplayReplyMessage(RegisterSuccess == 1, ReplyMessage); //show the reply message to the console log
        //Update the menu UI state based on if the account was created successfully or not
        if (RegisterSuccess == 1)
            MenuStateManager.GetMenuComponents("Account Creation").GetComponent<AccountCreationButtonFunctions>().RegisterSuccess();
        else
            MenuStateManager.GetMenuComponents("Account Creation").GetComponent<AccountCreationButtonFunctions>().RegisterFail();
    }

    //Gets a reply from the server letting us know if our account login request was successful or not
    private void HandleLoginReply(byte[] PacketData)
    {
        ChatWindow.Log("handle login reply");
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        int LoginSuccess = PacketReader.ReadInteger();   //1 = success, 0 = failure
        string ReplyMessage = PacketReader.ReadString();   //tells us what went wrong if the account was not able to be logged in to
        PacketReader.Dispose();
        ChatWindow.Instance.DisplayReplyMessage(LoginSuccess == 1, ReplyMessage);
        //Update the menu UI state based on if the account was logged into successfully or not
        if (LoginSuccess == 1)
            MenuStateManager.GetMenuComponents("Account Login").GetComponent<AccountLoginButtonFunctions>().LoginSuccess();
        else
            MenuStateManager.GetMenuComponents("Account Login").GetComponent<AccountLoginButtonFunctions>().LoginFail();
    }

    //server tells us if our character creation was succesful
    private void HandleCreateCharacterReply(byte[] PacketData)
    {
        ChatWindow.Log("handle create character reply");
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        int CreationSuccess = PacketReader.ReadInteger();
        string ReplyMessage = PacketReader.ReadString();
        PacketReader.Dispose();
        //If the character was not able to be created, return to character creation menu and print error message to the console
        ChatWindow.Instance.DisplayReplyMessage(CreationSuccess == 1, ReplyMessage);
        if(CreationSuccess == 0)
        {
            MenuStateManager.GetMenuComponents("Character Creation").GetComponent<CharacterCreationButtonFunctions>().CreateCharacterFail();
            return;
        }
        MenuStateManager.GetMenuComponents("Character Creation").GetComponent<CharacterCreationButtonFunctions>().CreateCharacterSuccess();
    }

    //Gets a packet from the server with info on every character registered to our account so far
    private void HandleSendCharacterData(byte[] PacketData)
    {
        ChatWindow.Log("handle send character data");
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        //Get how many characters info is in this packet
        int CharacterCount = PacketReader.ReadInteger();
        //If the character count is 0, we must send the client straight to the character creation screen
        if (CharacterCount == 0)
        {
            MenuStateManager.GetMenuComponents("Character Selection").GetComponent<CharacterSelectionButtonFunctions>().NoCharactersCreated();
            return;
        }
        //Otherwise we need to loop through however many characters exists in this account and extract all of the information for each of them
        CharacterSelectionButtonFunctions CharacterSelect = MenuStateManager.GetMenuComponents("Character Selection").GetComponent<CharacterSelectionButtonFunctions>();
        for (int i = 0; i < CharacterCount; i++)
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
            CharacterSelect.SaveCharacterData(i + 1, Data);
            CharacterSelect.SetSelectedCharacter(i + 1);
        }
        //All information has been extracted successfully
        CharacterSelect.CharacterCount = CharacterCount;
        PacketReader.Dispose();
        CharacterSelect.CharacterDataLoaded();
    }

    //server tells us to enter into the game world
    private void HandlePlayerEnterWorld(byte[] PacketData)
    {
        ChatWindow.Log("handle player enter world");
        //Spawn our character into the world
        CharacterData Data = MenuStateManager.GetMenuComponents("Character Selection").GetComponent<CharacterSelectionButtonFunctions>().SelectedCharacter;
        GameObject NewPlayer = Instantiate(PrefabList.ClientPlayer, Data.Position, Quaternion.identity);
        //Disable the main scene camera
        GameObject.Find("Main Camera").SetActive(false);
        MenuStateManager.SetMenuState("UI");
        MenuStateManager.GetMenuComponents("UI").GetComponent<MenuComponentObjects>().GetComponentObject("InputField").GetComponentInChildren<Text>().text = "[Press Enter to chat]";
        NewPlayer.name = Data.Name;
        PlayerInfo.AccountName = Data.Account;
        PlayerInfo.CharacterName = Data.Name;
        PlayerInfo.PlayerObject = NewPlayer;
        PlayerInfo.GameState = GameStates.PlayingGame;
        //Spawn all the other players characters into the world too
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        int OtherPlayerCount = PacketReader.ReadInteger();
        for (int i = 0; i < OtherPlayerCount; i++)
        {
            string CharacterName = PacketReader.ReadString();
            //ChatWindow.Instance.DisplaySystemMessage(CharacterName + " is already playing ")
            Vector3 CharacterPosition = new Vector3(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());
            GameObject OtherPlayer = Instantiate(PrefabList.ExternalPlayer, CharacterPosition, Quaternion.identity);
            OtherPlayer.GetComponentInChildren<TextMesh>().text = CharacterName;
            OtherPlayer.name = CharacterName;
            Connection.AddOtherPlayer(CharacterName, OtherPlayer);
        }
        int EntityCount = PacketReader.ReadInteger();
        for(int i = 0; i < EntityCount; i++)
        {
            string EntityID = PacketReader.ReadString();
            Vector3 EntityPos = new Vector3(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());
            GameObject NewEntity = GameObject.Instantiate(EntityPrefabs.GetEntityPrefab("Fox Princess"), EntityPos, Quaternion.identity);
            NewEntity.GetComponent<ServerEntity>().ID = EntityID;
            EntityManager.AddNewEntity(EntityID, NewEntity);
        }
        PacketReader.Dispose();
    }

    //server giving us another characters updated position information
    private void HandlePlayerUpdatePosition(byte[] PacketData)
    {
        ChatWindow.Log("handle update player position");
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
        Connection.GetOtherPlayer(CharacterName).GetComponent<ExternalPlayerMovement>().UpdatePosition(CharacterPosition, CharacterRotation);
    }

    private void HandleSpawnActiveEntityList(byte[] PacketData)
    {
        ChatWindow.Log("handle spawn entity list");
        //Extract from the packet the entire list of active entities
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        int EntityCount = PacketReader.ReadInteger();
        for(int i = 0; i < EntityCount; i++)
        {
            //Get the information for each of the entities that have been sent to us
            string ID = PacketReader.ReadString();
            Vector3 Location = new Vector3(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());
            //Spawn this entity into our game world and store them within the entity manager
            //ChatWindow.Log("spawn " + Type + " at " + Location);
            GameObject NewEntity = GameObject.Instantiate(EntityPrefabs.GetEntityPrefab("Fox Princess"), Location, Quaternion.identity);
            NewEntity.GetComponent<ServerEntity>().ID = ID;
            EntityManager.AddNewEntity(ID, NewEntity);
        }
        PacketReader.Dispose();
    }

    private void HandleEntityUpdates(byte[] PacketData)
    {
        ChatWindow.Log("handle entity updates");
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        int EntityCount = PacketReader.ReadInteger();
        //ChatWindow.Instance.DisplaySystemMessage("handle " + EntityCount + " entity updates");
        for (int i = 0; i < EntityCount; i++)
        {
            //Extract this entities data from the network packet
            string EntityID = PacketReader.ReadString();
            Vector3 EntityPosition = new Vector3(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());
            Quaternion EntityRotation = new Quaternion(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());
            //Send this info to the entity manager who will know where to find the entity who needs updating
            EntityManager.UpdateEntity(EntityID, EntityPosition, EntityRotation);
        }
        PacketReader.Dispose();
    }

    private void HandleUpdateEntityHealth(byte[] PacketData)
    {
        ChatWindow.Log("handle entity health update");
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        string EntityID = PacketReader.ReadString();
        int EntityHealth = PacketReader.ReadInteger();
        GameObject TargetEntity = EntityManager.ActiveEntities[EntityID];
        TargetEntity.GetComponentInChildren<TextMesh>().text = "Fox Princess " + EntityHealth + "/5";
        PacketReader.Dispose();
    }

    //server tells us to spawn someone elses character into our world
    private void HandleSpawnOtherPlayer(byte[] PacketData)
    {
        ChatWindow.Log("handle spawn other players");
        //Extract packet information
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        string CharacterName = PacketReader.ReadString();
        ChatWindow.Instance.DisplaySystemMessage(CharacterName + " has entered the world.");
        Vector3 CharacterPosition = new Vector3(PacketReader.ReadFloat(), PacketReader.ReadFloat(), PacketReader.ReadFloat());
        PacketReader.Dispose();

        //Spawn the other player into the world
        GameObject OtherPlayer = Instantiate(PrefabList.ExternalPlayer, CharacterPosition, Quaternion.identity);
        OtherPlayer.GetComponentInChildren<TextMesh>().text = CharacterName;
        OtherPlayer.name = CharacterName;
        Connection.AddOtherPlayer(CharacterName, OtherPlayer);
    }

    //server tells us to remove another clients character from our game world
    private void HandleRemoveOtherPlayer(byte[] PacketData)
    {
        ChatWindow.Log("handle remove other player");
        ByteBuffer.ByteBuffer PacketReader = new ByteBuffer.ByteBuffer();
        PacketReader.WriteBytes(PacketData);
        int PacketType = PacketReader.ReadInteger();
        string CharacterName = PacketReader.ReadString();
        ChatWindow.Instance.DisplaySystemMessage(CharacterName + " has left the world.");
        PacketReader.Dispose();
        //remove the player from the world and the list of other players
        Connection.RemoveOtherPlayer(CharacterName);
    }
}