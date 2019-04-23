// ================================================================================================================================
// File:        AccountLoginButtonFunctions.cs
// Description: Button function events for anything in the account login screen
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class AccountLoginButtonFunctions : MonoBehaviour
{
    public InputField UsernameInput;
    public InputField PasswordInput;

    public void ClickLoginAccount()
    {
        //Ignore the request if any of the input fields were left empty
        if(UsernameInput.text == "")
        {
            l.og("Account Login failed, no username entered.");
            return;
        }
        else if(PasswordInput.text == "")
        {
            l.og("Account Login failed, no password entered.");
            return;
        }

        //Send the account login request to the server
        PlayerManager.Instance.LocalPlayer.AccountName = UsernameInput.text;
        PlayerManager.Instance.LocalPlayer.AccountPass = PasswordInput.text;
        PacketManager.Instance.SendLoginRequest(PlayerManager.Instance.LocalPlayer.AccountName, PlayerManager.Instance.LocalPlayer.AccountPass);
        MenuPanelDisplayManager.Instance.DisplayPanel("Waiting Panel");
    }

    public void ClickCancelLogin()
    {
        MenuPanelDisplayManager.Instance.DisplayPanel("Main Menu Panel");
    }
}
