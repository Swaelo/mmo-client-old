// ================================================================================================================================
// File:        SetCharacterSlot.cs
// Description: Takes in all of a characters data and stores that to be displayed in one of the character slots in the select screen
// ================================================================================================================================

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
