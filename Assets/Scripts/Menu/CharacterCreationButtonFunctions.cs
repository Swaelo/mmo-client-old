// ================================================================================================================================
// File:        CharacterCreationButtonFunctions.cs
// Description: Button function events for anything in the character create screen
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class CharacterCreationButtonFunctions : MonoBehaviour
{
    public Text CharacterNameInput;
    public Slider CharacterGenderSlider;

    public void ClickCreateCharacter()
    {
        //Extract all the character information and store it in the local player variable in the player manager class
        PlayerData LocalPlayer = PlayerManager.Instance.LocalPlayer;
        CharacterData NewCharacter = new CharacterData();
        NewCharacter.CharacterName = CharacterNameInput.text;
        NewCharacter.CharacterGender = CharacterGenderSlider.value == 0 ? Gender.Male : Gender.Female;
        LocalPlayer.CurrentCharacter = NewCharacter;

        //Ignore the character creation request if no character name was entered
        if (LocalPlayer.CurrentCharacter.CharacterName == "")
        {
            l.og("Character creation failed: no name was entered.");
            return;
        }

        //Send a request to the server to have this character created and display the waiting information
        PacketManager.Instance.SendCreateCharacterRequest(LocalPlayer.AccountName, LocalPlayer.CurrentCharacter.CharacterName, LocalPlayer.CurrentCharacter.CharacterGender == Gender.Male);
        MenuPanelDisplayManager.Instance.DisplayPanel("Waiting Panel");
    }

    public void ClickCancelCreation()
    {
        //Return to the character selection screen when the cancel button is clicked
        MenuPanelDisplayManager.Instance.DisplayPanel("Waiting Panel");
        PacketManager.Instance.SendCharacterDataRequest(PlayerManager.Instance.LocalPlayer.AccountName);
    }
}