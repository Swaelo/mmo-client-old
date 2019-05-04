// ================================================================================================================================
// File:        PacketManager.cs
// Description: Defines all the functions used for sending and recieving data through the network to the game server
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

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
    PlayerActionBarRequest = 16,
    PlayerTakeItemRequest = 17,
    RemoveInventoryItem = 18,
    EquipInventoryItem = 19,
    UnequipItem = 20,

    PlayerMoveInventoryItem = 21,
    PlayerSwapInventoryItems = 22,
    PlayerSwapEquipmentItem = 23,
    PlayerDropItem = 24,
    PlayerEquipAbility = 25,
    PlayerSwapEquipAbility = 26,
    PlayerUnequipAbility = 27,
    PlayerSwapAbilities = 28,
    PlayerMoveAbility = 29,
    PlayerDropAbility = 30,

    PlayerPurgeItems = 31
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
    PlayerActionBarAbilities = 18,
    PlayerTotalItemUpdate = 19
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
        Packets.Add((int)ServerPacketType.PlayerActionBarAbilities, HandlePlayerActionBar);
        Packets.Add((int)ServerPacketType.PlayerTotalItemUpdate, HandlePlayerTotalItemUpdate);
    }

    //Tell the server this character wants to have every single item, piece of equipment and ability bar object removed from their character
    public void SendPlayerPurgeItems(string CharacterName)
    {
        //Start a new network packet
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerPurgeItems);
        //Write character name and send the packet to the server
        Writer.WriteString(CharacterName);
        SendPacket(Writer);
    }
    
    //Tell the server the player wants to drop one of the items from their inventory
    //Inside item drop client packets, the 2nd int value represents the source where the item is being dropped from 1 = Inventory, 2 = Equipment, 3 = ActionBar
    public void SendDropInventoryItem(string PlayerName, int BagSlotNumber, Vector3 DropLocation)
    {
        //Create packet and write the packet type
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerDropItem);
        //Write in the location where the item is being dropped from
        Writer.WriteInt(1); //1 = dropped from inventory
        //Now write the players name and the bag slot number of the item that is being dropped, and where they are dropping it
        Writer.WriteString(PlayerName);
        Writer.WriteInt(BagSlotNumber);
        Writer.WriteVector3(DropLocation);
        //Send the packet to the game server
        SendPacket(Writer);
    }
    //Tell the server the player wants to drop one of the items from their equipment screen
    public void SendDropEquipmentItem(string PlayerName, EquipmentSlot EquipmentSlot, Vector3 DropLocation)
    {
        //Create packet and write the packet type
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerDropItem);
        //Write in the location where the item is being dropped from
        Writer.WriteInt(2); //2 = dropped from equipment
        //write the rest of the data and send the packet to the server
        Writer.WriteString(PlayerName);
        Writer.WriteInt((int)EquipmentSlot);
        Writer.WriteVector3(DropLocation);
        //Send the packet to the server
        SendPacket(Writer);
    }
    //Tell the server the players wants to drop one of the ability gems from their action bar onto the ground
    public void SendDropActionBarItem(string PlayerName, int ActionBarSlotNumber, Vector3 DropLocation)
    {
        //Create packet and write the packet type
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerDropItem);
        //Write in the location where the item is being dropped from
        Writer.WriteInt(3); //3 = dropped from action bar
        //write the rest of the data and send it
        Writer.WriteString(PlayerName);
        Writer.WriteInt(ActionBarSlotNumber);
        Writer.WriteVector3(DropLocation);
        //Send the packet to the game server
        SendPacket(Writer);
    }

    //Tell the server the players wants to move the position of one of the items in their inventory
    public void SendMoveInventoryItem(string PlayerName, int OriginalBagSlot, int DestinationBagSlot)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerMoveInventoryItem);
        Writer.WriteString(PlayerName);
        Writer.WriteInt(OriginalBagSlot);
        Writer.WriteInt(DestinationBagSlot);
        SendPacket(Writer);
    }

    //Tell the server the player wants to swap the positions of two of the items in their inventory
    public void SendSwapInventoryItem(string PlayerName, int FirstBagSlot, int SecondBagSlot)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerSwapInventoryItems);
        Writer.WriteString(PlayerName);
        Writer.WriteInt(FirstBagSlot);
        Writer.WriteInt(SecondBagSlot);
        SendPacket(Writer);
    }

    //Tell the server the player wants to swap the piece of equipment with one in their inventory
    public void SendPlayerSwapEquipmentItem(string PlayerName, int ItemBagSlot, EquipmentSlot ItemEquipmentSlot)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerSwapEquipmentItem); //Packet type
        Writer.WriteString(PlayerName); //Who is swapping items around their inventory/equipment
        Writer.WriteInt(ItemBagSlot);   //What bag slot stores the item to newly be equipped, where the previously equipped item will now be stored
        Writer.WriteInt((int)ItemEquipmentSlot);    //The type of equipment slot being dealt with
        SendPacket(Writer);
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

    //Tells the server to equip an ability gem from the characters inventory
    public void SendCharacterEquipAbility(string CharacterName, int AbilityGemBagSlot, int ActionBarEquipSlot)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerEquipAbility);
        Writer.WriteString(CharacterName);
        Writer.WriteInt(AbilityGemBagSlot);
        Writer.WriteInt(ActionBarEquipSlot);
        SendPacket(Writer);
    }

    //Tells the server to unequip an ability into the characters inventory
    public void SendCharacterUnequipAbility(string CharacterName, int BagSlot, int ActionBarSlot)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerUnequipAbility);
        Writer.WriteString(CharacterName);
        Writer.WriteInt(BagSlot);
        Writer.WriteInt(ActionBarSlot);
        SendPacket(Writer);
    }

    //Tells the server to swap the positions of two of the characters currently equipped ability gems
    public void SendCharacterSwapAbilities(string CharacterName, int FirstActionBarSlot, int SecondActionBarSlot)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerSwapAbilities);
        Writer.WriteString(CharacterName);
        Writer.WriteInt(FirstActionBarSlot);
        Writer.WriteInt(SecondActionBarSlot);
        SendPacket(Writer);
    }

    //Tells the server to move an ability on the action bar to one of the other slots
    public void SendCharacterMoveAbility(string CharacterName, int ActionBarSlot, int DestinationActionBarSlot)
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerMoveAbility);

        Writer.WriteString(CharacterName);
        Writer.WriteInt(ActionBarSlot);
        Writer.WriteInt(DestinationActionBarSlot);

        SendPacket(Writer);
    }

    //Tells the server to swap one of the equipped ability gems with another from the characters inventory
    public void SendCharacterSwapEquipAbility(string CharacterName, int BagSlot, int ActionBarSlot)
    {
        //Define a new packet to send
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerSwapEquipAbility);

        //Fill it with the right data
        Writer.WriteString(CharacterName);
        Writer.WriteInt(BagSlot);
        Writer.WriteInt(ActionBarSlot);

        //Send it to the game server
        SendPacket(Writer);
    }

    //Requests from a server up to date information on all entities currently active in the game world
    public void SendActiveEntityRequest()
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.ActiveEntityRequest);
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

    //Read and update the contents of the players inventory
    private void HandlePlayerInventory(byte[] PacketData)
    {
        //Open a new network packet
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();

        //Grab all the inventory slot UI components from the scene
        DraggableUIComponent[] ItemSlots = DraggableUIControl.Instance.InventorySlots;
        //Loop through reading the data for each item in the characters inventory, updating the item slots where nessacery
        for (int i = 0; i < 9; i++)
        {
            //Read each slots values
            int ItemNumber = Reader.ReadInt();
            int ItemID = Reader.ReadInt();

            //Empty the slots with nothing in them
            if (ItemNumber == 0)
            {
                ItemSlots[i].ItemData = null;
                ItemSlots[i].UpdateUIDisplay();
            }
            //Display the slots that arent empty
            else
            {
                ItemSlots[i].ItemData = ItemList.Instance.GetItemData(ItemNumber);
                ItemSlots[i].EquipSlotType = ItemSlots[i].ItemData.Slot;
                ItemSlots[i].ItemID = ItemID;
                ItemSlots[i].UpdateUIDisplay();
            }
        }

        //Inventory updated, now request the characters equipment information
        SendPlayerEquipmentRequest();
    }

    //Sends a request to the game server to send us our characters current equipment screen status
    private void SendPlayerEquipmentRequest()
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerEquipmentRequest);
        Writer.WriteString(PlayerManager.Instance.GetCurrentPlayerName());
        SendPacket(Writer);
    }

    //Recieves a list of all the items equipped to the players characters
    private void HandlePlayerEquipment(byte[] PacketData)
    {
        //Open a new network packet
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();

        //Loop through and read the info about any items in the players equipment slots
        for(int i = 0; i < 13; i++)
        {
            //Read the initial item values from the packet data
            int ItemNumber = Reader.ReadInt();
            int ItemID = Reader.ReadInt();

            //Skip past the empty equipment slots
            if (ItemNumber == 0)
                continue;

            //Get the rest of the items data from the master item list
            ItemData ItemData = ItemList.Instance.GetItemData(ItemNumber);

            //Update the UI and character model to show the piece of gear being worn here
            DraggableUIComponent GearSlot = DraggableUIControl.Instance.GetEquipmentSlot(ItemData.Slot);
            GearSlot.ItemData = ItemData;
            GearSlot.ItemID = ItemID;
            GearSlot.UpdateUIDisplay();
            PlayerManager.Instance.GetCurrentCharacterObject().GetComponent<PlayerItemEquip>().EquipItem(ItemData.Slot, ItemData.Name);
        }

        //Characters equipment has been handled, now ask for the action bar status
        SendPlayerActionBarRequest();
    }

    //Receives a list of all the abilities equipped onto the characters action bar
    private void HandlePlayerActionBar(byte[] PacketData)
    {
        //Open a new network packet
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();

        //Grab all the action bar slot UI components from the scene
        DraggableUIComponent[] ActionBarSlots = DraggableUIControl.Instance.ActionBarSlots;
        //Loop through reading in each action bar items data, updating the slots as needed
        for(int i = 0; i < 5; i++)
        {
            //Read each slots values
            int ItemNumber = Reader.ReadInt();
            int ItemID = Reader.ReadInt();

            //Empty the action bar slots with nothing within them
            if(ItemNumber == 0)
            {
                ActionBarSlots[i].ItemData = null;
                ActionBarSlots[i].UpdateUIDisplay();
            }
            else
            //Display action bar slots with abilities within them
            {
                ActionBarSlots[i].ItemData = ItemList.Instance.GetItemData(ItemNumber);
                ActionBarSlots[i].ItemID = ItemID;
                ActionBarSlots[i].UpdateUIDisplay();
            }
        }

        //Now everything is ready and set up correctly, tell the server we have finished loading into the game successfully
        SendNewPlayerReadyNotification();
    }

    //Gets new state of the entire set of a players inventory, equipment and action bar
    private void HandlePlayerTotalItemUpdate(byte[] PacketData)
    {
        //Open a new network packet
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();

        //Read and update the contents of the characters inventory
        DraggableUIComponent[] ItemSlots = DraggableUIControl.Instance.InventorySlots;
        for(int i = 0; i < 9; i++)
        {
            //Read the values of the item in each inventory slot
            int ItemNumber = Reader.ReadInt();
            int ItemID = Reader.ReadInt();

            //Empty the slots with nothing in them
            if(ItemNumber == 0)
            {
                ItemSlots[i].ItemData = null;
                ItemSlots[i].UpdateUIDisplay();
            }
            else
            //Display the slots with items in them
            {
                ItemSlots[i].ItemData = ItemList.Instance.GetItemData(ItemNumber);
                ItemSlots[i].EquipSlotType = ItemSlots[i].ItemData.Slot;
                ItemSlots[i].ItemID = ItemID;
                ItemSlots[i].UpdateUIDisplay();
            }
        }

        //Loop through the characters equipment slots, updating the UI / character model as we go along
        for(int i = 0; i < 13; i++)
        {
            //Read the initial item values from the packet data
            EquipmentSlot GearSlot = (EquipmentSlot)Reader.ReadInt();
            int ItemNumber = Reader.ReadInt();
            int ItemID = Reader.ReadInt();

            //Empty UI slots / remove equipment from the player with empty equipment slots
            if(ItemNumber == 0)
            {
                //Update the UI
                DraggableUIComponent EquipmentSlot = DraggableUIControl.Instance.GetEquipmentSlot(GearSlot);
                EquipmentSlot.ItemData = null;
                EquipmentSlot.UpdateUIDisplay();
                //Remove gear from the character
                PlayerManager.Instance.GetCurrentCharacterObject().GetComponent<PlayerItemEquip>().UnequipItem(GearSlot);
            }
            //Update UI slots / add equipment to the player with filled equipment slots
            else
            {
                //Get the rest of the items data
                ItemData ItemData = ItemList.Instance.GetItemData(ItemNumber);
                //Update the UI
                DraggableUIComponent EquipmentSlot = DraggableUIControl.Instance.GetEquipmentSlot(GearSlot);
                EquipmentSlot.ItemData = ItemData;
                EquipmentSlot.ItemID = ItemID;
                EquipmentSlot.UpdateUIDisplay();
                //Equip the gear onto the character model
                PlayerManager.Instance.GetCurrentCharacterObject().GetComponent<PlayerItemEquip>().EquipItem(GearSlot, ItemData.Name);
            }
        }

        //Read and update the contents of the characters action bar
        DraggableUIComponent[] AbilitySlots = DraggableUIControl.Instance.ActionBarSlots;
        for(int i = 0; i < 5; i++)
        {
            //read each items values
            int ItemNumber = Reader.ReadInt();
            int ItemID = Reader.ReadInt();

            //Empty slots
            if(ItemNumber == 0)
            {
                AbilitySlots[i].ItemData = null;
                AbilitySlots[i].UpdateUIDisplay();
            }
            //Filled Slots
            else
            {
                AbilitySlots[i].ItemData = ItemList.Instance.GetItemData(ItemNumber);
                AbilitySlots[i].ItemID = ItemID;
                AbilitySlots[i].UpdateUIDisplay();
            }
        }
    }

    private void SendPlayerActionBarRequest()
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerActionBarRequest);
        Writer.WriteString(PlayerManager.Instance.GetCurrentPlayerName());
        SendPacket(Writer);
    }

    private void SendNewPlayerReadyNotification()
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.NewPlayerReady);
        SendPacket(Writer);
    }

    //Sends a request to the game server that the player wants to unequip whatever item is in this equipment slot, then move it into their first available inventory spot
    //NOTE: Assumes the player has some available space in their inventory
    public void SendUnequipItemRequest(string CharacterName, EquipmentSlot EquipmentSlot, int BagSlot)
    {
        //Start the packet writer
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.UnequipItem);

        //Write in who is unequipping the item, what slot they are unequipping from, and what inventory slot the item wants to be placed into (default -1 value places the item into the first available inventory slot)
        Writer.WriteString(CharacterName);
        Writer.WriteInt((int)EquipmentSlot);
        Writer.WriteInt(BagSlot);

        //Send the packet to the game server so the request can be processed
        SendPacket(Writer);
    }

    //Sends a request to the game server that the players wants to equip one of the items from the inventory
    public void SendEquipItemRequest(string CharacterName, int BagSlot, EquipmentSlot GearSlot)
    {
        //Define the new packet to send
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.EquipInventoryItem);

        //Write the characters name, the bag slot number where the item is and the equipment slot type where they are going to equip the item to
        Writer.WriteString(CharacterName);
        Writer.WriteInt(BagSlot);
        Writer.WriteInt((int)GearSlot);

        //Send the request to the game server
        SendPacket(Writer);
    }

    //Sends a request to the game server that the player wants to pick up an item from the ground
    public void SendTakeItemRequest(string CharacterName, ItemData ItemData, int ItemID)
    {
        //Create the new packet to send to the server
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.PlayerTakeItemRequest);

        //Write the characters name and the items data into the network packet
        Writer.WriteString(CharacterName);
        Writer.WriteInt(ItemData.ItemNumber);
        Writer.WriteInt(ItemID);

        //Send the packet to the game server
        SendPacket(Writer);
    }

    //Spawns a new item pickup into the game world
    private void HandleSpawnItem(byte[] PacketData)
    {
        //Open the network packet
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();

        //Read in the items information values
        int ItemNumber = Reader.ReadInt();
        int ItemID = Reader.ReadInt();
        Vector3 ItemLocation = Reader.ReadVector3();

        //Use the ItemManager class to add this object into the game world as a new pickup object
        ItemManager.Instance.AddItemPickup(ItemNumber, ItemID, ItemLocation);
    }

    //Removes an existing item pickup from the game world
    private void HandleRemoveItem(byte[] PacketData)
    {
        //Open the network packet
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();

        //Get the universal ItemID of the item thats being removed, then use that to tell the ItemManager to remove it from the game world
        int ItemID = Reader.ReadInt();
        ItemManager.Instance.RemoveItemPickup(ItemID);
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
        if (LoginSuccess)
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
        for (int i = 0; i < ActivePlayerCount; i++)
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
        SendActiveEntityRequest();
    }
    //2nd step of the enter world request, the server will send us a list of all the entities currently active in the game world
    private void HandleActiveEntityList(byte[] PacketData)
    {
        //Read in how many entities there are in the server already
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();
        int EntityCount = Reader.ReadInt();
        for (int i = 0; i < EntityCount; i++)
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
        SendActiveItemRequest();
    }

    //Sends a request to the game server to recieve a complete list of all the currently active item drops in the game world
    private void SendActiveItemRequest()
    {
        PacketWriter Writer = new PacketWriter();
        Writer.WriteInt((int)ClientPacketType.ActiveItemRequest);
        SendPacket(Writer);
    }

    //Receives from the server the initial set of active item drops when first entering into the game world
    private void HandleActiveItemList(byte[] PacketData)
    {
        //Open the network packet
        PacketReader Reader = new PacketReader(PacketData);
        int PacketType = Reader.ReadInt();

        //Read the amount of item pickups that need exist, then loop through and do each one of them
        int ItemPickups = Reader.ReadInt();
        l.og(ItemPickups + " item pickups to handle.");
        for(int i = 0; i < ItemPickups; i++)
        {
            //Extract each items values from the packet
            int ItemNumber = Reader.ReadInt();
            int ItemID = Reader.ReadInt();
            Vector3 ItemLocation = Reader.ReadVector3();

            //Spawn each item into the game world as a new item pickup object
            ItemManager.Instance.AddItemPickup(ItemNumber, ItemID, ItemLocation);
        }

        //Spawn our player character into the game world, change cameras and update the UI settings
        PlayerManager.Instance.SpawnLocalPlayer();
        GameObject.Find("Main Camera").SetActive(false);
        UIManager.Instance.HideAllPanels();
        UIManager.Instance.TogglePanelDisplay("Chat Panel", true);
        UIManager.Instance.TogglePanelDisplay("Inventory Panel", true);
        UIManager.Instance.TogglePanelDisplay("Equipment Panel", true);
        UIManager.Instance.TogglePanelDisplay("Action Bar Panel", true);

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
        if (CharacterCount == 0)
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
        for (int i = 0; i < EntityCount; i++)
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
        for (int i = 0; i < EntityCount; i++)
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
