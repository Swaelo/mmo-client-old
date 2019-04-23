// ================================================================================================================================
// File:        CharacterData.cs
// Description: Stores all the information for a single character in the game world
// ================================================================================================================================

using UnityEngine;

public class CharacterData
{
    public string AccountOwner; //The user account this character is registered under
    public Vector3 CharacterPosition;   //World Position
    public Quaternion CharacterRotation;    //Rotation
    public string CharacterName;    //Characters ingame name
    public Gender CharacterGender;  //Gender
    public int CharacterXP; //Current XP
    public int CharacterXPToLevel;  //XP required to reach next level
    public int CharacterLevel;  //Current level
    public GameObject CharacterObject; //The game object
}
