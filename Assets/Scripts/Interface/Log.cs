using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Log
{
    //Displays a message to the chat window
    public static void PrintChatMessage(string Message)
    {
        ChatWindowManager.Instance.DisplayMessage(Message);
    }

    //Display a message to the incoming packets display window
    public static void PrintIncomingPacket(string Message)
    {
        IncomingPacketsDisplay.Instance.DisplayIncomingPacket(Message);
    }

    //Display a message to the outgoing packets display window
    public static void PrintOutgoingPacket(string Message)
    {
        OutgoingPacketsDisplay.Instance.DisplayOutgoingPacket(Message);
    }
}
