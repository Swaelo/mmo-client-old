using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncomingPacketsDisplay : MonoBehaviour
{
    public static IncomingPacketsDisplay Instance = null;   //Singleton class instance
    public Text[] DisplayLines;   //Set of UI Text objects used to display the contents of each message line to the UI
    private string[] MessageContents;  //The string message contents of each line in the window

    private void Awake()
    {
        //Assign the singleton instance
        Instance = this;

        //Create the new array used to store the string contents of each message currently being displayed in the window
        MessageContents = new string[DisplayLines.Length];

        //Give a default value for each line in the window
        for (int i = 0; i < DisplayLines.Length; i++)
        {
            MessageContents[i] = "";
            DisplayLines[i].text = MessageContents[i];
        }
    }

    //Moves all the previous messages back 1 line, then displays the new message in the first line
    public void DisplayIncomingPacket(string PacketMessage)
    {
        //Move all the previous messages back 1 line
        for(int i = DisplayLines.Length - 1; i > 0; i--)
        {
            MessageContents[i] = MessageContents[i - 1];
            DisplayLines[i].text = MessageContents[i];
        }

        //Display the new message in the first line
        MessageContents[0] = PacketMessage;
        DisplayLines[0].text = MessageContents[0];
    }
}
