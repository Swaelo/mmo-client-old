// ================================================================================================================================
// File:        AccountCreationButtonFunctions.cs
// Description: Button function events for anything in the account create screen
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class AccountCreationButtonFunctions : MonoBehaviour
{
    public InputField UsernameInput;
    public InputField PasswordInput;
    public InputField PasswordVerification;

    public void ClickCreateAccount()
    {
        //Retrieve the strings from the 3 input fields used for account registration
        string AccountName = UsernameInput.text;
        string AccountPass = PasswordInput.text;
        string PassVerify = PasswordVerification.text;

        //Ignore the request if any of the input fields have been left empty
        if(AccountName == "")
        {
            Log.PrintChatMessage("Registration failed, no account name was entered.");
            return;
        }
        else if(AccountPass == "")
        {
            Log.PrintChatMessage("Registration failed, no password was entered.");
            return;
        }
        else if(PassVerify == "")
        {
            Log.PrintChatMessage("Registration failed, please verify your password.");
            return;
        }

        //Make sure the password and verification fields match
        if(AccountPass != PassVerify)
        {
            Log.PrintChatMessage("Registration failed, password verification did not match.");
            return;
        }

        //Send the account registration request to the game server
        AccountManagementPacketSender.SendRegisterRequest(AccountName, AccountPass);
        PlayerManager.Instance.LocalPlayer.AccountName = AccountName;
        PlayerManager.Instance.LocalPlayer.AccountPass = AccountPass;

        //Display the menu waiting animation until we getr a reply from the server regarding our account registration request
        MenuPanelDisplayManager.Instance.DisplayPanel("Waiting Panel");
    }

    public void ClickCancelCreate()
    {
        MenuPanelDisplayManager.Instance.DisplayPanel("Main Menu Panel");
    }
}
