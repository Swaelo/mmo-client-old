// ================================================================================================================================
// File:        AccountCreationButtonFunctions.cs
// Description: Button function events for anything in the account create screen
// Author:      Harley Laurie          
// Notes:       
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountCreationButtonFunctions : MonoBehaviour
{
    private MenuComponentObjects Components;
    private void Awake() { Components = GetComponent<MenuComponentObjects>(); }

    public void ClickCreateAccountButton()
    {
        //Get the account credentials the user has entered into the UI
        string AccountName = GetComponent<MenuComponentObjects>().GetComponentObject("Username Input").GetComponent<InputField>().text;
        string AccountPassword = GetComponent<MenuComponentObjects>().GetComponentObject("Password Input").GetComponent<InputField>().text;
        string AccountPasswordVerification = GetComponent<MenuComponentObjects>().GetComponentObject("Password Verification Input").GetComponent<InputField>().text;
        //Ignore the request if any of the input fields have been left empty
        if(AccountName == "" || AccountPassword == "" || AccountPasswordVerification == "")
        {
            ChatWindow.Instance.DisplayErrorMessage("Please fill the form correctly, something has been left empty.");
            return;
        }
        //Make sure both password fields match
        if (AccountPassword != AccountPasswordVerification)
        {
            ChatWindow.Instance.DisplayErrorMessage("Password verification does not match");
            return;
        }
        //Send a request to the server to register the new account
        PacketSenderLogic.Instance.SendRegisterRequest(AccountName, AccountPassword);
        //Save this account info into the player info class
        PlayerInfo.AccountName = AccountName;
        PlayerInfo.AccountPass = AccountPassword;
        //Display the waiting animation until we get a reply from the server
        GetComponent<MenuComponentObjects>().ToggleAllBut("Waiting Animation", true);
    }

    public void ClickCancelButton()
    {
        //Return back to the main menu
        MenuStateManager.SetMenuState("Main Menu");

    }

    public void RegisterSuccess()
    {
        //Disable the waiting animation
        GetComponent<MenuComponentObjects>().ToggleAllBut("Waiting Animation", false);
        //Change to the logging in menu
        MenuStateManager.SetMenuState("Account Login");
        //Display the login menus waiting animation
        MenuStateManager.GetCurrentMenuStateObject().GetComponent<MenuComponentObjects>().ToggleAllBut("Waiting Animation", true);
        //Send a request to the server to log into our new account
        PacketSenderLogic.Instance.SendLoginRequest(PlayerInfo.AccountName, PlayerInfo.AccountPass);
    }

    public void RegisterFail()
    {
        //Disable the waiting animation returning back to the account registration menu
        Components.ToggleAllBut("Waiting Animation", false);
    }
}
