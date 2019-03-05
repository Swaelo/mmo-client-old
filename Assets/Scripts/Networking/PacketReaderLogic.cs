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
    AccountRegistrationReply = 1,
    AccountLoginReply = 2,
    CharacterCreationReply = 3,
    CharacterDataReply = 4,
    ActivePlayerList = 5,
    ActiveEntityList = 6,
    EntityUpdates = 7,
    SpawnPlayer = 8,
    PlayerChatMessage = 9,
    PlayerUpdate = 10,
    RemovePlayer = 11,
    ConnectionCheckRequest = 12
}

public class PacketReaderLogic : MonoBehaviour
{
    public static PacketReaderLogic Instance;    //Initialise our singleton instance reference for easy global access, and register the packet handler functions for each packet type
    private PlayerPrefabs PrefabList; //Used to spawn in clients when instructed to
                                      // private MenuInterface UIElements;
    private ServerConnection Connection;

    //Each type of packet is mapped through the dictionary, executed through the delegate so each packet type is automatically passed on to its corresponding handler function
    private delegate void Packet(byte[] PacketData);
    private Dictionary<int, Packet> Packets;

    private void Awake()
    {
        Instance = this;    //store singleton instance
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

        Packets.Add((int)ServerPacketType.AccountRegistrationReply, HandleRegisterReply);
        Packets.Add((int)ServerPacketType.AccountLoginReply, HandleLoginReply);
        Packets.Add((int)ServerPacketType.CharacterCreationReply, HandleCreateCharacterReply);
        Packets.Add((int)ServerPacketType.CharacterDataReply, HandleSendCharacterData);
        Packets.Add((int)ServerPacketType.ActivePlayerList, HandleNewClientOtherPlayers);
        Packets.Add((int)ServerPacketType.ActiveEntityList, HandleNewClientActiveEntities);
        //Packets.Add((int)ServerPacketType.ActiveEntityList, HandleSpawnActiveEntityList);
        Packets.Add((int)ServerPacketType.EntityUpdates, HandleEntityUpdates);
        Packets.Add((int)ServerPacketType.SpawnPlayer, HandleSpawnOtherPlayer);
        Packets.Add((int)ServerPacketType.PlayerChatMessage, HandlePlayerMessage);
        Packets.Add((int)ServerPacketType.PlayerUpdate, HandlePlayerUpdatePosition);
        Packets.Add((int)ServerPacketType.RemovePlayer, HandleRemoveOtherPlayer);
        Packets.Add((int)ServerPacketType.ConnectionCheckRequest, HandleConnectionCheck); 
    }

    //Gets a packet from the server and sends it onto whatever function its mapped to in the dictionary
    public void HandlePacket(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
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
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        string Message = Reader.ReadString();
        //Display the message to the console
        ChatWindow.Instance.DisplaySystemMessage(Message);
    }

    //message from server to display a players chat message in the chat window
    private void HandlePlayerMessage(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        string Sender = Reader.ReadString();
        string Message = Reader.ReadString();

        ChatWindow.Instance.DisplayPlayerMessage(Sender, Message);
    }

