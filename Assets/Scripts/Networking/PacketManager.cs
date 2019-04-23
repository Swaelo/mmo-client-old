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
    ActiveItemRequest = 7,
    NewPlayerReady = 8,
    PlayerChatMessage = 9,
    PlayerUpdate = 10,
    PlayerAttack = 11,
    DisconnectionNotice = 12,
    ConnectionCheckReply = 13,

    PlayerInventoryRequest = 14,
    PlayerEquipmentRequest = 15,
    PlayerTakeItemRequest = 16,
    RemoveInventoryItem = 17,
    EquipInventoryItem = 18,
    UnequipItem = 19
}
public enum ServerPacketType
{
    AccountRegistrationReply = 1,
    AccountLoginReply = 2,
    CharacterCreationReply = 3,
    CharacterDataReply = 4,

    ActivePlayerList = 5,
    ActiveEntityList = 6,
    ActiveItemList = 7,
    SpawnItem = 8,
    RemoveItem = 9,

    EntityUpdates = 10,
    RemoveEntities = 11,

    PlayerChatMessage = 12,
    PlayerUpdate = 13,
    SpawnPlayer = 14,
    RemovePlayer = 15,

    PlayerInventoryItems = 16,
    PlayerEquipmentItems = 17,
    PlayerInventoryUpdate = 18
}

public class PacketManager : MonoBehaviour
{
    //Singleton instance of the packet manager
    public static PacketManager Instance = null;

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
        Packets.Add((int)ServerPacketType.ActiveItemList, HandleActiveItemList);
        Packets.Add((int)ServerPacketType.SpawnItem, HandleSpawnItem);
        Packets.Add((int)ServerPacketType.RemoveItem, HandleRemoveItem);

        Packets.Add((int)ServerPacketType.EntityUpdates, HandleEntityUpdates);
        Packets.Add((int)ServerPacketType.RemoveEntities, HandleRemoveEntities);

        Packets.Add((int)ServerPacketType.PlayerChatMessage, HandlePlayerMessage);
        Packets.Add((int)ServerPacketType.PlayerUpdate, HandlePlayerUpdate);
        Packets.Add((int)ServerPacketType.SpawnPlayer, HandleSpawnPlayer);
        Packets.Add((int)ServerPacketType.RemovePlayer, HandleRemovePlayer);

