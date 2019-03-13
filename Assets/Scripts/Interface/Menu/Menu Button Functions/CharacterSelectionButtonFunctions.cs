// ================================================================================================================================
// File:        CharacterSelectionButtonFunctions.cs
// Description: Button function events for anything in the character select screen
// ================================================================================================================================

using UnityEngine;

public class CharacterSelectionButtonFunctions : MonoBehaviour
{
    private MenuComponentObjects Components;
    private void Awake() { Components = GetComponent<MenuComponentObjects>(); }

    public CharacterData[] CharacterSlots;
    public GameObject[] CharacterSlotObjects;
    public CharacterData SelectedCharacter;
    public int CharacterCount = -1;

    public void ClickSelectCharacterButton(int CharacterSlot)
    {
        SelectedCharacter = CharacterSlots[CharacterSlot - 1];
        PlayerInfo.CharacterName = SelectedCharacter.Name;
        //Display the waiting animation
        Components.ToggleAllBut("Select Waiting Animation", true);
        //Send a request to the server to enter the game with this character
        PacketManager.Instance.SendEnterWorldRequest(SelectedCharacter);
    }

    public void ClickCreateCharacterButton()
    {
        //Change to the character creation menu
        MenuStateManager.SetMenuState("Character Creation");
    }

    public void ClickLogoutAccountButton()
    {
        //Change to the main menu
        MenuStateManager.SetMenuState("Main Menu");
    }

    public void NoCharactersCreated()
    {
        //Disable this waiting animation
        MenuStateManager.GetCurrentMenuStateObject().GetComponent<MenuComponentObjects>().ToggleAllBut("Waiting Animation", false);
        //Change to the character creation menu
        MenuStateManager.SetMenuState("Character Creation");
    }

    public void SaveCharacterData(int SaveSlot, CharacterData Data)
    {
        CharacterSlots[SaveSlot - 1] = Data;
    }

    public void SetSelectedCharacter(int SaveSlot)
    {
        SelectedCharacter = CharacterSlots[SaveSlot - 1];
    }

    public void CharacterDataLoaded()
    {
        GetComponent<MenuComponentObjects>().ViewAllButXAndY("Waiting Animation", "Select Waiting Animation");
        //First character slot
        if(CharacterCount >= 1)
        {
            CharacterSlotObjects[0].SetActive(true);
            CharacterSlotObjects[0].GetComponent<SetCharacterSlot>().SetData(CharacterSlots[0]);
        }
        //second character slot
        if (CharacterCount >= 2)
        {
            CharacterSlotObjects[1].SetActive(true);
            CharacterSlotObjects[1].GetComponent<SetCharacterSlot>().SetData(CharacterSlots[1]);
        }
        //third character slot
        if (CharacterCount >= 3)
        {
            CharacterSlotObjects[2].SetActive(true);
            CharacterSlotObjects[2].GetComponent<SetCharacterSlot>().SetData(CharacterSlots[2]);
        }
        //Only display the character creation button if the player has less than 3 character on their account
        GetComponent<MenuComponentObjects>().GetComponentObject("Create New Character Button").SetActive(CharacterCount < 3);
    }
}