    //Gets a reply from the server letting us know if our account registration request was successfull or not
    private void HandleRegisterReply(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        int RegisterSuccess = Reader.ReadInt();   //1 = success, 0 = failure
        string ReplyMessage = Reader.ReadString();   //tells us what went wrong if the account was not able to be created
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
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        int LoginSuccess = Reader.ReadInt();   //1 = success, 0 = failure
        string ReplyMessage = Reader.ReadString();   //tells us what went wrong if the account was not able to be logged in to
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
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        int CreationSuccess = Reader.ReadInt();
        string ReplyMessage = Reader.ReadString();
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
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        //Get how many characters info is in this packet
        int CharacterCount = Reader.ReadInt();
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
            Data.Account = Reader.ReadString();
            Data.Position = Reader.ReadVector3();
            Data.Name = Reader.ReadString();
            Data.Experience = Reader.ReadInt();
            Data.ExperienceToLevel = Reader.ReadInt();
            Data.Level = Reader.ReadInt();
            Data.IsMale = Reader.ReadInt() == 1;
            CharacterSelect.SaveCharacterData(i + 1, Data);
            CharacterSelect.SetSelectedCharacter(i + 1);
        }
        //All information has been extracted successfully
        CharacterSelect.CharacterCount = CharacterCount;
        CharacterSelect.CharacterDataLoaded();
    }

    private void HandleNewClientOtherPlayers(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        int OtherPlayerCount = Reader.ReadInt();
        for(int i = 0; i < OtherPlayerCount; i++)
        {
            string OtherPlayerName = Reader.ReadString();
            Vector3 OtherPlayerPosition = Reader.ReadVector3();
            GameObject OtherPlayerSpawn = Instantiate(PrefabList.ExternalPlayer, OtherPlayerPosition, Quaternion.identity);
            OtherPlayerSpawn.GetComponentInChildren<TextMesh>().text = OtherPlayerName;
            OtherPlayerSpawn.name = OtherPlayerName;
            Connection.AddOtherPlayer(OtherPlayerName, OtherPlayerSpawn);
        }
        //Now we have spawned in all the other players, now ask the server to tell us about all the active entities
        PacketSenderLogic.Instance.SendNewClientGetEntities();
    }

    private void HandleNewClientActiveEntities(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        int EntityCount = Reader.ReadInt();
        for(int i = 0; i < EntityCount; i++)
        {
            string Type = Reader.ReadString();
            string ID = Reader.ReadString();
            Vector3 Pos = Reader.ReadVector3();
            GameObject EntitySpawn = GameObject.Instantiate(EntityPrefabs.GetEntityPrefab(Type), Pos, Quaternion.identity);
            EntitySpawn.GetComponent<ServerEntity>().ID = ID;
            EntityManager.AddNewEntity(ID, EntitySpawn);
        }

        //Now all active entites have been spawn in, we may enter into the game world now
        l.og("entering world");
        CharacterData Data = MenuStateManager.GetMenuComponents("Character Selection").GetComponent<CharacterSelectionButtonFunctions>().SelectedCharacter;
        GameObject NewPlayer = Instantiate(PrefabList.ClientPlayer, Data.Position, Quaternion.identity);
        GameObject.Find("Main Camera").SetActive(false);
        MenuStateManager.SetMenuState("UI");
        MenuStateManager.GetMenuComponents("UI").GetComponent<MenuComponentObjects>().GetComponentObject("InputField").GetComponentInChildren<Text>().text = "[Press Enter to chat]";
        NewPlayer.name = Data.Name;
        PlayerInfo.AccountName = Data.Account;
        PlayerInfo.CharacterName = Data.Name;
        PlayerInfo.PlayerObject = NewPlayer;
        PlayerInfo.GameState = GameStates.PlayingGame;

        //Now instruct the server that we have completed entering into the game world
        PacketSenderLogic.Instance.SendNewPlayerReady();
    }

    //server giving us another characters updated position information
    private void HandlePlayerUpdatePosition(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();    //read in the packet type
        string CharacterName = Reader.ReadString();
        Vector3 CharacterPosition = Reader.ReadVector3();
        Quaternion CharacterRotation = Reader.ReadQuaternion();

        //Client dont need to update themselves
        if (CharacterName == PlayerInfo.CharacterName)
            return;
        //Otherwise we find this character in the world and update their position value
        Connection.GetOtherPlayer(CharacterName).GetComponent<ExternalPlayerMovement>().UpdatePosition(CharacterPosition, CharacterRotation);
    }

    private void HandleSpawnActiveEntityList(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        int EntityCount = Reader.ReadInt();
        for(int i = 0; i < EntityCount; i++)
        {
            //Get the information for each of the entities that have been sent to us
            string ID = Reader.ReadString();
            Vector3 Location = Reader.ReadVector3();
            //Spawn this entity into our game world and store them within the entity manager
            //ChatWindow.Log("spawn " + Type + " at " + Location);
            GameObject NewEntity = GameObject.Instantiate(EntityPrefabs.GetEntityPrefab("Fox Princess"), Location, Quaternion.identity);
            NewEntity.GetComponent<ServerEntity>().ID = ID;
            EntityManager.AddNewEntity(ID, NewEntity);
        }
    }

    private void HandleEntityUpdates(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        int EntityCount = Reader.ReadInt();
        for (int i = 0; i < EntityCount; i++)
        {
            //Extract this entities data from the network packet
            string EntityID = Reader.ReadString();
            Vector3 EntityPosition = Reader.ReadVector3();
            Quaternion EntityRotation = Reader.ReadQuaternion();
            //Send this info to the entity manager who will know where to find the entity who needs updating
            EntityManager.UpdateEntity(EntityID, EntityPosition, EntityRotation);
        }
    }

    private void HandleUpdateEntityHealth(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        string EntityID = Reader.ReadString();
        int EntityHealth = Reader.ReadInt();
        GameObject TargetEntity = EntityManager.ActiveEntities[EntityID];
        TargetEntity.GetComponentInChildren<TextMesh>().text = "Fox Princess " + EntityHealth + "/5";
    }

    //server tells us to spawn someone elses character into our world
    private void HandleSpawnOtherPlayer(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        string CharacterName = Reader.ReadString();
        ChatWindow.Instance.DisplaySystemMessage(CharacterName + " has entered the world.");
        Vector3 CharacterPosition = Reader.ReadVector3();

        //Spawn the other player into the world
        GameObject OtherPlayer = Instantiate(PrefabList.ExternalPlayer, CharacterPosition, Quaternion.identity);
        OtherPlayer.GetComponentInChildren<TextMesh>().text = CharacterName;
        OtherPlayer.name = CharacterName;
        Connection.AddOtherPlayer(CharacterName, OtherPlayer);
    }

    //server tells us to remove another clients character from our game world
    private void HandleRemoveOtherPlayer(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        string CharacterName = Reader.ReadString();
        ChatWindow.Instance.DisplaySystemMessage(CharacterName + " has left the world.");
        //remove the player from the world and the list of other players
        Connection.RemoveOtherPlayer(CharacterName);
    }

    //Recieve and automatically reply to a connection check send to us from the game server
    private void HandleConnectionCheck(byte[] PacketData)
    {
        PacketSenderLogic.Instance.SendConnectionCheckReply();
    }
}