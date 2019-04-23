// ================================================================================================================================
// File:        SetCharacterSlot.cs
// Description: Takes in all of a characters data and stores that to be displayed in one of the character slots in the select screen
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class SetCharacterSlot : MonoBehaviour
{
    public Text CharacterName;
    public Text CharacterRaceGender;
    public Text CharacterLevelClass;

    public void UpdateData(CharacterData NewData)
    {
        CharacterName.text = NewData.CharacterName;
        CharacterRaceGender.text = "Human " + (NewData.CharacterGender == Gender.Male ? "Male" : "Female");
        CharacterLevelClass.text = NewData.CharacterLevel.ToString() + " " + " Wanderer";
    }
}
