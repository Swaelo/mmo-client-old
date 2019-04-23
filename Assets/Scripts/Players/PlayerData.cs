// ================================================================================================================================
// File:        PlayerData.cs
// Description: Keeps track of all of a player characters data and the account data the character belongs to
// ================================================================================================================================

using UnityEngine;

public class PlayerData
{
    public string AccountName = ""; //This players account name
    public string AccountPass = ""; //This players account password
    public CharacterData CurrentCharacter;  //The players character that is currently being used
    public CharacterData[] PlayerCharacters = new CharacterData[3]; //A list of all the players characters

    //Stores a characters data into the player character list
    public void SaveCharacterData(CharacterData Data, int Slot)
    {
        PlayerCharacters[Slot - 1] = Data;
    }
}