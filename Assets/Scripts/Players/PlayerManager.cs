// ================================================================================================================================
// File:        PlayerManager.cs
// Description: Keeps track of all player characters currently active in the game world
// ================================================================================================================================

using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //This is a singleton class
    public static PlayerManager Instance = null;
    private void Awake() { Instance = this; }

    //The player character the current user is playing with right now
    public PlayerData LocalPlayer = new PlayerData();
    //All the other players currently active in the game
    public Dictionary<string, PlayerData> OtherPlayers = new Dictionary<string, PlayerData>();

    //Spawns a remote player into the game world
    public void AddOtherPlayer(string PlayerName, PlayerData PlayerObject)
    {
        OtherPlayers.Add(PlayerName, PlayerObject);
    }

    //Returns the name of the character the player is currently active in the game world with
    public string GetCurrentPlayerName()
    {
        return LocalPlayer.CurrentCharacter.CharacterName;
    }

    //Returns the currently active characters game object
    public GameObject GetCurrentCharacterObject()
    {
        return LocalPlayer.CurrentCharacter.CharacterObject;
    }

    //Returns the world position of the character the player is currently active in the game world with
    public Vector3 GetCurrentCharacterLocation()
    {
        return LocalPlayer.CurrentCharacter.CharacterObject.transform.position;
    }

    //Removes a remote player from the game world
    public void RemoveOtherPlayer(string PlayerName)
    {
        //Remove the other player from the list, then destroy them
        PlayerData OtherPlayerObject = OtherPlayers[PlayerName];
        OtherPlayers.Remove(PlayerName);
        GameObject.Destroy(OtherPlayerObject.CurrentCharacter.CharacterObject);
    }

    //Returns one of the players by their name
    public PlayerData GetOtherPlayer(string PlayerName)
    {
        return OtherPlayers[PlayerName];
    }

    //Spawns the local player into the game world
    public void SpawnLocalPlayer()
    {
        //Get the prefab for local player character
        GameObject PlayerPrefab = PlayerPrefabs.Instance.ClientPlayer;
        //Instantiate this player into the game world and store it as the active local player, assign its name
        GameObject NewLocalPlayer = Instantiate(PlayerPrefab, LocalPlayer.CurrentCharacter.CharacterPosition, Quaternion.identity);
        NewLocalPlayer.transform.name = LocalPlayer.CurrentCharacter.CharacterName;
        LocalPlayer.CurrentCharacter.CharacterObject = NewLocalPlayer;
    }
}
