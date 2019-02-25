// ================================================================================================================================
// File:        PlayerInfo.cs
// Description: Tracks all the users information as they login to their account and enter the game
// Author:      Harley Laurie          
// Notes:       
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerInfo
{
    public static GameStates GameState = GameStates.MainMenu;
    public static string AccountName = "Not Logged In";
    public static string AccountPass = "";
    public static string CharacterName = "Not Logged In";
    public static GameObject PlayerObject = null;
}
