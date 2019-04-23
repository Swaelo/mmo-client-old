// ================================================================================================================================
// File:        MainMenuButtonFunctions.cs
// Description: Button function events for anything in the main menu
// ================================================================================================================================

using UnityEngine;

public class MainMenuButtonFunctions : MonoBehaviour
{
    public void ClickLoginAccount()
    {
        MenuPanelDisplayManager.Instance.DisplayPanel("Account Login Panel");
    }

    public void ClickRegisterAccount()
    {
        MenuPanelDisplayManager.Instance.DisplayPanel("Account Creation Panel");
    }

    public void ClickQuitGame()
    {
        ConnectionManager.Instance.CloseConnection("User is quitting the game.");
        Application.Quit();
    }
}