        Packets.Add((int)ServerPacketType.PlayerInventoryItems, HandlePlayerInventory);
        Packets.Add((int)ServerPacketType.PlayerEquipmentItems, HandlePlayerEquipment);
        Packets.Add((int)ServerPacketType.PlayerInventoryUpdate, HandlePlayerInventoryUpdate);

    }

    //Tells the server to remove an item from a players inventory
    public void SendRemoveInventoryItem(string PlayerName, int BagSlot)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.RemoveInventoryItem);
        Writer.WriteString(PlayerName);
        Writer.WriteInt(BagSlot);
        SendPacket(Writer);
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
        //Make sure the connection to the game server is still open
        if (!ConnectionManager.Instance.IsConnected || !ConnectionManager.Instance.ClientStream.CanRead)
            return;
        //Otherwise, send the packet as normal
        ConnectionManager.Instance.ClientStream.Write(Writer.ToArray(), 0, Writer.ToArray().Length);
    }

    //Sends a players chat message to the server so it can be delivered to all the other clients chat windows
    public void SendPlayerMessage(string Message)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerChatMessage);
        Writer.WriteString(PlayerManager.Instance.LocalPlayer.CurrentCharacter.CharacterName);
        Writer.WriteString(Message);
        SendPacket(Writer);
    }

    //Sends a request to the game server for a list of all items in a players inventory
    public void SendPlayerInventoryRequest()
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerInventoryRequest);
        Writer.WriteString(PlayerManager.Instance.GetCurrentPlayerName());
        SendPacket(Writer);
    }

    //Recieves a list of all the items equipped to the players characters
    private void HandlePlayerEquipment(byte[] PacketData)
    {
        //Read in what item is equipped into each of the players equipment slots
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        int HeadItem = Reader.ReadInt();
        int BackItem = Reader.ReadInt();
        int NeckItem = Reader.ReadInt();
        int ChestItem = Reader.ReadInt();
        int LeftShoulderItem = Reader.ReadInt();
        int RightShoulderItem = Reader.ReadInt();
        int LeftGloveItem = Reader.ReadInt();
        int RightGloveItem = Reader.ReadInt();
        int LegItem = Reader.ReadInt();
        int LeftHandItem = Reader.ReadInt();
        int RightHandItem = Reader.ReadInt();
        int LeftFootItem = Reader.ReadInt();
        int RightFootItem = Reader.ReadInt();

        //Find the PlayerEquipmentControl class and use it to add all these items to the players equipment screen
        ItemList Items = ItemList.Instance;
        PlayerEquipmentControl Equipment = PlayerEquipmentControl.Instance;
        Equipment.GetEquipmentPanel(EquipmentSlot.Head).GetComponent<EquipSlot>().UpdateItem(Items.GetItem(HeadItem));
        Equipment.GetEquipmentPanel(EquipmentSlot.Back).GetComponent<EquipSlot>().UpdateItem(Items.GetItem(BackItem));
        Equipment.GetEquipmentPanel(EquipmentSlot.Neck).GetComponent<EquipSlot>().UpdateItem(Items.GetItem(NeckItem));
        Equipment.GetEquipmentPanel(EquipmentSlot.Chest).GetComponent<EquipSlot>().UpdateItem(Items.GetItem(ChestItem));
        Equipment.GetEquipmentPanel(EquipmentSlot.LeftShoulder).GetComponent<EquipSlot>().UpdateItem(Items.GetItem(LeftShoulderItem));
        Equipment.GetEquipmentPanel(EquipmentSlot.RightShoulder).GetComponent<EquipSlot>().UpdateItem(Items.GetItem(RightShoulderItem));
        Equipment.GetEquipmentPanel(EquipmentSlot.LeftGlove).GetComponent<EquipSlot>().UpdateItem(Items.GetItem(LeftGloveItem));
        Equipment.GetEquipmentPanel(EquipmentSlot.RightGlove).GetComponent<EquipSlot>().UpdateItem(Items.GetItem(RightGloveItem));
        Equipment.GetEquipmentPanel(EquipmentSlot.Legs).GetComponent<EquipSlot>().UpdateItem(Items.GetItem(LegItem));
        Equipment.GetEquipmentPanel(EquipmentSlot.LeftHand).GetComponent<EquipSlot>().UpdateItem(Items.GetItem(LeftHandItem));
        Equipment.GetEquipmentPanel(EquipmentSlot.RightHand).GetComponent<EquipSlot>().UpdateItem(Items.GetItem(RightHandItem));
        Equipment.GetEquipmentPanel(EquipmentSlot.LeftFoot).GetComponent<EquipSlot>().UpdateItem(Items.GetItem(LeftFootItem));
        Equipment.GetEquipmentPanel(EquipmentSlot.RightFoot).GetComponent<EquipSlot>().UpdateItem(Items.GetItem(RightFootItem));

        //Now everything is done loading we can let the server know the player has entered into the game world without error
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.NewPlayerReady);
        SendPacket(Writer);
    }

    //Recieves a list of all the items in the players inventory
    private void HandlePlayerInventory(byte[] PacketData)
    {
        //Read the ID number of each item stored in the players inventory
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();

        //Update each slot in the players inventory
        GameObject[] InventoryPanels = PlayerInventoryControl.Instance.InventorySlotPanels;
        for (int i = 0; i < 9; i++)
        {
            //Get the ID number of this item
            int ItemID = Reader.ReadInt();
            //Get the slot used to store this item
            InventorySlot BagSlot = InventoryPanels[i].GetComponent<InventorySlot>();

            //Update the slot to display what it contains, item ID 0 means the slot is empty
            if (ItemID == 0)
                BagSlot.SetEmpty();
            else
                BagSlot.UpdateItem(ItemList.Instance.GetItem(ItemID));
        }

        //Now request to know what equipment this character is wearing
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerEquipmentRequest);
        Writer.WriteString(PlayerManager.Instance.GetCurrentPlayerName());
        SendPacket(Writer);
    }

    private void HandlePlayerInventoryUpdate(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        GameObject[] InventoryPanels = PlayerInventoryControl.Instance.InventorySlotPanels;
        for(int i = 0; i < 9; i++)
        {
            int ItemID = Reader.ReadInt();
            InventorySlot BagSlot = InventoryPanels[i].GetComponent<InventorySlot>();
            if (ItemID == 0)
                BagSlot.SetEmpty();
            else
                BagSlot.UpdateItem(ItemList.Instance.GetItem(ItemID));
        }
    }

    //Sends a request to the game server that the player wants to unequip on of their items and move it to their inventory
    public void SendUnequipItemRequest(string PlayerName, int ItemNumber, EquipmentSlot EquipSlot)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.UnequipItem);
        Writer.WriteString(PlayerName);
        Writer.WriteInt(ItemNumber);
        Writer.WriteInt((int)EquipSlot);
        SendPacket(Writer);
    }

    //Sends a request to the game server that the player wnats to equip one of the items from their inventory
    public void SendEquipItemRequest(string PlayerName, int BagSlot, int ItemNumber, EquipmentSlot EquipSlot)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.EquipInventoryItem);
        Writer.WriteString(PlayerName);
        Writer.WriteInt(BagSlot);
        Writer.WriteInt(ItemNumber);
        Writer.WriteInt((int)EquipSlot);
        SendPacket(Writer);
    }

    //Sends a request to the game server that the player wants to pick up an item from the ground
    public void SendTakeItemRequest(string PlayerName, int ItemNumber, int ItemID)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerTakeItemRequest);
        Writer.WriteString(PlayerName);
        Writer.WriteInt(ItemNumber);
        Writer.WriteInt(ItemID);
        SendPacket(Writer);
    }

    private void HandleSpawnItem(byte[] PacketData)
    {
        //Read in the packet data to find out what item is being spawned into the game world
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        string ItemType = Reader.ReadString();
        string ItemName = Reader.ReadString();
        int ItemID = Reader.ReadInt();
        Vector3 ItemPos = Reader.ReadVector3();

        //Sort by item type what item is being added into the game world
        switch (ItemType)
        {
            //First check for consumable items
            case ("Consumable"):
                //Use the ItemManager to spawn the correct consumable pickup prefab into the game world
                ItemManager.Instance.AddConsumable(ItemID, ItemName, ItemPos);
                break;
            case ("Equipment"):
                ItemManager.Instance.AddEquipment(ItemID, ItemName, ItemPos);
                break;
        }
    }

    private void HandleRemoveItem(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        int ItemID = Reader.ReadInt();
        ItemManager.Instance.RemoveItem(ItemID);
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
        l.og(MessageSender + MessageContents);
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
        l.og(RegisterSuccess.ToString() + ReplyMessage);
        if (RegisterSuccess)
        {
            //If the account was registered send a request to log into it
            l.og("New Account Registered, logging in now...");
            PacketManager.Instance.SendLoginRequest(PlayerManager.Instance.LocalPlayer.AccountName, PlayerManager.Instance.LocalPlayer.AccountPass);
        }
        else
        {
            //Otherwise, return back to the account registration window
            l.og("Account registration failed: " + ReplyMessage);
            MenuPanelDisplayManager.Instance.DisplayPanel("Account Creation Panel");
        }
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
        if(LoginSuccess)
        {
            //Move to character selection screen when logging in sucessfully
            MenuPanelDisplayManager.Instance.DisplayPanel("Character Selection Panel");
            //Request all of our character data from the server
            PacketManager.Instance.SendCharacterDataRequest(PlayerManager.Instance.LocalPlayer.AccountName);
        }
        else
        {
            //Return to the account login page if the request was denied
            MenuPanelDisplayManager.Instance.DisplayPanel("Account Login Panel");
            l.og("Account Login Failed: " + ReplyMessage);
        }
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
        if (CreationSuccess)
        {
            l.og("New character created!");
            //When a new character has been created, request all character data and display the waiting animation until its received, then go to the character select screen
            MenuPanelDisplayManager.Instance.DisplayPanel("Waiting Panel");
            PacketManager.Instance.SendCharacterDataRequest(PlayerManager.Instance.LocalPlayer.AccountName);
        }
        else
        {
            //Return to the character creation screen
            l.og("Character creation failed: " + ReplyMessage);
            MenuPanelDisplayManager.Instance.DisplayPanel("Character Creation Panel");
        }
    }

    //Sends a new request to the game server that we want to enter into the game world with our selected player character
    public void SendEnterWorldRequest(CharacterData CharacterData)
    {
        l.og("enter world request");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.EnterWorldRequest);
        Writer.WriteString(CharacterData.AccountOwner);
        Writer.WriteString(CharacterData.CharacterName);
        Writer.WriteVector3(CharacterData.CharacterPosition);
        SendPacket(Writer);
    }
    //Once a new enter world request has been sent to the server, the first thing we will receive back from them is a list of all the other active players
    private void HandleActivePlayerList(byte[] PacketData)
    {
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

            //Store all the other player characters data in a new object, these objects in a list
            PlayerData Data = new PlayerData();
            Data.CurrentCharacter = new CharacterData();
            Data.CurrentCharacter.CharacterName = PlayerName;
            Data.CurrentCharacter.CharacterPosition = PlayerPosition;
            Data.CurrentCharacter.CharacterObject = PlayerSpawn;
            PlayerManager.Instance.OtherPlayers.Add(PlayerName, Data);
        }

        //Once the player list has been loaded, the next step of entering into the game world is to request a list of all the currently active entities
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.ActiveEntityRequest);
        SendPacket(Writer);
    }
    //2nd step of the enter world request, the server will send us a list of all the entities currently active in the game world
    private void HandleActiveEntityList(byte[] PacketData)
    {
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
            int EntityHealth = Reader.ReadInt();
            //Spawn each new entity into the game world and store them in the entity manager
            GameObject EntitySpawn = Instantiate(EntityPrefabs.GetEntityPrefab(EntityType), EntityPosition, Quaternion.identity);
            ServerEntity EntityDetails = EntitySpawn.GetComponent<ServerEntity>();
            EntityDetails.ID = EntityID;
            EntityDetails.Health = EntityHealth;
            EntityDetails.MaxHealth = EntityHealth;
            EntitySpawn.GetComponent<ServerEntity>().ID = EntityID;
            EntitySpawn.GetComponentInChildren<TextMesh>().text = EntityType + " " + EntityDetails.Health + "/" + EntityDetails.MaxHealth;
            EntityManager.AddNewEntity(EntityID, EntitySpawn);
        }

        //Now request from the server the active item list
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.ActiveItemRequest);
        SendPacket(Writer);
    }
    private void HandleActiveItemList(byte[] PacketData)
    {
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        int ItemCount = Reader.ReadInt();
        
        for(int i = 0; i < ItemCount; i++)
        {
            //Extract the items information from the network packet
            string ItemName = Reader.ReadString();
            string ItemType = Reader.ReadString();
            int ItemID = Reader.ReadInt();
            Vector3 ItemPosition = Reader.ReadVector3();

            //Sort by item type what item is being added into the game world
            switch (ItemType)
            {
                //First check for consumable items
                case ("Consumable"):
                    //Use the ItemManager to spawn the correct consumable pickup prefab into the game world
                    ItemManager.Instance.AddConsumable(ItemID, ItemName, ItemPosition);
                    break;
                case ("Equipment"):
                    ItemManager.Instance.AddEquipment(ItemID, ItemName, ItemPosition);
                    break;
            }
        }
        
        //Now use the player manager to spawn the players character into the game world
        PlayerManager.Instance.SpawnLocalPlayer();
        //Disable the main scene camera
        GameObject.Find("Main Camera").SetActive(false);
        //Hide all UI components, then display only the chat window and inventory panel
        UIManager UI = UIManager.Instance;
        UI.HideAllPanels();
        UI.TogglePanelDisplay("Chat Panel", true);
        UI.TogglePanelDisplay("Inventory Panel", true);
        UI.TogglePanelDisplay("Equipment Panel", true);
        

        //Request from the server information regarding what the player has in their inventory
        SendPlayerInventoryRequest();
    }

    //Sends a new request to the game server that we want all the data regarding all the characters existing under our user account
    public void SendCharacterDataRequest(string Username)
    {
        l.og("request character data");
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.CharacterDataRequest);
        Writer.WriteString(Username);
        SendPacket(Writer);
    }
    //Receives data from the game server listing all the characters existing under our account and their information
    private void HandleCharacterData(byte[] PacketData)
    {
        //First find out how many different characters information has been received, loop through them 1 at a time to extract each character information
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        int CharacterCount = Reader.ReadInt();
        
        //If 0 characters data was sent through, go straight to the character creation screen
        if(CharacterCount == 0)
        {
            MenuPanelDisplayManager.Instance.DisplayPanel("Character Creation Panel");
            return;
        }
        //Otherwise go to the character selection screen
        else
        {
            MenuPanelDisplayManager.Instance.DisplayPanel("Character Selection Panel");

            //Loop through and extract each of the characters information
            for (int i = 0; i < CharacterCount; i++)
            {
                //Extract this characters information from the network packet and store it into the new CharacterData structure
                CharacterData Data = new CharacterData();
                Data.AccountOwner = Reader.ReadString();    //The account this character belongs to
                Data.CharacterName = Reader.ReadString();   //This characters ingame name
                Data.CharacterPosition = Reader.ReadVector3();  //This characters position in the game world
                Data.CharacterLevel = Reader.ReadInt(); //This characters current level
                Data.CharacterGender = (Reader.ReadInt() == 1 ? Gender.Male : Gender.Female);
                //Store this data structure in the local players list of characters
                PlayerManager.Instance.LocalPlayer.PlayerCharacters[i] = Data;
            }
        }
        //Now update and display the character selection screen
        CharacterSelectionButtonFunctions.Instance.CharacterCount = CharacterCount;
        CharacterSelectionButtonFunctions.Instance.CharacterDataLoaded();
    }

    //Sends a new packet to the server with our current player characters updated world position and rotation values to be shared with all the other players
    public void SendPlayerUpdate(Vector3 Position, Quaternion Rotation)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerUpdate);
        Writer.WriteString(PlayerManager.Instance.LocalPlayer.CurrentCharacter.CharacterName);
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
        if (CharacterName == PlayerManager.Instance.LocalPlayer.CurrentCharacter.CharacterName)
            return;
        //Otherwise, update the player accordingly
        PlayerManager.Instance.GetOtherPlayer(CharacterName).CurrentCharacter.CharacterObject.GetComponent<ExternalPlayerMovement>().UpdateTargetValues(CharacterPosition, CharacterRotation);
    }

    //Sends a new packet to the server with the position where the players attack landed so collision detection and damage calculations can be done server side
    public void SendPlayerAttack(Vector3 Position, Vector3 Scale, Quaternion Rotation)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerAttack);
        Writer.WriteVector3(Position);
        Writer.WriteVector3(Scale);
        Writer.WriteQuaternion(Rotation);
        SendPacket(Writer);
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
            int EntityHealth = Reader.ReadInt();
            //Send each entities updated values to the entity manager to be handled
            EntityManager.UpdateEntity(EntityID, EntityPosition, EntityRotation, EntityHealth);
        }
    }

    //Recives message from the server to remove one of the entities from the game world
    private void HandleRemoveEntities(byte[] PacketData)
    {
        l.og("Handle Remove Entities");
        //Find out how many entities need to be removed
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        int EntityCount = Reader.ReadInt();
        //Loop through and remove them all
        for(int i = 0; i < EntityCount; i++)
        {
            string EntityID = Reader.ReadString();
            EntityManager.RemoveEntity(EntityID);
        }
    }

    //Receives instructions from the server to spawn in another player character which has just entered into the game world
    private void HandleSpawnPlayer(byte[] PacketData)
    {
        l.og("Handle Spawn Player");
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        //Read in this characters information
        string CharacterName = Reader.ReadString();
        Vector3 CharacterPosition = Reader.ReadVector3();
        //Spawn this new player into the game world, set their name display and add them to the list of players
        GameObject PlayerSpawn = Instantiate(PlayerPrefabs.Instance.ExternalPlayer, CharacterPosition, Quaternion.identity);
        PlayerSpawn.GetComponentInChildren<TextMesh>().text = CharacterName;
        PlayerSpawn.name = CharacterName;
        PlayerData NewPlayerData = new PlayerData();
        NewPlayerData.CurrentCharacter.CharacterName = CharacterName;
        NewPlayerData.CurrentCharacter.CharacterObject = PlayerSpawn;
        PlayerManager.Instance.AddOtherPlayer(CharacterName, NewPlayerData);
    }

    //Recieves instructions from the server to remove another player character who has disconnected from the game world
    private void HandleRemovePlayer(byte[] PacketData)
    {
        l.og("Handle Remove Player");
        //Read in the players information
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        string CharacterName = Reader.ReadString();
        PlayerManager.Instance.RemoveOtherPlayer(CharacterName);
    }
}
