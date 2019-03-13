// ================================================================================================================================
// File:        PacketManager.cs
// Description: Defines all the functions used for sending and recieving data through the network to the game server
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    DisconnectionNotice = 10
}

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
    RemovePlayer = 11
}

public class PacketManager : MonoBehaviour
{
    public static PacketManager Instance;   //Singleton instance of the packet manager
    private void Awake()
    {
        Instance = this;
        RegisterPacketHandlers();
    }

    //Dictionary of packet handlers, each mapped to their handler function
    private delegate void Packet(byte[] PacketData);
    private Dictionary<int, Packet> Packets = new Dictionary<int, Packet>();
    private void RegisterPacketHandlers()
    {
        Packets.Add((int)ServerPacketType.AccountRegistrationReply, HandleRegisterReply);
        Packets.Add((int)ServerPacketType.AccountLoginReply, HandleLoginReply);
        Packets.Add((int)ServerPacketType.CharacterCreationReply, HandleCreateCharacterReply);
        Packets.Add((int)ServerPacketType.CharacterDataReply, HandleCharacterData);
        Packets.Add((int)ServerPacketType.ActivePlayerList, HandleActivePlayerList);
        Packets.Add((int)ServerPacketType.ActiveEntityList, HandleActiveEntityList);
        Packets.Add((int)ServerPacketType.EntityUpdates, HandleEntityUpdates);
        Packets.Add((int)ServerPacketType.SpawnPlayer, HandleSpawnPlayer);
        Packets.Add((int)ServerPacketType.PlayerChatMessage, HandlePlayerMessage);
        Packets.Add((int)ServerPacketType.PlayerUpdate, HandlePlayerUpdate);
        Packets.Add((int)ServerPacketType.RemovePlayer, HandleRemovePlayer);
    }

    //Receives a network packet sent from the game server and passes it on to be handled by the correct handler function
    public void HandlePacket(byte[] PacketData)
    {
        //Read the packet data to find out what type of packet this is
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        //Ignore empty packets
        if (PacketType == 0)
            return;
        //Otherwise, pass the packet onto the registered handler function for this packet type
        Packet NewPacket;
        if (Packets.TryGetValue(PacketType, out NewPacket))
            NewPacket.Invoke(PacketData);
    }

    //Sends a packet to the current game server connection
    private void SendPacket(PacketWriter Writer)
    {
        ServerConnection ServerConnection = ServerConnection.Instance;
        //Make sure the connection to the game server is still open
        if (!ServerConnection.IsConnected || !ServerConnection.ClientStream.CanRead)
            return;
        //Otherwise, send the packet as normal
        ServerConnection.ClientStream.Write(Writer.ToArray(), 0, Writer.ToArray().Length);
    }

