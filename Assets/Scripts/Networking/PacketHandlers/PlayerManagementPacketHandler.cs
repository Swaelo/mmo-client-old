using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class PlayerManagementPacketHandler
{
    public static void HandlePlayerUpdate(PacketReader Reader)
    {
        Log.PrintIncomingPacket("PlayerManagement.HandlePlayerUpdate");
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

    public static void HandleSpawnPlayer(PacketReader Reader)
    {
        Log.PrintIncomingPacket("PlayerManagement.HandleSpawnOtherPlayer");
        //Read in this characters information
        string CharacterName = Reader.ReadString();
        Vector3 CharacterPosition = Reader.ReadVector3();
        //Spawn this new player into the game world, set their name display and add them to the list of players
        GameObject PlayerSpawn = ItemSpawner.Instance.SpawnItem(PlayerPrefabs.Instance.ExternalPlayer, CharacterPosition, Quaternion.identity);
        PlayerSpawn.GetComponentInChildren<TextMesh>().text = CharacterName;
        PlayerSpawn.name = CharacterName;
        PlayerData NewPlayerData = new PlayerData();
        NewPlayerData.CurrentCharacter.CharacterName = CharacterName;
        NewPlayerData.CurrentCharacter.CharacterObject = PlayerSpawn;
        PlayerManager.Instance.AddOtherPlayer(CharacterName, NewPlayerData);
    }

    public static void HandleRemovePlayer(PacketReader Reader)
    {
        Log.PrintIncomingPacket("PlayerManagement.HandleRemoveOtherPlayer");
        string CharacterName = Reader.ReadString();
        PlayerManager.Instance.RemoveOtherPlayer(CharacterName);
    }
}