// ================================================================================================================================
// File:        CharacterData.cs
// Description: Stores all the information for a single character in the game world
// Author:      Harley Laurie          
// Notes:       
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public string Account;
    public Vector3 Position;
    public string Name;
    public int Experience;
    public int ExperienceToLevel;
    public int Level;
    public bool IsMale;
}