    //Sends a players chat message to the server so it can be delivered to all the other clients chat windows
    public void SendPlayerMessage(string Message)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerChatMessage);
        Writer.WriteString(PlayerInfo.CharacterName);
        Writer.WriteString(Message);
        SendPacket(Writer);
    }
    //Receives another players chat message from the server to be displayed in our chat window
    private void HandlePlayerMessage(byte[] PacketData)
    {
        //Extract the message information from the packet
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        string MessageSender = Reader.ReadString();
        string MessageContents = Reader.ReadString();
        //Display the message to the UI chat window
        ChatWindow.Instance.DisplayPlayerMessage(MessageSender, MessageContents);
    }

    //Sends a new request to the game server that we want to register a new user account
    public void SendRegisterRequest(string Username, string Password)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.AccountRegistrationRequest);
        Writer.WriteString(Username);
        Writer.WriteString(Password);
        SendPacket(Writer);
    }
    //Receives a reply from the game server if our account registration request was accepted or not
    private void HandleRegisterReply(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        bool RegisterSuccess = Reader.ReadInt() == 1;
        string ReplyMessage = Reader.ReadString();
        ChatWindow.Instance.DisplayReplyMessage(RegisterSuccess, ReplyMessage);
        if (RegisterSuccess)
            MenuStateManager.GetMenuComponents("Account Creation").GetComponent<AccountCreationButtonFunctions>().RegisterSuccess();
        else
            MenuStateManager.GetMenuComponents("Account Creation").GetComponent<AccountCreationButtonFunctions>().RegisterFail();
    }

    //Sends a new request to the game server that we want to log into one of the user accounts
    public void SendLoginRequest(string Username, string Password)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.AccountLoginRequest);
        Writer.WriteString(Username);
        Writer.WriteString(Password);
        SendPacket(Writer);
    }
    //Receives a reply from the game server if our account login request was accepted or not
    private void HandleLoginReply(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        bool LoginSuccess = Reader.ReadInt() == 1;
        string ReplyMessage = Reader.ReadString();
        ChatWindow.Instance.DisplayReplyMessage(LoginSuccess, ReplyMessage);
        if (LoginSuccess)
            MenuStateManager.GetMenuComponents("Account Login").GetComponent<AccountLoginButtonFunctions>().LoginSuccess();
        else
            MenuStateManager.GetMenuComponents("Account Login").GetComponent<AccountLoginButtonFunctions>().LoginFail();
    }

    //Sends a new request to the game server that we want to create a new player character under our user account
    public void SendCreateCharacterRequest(string AccountName, string CharacterName, bool IsMale)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.CharacterCreationRequest);
        Writer.WriteString(AccountName);
        Writer.WriteString(CharacterName);
        Writer.WriteInt(IsMale ? 1 : 0);
        SendPacket(Writer);
    }
    //Receives a reply from the game server if our character creation request was accepted or not
    private void HandleCreateCharacterReply(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        bool CreationSuccess = Reader.ReadInt() == 1;
        string ReplyMessage = Reader.ReadString();
        ChatWindow.Instance.DisplayReplyMessage(CreationSuccess, ReplyMessage);
        if(CreationSuccess)
            MenuStateManager.GetMenuComponents("Character Creation").GetComponent<CharacterCreationButtonFunctions>().CreateCharacterSuccess();
        else
            MenuStateManager.GetMenuComponents("Character Creation").GetComponent<CharacterCreationButtonFunctions>().CreateCharacterFail();
    }

    //Sends a new request to the game server that we want to enter into the game world with our selected player character
    public void SendEnterWorldRequest(CharacterData CharacterData)
    {
        l.og("sending enter world request");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.EnterWorldRequest);
        Writer.WriteString(CharacterData.Account);
        Writer.WriteString(CharacterData.Name);
        Writer.WriteVector3(CharacterData.Position);
        SendPacket(Writer);
    }
    //Once a new enter world request has been sent to the server, the first thing we will receive back from them is a list of all the other active players
    private void HandleActivePlayerList(byte[] PacketData)
    {
        l.og("handle active player list");
        //Read in how many players there are in the server already
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        int ActivePlayerCount = Reader.ReadInt();
        for(int i = 0; i < ActivePlayerCount; i++)
        {
            //Extract each player characters information as we loop through the packet data
            string PlayerName = Reader.ReadString();
            Vector3 PlayerPosition = Reader.ReadVector3();
            //Spawn each new character into the game world and set their name to be displayed above their heads so we know who they are
            GameObject PlayerSpawn = Instantiate(PlayerPrefabs.Instance.ExternalPlayer, PlayerPosition, Quaternion.identity);
            PlayerSpawn.name = PlayerName;
            PlayerSpawn.GetComponentInChildren<TextMesh>().text = PlayerName;
            //Store all the other player characters in a list
            ServerConnection.Instance.AddOtherPlayer(PlayerName, PlayerSpawn);
        }

        //Once the player list has been loaded, the next stop of entering into the game world is to request a list of all the currently active entities
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.ActiveEntityRequest);
        SendPacket(Writer);
    }
    //2nd step of the enter world request, the server will send us a list of all the entities currently active in the game world
    private void HandleActiveEntityList(byte[] PacketData)
    {
        l.og("handle active entity list");
        //Read in how many entities there are in the server already
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        int EntityCount = Reader.ReadInt();
        for(int i = 0; i < EntityCount; i++)
        {
            //Extract each entities information as we loop through the packet data
            string EntityType = Reader.ReadString();
            string EntityID = Reader.ReadString();
            Vector3 EntityPosition = Reader.ReadVector3();
            //Spawn each new entity into the game world and store them in the entity manager
            GameObject EntitySpawn = Instantiate(EntityPrefabs.GetEntityPrefab(EntityType), EntityPosition, Quaternion.identity);
            EntitySpawn.GetComponent<ServerEntity>().ID = EntityID;
            EntityManager.AddNewEntity(EntityID, EntitySpawn);
        }

        l.og("entering world");
        //This was the final step to be taken before we are able to enter into the game world, so lets do that now
        CharacterData CharacterData = MenuStateManager.GetMenuComponents("Character Selection").GetComponent<CharacterSelectionButtonFunctions>().SelectedCharacter;
        GameObject CharacterSpawn = Instantiate(PlayerPrefabs.Instance.ClientPlayer, CharacterData.Position, Quaternion.identity);
        GameObject.Find("Main Camera").SetActive(false);
        MenuStateManager.SetMenuState("UI");
        MenuStateManager.GetMenuComponents("UI").GetComponent<MenuComponentObjects>().GetComponentObject("InputField").GetComponentInChildren<Text>().text = "[Press Enter to chat]";
        CharacterSpawn.name = CharacterData.Name;
        PlayerInfo.AccountName = CharacterData.Account;
        PlayerInfo.CharacterName = CharacterData.Name;
        PlayerInfo.PlayerObject = CharacterSpawn;
        PlayerInfo.GameState = GameStates.PlayingGame;

        //Finally, instruct the server that we have successfully entered into the game world
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.NewPlayerReady);
        SendPacket(Writer);
    }

    //Sends a new request to the game server that we want all the data regarding all the characters existing under our user account
    public void SendCharacterDataRequest(string Username)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.CharacterDataRequest);
        Writer.WriteString(Username);
        SendPacket(Writer);
    }
    //Receives data from the game server listing all the characters existing under our account and their information
    private void HandleCharacterData(byte[] PacketData)
    {
        //Read in how many characters exist under our user account
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        int CharacterCount = Reader.ReadInt();
        //If 0 characters exist, the user is sent straight to the character creation screen
        if(CharacterCount == 0)
        {
            MenuStateManager.GetMenuComponents("Character Selection").GetComponent<CharacterSelectionButtonFunctions>().NoCharactersCreated();
            return;
        }
        //Otherwise we loop through each character and load in each characters data as we loop through them all
        CharacterSelectionButtonFunctions CharacterSelect = MenuStateManager.GetMenuComponents("Character Selection").GetComponent<CharacterSelectionButtonFunctions>();
        for(int i = 0; i < CharacterCount; i++)
        {
            //Extract the information for each character as we loop through them all in the packet data
            CharacterData CharacterData = new CharacterData();
            CharacterData.Account = Reader.ReadString();
            CharacterData.Position = Reader.ReadVector3();
            CharacterData.Name = Reader.ReadString();
            CharacterData.Experience = Reader.ReadInt();
            CharacterData.ExperienceToLevel = Reader.ReadInt();
            CharacterData.Level = Reader.ReadInt();
            CharacterData.IsMale = Reader.ReadInt() == 1;
            //Save each set of character data into the character select screen
            CharacterSelect.SaveCharacterData(i + 1, CharacterData);
            CharacterSelect.SetSelectedCharacter(i + 1);
        }
        //Update the character select screen once all information has been loaded
        CharacterSelect.CharacterCount = CharacterCount;
        CharacterSelect.CharacterDataLoaded();
    }

    //Sends a new packet to the server with our current player characters updated world position and rotation values to be shared with all the other players
    public void SendPlayerUpdate(Vector3 Position, Quaternion Rotation)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerUpdate);
        Writer.WriteString(PlayerInfo.CharacterName);
        Writer.WriteVector3(Position);
        Writer.WriteQuaternion(Rotation);
        SendPacket(Writer);
    }
    //Recieves from the server one of the other player characters updated postion/rotation values to be updated on our end
    private void HandlePlayerUpdate(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        //Read in the characters new values
        string CharacterName = Reader.ReadString();
        Vector3 CharacterPosition = Reader.ReadVector3();
        Quaternion CharacterRotation = Reader.ReadQuaternion();
        //Client doesnt need to update its own character
        if (CharacterName == PlayerInfo.CharacterName)
            return;
        //Otherwise, update the player accordingly
        ServerConnection.Instance.GetOtherPlayer(CharacterName).GetComponent<ExternalPlayerMovement>().UpdateTargetValues(CharacterPosition, CharacterRotation);
    }

    //Sends a message to the server letting it know we are disconnecting from the game now
    public void SendDisconnectNotice()
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.DisconnectionNotice);
        SendPacket(Writer);
    }

    //Recieves from the server updated values for all the currently active entities in the game world
    private void HandleEntityUpdates(byte[] PacketData)
    {
        //Read in the amount of entity updates which need to be processed and loop through each of them
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        int EntityCount = Reader.ReadInt();
        for(int i = 0; i < EntityCount; i++)
        {
            //Extract the information for each entity as we loop through them all
            string EntityID = Reader.ReadString();
            Vector3 EntityPosition = Reader.ReadVector3();
            Quaternion EntityRotation = Reader.ReadQuaternion();
            //Send each entities updated values to the entity manager to be handled
            EntityManager.UpdateEntity(EntityID, EntityPosition, EntityRotation);
        }
    }

    //Receives instructions from the server to spawn in another player character which has just entered into the game world
    private void HandleSpawnPlayer(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        //Read in this characters information
        string CharacterName = Reader.ReadString();
        Vector3 CharacterPosition = Reader.ReadVector3();
        //Spawn this new player into the game world, set their name display and add them to the list of players
        GameObject PlayerSpawn = Instantiate(PlayerPrefabs.Instance.ExternalPlayer, CharacterPosition, Quaternion.identity);
        PlayerSpawn.GetComponentInChildren<TextMesh>().text = CharacterName;
        PlayerSpawn.name = CharacterName;
        ServerConnection.Instance.AddOtherPlayer(CharacterName, PlayerSpawn);
    }

    //Recieves instructions from the server to remove another player character who has disconnected from the game world
    private void HandleRemovePlayer(byte[] PacketData)
    {
        //Read in the players information
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        string CharacterName = Reader.ReadString();
        ServerConnection.Instance.RemoveOtherPlayer(CharacterName);
    }
}
