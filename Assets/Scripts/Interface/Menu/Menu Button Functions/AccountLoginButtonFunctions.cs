// ================================================================================================================================
// File:        AccountLoginButtonFunctions.cs
// Description: Button function events for anything in the account login screen
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class AccountLoginButtonFunctions : MonoBehaviour
{
    private MenuComponentObjects Components;
    private void Awake() { Components = GetComponent<MenuComponentObjects>(); }

    public void ClickLoginAccountButton()
    {
        //Extract the account credentials entered into the user interface
        PlayerInfo.AccountName = Components.GetComponentObject("Login Username Input").GetComponent<InputField>().text;
        PlayerInfo.AccountPass = Components.GetComponentObject("Login Password Input").GetComponent<InputField>().text;
        //Ignore this request if any of the fields were left empty
        if(PlayerInfo.AccountName == "" || PlayerInfo.AccountPass == "")
        {
            ChatWindow.Instance.DisplayErrorMessage("Please fill account login form correctly.");
            return;
        }
        //Send a request to the server to login to this account
        PacketManager.Instance.SendLoginRequest(PlayerInfo.AccountName, PlayerInfo.AccountPass);
        //Display the waiting animation
        Components.ToggleAllBut("Waiting Animation", true);
    }

    public void ClickExitAccountLoginButton()
    {
        //Change back to the main menu
        MenuStateManager.SetMenuState("Main Menu");
    }

    public void LoginSuccess()
    {
        //Disable the waiting animation
        Components.ToggleAllBut("Waiting Animation", false);
        //Move onto the character select screen
        MenuStateManager.SetMenuState("Character Selection");
        //Display the loading animation
        MenuStateManager.GetCurrentMenuStateObject().GetComponent<MenuComponentObjects>().ToggleAllBut("Waiting Animation", true);
        //Request all of our character data from the server
        PacketManager.Instance.SendCharacterDataRequest(PlayerInfo.AccountName);
    }

    public void LoginFail()
    {
        //Disable the waiting animation returning back to the account login menu
        Components.ToggleAllBut("Waiting Animation", false);
    }
}
