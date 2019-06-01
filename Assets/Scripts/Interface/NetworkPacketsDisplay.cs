using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkPacketsDisplay : MonoBehaviour
{
    public static NetworkPacketsDisplay Instance = null;
    public Text[] DisplayLines;
    private string[] MessageContents;

    private void Awake()
    {
        Instance = this;
        MessageContents = new string[DisplayLines.Length];
        for(int i = 0; i < DisplayLines.Length; i++)
        {
            MessageContents[i] = "";
            DisplayLines[i].text = MessageContents[i];
        }
    }

    private void DisplayNewMessage(string PacketMessage)
    {
        //Move all the previous messages back 1 line in the message box
        for(int i = DisplayLines.Length - 1; i > 0; i--)
        {
            MessageContents[i] = MessageContents[i - 1];
            DisplayLines[i].text = MessageContents[i];
        }

        //Display the new message at the front
        MessageContents[0] = PacketMessage;
        DisplayLines[0].text = MessageContents[0];
    }

    public void DisplayIncomingPacket(string PacketMessage)
    {
        DisplayNewMessage("IN: " + PacketMessage);
    }

    public void DisplayOutgoingPacket(string PacketMessage)
    {
        DisplayNewMessage("OUT: " + PacketMessage);
    }
}
