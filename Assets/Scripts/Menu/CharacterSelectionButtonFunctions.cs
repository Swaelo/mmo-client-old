// ================================================================================================================================
// File:        CharacterSelectionButtonFunctions.cs
// Description: Button function events for anything in the character select screen
// ================================================================================================================================

using UnityEngine;

public class CharacterSelectionButtonFunctions : MonoBehaviour
{
    //This is a singleton class to the PacketManager can easily access it and tell it to update and display the menu once it loads in all the character information
    public static CharacterSelectionButtonFunctions Instance = null;
    private void Awake() { Instance = this; }

    //Data regarding each player character the user owns, and the UI elements used to display that information to them
    public GameObject FirstCharacterPanel;
    public GameObject SecondCharacterPanel;
    public GameObject ThirdCharacterPanel;

    //Button which sends the user to the character creation screen
    public GameObject CreateCharacterButton;

    //The amount of characters the player currently has to select from
    public int CharacterCount = 0;

    public void ClickSelectCharacter(int CharacterSlot)
    {
        //Keep note of which character was selected and attempt to enter into the game world with them
        PlayerData LocalPlayer = PlayerManager.Instance.LocalPlayer;
        LocalPlayer.CurrentCharacter = LocalPlayer.PlayerCharacters[CharacterSlot - 1];
        MenuPanelDisplayManager.Instance.DisplayPanel("Waiting Panel");
        PacketManager.Instance.SendEnterWorldRequest(LocalPlayer.CurrentCharacter);
    }

    public void ClickCreateCharacter()
    {
        //Proceed to character creation
        MenuPanelDisplayManager.Instance.DisplayPanel("Character Creation Panel");
    }

    public void ClickLogoutAccount()
    {
        //Return to the main menu
        MenuPanelDisplayManager.Instance.DisplayPanel("Main Menu Panel");
    }

    //Update the UI with all the new character data that has been loaded in, then display the menu to be seen by the user
    public void CharacterDataLoaded()
    {
        //Get the local player class where all the data regarding the characters they own is stored
        PlayerData LocalPlayer = PlayerManager.Instance.LocalPlayer;

        //Set up which panels should be displayed depending on how many characters data has been loaded in
        FirstCharacterPanel.SetActive(CharacterCount > 0);
        SecondCharacterPanel.SetActive(CharacterCount > 1);
        ThirdCharacterPanel.SetActive(CharacterCount > 2);

        //Update the information displayed in each character slot which is currently being displayed
        if (CharacterCount > 0)
            FirstCharacterPanel.GetComponent<SetCharacterSlot>().UpdateData(LocalPlayer.PlayerCharacters[0]);
        if (CharacterCount > 1)
            SecondCharacterPanel.GetComponent<SetCharacterSlot>().UpdateData(LocalPlayer.PlayerCharacters[1]);
        if (CharacterCount > 2)
            ThirdCharacterPanel.GetComponent<SetCharacterSlot>().UpdateData(LocalPlayer.PlayerCharacters[2]);

        //Hide the create new character button if they already have 3 characters created
        CreateCharacterButton.SetActive(CharacterCount < 3);
    }
}
