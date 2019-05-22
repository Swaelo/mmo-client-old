using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class GameWorldStatePacketHandler
{
    public static void HandleActivePlayerList(PacketReader Reader)
    {
        Log.PrintIncomingPacket("GameWorldState.HandleActivePlayerList");
        int ActivePlayerCount = Reader.ReadInt();
        for (int i = 0; i < ActivePlayerCount; i++)
        {
            //Extract each player characters information as we loop through the packet data
            string PlayerName = Reader.ReadString();
            Vector3 PlayerPosition = Reader.ReadVector3();
            //Spawn each new character into the game world and set their name to be displayed above their heads so we know who they are
            GameObject PlayerSpawn = ItemSpawner.Instance.SpawnItem(PlayerPrefabs.Instance.ExternalPlayer, PlayerPosition, Quaternion.identity);
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
        //GameWorldStatePacketSender.SendActiveEntityRequest();
    }

    public static void HandleActiveEntityList(PacketReader Reader)
    {
        Log.PrintIncomingPacket("GameWorldState.HandleActiveEntityList");
        int EntityCount = Reader.ReadInt();
        for (int i = 0; i < EntityCount; i++)
        {
            //Extract each entities information as we loop through the packet data
            string EntityType = Reader.ReadString();
            string EntityID = Reader.ReadString();
            Vector3 EntityPosition = Reader.ReadVector3();
            int EntityHealth = Reader.ReadInt();
            //Spawn each new entity into the game world and store them in the entity manager
            GameObject EntitySpawn = ItemSpawner.Instance.SpawnItem(EntityPrefabs.GetEntityPrefab(EntityType), EntityPosition, Quaternion.identity);
            ServerEntity EntityDetails = EntitySpawn.GetComponent<ServerEntity>();
            EntityDetails.ID = EntityID;
            EntityDetails.Health = EntityHealth;
            EntityDetails.MaxHealth = EntityHealth;
            EntitySpawn.GetComponent<ServerEntity>().ID = EntityID;
            EntitySpawn.GetComponentInChildren<TextMesh>().text = EntityType + " " + EntityDetails.Health + "/" + EntityDetails.MaxHealth;
            EntityManager.AddNewEntity(EntityID, EntitySpawn);
        }

        //Now request from the server the active item list
        //GameWorldStatePacketSender.SendActiveItemRequest();
    }

    public static void HandleActiveItemList(PacketReader Reader)
    {
        Log.PrintIncomingPacket("GameWorldState.HandleActiveItemList");
        //Read the amount of item pickups that need exist, then loop through and do each one of them
        int ItemPickups = Reader.ReadInt();
        //Log.Print(ItemPickups + " item pickups to handle.");
        for (int i = 0; i < ItemPickups; i++)
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
        //InventoryEquipmentManagementPacketSender.SendPlayerInventoryRequest();
    }
}