// ================================================================================================================================
// File:        CharacterCreationButtonFunctions.cs
// Description: Button function events for anything in the character create screen
// Author:      Harley Laurie          
// Notes:       
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreationButtonFunctions : MonoBehaviour
{
    private MenuComponentObjects Components;
    private void Awake() { Components = GetComponent<MenuComponentObjects>(); }

    public void ClickCreateCharacterButton()
    {
        //Get the character info from the user interface
        PlayerInfo.CharacterName = Components.GetComponentObject("Create Character Name Input").GetComponent<InputField>().text;
        bool IsMale = Components.GetComponentObject("Create Character Gender Switch").GetComponent<Slider>().value == 0;
        //Ignore the request if no character name has been entered
        if(PlayerInfo.CharacterName == "")
        {
            ChatWindow.Instance.DisplayErrorMessage("Please enter a name for your character");
            return;
        }
        //Send a request to the server to create this character
        PacketSender.Instance.SendCreateCharacterRequest(PlayerInfo.AccountName, PlayerInfo.CharacterName, IsMale);
        //Display the waiting animation
        Components.ToggleAllBut("Waiting Animation", true);
    }

    public void ClickExitCharacterCreationButton()
    {
        //return to the character select screen
        MenuStateManager.SetMenuState("Character Selection");
        //Display the waiting animation
        MenuStateManager.GetCurrentMenuStateObject().GetComponent<MenuComponentObjects>().ToggleAllBut("Waiting Animation", true);
        //Send a request to the server for all of our character information
        PacketSender.Instance.SendGetCharacterDataRequest(PlayerInfo.AccountName);
    }

    public void CreateCharacterSuccess()
    {
        //Change to the character selection screen
        MenuStateManager.SetMenuState("Character Selection");
        //Display the waiting animation
        MenuStateManager.GetCurrentMenuStateObject().GetComponent<MenuComponentObjects>().ToggleAllBut("Waiting Animation", true);
        //Send a request to the server for all of our character information
        PacketSender.Instance.SendGetCharacterDataRequest(PlayerInfo.AccountName);
    }

    public void CreateCharacterFail()
    {
        //Disable the waiting animation, returning to the character creation screen
        Components.ToggleAllBut("Waiting Animation", false);
    }
}
