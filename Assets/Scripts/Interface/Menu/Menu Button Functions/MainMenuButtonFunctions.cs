// ================================================================================================================================
// File:        MainMenuButtonFunctions.cs
// Description: Button function events for anything in the main menu
// Author:      Harley Laurie          
// Notes:       
// ================================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtonFunctions : MonoBehaviour
{
    public void ClickRegisterAccountButton()
    {
        MenuStateManager.SetMenuState("Account Creation");
    }

    public void ClickLoginAccountButton()
    {
        MenuStateManager.SetMenuState("Account Login");
    }

    public void ClickQuitGameButton()
    {
        ServerConnection.Instance.CloseConnection();
        Application.Quit();
    }
}
