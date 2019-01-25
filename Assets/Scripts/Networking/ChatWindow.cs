using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatWindow : MonoBehaviour
{
    public static ChatWindow Instance;
    void Awake() { Instance = this; }

    public Text[] ChatLines;
    public InputField Input;
    public string AccountName = "";

    public void ChatMessage()
    {
        //Get the message from the input field
        string Message = Input.text;
        //Empty the message box
        Input.text = "";
        //Display the message in the chat box
        ChatLines[5].text = ChatLines[4].text;
        ChatLines[4].text = ChatLines[3].text;
        ChatLines[3].text = ChatLines[2].text;
        ChatLines[2].text = ChatLines[1].text;
        ChatLines[1].text = ChatLines[0].text;
        ChatLines[0].text = AccountName + ": " + Message;
        //Send the chat message to the server for all other clients to receive
        PacketSender.instance.SendChatMessage(Message);
    }

    public void ExternalMessage(string Sender, string Message)
    {
        ChatLines[5].text = ChatLines[4].text;
        ChatLines[4].text = ChatLines[3].text;
        ChatLines[3].text = ChatLines[2].text;
        ChatLines[2].text = ChatLines[1].text;
        ChatLines[1].text = ChatLines[0].text;
        ChatLines[0].text = Sender + ": " + Message;
    }
}