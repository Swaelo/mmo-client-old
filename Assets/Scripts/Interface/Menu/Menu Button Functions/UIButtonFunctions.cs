using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonFunctions : MonoBehaviour
{
    public void ClickQuitGameButton()
    {
        PacketSender.Instance.SendDisconnectNotice();
        Application.Quit();
    }

    public void ClickChangeCharacterButton()
    {

    }

    public void ClickLogoutAccountButton()
    {

    }
}
