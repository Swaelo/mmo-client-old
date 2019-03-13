// ================================================================================================================================
// File:        UIButtonFunctions.cs
// Description: Button function events for gameplay UI
// ================================================================================================================================

using UnityEngine;

public class UIButtonFunctions : MonoBehaviour
{
    public void ClickQuitGameButton()
    {
        PacketManager.Instance.SendDisconnectNotice();
        Application.Quit();
    }

    public void ClickChangeCharacterButton()
    {

    }

    public void ClickLogoutAccountButton()
    {

    }
}
