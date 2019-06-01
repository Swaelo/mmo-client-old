using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageDisplayWindow : MonoBehaviour
{
    public Text[] MessageDisplayLines;  //Assign these in the inspector
    private string[] MessageLineContents;

    private void Awake()
    {
        //Create an array to store the message in each line, match the size to the number of lines that were assigned in the inspector
        MessageLineContents = new string[MessageDisplayLines.Length];
        //Set default values for each line
        for(int i = 0; i < MessageDisplayLines.Length; i++)
        {
            MessageLineContents[i] = "";
            MessageDisplayLines[i].text = "";
        }
    }

    //Moves all the existing messages back 1 line, then places a new message at the front
    public void AddNewMessage(string NewMessage)
    {
        for(int i = MessageDisplayLines.Length - 1; i > 0; i--)
        {
            MessageLineContents[i] = MessageLineContents[i - 1];
            MessageDisplayLines[i].text = MessageLineContents[i];
        }

        MessageLineContents[0] = NewMessage;
        MessageDisplayLines[0].text = NewMessage;
    }
}
