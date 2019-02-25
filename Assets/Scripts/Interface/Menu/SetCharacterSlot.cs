// ================================================================================================================================
// File:        
// Description: 
// Author:      Harley Laurie          
// Notes:       
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetCharacterSlot : MonoBehaviour
{
    [SerializeField] private Text CharacterName;
    [SerializeField] private Text CharacterGender;
    [SerializeField] private Text CharacterLevel;
    [SerializeField] private Text CharacterExperience;
    [SerializeField] private Text CharacterExperienceToLevel;

    public void SetData(CharacterData Data)
    {
        CharacterName.text = Data.Name;
        CharacterGender.text = Data.IsMale ? "Male" : "Female";
        CharacterLevel.text = "Level " + Data.Level;
        CharacterExperience.text = Data.Experience + " experience";
        CharacterExperienceToLevel.text = Data.ExperienceToLevel + " xp to level";
    }
}
